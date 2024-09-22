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
    /// PluginExecutionState is base class that identifies the current state of a plugin.
    /// A Plugin can only have on single state at a time. The current state of a plugin does define all
    /// transitions to following states of the plugin and throws an exception if a certain transition is
    /// illegal for the current state. Each Method prefixed with 'Cross' represents a state transition and 
    /// moves the PluginStateMachine into a new state corresponding to the transition or throws an exception if the transition is
    /// illegal. 
    /// 
    /// This implementation follows the well known State - DesignPatter (GoF)
    /// </summary>
    public abstract class PluginExecutionState
    {
        private readonly PluginStateMachine machine;
        private IPluginResult result;
        private Exception thrownException;
        private String message;

        /// <summary>
        /// Factory method which sets the initial state for the state machine.
        /// </summary>
        /// <param name="machine">the PluginStateMachine to set the initial state to.</param>
        internal static void InitializeStateMachine(PluginStateMachine machine)
        {
            machine.State = new ReadyState(machine);
        }

        /// <summary>
        /// Default constructor - creates a new PluginExectuionState
        /// </summary>
        /// <param name="a_machine">the state machine</param>
        internal PluginExecutionState(PluginStateMachine a_machine)
        {
            if (a_machine == null) throw new ArgumentNullException("a_machine");
            machine = a_machine;
        }

        /// <summary>
        /// Transition to the <see cref="ExceptionState"/>
        /// This transition is valid for the states ReadyState, RunningState, InitializedState
        /// </summary>
        /// <param name="ex">The thrown exception</param>
        /// <param name="a_message">an exception message</param>
        internal virtual void CrossException(Exception ex, String a_message)
        {
            ThrowIllegalTransition(new ExceptionState(Machine, null, null));
        }


        /// <summary>
        /// Transition to the <see cref="ErrorState"/>.
        /// This transition is valid for the states CompletedState
        /// </summary>
        /// <param name="reason">the reason fo the error</param>
        /// <param name="a_result">the plugin result</param>
        internal virtual void CrossError(string reason, IPluginResult a_result)
        {
            ThrowIllegalTransition(new ErrorState(Machine, null, a_result));
        }

        /// <summary>
        /// Transition to the <see cref="RunningState"/>
        /// This transition is valid for the states InitializedState
        /// </summary>
        internal virtual void CrossRunning()
        {
            ThrowIllegalTransition(new RunningState(Machine));
        }

        /// <summary>
        /// Transition to the <see cref="InitializedState"/>
        /// This transition is valid for the states ReadyState
        /// </summary>
        internal virtual void CrossInitialized()
        {
            ThrowIllegalTransition(new InitializedState(Machine));
        }

        /// <summary>
        /// Transition to the <see cref="CancledState"/>
        /// This transition is valid for the states ReadyState, RunningState, InitializedState
        /// </summary>
        internal virtual void CrossCancled()
        {
            ThrowIllegalTransition(new CancledState(Machine));
        }

        /// <summary>
        /// Transition to the <see cref="CompletedState"/>
        /// This transition is valid for the states RunningState
        /// </summary>
        internal virtual void CrossCompleted(IPluginResult a_result)
        {
            ThrowIllegalTransition(new CompletedState(Machine, a_result));
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents an exception state. Otherwise <code>false</code>.
        /// </summary>
        public virtual bool Exception
        {
            get { return false; }
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents a completed state. Otherwise <code>false</code>.
        /// </summary>
        public virtual bool Completed
        {
            get { return false; }
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents a running state. Otherwise <code>false</code>.
        /// </summary>
        public virtual bool Running
        {
            get { return false; }
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents an error state. Otherwise <code>false</code>.
        /// </summary>
        public virtual bool Error
        {
            get { return false; }
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents an initialized state. Otherwise <code>false</code>.
        /// </summary>
        public virtual bool Initialized
        {
            get { return false; }
        }

        /// <summary>
        /// Returns <code>true</code> if the State represents a cancled state. Otherwise <code>false</code>.
        /// </summary>
        public virtual bool Cancled
        {
            get { return false; }
        }


        /// <summary>
        /// Returns <code>true</code> if the State represents a ready state. Otherwise <code>false</code>.
        /// </summary>
        public virtual bool Ready
        {
            get { return false; }
        }

        /// <summary>
        /// Sets or Gets the result of the represented plugin if available. Otherwise it returns null.
        /// </summary>
        public IPluginResult Result
        {
            get { return result; }
            set { result = value; }
        }
        /// <summary>
        /// Returns the thrown Exception if the state represents an exception state.
        /// </summary>
        public Exception ThrownException
        {
            get { return thrownException; }
            protected set { thrownException = value; }
        }

        /// <summary>
        /// Return the exception message if the state represents an exception state.
        /// </summary>
        public String Message
        {
            get { return message; }
            protected set { message = value; }
        }

        protected PluginStateMachine Machine
        {
            get { return machine; }
        }


        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override string ToString()
        {
            return StateId;
        }

        /// <summary>
        /// Returns the state id which is most likely a simple string which represents the state.
        /// </summary>
        public abstract string StateId { get; }

        private void ThrowIllegalTransition(PluginExecutionState state)
        {
            throw new Exception(String.Format("Can not change state to {0} from state {1}", state, ToString()));
        }
    }
}
