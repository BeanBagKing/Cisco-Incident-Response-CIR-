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
    public interface IHeapBlockFree32
    {
        UInt32 Magic { get; }
        UInt32 LastFreePC { get; }
        // UInt32 Dummy
        // UInt32 Dummy
        UInt32 FreeNext { get; }
        UInt32 FreePrev { get; }
    }
}
