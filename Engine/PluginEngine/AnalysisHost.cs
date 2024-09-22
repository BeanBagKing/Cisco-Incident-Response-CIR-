//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 

using System;
using System.Collections.Generic;
using System.IO;
using PluginHost;
using PluginHost.PluginValidation;

namespace Recurity.CIR.Engine.PluginEngine
{
    public class AnalysisHost<T> : PluginHost<T> where T : class, IAnalysisPlugin
    {
        protected PluginHost<IPlatformPlugin> _platformPlugins;
        protected List<T> _runablePlugins;
        protected ResultStore _resultStore;
        protected List<T> _unsupportedPlugins;

        public AnalysisHost(PluginValidator theValidator)
            : base(theValidator)
        {
            _resultStore = new ResultStore();
            _platformPlugins = new PluginHost<IPlatformPlugin>(theValidator);

            _runablePlugins = new List<T>();
            _unsupportedPlugins = new List<T>();
        }

        //
        // Platform Plugin Management
        //

        public void LoadPlatformPlugin(string file)
        {
            _platformPlugins.Add(file);
        }

        public List<IPlatformPlugin> PlatformPlugins
        {
            get { return _platformPlugins.Plugins; }
        }

        /// <summary>
        /// Gets or sets the Platform the AnalysisHost works with. 
        /// </summary>
        public CiscoPlatforms Platform
        {
            get
            {
                List<IPlatformPlugin> p = _resultStore.GetResults<IPlatformPlugin>();

                if (0 == p.Count)
                {
                    return CiscoPlatforms.NONE;
                }
                else if (1 == p.Count)
                {
                    return p[0].Platform;
                }
                else
                {
                    throw new ArgumentException("More than one PlatformPlugin activated!");
                }
            }
            set
            {
                List<IPlatformPlugin> p = _resultStore.GetResults<IPlatformPlugin>();

                if (0 != p.Count)
                {
                    throw new ArgumentException("Cannot set more than one PlatformPlugin !");
                }
                else
                {
                    foreach (IPlatformPlugin plat in _platformPlugins.Plugins)
                    {
                        if (plat.Platform == value)
                        {
                            _resultStore.Add(plat);
                        }
                    }
                }
            }
        }

        //
        // Analysis Plugin Management
        //

        public ResultStore ResultStore
        {
            get { return _resultStore; }
        }

        public void AddResults(ICollection<IPluginResult> resultList)
        {
            foreach (IPluginResult result in resultList)
            {
                _resultStore.Add(result);
            }
        }

        public List<T> AvailablePlugins
        {
            get { return _runablePlugins; }
        }

        public List<T> UnsupportedPlugins
        {
            get { return _unsupportedPlugins; }
        }

        public void UpdateAvailablePlugins()
        {
            _runablePlugins.Clear();
            _unsupportedPlugins.Clear();

            foreach (T p in _plugins)
            {
                if (PlatformSupported(p.Platforms))
                {
                    if (!p.Done && p.Enabled)
                    {
                        object[] handOver = new object[p.Requirements.Length];

                        for (int i = 0; i < p.Requirements.Length; i++)
                        {
                            handOver[i] = null;

                            List<IPluginResult> ofThisType = _resultStore.GetResults(p.Requirements[i]);
                            if (1 <= ofThisType.Count)
                            {
                                handOver[i] = ofThisType[0];
                            }
                        }

                        bool allRequirementsMet = true;
                        for (int i = 0; i < p.Requirements.Length; i++)
                        {
                            allRequirementsMet = allRequirementsMet && (null != handOver[i]);
                        }

                        if (allRequirementsMet)
                        {
                            p.FulFill(handOver);
                            _runablePlugins.Add(p);
                        }
                    } // if ( ! p.Done )
                } // platform supported 
                else
                {
                    _unsupportedPlugins.Add(p);
                }
            } // foreach plugin
        }

        private bool PlatformSupported(IEnumerable<CiscoPlatforms> platforms)
        {
            foreach (CiscoPlatforms supported in platforms)
            {
                if (supported == CiscoPlatforms.ANY)
                    return true;

                if (supported == Platform)
                    return true;
            }

            return false;
        }

        #region DebugGraphMaker

        private struct Edge
        {
            public uint source;
            public uint destination;
        }

        public void WritePluginDebugGraph(string filename)
        {
            Dictionary<string, uint> pluginInts = new Dictionary<string, uint>();
            uint counter = 1;
            List<Edge> edges = new List<Edge>();
            List<Edge> depends = new List<Edge>();

            //
            // collect plugin names themselves
            //
            foreach (T p in _plugins)
            {
                if (!pluginInts.ContainsKey(p.GetType().ToString()))
                {
                    pluginInts.Add(p.GetType().ToString(), counter++);
                }
            }
            //
            // collect interface names
            //
            foreach (T p in _plugins)
            {
                foreach (Type input in p.Requirements)
                {
                    if (!pluginInts.ContainsKey(input.ToString()))
                    {
                        pluginInts.Add(input.ToString(), counter++);
                    }
                }
                foreach (Type output in p.ResultTypes)
                {
                    if (!pluginInts.ContainsKey(output.ToString()))
                    {
                        pluginInts.Add(output.ToString(), counter++);
                    }
                }
            }

            //
            // collect Edges from and to plugins
            // 
            foreach (T p in _plugins)
            {
                foreach (Type input in p.Requirements)
                {
                    Edge e = new Edge();
                    e.source = pluginInts[input.ToString()];
                    e.destination = pluginInts[p.GetType().ToString()];
                    edges.Add(e);
                }
                foreach (Type output in p.ResultTypes)
                {
                    Edge e = new Edge();
                    e.source = pluginInts[p.GetType().ToString()];
                    e.destination = pluginInts[output.ToString()];
                    edges.Add(e);
                }
            }

            //
            // Write the whole show
            // 
            StreamWriter streamWriter = new StreamWriter(filename);
            foreach (string name in pluginInts.Keys)
            {
                string[] nameElements = name.Split(".".ToCharArray());
                streamWriter.WriteLine("{0:d} {1}", pluginInts[name], nameElements[nameElements.Length - 1]);
            }
            // Note/edge splitter line
            streamWriter.WriteLine("#");
            // Producer edges            
            foreach (Edge e in edges)
            {
                streamWriter.WriteLine("{0:d} {1:d} ", e.source, e.destination);
            }
            // Input/Output Edges (does not work)
            /*
            foreach( Edge e in depends )
            {
                streamWriter.WriteLine( "{0:d} {1:d} dependency", e.source, e.destination );
            }
            */
            streamWriter.Close();
        }

        #endregion DebugGraphMaker

        public void AddPlatform(IPlatformPlugin plugin)
        {
            _platformPlugins.Add(plugin);
        }
    }
}