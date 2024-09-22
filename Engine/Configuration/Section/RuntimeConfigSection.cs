// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Configuration;

namespace Recurity.CIR.Engine.Configuration.Section
{
    public class RuntimeConfigSection : ConfigurationSection 
    {


        [ConfigurationProperty("plugins", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(CirPluginCollection))]
        public CirPluginCollection Plugins
        {
            get { return (CirPluginCollection)this["plugins"]; }
            set { this["plugins"] = value; }
        }

        [ConfigurationProperty("platforms", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(CirPlatformCollection))]
        public CirPlatformCollection Platforms
        {
            get { return (CirPlatformCollection)this["platforms"]; }
            set { this["platforms"] = value; }
        }

        [ConfigurationProperty("outputfolder", IsRequired = true)]
        public String OutputFolder
        {
            get { return (string) this["outputfolder"]; }
            set { this["outputfolder"] = value;}

        }
        [ConfigurationProperty("daemonWorkDir", IsRequired = true)]
        public String DaemonWorkDirectory
        {
            get { return (string)this["daemonWorkDir"]; }
            set { this["daemonWorkDir"] = value; }

        }
        [ConfigurationProperty("setDaemon", IsRequired = true)]
        public bool IsDaemon
        {
            get { return (bool)this["setDaemon"]; }
            set{ this["setDaemon"] = value;}

        }
    }
}
