//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

namespace Recurity.CommandLineParser.Utils
{
    /// <summary>
    /// Throw if a given argument value can not be converted into the desired type.
    /// </summary>
    internal class ConversionException : CommandlineException
    {
        internal ConversionException(string message):base(message)
        {
        }
    }
}