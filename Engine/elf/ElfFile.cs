// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.ELF
{
    public class ElfFile : IDisposable
    {
        protected FileRepresentation _file;        
        protected BinaryEndianessReader _binRead;

        protected ElfIdentHeader _identHeader;
        protected ElfHeader _header;
        protected List<ElfSectionHeader> _sectionHeader;
        protected List<ElfProgramHeader> _programHeader;
        protected byte[] _stringTable;
        protected Dictionary<int, string> _sectionNameCache;

        public ElfFile( FileRepresentation a_file )
        {
            _sectionHeader = null;
            _stringTable = null;

            _file = a_file;
            Stream fs = _file.Stream(FileMode.Open, FileAccess.Read );
            _binRead = new BinaryEndianessReader( fs, Encoding.GetEncoding( "us-ascii" ) );

            ReadElf();
        }                

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat( "{0}: {1} (Class: {2}, Data: {3})",
                this.GetType().ToString(),
                _file.Info.FullName,
                _identHeader.Class,
                _identHeader.Data
            );
            sb.AppendLine();

            sb.AppendLine( "Header:" );
            sb.AppendFormat(
                "Type: {0}, Machine: {1}, Version: {2}, EntryPoint: {3,8:X},\n" +
                "ProgHdrTab Offset: {4,8:X}, SectHdrTab Offset: {5,8:X}, Flags: {6,8:X}, HdrSize: {7,8:X}\n" +
                "ProgHdrTab Entry Size: {8,8:X} - Count: {9,8:X}\n" +
                "SectHdrTab Entry Size: {10,8:X} - Count: {11,8:X} - String Table Index: {12,8:X}\n",
                _header.Type,
                _header.Machine,
                _header.Version,
                _header.EntryPoint,
                _header.ProgramHeaderTable_Offset,
                _header.SectionHeaderTable_Offset,
                _header.Flags,
                _header.HeaderSize,
                _header.ProgramHeaderTable_EntrySize,
                _header.ProgramHeaderTable_EntryCount,
                _header.SectionHeaderTable_EntrySize,
                _header.SectionHeaderTable_EntryCount,
                _header.SectionHeaderTable_StringTableIndex
            );

            sb.AppendLine( "Section Headers:" );
            sb.AppendLine( "ID| Name     | Type   | Flags  |Address | Offset | Size   |Idx|Info|Algn" );                

            for ( int i = 0; i < _sectionHeader.Count; i++ )
            {
                sb.AppendFormat(
                    "{0,2}|{1,10}|{2,8}|{3,8:X}|{4,8:X}|{5,8:X}|{6,8:X}|{7,3:X}|{8,4:X}|{9,4:X}\n",
                    i,
                    ElfStringInternal( _sectionHeader[ i ].Name ),
                    _sectionHeader[ i ].Type,
                    _sectionHeader[ i ].Flags,
                    _sectionHeader[ i ].Address,
                    _sectionHeader[ i ].Offset,
                    _sectionHeader[ i ].Size,
                    _sectionHeader[ i ].SectionHeaderTable_IndexLink,
                    _sectionHeader[ i ].Info,
                    _sectionHeader[ i ].AddressAlignment,
                    _sectionHeader[ i ].FixedSizeEntry_Size );
            }

            sb.AppendLine( "Program Headers:" );
            sb.AppendLine( "ID| Type      | Offset |VAddress|PAddress|FileSize|MemrSize| Flags  | Alignm." );

            for ( int i = 0; i < _programHeader.Count; i++ )
            {
                sb.AppendFormat(
                    "{0,2}|{1,11}|{2,8:X}|{3,8:X}|{4,8:X}|{5,8:X}|{6,8:X}|{7,8:X}|{8,8:X}\n",
                    i,
                    _programHeader[ i ].Type,
                    _programHeader[ i ].Offset,
                    _programHeader[ i ].VirtualAddress,
                    _programHeader[ i ].PhysicalAddress,
                    _programHeader[ i ].SizeInFile,
                    _programHeader[ i ].SizeInMemory,
                    _programHeader[ i ].Flags,
                    _programHeader[ i ].Alignment
                );
            }

            return sb.ToString();
        }

        public List<ElfSectionHeader> SectionHeaders
        {
            get
            {
                return _sectionHeader;
            }
        }

        public List<ElfProgramHeader> ProgramHeaders
        {
            get
            {
                return _programHeader;
            }
        }

        public BinaryEndianessReader BinaryEndianessReader
        {
            get
            {
                return _binRead;
            }
        }

        private void ReadElf()
        {
            _binRead.BaseStream.Seek( 0, SeekOrigin.Begin );

            ReadIdent();

            switch ( _identHeader.Class )
            {
                case ElfClass.CLASS32:
                    ReadElf32Header();
                    ReadElf32SectionHeaders();
                    ReadElf32ProgramHeaders();
                    ReadElf32StringTable();
                    break;

                case ElfClass.CLASS64:
                    throw new NotImplementedException( "Elf64 not implemented" );
                //break;                
            }

        }

        private void ReadIdent()
        {
            byte[] ident = _binRead.ReadBytes( 4 );

            if ( ( ident[ 0 ] != 0x7F ) || ( ident[ 1 ] != 0x45 ) || ( ident[ 2 ] != 0x4C ) || ( ident[ 3 ] != 0x46 ) )
                throw new FormatException( "Missing ELF signature" );

            byte @class = _binRead.ReadByte();            
            switch ( @class )
            {
                case (byte)ElfClass.CLASS32: 
                    _identHeader.Class = ElfClass.CLASS32; 
                    break;

                case (byte)ElfClass.CLASS64:
                    _identHeader.Class = ElfClass.CLASS64; 
                    break;

                default:
                    throw new FormatException( "Invalid ELF class in header" );
            }

            byte Data = _binRead.ReadByte();
            switch ( Data )
            {
                case (byte)ElfData.LSB:
                    _identHeader.Data = ElfData.LSB;
                    break;

                case (byte)ElfData.MSB:
                    _identHeader.Data = ElfData.MSB;
                    _binRead.SwapOn = true;
                    break;

                default:
                    throw new FormatException( "Invalid ELF data type (endianess) in header" );
            }

            byte version = _binRead.ReadByte();
            if ( 1 != version )
                throw new FormatException( "Invalid ELF version in ELF header ident" );

            // eat padding
            _binRead.ReadBytes( 9 );
        }

        private void ReadElf32Header()
        {
            _header = new ElfHeader();

            _header.Type = (ElfType)_binRead.ReadUInt16();
            _header.Machine = (ElfMachine)_binRead.ReadUInt16();
            _header.Version = _binRead.ReadUInt32();
            _header.EntryPoint = _binRead.ReadUInt32();
            _header.ProgramHeaderTable_Offset = _binRead.ReadUInt32();
            _header.SectionHeaderTable_Offset = _binRead.ReadUInt32();
            _header.Flags = _binRead.ReadUInt32();
            _header.HeaderSize = _binRead.ReadUInt16();
            _header.ProgramHeaderTable_EntrySize = _binRead.ReadUInt16();
            _header.ProgramHeaderTable_EntryCount = _binRead.ReadUInt16();
            _header.SectionHeaderTable_EntrySize = _binRead.ReadUInt16();
            _header.SectionHeaderTable_EntryCount = _binRead.ReadUInt16();
            _header.SectionHeaderTable_StringTableIndex = _binRead.ReadUInt16();
        }

        private void ReadElf32SectionHeaders()
        {
            _sectionHeader = new List<ElfSectionHeader>();

            for ( uint i = 0; i < _header.SectionHeaderTable_EntryCount; i++ )
            {
                ElfSectionHeader sh = new ElfSectionHeader();            

                _binRead.BaseStream.Seek(
                    (long)( _header.SectionHeaderTable_Offset + ( i * _header.SectionHeaderTable_EntrySize ) ),
                    SeekOrigin.Begin );

                sh.Name = _binRead.ReadUInt32();                
                //sh.Type = (ElfSectionType)_binRead.ReadUInt32();
                UInt32 debugTest = _binRead.ReadUInt32();
                sh.Type = (ElfSectionType)debugTest;
                // sh.Flags = (ElfSectionFlags)_binRead.ReadUInt32();
                UInt32 debugTest2 = _binRead.ReadUInt32();
                sh.Flags = (ElfSectionFlags)debugTest2;
                sh.Address = _binRead.ReadUInt32();
                sh.Offset = _binRead.ReadUInt32();
                sh.Size = _binRead.ReadUInt32();
                sh.SectionHeaderTable_IndexLink = _binRead.ReadUInt32();
                sh.Info = _binRead.ReadUInt32();
                sh.AddressAlignment = _binRead.ReadUInt32();
                sh.FixedSizeEntry_Size = _binRead.ReadUInt32();

                _sectionHeader.Add( sh );
            }
        }

        private void ReadElf32StringTable()
        {
            bool foundFlag = false;

            for ( int i = 0; i < _sectionHeader.Count; i++ )
            {
                if ( _sectionHeader[ i ].Type == ElfSectionType.STRTAB )
                {
                    if ( foundFlag )
                        throw new FormatException( "Multiple string table sections found" );

                    _binRead.BaseStream.Seek( (long)_sectionHeader[ i ].Offset, 0 );
                    _stringTable = _binRead.ReadBytes( (int)_sectionHeader[ i ].Size );
                    foundFlag = true;
                }
            }

            //
            // HACK: SHSTRTAB
            // An image of the 2691 platform contained only a SHSTRTAB that didn't have
            // the STRTAB type assigned to it. This code attempts detection of such 
            // "hidden" string table.
            //
            if ( !foundFlag )
            {
                for ( int i = 0; i < _sectionHeader.Count; i++ )
                {
                    if (
                        ( _sectionHeader[ i ].Type == ElfSectionType.NOBITS )
                        && ( _sectionHeader[ i ].AddressAlignment == 1 )
                        && ( _sectionHeader[ i ].Size == 0 )
                        && ( _sectionHeader[ i ].Offset != 0 )
                        && ( _sectionHeader[ i ].Name == 1 )
                        && ( _sectionHeader[ i ].Flags == 0 )
                        )
                    {
                        if ( foundFlag )
                            throw new FormatException( "SHSTRTAB detection matched on multiple sections" );

                        //
                        // More heuristics: check that the section starts with 0x00 0x2E 
                        // (first entry is NULL, second begins with ".????")
                        //
                        _binRead.BaseStream.Seek( (long)_sectionHeader[ i ].Offset, SeekOrigin.Begin );
                        byte[] test2 = _binRead.ReadBytes( 2 );
                        if ( ( 0x00 != test2[ 0 ] ) || ( 0x2E != test2[ 1 ] ) )
                            continue;

                        //
                        // This beautiful section also does not have any size associated
                        // with it, so we scan for double-0x00 bytes. Yea! Fuck.
                        //
                        UInt32 sizeScan = 0;
                        _binRead.BaseStream.Seek( (long)_sectionHeader[ i ].Offset, 0 );
                        while ( _binRead.ReadUInt16() != 0x00 )
                        {
                            sizeScan += 2;
                        }

                        _binRead.BaseStream.Seek( (long)_sectionHeader[ i ].Offset, 0 );
                        _stringTable = _binRead.ReadBytes( (int)sizeScan );
                        foundFlag = true;
                    }
                }
            }
        }

        public string ElfString( UInt32 index )
        {
            if ( null == _stringTable )
                return ElfStringHeurisitc( index );                
            else
                return ElfStringInternal( index );
        }

        private string ElfStringInternal( UInt32 index )
        {
            string ret = "";
            int i;

            if ( null == _stringTable )
            {
                return ret;
            }

            if ( index > ( _stringTable.Length - 1 ) )
                return ret;

            for ( i = (int)index; i < ( _stringTable.Length - 1 ); i++ )
            {
                if ( _stringTable[ i ] == 0x00 )
                    break;
            }

            ret = ASCIIEncoding.ASCII.GetString( _stringTable, (int)index, (int)( i - index ) );

            return ret;
        }

        private string ElfStringHeurisitc( UInt32 index )
        {
            string ret = "";
            int sectionIndex = -1 ;
            
            for ( int i = 0; i < _sectionHeader.Count; i++ )
            {
                if ( _sectionHeader[ i ].Name == index )
                {
                    sectionIndex = i;
                    break;
                }
            }

            if ( null == _sectionNameCache )
            {
                Heurictics_ElfSectionNames();
            }

            if ( _sectionNameCache.ContainsKey( sectionIndex ) )
            {
                ret = _sectionNameCache[ sectionIndex ];
            }

            return ret;
        }

        private void Heurictics_ElfSectionNames()
        {
            Dictionary<int, ElfSectionHeader> rList = new Dictionary<int, ElfSectionHeader>();
            int removalIndex = -1;

            for ( int i = 0; i < _sectionHeader.Count; i++ )
            {
                rList.Add( i, _sectionHeader[ i ] );
            }

            _sectionNameCache = new Dictionary<int, string>();
            
            //
            // EntryPoint test: The section the entry point is in is .text
            //
            foreach( int i in rList.Keys )
            {            
                if (
                    ( rList[ i ].Address <= _header.EntryPoint )
                    &&
                    ( rList[ i ].Address + rList[ i ].Size > _header.EntryPoint )
                    )
                {
                    _sectionNameCache.Add( i, ".text" );

                    if ( -1 != removalIndex )
                    {
                        throw new FormatException( "ELF Section name heuristics failed: " +
                            "multiple sections could contain the ELF file's entry point." );
                    }
                    removalIndex = i;
                }
            }
            if ( -1 != removalIndex )
                rList.Remove( removalIndex );

            //
            // Size / Type test:
            // The largest section that is to be allocated, is NOT writable and does 
            // NOT contain code is likely to be .rodata
            //
            int largestSection = -1;
            UInt64 sizeCheck = 0;

            foreach( int i in rList.Keys )
            {
                if (
                    ( ( rList[ i ].Flags & ElfSectionFlags.ALLOC ) != 0 )
                    && ( ( rList[ i ].Flags & ElfSectionFlags.WRITE ) == 0 )
                    && ( ( rList[ i ].Flags & ElfSectionFlags.EXECINSTR ) == 0 )
                    )
                {
                    if ( rList[ i ].Size > sizeCheck )
                    {
                        sizeCheck = rList[ i ].Size;
                        largestSection = i;
                    }
                }
            }

            if ( -1 != largestSection )
            {
                _sectionNameCache.Add( largestSection, ".rodata" );
                rList.Remove( largestSection );
            }

            //
            // Size / Type test II:
            // The largest section that is to be allocated, IS writable and does 
            // NOT contain code is likely to be .data
            //
            largestSection = -1;
            sizeCheck = 0;

            foreach ( int i in rList.Keys )
            {
                if (
                    ( ( rList[ i ].Flags & ElfSectionFlags.ALLOC ) != 0 )
                    && ( ( rList[ i ].Flags & ElfSectionFlags.WRITE ) != 0 )
                    && ( ( rList[ i ].Flags & ElfSectionFlags.EXECINSTR ) == 0 )
                    )
                {
                    if ( rList[ i ].Size > sizeCheck )
                    {
                        sizeCheck = rList[ i ].Size;
                        largestSection = i;
                    }
                }
            }

            if ( -1 != largestSection )
            {
                _sectionNameCache.Add( largestSection, ".data" );
                rList.Remove( largestSection );
            }

            //
            // TODO: more tests
            //            
        }

        private void ReadElf32ProgramHeaders()
        {
            _programHeader = new List<ElfProgramHeader>();

            for ( uint i = 0; i < _header.ProgramHeaderTable_EntryCount; i++ )
            {
                _binRead.BaseStream.Seek( 
                    (long)(_header.ProgramHeaderTable_Offset + ( i * _header.ProgramHeaderTable_EntrySize )),
                    SeekOrigin.Begin );

                ElfProgramHeader ph = new ElfProgramHeader();

                ph.Type = (ElfProgramType)_binRead.ReadUInt32();
                ph.Offset = _binRead.ReadUInt32();
                ph.VirtualAddress = _binRead.ReadUInt32();
                ph.PhysicalAddress = _binRead.ReadUInt32();
                ph.SizeInFile = _binRead.ReadUInt32();
                ph.SizeInMemory = _binRead.ReadUInt32();
                ph.Flags = _binRead.ReadUInt32();
                ph.Alignment = _binRead.ReadUInt32();

                _programHeader.Add( ph );
            }
        }

        public void Dispose()
        {
           _binRead.Close();
        }
    }
}
