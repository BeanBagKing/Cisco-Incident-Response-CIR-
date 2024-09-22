// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using PluginHost;

namespace Recurity.CIR.Engine.PluginEngine
{
    //public interface IAnalysisPlugin : PluginHost.IBackgroundPlugin
    public interface IAnalysisPlugin : IBackgroundPlugin
    {
        CiscoPlatforms[] Platforms { get; }
     
        Type[] ResultTypes { get; }
        IPluginResult[] Results { get; }
        
        Type[] Requirements { get; }
        void FulFill( object[] requirements );     
    }
}
