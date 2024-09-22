// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using PluginHost;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;

namespace Recurity.CIR.PlatformPlugins.Cisco2600
{
    public class C2600Platform : IPlatformPlugin
    {
        #region IPlatformPlugin Members

        CiscoPlatforms IPlatformPlugin.Platform
        {
            get { return CiscoPlatforms.C2600; }
        }

        ulong IPlatformPlugin.CoreMemoryBase
        {
            get { return 0x80000000; }
        }

        bool IPlatformPlugin.BigEndian
        {
            get { return true; }
        }

        public UInt64 IOCoreMemoryBase
        {
            get { return 0x04000000; }
        }

        public IVirtualAddressMapper VirtualAddressMapper
        {
            get { return null; }
        }

        #endregion

        #region IPlugin Members

        string IPlugin.Name
        {
            get { return "Cisco 2600"; }
        }

        string IPlugin.Description
        {
            get { return "Platform Plugin for Cisco 2600 Series"; }
        }

        public bool Errors
        {
            get { return false; }
        }

        #endregion
    }
}
