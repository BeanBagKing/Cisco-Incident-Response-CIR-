// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

namespace Recurity.CIR.Plugins.ProcessList.Offsets
{
    public class ProcessListOffsets_12_3J : IProcessListOffsets
    {
        #region IProcessListOffsets Members

        public ulong Stack              { get { return 0x00; } }
        public ulong StackOld           { get { return 0x04; } }
        public ulong IsProfiled         { get { return 0x74; } }
        public ulong IsAnalyzed         { get { return 0x75; } }
        public ulong IsBlockedAtCrash   { get { return 0x76; } }
        public ulong IsCrashed          { get { return 0x77; } }
        public ulong IsKilled           { get { return 0x78; } }
        public ulong IsCorrupt          { get { return 0x79; } }
        public ulong IsPreferringNew    { get { return 0x7A; } }
        public ulong IsOnOldQueue       { get { return 0x7B; } }
        public ulong IsWakeupPosted     { get { return 0x7C; } }
        public ulong IsProfiledProcess  { get { return 0x7D; } }
        public ulong IsProcessArgValid  { get { return 0x7E; } }
        public ulong IsInitProcess      { get { return 0x7F; } }
        //
        public ulong PID                { get { return 0x8C; } }
        public ulong Caller             { get { return 0x90; } }
        public ulong Callee             { get { return 0x94; } }
        //4Bytes
        public ulong TotalMalloc       { get { return 0x9C; } }
        public ulong TotalFree         { get { return 0xA0; } }
        public ulong TotalPoolAlloc    { get { return 0xA4; } }
        public ulong TotalPoolFree     { get { return 0xA8; } }
        public ulong Invokations        { get { return 0xE8; } }
        //16 Bytes in Betweeen
        public ulong Name               { get { return 0xF8; } }
        //4 Bytes
        public ulong CpuTicks5sec     { get { return 0x100; } }
        public ulong CpuPercent5sec   { get { return 0x104; } }
        public ulong CpuPercent1min   { get { return 0x108; } }
        public ulong CpuPercent5min   { get { return 0x10C; } }
        public ulong StackSize          { get { return 0x110; } }
        public ulong StackLowLimit      { get { return 0x114; } }

        #endregion
    }
}