// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.IO;
using Recurity.PluginHost;

namespace Recurity.CIR.Engine.Report
{
    public class ReportSerializer
    {
        private readonly List<IReport> reports = new List<IReport>();

        public void Add(IReport report)
        {
            if (report == null) throw new ArgumentNullException("report");
            reports.Add(report);
        }

        public void AddRange(IList<IReport> report)
        {
            if (report == null) throw new ArgumentNullException("report");
            reports.AddRange(report);
        }

        public void Serialize(DirectoryInfo aAutputDirectory)
        {
            IReport report = ReportFactory.Instance.Combine(reports);
            IReportTransformer trans = ReportFactory.Instance.Transformer();
            trans.Transform(aAutputDirectory, report);
        }
        
        public void AddLicenceReport()
        {
            IReport report = ReportFactory.Instance.CreateMetaReport();
            report.Summary = HostOwner.Owner;
            report.Description = HostOwner.LicenseComment;
            report.Author = "LicenseInformation";
            report.Details = 0;
            report.State = 0;
            Add(report);
            
        }
    }
}
