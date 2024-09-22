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
    abstract public class AbstractHeapStructure 
    {
        protected UInt32 _offset;
        protected List<IHeapBlock32> _struct;

        public AbstractHeapStructure()
        {
            _struct = new List<IHeapBlock32>();
        }

        public List<IHeapBlock32> Blocks
        {
            get 
            {
                return _struct;
            }
        }

        public UInt32 DataOffset
        {
            get
            {
                return _offset;
            }

            set
            {
                _offset = value;
            }
        }

        public IHeapBlock32 this[ UInt64 address ]
        {
            get
            {
                return GetBlockAt( address );
            }
        }

        public IHeapBlock32 GetBlockAt( UInt64 address )
        {
            foreach ( IHeapBlock32 heapBlock in _struct )
            {
                if ( ( heapBlock.Address <= address ) && ( ( heapBlock.Address + heapBlock.SizeFull ) > address ) )
                {
                    return heapBlock;
                }
            }

            return null;
        }

        public IHeapBlock32 GetBlockWithPayloadAt( UInt64 address )
        {
            IHeapBlock32 block = GetBlockAt( address );

            //
            // we already checked that the address is within the bounds
            // of the block. Now we only need to see if the address is
            // within the bounds of the payload area
            //
            if ( 
                ( ( block.Address + _offset ) <= address ) // it is past header
                &&
                ( ( ( block.Address + block.SizeFull ) - 4 ) > address ) // it is not the REDZONE (4 bytes)
               )
            {
                return block;
            }
            else
            {
                return null;
            }
        }
    }
}
