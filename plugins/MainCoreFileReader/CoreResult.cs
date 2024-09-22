// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using Recurity.CIR.CiscoCore;
using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Plugins.CiscoCore
{
    public class CoreResult : CiscoCoreMemory
    {
        public CoreResult(FileRepresentation a_file, UInt64 baseAddress, bool bigEndian)
            : base(a_file, baseAddress, bigEndian)
        {
        }
    }
}