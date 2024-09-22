//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Recurity.CommandLineParser.Grouping;

namespace Recurity.CommandLineParser.UnitTest.Grouping
{
    [TestFixture]
    public class GroupValidatorUnitTest
    {
        [Test]
        public void TestConstructor()
        {
            try
            {
                GroupValidator validator = new GroupValidator(null);
                Assert.Fail("must not be null");
            }
            catch (ArgumentNullException
                )
            {
            }
        }


        [Test]
        public void TestSatified()
        {
            List<CommandlineOption> list;

            {
                CommandlineOption option =
                    new CommandlineOption("a", typeof (string), "foo", new Group[] {new Group("one", true)});
                GroupValidator validator = new GroupValidator(new CommandlineOption[] {option});
            

                try
                {
                    validator.Satisfied("notrec", new CommandlineOption[] {option}, out list);
                    Assert.Fail("no such group");
                }
                catch (ArgumentException)
                {
                }

                Assert.IsFalse(validator.Satisfied("one", new CommandlineOption[] {}, out list));
                Assert.IsTrue(validator.Satisfied("one", new CommandlineOption[] {option}, out list));
            }
            {
                CommandlineOption g1_req =
                    new CommandlineOption("a", typeof (string), "foo", new Group[] {new Group("one", true)});
                CommandlineOption g1_noreq =
                    new CommandlineOption("e", typeof (string), "foo", new Group[] {new Group("one", false)});
                CommandlineOption g2_req =
                    new CommandlineOption("b", typeof (string), "foo", new Group[] {new Group("two", true)});
                CommandlineOption g2_noreq =
                    new CommandlineOption("d", typeof (string), "foo", new Group[] {new Group("two", false)});
                CommandlineOption g3_req =
                    new CommandlineOption("c", typeof (string), "foo", new Group[] {new Group("three", true)});
                CommandlineOption g3_and_g4 =
                    new CommandlineOption("f", typeof (string), "foo",
                                          new Group[] {new Group("three", true), new Group("four", true)});

               
                CommandlineOption[] options =
                    new CommandlineOption[] {g1_req, g2_req, g3_req, g2_noreq, g1_noreq, g3_and_g4};
                GroupValidator validator = new GroupValidator(options);
                Assert.IsTrue(validator.Satisfied("one", new CommandlineOption[] {g1_req}, out list));
                Assert.IsFalse(validator.Satisfied("one", new CommandlineOption[] {g1_noreq}, out list));
                Assert.AreEqual(1, list.Count);
                AssertUtils.AssertContains(list, g1_req);

                Assert.IsTrue(validator.Satisfied("one", new CommandlineOption[] {g1_noreq, g1_req, g2_noreq}, out list));
                Assert.IsFalse(validator.Satisfied("two", new CommandlineOption[] {g1_noreq, g2_noreq,}, out list));
                Assert.AreEqual(1, list.Count);
                AssertUtils.AssertContains(list, g2_req);


                Assert.IsTrue(validator.Satisfied("three", new CommandlineOption[] {g3_and_g4, g3_req}, out list));
                Assert.IsFalse(validator.Satisfied("three", new CommandlineOption[] { g3_and_g4 }, out list));
                Assert.AreEqual(1, list.Count);
                AssertUtils.AssertContains(list, g3_req);

                Assert.IsTrue(validator.Satisfied("four", new CommandlineOption[] { g3_and_g4 }, out list));
                Assert.IsFalse(validator.Satisfied("four", new CommandlineOption[] {g3_req}, out list));
                Assert.AreEqual(1, list.Count);
                AssertUtils.AssertContains(list, g3_and_g4);
                
            }
        }

        [Test]
        public void TestValidate()
        {
            CommandlineOption g1_req =
                new CommandlineOption("a", typeof (string), "foo", new Group[] {new Group("one", true)});
            CommandlineOption g1_noreq =
                new CommandlineOption("e", typeof (string), "foo", new Group[] {new Group("one", false)});
            CommandlineOption g2_req =
                new CommandlineOption("b", typeof (string), "foo", new Group[] {new Group("two", true)});
            CommandlineOption g2_noreq =
                new CommandlineOption("d", typeof (string), "foo", new Group[] {new Group("two", false)});
            CommandlineOption g3_req =
                new CommandlineOption("c", typeof (string), "foo", new Group[] {new Group("three", true)});
            CommandlineOption g3_and_g4 =
                new CommandlineOption("f", typeof (string), "foo",
                                      new Group[] {new Group("three", true), new Group("four", true)});


            CommandlineOption[] options =
                new CommandlineOption[] {g1_req, g2_req, g3_req, g2_noreq, g1_noreq, g3_and_g4};
            GroupValidator validator = new GroupValidator(options);


            CommandlineOption[] given =
                new CommandlineOption[] {g1_req, g2_req, g3_req, g2_noreq, g1_noreq, g3_and_g4};
            // check for null
            try
            {
                validator.Validate(null);
                Assert.Fail("null must throw argument exc.");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                validator.Validate(given);
            }
            catch (GroupValidationException)
            {
                Assert.Fail("unexpected exception");
            }

            given =
                new CommandlineOption[] {g1_req, g2_req, g3_req, g3_and_g4};
            try
            {
                validator.Validate(given);
            }
            catch (GroupValidationException)
            {
                Assert.Fail("unexpected exception");
            }

            given =
                new CommandlineOption[] {g1_noreq, g3_req, g3_and_g4};
            try
            {
                validator.Validate(given);
            }
            catch (GroupValidationException)
            {
                Assert.Fail("unexpected exception");
            }

            given =
                new CommandlineOption[] {g3_and_g4};
            try
            {
                validator.Validate(given);
                Assert.Fail("group three is not satisfied");
            }
            catch (GroupValidationException)
            {
            }
        }
    }
}