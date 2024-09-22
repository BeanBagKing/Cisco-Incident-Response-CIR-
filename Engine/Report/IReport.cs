// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Recurity.CIR.Engine.Report
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReport
    {
        string Summary { get; set; }
        uint Details { get; set; }
        string Author { get; set; }
        uint State { get; set; }
        string Description { get; set; } 
        void AddReportNode(IReportNode node);
        void WriteTo(TextWriter writer);
    }
}
