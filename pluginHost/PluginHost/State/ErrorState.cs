//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 

namespace PluginHost.State
{
    /// <summary>
    /// Represents a plugin which has detected some kind of irregularities during the plugin run. 
    /// This does not indicate that the plugin crashed or raised an exception it rather indicates a domain specific error.
    /// 
    /// This state has no valid following state.
    /// </summary>
    public class ErrorState : PluginExecutionState
    {
        
        internal ErrorState(PluginStateMachine a_machine, string message, IPluginResult a_result)
            : base(a_machine)
        {
            Message = message;
            Result = a_result;
        }

        /// <summary>
        /// Returns the state id which is most likely a simple string which represents the state.
        /// </summary>
        public override string StateId
        {
            get { return "Error"; }
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents an error state. Otherwise <code>false</code>.
        /// </summary>
        public override bool Error { get { return true; }  }

        /// <summary>
        /// Returns <code>true</code> if the State represents a completed state. Otherwise <code>false</code>.
        /// </summary>
        public override bool Completed { get { return true; } }

    }
}