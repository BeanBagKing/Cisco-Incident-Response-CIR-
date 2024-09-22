// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using PluginHost;

namespace Recurity.CIR.Engine.Interfaces
{
    public interface IHeapStructure : IPluginResult
    {
        List<IHeapBlock32> Blocks { get; }
        UInt32 DataOffset { get; }
        IHeapBlock32 this[ UInt64 address ] { get; }
        IHeapBlock32 GetBlockAt( UInt64 address );
        IHeapBlock32 GetBlockWithPayloadAt( UInt64 address );
    }
}
