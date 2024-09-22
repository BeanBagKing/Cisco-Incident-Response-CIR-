// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using PluginHost;
using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Engine.PluginEngine
{
    public interface IPlatformPlugin : IPlugin, IPluginResult
    {
        CiscoPlatforms Platform { get; }
        
        //
        // Core file info
        //
        UInt64 CoreMemoryBase { get; }
        bool BigEndian { get; }

        //
        //
        // IOMEM Core file info
        UInt64 IOCoreMemoryBase { get; }

        //
        // ELF virtual memory mapper 
        //
        IVirtualAddressMapper VirtualAddressMapper { get; }
    }
}
