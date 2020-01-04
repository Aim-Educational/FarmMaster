using Microsoft.VisualStudio.TestTools.UnitTesting;
using GroupScript;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Business.Model;

namespace GroupScript.Tests
{
    [TestClass()]
    public class GroupScriptEvaluatorTests
    {
        [TestMethod()]
        public void GroupScriptEvaluatorTest()
        {
            var animals = new List<Animal>()
            {
                new Animal
                {
                    Name = "Zuzu",
                    SpeciesId = 1,
                    LifeEventEntries = new List<MapLifeEventEntryToAnimal>()
                    {
                        new MapLifeEventEntryToAnimal
                        {
                            LifeEventEntry = new LifeEventEntry
                            {
                                LifeEvent = new LifeEvent { Name = BusinessConstants.BuiltinLifeEvents.BORN },
                                Values = new List<LifeEventDynamicFieldValue>()
                                {
                                    new LifeEventDynamicFieldValue
                                    {
                                        LifeEventDynamicFieldInfo = new LifeEventDynamicFieldInfo{ Name = BusinessConstants.BuiltinLifeEventFields.BORN_DATE },
                                        Value = new DynamicFieldDateTime{ DateTime = new DateTimeOffset(2019, 04, 13, 0, 0, 0, TimeSpan.FromSeconds(0)) }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var code =
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
                END";

            var evaluator = new GroupScriptEvaluator(code);

            Assert.AreEqual("Born2019BySpecies", evaluator.Name);

            var animal = animals[0];
            Assert.IsTrue(evaluator.MatchesFilter(animal, new Dictionary<string, object>
            {
                { "species", 1 }
            }));
            Assert.IsFalse(evaluator.MatchesFilter(animal, new Dictionary<string, object>
            {
                { "species", 2 }
            }));
        }

        [TestMethod()]
        public void DecompileToStringTest()
        {
            var code =
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
                END";

            var evaluator = new GroupScriptEvaluator(code);

            var decompiled = 
                "NAME Born2019BySpecies\n\n" +
                "PARAMS\n" +
                "    SPECIES species\n" +
                "END\n\n" +
                "ROUTINE\n" +
                "    AND\n" +
                "    {\n" +
                "        BORN AFTER DATE:01/01/2019;\n" +
                "        BORN BEFORE DATE:01/01/2020;\n" +
                "        SPECIES IS PARAM:species;\n" +
                "    }\n" +
                "END";

            var got = evaluator.DecompileToString();

            for(int i = 0; i < decompiled.Length; i++)
            {
                Assert.IsTrue(i < got.Length, "got is shorter than expected");
                Assert.AreEqual(decompiled[i], got[i], $"Index {i}. expected {(byte)decompiled[i]}, got {(byte)got[i]}");
            }

            Assert.AreEqual(decompiled.Length, got.Length);
        }
    }
}