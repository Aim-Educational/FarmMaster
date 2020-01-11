using Microsoft.VisualStudio.TestTools.UnitTesting;
using GroupScript;
using System;
using System.Collections.Generic;
using System.Text;

namespace GroupScript.Tests
{
    [TestClass()]
    public class GroupScriptCompilerTests
    {
        [TestMethod()]
        public void CompileToStoredProcedureCodeTest()
        {
            var parser = new GroupScriptParser(
                @"NAME Born2019BySpecies

                PARAMETERS
	                SPECIES species
                    DATE after
                END

                ROUTINE
	                AND
	                {
		                BORN AFTER  PARAM:after;
		                BORN BEFORE DATE:01/01/2020;
		                SPECIES IS  PARAM:species;
	                }
                END"
            );

            var ast = new GroupScriptNodeTree(parser);
            var result = GroupScriptCompiler.CompileToStoredProcedureCode(ast, new Dictionary<string, object>()
            {
                { "after", new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.FromSeconds(0)) },
                { "species", 1 }
            });

            Assert.AreEqual(
                "AND~BORN_AFTER 2019-01-01T00:00:00.0000000+00:00~BORN_BEFORE 2020-01-01T00:00:00.0000000+00:00~SPECIES_IS 1~END~", 
                result
            );
        }
    }
}