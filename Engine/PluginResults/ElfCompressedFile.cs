// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Engine.PluginResults
{
    public class ELFcompressedFile : SomeFile, IElfCompressedFile
    {
        public ELFcompressedFile( FileInfo info) : base( info ) { }
        public bool Errors { get { return false; } }

        
    }
}
