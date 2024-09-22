//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;

namespace Recurity.CommandLineParser.Attributes
{
    class BuilderException : CommandlineException
    {
        public BuilderException(string aMessage, string aHumandReadableMessage) : base(aMessage, aHumandReadableMessage)
        {
            
        }

        public BuilderException(string aMessage)
            : base(aMessage)
        {

        }
    }
}
