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
    public interface IProcessInformation
    {
        UInt64 RecordLocation { get; }
        bool InProcessArray { get; }

        string Name { get; }
        UInt64 PID { get; }

        // stack
        UInt64 Stack { get; }
        UInt64 StackOld { get; }
        UInt64 StackSize { get; }
        UInt64 StackLowLimit { get; }

        // performance
        UInt64 CPU_ticks_5sec { get; }
        UInt64 CPU_percent_5sec { get; }
        UInt64 CPU_percent_1min { get; }
        UInt64 CPU_percent_5min { get; }
        UInt64 Invokations { get; }

        // memory 
        UInt64 Total_Malloc { get; }
        UInt64 Total_Free { get; }
        UInt64 Total_PoolAlloc { get; }
        UInt64 Total_PoolFree { get; }

        // calls
        UInt64 Caller { get; }
        UInt64 Callee { get; }

        // flags
        bool IsProfiled { get; }
        bool IsAnalyzed { get; }
        bool IsBlockedAtCrash { get; }
        bool IsCrashed { get; }
        bool IsKilled { get; }
        bool IsCorrupt { get; }
        bool IsPreferringNew { get; }
        bool IsOnOldQueue { get; }
        bool IsWakeupPosted { get; }
        bool IsProfiledProcess { get; }
        bool IsProcessArgValid { get; }
        bool IsInitProcess { get; }
    }
}
