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
                        NOT BORN BEFORE DATE:01/01/2019;
                        OR {}
                        NOT NOT SPECIES IS SPECIES:20;
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

            Assert.AreEqual(6, andNode.Actions.Count);
            Assert.IsFalse(andNode.IsInverse);

            Assert.IsInstanceOfType(andNode.Actions[0], typeof(GroupScriptBornAfterActionNode));
            var bornAfter = andNode.Actions[0] as GroupScriptBornAfterActionNode;
            Assert.IsNotNull(bornAfter.Date);
            Assert.IsFalse(bornAfter.IsInverse);
            Assert.AreEqual(GroupScriptTokenType.Keyword_Date, bornAfter.Date.DataType);
            Assert.AreEqual("01/01/2019",                      bornAfter.Date.Value);

            Assert.IsInstanceOfType(andNode.Actions[1], typeof(GroupScriptBornBeforeActionNode));
            var bornBefore = andNode.Actions[1] as GroupScriptBornBeforeActionNode;
            Assert.IsNotNull(bornBefore.Date);
            Assert.IsFalse(bornBefore.IsInverse);
            Assert.AreEqual(GroupScriptTokenType.Keyword_Date, bornBefore.Date.DataType);
            Assert.AreEqual("01/01/2020",                      bornBefore.Date.Value);

            Assert.IsInstanceOfType(andNode.Actions[2], typeof(GroupScriptSpeciesIsActionNode));
            var speciesIs = andNode.Actions[2] as GroupScriptSpeciesIsActionNode;
            Assert.IsNotNull(speciesIs.SpeciesId);
            Assert.IsFalse(speciesIs.IsInverse);
            Assert.AreEqual(GroupScriptTokenType.Keyword_Param, speciesIs.SpeciesId.DataType);
            Assert.AreEqual("species",                          speciesIs.SpeciesId.Value);

            Assert.IsInstanceOfType(andNode.Actions[3], typeof(GroupScriptBornBeforeActionNode));
            var notBornBefore = andNode.Actions[3] as GroupScriptBornBeforeActionNode;
            Assert.IsNotNull(notBornBefore.Date);
            Assert.IsTrue(notBornBefore.IsInverse);
            Assert.AreEqual(GroupScriptTokenType.Keyword_Date, notBornBefore.Date.DataType);
            Assert.AreEqual("01/01/2019",                      notBornBefore.Date.Value);

            Assert.IsInstanceOfType(andNode.Actions[4], typeof(GroupScriptOrActionNode));
            var orNode = andNode.Actions[4] as GroupScriptOrActionNode;
            Assert.AreEqual(0, orNode.Actions.Count);
            Assert.IsFalse(orNode.IsInverse);

            Assert.IsInstanceOfType(andNode.Actions[5], typeof(GroupScriptSpeciesIsActionNode));
            speciesIs = andNode.Actions[5] as GroupScriptSpeciesIsActionNode;
            Assert.IsNotNull(speciesIs.SpeciesId);
            Assert.IsFalse(speciesIs.IsInverse);
            Assert.AreEqual(GroupScriptTokenType.Keyword_Species, speciesIs.SpeciesId.DataType);
            Assert.AreEqual("20",                                 speciesIs.SpeciesId.Value);
        }
    }
}