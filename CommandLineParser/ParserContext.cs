//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System.Collections.Generic;
using Recurity.CommandLineParser.Grouping;
using Recurity.CommandLineParser.Utils;

namespace Recurity.CommandLineParser
{
    internal class ParserContext
    {
        private readonly StringConverter converter;
        private readonly object optionDefinition;
        private string[] args;
        private ICollection<CommandlineOption> givenOptions;
        private IDictionary<CommandlineOption, OptionValueSetter> optionMapping;

        internal ParserContext(StringConverter converter, object aPropertyObject)
        {
            this.converter = converter;
            optionDefinition = aPropertyObject;
        }


        internal ICollection<CommandlineOption> DefinedOptions
        {
            get { return optionMapping == null ? null : optionMapping.Keys; }
        }

        public string[] Args
        {
            get { return args; }
            set { args = value; }
        }

        internal ICollection<CommandlineOption> GivenOptions
        {
            get { return givenOptions; }
            set { givenOptions = value; }
        }

        internal StringConverter Converter
        {
            get { return converter; }
        }

        public object OptionDefinition
        {
            get { return optionDefinition; }
        }

        public IDictionary<CommandlineOption, OptionValueSetter> OptionMapping
        {
            get { return optionMapping; }
            set { optionMapping = value; }
        }

        internal void OptionValue(CommandlineOption option, object result)
        {
            if (optionMapping.ContainsKey(option))
            {
                optionMapping[option](result);
            }
        }

        
    }


    internal delegate void OptionValueSetter(object o);
}