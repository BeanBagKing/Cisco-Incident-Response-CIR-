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
using Recurity.CIR.Engine.PluginResults.Xml;
using Recurity.CIR.Engine.Report;

namespace Recurity.CIR.ELF
{
    public class ElfFileMemory : AbstractMemory, IElfMemory, IElfMemoryMap, IPluginReporter, IDisposable
    {
        protected IMemoryMap _map;
        protected ElfFile _elfFile;
        protected List<IVirtualMemorySection> _sections;
        protected ElfMemoryMapDecorator _decorator;

        public ElfFileMemory(FileRepresentation file, IVirtualAddressMapper addressMapper)
        {
            _map = this;
            _sections = null;
            if (null != addressMapper)
            {
                _decorator = new ElfMemoryMapDecorator(addressMapper, this);
            }
            _elfFile = new ElfFile(file);
            // this resource must be released in ElfFile -- do not close in Dispose
            BinaryReader = _elfFile.BinaryEndianessReader;
        }

        //
        // IElfMemory interface
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

        public override byte[] GetBytes(UInt64 virtualAddress, uint length)
        {
            for (int i = 0; i < _elfFile.ProgramHeaders.Count; i++)
            {
                if (
                    (_elfFile.ProgramHeaders[i].VirtualAddress <= virtualAddress)
                    && ((_elfFile.ProgramHeaders[i].VirtualAddress + _elfFile.ProgramHeaders[i].SizeInFile) >= virtualAddress)
                    && ((_elfFile.ProgramHeaders[i].VirtualAddress + _elfFile.ProgramHeaders[i].SizeInFile) >= (virtualAddress + length))
                    )
                {
                    BaseStream.Seek((long)(_elfFile.ProgramHeaders[i].Offset + (virtualAddress - _elfFile.ProgramHeaders[i].VirtualAddress)), SeekOrigin.Begin);
                    return ReadBytes((int)length);
                }
            }

            // not found, fall through
            throw new ArgumentOutOfRangeException("ELF program header section not found or too short, VA: 0x" + virtualAddress.ToString("X") + "/Length:" + length.ToString("d"));
        }

        public override UInt64 VirtualAddress2FileOffset(UInt64 virtualAddress)
        {
            for (int i = 0; i < _elfFile.ProgramHeaders.Count; i++)
            {
                if (
                    (_elfFile.ProgramHeaders[i].VirtualAddress <= virtualAddress)
                    && ((_elfFile.ProgramHeaders[i].VirtualAddress + _elfFile.ProgramHeaders[i].SizeInFile) >= virtualAddress)
                    )
                {
                    return _elfFile.ProgramHeaders[i].Offset + (virtualAddress - _elfFile.ProgramHeaders[i].VirtualAddress);
                }
            }

            throw new ArgumentOutOfRangeException("ELF program header section not found for VA 0x" + virtualAddress.ToString("X"));
        }

        public override bool IsValidPointer( ulong virtualAddress )
        {
            for ( int i = 0; i < _elfFile.ProgramHeaders.Count; i++ )
            {
                if (
                    ( _elfFile.ProgramHeaders[ i ].VirtualAddress <= virtualAddress )
                    && ( ( _elfFile.ProgramHeaders[ i ].VirtualAddress + _elfFile.ProgramHeaders[ i ].SizeInFile ) >= virtualAddress )                    
                    )
                {
                    return true;
                }
            }
            return false;
        }

        //
        // IElfMemoryMap interface
        // 

        public Type MemoryMapType
        {
            get
            {
                return this.GetType();
            }
        }

        public IVirtualMemorySection GetMemorySection(UInt64 virtualAddress)
        {
            foreach (IVirtualMemorySection sec in GetKnownMemorySections())
            {
                if ((sec.Address <= virtualAddress) && ((sec.Address + sec.Size) >= virtualAddress))
                {
                    return sec;
                }
            }

            // fall through
            //throw new ArgumentOutOfRangeException( "Section at " + virtualAddress.ToString( "X" ) + " not found" );
            return null;
        }

        public List<IVirtualMemorySection> GetKnownMemorySections()
        {
            //
            // cached, we already know
            //
            if (null != _sections)
                return _sections;

            //
            // uncached, make them
            //
            _sections = new List<IVirtualMemorySection>();

            for (int i = 0; i < _elfFile.SectionHeaders.Count; i++)
            {
                VirtualMemorySectionProperties properties = new VirtualMemorySectionProperties();

                //
                // code sections
                //
                if ((_elfFile.SectionHeaders[i].Type == ElfSectionType.PROGBITS)
                    && ((_elfFile.SectionHeaders[i].Flags & ElfSectionFlags.ALLOC) == ElfSectionFlags.ALLOC))
                {
                    properties.Executable = ((_elfFile.SectionHeaders[i].Flags & ElfSectionFlags.EXECINSTR) == ElfSectionFlags.EXECINSTR);
                    properties.Writable = ((_elfFile.SectionHeaders[i].Flags & ElfSectionFlags.WRITE) == ElfSectionFlags.WRITE);
                    properties.Initialized = true;
                    properties.DataAvailable = true;
                }
                else if ((_elfFile.SectionHeaders[i].Type == ElfSectionType.NOBITS)
                    && ((_elfFile.SectionHeaders[i].Flags & ElfSectionFlags.ALLOC) == ElfSectionFlags.ALLOC))
                {
                    properties.Executable = false;
                    properties.Executable = ((_elfFile.SectionHeaders[i].Flags & ElfSectionFlags.EXECINSTR) == ElfSectionFlags.EXECINSTR);
                    properties.Writable = ((_elfFile.SectionHeaders[i].Flags & ElfSectionFlags.WRITE) == ElfSectionFlags.WRITE);
                    properties.Initialized = false;
                    properties.DataAvailable = false;
                }
                else
                {
                    continue;
                }

                ElfVirtualMemorySection sec = new ElfVirtualMemorySection();
                sec.Address = _elfFile.SectionHeaders[i].Address;
                sec.Size = _elfFile.SectionHeaders[i].Size;
                sec.Name = _elfFile.ElfString(_elfFile.SectionHeaders[i].Name);
                sec.Properties = properties;

                _sections.Add(sec);
            }

            return _sections;

        }

        public List<ElfProgramHeader> ProgramHeaders
        {
            get { return _elfFile.ProgramHeaders; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(base.ToString());
            foreach (IVirtualMemorySection vsec in this.GetKnownMemorySections())
            {
                sb.AppendFormat(
                    "{1,8:X}: '{0}', Size: {2}",
                    vsec.Name,
                    vsec.Address,
                    vsec.Size);
                sb.AppendLine();
            }

            sb.AppendLine( "Based on ElfFile:" );
            sb.AppendLine( _elfFile.ToString() );

            return sb.ToString();
        }

        public IElfMemoryMap GetPlatformAdjusted()
        {
            return _decorator == null ? (IElfMemoryMap)this : _decorator;
        }

        #region IPluginReporter Members

        public Recurity.CIR.Engine.Report.IReport Report
        {
            get
            {
                Recurity.CIR.Engine.Report.IReport report = ReportFactory.Instance.CreatePluginReport();
                report.Summary = String.Format("Elf File contains {0} virtual memory sections", this.GetKnownMemorySections().Count);
                report.Details = 1;
                report.State = 0;
                report.Author = this.GetType().ToString();
                // TODO: (simonw) add description
                report.Description = "";
                foreach (IVirtualMemorySection vsec in this.GetKnownMemorySections())
                {
                    SegmentResult segment = new SegmentResult();
                    segment.name = vsec.Name;
                    segment.size = vsec.Size;
                    segment.address = vsec.Address;
                    report.AddReportNode(segment);



                }
                return report;
            }
        }

        public override bool Errors { get { return Report.State == 2; } }

        #endregion

        public override void Dispose()
        {
           if(_elfFile != null)
           {
               _elfFile.Dispose();
           }
        }
    }
}
