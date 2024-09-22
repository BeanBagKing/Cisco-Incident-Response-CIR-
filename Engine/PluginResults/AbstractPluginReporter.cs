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

namespace Recurity.CIR.Engine.PluginResults
{
    public abstract class AbstractPluginReporter : IPluginReporter
    {
        private readonly String who;
        private readonly String summary;
        private readonly bool success;

        public AbstractPluginReporter(String who, bool success, String summary)
        {
            this.who = who;
            this.success = success;
            this.summary = summary;
        }
        public virtual IReport Report
        {
            get 
            {
                Recurity.CIR.Engine.Report.IReport report = ReportFactory.Instance.CreatePluginReport();
                report.Details = 0;
                report.State = (uint)(this.success ? 0 : 2);
                report.Summary = this.summary;
                report.Author = this.who;
                // TODO: (simonw) add description
                report.Description = "";
                return report;
            
            }
        }

        public bool Errors
        {
            get { return Report.State == 2; }
        }

        
    }
}
