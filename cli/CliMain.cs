//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Felix Lindner (fx@recurtiy-labs.com)
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;
using PluginHost;
using PluginHost.PluginValidation;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Executor;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.PluginResults;
using Recurity.CIR.Engine.PluginResults.Xml;
using Recurity.CIR.Engine.Report;
using Recurity.PluginHost;

namespace CLI
{
    public class CliMain : IDisposable
    { 
        private static readonly ILog LOG = LogManager.GetLogger(typeof (CliMain));
        private readonly IPluginConfiguration configuration;

        private readonly List<IDisposable> disposeable_resources = new List<IDisposable>();
        private readonly ReportSerializer report_serializer = new ReportSerializer();

        public CliMain(IPluginConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            configuration = config;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (IDisposable disposable in disposeable_resources)
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        #endregion

        public CirMain BuildEngine()
        {
            PluginValidator validator = PluginValidator.Validator(GetType().Assembly); 
            CirMain engine = new CirMain(validator);
            LOG.Info("[CliMain] Loading Platform Plugins");

            foreach (string platform in configuration.AllPlatforms)
            {
                engine.LoadPlatformPlugin(platform);
            }

            LOG.Info("Available Platforms: ");
            List<IPlatformPlugin> ppinfo = engine.PlatformPlugins;

            for (int i = 0; i < ppinfo.Count; i++)
            {
                LOG.InfoFormat("{0}: {1} ({2})", i, ppinfo[i].Name, ppinfo[i].Description);
            }

            //
            // assign prerequisite results
            //
            //IElfUncompressedFile elf = null;
            IElfCompressedFile elf;
            ICiscoMainCoreFile mainCore;
            ICiscoIOCoreFile ioCore = null;
            try
            {
                //  elf = new ELFuncompressedFile(configuration.ElfFile.ToString());
                elf = new ELFcompressedFile(configuration.ElfFile);
                disposeable_resources.Add(elf);
                mainCore = new CiscoMainCoreFile(configuration.CoreFile);
                disposeable_resources.Add(mainCore);

                if (null != configuration.IOCoreFile)
                {
                    ioCore = new CiscoIOCoreFile(configuration.IOCoreFile);
                    disposeable_resources.Add(ioCore);
                }
            }
            catch (IOException)
            {
                LOG.Error("Failed to initialize Prerequisits");
                throw;
            }

            LOG.Info("[CliMain] Adding Prerequisites");
            if (null != configuration.IOCoreFile)
            {
                engine.AddPrerequisites(configuration, elf, mainCore, ioCore);
            }
            else
            {
                engine.AddPrerequisites(configuration, elf, mainCore);
            }

            LOG.Info("[CliMain] Loading Plugins");
            foreach (string path in configuration.AllPlugins)
            {
                engine.LoadPlugin(path);
            }
            LOG.Info("[CliMain] " + engine.Plugins.Count + " plugins loaded:");
            if (LOG.IsDebugEnabled)
            {
                LogPluginRequirementDeliveries(engine);
            }

            LOG.Info("[CliMain] Updating dependencies");
            engine.UpdateDependencies();
            LOG.Info(engine.AnalysisModules.Count + " plugins initially available");
#if DEBUG
            // DEBUG ONLY
            LOG.Debug("[CliMain] Writing debug graph file");
            engine.WritePluginDebugGraph("debug.tgf");
#endif

            LOG.Info("[CliMain] Ready");
            return engine;
        }

        private static void LogPluginRequirementDeliveries(CirMain engine)
        {
            foreach (IAnalysisPlugin p in engine.Plugins)
            {
                LOG.Debug(p.Name);
                StringBuilder requires = new StringBuilder("requires: ");
                foreach (Type t in p.Requirements)
                {
                    requires.Append(t).Append(", ");
                }
                LOG.Debug(requires);
                StringBuilder delivers = new StringBuilder("delivers: ");

                foreach (Type t in p.ResultTypes)
                {
                    delivers.Append(t).Append(", ");
                }
                LOG.Debug(delivers);
            }
        }

        public void Run()
        {
            Run(BuildEngine());
        }

        public void Run(CirMain engine)
        {
            if(LOG.IsInfoEnabled)
                LOG.InfoFormat("Executing Engine licenced for {0} -- {1}", HostOwner.Owner, HostOwner.LicenseComment );
            IEngineExecuter executer = ExecutorFactory.SingleThreadedExecutor();
            executer.PluginStep += PluginStep;
            executer.PluginExceptionEvent += PluginErrorEvent;
            executer.Execute(engine);
            WriteReport(engine);
        }


        private  void WriteReport(CirMain engine)
        {
#if DEBUG
            // DEBUG ONLY
            LOG.Info( "Dumping DEBUG Result Store file" );
            DumpResultStore(engine);
            LOG.Info( "Done dumping DEBUG Result Store file" );
#endif
            LOG.Info("REPORTING");
            report_serializer.AddRange(engine.GetReport());
            CreatePluginsReport(engine);
            report_serializer.AddLicenceReport();
            report_serializer.Serialize(new DirectoryInfo(configuration.RuntimeConfiguration.OutputFolder));
        }

        private void CreatePluginsReport(CirMain engine)
        {
            RunPluginsResult runPlugins = new RunPluginsResult(); 
            foreach (IAnalysisPlugin p in engine.Plugins)
            {
                runPlugins.Add(p);   
            }
            IReport report = ReportFactory.Instance.CreateMetaReport();
            report.AddReportNode(runPlugins);
            report.State = 0;
            report.Details = 0;
            report.Summary = "All successfully loaded plugins.";
            report.Author = "Main";
            report.Description = report.Summary;
            report_serializer.Add(report);
        }

        

        private static void DumpResultStore(CirMain engine)
        {

            LOG.Info("DUMPING RESULT STORE to file");

            StreamWriter streamWriter = new StreamWriter("resultStoreDebug.txt");

            streamWriter.WriteLine(engine.ToString());
            streamWriter.WriteLine("Plugin Stats:");
            foreach (IAnalysisPlugin plugin in engine.Plugins)
            {
                streamWriter.WriteLine(plugin.Name + (plugin.Done ? " - completed" : " - NOT completed"));
            }
            streamWriter.Close();

        }

        private static IReport ReportError(IPlugin plugin, Exception ex, String message)
        {
            IReport error_report = ReportFactory.Instance.CreatePluginReport();
            error_report.Description = message;
            error_report.Details = 0;
            error_report.Author = plugin.Name;
            error_report.Summary = ex.Message;
            return error_report;
        }

        private void PluginErrorEvent(IBackgroundPlugin plugin, PluginEventType type, Exception ex)
        {
            switch (type)
            {
                case PluginEventType.Init_Failed:
                    report_serializer.Add(ReportError(plugin, ex, "Failed to initialize plugin."));
                    break;
                case PluginEventType.Run_Error:
                    report_serializer.Add(ReportError(plugin, ex, "Plugin run failed."));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This method will be executed by the EngineExecuter after each step for a plugin.
        /// </summary>
        /// <param name="plugin"></param>
        private static void PluginStep(IBackgroundPlugin plugin)
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                LOG.Info("[Run] canceling operation");
                if(!plugin.Canceled)
                    plugin.Cancel();
            }
        }
    }
}