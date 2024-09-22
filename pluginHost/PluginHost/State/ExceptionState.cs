//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 
using System;

namespace PluginHost.State
{
    /// <summary>
    /// Represents a plugin which has thrown an exception during the initialization or runtime of the plugin.
    /// This state has no valid following state.
    /// </summary>
    public class ExceptionState : PluginExecutionState
    {
        internal ExceptionState(PluginStateMachine a_machine, Exception ex, string message)
            : base(a_machine)
        {
            ThrownException = ex;
            Message = message;
        }

        /// <summary>
        /// Returns the state id which is most likely a simple string which represents the state.
        /// </summary>
        public override string StateId
        {
            get { return "Exception"; }
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents an exception state. Otherwise <code>false</code>.
        /// </summary>
        public override bool Exception
        {
            get { return true; }
        }
    }
}