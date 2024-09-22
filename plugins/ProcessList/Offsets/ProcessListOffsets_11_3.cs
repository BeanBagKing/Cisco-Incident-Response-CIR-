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
    public class ProcessListOffsets_11_3 : IProcessListOffsets
    {
        #region IProcessListOffsets Members

        public ulong Stack { get { return 0x00; } }
        public ulong StackOld { get { return 0x04; } }
        public ulong IsProfiled { get { return 0x74; } }
        public ulong IsAnalyzed { get { return 0x75; } }
        public ulong IsBlockedAtCrash { get { return 0x76; } }
        public ulong IsCrashed { get { return 0x77; } }
        public ulong IsKilled { get { return 0x78; } }
        public ulong IsCorrupt { get { return 0x79; } }
        public ulong IsPreferringNew { get { return 0x7A; } }
        public ulong IsOnOldQueue { get { return 0x7B; } }
        public ulong IsWakeupPosted { get { return 0x7C; } }
        public ulong IsProfiledProcess { get { return 0x7D; } }
        public ulong IsProcessArgValid { get { return 0x7E; } }
        public ulong IsInitProcess { get { return 0x7F; } }
        // 4 additional bytes in between 
        public ulong PID                { get { return 0x88; } }//DONE
        public ulong Caller             { get { return 0x8C; } }//DONE
        public ulong Callee             { get { return 0x90; } }//DONE
        // 4 bytes in between 
        public ulong TotalMalloc { get { return 0x9C; } }
        public ulong TotalFree { get { return 0xA0; } }
        public ulong TotalPoolAlloc { get { return 0xA4; } }
        public ulong TotalPoolFree { get { return 0xA8; } }
        public ulong Invokations { get { return 0xD8; } }
        // 10 bytes from nowhere
        public ulong Name               { get { return 0xD0; } }//DONE
        public ulong CpuTicks5sec     { get { return 0xD4; } }//DONE
        public ulong CpuPercent5sec   { get { return 0xD8; } }//DONE
        public ulong CpuPercent1min   { get { return 0xDC; } }//DONE
        public ulong CpuPercent5min   { get { return 0xE0; } }//DONE
        public ulong StackSize          { get { return 0xE4; } }//DONE
        public ulong StackLowLimit      { get { return 0xE8; } }//DONE

        #endregion
    }
}
