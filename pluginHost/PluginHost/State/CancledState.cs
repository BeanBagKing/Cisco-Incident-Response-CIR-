//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 

namespace PluginHost.State
{
    /// <summary>
    /// This state represents a cancled plugin.
    /// This state has no valid following state.
    /// </summary>
    public class CancledState : PluginExecutionState
    {
        internal CancledState(PluginStateMachine machine)
            : base(machine)
        {
            
        }

        public override bool Cancled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the state id which is most likely a simple string which represents the state.
        /// </summary>
        public override string StateId
        {
            get { return "Cancled"; }
        }
    }
}