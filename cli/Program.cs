//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Felix Lindner (fx@recurtiy-labs.com)
// 
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using log4net;
using log4net.Config;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginResults;
using Recurity.CommandLineParser;

namespace CLI
{
    internal class Program
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (Program));


        private static void Main(string[] args)
        {
            Console.WriteLine(
                "CIR - Cisco Incident Response. Copyright (c) 2008-2012, Recurity Labs GmbH. All rights reserved");
            try
            {
                XmlConfigurator.Configure();
                CLIOptions cliOptions = ParseCommandline(args);

                if (cliOptions == null || cliOptions.Help)
                {
                    PrintUsage();
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                    return;
                }

                IPluginConfiguration configuration = GetConfiguration(cliOptions);

                if (configuration.RuntimeConfiguration.IsDaemon)
                {
                    if (!RunDaemon(configuration))
                    {
                        Console.WriteLine("Press any key to exit.");
                        Console.ReadKey();
                        return;
                    }
                }
                else
                {
                    if (!RunStandalone(configuration, cliOptions))
                    {
                        PrintUsage();
                        Console.WriteLine("Press any key to exit.");
                        Console.ReadKey();
                        return;
                    }
                }
                LOG.Info("*** all done ***");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            catch (CirConfigurationException ex)
            {
                Console.WriteLine(" Can not start circli.exe:");
                Console.WriteLine("Can not configure circli - {0}", ex.Message);
                Console.WriteLine("Please check you configuration file or commandline option");
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Can not start circli.exe:");
                Console.WriteLine("circli failed. Unexpected Fatal Error {0}", ex);
            }
        }

        private static IPluginConfiguration GetConfiguration(CLIOptions aCLIOptions)
        {
            PluginConfiguration config = new PluginConfiguration();
            if (aCLIOptions.DaemonFlagSet)
                config.RuntimeConfiguration.IsDaemon = aCLIOptions.Daemon;
            if (aCLIOptions.DaemonWorkDir != null)
            {
                config.RuntimeConfiguration.DaemonWorkDirectory = aCLIOptions.DaemonWorkDir.FullName;
            }

            if (aCLIOptions.OutputPath != null)
            {
                config.RuntimeConfiguration.OutputFolder = aCLIOptions.OutputPath.FullName;
            }

            config.Validate();
            return config;
        }

        private static CLIOptions ParseCommandline(string[] args)
        {
            CLIOptions cliOptions = new CLIOptions();

            Commandline<CLIOptions> parser = new Commandline<CLIOptions>();

            try
            {
                return parser.Parse(args, cliOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Can not parse commandline: [{0}]", ex.Message);
                Console.WriteLine();
            }
            return null;
        }

        private static void PrintUsage()
        {
            CLIOptions cliOptions = new CLIOptions();
            PrintUsage(cliOptions);
        }

        private static void PrintUsage(CLIOptions cmd)
        {
            Commandline<CLIOptions> parser = new Commandline<CLIOptions>();
            CommandlineUsage usage = parser.BuildUsage(cmd);
            usage.Description = "CIR -  Cisco Incident Response commandline interface";
            usage.Usage = "circli.exe [options]";
            String result = parser.Usage(usage);
            Console.WriteLine(String.Format("circli.exe\t[Version {0}]",
                                            Assembly.GetExecutingAssembly().GetName().Version.ToString(4)));
            Console.Write(result);
        }

        private static bool RunDaemon(IPluginConfiguration configuration)
        {
            if (LOG.IsInfoEnabled)
            {
                LOG.Info("Starting CIR in [daemon] mode");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            CLIDaemon daemon = new CLIDaemon(configuration);
            try
            {
                daemon.Run();
            }
            catch (Exception ex)
            {
                // this should not happen but we catch it here again just in case
                LOG.Fatal(String.Format("Unrecoverable error caught -- [{0}]", ex.Message), ex);
                return false;
            }
            return true;
        }

        private static FileInfo GetFile(string path)
        {
            if (path == null || path.Length == 0)
                return null;
            try
            {
                return new FileInfo(path);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool RunStandalone(IPluginConfiguration configuration, CLIOptions cliOptions)
        {
            // we shoul introduce something to validate groups for satisfaction
            if (cliOptions.Elf == null || cliOptions.Core == null)
            {
                return false;
            }
            configuration.ElfFile = cliOptions.Elf;
            configuration.CoreFile = cliOptions.Core;
            //
            // coreio is optional
            //
            if (cliOptions.CoreIOMem != null)
            {
                configuration.IOCoreFile = cliOptions.CoreIOMem;
            }

            if (LOG.IsInfoEnabled)
            {
                LOG.Info("Starting CIR in [standalone] mode");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            try
            {
                using (CliMain cli = new CliMain(configuration))
                {
                    cli.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                LOG.FatalFormat("Exiting -- [{0}]", ex.Message);
                return false;
            }
            return true;
        }
    }
}