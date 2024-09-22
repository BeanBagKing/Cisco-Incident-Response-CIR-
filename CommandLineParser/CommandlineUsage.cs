//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System.Collections.Generic;

namespace Recurity.CommandLineParser
{

    /// <summary>
    /// 
    /// </summary>
    public class CommandlineUsage
    {
        private string usage;
        private string example;
        private string description;
        private ICollection<CommandlineOption> options;

        protected internal CommandlineUsage()
        {
        }

        public string Usage
        {
            get { return usage; }
            set { usage = value; }
        }

        public string Example
        {
            get { return example; }
            set { example = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        internal ICollection<CommandlineOption> Options
        {
            get { return options; }
            set { options = value; }
        }
    }
}