//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using Recurity.CommandLineParser.Grouping;

namespace Recurity.CommandLineParser.Attributes
{
    /// <summary>
    /// The OptionAttribute represents a very natural way of defining commandline options for C# developer.
    /// Is is used to annotate class properies in a option definition class to define type, short option, the groupd
    /// and several other properties of an option definition. 
    /// This attribute is simply added to an public read / write C# property of an arbitary class. The type of property must either 
    /// provide a public constructor with a single string argument, be convertable by a System.Convert or by one of the provided custom converter.
    /// </summary>
    /// <example>
    /// 
    /// [Option("c", "ConfigurationFile", Description = "A path to the main configuration file")]
    /// [Group("configuration", true)]
    /// [Group("base", false)]
    /// public FileInfo Config {
    ///   get {
    ///     return config;
    ///   }
    ///   set {
    ///     config = value;
    ///   }
    /// 
    /// }
    /// </example>
    [AttributeUsage(
        AttributeTargets.Property,
        AllowMultiple = false)]
    public class OptionAttribute : Attribute
    {
        private readonly string name;
        private readonly string shortOption;

        private string description;
        private Type type;

        /// <summary>
        /// Creates a new OptionAttribute
        /// </summary>
        /// <param name="aShortOption"></param>
        /// <param name="aName"></param>
        public OptionAttribute(string aShortOption, string aName)
        {
            name = aName;
            shortOption = aShortOption;
            type = typeof (string);
            description = "";
        }



        /// <summary>
        /// The description used in the usage generation.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

         /// <summary>
        /// Explicite definition of the option type.
        /// This is optional and will be detected automatically.
        /// </summary>
        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// The name of this option.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The actual short option which has to be provided by the user via the commandline.
        /// The value given to this property is prefixed with a '-' such that the short option 'foo'
        /// is specified by '-foo' on the commandline.
        /// Short options must match this pattern [a-zA-Z]+
        /// </summary>
        public string ShortOption
        {
            get { return shortOption; }
        }
    }
}