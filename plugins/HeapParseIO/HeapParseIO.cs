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
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.PluginResults;

namespace Recurity.CIR.Plugins.HeapParserIO
{
    public class HeapParseIO : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        protected const UInt32 HEAP_MAGIC = 0xAB1234CD;

        protected IMemory _core;
        protected IMemory _mainCore;
        protected IVirtualMemorySection _heapSection;
        protected ILifeIOMemoryMap _map;
        protected HeapStructureIO _structure;

        protected UInt64 _currentPtr;

       
        public override string Name
        {
            get { return "IO Heap Parser"; }
        }

        public override string Description
        {
            get { return "Parses the IOS IO Memory heap region"; }
        }

        protected override void InitializeInternal()
        {            
            _heapSection = _map.GetKnownMemorySections()[ 0 ];
            _currentPtr = _heapSection.Address;
            _structure = new HeapStructureIO(this.GetType().ToString());
        }

        protected override uint doStep()
        {
            if ( _core.GetUInt32( _currentPtr ) == HEAP_MAGIC )
            {
                HeapBlock32 block = new HeapBlock32();
                block.Magic = _core.GetUInt32( _currentPtr );
                block.PID = _core.GetUInt32( _currentPtr + 4 );
                block.AllocCheck = _core.GetUInt32( _currentPtr + 8 );
                block.AllocName = _core.GetUInt32( _currentPtr + 12 );
                block.AllocPC = _core.GetUInt32( _currentPtr + 16 );
                block.NextBlock = _core.GetUInt32( _currentPtr + 20 );
                block.PrevBlock = _core.GetUInt32( _currentPtr + 24 );
                block.Size = _core.GetUInt32( _currentPtr + 28 );
                block.RefCount = _core.GetUInt32( _currentPtr + 32 );
                block.FreePC = _core.GetUInt32( _currentPtr + 36 );

                block.RedZone = _core.GetUInt32( _currentPtr + ( block.SizeFull - 4 ) );

                if ( !block.Used )
                {
                    HeapBlockFree32 free = new HeapBlockFree32();
                    free.Magic = _core.GetUInt32( _currentPtr + 40 );
                    free.LastFreePC = _core.GetUInt32( _currentPtr + 44 );
                    // 48 - dummy
                    // 52 - dummy
                    free.FreeNext = _core.GetUInt32( _currentPtr + 56 );
                    free.FreePrev = _core.GetUInt32( _currentPtr + 60 );
                    block.FreeBlock = free;
                }

                block.AllocNameString = _mainCore.GetString( block.AllocName );
                block.Address = _currentPtr;

                _structure.Blocks.Add( block );

                _currentPtr += block.SizeFull;
            }
            else
            {
                _currentPtr += 1;
            }

            if ( _currentPtr >= ( ( _heapSection.Address + _heapSection.Size ) - 4 ) ) // -4 for magic check
            {
                done( _structure );
                return 100;
            }

            return (uint)( ( (float)( _currentPtr - _heapSection.Address ) / (float)( _heapSection.Size ) ) * 100.0 );
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
                Type[] ret = { typeof( IHeapStructureIO ) };
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
                Type[] ret = { typeof( ILifeIOMemory ), typeof( ILifeIOMemoryMap ), typeof( ILifeMemory ) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _core = (ILifeIOMemory)requirements[ 0 ];
            _map = (ILifeIOMemoryMap)requirements[ 1 ];
            _mainCore = (ILifeMemory)requirements[ 2 ];
        }

        #endregion
    }
}

