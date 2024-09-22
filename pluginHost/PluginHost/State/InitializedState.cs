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
    /// Represents a successfully initialized plugin. 
    /// </summary>
    public class InitializedState : PluginExecutionState
    {
        internal InitializedState(PluginStateMachine a_machine)
            : base(a_machine)
        {
        }

        /// <summary>
        /// Returns the state id which is most likely a simple string which represents the state.
        /// </summary>
        public override string StateId
        {
            get { return "Initialized"; }
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents an initialized state. Otherwise <code>false</code>.
        /// </summary>
        public override bool Initialized
        {
            get { return true; }
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
        /// Transition to the <see cref="CancledState"/>
        /// This transition is valid for the states ReadyState, RunningState, InitializedState
        /// </summary>
        internal override void CrossCancled()
        {
            Machine.State = new CancledState(Machine);
        }

        /// <summary>
        /// Transition to the <see cref="RunningState"/>
        /// This transition is valid for the states InitializedState
        /// </summary>
        internal override void CrossRunning()
        {
            Machine.State = new RunningState(Machine);
        }

    }
}