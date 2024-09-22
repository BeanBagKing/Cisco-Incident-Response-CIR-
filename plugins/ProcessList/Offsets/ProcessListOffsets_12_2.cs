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
    public class ProcessListOffsets_12_2 : IProcessListOffsets
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
        public ulong PID                { get { return 0x88; } }
        public ulong Caller             { get { return 0x8C; } }
        public ulong Callee             { get { return 0x90; } }
        public ulong TotalMalloc       { get { return 0x94; } }
        public ulong TotalFree         { get { return 0x98; } }
        public ulong TotalPoolAlloc    { get { return 0x9C; } }
        public ulong TotalPoolFree     { get { return 0xA0; } }
        public ulong Invokations        { get { return 0xD0; } }
        public ulong Name               { get { return 0xD8; } }
        public ulong CpuTicks5sec     { get { return 0xE0; } }
        public ulong CpuPercent5sec   { get { return 0xE4; } }
        public ulong CpuPercent1min   { get { return 0xE8; } }
        public ulong CpuPercent5min   { get { return 0xEC; } }
        public ulong StackSize          { get { return 0xF0; } }
        public ulong StackLowLimit      { get { return 0xF4; } }


        /*


         public ulong Name
        {
            get { return 0xD8; }
        }

         public ulong PID
        {
            get { return 0x88; }
        }

         public ulong Stack
        {
            get { return 0x00; }
        }

         public ulong StackOld
        {
            get { return 0x04; }
        }

         public ulong StackSize
        {
            get { return 0xF0; }
        }

         public ulong StackLowLimit
        {
            get { return 0xF4; }
        }

         public ulong CPU_ticks_5sec
        {
            get { return 0xE0; }
        }

         public ulong CPU_percent_5sec
        {
            get { return 0xE4; }
        }

         public ulong CPU_percent_1min
        {
            get { return 0xE8; }
        }

         public ulong CPU_percent_5min
        {
            get { return 0xEC; }
        }

         public ulong Invokations
        {
            get { return 0xD0; }
        }



         public ulong Total_Malloc
        {
            get { return 0x94; }
        }

         public ulong Total_Free
        {
            get { return 0x98; }
        }

         public ulong Total_PoolAlloc
        {
            get { return 0x9C; }
        }

         public ulong Total_PoolFree
        {
            get { return 0xA0; }
        }

         public ulong Caller
        {
            get { return 0x8C; }
        }

         public ulong Callee
        {
            get { return 0x90; }
        }

         public ulong IsProfiled
        {
            get { return 0x74; }
        }

         public ulong IsAnalyzed
        {
            get { return 0x75; }
        }

         public ulong IsBlockedAtCrash
        {
            get { return 0x76; }
        }

         public ulong IsCrashed
        {
            get { return 0x77; }
        }

         public ulong IsKilled
        {
            get { return 0x78; }
        }

         public ulong IsCorrupt
        {
            get { return 0x79; }
        }

         public ulong IsPreferringNew
        {
            get { return 0x7A; }
        }

         public ulong IsOnOldQueue
        {
            get { return 0x7B; }
        }

         public ulong IsWakeupPosted
        {
            get { return 0x7C; }
        }

         public ulong IsProfiledProcess
        {
            get { return 0x7D; }
        }

         public ulong IsProcessArgValid
        {
            get { return 0x7E; }
        }

         public ulong IsInitProcess
        {
            get { return 0x7F; }
        }
         */

        #endregion
    }
}
