//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;

namespace Recurity.CommandLineParser {
    class StageException : Exception
    {
        public StageException(string message):base(message)
        {
        }
    }
}
