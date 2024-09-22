//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Felix Lindner (fx@recurtiy-labs.com)
// 
using System;
using PluginHost.State;

namespace PluginHost
{
    public interface IBackgroundPlugin : IPlugin
    {
        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin is running. Otherwise <code>false</code>.
        /// </summary>
        bool Running { get; }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin is cancled. Otherwise <code>false</code>.
        /// </summary>
        bool Canceled { get; }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin thrown and exception . Otherwise <code>false</code>.
        /// </summary>
        bool Error { get; }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin has finished successfully and has detected some irregularities.
        /// Otherwise <code>false</code>.
        /// </summary>
        bool Detection { get; }

        /// <summary>
        /// The exception thrown by the plugin if the <code>Error</code>  property returns <code>true</code> otherwise <code>false</code>
        /// </summary>
        Exception ReportedError { get; }

        /// <summary>
        /// The current plugin progress in percent.
        /// </summary>
        uint PercentComplete { get; }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin has finished successfully and has a result submitted. Otherwise <code>false</code>.
        /// </summary>
        bool ResultAvailable { get; }

        /// <summary>
        /// Returns the current state of execution.
        /// </summary>
        PluginExecutionState ExecutionState { get; }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin is in the state Ready. Otherwise <code>false</code>.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Retruns <code>true</code> if and only if the plugin execution is done. Otherwise <code>false</code>.
        /// </summary>
        bool Done { get; }


        /// <summary>
        /// The plugins Setup - procedure.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Executes the plugin immediately and returns when the plugin has finished.
        /// If the plugin executes successfully the plugin result should be avaiable via the <code>ResultAvailable</code> Property.
        /// </summary>
        void Run();


        /// <summary>
        /// Cancles the plugin and stops execution immediately.
        /// </summary>
        void Cancel();

        /// <summary>
        /// This event is invoked if the plugin state changes.
        /// </summary>
        event PluginEvents.StateChanged StateChangedEvent;

        /// <summary>
        /// This event is invoked if the plugin progress changes.
        /// </summary>
        event PluginEvents.ProgressReport ProgressEvent;
    }
}