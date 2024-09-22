//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurity-labs.com)
// 

using System;
using System.Collections.Generic;
using System.Text;
using Recurity.CommandLineParser.Attributes;
using Recurity.CommandLineParser.Grouping;
using Recurity.CommandLineParser.Parser;
using Recurity.CommandLineParser.Utils;

namespace Recurity.CommandLineParser
{
    /// <summary>
    /// Commandline represents a simple and easy to use interface for parsing options passes as commandline arguments to an application.
    ///
    /// To parse options from the commandline array a option definition is required and must be user defined. The option definition is done 
    /// in an object oriented way using C# features like properties and attributes. The option definition is represented by a class which defines public
    /// Read / Write Properties annotated with an <see cref="Recurity.CommandLineParser.Attributes.OptionAttribute"/> Attribute. An instance of this class
    /// is passed to the Parse method together with the string array which holds the actual options given to the main method of the application.
    /// 
    /// The same object can be used to generate a usage string to print to the console in the case of an exception or if the usage is requested.
    /// Both methods, Parse as well as BuildUsage will throw Exceptions of the type CommandlineException. This type of exception provides a human readable string
    /// which explains the exception e.g. the cause of the exception in a user frienly way. This message can be used to print to the commandline.
    /// </summary>
    /// 
    /// <example>
    ///   Commandline<SomeOptionDef> cmd = new Commandline<SomeOptionDef>();
    ///   SomeOptionDef definition = new SomeOptionDef();
    ///   SomeOptionDef result = null
    ///   try{
    ///     null = cmd.Parse(args, definition);
    ///   }catch(CommandlineException ex){
    ///     //  print usage
    ///     return;
    ///   }
    ///   if(result.Usage){
    ///     CommandlineUsage usage = cmd.BuildUsage(definition);
    ///     usage.Example = "test.exe -o -h ";
    ///     usage.Usage = "test.exe [OPTIONS]";
    ///     usage.Description = "This is a test application";
    ///     Console.WriteLine(cmd.Usage(usage));
    ///   }
    ///   
    /// </example>
    /// <typeparam name="T"></typeparam>
    public class Commandline<T> where T : class
    {
        private readonly StringConverter converter;
        private readonly ParserStage[] stages;

        /// <summary>
        /// Creates a new Commandline instance. 
        /// This constructor takes a dictionary with custom converters for class types.
        /// The converter is used to convert the string from the commanline options into the
        /// desired type. This could be used to perform some custom checking of input values or event to
        /// instanciate types without an string constructor using a closure.
        /// </summary>
        /// <param name="aCustomConverters">the converter mapping</param>
        public Commandline(IDictionary<Type, Converter> aCustomConverters)
        {
            Init(out converter, out stages, aCustomConverters);
        }

        /// <summary>
        /// Creates a new Commandline instance.
        /// </summary>
        public Commandline()
        {
            Init(out converter, out stages, new Dictionary<Type, Converter>());
        }

        private static void Init(out StringConverter aConverter, out ParserStage[] aStages,
                                 IDictionary<Type, Converter> aCustomConverters)
        {
            aConverter = new StringConverter(aCustomConverters);
            aStages =
                new ParserStage[]
                    {
                        new ExtractOptionsStage(), new ParseArgumentsStage(), new ValidateGroupsStage(),
                        new ConverterStage()
                    };
        }


        /// <summary>
        /// Parses the given commandline arguments according to the given option definition instance.
        /// </summary>
        /// <param name="args">the commandline arguments given to the main method.</param>
        /// <param name="aOptionDefinition">An object with valid option definitions.</param>
        /// <returns>The given instance of T filled with the provided arguments.</returns>
        /// <exception cref="CommandlineException">If the commanline can not be parsed or not all required options are provided.</exception>
        public T Parse(string[] args, T aOptionDefinition)
        {
            if (args == null) throw new ArgumentNullException("args");
            if (aOptionDefinition == null) throw new ArgumentNullException("aOptionDefinition");
            ParserContext context = new ParserContext(converter, aOptionDefinition);
            context.Args = args;
            foreach (ParserStage stage in stages)
            {
                stage.Apply(context);
            }

            return aOptionDefinition;
        }


        /// <summary>
        /// Builds a usage instance from the given option definition.
        /// </summary>
        /// <param name="aOptionDefinition">the option definition object.</param>
        /// <returns>a new CommanlineUsage instance</returns>
        /// <exception cref="CommandlineException">if the option definition contains errors und inconsistencies.</exception>
        public CommandlineUsage BuildUsage(T aOptionDefinition)
        {
            if (aOptionDefinition == null) throw new ArgumentNullException("aOptionDefinition");
            ExtractOptionsStage stage = new ExtractOptionsStage();
            ParserContext context = new ParserContext(converter, aOptionDefinition);
            stage.Apply(context);
            CommandlineUsage usage = new CommandlineUsage();
            usage.Options = context.DefinedOptions;
            return usage;
        }

        
        /// <summary>
        /// Builds a usage string from the provided CommandlineUsage instance.
        /// </summary>
        /// <param name="aUsage">the usage instance</param>
        /// <returns>a String representation of the usage instance.</returns>
        public String Usage(CommandlineUsage aUsage)
        {
            if (aUsage == null) throw new ArgumentNullException("aUsage");
            StringBuilder builder = new StringBuilder();
            builder.Append("Usage: ").Append(aUsage.Usage).Append(Environment.NewLine);
            if (aUsage.Description != null)
                builder.Append(aUsage.Description).Append(Environment.NewLine);
            if (aUsage.Example != null)
                builder.Append("Example: ").Append(aUsage.Example).Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            uint maxLen = GetMaxOptionLength(aUsage.Options);

            foreach (KeyValuePair<Group, List<Pair<CommandlineOption, Group>>> groupOptions in CreateGroupToOptionMapping(aUsage))
            {
                builder.Append("Group [").Append(groupOptions.Key.Name).Append("]").Append(Environment.NewLine);
                groupOptions.Value.Sort(CompareOptionGroupPair);
                foreach (Pair<CommandlineOption, Group> pair in groupOptions.Value)
                {
                    builder.Append(" ");
                    builder.Append(FormatOption(pair.Head, pair.Tail, maxLen)).Append(Environment.NewLine);
                }
                builder.Append(Environment.NewLine);
            }

            return builder.ToString();
        }

        private static Dictionary<Group, List<Pair<CommandlineOption, Group>>> CreateGroupToOptionMapping(CommandlineUsage aUsage)
        {
            Dictionary<Group, List<Pair<CommandlineOption, Group>>> groupMapping = new Dictionary<Group, List<Pair<CommandlineOption, Group>>>();
            foreach (CommandlineOption option in aUsage.Options)
            {
                foreach(Group group in option.Groups)
                {
                    if(!groupMapping.ContainsKey(group))
                    {
                        groupMapping[group] = new List<Pair<CommandlineOption, Group>>();
                    }
                    groupMapping[group].Add(new Pair<CommandlineOption, Group>(option, group));
                
                }
            }
            return groupMapping;
        }

        private static uint GetMaxOptionLength(IEnumerable<CommandlineOption> option)
        {
            uint len = 0;
            foreach (CommandlineOption commandlineOption in option)
            {
                if (commandlineOption.ShortOption.Length > len)
                    len = (uint) commandlineOption.ShortOption.Length;
            }
            return len;
        }

        private static String FormatOption(CommandlineOption option, Group aGroup, uint maxLen)
        {
            uint whitespaces = (uint) (maxLen - option.ShortOption.Length);
            StringBuilder builder = new StringBuilder();
            builder.Append("-");
            builder.Append(option.ShortOption);
            for (uint i = 0; i < whitespaces; i++)
                builder.Append(" ");
            builder.Append(" ");
            builder.Append(aGroup.Required ? "required" : "optional" );
            builder.Append(" ");
            builder.Append(" ");
            builder.Append(option.Name);
            return builder.ToString();
        }

        private static int CompareOptionGroupPair(Pair<CommandlineOption, Group> left, Pair<CommandlineOption, Group> right)
        {
            return left.Head.CompareTo(right.Head);
        }

        
    }
}