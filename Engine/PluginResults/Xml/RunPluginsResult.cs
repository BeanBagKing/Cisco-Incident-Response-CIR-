//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using PluginHost;
using Recurity.CIR.Engine.Report;

namespace Recurity.CIR.Engine.PluginResults.Xml
{
    public partial class RunPluginsResult : IReportNode
    {
        private readonly Dictionary<IPlugin, ExecutedPlugin> addedPlugins = new Dictionary<IPlugin, ExecutedPlugin>();

        public void AddRange(List<IPlugin> aPluginList)
        {
            if (aPluginList == null) throw new ArgumentNullException("aPluginList");
            foreach (IPlugin plugin in aPluginList)
            {
                Add(plugin);
            }
        }

        public void Add(IPlugin aPlugin)
        {
            if (aPlugin == null) throw new ArgumentNullException("aPlugin");
            if (!addedPlugins.ContainsKey(aPlugin))
            {
                Assembly asm = aPlugin.GetType().Assembly;
                ExecutedPlugin execPlugin = new ExecutedPlugin();
                execPlugin.name = aPlugin.Name;
                execPlugin.version = asm.GetName().Version.ToString();
                byte[] publicKeyToken = asm.GetName().GetPublicKeyToken();
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < publicKeyToken.GetLength(0); i++)
                {
                    builder.AppendFormat("{0:x}", publicKeyToken[i]);
                }
                execPlugin.publickey = builder.ToString();
                addedPlugins.Add(aPlugin, execPlugin);

                SetPlugins();
            }
        }

        private void SetPlugins()
        {
            ExecutedPlugin[] allPlugins = new ExecutedPlugin[addedPlugins.Count];
            addedPlugins.Values.CopyTo(allPlugins, 0);
            ExecutedPlugin = allPlugins;
        }
    }

}