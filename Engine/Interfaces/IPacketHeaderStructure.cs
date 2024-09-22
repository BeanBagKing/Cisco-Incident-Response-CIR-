// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System.Collections.Generic;
using PluginHost;

namespace Recurity.CIR.Engine.Interfaces
{
    public interface IPacketHeaderStructure : IPluginReporter, IPluginResult
    {
        List<IPacketHeader> Headers { get; }
    }
}
