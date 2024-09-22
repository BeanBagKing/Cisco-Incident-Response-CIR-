//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;

namespace Recurity.CommandLineParser
{
    /// <summary>
    /// This exception is used to provied detailed error information to the user of the Commandline Interface.
    /// Instances of this exception contain a human readable detailed exception message additionally to the actual rather 
    /// technical oriented message.
    /// 
    /// This type of exception enables to provide detailed infromation about the execption condition straight to the user.
    /// </summary>
    public class CommandlineException : Exception
    {
        private readonly string reportableMessage;


        public CommandlineException(string aMessage)
            : base(aMessage)
        {
            if (aMessage == null) throw new ArgumentNullException("aMessage");
            reportableMessage = aMessage;
        }

        public CommandlineException(string aMessage, string aReportableMessage):base(aMessage)
        {
            if (aReportableMessage == null) throw new ArgumentNullException("aReportableMessage");
            reportableMessage = aReportableMessage;
        }

        public CommandlineException(string aMessage, string aReportableMessage, Exception aCause)
            : base(aMessage, aCause)
        {
            if (aReportableMessage == null) throw new ArgumentNullException("aReportableMessage");
            reportableMessage = aReportableMessage;
        }

        /// <summary>
        /// A human readable error message defining the actual problem in a user friendly way.
        /// </summary>
        public string HumanReadableMessage
        {
            get { return reportableMessage; }
        }
    }
}
