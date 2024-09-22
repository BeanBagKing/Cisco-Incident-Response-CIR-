//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 
using System;
using System.IO;
using Recurity.CommandLineParser.Attributes;

namespace CLI
{
    /// <summary>
    /// This class defines the commandline arguments for the CIR - commandline interface.
    /// An instance of this class is passed to the class CommandLine.OptParse.ParserFactory to build a 
    /// parser dynamically.
    /// 
    /// The Attributes are also used to build a usage message.
    /// </summary>
    public class CLIOptions
    {
        private FileInfo coreIOMem;
        private FileInfo core;
        private FileInfo elf;
        private DirectoryInfo outputPath;
        private bool help;
        private bool daemonFlagSet = false;
        private bool daemon;
        private DirectoryInfo daemonWorkDir;

        
        [Option("i", "The path to the IOS IOcore file.")]
        [Group("InputFiles", false)]
        public FileInfo CoreIOMem
        {
            get { return coreIOMem; }
            set
            {
                if (value == null || !value.Exists)
                    throw new ArgumentException(string.Format("No such file: {0}", value));
                coreIOMem = value;
            }
        }

       
        [Option("c", "The path to the IOS core file.")]
        [Group("InputFiles", true)]
        public FileInfo Core
        {
            get { return core; }
            set
            {
                if(value == null || !value.Exists)
                    throw new ArgumentException(string.Format("No such file: {0}", value));
                core = value;
            }
        }

       
        [Option("e", "The path to the IOS image file.")]
        [Group("InputFiles", true)]
        public FileInfo Elf
        {
            get { return elf; }
            set
            {
                if (value == null || !value.Exists)
                    throw new ArgumentException(string.Format("No such file: {0}", value));
                elf = value;
            }
        }

        [Option("o", "The path to the CIR output directory. This option overrides the option configured in the application configuration.")]
        [Group("Daemon", false)]
        [Group("Standalone", false)]
        public DirectoryInfo OutputPath
        {
            get { return outputPath; }
            set { outputPath = value; }
        }

        
        [Option("h", "Prints this help message.")]
        [Group("Usage")]
        public bool Help
        {
            get { return help; }
            set { help = value; }
        }

        
        [Option("d", "Executes the CIR-CLI as a daemon if set to true. This option overrides the setDaemon option configured in the application configuration.")]
        [Group("Daemon", true)]
        public bool Daemon
        {
            get { return daemon; }
            set
            {
                daemonFlagSet = true;
                daemon = value;
            }
        }

       

        
        [Option("w", "Sets the daemon working directory path if the CIR-CLI is executed in daemon daemon. This option overrides the daemon working directory configured in the application configuration.")]
        [Group("Daemon", false)]
        public DirectoryInfo DaemonWorkDir
        {
            get
            {
                return daemonWorkDir;
            }
            set
            {
        
                daemonWorkDir = value;
            }
        }

        public bool DaemonFlagSet
        {
            get { return daemonFlagSet; }
        }
    }

 
}