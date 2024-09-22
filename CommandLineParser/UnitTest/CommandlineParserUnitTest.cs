//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Recurity.CommandLineParser.Attributes;
using Recurity.CommandLineParser.Parser;

namespace Recurity.CommandLineParser 
{
    [TestFixture]
    public class CommandlineParserUnitTest
    {

        public void TestSimpleParse()
        {
            Commandline<SimpleTest> parser = new Commandline<SimpleTest>();
            try
            {
                parser.Parse(null, new SimpleTest());
                Assert.Fail("args must not be null");
            }catch(ArgumentException)
            {
            }

            SimpleTest result = parser.Parse(new string[] { "-two", "helloworld"}, new SimpleTest());
            Assert.AreEqual("helloworld", result.Test2);
            Assert.IsNull(result.Test1, "is optional");
            parser = new Commandline<SimpleTest>();

            result = parser.Parse(new string[] { "-two", "helloworld", "-one", "test.txt" }, new SimpleTest());
            Assert.AreEqual("helloworld", result.Test2);
            Assert.AreEqual(new FileInfo("test.txt").Name, result.Test1.Name);

            try
            {
                result = parser.Parse(new string[] {"-three", "helloworld", "-one", "test.txt"}, new SimpleTest());
                Assert.Fail("Option is not optional");
            }catch(ParseException)
            {
                
            }
        }

        [Test]
        public void TestUsage()
        {
            string[] usageArr = {"Usage: test.exe [OPTIONS]", "This is a test app.", "Example: test.exe -one helloworld -two test", ""
            , "Group [AGroup]"
            , " -one required  bar", ""
            , "Group [BGroup]"
            , " -one required  bar"
            , " -two optional  foo", ""
            , "Group [CGroup]"                
            , " -two optional  foo"};
            StringBuilder builder = new StringBuilder();
            foreach (string s in usageArr)
            {
                builder.Append(s).Append(Environment.NewLine);
            }

            
            Commandline<SimpleTest> parser = new Commandline<SimpleTest>();
            CommandlineUsage usageObj = parser.BuildUsage(new SimpleTest());
            usageObj.Example = "test.exe -one helloworld -two test";
            usageObj.Usage = "test.exe [OPTIONS]";
            usageObj.Description = "This is a test app.";
            String result = parser.Usage(usageObj);
            Assert.AreEqual(builder.ToString().Trim(), result.Trim());
        }

      
        [Test]
        public void TestGroups()
        {
            // TODO
        }


        public class SimpleTest
        {
            private FileInfo testvalue;
            private string two;

            [Option("one", "bar")]
            [Group("AGroup", true)]
            [Group("BGroup", true)]
            public FileInfo Test1
            {
                get { return testvalue; }
                set { testvalue = value; }
            }

            [Option("two", "foo")]
            [Group("BGroup", false)]
            [Group("CGroup", false)]
            public String Test2
            {
                get { return two; }
                set { two = value; }
            }
        }
    }
  
}
