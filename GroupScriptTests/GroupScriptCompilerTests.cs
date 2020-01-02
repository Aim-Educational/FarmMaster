using Microsoft.VisualStudio.TestTools.UnitTesting;
using GroupScript;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GroupScript.Tests
{
    [TestClass()]
    public class GroupScriptCompilerTests
    {
        [TestMethod()]
        public void CompileTest()
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
            var compiler = new GroupScriptCompiler();

            byte[] data = null;
            using(var stream = new MemoryStream())
            {
                compiler.Compile(ast, stream);
                data = stream.ToArray();
            }

            // So I can analyse any issues in a hex editor.
            File.WriteAllBytes("group_script_compile_test.bin", data);

            var expected = new byte[] 
            {
                0x00, 0x01,                                                                         // Version
                0x00, 0x00, 0x00, 0x45,                                                             // Length

                0x00, 0x11, 0x42, 0x6F, 0x72, 0x6E, 0x32, 0x30, 0x31, 0x39, 0x42, 0x79, 0x53, 0x70, // Name
                0x65, 0x63, 0x69, 0x65, 0x73,                                                       // Name cont.

                0x00, 0x01,                                                                         // Param count
                0x02, 0x00, 0x07, 0x73, 0x70, 0x65, 0x63, 0x69, 0x65, 0x73,                         // Param 1
                0x01, 0x00, 0x03,                                                                   // Action (AND)         + Action count
                0x02, 0x01, 0x08, 0xD6, 0x6F, 0x7C, 0x12, 0xB5, 0x40, 0x00,                         // Action (BORN AFTER)  + Date
                0x03, 0x01, 0x08, 0xD7, 0x8E, 0x4D, 0x8B, 0x7C, 0x00, 0x00,                         // Action (BORN BEFORE) + Date
                0x04, 0x00, 0x00, 0x07, 0x73, 0x70, 0x65, 0x63, 0x69, 0x65, 0x73                    // Action (SPECIES IS)  + Param Ref
            };

            Assert.AreEqual(expected.Length, data.Length, $"Length mismatch: Wanted {expected.Length} bytes vs got {data.Length} bytes");

            for(int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], data[i], $"Mismatch at index {i}: Wanted {expected[i]} but got {data[i]}");
        }
    }
}