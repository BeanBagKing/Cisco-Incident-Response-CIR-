// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using Recurity.CIR.Engine.Helper;

namespace Recurity.CIR.Engine.Report
{
    public class ReportFactory : Singleton<ReportFactory>
    {
        private ReportFactory() { }

        public IReport CreatePluginReport()
        {
            Report retval = new Report();
            retval.IsPlugin = true;
            return retval;
        }

        public IReport CreateMetaReport()
        {
            Report retval = new Report();
            retval.IsPlugin = false;
            return retval;
        }

        public IReport Combine(IList<IReport> reports)
        {
            return new XmlReportComposite(reports);
        }

        public IReportTransformer Transformer()
        {
            return new XmlReportTransfromer();
        }
    }
}
