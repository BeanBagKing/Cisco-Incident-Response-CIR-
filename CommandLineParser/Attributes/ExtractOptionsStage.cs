//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 
using System;
using System.Collections.Generic;

namespace Recurity.CommandLineParser.Attributes
{

    /// <summary>
    /// Extracts the defined options from the given "option definition" instance.
    /// </summary>
    internal class ExtractOptionsStage : ParserStage
    {
        public void Apply(ParserContext aContenxt)
        {
            if (aContenxt == null) throw new ArgumentNullException("aContenxt");
            object optionDefinition = aContenxt.OptionDefinition;
            if(optionDefinition == null)
                throw new StageException("Illegal state, OptionDefinition is not available.");
            OptionBuilder builder = new OptionBuilder();
            IDictionary<CommandlineOption, OptionValueSetter> dict = builder.Build(optionDefinition);
            aContenxt.OptionMapping = dict;
        }
    }
}