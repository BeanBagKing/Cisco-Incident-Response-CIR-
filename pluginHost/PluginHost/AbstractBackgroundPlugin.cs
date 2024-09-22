//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 
using System;
using System.Collections.Generic;
using log4net;
using PluginHost.State;

namespace PluginHost
{
    /// <summary>
    /// This abstract class is used as a base class for concrete IBackgroundPlugin implementations.
    /// It implements the PluginStateMachine interface and provides events for state changes as well as progress notification.
    /// The abstract methods declared in this class mark a straight forward way to implement a contrete plugin for the CIR application.
    /// An executor of a IBackground plugin should call <code>Initialize</code> to set up the plugin this will cause
    /// a call to the abstract method <code>InitializeInteral</code> to set up the concrete plugin. 
    /// If the set up procedure was successful the plugin can be executed due to a call to the <code>Run</code> method. The AbstractBackgroundPlugin
    /// will call the abstract method <code>doStep</code> in a loop until the concrete implementation calls the <code>done</code> routine to indicate 
    /// that the plugin has completed the execution and the <code>Run</code> method will return.
    /// 
    /// To enable an intuitive communication with the plugin, executer can register event handler for a <code>StateChangedEvent</code> which will be executed for every 
    /// state change in the plugin. The simple event <code>ProgressEvent</code> is executed if the plugin progress has changed.
    /// </summary>
    public abstract class AbstractBackgroundPlugin : IBackgroundPlugin, PluginStateMachine
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (AbstractBackgroundPlugin));

        private readonly List<IDisposable> disposeable_resourecs = new List<IDisposable>();

        private uint _percentComplete = 0;

        private PluginExecutionState execution_state;

        /// <summary>
        /// Creates a new plugin instance and initializes the state machine.
        /// </summary>
        public AbstractBackgroundPlugin()
        {
            PluginExecutionState.InitializeStateMachine(this);
        }

        /// <summary>
        /// Returns the result or <code>null</code> if the result is not available
        /// </summary>
        protected IPluginResult Result
        {
            get { return execution_state.Result; }
        }

        #region IBackgroundPlugin Members

        /// <summary>
        /// Executed for every event change.
        /// </summary>
        public event PluginEvents.StateChanged StateChangedEvent;

        /// <summary>
        /// Executed for every progress change.
        /// </summary>
        public event PluginEvents.ProgressReport ProgressEvent;

        public abstract string Name { get; }
        public abstract string Description { get; }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin is running. Otherwise <code>false</code>.
        /// </summary>
        public bool Running
        {
            get { return execution_state.Running; }
        }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin is cancled. Otherwise <code>false</code>.
        /// </summary>
        public bool Canceled
        {
            get { return execution_state.Cancled; }
        }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin thrown and exception . Otherwise <code>false</code>.
        /// </summary>
        public bool Error
        {
            //TODO refactor this in the interface - error should be Exception or something like that
            get { return execution_state.Exception; }
        }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin has finished successfully and has detected some irregularities.
        /// Otherwise <code>false</code>.
        /// </summary>
        public bool Detection
        {
            get { return execution_state.Error; }
        }

        /// <summary>
        /// The exception thrown by the plugin if the <code>Error</code>  property returns <code>true</code> otherwise <code>false</code>
        /// </summary>
        public Exception ReportedError
        {
            get { return execution_state.ThrownException; }
        }

        /// <summary>
        /// The current plugin progress in percent.
        /// </summary>
        public uint PercentComplete
        {
            get { return _percentComplete; }
        }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin has finished successfully and has a result submitted. Otherwise <code>false</code>.
        /// </summary>
        public bool ResultAvailable
        {
            get { return execution_state.Result != null; }
        }

        /// <summary>
        /// Returns the current state of execution.
        /// </summary>
        public PluginExecutionState ExecutionState
        {
            get { return execution_state; }
        }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin is in the state Ready. Otherwise <code>false</code>.
        /// </summary>
        public bool Enabled
        {
            get { return execution_state.Ready; }
        }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin execution is done. Otherwise <code>false</code>.
        /// </summary>
        public bool Done
        {
            get { return execution_state.Completed; }
        }

        /// <summary>
        /// The plugins Setup - procedure.
        /// </summary>
        public void Initialize()
        {
            try
            {
                InitializeInternal();
                // successful initialized
                execution_state.CrossInitialized();
            }
            catch (Exception ex)
            {
                execution_state.CrossException(ex, "Can not initialize plugin");
                throw;
            }
        }

        /// <summary>
        /// Executes the plugin immediately and returns when the plugin has finished.
        /// If the plugin executes successfully the plugin result should be avaiable via the <code>ResultAvailable</code> Property.
        /// </summary>
        public void Run()
        {
            if (execution_state.Initialized)
            {
                execution_state.CrossRunning();
                _percentComplete = 0;
                RunInternal();
            }
        }

        /// <summary>
        /// Cancles the plugin and stops execution immediately.
        /// </summary>
        public void Cancel()
        {
            execution_state.CrossCancled();
        }

        #endregion

        #region PluginStateMachine Members

        /// <summary>
        /// Gets or Sets the current state.
        /// </summary>
        public PluginExecutionState State
        {
            get { return execution_state; }
            set
            {
                execution_state = value;
                if (StateChangedEvent != null)
                    StateChangedEvent.Invoke(this, execution_state);
            }
        }

        #endregion

        /// <summary>
        /// InitializeInternal must be implemented by subclasses and will be called by the
        /// <code>Initialize</code> method.
        /// </summary>
        protected abstract void InitializeInternal();

        /// <summary>
        /// Must be implemented by subclasses and will be called by the <code>Cancle</code> method.
        /// </summary>
        protected abstract void CancelInternal();


        private void RunInternal()
        {
            ReportProgress(0);
            try
            {
                while (!Done)
                {
                    try
                    {
                        ReportProgress(doStep());
                    }
                    catch (PluginError error)
                    {
                        LOG.Error("Caught unrecoverable exception", error);
                        execution_state.CrossException(error, error.Message);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        LOG.Error(String.Format("Caught exception in plugin [{0}]", Name), ex);
                        execution_state.CrossException(ex, ex.Message);
                        return;
                    }


                    if (execution_state.Cancled)
                    {
                        CancelInternal();
                        break;
                    }
                }
            }
            finally
            {
                foreach (IDisposable disposable in disposeable_resourecs)
                {
                    Dispose(disposable);
                }
            }
        }

        private void ReportProgress(uint progress)
        {
            if (_percentComplete < progress)
            {
                _percentComplete = progress;
                if (ProgressEvent != null)
                    ProgressEvent.Invoke(this, _percentComplete);
            }
        }


        // TODO add comment
        protected abstract uint doStep();


        protected void done(IPluginResult result)
        {
            execution_state.CrossCompleted(result);
            if (result != null && result.Errors)
            {
                execution_state.CrossError("Result has Errors", result);
            }
        }

        protected void RegisterDisposeableResource(IDisposable a_disposeable)
        {
            if (a_disposeable == null) throw new ArgumentNullException("a_disposeable");
            if (!disposeable_resourecs.Contains(a_disposeable))
            {
                disposeable_resourecs.Add(a_disposeable);
            }
        }

        protected static void Dispose(IDisposable a_diDisposable)
        {
            if (a_diDisposable != null)
            {
                a_diDisposable.Dispose();
            }
        }
    }
}