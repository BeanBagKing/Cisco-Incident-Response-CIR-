// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

namespace Recurity.CIR.Engine.Report
{
    internal class XmlReportComposite : Report
    {
        private readonly IList<IReport> reports;
        internal XmlReportComposite(IList<IReport> reports)
        {
            if (reports == null)
            {
                throw new ArgumentException("reports must not be null");
            }
            this.reports = reports;
        }

        public override void WriteTo(System.IO.TextWriter writer)
        {
            writer.WriteLine("<CIR>");
            foreach (IReport report in this.reports)
            {
                report.WriteTo(writer);
            }
            writer.WriteLine("</CIR>");
            writer.Flush();
        }


    }
}
