// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using PluginHost;
using Recurity.CIR.Engine.Helper;

namespace Recurity.CIR.Engine.Interfaces
{
 
    public struct VirtualMemorySectionProperties
    {
        public TriBool Writable;
        public TriBool Executable;
        public TriBool Initialized;
        public TriBool DataAvailable;
    }

    public interface IVirtualMemorySection : IPluginResult
    {
        string Name { get; }
        UInt64 Address { get; }
        UInt64 Size { get; }
        VirtualMemorySectionProperties Properties { get; }
    }

}
