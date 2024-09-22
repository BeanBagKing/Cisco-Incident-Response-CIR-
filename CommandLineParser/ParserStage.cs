//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

namespace Recurity.CommandLineParser
{

    /// <summary>
    /// ParserStage represents an independent step in the parsing process. Each step works on the data provided by the ParserContext and
    /// might add information to the context during execution.
    /// </summary>
    interface ParserStage
    {
        /// <summary>
        /// Executes the stage
        /// </summary>
        /// <param name="parser">a parser context instance.</param>
        void Apply(ParserContext parser);
    }
}