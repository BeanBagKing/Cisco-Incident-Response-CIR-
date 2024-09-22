// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using log4net;
using Recurity.CIR.Engine.Configuration.Section;
using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Engine.PluginResults
{
    public class PluginConfiguration : IPluginConfiguration
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (PluginConfiguration));
        private readonly System.Configuration.Configuration config;
        private readonly RuntimeConfigSection engineSection;
        private readonly Dictionary<string, bool> platformDict = new Dictionary<string, bool>(30);
        private readonly Dictionary<string, bool> pluginDict = new Dictionary<string, bool>(30);
        private FileInfo coreFile;
        private FileInfo elfFile;
        private FileInfo ioCoreFile;

        public PluginConfiguration()
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            object o = config.GetSection("Cir.Engine");
            engineSection = config.GetSection("Cir.Engine") as RuntimeConfigSection;
            Validate();
            LoadPlugins();
            LoadPlatforms();
        }

        #region IPluginConfiguration Members

        public RuntimeConfigSection RuntimeConfiguration
        {
            get { return engineSection; }
        }

        public String[] AllPlugins
        {
            get
            {
                String[] result = new String[pluginDict.Count];
                uint i = 0;
                foreach (String key in pluginDict.Keys)
                {
                    result[i++] = key;
                }
                return result;
            }
        }

        public String[] AllPlatforms
        {
            get
            {
                String[] result = new String[platformDict.Count];
                uint i = 0;
                foreach (String key in platformDict.Keys)
                {
                    result[i++] = key;
                }
                return result;
            }
        }

        public FileInfo ElfFile
        {
            get { return elfFile; }
            set { elfFile = value; }
        }

        public FileInfo CoreFile
        {
            get { return coreFile; }
            set { coreFile = value; }
        }

        public FileInfo IOCoreFile
        {
            get { return ioCoreFile; }
            set { ioCoreFile = value; }
        }

        public bool Errors
        {
            get { return false; }
        }

        #endregion

        private void LoadPlugins()
        {
            CirPluginCollection collection = engineSection.Plugins;
            foreach (CirPlugin plugin in collection)
            {
                if ("*".Equals(plugin.Assembly.Trim()))
                {
                    try
                    {
                        AddDirectory(pluginDict, plugin.Path);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        LOG.WarnFormat("Can not load Plugins from Directory. {0}", ex.Message);
                    }
                }
                else
                {
                    String path = Path.Combine(plugin.Path, plugin.Assembly);
                    if (pluginDict.ContainsKey(path))
                    {
                        throw new CirConfigurationException(String.Format("duplicated plugin configured {0}", path));
                    }
                    LOG.InfoFormat("Plugin {0} configured", plugin.Assembly);
                    pluginDict.Add(path, true);
                }
            }
        }

        private void LoadPlatforms()
        {
            CirPlatformCollection collection = engineSection.Platforms;
            if (collection == null)
                return;
            foreach (CirPlatform platform in collection)
            {
                if ("*".Equals(platform.Assembly.Trim()))
                {
                    try
                    {
                        AddDirectory(platformDict, platform.Path);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        LOG.WarnFormat("Can not load PlatformPlugins from Directory. {0}", ex.Message);
                    }
                }
                else
                {
                    String path = Path.Combine(platform.Path, platform.Assembly);
                    if (!platformDict.ContainsKey(path))
                    {
                        LOG.InfoFormat("Platform {0} configured", platform.Assembly);
                        platformDict.Add(path, true);
                    }
                }
            }
        }

        private static void AddDirectory(IDictionary<string, bool> dict, String directory)
        {
            if (directory == null || directory.Length == 0)
                return;
            DirectoryInfo info;
            try
            {
                info = new DirectoryInfo(directory);
                if (!info.Exists)
                    throw new DirectoryNotFoundException(String.Format("Directory {0} does not exist", info.FullName));
                foreach (FileInfo dll in info.GetFiles("*.dll"))
                {
                    if (!dict.ContainsKey(dll.FullName))
                    {
                        dict.Add(dll.FullName, true);
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LOG.Warn(String.Format("Failed to load directory {0}", directory), ex);
            }
        }

        private static void CheckPaths(params String[] paths)
        {
            DirectoryInfo info;
            foreach (String apath in paths)
            {
                try
                {
                    info = new DirectoryInfo(apath);
                }
                catch (Exception)
                {
                    throw new CirConfigurationException(
                        String.Format("No such File or Directory [{0}]", apath));
                }
                if (!info.Exists)
                    throw new CirConfigurationException(
                        String.Format("No such File or Directory [{0}]", info.ToString()));
            }
        }

        public void Validate()
        {
            if (engineSection == null)
            {
                throw new CirConfigurationException("Cir.Engine configuration section missing");
            }
            CheckPaths(engineSection.OutputFolder, engineSection.DaemonWorkDirectory);
        }
    }

    public class CirConfigurationException : Exception
    {
        public CirConfigurationException(String message, Exception innerException) : base(message, innerException)
        {
        }

        public CirConfigurationException(String message)
            : base(message)
        {
        }
    }
}