// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Engine.PluginResults
{
    public class CiscoCoreFile : SomeFile
    {
        public CiscoCoreFile( FileInfo info ) : base(info) { }
    }
}
