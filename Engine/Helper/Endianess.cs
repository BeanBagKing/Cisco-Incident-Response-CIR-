// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

namespace Recurity.CIR
{
    public class Endianess
    {
        public static UInt16 swap( UInt16 x )
        {
            // looks like the x>> operation implicitly 
            // converts the UInt16 into a int ?!?


            return unchecked ((UInt16)( ( x >> 8 ) | ( x << 8 ) ));
        }

        public static UInt32 swap( UInt32 x )
        {
            return unchecked(
                ( x >> 24 ) |
                ( ( x << 8 ) & 0x00FF0000 ) |
                ( ( x >> 8 ) & 0x0000FF00 ) |
                ( x << 24 )
            );
        }

        public static UInt64 swap( UInt64 x )
        {
            return unchecked(
                ( x >> 56 ) |
                ( ( x << 40 ) & 0x00FF000000000000 ) |
                ( ( x << 24 ) & 0x0000FF0000000000 ) |
                ( ( x << 8 ) & 0x000000FF00000000 ) |
                ( ( x >> 8 ) & 0x00000000FF000000 ) |
                ( ( x >> 24 ) & 0x0000000000FF0000 ) |
                ( ( x >> 40 ) & 0x000000000000FF00 ) |
                ( x << 56 )
            );
        }
    }
}
