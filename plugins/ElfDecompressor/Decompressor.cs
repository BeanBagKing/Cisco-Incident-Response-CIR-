// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

using Recurity.CIR.ELF;
using PluginHost;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.PluginResults;

namespace ElfDecompressor
{
    public class Decompressor : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        protected const UInt32 MAGIC = 0xFEEDFACE;
        protected const UInt32 ZIPMAGIC = 0x504B0304;
        protected const string DECOMPDIR = @"uncompress\";
        protected const int BUFFERSIZE = ( 1024 * 1024 );

        protected FileRepresentation file;
        protected string _tempDir;
       // protected ElfFileMemory _outerElf;

        protected ZipInputStream _zipInputStream;
        protected ZipEntry _zipEntry;
        protected ulong _sizeInSection;
        protected string _firstFileName;
        protected long _fileOffset;
        protected FileStream _writerStream;
        protected int _decompBufferSize;
        protected byte[] _decompBuffer;
        protected ulong _processedBytes;
        protected ulong _uncompressedSizeXCheck;


        public override string Name
        {
            get { return "IOS ELF Unpacker"; }
        }

        public override string Description
        {
            get { return "Extracts the compressed IOS image from the loader ELF image"; }
        }

        protected override void InitializeInternal()
        {
            IVirtualAddressMapper nullMapper = null;
            bool zipSectionFound = false;

            _tempDir = Path.Combine( _tempDir, DECOMPDIR );
            Directory.CreateDirectory( _tempDir );

            using ( ElfFileMemory _outerElf = new ElfFileMemory( file, nullMapper ) )
            {

                Stream fileStream = null;

                try
                {                    
                    //
                    // Packed IOS images have only one program header (at least one relevant)
                    //
                    UInt64 programHeaderBaseAddress = _outerElf.ProgramHeaders[ 0 ].VirtualAddress;

                    fileStream = file.Stream( FileMode.Open, FileAccess.Read );

                    foreach ( IVirtualMemorySection sec in _outerElf.GetKnownMemorySections() )
                    {
                        if ( sec.Properties.DataAvailable && ( sec.Size > 0x1F ) )
                        {
                            //
                            // we are using a running variable of programHeaderBaseAddress instead of 
                            // the section's sec.Address here, since there are images where the section
                            // address of the packed payload image is simply bullshit.
                            //
                            if ( ( MAGIC == _outerElf.GetUInt32( programHeaderBaseAddress ) ) &&
                                ( ZIPMAGIC == _outerElf.GetUInt32( programHeaderBaseAddress + 20 ) ) )
                            {
                                _uncompressedSizeXCheck = _outerElf.GetUInt32( programHeaderBaseAddress + 4 );
                                // compressed image length
                                uint size = _outerElf.GetUInt32( programHeaderBaseAddress + 8 );
                                // file offset of the ZIP magic
                                long offset = (long)_outerElf.VirtualAddress2FileOffset( programHeaderBaseAddress + 20 );



                                _zipInputStream = new ZipInputStream( fileStream );
                                fileStream.Seek( offset, SeekOrigin.Begin );

                                if ( null != ( _zipEntry = ( _zipInputStream.GetNextEntry() ) ) )
                                {
                                    zipSectionFound = true;
                                    _firstFileName = Path.GetFileName( _zipEntry.Name );
                                    if ( _firstFileName.Length < 1 )
                                        _firstFileName = "uncompressed_nameless.bin";

                                    _fileOffset = offset;

                                    if ( size < _zipEntry.CompressedSize )
                                    {
                                        throw new FormatException( "Cisco reported size " + size.ToString( "d" ) +
                                                                  " < Zip reported entry size " +
                                                                  _zipEntry.CompressedSize.ToString( "d" ) );
                                    }
                                    else
                                        _sizeInSection = (ulong)_zipEntry.Size;

                                    _writerStream = File.Create( Path.Combine( _tempDir, _firstFileName ) );
                                    _decompBufferSize = BUFFERSIZE;
                                    _decompBuffer = new byte[ _decompBufferSize ];
                                    _processedBytes = 0;

                                    // we are done, only one ZIP entry currently supported
                                    break;
                                }
                            }
                        }

                        //
                        // if the section is included in the file, we advance the address, 
                        // as sections without data available will not be part of the file
                        //
                        if ( sec.Properties.DataAvailable )
                            programHeaderBaseAddress += sec.Size;
                    }


                    if ( !zipSectionFound )
                    {
                        throw new FormatException( "No suitable compressed section found in file" );
                    }
                }
                catch ( Exception )
                {
                    Dispose( fileStream );
                    Dispose( _zipInputStream );
                    Dispose( _writerStream );
                    throw;
                }

            }
            // will be disposed when the plugin terminates
            RegisterDisposeableResource( _writerStream );
            RegisterDisposeableResource( _zipInputStream );
        }

        protected override uint doStep()
        {
            int inSize = _zipInputStream.Read( _decompBuffer, 0, _decompBufferSize );

            if ( inSize <= 0 )
            {
                FileInfo fi = new FileInfo( Path.Combine( _tempDir, _firstFileName ) );

                //
                // sanity check (not sure who's sanity we are checking here, Cisco's 
                // or ours)
                //
                if ( (ulong)fi.Length != _uncompressedSizeXCheck )
                {
                    throw new FormatException( "Uncompressed " + fi.Length + " but expected " + _uncompressedSizeXCheck );
                }

                ELFuncompressedFile res = new ELFuncompressedFile( fi );
                done( res );
            }
            else
            {
                _writerStream.Write( _decompBuffer, 0, inSize );
                _processedBytes += (ulong)inSize;
            }

            return (uint) ( ( (float)_processedBytes / (float)_sizeInSection ) * (float)100 );
        }

        protected override void CancelInternal()
        {
            
        }

        #region IAnalysisPlugin Members

        CiscoPlatforms[] IAnalysisPlugin.Platforms
        {
            get 
            {
                CiscoPlatforms[] ret = { CiscoPlatforms.ANY };
                return ret;
            }
        }

        Type[] IAnalysisPlugin.ResultTypes
        {
            get 
            {
                Type[] ret = { typeof(IElfUncompressedFile) };
                return ret;
            }
        }

        IPluginResult[] IAnalysisPlugin.Results
        {
            get 
            {   
                return Result != null ? ArrayMaker.Make( Result ): new IPluginResult[0];
            }
        }

        Type[] IAnalysisPlugin.Requirements
        {
            get 
            {
                Type[] ret = { typeof( IElfCompressedFile ), typeof( IPluginConfiguration ) };
                return ret;
            }
        }

        void IAnalysisPlugin.FulFill( object[] requirements )
        {
            file = ( (IElfCompressedFile)requirements[ 0 ] );
            _tempDir = ( (IPluginConfiguration)requirements[ 1 ] ).RuntimeConfiguration.OutputFolder;
        }

        #endregion
    }
}
