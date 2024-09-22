//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Recurity.CommandLineParser.Grouping;
using Recurity.CommandLineParser.UnitTest;
using Recurity.CommandLineParser.Utils;

namespace Recurity.CommandLineParser.Attributes
{
    [TestFixture]
    public class OptionBuilderUnitTest
    {
        private object testvalue;
        public string TestWithout
        {
            get { return ""; }
        }

        [Option("tro", "TestRO")]
        public string TestRO
        {
            get { return ""; }
        }

        [Option("trw", "TestRW")]
        public string TestRW
        {
            get { return ""; }
            set { testvalue = value;}
        }

        [Option("trwtype", "TestRWType")]
        
        public FileInfo TestRWType
        {
            get { return new FileInfo("."); }
            set { testvalue = value; }
        }

        [Test]
        public void TestBuildFromInstance()
        {
            OptionBuilder builder = new OptionBuilder();
            IDictionary<CommandlineOption, OptionValueSetter> dict = builder.Build(new TestAttributes());
            Assert.IsNotNull(dict, "must be not null");
            Assert.IsNotEmpty((ICollection)dict);
            Assert.AreEqual(2, dict.Count, "two properties must be found");
            //  [Option("one", "bar")]
            Assert.IsTrue(dict.ContainsKey(new CommandlineOption("one",typeof(FileInfo), "bar", new Group[0])));
            //  [Option("one", "foo")]
            Assert.IsTrue(dict.ContainsKey(new CommandlineOption("two", typeof(string),  "foo", new Group[0])));
            
            try
            {
                dict = builder.Build(new TestAttributesDoubleKey());
                Assert.Fail("Two properties must not have the same short option");
            }catch(BuilderException)
            {
            }

            foreach (CommandlineOption key in dict.Keys)
            {
                // check if groups are set.
                Assert.IsNotEmpty(key.Groups);
            }
        }

        [Test]
        public void TestBuildFromPropertyInfo()
        {
            Pair<CommandlineOption, OptionValueSetter> retval = null;
            try
            {
                retval =
                    OptionBuilder.BuildFromPropertyInfo(GetType().GetProperty("TestRO"), this);
                Assert.Fail("Property must be writeable");
            }
            catch (BuilderException)
            {
            }


            retval =
                OptionBuilder.BuildFromPropertyInfo(GetType().GetProperty("TestWithout"), this);
            Assert.IsNull(retval);


            retval =
                OptionBuilder.BuildFromPropertyInfo(GetType().GetProperty("TestRW"), this);
            Assert.IsNotNull(retval);
            Assert.IsNotNull(retval.Tail);
            Assert.AreEqual("TestRW", retval.Head.Name);
            Assert.AreEqual("trw", retval.Head.ShortOption);
            
            Assert.AreEqual(typeof(string), retval.Head.Type);
            

            testvalue = null;
            retval.Tail(".");
            Assert.IsNotNull(testvalue);
            Assert.AreEqual(".", testvalue);
            

            retval =
                OptionBuilder.BuildFromPropertyInfo(GetType().GetProperty("TestRWType"), this);
            Assert.IsNotNull(retval);
            Assert.IsNotNull(retval.Tail);
            Assert.AreEqual("TestRWType", retval.Head.Name);
            Assert.AreEqual("trwtype", retval.Head.ShortOption);
            
            Assert.AreEqual(typeof(FileInfo), retval.Head.Type);
            

            testvalue = null;
            retval.Tail(new FileInfo("."));
            Assert.IsNotNull(testvalue);
            Assert.AreEqual(typeof(FileInfo), testvalue.GetType());
        }

        [Test]
        public void TestGetGroups()
        {
            try
            {
                OptionBuilder.GetGroups(null, new TestAttribute());
                Assert.Fail("info is null -- expected exception");
            }catch(ArgumentException)
            {
            }
            try
            {
                OptionBuilder.GetGroups(typeof(TestAttributes).GetProperty("Test1"), null);
                Assert.Fail("object is null -- expected exception");
            }
            catch (ArgumentException)
            {
            }


            Group[] groups = OptionBuilder.GetGroups(typeof (TestAttributes).GetProperty("Test1"), new TestAttribute());
            AssertUtils.AssertContains(groups, new Group("gr", false));
            AssertUtils.AssertContains(groups, new Group("gr1", true));

            groups = OptionBuilder.GetGroups(typeof(TestAttributes).GetProperty("Test2"), new TestAttribute());
            AssertUtils.AssertContains(groups, new Group("gr", true));
            AssertUtils.AssertContains(groups, new Group("gr3", true));

            groups = OptionBuilder.GetGroups(typeof(TestAttributesDoubleKey).GetProperty("Test1"), new TestAttributesDoubleKey());
            Assert.IsEmpty(groups);
        }

    }

    public class TestAttributes
    {
        private FileInfo testvalue;
        private string two;

        [Option("one", "bar")]
        [Group("gr", false)]
        [Group("gr1", true)]
        public FileInfo Test1
        {
            get { return testvalue; }
            set { testvalue = value; }
        }

        [Option("two", "foo")]
        [Group("gr", true)]
        [Group("gr3", true)]
        public String Test2
        {
            get { return two; }
            set { two = value; }
        }
    }

    public class TestAttributesDoubleKey
    {
        private FileInfo testvalue;
        private string two;

        [Option("one", "bar")]
        public FileInfo Test1
        {
            get { return testvalue; }
            set { testvalue = value; }
        }

        // both have the same key
        [Option("one", "foo")]
        public String Test2
        {
            get { return two; }
            set { two = value; }
        }
    }
}