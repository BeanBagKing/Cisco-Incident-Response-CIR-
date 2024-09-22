//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 

namespace PluginHost.State
{
    /// <summary>
    /// The PluginStateMachine is a simple base interface which acts as a state-context for a concrete PluginExecutionState.
    /// </summary>
    public interface PluginStateMachine
    {
        /// <summary>
        /// Gets or Sets the current state.
        /// </summary>
       PluginExecutionState State { get;  set; }
    }
}