using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GroupScript
{
    public static class GroupScriptCompiler
    {
        private const string COMMAND_DELIM = "~";

        public static string CompileToStoredProcedureCode(GroupScriptNodeTree ast, IDictionary<string, object> parameters)
        {
            GroupScriptCompiler.EnforceAllParametersAreValid(ast.Parameters, parameters);
            
            var output = new StringBuilder(128);

            if(ast.RoutineHeadNode != null)
                GroupScriptCompiler.CompileNode(ast.RoutineHeadNode, output, parameters);

            return output.ToString();
        }

        private static void CompileNode(GroupScriptRoutineActionNode action, StringBuilder output, IDictionary<string, object> parameters)
        {
            if(action.IsInverse)
            {
                output.Append("NOT");
                output.Append(COMMAND_DELIM);
            }

            // Pfft, "performance"?
            if(action is GroupScriptAndActionNode andAction)
            {
                output.Append("AND");
                output.Append(COMMAND_DELIM);

                foreach(var subAction in andAction.Actions)
                    GroupScriptCompiler.CompileNode(subAction, output, parameters);

                output.Append("END");
                output.Append(COMMAND_DELIM);
            }
            else if (action is GroupScriptOrActionNode orAction) // Inline "is" variables don't mesh with "||" well, so we duplicate the code.
            {
                output.Append("OR");
                output.Append(COMMAND_DELIM);

                foreach (var subAction in orAction.Actions)
                    GroupScriptCompiler.CompileNode(subAction, output, parameters);

                output.Append("END");
                output.Append(COMMAND_DELIM);
            }
            else if(action is GroupScriptBornAfterActionNode bornAfterNode)
            {
                output.Append("BORN_AFTER ");
                output.Append(GroupScriptCompiler.GetDate(bornAfterNode.Date, parameters).ToString("o"));
                output.Append(COMMAND_DELIM);
            }
            else if(action is GroupScriptBornBeforeActionNode bornBeforeNode)
            {
                output.Append("BORN_BEFORE ");
                output.Append(GroupScriptCompiler.GetDate(bornBeforeNode.Date, parameters).ToString("o"));
                output.Append(COMMAND_DELIM);
            }
            else if(action is GroupScriptSpeciesIsActionNode speciesIsNode)
            {
                output.Append("SPECIES_IS ");
                output.Append(GroupScriptCompiler.GetInt(speciesIsNode.SpeciesId, parameters));
                output.Append(COMMAND_DELIM);
            }
            else
                throw new NotImplementedException($"Action node {action.GetType()} doesn't have a handler yet.");
        }

        private static int GetInt(GroupScriptParameterValueNode param, IDictionary<string, object> parameters)
        {
            if (param.DataType == GroupScriptTokenType.Keyword_Species || param.DataType == GroupScriptTokenType.Keyword_Int)
                return Convert.ToInt32(param.Value);
            else if (param.DataType == GroupScriptTokenType.Keyword_Param)
                return Convert.ToInt32(parameters[param.Value]);
            else
                throw new NotSupportedException($"Expected a SPECIES, INT, or PARAM value, not a {param.DataType}.");
        }

        private static DateTimeOffset GetDate(GroupScriptParameterValueNode param, IDictionary<string, object> parameters)
        {
            if(param.DataType == GroupScriptTokenType.Keyword_Date)
                return DateTimeOffset.ParseExact(param.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            else if(param.DataType == GroupScriptTokenType.Keyword_Param)
                return (DateTimeOffset)parameters[param.Value];
            else
                throw new NotSupportedException($"Expected a DATE or PARAM value, not a {param.DataType}.");
        }

        private static void EnforceAllParametersAreValid(
            IList<GroupScriptParameterDeclarationNode> ExpectedParameters,
            IDictionary<string, object>                GotParameters
        )
        {
            if(GotParameters == null)
            {
                if(ExpectedParameters.Count > 0)
                    throw new Exception($"Expected {ExpectedParameters.Count} parameters, but was given null instead.");

                return;
            }

            foreach(var param in ExpectedParameters)
            {
                if(!GotParameters.ContainsKey(param.Name))
                    throw new KeyNotFoundException($"No value for parameter '{param.Name}' was passed.");

                var paramValueType = GotParameters[param.Name]?.GetType();
                if(paramValueType == null)
                    throw new NullReferenceException($"Value for parameter '{param.Name}' is null.");

                switch(param.DataType)
                {
                    case GroupScriptTokenType.Keyword_Int:
                    case GroupScriptTokenType.Keyword_Species:
                        if (paramValueType != typeof(int) && paramValueType != typeof(long))
                            throw new Exception($"For SPECIES or INT parameter '{param.Name}' expected value of type int not {paramValueType}");
                        break;

                    case GroupScriptTokenType.Keyword_Date:
                        if (paramValueType != typeof(DateTimeOffset))
                            throw new Exception($"For DATE parameter '{param.Name}' expected value of type DateTimeOffset not {paramValueType}");
                        break;

                    default: throw new NotSupportedException($"{param.DataType}");
                }
            }
        }
    }
}
