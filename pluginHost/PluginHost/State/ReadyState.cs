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
    /// Represents the intial state of each plugin. A plugin in the ReadyState is ready to run.
    /// </summary>
    public class ReadyState : PluginExecutionState
    {
        internal ReadyState(PluginStateMachine machine)
            : base(machine)
        {
        }

        /// <summary>
        /// Transition to the <see cref="InitializedState"/>
        /// This transition is valid for the states ReadyState
        /// </summary>
        internal override void CrossInitialized()
        {
            Machine.State = new InitializedState(Machine);
        }

        /// <summary>
        /// Transition to the <see cref="ExceptionState"/>
        /// This transition is valid for the states ReadyState, RunningState, InitializedState
        /// </summary>
        /// <param name="ex">The thrown exception</param>
        /// <param name="a_message">an exception message</param>
        internal override void CrossException(Exception ex, string a_message)
        {
            Machine.State = new ExceptionState(Machine, ex, a_message);
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents a ready state. Otherwise <code>false</code>.
        /// </summary>
        public override bool Ready
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the state id which is most likely a simple string which represents the state.
        /// </summary>
        public override string StateId
        {
            get { return "Ready"; }
        }
    }
}