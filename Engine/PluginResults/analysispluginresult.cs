// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Engine.PluginResults
{
    public class AnalysisPluginResult : IAnalysisReportResult
    {
        protected bool _ok;
        protected string _xml;
        protected string _pluginName;

        public AnalysisPluginResult( string reporter, bool ok, string xml )
        {
            _ok = ok;
            _xml = xml;
            _pluginName = reporter;
        }

        public bool OK
        {
            get 
            {  
                return _ok;
            }
        }

        public string XML
        {
            get 
            { 
                return _xml;
            }
        }

        public string Reporter
        {
            get
            {
                return _pluginName;
            }
        }
    }
}
