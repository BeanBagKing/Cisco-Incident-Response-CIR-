// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

namespace Recurity.CIR.Plugins.ProcessList
{
    internal interface IProcessListOffsets
    {
        UInt64 Name { get; }
        UInt64 PID { get; }

        // stack
        UInt64 Stack { get; }
        UInt64 StackOld { get; }
        UInt64 StackSize { get; }
        UInt64 StackLowLimit { get; }

        // performance
        UInt64 CpuTicks5sec { get; }
        UInt64 CpuPercent5sec { get; }
        UInt64 CpuPercent1min { get; }
        UInt64 CpuPercent5min { get; }
        UInt64 Invokations { get; }

        // memory 
        UInt64 TotalMalloc { get; }
        UInt64 TotalFree { get; }
        UInt64 TotalPoolAlloc { get; }
        UInt64 TotalPoolFree { get; }

        // calls
        UInt64 Caller { get; }
        UInt64 Callee { get; }

        // flags
        UInt64 IsProfiled { get; }
        UInt64 IsAnalyzed { get; }
        UInt64 IsBlockedAtCrash { get; }
        UInt64 IsCrashed { get; }
        UInt64 IsKilled { get; }
        UInt64 IsCorrupt { get; }
        UInt64 IsPreferringNew { get; }
        UInt64 IsOnOldQueue { get; }
        UInt64 IsWakeupPosted { get; }
        UInt64 IsProfiledProcess { get; }
        UInt64 IsProcessArgValid { get; }
        UInt64 IsInitProcess { get; }
    }
}
