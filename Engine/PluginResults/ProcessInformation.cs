// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Engine.PluginResults
{
    public class ProcessInformation : IProcessInformation 
    {
        protected UInt64 _RecordLocation;
        protected bool _is_in_ProcessArray;

        protected string _name;
        protected UInt64 _pid;
        protected UInt64 _stack;
        protected UInt64 _stackOld;
        protected UInt64 _stackSize;
        protected UInt64 _stackLowLimit;
        protected UInt64 _cpuTick5sec;
        protected UInt64 _cpuPercent5min;
        protected UInt64 _cpuPercent1min;
        protected UInt64 _cpuPercent5sec;
        protected UInt64 _invocations;
        protected UInt64 _totalMalloc;
        protected UInt64 _totalFree;
        protected UInt64 _totalPoolAlloc;
        protected UInt64 _totalPoolFree;
        protected UInt64 _caller;
        protected UInt64 _callee;
        protected bool _is_profiled;
        protected bool _is_analyzed;
        protected bool _is_blocked;
        protected bool _is_crashed;
        protected bool _is_killed;
        protected bool _is_corrupt;
        protected bool _is_preferring_new;
        protected bool _is_on_old_queue;
        protected bool _is_wakeup_posted;
        protected bool _is_profiled_process;
        protected bool _is_process_arg_valid;
        protected bool _is_init_process;


        #region IProcessInformation Members

        public UInt64 RecordLocation
        {
            get { return _RecordLocation; }
            set { _RecordLocation = value; }
        }

        public bool InProcessArray
        {
            get { return _is_in_ProcessArray; }
            set { _is_in_ProcessArray = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ulong PID
        {
            get { return _pid; }
            set { _pid = value; }
        }

        public ulong Stack
        {
            get { return _stack; }
            set { _stack = value; }
        }

        public ulong StackOld
        {
            get { return _stackOld; }
            set { _stackOld = value; }
        }

        public ulong StackSize
        {
            get { return _stackSize; }
            set { _stackSize = value; }
        }

        public ulong StackLowLimit
        {
            get { return _stackLowLimit; }
            set { _stackLowLimit = value; }
        }

        public ulong CPU_ticks_5sec
        {
            get { return _cpuTick5sec; }
            set { _cpuTick5sec = value; }
        }

        public ulong CPU_percent_5sec
        {
            get { return _cpuPercent5sec; }
            set { _cpuPercent5sec = value; }
        }

        public ulong CPU_percent_1min
        {
            get { return _cpuPercent1min; }
            set { _cpuPercent1min = value; }
        }

        public ulong CPU_percent_5min
        {
            get { return _cpuPercent5min; }
            set { _cpuPercent5min = value; }
        }

        public ulong Invokations
        {
            get { return _invocations; }
            set { _invocations = value; }
        }

        public ulong Total_Malloc
        {
            get { return _totalMalloc; }
            set { _totalMalloc = value; }
        }

        public ulong Total_Free
        {
            get { return _totalFree; }
            set { _totalFree = value; }
        }

        public ulong Total_PoolAlloc
        {
            get { return _totalPoolAlloc; }
            set { _totalPoolAlloc = value; }
        }

        public ulong Total_PoolFree
        {
            get { return _totalPoolFree; }
            set { _totalPoolFree = value; }
        }

        public ulong Caller
        {
            get { return _caller; }
            set { _caller = value; }
        }

        public ulong Callee
        {
            get { return _callee; }
            set { _callee = value; }
        }

        public bool IsProfiled
        {
            get { return _is_profiled; }
            set { _is_profiled = value; }
        }

        public bool IsAnalyzed
        {
            get { return _is_analyzed; }
            set { _is_analyzed = value; }
        }

        public bool IsBlockedAtCrash
        {
            get { return _is_blocked; }
            set { _is_blocked = value; }
        }

        public bool IsCrashed
        {
            get { return _is_crashed; }
            set { _is_crashed = value; }
        }

        public bool IsKilled
        {
            get { return _is_killed; }
            set { _is_killed = value; }
        }

        public bool IsCorrupt
        {
            get { return _is_corrupt; }
            set { _is_corrupt = value; }
        }

        public bool IsPreferringNew
        {
            get { return _is_preferring_new; }
            set { _is_preferring_new = value; }
        }

        public bool IsOnOldQueue
        {
            get { return _is_on_old_queue; }
            set { _is_on_old_queue = value; }
        }

        public bool IsWakeupPosted
        {
            get { return _is_wakeup_posted; }
            set { _is_wakeup_posted = value; }
        }

        public bool IsProfiledProcess
        {
            get { return _is_profiled_process; }
            set { _is_profiled_process = value; }
        }

        public bool IsProcessArgValid
        {
            get { return _is_process_arg_valid; }
            set { _is_process_arg_valid = value; }
        }

        public bool IsInitProcess
        {
            get { return _is_init_process; }
            set { _is_init_process = value; }
        }

        #endregion
    }
}
