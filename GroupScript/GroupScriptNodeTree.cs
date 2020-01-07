﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GroupScript
{
    public class GroupScriptNodeTree
    {
        public string ScriptName { get; set; }

        public IList<GroupScriptParameterDeclarationNode> Parameters { get; set; }

        public GroupScriptRoutineActionNode RoutineHeadNode { get; set; }

        public GroupScriptNodeTree(IEnumerable<GroupScriptToken> tokens)
        {
            this.Parameters = new List<GroupScriptParameterDeclarationNode>();
            this.FromTokens(tokens.GetEnumerator());
        }

        #region Parsers
        void FromTokens(IEnumerator<GroupScriptToken> tokens)
        {
            if(tokens.Current == null)
                tokens.MoveNext();

            this.ParseName(tokens);
            this.ParseParameters(tokens);
            this.ParseRoutine(tokens);

            tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.EoF);
        }

        void ParseName(IEnumerator<GroupScriptToken> tokens)
        {
            tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Keyword_Name);
            tokens.MoveNext();

            tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Identifier);
            this.ScriptName = tokens.Current.Text;
            tokens.MoveNext();
        }

        void ParseParameters(IEnumerator<GroupScriptToken> tokens)
        {
            tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Keyword_Parameters);
            tokens.MoveNext();

            while(true)
            {
                tokens.Current.EnforceTokenTypeIsAnyOf(
                    GroupScriptTokenType.Keyword_End,
                    GroupScriptTokenType.Keyword_Species
                );

                if(tokens.Current.Type == GroupScriptTokenType.Keyword_End)
                {
                    tokens.MoveNext();
                    return;
                }

                var type = tokens.Current.Type;
                tokens.MoveNext();

                var value = tokens.Current.Text;
                tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Identifier);
                tokens.MoveNext();

                this.Parameters.Add(new GroupScriptParameterDeclarationNode
                {
                    DataType = type,
                    Name     = value
                });
            }
        }

        GroupScriptParameterValueNode ParseParameterValue(IEnumerator<GroupScriptToken> tokens)
        {
            tokens.Current.EnforceTokenTypeIsAnyOf(
                GroupScriptTokenType.Keyword_Date,
                GroupScriptTokenType.Keyword_Param
            );

            var type = tokens.Current.Type;
            
            tokens.MoveNext();
            tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Operator_Colon);
            tokens.MoveNext();

            switch (type)
            {
                case GroupScriptTokenType.Keyword_Date:
                    tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Literal_Date);
                    break;

                case GroupScriptTokenType.Keyword_Param:
                    tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Identifier);
                    break;
            }

            var value = tokens.Current.Text;
            tokens.MoveNext();

            return new GroupScriptParameterValueNode 
            { 
                DataType = type,
                Value    = value
            };
        }

        void ParseRoutine(IEnumerator<GroupScriptToken> tokens)
        {
            tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Keyword_Routine);
            tokens.MoveNext();

            var blocks = new Stack<GroupScriptRoutineActionBlockNode>();
            var addOrPushAction = new Action<GroupScriptRoutineActionNode>(action => 
            {
                if (blocks.Count > 0)
                    blocks.Peek().Actions.Add(action);
                else
                {
                    if(this.RoutineHeadNode != null)
                        throw new Exception("There can only be one top-level action inside the routine.");

                    this.RoutineHeadNode = action;
                }
            });

            while(true)
            {
                tokens.Current.EnforceTokenTypeIsAnyOf(
                    GroupScriptTokenType.Keyword_End,
                    GroupScriptTokenType.Keyword_And,
                    GroupScriptTokenType.Keyword_Born,
                    GroupScriptTokenType.Keyword_Species,
                    GroupScriptTokenType.Operator_BracketR
                );

                if (tokens.Current.Type == GroupScriptTokenType.Keyword_End)
                {
                    tokens.MoveNext();
                    return;
                }

                switch(tokens.Current.Type)
                {
                    case GroupScriptTokenType.Operator_BracketR:
                        if(blocks.Count == 0)
                            throw new Exception($"Stray right bracket ('}}') around line {tokens.Current.Line} column {tokens.Current.Column}");

                        blocks.Pop();
                        tokens.MoveNext();
                        break;

                    case GroupScriptTokenType.Keyword_And:
                        tokens.MoveNext();
                        tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Operator_BracketL);
                        tokens.MoveNext();

                        var andBlock = new GroupScriptAndActionNode();
                        addOrPushAction(andBlock);
                        blocks.Push(andBlock);
                        break;

                    case GroupScriptTokenType.Keyword_Species:
                        tokens.MoveNext();
                        tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Keyword_Is);
                        tokens.MoveNext();

                        addOrPushAction(new GroupScriptSpeciesIsActionNode
                        {
                            SpeciesId = this.ParseParameterValue(tokens)
                        });

                        tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Operator_Semicolon);
                        tokens.MoveNext();
                        break;

                    case GroupScriptTokenType.Keyword_Born:
                        tokens.MoveNext();
                        tokens.Current.EnforceTokenTypeIsAnyOf(
                            GroupScriptTokenType.Keyword_After, 
                            GroupScriptTokenType.Keyword_Before
                        );

                        var bornType = tokens.Current.Type;
                        tokens.MoveNext();

                        var param = this.ParseParameterValue(tokens);
                        addOrPushAction(
                            (bornType == GroupScriptTokenType.Keyword_After)
                            ? new GroupScriptBornAfterActionNode  { Date = param } as GroupScriptRoutineActionNode
                            : new GroupScriptBornBeforeActionNode { Date = param }
                        );

                        tokens.Current.EnforceTokenTypeIsAnyOf(GroupScriptTokenType.Operator_Semicolon);
                        tokens.MoveNext();
                        break;

                    default: throw new Exception("This shouldn't happen");
                }
            }
        }
        #endregion
    }

    public class GroupScriptParameterDeclarationNode
    {
        public string Name { get; set; }
        public GroupScriptTokenType DataType { get; set; }
    }

    public class GroupScriptParameterValueNode
    {
        public GroupScriptTokenType DataType { get; set; }
        public string Value { get; set; }
    }

    public abstract class GroupScriptRoutineActionNode
    {
    }

    public abstract class GroupScriptRoutineActionBlockNode : GroupScriptRoutineActionNode
    {
        public IList<GroupScriptRoutineActionNode> Actions { get; set; }

        public GroupScriptRoutineActionBlockNode()
        {
            this.Actions = new List<GroupScriptRoutineActionNode>();
        }
    }

    public class GroupScriptAndActionNode : GroupScriptRoutineActionBlockNode
    {
    }

    public class GroupScriptBornAfterActionNode : GroupScriptRoutineActionNode
    {
        public GroupScriptParameterValueNode Date { get; set; }
    }

    public class GroupScriptBornBeforeActionNode : GroupScriptRoutineActionNode
    {
        public GroupScriptParameterValueNode Date { get; set; }
    }

    public class GroupScriptSpeciesIsActionNode : GroupScriptRoutineActionNode
    {
        public GroupScriptParameterValueNode SpeciesId { get; set; }
    }
}