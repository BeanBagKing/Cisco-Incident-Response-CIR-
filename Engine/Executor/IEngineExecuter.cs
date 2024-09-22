//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 
using System;
using PluginHost;

namespace Recurity.CIR.Engine.Executor
{
    public interface IEngineExecuter
    {
        event ExecutorEvents.PluginEventDelegate PluginEvent;
        event ExecutorEvents.PluginExceptionDelegate PluginExceptionEvent;
        event ExecutorEvents.PluginStepDelegate PluginStep;
        void Execute(CirMain engine);
        
    }

    public class ExecutorEvents
    {
        public delegate void PluginEventDelegate(IBackgroundPlugin plugin, PluginEventType type);

        public delegate void PluginExceptionDelegate(IBackgroundPlugin plugin, PluginEventType type, Exception ex);

        public delegate void PluginStepDelegate(IBackgroundPlugin plugin);
    }
    public enum PluginEventType
    {
        Success,
        Run_Error,
        Init_Success,
        Init_Failed,
        Cancled
    }
}