//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;

namespace Recurity.CommandLineParser.Grouping
{
    class GroupValidationException : CommandlineException
    {
        public GroupValidationException(string aMessage, string aReportableMessage, Exception aCause) : base(aMessage, aReportableMessage, aCause)
        {
        }

        public GroupValidationException(string aMessage, string aReportableMessage) : base(aMessage, aReportableMessage)
        {
        }

        public GroupValidationException(string aMessage) : base(aMessage)
        {
        }
    }
}
