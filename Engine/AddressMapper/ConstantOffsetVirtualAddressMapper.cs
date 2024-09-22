// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Engine.AddressMapper
{
    public class ConstantOffsetVirtualAddressMapper : IVirtualAddressMapper
    {
        private Int64 _offset;

        public ConstantOffsetVirtualAddressMapper( Int64 offset )
        {
            _offset = offset;
        }

        public UInt64 Map( UInt64 virtualAddress )
        {
            // should be 
            // return virtualAddress + _offset;
            // but type safety eats my ass:
            // Error	2	Operator '+' cannot be applied to operands of type 'ulong' and 'long'

            if ( _offset < 0 )
            {
                return virtualAddress - (UInt64)( -1 * _offset );
            }
            else
            {
                return virtualAddress + (UInt64)_offset;
            }
        }
    }
}
