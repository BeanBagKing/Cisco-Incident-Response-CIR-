//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.Collections.Generic;
using System.Text;

namespace Recurity.CommandLineParser.Grouping
{
    internal class GroupValidator
    {
        private readonly IDictionary<string, List<CommandlineOption>> mapping =
            new Dictionary<string, List<CommandlineOption>>();

        protected internal GroupValidator(ICollection<CommandlineOption> aDefinition)
        {
            if (aDefinition == null) throw new ArgumentNullException("aDefinition");
            foreach (CommandlineOption option in aDefinition)
            {
                foreach (Group group in option.Groups)
                {
                    if (group.Required)
                    {
                        if (!mapping.ContainsKey(group.Name))
                        {
                            mapping[group.Name] = new List<CommandlineOption>();
                        }
                        mapping[group.Name].Add(option);
                    }
                }
            }
        }

        protected internal void Validate(ICollection<CommandlineOption> aOptions)
        {
            if (aOptions == null) throw new ArgumentNullException("aOptions");
            List<CommandlineOption> missing_options;
            foreach (CommandlineOption option in aOptions)
            {
                foreach (Group group in option.Groups)
                {
                    if (group.Required)
                    {
                        if (!Satisfied(group.Name, aOptions, out missing_options))
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.Append("Group ").Append(group.Name);
                            builder.Append(" is not satisfied. Missing options are: ");
                            builder.Append(Environment.NewLine);

                            foreach (CommandlineOption missing_option in missing_options)
                            {
                                builder.Append("  -");
                                builder.Append(missing_option.ShortOption);
                            }
                            throw new GroupValidationException(builder.ToString());
                        }
                    }
                }
            }
        }

        protected internal bool Satisfied(String group, ICollection<CommandlineOption> option,
                                          out List<CommandlineOption> missing)
        {
            if (!mapping.ContainsKey(group))
                throw new ArgumentException(string.Format("Unrecognized group {0}", group));
            List<CommandlineOption> requiredOptions = mapping[group];
            missing = new List<CommandlineOption>();
            // nothing is required in this group 
            if (requiredOptions.Count == 0)
                return true;
            // if one required option is not present it is not satisfied
            // TODO this could be done more effecient this might run in n square these collection are usually very very small
            bool retval = true;
            foreach (CommandlineOption requiredOption in requiredOptions)
            {
                if (!option.Contains(requiredOption))
                {
                    retval = false;
                    missing.Add(requiredOption);
                }
            }

            return retval;
        }
    }
}