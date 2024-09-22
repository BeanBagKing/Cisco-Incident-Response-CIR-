// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.IO;
using PluginHost;
using Recurity.CIR.Engine.Configuration;
using Recurity.CIR.Engine.Configuration.Section;

namespace Recurity.CIR.Engine.Interfaces
{
    public interface IPluginConfiguration : IPluginResult
    {
        RuntimeConfigSection RuntimeConfiguration
        {
            get;
        }
        String[] AllPlugins
        {
            get;
        }

        String[] AllPlatforms
        {
            get;
        }

        
        FileInfo ElfFile
        { 
            get; set;
        }
        FileInfo CoreFile
        {
            get;
            set;
        }
        FileInfo IOCoreFile
        {
            get;
            set;
        }}
}
