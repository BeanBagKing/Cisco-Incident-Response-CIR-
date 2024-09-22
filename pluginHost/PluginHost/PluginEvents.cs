//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 

using PluginHost.State;

namespace PluginHost
{
    /// <summary>
    /// This public class defines the delegate signature for events available via IBackgroundPlugin instances. 
    /// The reason for a new class is that interfaces can not define delegates.
    /// </summary>
    public class PluginEvents
    {
        public delegate void ProgressReport(IBackgroundPlugin a_plugin, uint progress);
        public delegate void StateChanged(IBackgroundPlugin a_plugin, PluginExecutionState a_state);
    }
}
