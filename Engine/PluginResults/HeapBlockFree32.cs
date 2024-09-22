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
    public class HeapBlockFree32 : IHeapBlockFree32
    {
        protected UInt32 _Magic;
        protected UInt32 _LastFreePC;
        protected UInt32 _FreeNext;
        protected UInt32 _FreePrev;


        #region Boring getters and setters 

        public UInt32 Magic
        {
            get { return _Magic; }
            set { _Magic = value; }
        }

        public UInt32 LastFreePC
        {
            get { return _LastFreePC; }
            set { _LastFreePC = value; }
        }

        public UInt32 FreeNext
        {
            get { return _FreeNext; }
            set { _FreeNext = value; }
        }

        public UInt32 FreePrev
        {
            get { return _FreePrev; }
            set { _FreePrev = value; }
        }

        #endregion
    }
}
