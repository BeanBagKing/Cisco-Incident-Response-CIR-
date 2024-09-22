// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginResults.Xml;
using Recurity.CIR.Engine.Report;

namespace Recurity.CIR.Engine.PluginResults
{
    public class CheckHeapUseResultRecord : ICheckHeapUseResultRecord
    {
        private IProcessInformation _process;
        private UInt64 _heapBlocks;
        private UInt64 _heapSize;

        public CheckHeapUseResultRecord( IProcessInformation process, UInt64 blocks, UInt64 size )
        {
            _process = process;
            _heapBlocks = blocks;
            _heapSize = size;
        }

        public IProcessInformation Process 
        { 
            get { return _process; } 
        }

        public UInt64 HeapBlocks 
        { 
            get { return _heapBlocks; } 
        }

        public UInt64 HeapSize
        {
            get { return _heapSize; }
        }
    }


    public class CheckHeapUseResult : ICheckHeapUseResult, IPluginReporter
    {
        private List<ICheckHeapUseResultRecord> _results;
        private string _pluginName;

        public CheckHeapUseResult(string pluginName)
        {
            _results = new List<ICheckHeapUseResultRecord>();
            _pluginName = pluginName;
        }

        public List<ICheckHeapUseResultRecord> Results 
        {
            get
            {
                return _results;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( this.GetType().ToString() );

            foreach ( ICheckHeapUseResultRecord rec in _results )
            {
                sb.AppendFormat( "PID {0,8:d} / ({3,30}): {1,8:d} heap blocks, {2,8:d} bytes allocated in total",
                    rec.Process.PID,
                    rec.HeapBlocks,
                    rec.HeapSize,
                    rec.Process.Name );
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

                report.Author = _pluginName;
                // TODO: (simonw) add Descriptions to all results!! 
                report.Description = "";
                report.State = 0;
                report.Summary = "Per-process heap block ownership list created";

                foreach ( ICheckHeapUseResultRecord r in _results )
                {
                    CheckHeapUseReportResult result = new CheckHeapUseReportResult();
                    result.pid = r.Process.PID;
                    result.processName = r.Process.Name;
                    result.numberOfBlock = r.HeapBlocks;
                    result.sumOfBytes = r.HeapSize;
                    report.AddReportNode( result );
                }

                return report;
            }
        }
        public bool Errors { get { return Report.State == 2; } }

        #endregion

    }
}
