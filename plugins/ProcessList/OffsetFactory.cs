// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginResults;
using Recurity.CIR.Plugins.ProcessList.Offsets;

namespace Recurity.CIR.Plugins.ProcessList
{
    internal static class OffsetFactory
    {
        /// <summary>
        /// This method returns the right process list element offset 
        /// class depending on the IOS version.
        /// ((All the magic happens in here.))
        /// </summary>
        /// <param name="imageSignature">Signature of the IOS image</param>
        /// <returns>Object containing all the offsets we know</returns>
        internal static IProcessListOffsets Offsets( IIOSSignature imageSignature )
        {
            IOSVersion v11_3 = new IOSVersion( "11.3(0)" );
            IOSVersion v12_0 = new IOSVersion( "12.0(0)" );
            IOSVersion v12_1 = new IOSVersion( "12.1(0)" );
            IOSVersion v12_2 = new IOSVersion( "12.2(0)" );
            IOSVersion v12_3 = new IOSVersion( "12.3(0)" );
            IOSVersion v12_4 = new IOSVersion( "12.4(0)" );
            IOSVersion v12_5 = new IOSVersion( "12.5(0)" );

            //
            // more specific matching first: 
            //
            if ((imageSignature.IOSVersion == new IOSVersion("12.4(15)T")) && (imageSignature.KnownPlatform == CiscoPlatforms.C1700))
            {
                return new ProcessListOffsets_12_4_15T();
            }
            else if ((imageSignature.IOSVersion == new IOSVersion("12.3(7)T1")) && (imageSignature.KnownPlatform == CiscoPlatforms.C1700))
            {
                return new ProcessListOffsets_12_3_7_T1();
            }
            else if ((imageSignature.IOSVersion == new IOSVersion("12.4(1c)")) && (imageSignature.KnownPlatform == CiscoPlatforms.C1700))
            {
                return new ProcessListOffsets_12_4_1c();
            }
            else if((imageSignature.IOSVersion == new IOSVersion("12.3(11)YZ2")) && (imageSignature.KnownPlatform == CiscoPlatforms.C2600))
            {
                return new ProcessListOffsets_12_3_11_YZ2();
            }
            else if ((imageSignature.IOSVersion == new IOSVersion("12.3(8)T11")) && (imageSignature.KnownPlatform == CiscoPlatforms.C2600))
            {
                return new ProcessListOffsets_12_3_8_T11();
            }
            else if ((imageSignature.IOSVersion == new IOSVersion("12.4(11)T")) && (imageSignature.KnownPlatform == CiscoPlatforms.C2600))
            {
                return new ProcessListOffsets_12_4_11T();
            }

            else if ((imageSignature.IOSVersion == new IOSVersion("12.4(2)T")) && (imageSignature.KnownPlatform == CiscoPlatforms.C2600))
            {
                return new ProcessListOffsets_12_4_2T();
            }
            else if ((imageSignature.IOSVersion == new IOSVersion("12.4(3)")) && (imageSignature.KnownPlatform == CiscoPlatforms.C2600))
            {
                return new ProcessListOffsets_12_4_3();
            }
            else if ((imageSignature.IOSVersion == new IOSVersion("12.4(3j)")) && (imageSignature.KnownPlatform == CiscoPlatforms.C2600))
            {
                return new ProcessListOffsets_12_3J();
            }
            //
            // general Major/Minor version only 
            //
            if ( ( imageSignature.IOSVersion >= v11_3 ) && ( imageSignature.IOSVersion < v12_0 ) )
            {
                return new ProcessListOffsets_11_3();
            }
            else if ( ( imageSignature.IOSVersion >= v12_0 ) && ( imageSignature.IOSVersion < v12_1 ) )
            {                
                return new ProcessListOffsets_12_0();
            }
            else if ( ( imageSignature.IOSVersion >= v12_1 ) && ( imageSignature.IOSVersion < v12_2 ) )
            {
                return new ProcessListOffsets_12_1();
            }
            else if ( ( imageSignature.IOSVersion >= v12_2 ) && ( imageSignature.IOSVersion < v12_3 ) )
            {
                return new ProcessListOffsets_12_2();
            }
            else if ( ( imageSignature.IOSVersion >= v12_3 ) && ( imageSignature.IOSVersion < v12_4 ) )
            {
                return new ProcessListOffsets_12_3();
            }
            else if ( ( imageSignature.IOSVersion >= v12_4 ) && ( imageSignature.IOSVersion < v12_5 ) )
            {                
                return new ProcessListOffsets_12_4();
            }
            else
            {
                throw new NotImplementedException(
                    "ProcessList offsets for image version number " +
                    imageSignature.Version +
                    " unknown"
                    );
            }
        }
    }
}
