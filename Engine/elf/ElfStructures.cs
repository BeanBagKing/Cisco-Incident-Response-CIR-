// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

namespace Recurity.CIR.ELF
{
    public struct ElfIdentHeader
    {
        public ElfClass Class;
        public ElfData Data;
    }

    public struct ElfHeader
    {
        public ElfType Type;
        public ElfMachine Machine;
        public UInt32 Version;
        public UInt64 EntryPoint;
        public UInt64 ProgramHeaderTable_Offset;
        public UInt64 SectionHeaderTable_Offset;
        public UInt32 Flags;
        public UInt16 HeaderSize;
        public UInt16 ProgramHeaderTable_EntrySize;
        public UInt16 ProgramHeaderTable_EntryCount;
        public UInt16 SectionHeaderTable_EntrySize;
        public UInt16 SectionHeaderTable_EntryCount;
        public UInt16 SectionHeaderTable_StringTableIndex;
    }

    /// <summary>
    /// Linker View 
    /// </summary>
    public struct ElfSectionHeader
    {
        public UInt32 Name;
        public ElfSectionType Type;
        public ElfSectionFlags Flags;
        public UInt64 Address;
        public UInt64 Offset;
        public UInt64 Size;
        public UInt32 SectionHeaderTable_IndexLink;
        public UInt32 Info;
        public UInt64 AddressAlignment;
        public UInt64 FixedSizeEntry_Size;
    }
    /// <summary>
    /// Loaderview 
    /// </summary>
    public struct ElfProgramHeader
    {
        public ElfProgramType Type;
        public UInt64 Offset;
        public UInt64 VirtualAddress;
        public UInt64 PhysicalAddress;
        public UInt64 SizeInFile;
        public UInt64 SizeInMemory;
        public UInt32 Flags;
        public UInt64 Alignment;
    }
}
