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
    public class CheckHeapResultRecord
    {
        public IHeapBlock32 block;
        public string issue;

        public CheckHeapResultRecord( IHeapBlock32 block, string issue )
        {
            this.issue = issue;
            this.block = block;
        }
    }

    public class CheckHeapResults : ICheckHeapResults, IPluginReporter
    {
        private readonly string _pluginName;
        private List<CheckHeapResultRecord> _results;

        public CheckHeapResults( string pluginName )
        {
            _pluginName = pluginName;
            _results = new List<CheckHeapResultRecord>();
        }

        public List<CheckHeapResultRecord> Results
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
            if ( _results.Count > 0 )
            {
                foreach ( CheckHeapResultRecord rec in _results )
                {
                    sb.AppendLine( rec.block.Address.ToString( "X" ) + ": " + rec.issue );
                }
            }
            else
            {
                sb.AppendLine( "No entries" );
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

                if ( _results.Count > 0 )
                {
                    report.State = 2;
                    report.Summary = "IOS Heap shows detectable inconsistencies";

                    foreach ( CheckHeapResultRecord r in _results )
                    {
                        CheckHeapReportResult reportResult = new CheckHeapReportResult();
                        reportResult.blockAddress = r.block.Address;
                        reportResult.issue = r.issue;
                        report.AddReportNode( reportResult );
                    }
                }
                else
                {
                    report.State = 0;
                    report.Summary = "IOS Heap consistency checks passed";
                }

                return report;
            }
        }
        public bool Errors { get { return Report.State == 2; } }

        #endregion

    }
}
