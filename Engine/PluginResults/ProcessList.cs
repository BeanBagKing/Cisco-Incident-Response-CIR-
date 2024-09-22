// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.Report;
using Recurity.CIR.Engine.PluginResults.Xml;

namespace Recurity.CIR.Engine.PluginResults
{
    public sealed class ProcessList : IProcessList, IPluginReporter
    {
        private const float PERFORMANCE_DIVISOR = (float)1024.0;

        private readonly List<IProcessInformation> _processList;
        private readonly string pluginName;

        public ProcessList(string pluginName) 
        {
            _processList = new List<IProcessInformation>();
            this.pluginName = pluginName;
            
        }

        public List<IProcessInformation> Processes
        {
            get 
            { 
                return _processList;
            }        
        }

        public IProcessInformation this[ uint pid ]
        {
            get
            {
                return GetProcessByPID( pid );
            }
        }

        public IProcessInformation GetProcessByPID( uint pid )
        {
            foreach ( IProcessInformation procInfo in _processList )
            {
                if ( procInfo.PID == pid )
                {
                    if ( 0 != procInfo.RecordLocation )
                    {
                        return procInfo;
                    }
                }
            }

            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append( base.ToString() );
            sb.AppendLine( " - Process List" );

            foreach ( IProcessInformation pi in _processList )
            {
                sb.AppendFormat(
                    "[{29,8:X}] {0,50}{30,1}: {1,4:d} Stack: {2,8:X},{3,8:X},{4,8:X},{5,8:X}" +
                    "   CPU: {6,5:d}, {7,6:n2}%, {8,6:n2}%, {9,6:n2}% (inv:{10,8:d})" +
                    "   Memory: {11,8:X}, {12,8:X}, {13,8:X}, {14,8:X}"+
                    "   Caller: {15,9:X}, {16,9:X}"+
                    " {17}{18}{19}{20}{21}{22}{23}{24}{25}{26}{27}{28}",

                    pi.Name,    // 0
                    pi.PID,     // 1

                    pi.Stack,   // 2
                    pi.StackOld,            // 3
                    pi.StackSize,           // 4
                    pi.StackLowLimit,       // 5
                    pi.CPU_ticks_5sec,      // 6
                    (float)pi.CPU_percent_5sec / PERFORMANCE_DIVISOR,    // 7
                    (float)pi.CPU_percent_1min / PERFORMANCE_DIVISOR,    // 8
                    (float)pi.CPU_percent_5min / PERFORMANCE_DIVISOR,    // 9
                    pi.Invokations,         // 10

                    pi.Total_Malloc,        // 11
                    pi.Total_Free,          // 12
                    pi.Total_PoolAlloc,     // 13
                    pi.Total_PoolFree,      // 14

                    pi.Caller,              // 15
                    pi.Callee,              // 16

                    pi.IsProfiled?"profiled,":"",          // 17
                    pi.IsAnalyzed?"analyzed,":"",          // 18
                    pi.IsBlockedAtCrash?"blocked at crash,":"",    // 19
                    pi.IsCrashed?"crashed,":"",           // 20
                    pi.IsKilled?"killed,":"",            // 21
                    pi.IsCorrupt?"corrupt,":"",           // 22
                    pi.IsPreferringNew?"prefers new,":"",     // 23
                    pi.IsOnOldQueue?"on old queue,":"",        // 24
                    pi.IsWakeupPosted?"wakeup posted,":"",      // 25
                    pi.IsProfiledProcess?"profiled process,":"",   // 26
                    pi.IsProcessArgValid?"process argument valid,":"",   // 27
                    pi.IsInitProcess?"is init process":"",        // 28

                    pi.RecordLocation,
                    (pi.InProcessArray?" ":"+")
                );

                sb.AppendLine();
            }

            return sb.ToString();
        }

        #region IPluginReporter Members

        public Recurity.CIR.Engine.Report.IReport Report
        {
            get 
            {
                IReport report = ReportFactory.Instance.CreatePluginReport();
                report.Details = 1;
                report.Summary = "IOS Process List extracted successfully";
                report.Author = this.pluginName;
                // TODO: (simonw) add Descriptions to all results!! 
                report.Description = "";
                report.State = 0;

                foreach ( IProcessInformation pi in _processList )
                {
                    ProcessRecordResult result = new ProcessRecordResult();
                    
                    result.processName = pi.Name;
                    result.pid = pi.PID;
                    result.stackAddress = pi.Stack;
                    result.stackAddressOld = pi.StackOld;
                    result.stackSize = pi.StackSize;
                    result.stackLowLimit = pi.StackLowLimit;
                    result.cpuTicks = pi.CPU_ticks_5sec;
                    result.cpuUsage5sec = (float)pi.CPU_percent_5sec / PERFORMANCE_DIVISOR;
                    result.cpuUsage1min = (float)pi.CPU_percent_1min / PERFORMANCE_DIVISOR;
                    result.cpuUsage5min = (float)pi.CPU_percent_5min / PERFORMANCE_DIVISOR;
                    result.cpuInvoke = pi.Invokations;
                    result.memMalloc = pi.Total_Malloc;
                    result.memFree = pi.Total_Free;
                    result.memPoolAlloc = pi.Total_PoolAlloc;
                    result.memPoolFree = pi.Total_PoolFree;
                    result.caller = pi.Caller;
                    result.callee = pi.Callee;
                    
                    result.isProfiled = pi.IsProfiled;
                    result.isAnalyzed = pi.IsAnalyzed;
                    result.isBlockedAtCrash = pi.IsBlockedAtCrash;
                    result.isCrashed = pi.IsCrashed;
                    result.isKilled = pi.IsKilled;
                    result.isCorrupt = pi.IsCorrupt;
                    result.isPreferringNew = pi.IsPreferringNew;
                    result.isOnOldQueue = pi.IsOnOldQueue;
                    result.isWakeupPosted = pi.IsWakeupPosted;
                    result.isProfiledProcess = pi.IsProfiledProcess;
                    result.isProcessArgValid = pi.IsProcessArgValid;
                    result.isInitProcess = pi.IsInitProcess;

                    result.processRecordAddress = pi.RecordLocation;
                    result.fromProcessArray = pi.InProcessArray;

                    report.AddReportNode( result );
                }

                return report;
            }
        }
        public bool Errors { get { return Report.State == 2; } }

        #endregion
    }
}
