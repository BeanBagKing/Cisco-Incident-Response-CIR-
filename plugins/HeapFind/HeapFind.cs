// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using PluginHost;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Helper;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.PluginResults;

namespace Recurity.CIR.Plugins.HeapFind
{
    public class HeapFind : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        protected IElfMemoryMap _elfMap;
        protected ICiscoCoreMemoryMap _coreMap;

        public override string Name
        {
            get { return "Heap finder"; }
        }

        public override string Description
        {
            get { return "Identifies the IOS Heap region"; }
        }

        protected override void InitializeInternal()
        {
            // nothing to be done
        }

        protected override uint doStep()
        {
            HeapMemorySection heap = new HeapMemorySection();

            UInt64 elfTop = 0;
            foreach ( IVirtualMemorySection sec in _elfMap.GetKnownMemorySections() )
            {
                if ( sec.Address > elfTop )
                    elfTop = sec.Address + sec.Size;
            }

            heap.Size = ( _coreMap.GetMemorySection( elfTop ).Address + _coreMap.GetMemorySection( elfTop ).Size ) - elfTop;
            heap.Name = ".heap";
            heap.Address = ( _coreMap.GetMemorySection( elfTop ).Address + _coreMap.GetMemorySection( elfTop ).Size ) - heap.Size;
            VirtualMemorySectionProperties pro = new VirtualMemorySectionProperties();
            pro.DataAvailable = TriBool.UNKNOWN;
            pro.Executable = TriBool.UNKNOWN;
            pro.Initialized = false;
            pro.Writable = true;
            heap.Properties = pro;
            done( heap );
            return 100;
        }

        protected override void CancelInternal()
        {
            // nothing
        }

        #region IAnalysisPlugin Members

        public CiscoPlatforms[] Platforms
        {
            get 
            {
                CiscoPlatforms[] ret = { CiscoPlatforms.C2600, CiscoPlatforms.C1700 };
                return ret;
            }
        }

        public Type[] ResultTypes
        {
            get 
            {
                Type[] ret = { typeof( IHeap ) };
                return ret;
            }
        }

        public IPluginResult[] Results
        {
            get 
            {
                return ArrayMaker.Make( Result );
            }
        }

        public Type[] Requirements
        {
            get 
            {  
                Type[] ret = { typeof(IElfMemoryMap), typeof(ICiscoCoreMemoryMap) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _elfMap = ( (IElfMemoryMap)requirements[ 0 ] ).GetPlatformAdjusted();
            _coreMap = (ICiscoCoreMemoryMap)requirements[ 1 ];
        }

        #endregion
    }
}
