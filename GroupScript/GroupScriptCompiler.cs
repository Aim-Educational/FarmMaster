using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace GroupScript
{
    public enum GroupScriptVersion : ushort
    {
        v1 = 1,

        Latest = v1
    }

    public enum GroupScriptOpcode : byte
    {
        Nonce = 0, // Just like me!

        AND = 1,
        BORN_AFTER = 2,
        BORN_BEFORE = 3,
        SPECIES_IS = 4,

        INVALID_OPCODE // Keep last
    }

    public enum GroupScriptParamType : byte
    {
        ParameterReference = 0,
        Date = 1,
        Int32 = 2
    }

    /**
     * All arrays (including strings) are prefixed
     * by a ushort for length.
     * 
     * Unless specified otherwise, the length describes elements,
     * not bytes.
     * 
     * The notation "byte(Opcode)" denotes that a byte is used to
     * represent an enum value for the "Opcode" enum.
     * 
     * All numbers are stored in big endian.
     * 
     * Format (Version 1):
     *  [ushort      VERSION]
     *  [uint(bytes) LENGTH_EXCLUDING_HEADER]
     *  [string      NAME]
     *  [ParamInfo[] PARAMETERS]
     *  [Action      MAIN_ACTION]
     * 
     * ParamInfo:
     *  [byte(ParamType) DATA_TYPE]
     *  [string          NAME]
     * 
     * Action:
     *  [byte(Opcode) OPCODE]
     *  [Depends on the action]
     * 
     * ParamValue:
     *  This will either be a reference to one of the script's parameters (DATA_TYPE of 0), or
     *  an inline literal value (any other DATA_TYPE). It depends what the user put into the script.
     *  
     *  [byte(ParamType)             DATA_TYPE]
     *  [{IF PARAM REFERENCE} string PARAM_NAME]
     *  [{OTHERWISE} Depends on the param value's DATA_TYPE]
     * 
     * ParamValue(int):
     *  Certain parameter types (like SPECIES) are implicitly an int, the extra typing
     *  is just for the interface generation (whenever I get to that ;^) )
     *
     *  [int VALUE]
     * 
     * ParamValue(Date):
     *  TODO
     * 
     * Action(AND):
     *  [Action[] ACTIONS]
     *  
     * Action(BORN AFTER) and Action(BORN BEFORE):
     *  [ParamValue(Date) DATE]
     *  
     * Action(SPECIES IS):
     *  [ParamValue(int) SPECIES_ID]
     * **/
    public class GroupScriptCompiler
    {
        private long                _lengthBytesIndex;
        private BinaryWriter        _output;
        private GroupScriptNodeTree _ast;

        private void ResetState(GroupScriptNodeTree ast, Stream outputStream)
        {
            this._lengthBytesIndex = 0;
            this._output           = new BinaryWriter(outputStream);
            this._ast              = ast;
        }

        public void Compile(GroupScriptNodeTree ast, Stream outputStream)
        {
            if(!outputStream.CanSeek)
                throw new Exception("The stream must support seeking.");

            if(!outputStream.CanWrite)
                throw new Exception("The stream must support writing.");

            this.ResetState(ast, outputStream);
            this.WriteHeader();
            this.WriteParameters();
            this.WriteAction(this._ast.RoutineHeadNode);
            this.FinishHeader();
        }

        private void WriteHeader()
        {
            this._output.WriteBigEndian((ushort)GroupScriptVersion.Latest);

            this._lengthBytesIndex = this._output.BaseStream.Position;
            this._output.Write((int)0); // Placeholder

            this._output.WriteUShortString(this._ast.ScriptName);
        }

        private void FinishHeader()
        {
            var finalIndex = this._output.BaseStream.Position;
            var count      = (finalIndex - this._lengthBytesIndex);

            if(count < 0)
                throw new Exception("???");

            this._output.BaseStream.Position = this._lengthBytesIndex;
            this._output.WriteBigEndian((uint)count);
        }

        private void WriteParameters()
        {
            this._output.WriteBigEndian((ushort)this._ast.Parameters.Count);
            foreach(var param in this._ast.Parameters)
            {
                GroupScriptParamType paramType;
                switch(param.DataType)
                {
                    case GroupScriptTokenType.Keyword_Species:
                        paramType = GroupScriptParamType.Int32; // This is correct, don't worry.
                        break;

                    default: throw new Exception($"Don't know how to handle parameter data type {param.DataType}");
                }
                this._output.Write((byte)paramType);
                this._output.WriteUShortString(param.Name);
            }
        }

        private void WriteAction(GroupScriptRoutineActionNode action)
        {
            // pfft, who needs performance where we're going
            if(action is GroupScriptAndActionNode andAction)
            {
                this._output.Write((byte)GroupScriptOpcode.AND);
                this._output.WriteBigEndian((ushort)andAction.Actions.Count);

                foreach(var subAction in andAction.Actions)
                    this.WriteAction(subAction);
            }
            else if(action is GroupScriptBornAfterActionNode bornAfter)
            {
                this._output.Write((byte)GroupScriptOpcode.BORN_AFTER);
                this.WriteParamValue(bornAfter.Date);
            }
            else if(action is GroupScriptBornBeforeActionNode bornBefore)
            {
                this._output.Write((byte)GroupScriptOpcode.BORN_BEFORE);
                this.WriteParamValue(bornBefore.Date);
            }
            else if(action is GroupScriptSpeciesIsActionNode speciesIs)
            {
                this._output.Write((byte)GroupScriptOpcode.SPECIES_IS);
                this.WriteParamValue(speciesIs.SpeciesId);
            }
            else
                throw new NotImplementedException($"Don't know how to handle {action.GetType()}");
        }

        private void WriteParamValue(GroupScriptParameterValueNode param)
        {
            switch(param.DataType)
            {
                case GroupScriptTokenType.Keyword_Param:
                    this._output.Write((byte)GroupScriptParamType.ParameterReference);
                    this._output.WriteUShortString(param.Value);
                    break;

                case GroupScriptTokenType.Keyword_Date:
                    this._output.Write((byte)GroupScriptParamType.Date);
                    this._output.WriteDate(param.Value);
                    break;

                case GroupScriptTokenType.Keyword_Species:
                    this._output.Write((byte)GroupScriptParamType.Int32);
                    this._output.WriteBigEndian(Convert.ToInt32(param.Value));
                    break;

                default:
                    throw new Exception($"Internal error: Don't know how to handle param type {param.DataType}");
            }
        }
    }

    internal static class BinaryWriterReaderExtention
    {
        #region Write
        public static void WriteUShortString(this BinaryWriter output, string value)
        {
            if(value.Length > ushort.MaxValue)
                throw new Exception("String is longer than a ushort.");

            output.WriteBigEndian((ushort)value.Length);
            output.Write(Encoding.ASCII.GetBytes(value));
        }

        // Don't ask... Just... don't...
        public static void WriteBigEndian(this BinaryWriter output, ushort value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                output.Write(value);
                return;
            }

            output.Write(BinaryPrimitives.ReverseEndianness(value));
        }

        public static void WriteBigEndian(this BinaryWriter output, int value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                output.Write(value);
                return;
            }

            output.Write(BinaryPrimitives.ReverseEndianness(value));
        }

        public static void WriteBigEndian(this BinaryWriter output, uint value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                output.Write(value);
                return;
            }

            output.Write(BinaryPrimitives.ReverseEndianness(value));
        }

        public static void WriteBigEndian(this BinaryWriter output, ulong value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                output.Write(value);
                return;
            }

            output.Write(BinaryPrimitives.ReverseEndianness(value));
        }

        public static void WriteDate(this BinaryWriter output, string dateString)
        {
            output.WriteDate(DateTimeOffset.ParseExact(dateString, "d/M/yyyy", CultureInfo.InvariantCulture));
        }

        public static void WriteDate(this BinaryWriter output, DateTimeOffset date)
        {
            output.WriteBigEndian((ulong)date.Ticks);
        }
        #endregion

        #region Read
        public static string ReadUShortString(this BinaryReader input)
        {
            var length = input.ReadBigEndianUInt16();
            return Encoding.ASCII.GetString(input.ReadBytes(length));
        }

        public static ushort ReadBigEndianUInt16(this BinaryReader input)
        {
            var value = BitConverter.ToUInt16(input.ReadBytes(2));
            if (!BitConverter.IsLittleEndian)
                return value;

            return BinaryPrimitives.ReverseEndianness(value);
        }

        public static uint ReadBigEndianUInt32(this BinaryReader input)
        {
            var value = BitConverter.ToUInt32(input.ReadBytes(4));
            if (!BitConverter.IsLittleEndian)
                return value;

            return BinaryPrimitives.ReverseEndianness(value);
        }

        public static int ReadBigEndianInt32(this BinaryReader input)
        {
            var value = BitConverter.ToInt32(input.ReadBytes(4));
            if (!BitConverter.IsLittleEndian)
                return value;

            return BinaryPrimitives.ReverseEndianness(value);
        }

        public static ulong ReadBigEndianUInt64(this BinaryReader input)
        {
            var value = BitConverter.ToUInt64(input.ReadBytes(8));
            if (!BitConverter.IsLittleEndian)
                return value;

            return BinaryPrimitives.ReverseEndianness(value);
        }
        #endregion
    }
}
