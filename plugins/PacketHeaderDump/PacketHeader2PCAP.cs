// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using PluginHost;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.PluginResults;

using Recurity.CIR.Engine.Helper;

namespace PacketHeaderDump
{
    public class PacketHeader2PCAP : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        IPacketHeaderStructure _struct;
        ILifeIOMemory _ioMem;
        ILifeMemory _mainMem;
        IPluginConfiguration _configuration;
        PCAPwriter _pcap;
        
        int _current;

        public override string Name
        {
            get { return "Packet Header PCAP writer";  }
        }

        public override string Description
        {
            get { return "Writes the IOS Packet Header indentified packets to a PCAP file"; }
        }

        protected override void InitializeInternal()
        {
            _current = 0;
            _pcap = new PCAPwriter( Path.Combine( _configuration.RuntimeConfiguration.OutputFolder, "PacketHeader.pcap") );
        }

        protected override uint doStep()
        {
            if ( _struct.Headers[ _current ].Frame != 0x00 )
            {
                // TODO: find out how to better implement this
                if ( _struct.Headers[ _current ].Frame != 0x54 )
                {
                    if ( _ioMem.IsValidPointer( _struct.Headers[ _current ].Frame ) )
                    {
                        byte[] p = _ioMem.GetBytes( _struct.Headers[ _current ].Frame, _struct.Headers[ _current ].Size );
                        _pcap.Add( p );
                    }
                    else if ( _mainMem.IsValidPointer( _struct.Headers[ _current ].Frame ) )
                    {
                        byte[] p = _mainMem.GetBytes( _struct.Headers[ _current ].Frame, _struct.Headers[ _current ].Size );
                        _pcap.Add( p );
                    }
                    //
                    // TODO: what else ?
                    //
                }
            }

            _current++;

            if ( _current == _struct.Headers.Count )
            {
                _pcap.Write();
                StringBuilder builder = new StringBuilder();
                builder.Append( _struct.Headers.Count );
                builder.Append(" extracted packets referenced by packet headers (process switched packets) ");
                builder.Append( "were written to the file PacketHeader.pcap" );
                PCAPHeaderResult result = new PCAPHeaderResult(this.ToString(), true, builder.ToString());
                done( result );
                return 100;
            }
            else
                return (uint) ( ( (float)_current / (float)_struct.Headers.Count ) * (float)100);
        }

        protected override void CancelInternal()
        {
            _pcap.Close();
        }

        #region IAnalysisPlugin Members

        public CiscoPlatforms[] Platforms
        {
            get 
            {
                CiscoPlatforms[] ret = { CiscoPlatforms.ANY };
                return ret;
            }
        }

        public Type[] ResultTypes
        {
            get 
            {
                Type[] ret = { typeof( PCAPHeaderResult ) };
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
                Type[] req = { typeof( ILifeIOMemory ), typeof(ILifeMemory), typeof( IPacketHeaderStructure ), typeof(IPluginConfiguration) };
                return req;
            }
        }

        public void FulFill( object[] requirements )
        {
            _ioMem = (ILifeIOMemory)requirements[ 0 ];
            _mainMem = (ILifeMemory)requirements[ 1 ];
            _struct = (IPacketHeaderStructure)requirements[ 2 ];
            _configuration = (IPluginConfiguration)requirements[ 3 ];
        }

        #endregion
    }
}
