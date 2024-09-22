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
    public class IOSSignatureElf : AbstractIOSSignature, IIOSSignatureElf
    {
        private readonly string pluginName;

        public IOSSignatureElf(string pluginName)
        {
            this.pluginName = pluginName;
        }



        protected override IReport CreateReportInstance()
        {
            Recurity.CIR.Engine.Report.IReport retval = ReportFactory.Instance.CreatePluginReport();
            retval.Author = this.pluginName;
            retval.Summary = "Not implemented yet.";
            retval.Details = 1;
            retval.State = 0;
            // TODO: (simonw) add description
            retval.Description = "";
            return retval;
        }
    }
}
