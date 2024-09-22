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
    public enum ElfClass
    {
        INVALID = 0,
        CLASS32 = 1,
        CLASS64 = 2
    }

    public enum ElfData
    {
        INVALID = 0,
        LSB = 1,
        MSB = 2
    }

    public enum ElfType
    {
        NONE = 0,
        RELOCATABLE = 1,
        EXECUTABLE = 2,
        SHAREDOBJECT = 3,
        CORE = 4
    }

    public enum ElfMachine
    {
        NONE                = 0,
        ATnT_WE_32100       = 1,
        SPARC               = 2,
        INTEL_80386         = 3,
        M68000              = 4,
        M88000              = 5,
        INTEL_80486         = 6,
        INTEL_80860         = 7,
        MIPS3000            = 8,
        MIPS4000            = 10,
        PPC                 = 0x14,
        PPC64               = 0x15,
        CISCO               = 0x2B,
        CISCO2691_MIPS      = 0x66
    }

    public enum ElfSectionType
    {
        NULL =      0,
        PROGBITS =  1,
        SYMTAB =    2,
        STRTAB =    3,
        RELA =      4,
        HASH =      5,
        DYNAMIC =   6,
        NOTE =      7,
        NOBITS =    8,
        REL =       9,
        SHLIB =     10,
        DYNSYM =    11
    }

    public enum ElfSectionFlags
    {
        NONE = 0,
        WRITE = 1,
        ALLOC = 2,
        EXECINSTR = 4,
    }

    public enum ElfProgramType
    {
        NULL = 0,
        LOAD = 1,
        DYNAMIC = 2,
        INTERPRETER = 3,
        NOTE = 4,
        SHLIB = 5,
        PROGRAMHEADERTAB = 6
    }    
}
