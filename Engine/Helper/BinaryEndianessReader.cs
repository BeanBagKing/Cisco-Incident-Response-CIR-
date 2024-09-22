// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Recurity.CIR
{
    public class BinaryEndianessReader : BinaryReader
    {
        protected bool _SwapOn;

        public BinaryEndianessReader( Stream source ) 
            : base( source )
        {
            _SwapOn = false;
        }

        public BinaryEndianessReader( Stream source, Encoding encoding )
            : base( source, encoding )
        {
            _SwapOn = false;
        }

        public override Int16 ReadInt16()
        {
            if ( _SwapOn )
                return (Int16)Endianess.swap( base.ReadUInt16() );
            else
                return base.ReadInt16();
        }

        public override Int32 ReadInt32()
        {
            if ( _SwapOn )
                return (Int32)Endianess.swap( base.ReadUInt32() );
            else
                return base.ReadInt32();
        }

        public override Int64 ReadInt64()
        {
            if ( _SwapOn )
                return (Int64)Endianess.swap( base.ReadUInt64() );
            else
                return base.ReadInt64();
        }

        public override UInt16 ReadUInt16()
        {
            if ( _SwapOn )
                return Endianess.swap( base.ReadUInt16() );
            else
                return base.ReadUInt16();
        }

        public override UInt32 ReadUInt32()
        {
            if ( _SwapOn )
                return Endianess.swap( base.ReadUInt32() );
            else
                return base.ReadUInt32();
        }

        public override UInt64 ReadUInt64()
        {
            if ( _SwapOn )
                return Endianess.swap( base.ReadUInt64() );
            else
                return base.ReadUInt64();
        }

        public string ReadStringZeroTerminated()
        {            
            List<byte> list = new List<byte>();
            byte b;
            
            while ( 0 != ( b = ReadByte() ) )
            {
                list.Add( b );
            }
            
            return ASCIIEncoding.ASCII.GetString( list.ToArray() );
        }

        public bool SwapOn
        {
            get
            {
                return _SwapOn;
            }
            set
            {
                _SwapOn = value;
            }
        }
    }
}
