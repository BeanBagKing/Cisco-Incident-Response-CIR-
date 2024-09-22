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

namespace Recurity.CIR.Plugins.ElfIOSSignature
{
    public class ElfIOSSig : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        protected IOSSignatureElf _sig;
        protected IElfMemory _mem;
        protected IElfMemoryMap _map;
        protected IVirtualMemorySection _dataSection;

        protected UInt64 _current;
        protected bool _sigStartDetected;
        protected UInt64 _sigStartAddress;

        public override string Name
        {
            get { return "IOS Signature Detection (ELF)"; }
        }

        public override string Description
        {
            get { return "Detects the IOS signature values in an ELF file (uncompressed)"; }
        }

        protected override void InitializeInternal()
        {
            foreach ( IVirtualMemorySection sec in _map.GetKnownMemorySections() )
            {
                if ( sec.Name.StartsWith( ".data", StringComparison.CurrentCultureIgnoreCase ) )
                {
                    _dataSection = sec;
                    _sigStartDetected = false;
                    _current = sec.Address;
                    _sig = new IOSSignatureElf(this.GetType().ToString());
                    return;
                }
            }

            // fall through 
            throw new FormatException( "Section .data not found" );
        }

        protected override uint doStep()
        {
            if ( _current >= ( ( _dataSection.Address + _dataSection.Size ) - 10 ) )
            {
                // TODO: not clear how we should handle it if we don't find
                // any signature in .DATA
                throw new FormatException( "No CW_BEGIN in .data found" );
                //done( null );
                //return 100;
            }

            if ( !_sigStartDetected )
            {
                string testLine = _mem.GetString( _current );
                if ( testLine.StartsWith( "CW_BEGIN" ) )
                {
                    _sigStartDetected = true;
                    _sigStartAddress = _current;
                    _current += (UInt64)testLine.Length;
                }
                else
                    _current ++;
            }
            else
            {
                string element;
                element = _mem.GetString( _current );

                if ( element.StartsWith( "CW_IMAGE" ) )
                {
                    _sig.Image = GetDollarElements( element )[ 1 ];
                    _current += (UInt64)element.Length;
                }
                else if ( element.StartsWith( "CW_FAMILY" ) )
                {
                    _sig.Family = GetDollarElements( element )[ 1 ];
                    _current += (UInt64)element.Length;
                }
                else if ( element.StartsWith( "CW_FEATURE" ) )
                {
                    _sig.FeatureSet = GetDollarElements( element )[ 1 ];
                    _current += (UInt64)element.Length;
                }
                else if ( element.StartsWith( "CW_VERSION" ) )
                {
                    _sig.Version = GetDollarElements( element )[ 1 ];
                    _current += (UInt64)element.Length;
                }
                else if ( element.StartsWith( "CW_MEDIA" ) )
                {
                    _sig.Media = GetDollarElements( element )[ 1 ];
                    _current += (UInt64)element.Length;
                }
                else if ( element.StartsWith( "CW_END" ) )
                {
                    done( _sig );
                    return 100;
                }
                else
                {
                    _current++;
                }

                //
                // safety check
                //
                if ( _current > ( _sigStartAddress + 2048 ) )
                {
                    //
                    // signature may not be bigger than 2k 
                    //
                    // reset everything as if we didn't find CW_BEGIN
                    //
                    _sigStartDetected = false;
                    _current = _sigStartAddress + 1;
                    _sig = new IOSSignatureElf(this.GetType().ToString());
                    
                    throw new NotImplementedException( "Signature finding safety check triggered" );
                }

            }

            return (uint)( ( (float)( _current - _dataSection.Address) / (float)_dataSection.Size ) * (float)100 );
        }

        protected override void CancelInternal()
        {
            // nothing
        }

        private string[] GetDollarElements( string input )
        {
            char[] delimiter = "$".ToCharArray();
            string[] lines = input.Split( delimiter );
            return lines;
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
                Type[] ret = { typeof( IIOSSignatureElf ) };
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
                Type[] ret = { typeof( IElfMemory ), typeof( IElfMemoryMap ) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _mem = (IElfMemory)requirements[ 0 ];
            _map = (IElfMemoryMap)requirements[ 1 ];
        }

        #endregion
    }
}