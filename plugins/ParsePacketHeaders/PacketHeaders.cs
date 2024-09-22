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

namespace Recurity.CIR.Plugins.ParsePacketHeaders
{
    public class PacketHeaders : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        IHeapStructure _heapStructure;
        ILifeMemory _memory;
        PacketHeaderStructure _resultStructure;

        int _current_block;


        public override string Name
        {
            get { return "Packet Header Parser"; }
        }

        public override string Description
        {
            get { return "Parses the Packet Headers in main memory"; }
        }

        protected override void InitializeInternal()
        {
            _current_block = 0;
            _resultStructure = new PacketHeaderStructure(this.GetType().ToString());
            
        }

        protected override uint doStep()
        {
            UInt64 pos;
            

            if ( _current_block == ( _heapStructure.Blocks.Count - 1 ) )
            {
                done( _resultStructure );
                return 100;
            }

            if ( _heapStructure.Blocks[ _current_block ].AllocNameString.StartsWith( "*Packet Header*" ) )
            {
                PacketHeader pak = new PacketHeader();

                pos = _heapStructure.Blocks[ _current_block ].Address + _heapStructure.Blocks[ _current_block ].HeaderSize;
                pak.Address = pos;
                pak.Next = _memory.GetUInt32( pos );
                pak.Frame = _memory.GetUInt32( pos + 36 );
                pak.Size = (UInt32)_memory.GetInt16( pos + 68 );
                /*
                 * Does not work (mostly NULL)
                UInt64 ifaddr = _memory.GetUInt32( pos + 60 );
                string name1 = _memory.GetString( ifaddr + 16 );
                string name2 = _memory.GetString( ifaddr + 20 );
                pak.Interface = name1 + " / " + name2;
                 */
                _resultStructure.Headers.Add( pak );
            }

            _current_block++;

            return (uint)( ( (float)_current_block / (float)( _heapStructure.Blocks.Count - 1 ) ) * (float)100 );
        }

        protected override void CancelInternal()
        {         
        }

        #region IAnalysisPlugin Members

        public CiscoPlatforms[] Platforms
        {
            get 
            { 
                CiscoPlatforms[] ret = { CiscoPlatforms.C2600 };
                return ret;
            }
        }

        public Type[] ResultTypes
        {
            get 
            {
                Type[] ret = { typeof( IPacketHeaderStructure ) };
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
                Type[] ret = { typeof(ILifeMemory), typeof( IHeapStructureMain ) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _memory = (ILifeMemory)requirements[ 0 ];
            _heapStructure = (IHeapStructure)requirements[ 1 ];
        }

        #endregion
    }
}
