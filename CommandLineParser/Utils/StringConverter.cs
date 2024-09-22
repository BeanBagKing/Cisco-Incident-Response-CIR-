//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.Collections.Generic;
using System.Reflection;
using Recurity.CommandLineParser.Utils;

namespace Recurity.CommandLineParser.Utils
{
    internal class StringConverter
    {
        private readonly IDictionary<Type, Converter> converters = new Dictionary<Type, Converter>();

        internal StringConverter(IDictionary<Type, Converter> aCustomConverters)
        {
            if (aCustomConverters == null) throw new ArgumentNullException("aCustomConverters");

            converters = aCustomConverters;
        }

        internal StringConverter()
        {
            converters = new Dictionary<Type, Converter>();
        }

        /// <summary>
        /// Checks if the string is convertable into the given type.
        /// </summary>
        /// <param name="aType">the type to convert to.</param>
        /// <param name="aString">the value to convert.</param>
        /// <returns>True if the string can be converted into the given type, otherwise False.</returns>
        internal bool CanConvert(Type aType, string aString)
        {
            try
            {
                return ConvertTo(aType, aString) != null;
            }
            catch (Exception)

            {
                return false;
            }
        }

        /// <summary>
        /// Converts the given string into the provided Type.
        /// If there is more than one possible converter for the given Type
        /// The
        /// </summary>
        /// <param name="aType"></param>
        /// <param name="aString"></param>
        /// <returns></returns>
        internal object ConvertTo(Type aType, string aString)
        {
            if (aType == typeof (string))
                return aString;
            try
            {
                if (converters.ContainsKey(aType))
                    return converters[aType](aString);
                else
                {
                    object retval = ConvertByStringConstructor(aType, aString);
                    return retval ?? Convert.ChangeType(aString, aType);
                }
            }catch(Exception ex)
            {
                throw new ConversionException(string.Format("Can not convert {0} to type {1} - {2}", aString, aType, ex.Message));
            }
        }


        private static object ConvertByStringConstructor(Type aType, string aString)
        {
            ConstructorInfo info = aType.GetConstructor(new Type[] {typeof(string)});
            if (info == null)
                return null;
            try
            {
                return info.Invoke(new object[] {aString});
            }
            catch (Exception)
            {
                return null;
            }
        }

        
    }

    public delegate object Converter(string aString);
}