//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.Collections.Generic;
using System.Reflection;
using Recurity.CommandLineParser.Grouping;
using Recurity.CommandLineParser.Utils;

namespace Recurity.CommandLineParser.Attributes
{
    internal class OptionBuilder
    {
        private readonly Dictionary<string, CommandlineOption> optionMapping =
            new Dictionary<string, CommandlineOption>();

        internal IDictionary<CommandlineOption, OptionValueSetter> Build(object aObject)
        {
            if (aObject == null) throw new ArgumentNullException("aObject");

            PropertyInfo[] infos =
                aObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Dictionary<CommandlineOption, OptionValueSetter> retval =
                new Dictionary<CommandlineOption, OptionValueSetter>();
            foreach (PropertyInfo info in infos)
            {
                Pair<CommandlineOption, OptionValueSetter> pair = BuildFromPropertyInfo(info, aObject);
                if (pair != null)
                {
                    if (optionMapping.ContainsKey(pair.Head.ShortOption))
                        throw new BuilderException(
                            string.Format("The commandline option {0} is defined more than once", pair.Head.ShortOption));
                    optionMapping.Add(pair.Head.ShortOption, pair.Head);
                    retval.Add(pair.Head, pair.Tail);
                }
            }

            return retval;
        }


        protected internal static Pair<CommandlineOption, OptionValueSetter> BuildFromPropertyInfo(
            PropertyInfo info, object target)
        {
            if (info == null) throw new ArgumentNullException("info");
            object[] attr = info.GetCustomAttributes(typeof (OptionAttribute), false);
            if (attr == null || attr.Length == 0)
                return null;
            if (!info.CanWrite)
                throw new BuilderException(string.Format("Property {0} is not writeable", info));
            if (attr.Length != 1)
                throw new BuilderException("Illegal state more than one attribute specified.");
            OptionAttribute optionAttribute = attr[0] as OptionAttribute;
            if (optionAttribute == null)
                return null;
            if (info.PropertyType != optionAttribute.Type)
                optionAttribute.Type = info.PropertyType;
            Group[] groups =
                GetGroups(info, target);
            CommandlineOption option =
                new CommandlineOption(optionAttribute.ShortOption, optionAttribute.Type,
                                      optionAttribute.Name, groups);

            Pair<CommandlineOption,
                OptionValueSetter> retval =
                    new Pair<CommandlineOption, OptionValueSetter>(option, GetSetter(info, target));
            return retval;
        }

        protected internal static Group[] GetGroups(PropertyInfo info, object target)
        {
            if (info == null) throw new ArgumentNullException("info");
            if (target == null) throw new ArgumentNullException("target");
            // TODO implement this
            GroupAttribute[] attributes = info.GetCustomAttributes(typeof (GroupAttribute), false) as GroupAttribute[];
            Group[] retval = new Group[attributes == null ? 0 : attributes.Length];
            for (uint i = 0; i < retval.Length; i++)
            {
                // stupid waring :)
                GroupAttribute attribute = attributes[i];
                retval[i] = new Group(attribute.Name, attribute.Required);
            }

            return retval;
        }

        private static OptionValueSetter GetSetter(PropertyInfo info, object obj)
        {
            // OptionValueSetter is an closure here -- anonymus method which keeps a reference to
            // the passed object instance 'obj' and 'info'.
            // this is simply an anaonymus setter method 
            return delegate(object e) { info.GetSetMethod(true).Invoke(obj, new object[] {e}); };
        }
    }
}