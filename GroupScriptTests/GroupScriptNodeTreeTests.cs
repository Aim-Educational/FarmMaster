using Microsoft.VisualStudio.TestTools.UnitTesting;
using GroupScript;
using System;
using System.Collections.Generic;
using System.Text;

namespace GroupScript.Tests
{
    [TestClass()]
    public class GroupScriptNodeTreeTests
    {
        [TestMethod()]
        public void GroupScriptNodeTreeTest()
        {
            var parser = new GroupScriptParser(
                @"NAME Born2019BySpecies

                PARAMETERS
	                SPECIES species
                END

                ROUTINE
	                AND
	                {
		                BORN AFTER  DATE:01/01/2019;
		                BORN BEFORE DATE:01/01/2020;
		                SPECIES IS  PARAM:species;
	                }
                END"
            );

            var ast = new GroupScriptNodeTree(parser);
            Assert.AreEqual("Born2019BySpecies", ast.ScriptName);

            Assert.AreEqual(1,                                    ast.Parameters.Count);
            Assert.AreEqual(GroupScriptTokenType.Keyword_Species, ast.Parameters[0].DataType);
            Assert.AreEqual("species",                            ast.Parameters[0].Name);

            Assert.IsInstanceOfType(ast.RoutineHeadNode, typeof(GroupScriptAndActionNode));
            var andNode = ast.RoutineHeadNode as GroupScriptAndActionNode;

            Assert.AreEqual(3, andNode.Actions.Count);

            Assert.IsInstanceOfType(andNode.Actions[0], typeof(GroupScriptBornAfterActionNode));
            var bornAfter = andNode.Actions[0] as GroupScriptBornAfterActionNode;
            Assert.IsNotNull(bornAfter.Date);
            Assert.AreEqual(GroupScriptTokenType.Keyword_Date, bornAfter.Date.DataType);
            Assert.AreEqual("01/01/2019",                      bornAfter.Date.Value);

            Assert.IsInstanceOfType(andNode.Actions[1], typeof(GroupScriptBornBeforeActionNode));
            var bornBefore = andNode.Actions[1] as GroupScriptBornBeforeActionNode;
            Assert.IsNotNull(bornBefore.Date);
            Assert.AreEqual(GroupScriptTokenType.Keyword_Date, bornBefore.Date.DataType);
            Assert.AreEqual("01/01/2020",                      bornBefore.Date.Value);

            Assert.IsInstanceOfType(andNode.Actions[2], typeof(GroupScriptSpeciesIsActionNode));
            var speciesIs = andNode.Actions[2] as GroupScriptSpeciesIsActionNode;
            Assert.IsNotNull(speciesIs.SpeciesId);
            Assert.AreEqual(GroupScriptTokenType.Keyword_Param, speciesIs.SpeciesId.DataType);
            Assert.AreEqual("species",                          speciesIs.SpeciesId.Value);
        }
    }
}