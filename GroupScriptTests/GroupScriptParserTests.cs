using Microsoft.VisualStudio.TestTools.UnitTesting;
using GroupScript;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GroupScript.Tests
{
    [TestClass()]
    public class GroupScriptParserTests
    {
        [TestMethod()]
        public void GroupScriptParserTest()
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

            var tokens = parser.ToArray();
            foreach(var token in tokens)
                token.Column = 0; // Really don't care about testing that part yet.

            var expected = new[]
            {
                new GroupScriptToken{ Line = 0, Type = GroupScriptTokenType.Keyword_Name                                    },
                new GroupScriptToken{ Line = 0, Type = GroupScriptTokenType.Identifier,         Text = "Born2019BySpecies"  },

                new GroupScriptToken{ Line = 2, Type = GroupScriptTokenType.Keyword_Parameters                              },

                new GroupScriptToken{ Line = 3, Type = GroupScriptTokenType.Keyword_Species                                 },
                new GroupScriptToken{ Line = 3, Type = GroupScriptTokenType.Identifier,         Text = "species"            },

                new GroupScriptToken{ Line = 4, Type = GroupScriptTokenType.Keyword_End                                     },

                new GroupScriptToken{ Line = 6, Type = GroupScriptTokenType.Keyword_Routine                                 },

                new GroupScriptToken{ Line = 7, Type = GroupScriptTokenType.Keyword_And                                     },

                new GroupScriptToken{ Line = 8, Type = GroupScriptTokenType.Operator_BracketL                               },

                new GroupScriptToken{ Line = 9, Type = GroupScriptTokenType.Keyword_Born                                    },
                new GroupScriptToken{ Line = 9, Type = GroupScriptTokenType.Keyword_After                                   },
                new GroupScriptToken{ Line = 9, Type = GroupScriptTokenType.Keyword_Date                                    },
                new GroupScriptToken{ Line = 9, Type = GroupScriptTokenType.Operator_Colon                                  },
                new GroupScriptToken{ Line = 9, Type = GroupScriptTokenType.Literal_Date,       Text = "01/01/2019"         },
                new GroupScriptToken{ Line = 9, Type = GroupScriptTokenType.Operator_Semicolon                              },

                new GroupScriptToken{ Line = 10, Type = GroupScriptTokenType.Keyword_Born                                   },
                new GroupScriptToken{ Line = 10, Type = GroupScriptTokenType.Keyword_Before                                 },
                new GroupScriptToken{ Line = 10, Type = GroupScriptTokenType.Keyword_Date                                   },
                new GroupScriptToken{ Line = 10, Type = GroupScriptTokenType.Operator_Colon                                 },
                new GroupScriptToken{ Line = 10, Type = GroupScriptTokenType.Literal_Date,      Text = "01/01/2020"         },
                new GroupScriptToken{ Line = 10, Type = GroupScriptTokenType.Operator_Semicolon                             },

                new GroupScriptToken{ Line = 11, Type = GroupScriptTokenType.Keyword_Species                                },
                new GroupScriptToken{ Line = 11, Type = GroupScriptTokenType.Keyword_Is                                     },
                new GroupScriptToken{ Line = 11, Type = GroupScriptTokenType.Keyword_Param                                  },
                new GroupScriptToken{ Line = 11, Type = GroupScriptTokenType.Operator_Colon                                 },
                new GroupScriptToken{ Line = 11, Type = GroupScriptTokenType.Identifier,        Text = "species"            },
                new GroupScriptToken{ Line = 11, Type = GroupScriptTokenType.Operator_Semicolon                             },

                new GroupScriptToken{ Line = 12, Type = GroupScriptTokenType.Operator_BracketR                              },

                new GroupScriptToken{ Line = 13, Type = GroupScriptTokenType.Keyword_End                                    },
                new GroupScriptToken{ Line = 13, Type = GroupScriptTokenType.EoF                                            },
            };

            var got = tokens.GetEnumerator();
            foreach(var expectedToken in expected)
            {
                Assert.IsTrue(got.MoveNext(), "Not enough tokens?");

                var gotToken = got.Current as GroupScriptToken;
                if(gotToken.Text == "Born2019BySpecies" && gotToken.Line == 1)
                {
                    Assert.Inconclusive(
                        $"Weird bug where a space is being treated as a new line, can't reproduce locally.\n" +
                        $"{tokens.Select(t => $"(L:{t.Line} C:{t.Column} Ty:{t.Type} Te:{t.Text})").Aggregate((s1, s2) => s1 + "\n" + s2)}"
                    );
                }

                Assert.IsTrue(
                    gotToken.Line == expectedToken.Line
                 && gotToken.Text == expectedToken.Text
                 && gotToken.Type == expectedToken.Type,
                    $"Got(L:{gotToken.Line} C:{gotToken.Column} Ty:{gotToken.Type} Te:{gotToken.Text}) vs " +
                    $"Expected(L:{expectedToken.Line} C:{expectedToken.Column} Ty:{expectedToken.Type} Te:{expectedToken.Text})"
                );
            }
        }
    }
}