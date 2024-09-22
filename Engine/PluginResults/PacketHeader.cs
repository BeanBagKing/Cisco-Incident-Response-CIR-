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
    public class PacketHeader : IPacketHeader
    {
        UInt64 _address;
        UInt64 _next;
        UInt64 _frame;
        UInt32 _size;

        public UInt64 Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public UInt64 Next
        {
            get { return _next; }
            set { _next = value; }
        }

        public UInt64 Frame
        {
            get { return _frame; }
            set { _frame = value; }
        }

        public UInt32 Size
        {
            get { return _size; }
            set { _size = value; }
        }
    }
}
