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
    public interface IReportTransformer
    {
       void Transform(DirectoryInfo targetDirectory, IReport report);
    }
}
