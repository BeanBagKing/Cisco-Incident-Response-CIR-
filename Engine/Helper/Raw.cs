// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Recurity.CIR.Engine.Helper
{
    public class Raw
    {
        /// <summary>
        /// Converts any object into a raw binary stream
        /// </summary>
        /// <param name="anything">Object to convert</param>
        /// <returns></returns>
        public static byte[] RawSerialize( object anything )
        {
            int rawsize = Marshal.SizeOf( anything );
            IntPtr buffer = Marshal.AllocHGlobal( rawsize );

            Marshal.StructureToPtr( anything, buffer, false );

            byte[] rawdata = new byte[ rawsize ];

            Marshal.Copy( buffer, rawdata, 0, rawsize );
            Marshal.FreeHGlobal( buffer );

            return rawdata;
        }

        /// <summary>
        /// Converts a binary stream into any object.
        /// </summary>
        /// <param name="rawdata">Binary data of sufficient length</param>
        /// <param name="anytype">Destination type, use aka var.GetType()</param>
        /// <returns></returns>
        public static object RawDeserialize( byte[] rawdata, Type anytype )
        {
            int rawsize = Marshal.SizeOf( anytype );

            if ( rawsize > rawdata.Length )
                throw new System.ArgumentOutOfRangeException( "rawdata" );

            IntPtr buffer = Marshal.AllocHGlobal( rawsize );

            Marshal.Copy( rawdata, 0, buffer, rawsize );

            object retobj = Marshal.PtrToStructure( buffer, anytype );

            Marshal.FreeHGlobal( buffer );

            return retobj;
        }
    }
}
