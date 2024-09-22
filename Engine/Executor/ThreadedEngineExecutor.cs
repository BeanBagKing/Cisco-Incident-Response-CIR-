//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 
using System;
using System.Collections.Generic;
using System.Threading;
using PluginHost;
using Recurity.CIR.Engine.PluginEngine;

namespace Recurity.CIR.Engine.Executor
{
    internal class ThreadedEngineExecutor : AbstractEngineExecutor
    {
        //private static readonly ILog LOG = LogManager.GetLogger(typeof (ThreadedEngineExecutor));
        private readonly IList<IAnalysisPlugin> running_plugins = new List<IAnalysisPlugin>();
        private readonly object WaitObject = new object();
        private volatile CirMain engine = null;
        private volatile PluginError error = null;


        /// <summary>
        /// Executes all plugin in the CirMain engine concurrently.
        /// TODO add more comments
        /// </summary>
        /// <param name="a_engine"></param>
        public override void Execute(CirMain a_engine)
        {
            Monitor.Enter(WaitObject);
            try
            {
                if (engine != null)
                    throw new Exception(
                        "ThreadedEngineExecuter is already executing an CirMain Engine. Multiple engines per executer is not supported");
                // we need to keep that state here as the update calls will be concurrent
                engine = a_engine;

                if (UpdateEngine())
                {
                    // spawn the first threads and go to sleep until all plugins are done or a pluginerror occured
                    // if there is nothing to do update engine returns false and we exist immediately.
                    Monitor.Wait(WaitObject);
                }

                // if an plugin error occured the error member will hold a reference to it.
                if(error != null)
                {
                    throw error;
                }
            }
            finally
            {
                engine = null;
                error = null;
                Monitor.Exit(WaitObject);
            }
        }

        /// <summary>
        /// 1. Updates the dependencies
        /// 2. Spawns all available plugins
        /// 3. if the engine has no plugin to run anymore and there is no plugin running anymore
        ///    we pluse on the monitor.
        /// </summary>
        private bool UpdateEngine()
        {
            lock (WaitObject)
            {
                // we do not try to recover from a PluginError
                if(error != null)
                {
                    Monitor.PulseAll(WaitObject);
                    return false;
                }
                engine.UpdateDependencies();
                IList<IAnalysisPlugin> plugins = GetReadyToRun(engine);
                // starts all plugins concurrently
                SpawnPlugins(plugins);
                if (engine.AnalysisModules.Count < 1 && running_plugins.Count < 1)
                {
                    // the return value is important if this is the first / initial call to UpdateEngine
                    // otherwise we would miss the Pulse call and sleep forever.
                    Monitor.PulseAll(WaitObject);
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Start all given plugins in a seperate thread.
        /// </summary>
        /// <param name="a_plugins">the plugins to spawn</param>
        private void SpawnPlugins(IEnumerable<IAnalysisPlugin> a_plugins)
        {
            foreach (IAnalysisPlugin plugin in a_plugins)
            {
                // make sure the plugin is in the running list before we spawn the thread.

                if (running_plugins.Contains(plugin))
                    throw new Exception(String.Format("Can not add one plugin twice {0}", plugin.Name));
                running_plugins.Add(plugin);

                /* 
                 * InitializeAndRun will be called concurrently as soon as a thread in the pool is 
                 * available. The plugin is passes as a parameter.
                 */
                ThreadPool.QueueUserWorkItem(InitializeAndRun, plugin);
            }
        }

        /// <summary>
        /// 1. Initialize the plugin
        /// 2a. If successful Run the plugin
        /// 2b. If init failed call goto 4.
        /// 3a. If run successful Invoke event success
        /// 3b. If run failed ???????
        /// 4. Call PluginTerminated to update the engine and remove the plugin from the running list.
        /// </summary>
        /// <param name="a_plugin"></param>
        private void InitializeAndRun(object a_plugin)
        {
            IAnalysisPlugin plugin = a_plugin as IAnalysisPlugin;
            if (plugin != null)
            {
                try
                {
                    if (InitializePlugin(plugin))
                    {
                        RunPlugin(plugin);
                        InvokePluginEvent(plugin, PluginEventType.Success);
                    }
                }
                catch (PluginError ex)
                {
                    error = ex;
                }
                finally
                {
                    // we need to call this to remove the plugin from the running list no matter what happened above
                    PluginTerminated(plugin);
                }
            }
        }


        /// <summary>
        /// 1. lock for exclusive access
        /// 2. remove plugin from 'running' list
        /// 3. add plugin result as prerequisite to engine if available
        /// 4. Call UpdateEngine to spawn new plugins
        /// </summary>
        /// <param name="a_plugin"></param>
        private void PluginTerminated(IAnalysisPlugin a_plugin)
        {
            // we really need exclusive access to the engine here.
            lock (WaitObject)
            {
                lock (running_plugins)
                {
                    running_plugins.Remove(a_plugin);
                }
                if (a_plugin.ResultAvailable)
                {
                    LogStatus(a_plugin, "Run", "got result");
                    IPluginResult[] results = a_plugin.Results;
                    engine.AddPrerequisites(results);
                }
                else
                {
                    LogStatus(a_plugin, "Run", "got no result");
                }
                // locks are reentrant
                UpdateEngine();
            }
        }

        /// <summary>
        /// Fetches all plugins which are ready to run from the engine
        /// </summary>
        /// <returns>A list of plugins - all ready to run</returns>
        private IList<IAnalysisPlugin> GetReadyToRun(CirMain a_engine)
        {
            List<IAnalysisPlugin> retval = new List<IAnalysisPlugin>();
            foreach (IAnalysisPlugin plugin in a_engine.AnalysisModules)
            {
                /* if the plugin was available before and the worker thread 
                 * has not been started yet or initialization is still running
                 * the plugin is still in state ready. In one of these cases
                 * the plugin is already in the running list and needs to be excluded
                 * here!
                 */
                if (!running_plugins.Contains(plugin))
                    retval.Add(plugin);
            }

            return retval;
        }
    }
}