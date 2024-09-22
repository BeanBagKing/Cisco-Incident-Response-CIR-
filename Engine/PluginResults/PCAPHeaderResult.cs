// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using PluginHost;

namespace Recurity.CIR.Engine.PluginResults
{
    public class PCAPHeaderResult : AbstractPluginReporter, IPluginResult
    {

         public PCAPHeaderResult(String who, bool success, String summary) : base(who, success, summary)
        {
    
        }
    }
}
