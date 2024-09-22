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

namespace Recurity.CIR.Plugins.HeapParser
{
    public class HeapParse : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        protected const UInt32 HEAP_MAGIC = 0xAB1234CD;

        protected IMemory _core;
        protected IVirtualMemorySection _heapSection;
        protected HeapStructure _structure;
        protected IIOSSignature _ios_version;

        protected UInt64 _currentPtr;

        public override string Name
        {
            get { return "Heap Parser"; }
        }

        public override string Description
        {
            get { return "Parses the IOS heap region"; }
        }

        protected override void InitializeInternal()
        {
            _currentPtr = _heapSection.Address;
            _structure = new HeapStructure(this.GetType().ToString());

            //
            // Important: this logic is required for IOS heap changes over versions
            //
            // TODO: Extensively test the heap offsets for 12.0 - 12.4 and adjust me 
            if ( _ios_version.IOSVersion < new IOSVersion( "12.4(0)" ) )
            {
                _structure.DataOffset = 40;
            }
            else if ( _ios_version.IOSVersion >= new IOSVersion( "12.4(0)" ) )
            {
                _structure.DataOffset = 48;
            }
            else
            {
                throw new NotImplementedException( _ios_version.Version + " DataOffset not implemented yet" );
            }
        }

        protected override uint doStep()
        {
            if ( _core.GetUInt32( _currentPtr ) == HEAP_MAGIC )
            {
                HeapBlock32 block = new HeapBlock32();
                block.DataOffset = _structure.DataOffset;

                block.Magic = _core.GetUInt32( _currentPtr );
                // 
                // apparently, 12.4 uses 16Bit PIDs - at least on the 1700/12.4(17a) we got
                //
                if ( _ios_version.IOSVersion >= new IOSVersion( "12.4(0)" ) )
                {
                    block.PID = _core.GetUInt16( _currentPtr + 4 );
                }
                else
                {
                    block.PID = _core.GetUInt32( _currentPtr + 4 );
                }
                block.AllocCheck = _core.GetUInt32( _currentPtr + 8 );
                block.AllocName = _core.GetUInt32( _currentPtr + 12 );
                block.AllocPC = _core.GetUInt32( _currentPtr + 16 );
                block.NextBlock = _core.GetUInt32( _currentPtr + 20 );
                block.PrevBlock = _core.GetUInt32( _currentPtr + 24 );
                block.Size = _core.GetUInt32( _currentPtr + 28 );
                block.RefCount = _core.GetUInt32( _currentPtr + 32 );
                block.FreePC = _core.GetUInt32( _currentPtr + 36 );

                try
                {
                    block.RedZone = _core.GetUInt32( _currentPtr + ( block.SizeFull - 4 ) );
                }
                catch ( ArgumentOutOfRangeException )
                {
                    block.RedZone = 0;
                }

                if ( !block.Used )
                {
                    HeapBlockFree32 free = new HeapBlockFree32();
                    free.Magic = _core.GetUInt32( _currentPtr + _structure.DataOffset + 0 );
                    free.LastFreePC = _core.GetUInt32( _currentPtr + _structure.DataOffset + 4 );
                    // 8 - dummy
                    // 12 - dummy
                    free.FreeNext = _core.GetUInt32( _currentPtr + _structure.DataOffset + 16 );
                    free.FreePrev = _core.GetUInt32( _currentPtr + _structure.DataOffset + 20 );
                    block.FreeBlock = free;
                }

                // this cannot fail, as AbstractMemory.GetString takes care of ArgumentOutOfRange
                block.AllocNameString = _core.GetString( block.AllocName );

                block.Address = _currentPtr;
                _structure.Blocks.Add( block );
                                
                _currentPtr += block.SizeFull;
                // In another HeapParser, we should do this:
                // _currentPtr += 4;
            }
            else
            {
                _currentPtr += 1;
            }

            if ( _currentPtr >= ( ( _heapSection.Address + _heapSection.Size ) - 4 ) )  // the -4 is for the MAGIC check above
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
                Type[] ret = { typeof( IHeapStructureMain ) };
                return ret;
            }
        }

        public IPluginResult[] Results
        {
            get 
            {
                return ArrayMaker.Make(Result);
            }
        }

        public Type[] Requirements
        {
            get 
            {
                Type[] ret = { typeof( ILifeMemory ), typeof( IHeap ), typeof( IIOSSignature ) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _core = (IMemory)requirements[ 0 ];
            _heapSection = (IVirtualMemorySection)requirements[ 1 ];
            _ios_version = (IIOSSignature)requirements[ 2 ];
        }

        #endregion
    }
}
