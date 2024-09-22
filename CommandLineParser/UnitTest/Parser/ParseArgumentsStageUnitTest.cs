//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System.Collections.Generic;
using NUnit.Framework;
using Recurity.CommandLineParser.Grouping;

namespace Recurity.CommandLineParser.Parser
{
    [TestFixture]
    public class ParseArgumentsStageUnitTest
    {
        [Test]
        public void TestStage()
        {
            ParseArgumentsStage stage = new ParseArgumentsStage();
            ParserContext ctx = new ParserContext(null, null);
            try
            {
                stage.Apply(ctx);
                Assert.Fail("context has no defined options");
            }
            catch (StageException)
            {
            }

            CommandlineOption a_option = new CommandlineOption("a", typeof (string),  "foo",new Group[0]);
            CommandlineOption b_option = new CommandlineOption("b", typeof(string),  "foo", new Group[0]);
            CommandlineOption c_option = new CommandlineOption("c", typeof(string),  "foo", new Group[0]);
            ctx.OptionMapping = new Dictionary<CommandlineOption, OptionValueSetter>();
            ctx.OptionMapping.Add(a_option, null);
            ctx.OptionMapping.Add(b_option, null);
            ctx.OptionMapping.Add(c_option, null);

            try
            {
                stage.Apply(ctx);
                Assert.Fail("context has no arguments");
            }
            catch (StageException)
            {
            }

            ctx.Args = new string[] {"-a", "valueA", "-b", "valueB", "-c", "valueC"};
            ctx.OptionMapping = new Dictionary<CommandlineOption, OptionValueSetter>();
            ctx.OptionMapping.Add(a_option, null);
            ctx.OptionMapping.Add(b_option, null);
            ctx.OptionMapping.Add(c_option, null);
            stage.Apply(ctx);
            Assert.IsNotNull(ctx.GivenOptions);
            Assert.AreEqual(3, ctx.GivenOptions.Count);
        }
    }
}