//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Felix Lindner (fx@recurtiy-labs.com)
// 

namespace PluginHost
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
    }
}
