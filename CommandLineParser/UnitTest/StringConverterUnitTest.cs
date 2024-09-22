//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Recurity.CommandLineParser.Utils;

namespace Recurity.CommandLineParser
{
    [TestFixture]
    public class StringConverterUnitTest
    {

        [Test]
        public void TestCanConvert()
        {
            
            StringConverter converter = new StringConverter();
            Assert.IsTrue(converter.CanConvert(typeof (string), "foo"));
            Assert.IsTrue(converter.CanConvert(typeof(Int32), "23"));
            Assert.IsTrue(converter.CanConvert(typeof(float), "-23.4"));
            Assert.IsFalse(converter.CanConvert(typeof(UInt32), "-500"));
            Assert.IsTrue(converter.CanConvert(typeof(FileInfo), "/foo"));
            Assert.IsFalse(converter.CanConvert(typeof(FileInfo), "-?1234"));
        }

        [Test]
        public void TestConvert()
        {
            StringConverter converter = new StringConverter();
            Assert.AreEqual("foo", converter.ConvertTo(typeof(string), "foo"));
            Assert.AreEqual(23, converter.ConvertTo(typeof(Int32), "23"));
            Assert.AreEqual(-23.4, (float)converter.ConvertTo(typeof(float), "-23.4"),0.001);
            Assert.AreEqual(new FileInfo("/foo").FullName, ((FileInfo)converter.ConvertTo(typeof(FileInfo), "/foo")).FullName);
            try
            {
                converter.ConvertTo(typeof (UInt32), "-500");
                Assert.Fail("Can not convert negative value to unsigned int");
            }catch(ConversionException)
            {
                
            }

            try
            {
                converter.ConvertTo(typeof (FileInfo), "-?1234");
                Assert.Fail("Can not convert illegal path");
            }
            catch (ConversionException)
            {

            }
        }

        [Test]
        public void TestCustomConverter()
        {
            IDictionary<Type, Converter> converterMap= new Dictionary<Type, Converter>();
            converterMap[typeof (ConversionTester)] = CustomConverter;
            StringConverter converter = new StringConverter(converterMap);
            Assert.IsNotNull(converter.ConvertTo(typeof(ConversionTester), "foo"));
            Assert.AreSame(typeof(ConversionTester), converter.ConvertTo(typeof(ConversionTester), "foo").GetType());
            Assert.AreEqual("foo", ((ConversionTester)converter.ConvertTo(typeof(ConversionTester), "foo")).TheValue);
            
        }

        public static object CustomConverter(string aString)
        {
            ConversionTester t = new ConversionTester();
            t.TheValue = aString;
            return t;
        }
    }

    internal class ConversionTester
    {
        private string thevalue;

        internal String TheValue
        {
            get { return thevalue; }
            set { thevalue = value; }
        }
    }
}
