// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.CiscoCore;
using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Plugins.IOCoreFileReader
{
    //public class IOCoreResult : CiscoIOCoreMemory, ICiscoIOCoreMemory, ICiscoIOCoreMemoryMap
    public class IOCoreResult :CiscoIOCoreMemory
    {
        public IOCoreResult( FileRepresentation a_file, UInt64 topAddress, bool bigEndian )
            : base( a_file, topAddress, bigEndian )
        {
        }
    }
}
