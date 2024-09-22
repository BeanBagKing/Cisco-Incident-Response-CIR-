// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.Report;

namespace Recurity.CIR.Engine.PluginResults
{
    public class IOSSignatureCore : AbstractIOSSignature, IIOSSignatureCore
    {
        private readonly string pluginName;
        public IOSSignatureCore(string pluginName) { 
            this.pluginName = pluginName;
        }
        protected override IReport CreateReportInstance()
        {
            Recurity.CIR.Engine.Report.IReport retval = ReportFactory.Instance.CreatePluginReport();
            retval.Author = this.pluginName;
            retval.Summary = "Not implemented yet.";
            retval.Details = 1;
            // TODO: (simonw) add description
            retval.Description = "";
            retval.State = 0;
            return retval;
        }
        
    }
}
