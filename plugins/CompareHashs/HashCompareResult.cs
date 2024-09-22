// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Robert Tezli (robert@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using Recurity.CIR.Engine.Interfaces;
using PluginHost;
using Recurity.CIR.Engine.Report;

namespace Recurity.CIR.Plugins.CompareHashs
{
    internal class HashCompareResult : IPluginReporter, IPluginResult 
    {
        string pluginname;
        string outcome;
        bool success;

        internal HashCompareResult(string aPluginname, bool aSuccess, string aOutcome)
        {
            pluginname = aPluginname;
            success = aSuccess;
            outcome = aOutcome;
        }

        public Recurity.CIR.Engine.Report.IReport Report
        {
            get { 
                IReport report = ReportFactory.Instance.CreatePluginReport();
                // this should be what the plugin does -- not in use 
                report.Description = "";
                // if the detailed report has something to expand on. set to 1 if you use 
                // report.AddReportNode 
                report.Details = 0;
                // changes the color of the output
                report.State = (uint)(success?0:1);
                // is shown in the output as the plugin result
                report.Summary = success ? outcome : "Image not found";
                report.Author = pluginname;
                return report;
            }
        }

        public bool Errors
        {
            get { return !success; }
        }

       
    }
}
