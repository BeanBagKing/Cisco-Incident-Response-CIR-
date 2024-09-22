//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 

namespace PluginHost.State
{
    /// <summary>
    /// Represents a successfully completed plugin.
    /// </summary>
    public class CompletedState : PluginExecutionState
    {
        internal CompletedState(PluginStateMachine machine, IPluginResult a_result)
            : base(machine)
        {
            Result = a_result;
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents a completed state. Otherwise <code>false</code>.
        /// </summary>
        public override bool Completed { get { return true; } }


        /// <summary>
        /// Transition to the <see cref="ErrorState"/>.
        /// This transition is valid for the states CompletedState
        /// </summary>
        /// <param name="reason">the reason fo the error</param>
        /// <param name="a_result"></param>
        internal override void CrossError(string reason, IPluginResult a_result)
        {
            Machine.State = new ErrorState(Machine, reason, a_result);
        }

        /// <summary>
        /// Returns the state id which is most likely a simple string which represents the state.
        /// </summary>
        public override string StateId
        {
            get { return "Completed"; }
        }
    }
}