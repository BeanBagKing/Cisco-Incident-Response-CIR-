//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.IO;
using NUnit.Framework;
using Recurity.CommandLineParser.Attributes;


namespace Recurity.CommandLineParser { 
    [TestFixture]
    public class ExtractOptionsStageUnitTest
    {

        [Test]
        public void TestStage()
        {
            ExtractOptionsStage stage = new ExtractOptionsStage();
            try
            {
                stage.Apply(null);
                Assert.Fail("must not be null");
            }catch(ArgumentNullException){
            }

            ParserContext ctx = new ParserContext(null, new TestAttributes() ); 
            stage.Apply(ctx);
            Assert.IsNotNull(ctx.OptionMapping);
            Assert.AreEqual(2, ctx.OptionMapping.Count, "must contain 2 configured options");
            
        }
    }

    public class TestAttributes
    {
        private FileInfo testvalue;
        private string two;

        [Option("one", "bar")]
        public FileInfo Test1
        {
            get { return testvalue; }
            set { testvalue = value; }
        }

        [Option("two", "foo")]
        public String Test2
        {
            get { return two; }
            set { two = value; }
        }
    }
}
