//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.Collections.Generic;

namespace Recurity.CommandLineParser.Grouping
{
    public class Group : IEquatable<Group>, IComparable<Group>
    {
        private readonly string name;

        private readonly bool required;

        public Group(string aName, bool aRequired)
        {
            if (aName == null) throw new ArgumentNullException("aName");
            name = aName;
            required = aRequired;
        }

        public Group(string aName)
        {
            name = aName;
            required = false;
        }

        public string Name
        {
            get { return name; }
        }

        public bool Required
        {
            get { return required; }
        }


        public static bool operator !=(Group group1, Group group2)
        {
            return !Equals(group1, group2);
        }

        public static bool operator ==(Group group1, Group group2)
        {
            return Equals(group1, group2);
        }

        public bool Equals(Group group)
        {
            if (group == null) return false;
            return Equals(name, group.name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Group);
        }

        public override int GetHashCode()
        {
            return name != null ? name.GetHashCode() : 0;
        }

        public int CompareTo(Group other)
        {
            if (other == null) throw new ArgumentNullException("other");
            return name.CompareTo(other.Name);
        }
    }
}