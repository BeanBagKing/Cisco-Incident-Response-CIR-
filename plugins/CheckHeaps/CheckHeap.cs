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

namespace Recurity.CIR.Plugins.CheckHeaps
{
    public class CheckHeap32 : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        private IHeapStructureMain _mainHeap;
        private ILifeMemory _memory;
        private UInt64 _current;
        private CheckHeapResults _results;
        private List<CheckHeapResultRecord> _potentialHeads;

        public override string Name
        {
            get { return "CheckHeaps"; }
        }

        public override string Description
        {
            get { return "Performs checks similar to the IOS Check Heaps process"; }
        }

        protected override void InitializeInternal()
        {
            _current = 0;
            _results = new CheckHeapResults( this.GetType().ToString() );
            _potentialHeads = new List<CheckHeapResultRecord>();
        }

        protected override void CancelInternal()
        {
            // none
        }

        protected override uint doStep()
        {
            // 
            // are we done?
            //
            if ( _current == (UInt64)_mainHeap.Blocks.Count )
            {
                //
                // before we close the session, verify that we have only one poential
                // heap linked list head block
                //
                if ( _potentialHeads.Count > 1 )
                {
                    //
                    // if we have more than one, add them all to the results,
                    // since this is worth complaining about
                    //
                    foreach ( CheckHeapResultRecord rec in _potentialHeads )
                    {
                        _results.Results.Add( rec );
                    }
                }
                else if ( _potentialHeads.Count == 0 )
                {
                    //
                    // if we don't have any, add a complaint 
                    //
                    HeapBlock32 dummy = new HeapBlock32();
                    _results.Results.Add( new CheckHeapResultRecord( dummy, "The heap appears to be circular - no head record found" ) );
                }

                done( _results );
                return 100;
            }

            //
            // get the block
            //
            IHeapBlock32 block = _mainHeap.Blocks[ (int)_current ];

            //
            // Check some fields
            //
            if ( block.Magic != 0xAB1234CD )
            {
                _results.Results.Add( new CheckHeapResultRecord( block, "invalid MAGIC" ) );
            }
            if ( ( block.Used ) && ( block.RedZone != 0xFD0110DF ) )
            {
                _results.Results.Add( new CheckHeapResultRecord( block, "corrupted REDZONE: " + block.RedZone.ToString("X") ) );
            }
            if ( ( block.RefCount & 0xFFFFFF00 ) > 0 )
            {
                _results.Results.Add( new CheckHeapResultRecord( block, "Reference count field > 256, suspicious" ) );
            }
            if ( 0x00 == block.AllocName )
            {
                _results.Results.Add( new CheckHeapResultRecord( block, "Allocation Name is NULL, suspicious" ) );
            }
            if ( 0x00 == block.AllocPC )
            {
                _results.Results.Add( new CheckHeapResultRecord( block, "Allocating PC is NULL, suspicious" ) );
            }
            if ( 0x00 != block.AllocName )
            {
                if ( !_memory.IsValidPointer( block.AllocName ) )
                {
                    _results.Results.Add( new CheckHeapResultRecord( block, "Allocation Name is not a valid pointer: " + block.AllocName.ToString( "X" ) ) );
                }
            }
            if ( 0x00 != block.AllocPC)
            {
                if ( !_memory.IsValidPointer( block.AllocPC ) )
                {
                    _results.Results.Add( new CheckHeapResultRecord( block, "Allocating PC is not a valid pointer: " + block.AllocPC.ToString( "X" ) ) );
                }
            }

            //
            // NEXT LINK TEST
            // if this block has a successor, make sure it points back to us
            // WARNING: the PrevBlock pointer in the next block points to our Next pointer, NOT to the beginning of the block
            //
            if ( 0x00 != block.NextBlock )
            {
                IHeapBlock32 next = _mainHeap.GetBlockAt( block.NextBlock );
                if ( null == next )
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendFormat( "Next block is not within the heap region: {0,8:X}", block.NextBlock );

                    if ( _memory.IsValidPointer( block.NextBlock ) )
                    {
                        if ( _memory.GetUInt32( block.NextBlock ) == 0xAB1234CD )
                        {
                            sb.Append( ", but begins with a heap block magic number (potential memory reclaim).\n" );
                        }
                        else
                        {
                            sb.Append( " and does not point to a valid heap block (highly suspicious).\n" );
                        }
                    }
                    else
                    {
                        sb.Append( " and is not a valid pointer (highly suspicious)!\n" );
                    }

                    sb.AppendFormat( "[file offset: {0,8:X}]", _memory.VirtualAddress2FileOffset( block.Address ) );

                    _results.Results.Add( new CheckHeapResultRecord( block, sb.ToString() ) );
                    
                }
                else if ( next.PrevBlock != ( block.Address + 0x14 ) )
                {
                    StringBuilder sb = new StringBuilder();                    
                    
                    sb.AppendFormat( "Next block does not back-link to this block:\n this: {0,8:X} / this->next: {1,8:X} / this->next->prev: {2,8:X} expected: {3,8:X}.\n",
                        block.Address,
                        block.NextBlock,
                        next.PrevBlock,
                        block.Address + 0x14 );
                    sb.AppendLine( block.ToString() );
                    sb.AppendLine( next.ToString() );                     

                    if ( _memory.IsValidPointer( next.PrevBlock ) )
                    {
                        if ( _memory.GetUInt32( ( next.PrevBlock - 0x14 ) ) == 0xAB1234CD )
                        {
                            sb.Append( "The pointer at this->next->prev points to a heap block magic.\n" );

                            foreach ( CheckHeapResultRecord r in _results.Results )
                            {
                                if ( r.block.Address == next.PrevBlock )
                                {
                                    sb.AppendFormat( "This issue is likely to be related to block {0,8:X}.", r.block.Address );
                                }
                            }
                        }
                        else
                        {
                            sb.Append( "The pointer at this->next->prev does not point to a heap block.\n" );
                        }
                    }
                    else
                    {
                        sb.Append( "The pointer at this->next->prev does not point to a valid memory address!\n" );
                    }

                    if ( ( !block.Used ) && ( next.PrevBlock == block.PrevBlock ) && ( next.Used ) )
                    {
                        sb.Append( "This is likely coalescing of a free block in progress,\n" +
                                    "as this block is not used, the next block is used and both point backwards\n" +
                                    "to the same previous block (that would have been freed).\n" );
                    }

                    sb.AppendFormat( "[file offset: {0,8:X}]", _memory.VirtualAddress2FileOffset( block.Address ) );

                    _results.Results.Add( new CheckHeapResultRecord( block, sb.ToString() ) );                        
                }
            }

            //
            // PREVIOUS LINK TEST
            // if this block has a predecessor, make sure it points forward to us
            //
            if ( 0x00 != block.PrevBlock )
            {
                IHeapBlock32 prev = _mainHeap.GetBlockAt( block.PrevBlock );

                if ( null == prev )
                {
                    //
                    // we didn't find a predecessor block, but it could be the heap
                    // base pointer. Verifying here that it does point back to us and
                    // add it to the list of potential heap base addresses. The list
                    // is inspected before we are done() with this plugin.
                    //
                    bool HeapBase = false;

                    try
                    {
                        UInt32 pointer = _memory.GetUInt32( block.PrevBlock );
                        if ( pointer == block.Address )
                        {
                            HeapBase = true;
                        }
                    }
                    catch ( ArgumentOutOfRangeException )
                    {
                        // do nothing
                    }

                    if ( !HeapBase )
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.AppendFormat( "Previous block is not within the heap region: {0,8:X}", block.PrevBlock );

                        if ( _memory.IsValidPointer( block.PrevBlock ) )
                        {
                            if ( _memory.GetUInt32( block.PrevBlock ) == 0xAB1234CD )
                            {
                                sb.Append( ", but begins with a heap block magic number.\n" );
                            }
                            else
                            {
                                sb.Append( " and does not point to a valid heap block.\n" );
                            }
                        }
                        else
                        {
                            sb.Append( " and is not a valid pointer.\n" );
                        }

                        sb.AppendFormat( "[file offset: {0,8:X}]", _memory.VirtualAddress2FileOffset( block.Address ) );


                        _results.Results.Add( new CheckHeapResultRecord( block,  sb.ToString() ) );
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.AppendFormat( "The block is the potential head of the heap linked list, as {0,8:X}\n"+
                            "points back to this block. There should, however, be only one of those. ",
                            block.PrevBlock );
                        sb.AppendFormat( "[file offset: {0,8:X}]", _memory.VirtualAddress2FileOffset( block.Address ) );

                        _potentialHeads.Add( new CheckHeapResultRecord( block, sb.ToString() ) );
                    }
                }
                //
                // we got a prev pointer and block, perform verification 
                //
                else if ( prev.NextBlock != block.Address )
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendFormat( "Previous block does not forward-link to this block:\n" +
                        "this: {0,8:X} / this->previous: {1,8:X} / this->previous->next: {2,8:X} expected: {3,8:X}.\n",
                        block.Address,
                        prev.Address,
                        prev.NextBlock,
                        block.Address );
                    sb.AppendLine( block.ToString() );
                    sb.AppendLine( prev.ToString() );

                    if ( _memory.IsValidPointer( prev.NextBlock ) )
                    {
                        if ( _memory.GetUInt32( prev.NextBlock ) == 0xAB1234CD )
                        {
                            sb.Append( "The pointer at this->previous->next points to a heap block magic.\n" );

                            foreach ( CheckHeapResultRecord r in _results.Results )
                            {
                                if ( r.block.Address == prev.NextBlock )
                                {
                                    sb.AppendFormat( "This issue is likely to be related to block {0,8:X}.\n", r.block.Address );
                                }
                            }
                        }
                        else
                        {
                            sb.Append( "The pointer at this->previous->next does not point to a heap block.\n" );
                        }
                    }
                    else
                    {
                        sb.Append( "The pointer at this->previous->next does not point to a valid memory address!\n" );
                    }

                    sb.AppendFormat( "[file offset: {0,8:X}]", _memory.VirtualAddress2FileOffset( block.Address ) );

                    _results.Results.Add( new CheckHeapResultRecord( block, sb.ToString() ) );
                }
            }
            else
            {
                //
                // Prev block entries of 0x00 should not happen
                //
                _results.Results.Add( new CheckHeapResultRecord( block, "Previous block pointer is NULL" ) );
            }

            //
            // advance !
            //
            _current++;

            //
            // percentage
            //
            return (uint)( ( (float)_current / (float)_mainHeap.Blocks.Count ) * 100.0 );
        }

        #region IAnalysisPlugin Members

        public CiscoPlatforms[] Platforms
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
                Type[] ret = { typeof( ICheckHeapResults ) };
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
                Type[] ret = { typeof( IHeapStructureMain ), typeof( ILifeMemory ) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _mainHeap = (IHeapStructureMain)requirements[ 0 ];
            _memory = (ILifeMemory)requirements[ 1 ];
        }

        #endregion
    }
}
