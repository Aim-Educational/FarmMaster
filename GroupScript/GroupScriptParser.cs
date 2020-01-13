﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GroupScript
{
    public enum GroupScriptTokenType : byte
    {
        ERROR = 0,

        // Values MUST be kept the same, even if you change the lexical order up.
        Identifier          = 1,
        Literal_Date        = 2,

        Operator_BracketL   = 3,
        Operator_BracketR   = 4,
        Operator_Colon      = 5,
        Operator_Semicolon  = 6,

        Keyword_Name        = 7,
        Keyword_Parameters  = 8,
        Keyword_End         = 9,
        Keyword_Routine     = 10,
        Keyword_Species     = 11,
        Keyword_Born        = 12,
        Keyword_After       = 13,
        Keyword_Before      = 14,
        Keyword_Date        = 15,
        Keyword_Param       = 16,
        Keyword_And         = 17,
        Keyword_Is          = 18,

        EoF = 255
    }

    public class GroupScriptToken
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string Text { get; set; }
        public GroupScriptTokenType Type { get; set; }

        public void EnforceTokenTypeIsAnyOf(params GroupScriptTokenType[] types)
        {
            if(!types.Any(t => t == this.Type))
            {    
                throw new Exception(
                    $"Unexpected token of type '{this.Type}' value '{this.Text}' at line {this.Line} column {this.Column}.\n" +
                    $"Expected any of: {{{types.Select(t => Convert.ToString(t)).Aggregate((s1, s2) => $"{s1}, {s2}")}}}"
                );
            }
        }
    }

    public class GroupScriptParser : IEnumerable<GroupScriptToken>
    {
        public readonly string Code;

        public GroupScriptParser(string code)
        {
            this.Code = code;
        }

        public IEnumerator<GroupScriptToken> GetEnumerator()
        {
            return new ParserImpl { _code = this.Code }; 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    internal class ParserImpl : IEnumerator<GroupScriptToken>
    {
        private int _line;
        private int _column;
        private int _index;
        internal string _code;

        private Dictionary<char, GroupScriptTokenType> _operators = new Dictionary<char, GroupScriptTokenType>
        {
            { '{', GroupScriptTokenType.Operator_BracketL  },
            { '}', GroupScriptTokenType.Operator_BracketR  },
            { ':', GroupScriptTokenType.Operator_Colon     },
            { ';', GroupScriptTokenType.Operator_Semicolon }
        };

        private Dictionary<string, GroupScriptTokenType> _keywords = new Dictionary<string, GroupScriptTokenType> 
        {
            { "NAME",       GroupScriptTokenType.Keyword_Name       },
            { "PARAMETERS", GroupScriptTokenType.Keyword_Parameters },
            { "END",        GroupScriptTokenType.Keyword_End        },
            { "ROUTINE",    GroupScriptTokenType.Keyword_Routine    },
            { "SPECIES",    GroupScriptTokenType.Keyword_Species    },
            { "BORN",       GroupScriptTokenType.Keyword_Born       },
            { "AFTER",      GroupScriptTokenType.Keyword_After      },
            { "BEFORE",     GroupScriptTokenType.Keyword_Before     },
            { "DATE",       GroupScriptTokenType.Keyword_Date       },
            { "PARAM",      GroupScriptTokenType.Keyword_Param      },
            { "AND",        GroupScriptTokenType.Keyword_And        },
            { "IS",         GroupScriptTokenType.Keyword_Is         }
        };

        public GroupScriptToken Current { get; private set; }

        object IEnumerator.Current => this.Current;

        public bool MoveNext()
        {
            this.EatWhitespace();

            if (this.Current != null && this.Current.Type == GroupScriptTokenType.EoF)
                return false;

            if (this.IsEoF)
            {
                this.CreateToken(GroupScriptTokenType.EoF);
                return true;
            }

            var isOperator = this._operators.TryGetValue(this.CurrentChar, out GroupScriptTokenType operatorType);
            if(isOperator)
            {
                this.EatChar();
                this.CreateToken(operatorType);
                return true;
            }

            var slice = this.ReadUntilWhitespaceOrOperator();
            var count = (slice.end - slice.start);
            var text  = this._code.Skip(slice.start).Take(count);

            // Check if it's a keyword.
            foreach(KeyValuePair<string, GroupScriptTokenType> kvp in this._keywords)
            {
                if(kvp.Key.SequenceEqual(text))
                {
                    this.CreateToken(kvp.Value);
                    return true;
                }
            }

            // Check if it's a date. Taken from: https://www.regular-expressions.info/dates.html
            var textString = this._code.Substring(slice.start, count);
            if(Regex.IsMatch(textString, @"^(0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[012])\/\d\d\d\d$"))
            {
                this.CreateToken(GroupScriptTokenType.Literal_Date, textString);
                return true;
            }

            // Otherwise, treat it as an identifier.
            this.CreateToken(GroupScriptTokenType.Identifier, textString);
            return true;
        }

        public void Reset()
        {
            this._line   = 0;
            this._column = 0;
            this._index  = 0;
            this.Current = null;
        }

        #region Helpers
        char CurrentChar => this._code[this._index];
        bool IsEoF       => this._index >= this._code.Length;

        void CreateToken(GroupScriptTokenType type, string text = null)
        {
            this.Current = new GroupScriptToken 
            {
                Column = this._column,
                Line   = this._line,
                Text   = text,
                Type   = type
            };
        }

        void EatChar()
        {
            this._index++;
            this._column++;

            if(!this.IsEoF && this.CurrentChar == '\n')
            {
                this._column = 0;
                this._line++;
            }
        }

        void EatWhitespace()
        {
            while(!this.IsEoF && Char.IsWhiteSpace(this.CurrentChar))
                this.EatChar();
        }

        (int start, int end) ReadUntilWhitespaceOrOperator()
        {
            var start = this._index;
            var end   = start;

            while(!this.IsEoF && !Char.IsWhiteSpace(this.CurrentChar) && !this._operators.ContainsKey(this.CurrentChar))
            {
                end++;
                this.EatChar();
            }

            return (start, end);
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
