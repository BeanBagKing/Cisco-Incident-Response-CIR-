// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.ELF;
using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.ELF
{
    public sealed class ElfMemoryMapDecorator : IElfMemoryMap
    {
        private IVirtualAddressMapper _mapping;
        private IElfMemoryMap _originalMap;

        public ElfMemoryMapDecorator( IVirtualAddressMapper mapping, IElfMemoryMap originalMap )
        {
            _mapping = mapping;
            _originalMap = originalMap;
        }


        #region IElfMemoryMap Members

        public IElfMemoryMap GetPlatformAdjusted()
        {
            // 
            // return this because this _is_ the adjusted map
            //
            return this;
        }

        #endregion

        #region IMemoryMap Members

        public IVirtualMemorySection GetMemorySection( ulong virtualAddress )
        {
            return _originalMap.GetMemorySection( _mapping.Map( virtualAddress ) );
        }

        public List<IVirtualMemorySection> GetKnownMemorySections()
        {   
            // TODO: (FX) may be optimized like in elfmemory.cs

            List<IVirtualMemorySection> adjustedMap = new List<IVirtualMemorySection>();

            foreach ( IVirtualMemorySection section in _originalMap.GetKnownMemorySections() )
            {
                ElfVirtualMemorySection adjustedSection = new ElfVirtualMemorySection();
                adjustedSection.Address = _mapping.Map( section.Address );
                adjustedSection.Name = section.Name;
                adjustedSection.Properties = adjustedSection.Properties;
                adjustedSection.Size = section.Size;

                adjustedMap.Add( adjustedSection );
            }

            return adjustedMap;
        }

        #endregion
    }
}
