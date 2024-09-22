// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using PluginHost;

namespace Recurity.CIR.Engine.Interfaces
{
    public interface IMemory : IPluginResult
    {
        IMemoryMap MemoryMap { get; set; }                
        UInt64 VirtualAddress2FileOffset( UInt64 virtualAddress );
        bool IsValidPointer( UInt64 virtualAddress );

        byte GetByte( UInt64 virtualAddress );
        byte[] GetBytes( UInt64 virtualAddress, uint length );

        Int16 GetInt16( UInt64 virtualAddress );
        Int32 GetInt32( UInt64 virtualAddress );
        Int64 GetInt64( UInt64 virtualAddress );
        UInt16 GetUInt16( UInt64 virtualAddress );
        UInt32 GetUInt32( UInt64 virtualAddress );
        UInt64 GetUInt64( UInt64 virtualAddress );

        string GetString( UInt64 virtualAddress );
    }
}
