//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System.Collections.Generic;

namespace Recurity.CommandLineParser.Parser
{
    /// <summary>
    /// Parses the arguments given by the user.
    /// </summary>
    class ParseArgumentsStage : ParserStage
    {
        public void Apply(ParserContext aParserContext)
        {
            ICollection<CommandlineOption> options = aParserContext.DefinedOptions;
            if(options == null)
                throw new StageException("DefinedOptions must not be null in this stage.");
            ArgumentParser parser = new ArgumentParser(options);
            string[] args = aParserContext.Args;
            if (args == null)
                throw new StageException("Args must not be null in this stage");
            ICollection<CommandlineOption> givenOptions = parser.Parse(args);
            aParserContext.GivenOptions = givenOptions;
        }
    }
}
