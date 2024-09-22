// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

namespace Recurity.CIR.Engine.Interfaces
{
    public interface IPacketHeader
    {
        UInt64 Address { get; }
        UInt64 Next { get; }
        UInt64 Frame { get; }
        UInt32 Size { get; }
    }
}
