using Business.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GroupScript 
{
    public class GroupScriptEvaluator
    {
        enum BornAction
        {
            AFTER,
            BEFORE
        }

        public class ParamDeclaration // Could technically use GroupScriptParamDeclarationNode instead, but their data might eventually differ... maybe.
        {
            public string Name { get; set; }
            public GroupScriptParamType Type { get; set; }
        }

        class ParamValue
        {
            public GroupScriptParamType Type { get; set; }
            public object Value { get; set; }
        }

        abstract class ScriptAction
        {
            public Func<ScriptAction, Animal, IDictionary<string, object>, bool> AnimalFilter { get; set; }
        }

        class SingleAction : ScriptAction
        {
            public GroupScriptOpcode Opcode { get; set; }
            public IEnumerable<ParamValue> Params { get; set; }
        }

        class BlockAction : ScriptAction
        {
            public GroupScriptOpcode Opcode { get; set; }
            public IList<ScriptAction> Actions { get; set; }

            public BlockAction()
            {
                this.Actions = new List<ScriptAction>();
            }
        }

        private BlockAction             _mainAction;
        private IList<ParamDeclaration> _paramInfo;

        public IEnumerable<ParamDeclaration> ParameterDeclarations => this._paramInfo;
        
        public string Name { get; private set; }

        public GroupScriptEvaluator(Stream byteStream)
        {
            this._paramInfo = new List<ParamDeclaration>();
            this.FromBytecode(new BinaryReader(byteStream));
        }

        public GroupScriptEvaluator(string code)
        {
            var parser   = new GroupScriptParser(code);
            var ast      = new GroupScriptNodeTree(parser);
            var compiler = new GroupScriptCompiler();
            using(var stream = new MemoryStream())
            {
                compiler.Compile(ast, stream);
                stream.Position = 0;

                this._paramInfo = new List<ParamDeclaration>();
                this.FromBytecode(new BinaryReader(stream));
            }
        }

        public bool MatchesFilter(Animal animal, IDictionary<string, object> paramValues)
        {
            foreach(var paramInfo in this._paramInfo)
            {
                if(!paramValues.ContainsKey(paramInfo.Name))
                    throw new InvalidOperationException($"Parameter '{paramInfo.Name}' of type {paramInfo.Type} was not provided.");
            }

            return this._mainAction.AnimalFilter(this._mainAction, animal, paramValues);
        }

        #region Filter funcs
        bool FilterFuncAND(ScriptAction action, Animal animal, IDictionary<string, object> paramValues)
        {
            var andAction = (action as BlockAction);
            if(andAction == null)
                throw new InvalidOperationException("Expected a BlockAction, not a SingleAction.");
            if(andAction.Opcode != GroupScriptOpcode.AND)
                throw new InvalidOperationException($"Expected an AND BlockAction, not a(n) {andAction.Opcode} BlockAction.");

            return andAction.Actions.All(a => a.AnimalFilter(a, animal, paramValues));
        }

        bool FilterFuncSPECIES_IS(ScriptAction action, Animal animal, IDictionary<string, object> paramValues)
        {
            var speciesAction = (action as SingleAction);
            if (speciesAction == null)
                throw new InvalidOperationException("Expected a SingleAction, not a BlockAction.");
            if (speciesAction.Opcode != GroupScriptOpcode.SPECIES_IS)
                throw new InvalidOperationException($"Expected a SPECIES_IS SingleAction, not a(n) {speciesAction.Opcode} SingleAction.");

            return animal.SpeciesId == this.GetParamInt32(speciesAction.Params.Single(), paramValues);
        }

        bool FilterFuncBORN_GENERIC(ScriptAction action, Animal animal, IDictionary<string, object> paramValues, BornAction bornType)
        {
            var bornAction = (action as SingleAction);
            if (bornAction == null)
                throw new InvalidOperationException("Expected a SingleAction, not a BlockAction.");
            if (bornAction.Opcode != GroupScriptOpcode.BORN_AFTER && bornAction.Opcode != GroupScriptOpcode.BORN_BEFORE)
                throw new InvalidOperationException($"Expected a BORN_AFTER or BORN_BEFORE SingleAction, not a(n) {bornAction.Opcode} SingleAction.");

            var bornDate = animal.LifeEventEntries
                                 .First(m => m.LifeEventEntry.LifeEvent.Name == BusinessConstants.BuiltinLifeEvents.BORN)
                                 .LifeEventEntry
                                 .Values
                                 .First(v => v.LifeEventDynamicFieldInfo.Name == BusinessConstants.BuiltinLifeEventFields.BORN_DATE)
                                 .Value
                                 as DynamicFieldDateTime;
            if(bornDate == null)
                throw new Exception("bornDate shouldn't be null unless the database was messed with in a weird way.");

            var paramDate = this.GetParamDate(bornAction.Params.First(), paramValues);
            return (bornType == BornAction.AFTER)
                ? bornDate.DateTime > paramDate
                : bornDate.DateTime < paramDate;
        }

        bool FilterFuncBORN_AFTER(ScriptAction action, Animal animal, IDictionary<string, object> paramValues)
        {
            return this.FilterFuncBORN_GENERIC(action, animal, paramValues, BornAction.AFTER);
        }

        bool FilterFuncBORN_BEFORE(ScriptAction action, Animal animal, IDictionary<string, object> paramValues)
        {
            return this.FilterFuncBORN_GENERIC(action, animal, paramValues, BornAction.BEFORE);
        }
        #endregion

        #region Helpers
        int GetParamInt32(ParamValue value, IDictionary<string, object> paramValues)
        {
            if (value.Type == GroupScriptParamType.Int32 || value.Type == GroupScriptParamType.Species)
                return (int)value.Value;
            else if (value.Type == GroupScriptParamType.ParameterReference)
                return (int)paramValues[(string)value.Value];
            else
                throw new InvalidOperationException($"Expected a param reference or an int32, not a {value.Type}.");
        }

        DateTimeOffset GetParamDate(ParamValue value, IDictionary<string, object> paramValues)
        {
            if (value.Type == GroupScriptParamType.Date)
                return (DateTimeOffset)value.Value;
            else if (value.Type == GroupScriptParamType.ParameterReference)
                return (DateTimeOffset)paramValues[(string)value.Value];
            else
                throw new InvalidOperationException($"Expected a param reference or a Date, not a {value.Type}.");
        }
        #endregion

        #region From Bytecode
        void FromBytecode(BinaryReader code)
        {
            // Read header
            var version = code.ReadBigEndianUInt16();
            if(version > (ushort)GroupScriptVersion.Latest)
                throw new InvalidDataException($"Input code is version {version}, but I can only handle up to version {(ushort)GroupScriptVersion.Latest}");

            var length = code.ReadBigEndianUInt32(); // Don't know why I put that in there tbh, don't really need it right now.
            this.Name = code.ReadUShortString();

            // Read parameters (to the script)
            var paramCount = code.ReadBigEndianUInt16();
            for(int i = 0; i < paramCount; i++)
                this.ReadParamInfoFromBytecode(code);

            // Read the main action.
            var action = this.ReadActionFromBytecode(code) as BlockAction;
            if(action == null)
                throw new InvalidDataException($"The main action for a script must be a block action (such as AND)");

            this._mainAction = action;
        }

        void ReadParamInfoFromBytecode(BinaryReader code)
        {
            this._paramInfo.Add(new ParamDeclaration
            {
                Type = (GroupScriptParamType)code.ReadByte(),
                Name = code.ReadUShortString()
            });
        }

        ParamValue ReadParamValueFromBytecode(BinaryReader code, GroupScriptOpcode opcode, GroupScriptParamType expectedType)
        {
            var type     = (GroupScriptParamType)code.ReadByte();
            object value = null;

            if(type != GroupScriptParamType.ParameterReference && type != expectedType)
            {
                throw new InvalidDataException(
                    $"Opcode {opcode} expected a reference or a parameter of type {expectedType} not {type}"
                );
            }
            
            switch(type)
            {
                case GroupScriptParamType.ParameterReference:
                    value = code.ReadUShortString();
                    break;

                case GroupScriptParamType.Species:
                case GroupScriptParamType.Int32:
                    value = code.ReadBigEndianInt32();
                    break;

                case GroupScriptParamType.Date:
                    value = new DateTimeOffset((long)code.ReadBigEndianUInt64(), TimeSpan.FromSeconds(0));
                    break;

                default: throw new InvalidDataException($"No handler for param type: {type}");
            }

            return new ParamValue 
            {
                Type  = type,
                Value = value
            };
        }

        ScriptAction ReadActionFromBytecode(BinaryReader code)
        {
            var opcodeByte = code.ReadByte();
            if(opcodeByte >= (byte)GroupScriptOpcode.INVALID_OPCODE)
                throw new InvalidDataException($"Illegal opcode: {opcodeByte}");

            var opcode = (GroupScriptOpcode)opcodeByte;
            switch(opcode)
            {
                case GroupScriptOpcode.AND:
                    var andAction          = new BlockAction();
                    andAction.Opcode       = opcode;
                    andAction.AnimalFilter = this.FilterFuncAND;

                    var andActionCount = code.ReadBigEndianUInt16();
                    for(int i = 0; i < andActionCount; i++)
                        andAction.Actions.Add(this.ReadActionFromBytecode(code));
                    return andAction;

                case GroupScriptOpcode.BORN_BEFORE:
                case GroupScriptOpcode.BORN_AFTER:
                    var bornAction = new SingleAction
                    {
                        AnimalFilter = this.FilterFuncBORN_BEFORE,
                        Opcode = opcode,
                        Params = new[] { this.ReadParamValueFromBytecode(code, opcode, GroupScriptParamType.Date) }
                    };

                    if(opcode == GroupScriptOpcode.BORN_AFTER)
                        bornAction.AnimalFilter = this.FilterFuncBORN_AFTER;

                    return bornAction;

                case GroupScriptOpcode.SPECIES_IS:
                    return new SingleAction
                    {
                        AnimalFilter = this.FilterFuncSPECIES_IS,
                        Opcode = opcode,
                        Params = new[] { this.ReadParamValueFromBytecode(code, opcode, GroupScriptParamType.Species) }
                    };

                default: throw new NotImplementedException($"No handler for opcode: {opcode}");
            }
        }
        #endregion
    }
}