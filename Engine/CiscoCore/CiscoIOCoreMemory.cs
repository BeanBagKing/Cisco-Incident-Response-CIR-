// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Helper;
using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.CiscoCore
{
    public class CiscoIOCoreMemory : AbstractMemory, ICiscoIOCoreMemory, ICiscoIOCoreMemoryMap
    {
        protected FileRepresentation file;
        protected FileInfo _fileInfo;
        protected UInt64 _baseAddress;
        protected UInt64 _size;

        protected IMemoryMap _map;
        protected CoreIOMemorySection _section;

        public CiscoIOCoreMemory( FileRepresentation a_file, UInt64 topAddress, bool bigEndian )
        {
            file = a_file;
            _fileInfo = file.Info;
             Stream fs = file.Stream(FileMode.Open, FileAccess.Read );
            BinaryEndianessReader binRead = new BinaryEndianessReader(fs, Encoding.GetEncoding("us-ascii"));
            binRead.SwapOn = bigEndian;
            BinaryReader = binRead;

            _baseAddress = topAddress - (UInt64)_fileInfo.Length;
            _size = (UInt64)_fileInfo.Length;

            _section = new CoreIOMemorySection();
            _section.Address = _baseAddress;
            _section.Size = _size;
            _section.Name = "Cisco CORE " + _fileInfo.Name;
            VirtualMemorySectionProperties properties = new VirtualMemorySectionProperties();
            properties.DataAvailable = true;
            properties.Executable = TriBool.UNKNOWN;
            properties.Initialized = true;
            properties.Writable = TriBool.UNKNOWN;
            _section.Properties = properties;

        }

        //
        // IMemory interface
        //

        public override IMemoryMap MemoryMap
        {
            get
            {
                return _map;
            }

            set
            {
                _map = value;
            }
        }

        public override byte[] GetBytes( ulong virtualAddress, uint length )
        {
            if ( ( virtualAddress >= _baseAddress ) && ( virtualAddress <= ( _baseAddress + _size ) )
                && ( ( virtualAddress + length ) <= ( _baseAddress + _size ) )
                )
            {
                BaseStream.Seek( (long)( virtualAddress - _baseAddress ), SeekOrigin.Begin );
                return ReadBytes( (int)length );
            }
            else
                throw new ArgumentOutOfRangeException( "IOMEM Core file does not contain bytes at 0x" + virtualAddress.ToString( "X" ) + "h" );
        }

        public override UInt64 VirtualAddress2FileOffset( UInt64 virtualAddress )
        {
            if ( ( virtualAddress >= _baseAddress ) && ( virtualAddress <= ( _baseAddress + _size ) ) )
            {
                return ( virtualAddress - _baseAddress );
            }
            else
                throw new ArgumentOutOfRangeException( "IOMEM Core file does not contain bytes at 0x" + virtualAddress.ToString( "X" ) + "h" );
        }

        public override bool IsValidPointer( ulong virtualAddress )
        {
            if ( ( virtualAddress >= _baseAddress ) && ( virtualAddress <= ( _baseAddress + _size ) ) )
            {
                return true;
            }
            else
                return false;            
        }

        //
        // IMemoryMap interface
        //
        public Type MemoryMapType
        {
            get
            {
                return this.GetType();
            }
        }

        public IVirtualMemorySection GetMemorySection( UInt64 virtualAddress )
        {
            if ( ( virtualAddress >= _section.Address ) && ( virtualAddress <= ( _section.Address + _section.Size ) ) )
                return _section;
            else
                return null;
        }

        public List<IVirtualMemorySection> GetKnownMemorySections()
        {
            List<IVirtualMemorySection> ret = new List<IVirtualMemorySection>( 1 );
            ret.Add(_section);
            return ret;
        }

        public override void Dispose()
        {
            Close();
        }
    }
}
