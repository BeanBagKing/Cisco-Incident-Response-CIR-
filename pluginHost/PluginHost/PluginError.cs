//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 
using System;

namespace PluginHost
{
    /// <summary>
    /// This Exception indicates an unrecoverable error. 
    /// If this exception is caught the Application must report the Error
    /// and exit immediately.
    /// </summary>
    public class PluginError : Exception
    {

        public PluginError(string message, Exception cause):base(message, cause)
        {
        }
        public PluginError(string message)
            : base(message)
        {
        }
    }
}
