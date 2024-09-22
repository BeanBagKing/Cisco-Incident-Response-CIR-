//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;

namespace Recurity.CommandLineParser.Grouping
{
    class ValidateGroupsStage : ParserStage
    {
        public void Apply(ParserContext parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            GroupValidator validator = new GroupValidator(parser.DefinedOptions);
            validator.Validate(parser.GivenOptions);
        }
    }
}