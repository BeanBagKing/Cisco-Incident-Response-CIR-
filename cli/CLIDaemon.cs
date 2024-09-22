//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 
using System;
using System.IO;
using System.Threading;
using log4net;
using Recurity.CIR.Engine.Interfaces;

namespace CLI
{
    /// <summary>
    /// CLIDaemon class acts  very much like a CronJob which processes all 
    /// subdirectories of a configured working directory using the CIR appplication.
    /// If a subdirectory of the working directory contains a core, coreio and image file
    /// the CLIDaemon creates a new CIR application and runs the analysis.
    /// 
    /// In the case of an error or if the directory has already been processed the CLIDaemon 
    /// marks the directory as run and proceeds to the next directory. If there is no directory left
    /// the daemon sleeps for a define time span. 
    /// </summary>
    internal class CLIDaemon
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (CLIDaemon));
        private readonly IPluginConfiguration config;
        private readonly double sleep_time;
        private readonly DirectoryInfo work_directory;

        /// <summary>
        /// Initializes a new CLIDaemon
        /// </summary>
        /// <param name="configuration">the configuration initialized from the App.config</param>
        internal CLIDaemon(IPluginConfiguration configuration)
        {
            String work_directory_path = configuration.RuntimeConfiguration.DaemonWorkDirectory;
            work_directory = new DirectoryInfo(work_directory_path);

            if (!work_directory.Exists)
            {
                throw new ArgumentException("Workdirectory does not exist");
            }
            config = configuration;
            sleep_time = 1;
            LOG.Info("Initialized Daemon");
        }

        private DirectoryInfo[] AllDirectories
        {
            get { return work_directory.GetDirectories(); }
        }

        internal void CleanDirectories()
        {
            DirectoryInfo[] all_directories = AllDirectories;
            foreach (DirectoryInfo info in all_directories)
            {
                Clean(info);
            }
        }

        /// <summary>
        /// Starts up the Daemon.
        /// </summary>
        internal void Run()
        {
            LOG.Info("Starting up Daemon");
            while (true)
            {
                // if we wake up from a sleep we check this again.
                if (CheckKeyboardInterrupt())
                    return;
                DirectoryInfo[] all_directories = AllDirectories;

                foreach (DirectoryInfo info in all_directories)
                {
                    if (LoadNextDirectory(info))
                    {
                        try
                        {
                            using (CliMain main = new CliMain(config))
                            {
                                main.Run();
                            }
                        }
                        catch (Exception ex)
                        {
                            LOG.Error(String.Format("GIR: CIR Run failed {0}", ex.Message), ex);
                            MarkAsRun(info);
                        }
                        // CTRL+C does stop the whole thing
                        if (CheckKeyboardInterrupt())
                            return;
                    }
                }
                // all directories are done
                // put the process to sleep
                WaitUntilNextRun();
            }
        }


        /// <summary>
        /// Removes the report.xml file from the given directory if the file is present.
        /// </summary>
        /// <param name="info"></param>
        private static void Clean(DirectoryInfo info)
        {
            FileInfo[] array = info.GetFiles("report.xml", SearchOption.TopDirectoryOnly);
            if (array.Length > 0)
            {
                array[0].Delete();
            }
        }

        private static bool DidRun(DirectoryInfo info)
        {
            // if there is a report.xml, this one did run
            FileInfo[] array = info.GetFiles("report.xml", SearchOption.TopDirectoryOnly);

            return array.Length > 0;
        }

        private static void MarkAsRun(DirectoryInfo info)
        {
            FileInfo fileinfo = new FileInfo(Path.Combine(info.FullName, "report.xml"));
            if (!fileinfo.Exists)
            {
                LOG.InfoFormat("Mark directory {0} as run.", info.ToString());
                using (FileStream st = fileinfo.Create())
                {
                }
            }
        }

        private static bool CheckKeyboardInterrupt()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo info = Console.ReadKey(true);
                return (info.Key.Equals(ConsoleKey.C) &&
                        info.Modifiers.Equals(ConsoleModifiers.Control));
            }
            return false;
        }

        private void WaitUntilNextRun()
        {
            LOG.InfoFormat("Daemon goes to sleep for {0} minutes", sleep_time);
            Thread.Sleep(TimeSpan.FromMinutes(sleep_time));
        }

        private bool LoadNextDirectory(DirectoryInfo info)
        {
            if (!DidRun(info))
            {
                LOG.InfoFormat("Load next directory {0}", info);
                try
                {
                    // set the core, coreio and image fileinfo 
                    // as well as the outputdirectory to the current directory
                    ResetConfiguration(info);
                    return true;
                }
                catch (Exception ex)
                {
                    // if the config reset or the CLIMain initialization does throw
                    // we do not run this directory but mark it as run
                    MarkAsRun(info);
                    LOG.Error(string.Format("CliMain Initialization failed [{0}]", ex.Message), ex);
                    return false;
                }
            }
            return false;
        }

        private void ResetConfiguration(DirectoryInfo info)
        {
            FileInfo[] core = info.GetFiles("core", SearchOption.TopDirectoryOnly);
            if (core.Length == 0)
            {
                throw new ArgumentException("Directory does not contain a [core] file");
            }

            FileInfo[] image = info.GetFiles("image", SearchOption.TopDirectoryOnly);
            if (image.Length == 0)
            {
                throw new ArgumentException("Directory does not contain a [coreio] file");
            }

            config.CoreFile = core[0];
            config.ElfFile = image[0];
            config.RuntimeConfiguration.OutputFolder = info.FullName;

            //
            // optional coreIO
            //
            FileInfo[] coreio = info.GetFiles("coreio", SearchOption.TopDirectoryOnly);
            if (coreio.Length != 0)
            {
                config.IOCoreFile = coreio[0];
            }
        }
    }
}