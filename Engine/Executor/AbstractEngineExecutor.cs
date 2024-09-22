// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using PluginHost;
using Recurity.CIR.Engine.PluginEngine;

namespace Recurity.CIR.Engine.Executor
{
    abstract class AbstractEngineExecutor : IEngineExecuter
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(AbstractBackgroundPlugin));
        public event ExecutorEvents.PluginEventDelegate PluginEvent;
        public event ExecutorEvents.PluginExceptionDelegate PluginExceptionEvent;
        public event ExecutorEvents.PluginStepDelegate PluginStep;

        public abstract void Execute(CirMain engine);

        protected bool InitializePlugin(IAnalysisPlugin plugin)
        {
            LogStatus(plugin, "Run", "Initializing plugin");
            try
            {
                plugin.Initialize();
                if (PluginEvent != null)
                {
                    PluginEvent(plugin, PluginEventType.Init_Success);
                }
            }
            catch (PluginError ex)
            {
                if (PluginExceptionEvent != null)
                {
                    PluginExceptionEvent(plugin, PluginEventType.Init_Failed, ex);
                }
                // Do not try to recover from error!
                throw;
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Failed to initialize plugin [{0}] -- reason: [{1}]", plugin.Name, ex.Message);
                // just if we run in debug mode
                if (LOG.IsDebugEnabled)
                    LOG.ErrorFormat("Initialization failed", ex);
                if (PluginExceptionEvent != null)
                {
                    PluginExceptionEvent(plugin, PluginEventType.Init_Failed, ex);
                }
                return false;
            }
            plugin.ProgressEvent += PrintPercentageDone;
            return true;
        }

        /// <summary>
        /// Executes the given plugin and raises a PluginExceptionEvent if the plugin has thrown an
        /// exception. 
        /// </summary>
        /// <param name="a_plugin">the plugin to execute</param>
        protected void RunPlugin(IAnalysisPlugin a_plugin)
        {
            LogStatus(a_plugin, "Run", "Started plugin");
            a_plugin.Run();

            if (a_plugin.Error)
            {
                Exception ex = a_plugin.ReportedError;
                if (PluginExceptionEvent != null)
                {
                    PluginExceptionEvent(a_plugin, PluginEventType.Run_Error, ex);
                }
                LogStatus(a_plugin, "Run", "Error: "+ex.Message);
            }
        }

        protected static void LogStatus(IAnalysisPlugin a_plugin, string state, string a_message)
        {
            if(LOG.IsInfoEnabled)
                LOG.Info(String.Format("Plugin [{0}] - [{1}] {2}", a_plugin.Name, state,  a_message));
        }


        private void PrintPercentageDone(IBackgroundPlugin a_plugin, uint progress)
        {
            LOG.Info( a_plugin.PercentComplete.ToString("d") + "% done");
            if (PluginStep != null)
            {
                PluginStep(a_plugin);
            }
        }

        protected void InvokePluginEvent(IBackgroundPlugin plugin, PluginEventType type)
        {
            if(PluginEvent != null)
            {
                PluginEvent(plugin, type);
            }
        }

        protected void InvokePluginStep(IBackgroundPlugin plugin)
        {
            if(PluginStep != null)
            {
                PluginStep(plugin);
            }
        }
    }
}
