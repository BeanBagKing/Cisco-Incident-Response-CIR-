// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

namespace Recurity.CIR.Engine.Interfaces
{
    public interface IMemoryMap
    {
        IVirtualMemorySection GetMemorySection( UInt64 virtualAddress );
        List<IVirtualMemorySection> GetKnownMemorySections();
    }
}
