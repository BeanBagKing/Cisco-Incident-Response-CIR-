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
    /// Represents a running plugin.
    /// </summary>
    public class RunningState : PluginExecutionState
    {
        internal RunningState(PluginStateMachine machine)
            : base(machine)
        {
        }


        /// <summary>
        /// Returns <code>true</code> if the State represents a running state. Otherwise <code>false</code>.
        /// </summary>
        public override bool Running { get { return true; }  }

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
        /// Transition to the <see cref="CompletedState"/>
        /// This transition is valid for the states RunningState
        /// </summary>
        internal override void CrossCompleted(IPluginResult a_result)
        {
            Machine.State = new CompletedState(Machine, a_result);
        }


        /// <summary>
        /// Transition to the <see cref="CancledState"/>
        /// This transition is valid for the states ReadyState, RunningState, InitializedState
        /// </summary>
        internal override void CrossCancled()
        {
            Machine.State = new CancledState(Machine);
        }

        /// <summary>
        /// Returns the state id which is most likely a simple string which represents the state.
        /// </summary>
        public override string StateId
        {
            get { return "Running"; }
        }
    }
}