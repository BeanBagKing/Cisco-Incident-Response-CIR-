// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using Recurity.CIR.Engine.Report;

namespace Recurity.CIR.Engine.Interfaces
{
    public interface IPluginReporter
    {
        IReport Report
        {
            get;
        }

        bool Errors { get; }
    }
}
