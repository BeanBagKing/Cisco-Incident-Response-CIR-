//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.Collections.Generic;

namespace Recurity.CommandLineParser.Utils
{
    internal class ConverterStage : ParserStage
    {
        #region ParserStage Members

        public void Apply(ParserContext aParserContext)
        {
            ICollection<CommandlineOption> options = aParserContext.GivenOptions;
            StringConverter converter = aParserContext.Converter;
            if (options == null)
                throw new StageException("GivenOptions must not be null at this stage");
            if (converter == null)
                throw new StageException("Converter must not be null at this stage");


            foreach (CommandlineOption option in options)
            {
                object result = converter.ConvertTo(option.Type, option.Value);
                try
                {
                    aParserContext.OptionValue(option, result);
                }
                catch (Exception ex)
                {
                    throw new ConversionException(
                        string.Format("Failed to set option [{0}] - {1}", option.ShortOption,
                                      ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                }
            }
        }

        #endregion
    }
}