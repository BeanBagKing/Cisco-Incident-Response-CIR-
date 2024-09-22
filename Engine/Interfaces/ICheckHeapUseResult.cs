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
    public interface ICheckHeapUseResultRecord
    {
        IProcessInformation Process { get; }
        UInt64 HeapBlocks { get; }
        UInt64 HeapSize { get; }
    }

    public interface ICheckHeapUseResult : IPluginResult
    {
        List<ICheckHeapUseResultRecord> Results { get; }
    }
}
