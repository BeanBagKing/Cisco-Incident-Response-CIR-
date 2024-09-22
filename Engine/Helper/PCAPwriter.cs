// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Recurity.CIR.Engine.Helper
{
    public class PCAPwriter
    {
        const UInt32 MAGIC = 0xa1b2c3d4;

        string _filename;
        List<byte[]> _buffers;
        BinaryWriter _binWrite;

        public PCAPwriter( string filename )
        {
            _filename = filename;
            _buffers = new List<byte[]>();
            FileStream fs = new FileStream( filename, FileMode.Create, FileAccess.Write );
            _binWrite = new BinaryWriter( fs );
        }

        public void Add( byte[] packet )
        {
            _buffers.Add( packet );
        }

        public void Write()
        {
            uint maxLen = 0;

            foreach ( byte[] packet in _buffers )
            {
                if ( maxLen < packet.Length )
                    maxLen = (uint)packet.Length;
            }

            WriteHeader( maxLen );

            foreach ( byte[] packet in _buffers )
            {
                // timestamp - we don't have any
                _binWrite.Write( (UInt64)0 );
                // snap length
                _binWrite.Write( (UInt32)packet.Length );
                // original lenght
                _binWrite.Write( (UInt32)packet.Length );
                _binWrite.Write( packet );
            }

            _binWrite.Close();
        }

        public void Close()
        {
            _binWrite.Close();
        }

        protected void WriteHeader(uint maxLen)
        {
            _binWrite.Write( MAGIC );
            _binWrite.Write( (UInt16)2 );
            _binWrite.Write( (UInt16)4 );
            _binWrite.Write( (UInt32)0 );
            _binWrite.Write( (UInt32)0 );
            _binWrite.Write( maxLen );
            // TODO: fixme - that's just Ethernet
            _binWrite.Write( (UInt32)1 );
        }
    }
}
