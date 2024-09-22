// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using PluginHost;
using Recurity.CIR.Engine.PluginResults;

namespace Recurity.CIR.Engine.Interfaces
{
    public interface IIOSSignature: IPluginResult
    {
        string Image { get; }
        string Family { get; }
        CiscoPlatforms KnownPlatform { get; }
        string FeatureSet { get; }
        string Version { get; }
        
        IOSVersion IOSVersion { get; }

        string Media { get; }
    }
}
