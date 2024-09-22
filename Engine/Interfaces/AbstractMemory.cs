// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Recurity.CIR.Engine.Interfaces
{
    abstract public class AbstractMemory : IMemory, IDisposable
    {
        private BinaryEndianessReader _binRead;

        protected BinaryEndianessReader BinaryReader
        {
            set { _binRead = value;}
            
        }

        protected Stream BaseStream
        {
            get
            {
                return _binRead.BaseStream;
            }
        }

        abstract public IMemoryMap MemoryMap { get; set; }        
        abstract public ulong VirtualAddress2FileOffset( ulong virtualAddress );
        abstract public bool IsValidPointer( UInt64 virtualAddress );
        
        public byte GetByte( UInt64 virtualAddress )
        {
            byte[] b = GetBytes( virtualAddress, 1 );
            return b[ 0 ];
        }
        protected byte[] ReadBytes(int length)
        {
            return _binRead.ReadBytes(length);
        }

        abstract public byte[] GetBytes( ulong virtualAddress, uint length );

        public Int16 GetInt16( UInt64 virtualAddress )
        {
            _binRead.BaseStream.Seek( (long)VirtualAddress2FileOffset( virtualAddress ), SeekOrigin.Begin );
            return _binRead.ReadInt16();
        }

        public Int32 GetInt32( UInt64 virtualAddress )
        {
            _binRead.BaseStream.Seek( (long)VirtualAddress2FileOffset( virtualAddress ), SeekOrigin.Begin );
            return _binRead.ReadInt32();
        }

        public Int64 GetInt64( UInt64 virtualAddress )
        {
            _binRead.BaseStream.Seek( (long)VirtualAddress2FileOffset( virtualAddress ), SeekOrigin.Begin );
            return _binRead.ReadInt64();
        }

        public UInt16 GetUInt16( UInt64 virtualAddress )
        {
            _binRead.BaseStream.Seek( (long)VirtualAddress2FileOffset( virtualAddress ), SeekOrigin.Begin );
            return _binRead.ReadUInt16();
        }

        public UInt32 GetUInt32( UInt64 virtualAddress )
        {
            _binRead.BaseStream.Seek( (long)VirtualAddress2FileOffset( virtualAddress ), SeekOrigin.Begin );
            return _binRead.ReadUInt32();
        }

        public UInt64 GetUInt64( UInt64 virtualAddress )
        {
            _binRead.BaseStream.Seek( (long)VirtualAddress2FileOffset( virtualAddress ), SeekOrigin.Begin );
            return _binRead.ReadUInt64();
        }

        public string GetString( UInt64 virtualAddress )
        {
            byte b;
            UInt64 i = 0;
            do
            {
                try
                {
                    b = GetByte( virtualAddress + i );
                }
                catch ( ArgumentOutOfRangeException )
                {
                    break;
                }
                i++;
            } while ( b != 0 );

            if ( i > 1 )
            {
                byte[] buffer = new byte[ (int)(i-1) ];
                buffer = GetBytes( virtualAddress, (uint)(i-1) );
                return ASCIIEncoding.ASCII.GetString( buffer );
            }
            else
            {
                return "";
            }
        }

        public virtual void Close()
        {
            if(_binRead != null)
                _binRead.Close();
        }

        public abstract void Dispose();
        public virtual bool Errors { get { return false; } }
        
    }
}
