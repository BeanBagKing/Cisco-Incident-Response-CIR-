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
    public abstract class AbstractVirtualMemorySection : IVirtualMemorySection
    {
        protected string _name;
        protected UInt64 _address;
        protected UInt64 _size;
        protected VirtualMemorySectionProperties _properties;


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public UInt64 Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public UInt64 Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public VirtualMemorySectionProperties Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }
        public bool Errors { get { return false; } }
    }
}

