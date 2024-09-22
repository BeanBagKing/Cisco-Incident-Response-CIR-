//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;

namespace Recurity.CommandLineParser.Parser
{
    public class ParseException : CommandlineException
    {

        internal ParseException(string message):base(message)
        {
        }
    }
}
