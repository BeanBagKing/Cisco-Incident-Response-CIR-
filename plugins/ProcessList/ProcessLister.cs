// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using PluginHost;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.PluginResults;

namespace Recurity.CIR.Plugins.ProcessList
{
    internal struct ProcessInformationSource
    {
        public UInt64 RecordAddress;
        public bool FromProcessArray;
    }

    public class ProcessLister : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        protected ILifeMemory _memory;
        protected IIOSSignature _signature;
        protected IHeapStructureMain _heapStructure;
        internal IProcessListOffsets _offsets;
        internal List<ProcessInformationSource> _processEntries;

        protected uint _currentEntry;

        protected Engine.PluginResults.ProcessList _result;
        

        public override string Name
        {
            get { return "Process List parser"; }
        }

        public override string Description
        {
            get { return "Finds and analyses the IOS process list"; }
        }

        protected override void InitializeInternal()
        {
            _result = new Recurity.CIR.Engine.PluginResults.ProcessList( this.GetType().ToString() );

            _offsets = OffsetFactory.Offsets( _signature );

            UInt64 arrayStart = 0;

            //
            // traverse the IOS main heap and find the block named
            // "Process Array" 
            //
            foreach ( IHeapBlock32 block in _heapStructure.Blocks )
            {
                if ( block.AllocNameString.Equals( "Process Array", StringComparison.InvariantCulture ) )                
                {
                    arrayStart = block.Address + _heapStructure.DataOffset;
                }
            }

            if ( 0 == arrayStart )
                throw new ArgumentOutOfRangeException( "ProcessArray not found in heap structure" );


            //
            // The Process Array is actually prepended by a "next" pointer at offset 0
            // (which points to the next heap block containing more of the process array)
            // and a count field (32Bit) at offset 4. This code parses that part and 
            // gets the addresses of the individual process descriptor blocks 
            //
            
            _processEntries = new List<ProcessInformationSource>();            

            UInt64 next = arrayStart;
            do
            {
                UInt32 count = _memory.GetUInt32( next + 4 );

                for ( UInt32 i = 0; i <= count; i++ )
                {
                    UInt64 e = _memory.GetUInt32( next + 8 + ( i * 4 ) );                    

                    ProcessInformationSource src = new ProcessInformationSource();
                    src.FromProcessArray = true;
                    src.RecordAddress = e;
                    _processEntries.Add( src );
                }

                next = _memory.GetUInt32( arrayStart );
            } while ( 0 != next );                        

            //
            // now, some entries in the process array are NULL but the corresponding
            // PID still has a process descriptor block lingering around in memory.
            //
            // We now itterate over the entire heap, looing for "Process" blocks and 
            // use the location of the process descriptor block to see if we already
            // know about this process.
            //
            foreach ( IHeapBlock32 hblock in _heapStructure.Blocks )
            {
                if ( hblock.AllocNameString.Equals( "Process", StringComparison.InvariantCulture ) )
                {
                    bool alreadyInList = false;
                    
                    foreach ( ProcessInformationSource src in _processEntries )
                    {
                        if ( src.RecordAddress == ( hblock.Address + _heapStructure.DataOffset ) )
                        {
                            alreadyInList = true;
                            break;
                        }
                    }

                    if ( !alreadyInList )
                    {                                        
                        ProcessInformationSource src = new ProcessInformationSource();
                        src.FromProcessArray = false;
                        src.RecordAddress = hblock.Address + _heapStructure.DataOffset;
                        _processEntries.Add( src );
                    }
                }
            }                        
        }

        protected override uint doStep()
        {
            if ( _currentEntry == _processEntries.Count ) 
            {
                done( _result );
                return 100;
            }
            
            UInt64 address = _processEntries[ (int)_currentEntry ].RecordAddress;
            
            ProcessInformation p = new ProcessInformation();

            p.InProcessArray = _processEntries[ (int)_currentEntry ].FromProcessArray;
            p.RecordLocation = address;

            if ( 0 != address )
            {
                p.Callee = _memory.GetUInt32( address + _offsets.Callee );
                p.Caller = _memory.GetUInt32( address + _offsets.Caller );
                p.CPU_percent_1min = _memory.GetUInt32( address + _offsets.CpuPercent1min );
                p.CPU_percent_5min = _memory.GetUInt32( address + _offsets.CpuPercent5min );
                p.CPU_percent_5sec = _memory.GetUInt32( address + _offsets.CpuPercent5sec );
                p.CPU_ticks_5sec = _memory.GetUInt32( address + _offsets.CpuTicks5sec );
                p.Invokations = _memory.GetUInt32( address + _offsets.Invokations );
                p.IsAnalyzed = _memory.GetByte( address + _offsets.IsAnalyzed ) == 0 ? false : true;
                p.IsBlockedAtCrash = _memory.GetByte( address + _offsets.IsBlockedAtCrash ) == 0 ? false : true;
                p.IsCorrupt = _memory.GetByte( address + _offsets.IsCorrupt ) == 0 ? false : true;
                p.IsCrashed = _memory.GetByte( address + _offsets.IsCrashed ) == 0 ? false : true;
                p.IsInitProcess = _memory.GetByte( address + _offsets.IsInitProcess ) == 0 ? false : true;
                p.IsKilled = _memory.GetByte( address + _offsets.IsKilled ) == 0 ? false : true;
                p.IsOnOldQueue = _memory.GetByte( address + _offsets.IsOnOldQueue ) == 0 ? false : true;
                p.IsPreferringNew = _memory.GetByte( address + _offsets.IsPreferringNew ) == 0 ? false : true;
                p.IsProcessArgValid = _memory.GetByte( address + _offsets.IsProcessArgValid ) == 0 ? false : true;
                p.IsProfiled = _memory.GetByte( address + _offsets.IsProfiled ) == 0 ? false : true;
                p.IsProfiledProcess = _memory.GetByte( address + _offsets.IsProfiledProcess ) == 0 ? false : true;
                p.IsWakeupPosted = _memory.GetByte( address + _offsets.IsWakeupPosted ) == 0 ? false : true;

                UInt32 namePtr = _memory.GetUInt32( address + _offsets.Name );
                p.Name = _memory.GetString( namePtr );
                
                p.PID = _memory.GetUInt32( address + _offsets.PID );
                p.Stack = _memory.GetUInt32( address + _offsets.Stack );
                p.StackLowLimit = _memory.GetUInt32( address + _offsets.StackLowLimit );
                p.StackOld = _memory.GetUInt32( address + _offsets.StackOld );
                p.StackSize = _memory.GetUInt32( address + _offsets.StackSize );
                p.Total_Free = _memory.GetUInt32( address + _offsets.TotalFree );
                p.Total_Malloc = _memory.GetUInt32( address + _offsets.TotalMalloc );
                p.Total_PoolAlloc = _memory.GetUInt32( address + _offsets.TotalPoolAlloc );
                p.Total_PoolFree = _memory.GetUInt32( address + _offsets.TotalPoolFree );
            }
            else
            {
                p.Name = "";                
            }

            _result.Processes.Add( p );

            _currentEntry++;            
            return (uint)( ( (float)_currentEntry / (float)_processEntries.Count ) * 100.0 );
        }

        protected override void CancelInternal()
        {
            // nothing
        }

        #region IAnalysisPlugin Members

        public Recurity.CIR.Engine.CiscoPlatforms[] Platforms
        {
            get 
            {
                CiscoPlatforms[] ret = { CiscoPlatforms.C2600, CiscoPlatforms.C1700 };
                return ret;
            }
        }

        public Type[] ResultTypes
        {
            get 
            {
                Type[] ret = { typeof( IProcessList ) };
                return ret;
            }
        }

        public IPluginResult[] Results
        {
            get 
            { 
                return ArrayMaker.Make( Result );
            }
        }

        public Type[] Requirements
        {
            get 
            {
                Type[] ret = { typeof( ILifeMemory ), typeof( IHeapStructureMain ), typeof( IIOSSignature ) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _memory = (ILifeMemory)requirements[ 0 ];
            _heapStructure = (IHeapStructureMain)requirements[ 1 ];
            _signature = (IIOSSignature)requirements[ 2 ];            
        }

        #endregion
    }
}
