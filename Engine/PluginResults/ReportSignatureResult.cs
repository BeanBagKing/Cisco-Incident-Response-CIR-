// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using PluginHost;
using Recurity.CIR.Engine.PluginResults.Xml;
using Recurity.CIR.Engine.Report;

namespace Recurity.CIR.Engine.PluginResults
{
    public class ReportSignatureResult : AbstractPluginReporter, IPluginResult
    {
        private readonly String imageSignature;
        private readonly String coreSignature;
         public ReportSignatureResult(String who, bool success, String summary, String imageSignature, String coreSignature)
           : base(who, success, summary)
        {
            this.imageSignature = imageSignature;
            this.coreSignature = coreSignature;
        }

        public override Recurity.CIR.Engine.Report.IReport Report
        {
            get
            {
                IReport report = base.Report;
                report.Details = 1;
                ReportSignaturesResult sigs = new ReportSignaturesResult();
                sigs.CoreSignature = this.coreSignature;
                sigs.ImageSignature = this.imageSignature;
                report.Summary = "Core and Image signature reported";
                report.AddReportNode(sigs);
                return report;
            }
        }
    }
}
