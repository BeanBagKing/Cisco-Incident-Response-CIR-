//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 

using log4net;
using PluginHost;
using Recurity.CIR.Engine.Executor;
using Recurity.CIR.Engine.PluginEngine;

namespace Recurity.CIR.Engine.Executor
{
    /// <summary>
    /// The EngineExecuter executes a preconfigured instance of CirMain. It iterates over all configured and loaded plugins 
    /// and executes all plugins ready to run. As soon as no plugin is able to run or all plugins are done the executer exits the 
    /// algorithm.
    /// The Executer defines several events for the plugin run. External resources can register delegates for each internal event. 
    /// While a plugin is running the executer checks the advance of the plugin frequently. Each time the advance is checked the PluginStep 
    /// Event is fired.
    /// </summary>
    class EngineExecuter : AbstractEngineExecutor
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (EngineExecuter));

        public override void Execute(CirMain engine)
        {
            do
            {
                foreach (IAnalysisPlugin p in engine.AnalysisModules)
                {
                    if (!InitializePlugin(p))
                        continue;
                    Run(engine, p);
                }
                LOG.InfoFormat("[Run] Updating dependencies, {0}/{1} plugins done", engine.PluginsCompleted,
                               engine.Plugins.Count
                    );

                engine.UpdateDependencies();
            } while (engine.AnalysisModules.Count > 0);
        }


        private void Run(CirMain engine, IAnalysisPlugin p)
        {
            LOG.Info("[Run] Starting plugin: " + p.Name);

            RunPlugin(p);
            if (p.Done)
            {
                if (p.ResultAvailable)
                {
                    LogStatus(p, "Run", "got result");
                    engine.AddPrerequisites(p.Results);
                }
                else
                {
                    LogStatus(p, "Run", "NO RESULT");
                }
                InvokePluginEvent(p, PluginEventType.Success);
            }
        }


      
    }
}