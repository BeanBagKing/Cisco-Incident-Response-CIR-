// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System.Collections.Generic;
using PluginHost;

namespace Recurity.CIR.Engine.Interfaces
{
    public interface IProcessList : IPluginResult
    {
        List<IProcessInformation> Processes { get; }
        IProcessInformation this[ uint pid ] { get; }
        IProcessInformation GetProcessByPID( uint pid );
    }
}
