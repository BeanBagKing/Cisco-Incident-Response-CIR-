// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System.Collections.Generic;
using PluginHost;
using Recurity.CIR.Engine.PluginResults;

namespace Recurity.CIR.Engine.Interfaces
{
    public interface ICheckHeapResults: IPluginResult 
    {
        List<CheckHeapResultRecord> Results { get; }
    }
}
