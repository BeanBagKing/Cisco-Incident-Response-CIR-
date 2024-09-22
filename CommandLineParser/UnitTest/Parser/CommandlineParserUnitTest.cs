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

namespace Recurity.CommandLineParser.Parser
{
    [TestFixture]
    public class CommandlineParserUnitTest
    {
        private static CommandlineOption Get(ICollection<CommandlineOption> result, CommandlineOption option)
        {
            foreach (CommandlineOption commandlineOption in result)
            {
                if (commandlineOption.Name.Equals(option.Name))
                    return commandlineOption;
            }
            return null;
        }

        [Test]
        public void TestConstructor()
        {
            CommandlineOption valid_option = new CommandlineOption("a", typeof(string),  "foo", new Group[0]);
            CommandlineOption empty = new CommandlineOption("  ", typeof(string), "foo", new Group[0]);
            CommandlineOption invalidOption = new CommandlineOption("--", typeof(string), "foo", new Group[0]);

            // valid option 
            ArgumentParser parser = new ArgumentParser(new CommandlineOption[] { valid_option });

            try
            {
                parser = new ArgumentParser(new CommandlineOption[] { empty });
                Assert.Fail("Expected Argument exception");
            }
            catch (ArgumentException)
            {
            }
            

            try
            {
                parser = new ArgumentParser(new CommandlineOption[] { invalidOption });
                Assert.Fail("Expected Formated exception");
            }
            catch (OptionFormatException)
            {
            }
        }

        [Test]
        public void TestExtractOptions()
        {
            CommandlineOption a_option = new CommandlineOption("a", typeof(string), "foo", new Group[0]);
            CommandlineOption b_option = new CommandlineOption("b", typeof(string),  "foo", new Group[0]);
            CommandlineOption c_option = new CommandlineOption("c", typeof(string),  "foo", new Group[0]);
            ArgumentParser parser = new ArgumentParser(new CommandlineOption[] { a_option, b_option, c_option });
            CommandlineOption extractedOption = parser.ExtractOption("a");
            Assert.AreSame(a_option, extractedOption);
            extractedOption = parser.ExtractOption("b");
            Assert.AreSame(b_option, extractedOption);
            extractedOption = parser.ExtractOption("c");
            Assert.AreSame(c_option, extractedOption);

            try
            {
                extractedOption = parser.ExtractOption("-a");
                Assert.Fail("Illegal option -- not recognized");
            }
            catch (ArgumentException)
            {
            }
        }

        [Test]
        public void TestParse()
        {
            CommandlineOption a_option = new CommandlineOption("a", typeof(string), "A", new Group[0]);
            CommandlineOption b_option = new CommandlineOption("b", typeof(string), "B", new Group[0]);
            CommandlineOption c_option = new CommandlineOption("c", typeof(string), "C", new Group[0]);
            string[] args = new string[] { "-a", "valueA", "-b", "valueB", "-c", "valueC" };
            ArgumentParser parser = new ArgumentParser(new CommandlineOption[] { a_option, b_option, c_option });
            ICollection<CommandlineOption> result = parser.Parse(args);
            Assert.AreEqual(3, result.Count);

            Assert.IsNotNull(Get(result, a_option));
            Assert.IsNotNull(Get(result, b_option));
            Assert.IsNotNull(Get(result, c_option));
            Assert.AreEqual("valueA", Get(result, a_option).Value);
            Assert.AreEqual("valueB", Get(result, b_option).Value);
            Assert.AreEqual("valueC", Get(result, c_option).Value);


            args = new string[] { "-a", "foo", "-notrec", "bar" };
            try
            {
                parser.Parse(args);
                Assert.Fail("Unknown optino -notrec");
            }
            catch (ParseException)
            {
            }

            args = new string[] { "-a", "foo", "-a", "bar" };
            try
            {
                parser.Parse(args);
                Assert.Fail("option specified twice");
            }
            catch (ParseException)
            {
            }


           
            CommandlineOption help = new CommandlineOption("h", typeof(bool), "HELP", new Group[] { new Group("help", true) });
            CommandlineOption output = new CommandlineOption("o", typeof(DirectoryInfo), "Output", new Group[] { new Group("b", false) });
            CommandlineOption elf = new CommandlineOption("e", typeof(FileInfo), "Elf", new Group[] { new Group("a", true) });
            CommandlineOption iomem = new CommandlineOption("i", typeof(FileInfo), "IOMem", new Group[] { new Group("a", false) });
            CommandlineOption core = new CommandlineOption("c", typeof(FileInfo), "Core", new Group[] { new Group("a", true) });
            args = new string[] { "-h", "true", "-o", @"c:\temp\", "-e", @"\\Tonne\CISCO\CIR_Test_Cases\newtestcases\c2600\c2600-entbase-mz.124-13\image\c2600-entbase-mz.124-13.bin", "-c", @"\\Tonne\CISCO\CIR_Test_Cases\newtestcases\c2600\c2600-entbase-mz.124-13\core\c2600-entbase-mz.124-13.core", "-i", @"\\Tonne\CISCO\CIR_Test_Cases\newtestcases\c2600\c2600-entbase-mz.124-13\core\c2600-entbase-mz.124-13.coreiomem" };
            parser = new ArgumentParser(new CommandlineOption[] { help, output, elf, iomem, core });
            ICollection<CommandlineOption> options =  parser.Parse(args);
            AssertUtils.AssertContains((ICollection) options, help);
            AssertUtils.AssertContains((ICollection)options, output);
            AssertUtils.AssertContains((ICollection)options, elf);
            AssertUtils.AssertContains((ICollection)options, iomem);
            AssertUtils.AssertContains((ICollection)options, core);

            parser = new ArgumentParser(new CommandlineOption[] { help, output });
            args = new string[] {"-h", "-o", @"c:\temp\"};
            options = parser.Parse(args);
            AssertUtils.AssertContains((ICollection)options, help);
            AssertUtils.AssertContains((ICollection)options, output);


            parser = new ArgumentParser(new CommandlineOption[] { help, output });
            args = new string[] { "-h"};
            options = parser.Parse(args);
            AssertUtils.AssertContains((ICollection)options, help);
            
        }

        [Test]
        public void TestParseUntil()
        {
            ArgumentParser parser = new ArgumentParser(new CommandlineOption[] { });
            string[] args = new string[] { };

            uint until;
            try
            {
                parser.ParseUntil(args, 1, out until);
                Assert.Fail("Index is out of bounds");
            }
            catch (ArgumentException)
            {
            }

            try
            {
                parser.ParseUntil(null, 1, out until);
                Assert.Fail("args are null");
            }
            catch (ArgumentException)
            {
            }
            args = new string[] { "-a" };
            Assert.IsEmpty(parser.ParseUntil(args, 0, out until));
            Assert.AreEqual(0, until);
            args = new string[] { "sir", "gallahad" };
            Assert.AreEqual("sir", parser.ParseUntil(args, 0, out until), "Must parse until gallahad");
            Assert.AreEqual(1, until);

            args = new string[] { "sir", "gallahad", "-foo" };
            Assert.AreEqual("sir", parser.ParseUntil(args, 0, out until), "Must parse until next arg");
            Assert.AreEqual(1, until);

            args = new string[] { "\"sir", "gallahad-thebrave\"", "-foo" };
            Assert.AreEqual("sir gallahad-thebrave", parser.ParseUntil(args, 0, out until),
                            "Must parse until closing quotes");
            Assert.AreEqual(2, until);

            args = new string[] { "-bar", "\"sir", "gallahad-thebrave\"", "-foo" };
            Assert.AreEqual("sir gallahad-thebrave", parser.ParseUntil(args, 1, out until),
                            "Must parse until closing quotes");
            Assert.AreEqual(3,
                            until);

            args = new string[] { "hello", "world\\\"hi\\\"" };
            Assert.AreEqual("world\"hi\"", parser.ParseUntil(args, 1, out until),
                            "Must parse until closing quotes");
            Assert.AreEqual(2,
                            until);

            args = new string[] { "hello", "world\\\"hi\"" };
            try
            {
                parser.ParseUntil(args, 1, out until);
                Assert.Fail("unexpected ending quotes");
            }
            catch (ParseException)
            {
            }


            
        }
    }
}