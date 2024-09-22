// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

namespace Recurity.CommandLineParser.Attributes
{
    [AttributeUsage(
      AttributeTargets.Property,
      AllowMultiple = true)]
    public class GroupAttribute : Attribute
    {
        private readonly string name;
        private readonly bool required;

        public GroupAttribute(string aName, bool aRequired)
        {
            if (aName == null) throw new ArgumentNullException("aName");
            name = aName;
            required = aRequired;
        }

        public GroupAttribute(string aName)
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
    }
}
