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
    public interface IHeapBlock32
    {
        UInt64 Address { get; }
        UInt32 DataOffset { get; }

        UInt32 Magic { get; }
        UInt32 PID { get; }
        UInt32 AllocCheck { get; }
        UInt32 AllocName { get; }
        UInt32 AllocPC { get; }
        UInt32 NextBlock { get; }
        UInt32 PrevBlock { get; }
        /// <summary>
        /// The size of the heap block as reported by the inline management data
        /// </summary>
        UInt32 Size { get; }
        UInt32 RefCount { get; }
        UInt32 FreePC { get; }
        UInt32 RedZone { get; }

        bool Used { get; }
        /// <summary>
        /// The size of the heap block, including headers
        /// </summary>
        UInt32 SizeFull { get; }
        UInt32 HeaderSize { get; }
        string AllocNameString { get; }

        IHeapBlockFree32 FreeBlock { get; }
    }
}
