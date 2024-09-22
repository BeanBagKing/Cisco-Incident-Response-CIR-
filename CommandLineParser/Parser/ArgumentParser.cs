//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Recurity.CommandLineParser.Parser
{
    /// <summary>
    /// Parses the argument out of the arguments array provided by the applications main routine.
    /// </summary>
    internal class ArgumentParser
    {
        private readonly IDictionary<string, CommandlineOption> shortNamesDict =
            new Dictionary<string, CommandlineOption>();

        private readonly Regex shortOptionInternalRegex = new Regex("^-[a-zA-Z]+");

        // if you change this -- remember to fix the doc sting in namespace Recurity.CommandLineParser.Attributes.OptionAttribute
        private readonly Regex shortOptionRegex = new Regex("[a-zA-Z]+");


        internal ArgumentParser(IEnumerable<CommandlineOption> aOptions)
        {
            if (aOptions == null) throw new ArgumentNullException("aOptions");

            foreach (CommandlineOption option in aOptions)
            {
                string shortOption = option.ShortOption;
                if (shortOption == null)
                    throw new ArgumentException(string.Format("ShortOption {0} is null", option.Name));
                shortOption = shortOption.Trim();
                if (shortOption.Length == 0)
                    throw new ArgumentException(string.Format("ShortOption {0} is empty", option.Name));

                if (!shortOptionRegex.IsMatch(shortOption))
                    throw new OptionFormatException(String.Format("Option {0} has an illegal format", option.Name));
                shortNamesDict.Add(shortOption, option);
            }
        }


        /// <summary>
        /// Parses the given argument array into a collection of CommandlineOption instances.
        /// This method parse all required options from the argument array. Each specifed option 
        /// must also have an value except of options with the type of boolean / bool are interpreted as
        /// True if no value is provided.
        /// </summary>
        /// <param name="args">the argument array given to the main method</param>
        /// <returns>All specified and recognized options from the argument array as CommandlineOption instances.</returns>
        /// <exception cref="ParseException">If an option can not be parsed or does not have a value or an option is requried but not specified.</exception>
        internal ICollection<CommandlineOption> Parse(string[] args)
        {
            CommandlineOption currentOption = null;
            Dictionary<String, CommandlineOption> retval = new Dictionary<String, CommandlineOption>();

            for (uint i = 0; i < args.Length;)
            {
                if (currentOption == null)
                {
                    if (shortOptionInternalRegex.IsMatch(args[i]))
                    {
                        string shortOption = args[i].TrimStart('-');
                        try
                        {
                            currentOption = ExtractOption(shortOption);
                            if (retval.ContainsKey(currentOption.ShortOption))
                                throw new ParseException(
                                    string.Format("The option {0} was supplied more than once.", shortOption));
                            i++;
                        }
                        catch (ArgumentException)
                        {
                            throw new ParseException(string.Format("The option {0} is unknown", shortOption));
                        }
                    }
                    else
                    {
                        throw new ParseException(string.Format("Can not parse option {0} - Illegal Format", args[i]));
                    }
                }
                else
                {
                    uint until;
                    String value = ParseUntil(args, i, out until);

                    if (String.IsNullOrEmpty(value))
                    {
                        if (currentOption.Flag)
                        {
                            value = bool.TrueString;
                        }
                        else
                        {
                            throw new ParseException(string.Format("Missing value for option {0}.", currentOption));
                        }
                    }
                    retval[currentOption.ShortOption] = new CommandlineOption(currentOption, value);
                    i = until;
                    // this must be null here see below.
                    currentOption = null;
                }
            }
            // if the last or only option has no value e.g. is a flag the option is not null
            if(currentOption != null)
            {
                if (currentOption.Flag)
                {
                    retval[currentOption.ShortOption] = new CommandlineOption(currentOption, bool.TrueString);
                }
                else
                {
                    throw new ParseException(string.Format("Missing value for option {0}.", currentOption));
                }

            }
            return retval.Values;
        }

        /// <summary>
        /// Parses the value of the current option until a new option is detected in the argument array.
        /// 
        /// This method can handle values containing arbitray whitespaces if the value is enclosed in
        /// double quotes. if double quotes are part of the value the quote must be escaped by a leading backslash.
        /// </summary>
        /// <param name="args">the argument array</param>
        /// <param name="index">the index of the array to start the parsing from.</param>
        /// <param name="until">the index of the next option in the array or Array.Length if the end of the array was reached.</param>
        /// <returns>A string which represents the parsed value.</returns>
        protected internal string ParseUntil(string[] args, uint index, out uint until)
        {
            if (args == null) throw new ArgumentNullException("args");
            if (index >= args.Length)
                throw new ArgumentException("Index is out of bounds");
            StringBuilder builder = new StringBuilder();
            bool lookForClosingQuote = false;
            until = index;
            // this might apply if the option is a flag
            if (shortOptionInternalRegex.IsMatch(args[index]))
                return "";
            for (uint i = index; i < args.Length; i++)
            {

                if (until != i && !lookForClosingQuote)
                    break;
                
                until = i;
                string current = args[i];
                if (current == null)
                    throw new ArgumentException("Args must not contain null values");
                current = current.Trim();

                if (!lookForClosingQuote && shortOptionInternalRegex.IsMatch(args[i]))
                {
                    break;
                }
                if (builder.Length > 0)
                    builder.Append(" ");

                for (uint j = 0; j < current.Length; j++)
                {
                    char c = current[(int) j];

                    switch (c)
                    {
                        case '"':
                            if (j > 0 && IsEscapeSequence(current, j - 1))
                            {
                                builder.Append(c);
                            }
                            else
                            {
                                lookForClosingQuote = !lookForClosingQuote;
                            }
                            break;
                        case '\\':
                            if (!IsEscapeSequence(current, j))
                            {
                                builder.Append(c);
                            }
                            break;
                        default:
                            builder.Append(c);
                            break;
                    }
                }
            }
            if (lookForClosingQuote)
                throw new ParseException(
                    string.Format("Illegal format. Expected closing quotes in {0}.", builder));

            until++;

            return builder.ToString();
        }

        private static bool IsEscapeSequence(string aSequence, uint offset)
        {
            if (aSequence.Length > offset + 1)
            {
                return aSequence[(int) offset] == '\\' && aSequence[(int) (offset + 1)] == '"';
            }
            return false;
        }

        protected internal CommandlineOption ExtractOption(string aOption)
        {
            if (aOption == null) throw new ArgumentNullException("aOption");

            aOption = aOption.Trim();

            if (shortNamesDict.ContainsKey(aOption))
                return shortNamesDict[aOption];
            throw new ArgumentException(string.Format("The option {0} is unknown.", aOption));
        }
    }
}