//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using Recurity.CommandLineParser.Grouping;

namespace Recurity.CommandLineParser
{
    internal class CommandlineOption : IEquatable<CommandlineOption>, IComparable<CommandlineOption>
    {
        private readonly Group[] groups;
        private readonly string name;
        
        private readonly string shortOption;
        private readonly Type type;
        private readonly string optionValue;

        internal CommandlineOption(string aShortOption, Type aType, string aName, Group[] aGroups)
        {
            if (aShortOption == null) throw new ArgumentNullException("aShortOption");
            if (aType == null) throw new ArgumentNullException("aType");
            if (aName == null) throw new ArgumentNullException("aName");
            if (aGroups == null) throw new ArgumentNullException("aGroups");
            shortOption = aShortOption;
            groups = aGroups;
            name = aName;
            type = aType;
            
        }

        internal CommandlineOption(CommandlineOption other, string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            shortOption = other.ShortOption;
            groups = other.groups;
            name = other.name;
            
            type = other.type;
            optionValue = value;
        }

        internal string Value
        {
            get { return optionValue; }
           
        }

        internal bool IsSet
        {
            get { return optionValue != null; }
        }


        internal string ShortOption
        {
            get { return shortOption; }
        }

        internal Type Type
        {
            get { return type; }
        }

        internal Group[] Groups
        {
            get { return groups; }
        }

        internal string Name
        {
            get { return name; }
        }

        internal bool Flag
        {
            get { return type == typeof (bool) || type == typeof (Boolean); }
        }

        public override string ToString()
        {
            return string.Format("[{0}], option: -{1}", Name, ShortOption);
        }

        public static bool operator !=(CommandlineOption commandlineOption1, CommandlineOption commandlineOption2)
        {
            return !Equals(commandlineOption1, commandlineOption2);
        }

        public static bool operator ==(CommandlineOption commandlineOption1, CommandlineOption commandlineOption2)
        {
            return Equals(commandlineOption1, commandlineOption2);
        }

        public bool Equals(CommandlineOption commandlineOption)
        {
            if (commandlineOption == null) return false;
            if (!Equals(name, commandlineOption.name)) return false;
            if (!Equals(shortOption, commandlineOption.shortOption)) return false;
            if (!Equals(type, commandlineOption.type)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as CommandlineOption);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = name != null ? name.GetHashCode() : 0;
                result = 29*result + (shortOption != null ? shortOption.GetHashCode() : 0);
                result = 29*result + (type != null ? type.GetHashCode() : 0);
                return result;
            }
        }

        public int CompareTo(CommandlineOption other)
        {
            if (other == null) throw new ArgumentNullException("other");

            return shortOption.CompareTo(other.shortOption);
        }
    }
}