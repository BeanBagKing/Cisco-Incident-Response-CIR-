// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using PluginHost;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.Report;
using Recurity.CIR.Engine.PluginResults.Xml;

namespace Recurity.CIR.Engine.PluginResults
{
    public class SegmentCompareResult : AbstractPluginReporter, IPluginResult
    {

        private List<SegmentDiffResult> _diffList;

        public SegmentCompareResult(String who, bool success, String summary)
            : base(who, success, summary)
        {
            _diffList = new List<SegmentDiffResult>();

        }

        public List<SegmentDiffResult> Diff
        {
            get
            {
                return _diffList;
            }
        }

        public override Recurity.CIR.Engine.Report.IReport Report
        {
            get
            {
                IReport report = base.Report;

                if ( _diffList.Count > 0 )
                    report.Details = 1;

                foreach ( SegmentDiffResult diff in _diffList )
                {                
                    report.AddReportNode( diff );
                }

                return report;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine( base.ToString() );
            sb.AppendFormat( "Differences found: {0}", _diffList.Count );
            sb.AppendLine();
            foreach ( SegmentDiffResult diff in _diffList )
            {
                sb.AppendFormat("{4,10:s}: {0,9:X8} {1,9:X8} {2,9:X8} for {3:d} bytes",
                    diff.virtualAddress,
                    diff.offsetElf,
                    diff.offsetCore,
                    diff.diffLength,
                    diff.SegmentName );
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
