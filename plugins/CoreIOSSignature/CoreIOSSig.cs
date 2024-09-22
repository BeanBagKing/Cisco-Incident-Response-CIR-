// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.IO;
using System.Text;
using PluginHost;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.PluginResults;

namespace Recurity.CIR.Plugins.CoreIOSSignature
{
    public class CoreIOSSig : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        protected const UInt64 _PER_STEP = 1048576; // 1MB per step 
        protected BinaryEndianessReader _binRead;
        protected UInt64 _current;
        protected Stream _fileStream;
        protected ICiscoMainCoreFile _mainCoreFile;
        protected IOSSignatureCore _sig;
        protected bool _sigStartDetected;
        protected UInt64 _sigStartOffset;

        #region IAnalysisPlugin Members

        public override string Name
        {
            get { return "IOS Signature Detection (Core)"; }
        }

        public override string Description
        {
            get { return "Detects the IOS signature values in a Core file"; }
        }

        protected override void InitializeInternal()
        {
            _fileStream = _mainCoreFile.Stream(FileMode.Open, FileAccess.Read);
            _binRead = new BinaryEndianessReader(_fileStream, Encoding.GetEncoding("us-ascii"));

            _current = 0;
            _sigStartDetected = false;
            _sig = new IOSSignatureCore(GetType().ToString());
            RegisterDisposeableResource(_fileStream);
        }

        public CiscoPlatforms[] Platforms
        {
            get
            {
                CiscoPlatforms[] ret = {CiscoPlatforms.ANY};
                return ret;
            }
        }

        public Type[] ResultTypes
        {
            get
            {
                Type[] ret = {typeof (IIOSSignatureCore)};
                return ret;
            }
        }

        public IPluginResult[] Results
        {
            get { return ArrayMaker.Make(Result); }
        }

        public Type[] Requirements
        {
            get
            {
                Type[] ret = {typeof (ICiscoMainCoreFile)};
                return ret;
            }
        }

        public void FulFill(object[] requirements)
        {
            _mainCoreFile = (ICiscoMainCoreFile) requirements[0];
        }

        #endregion

        protected override uint doStep()
        {
            UInt64 thisStep =
                (UInt64) (_fileStream.Length - (long) _current) > _PER_STEP
                    ?
                        _PER_STEP
                    :
                        (UInt64) (_fileStream.Length - (long) _current);

            if ((long) _current >= (_fileStream.Length - 10))
            {
                throw new FormatException("No CW_BEGIN found in core");
            }

            if (!_sigStartDetected)
            {
                // TODO: Let's see if we need this
                _binRead.BaseStream.Seek((long) _current, SeekOrigin.Begin);
                byte[] testbuf = _binRead.ReadBytes((int) thisStep);

                for (int i = 0; (UInt64) i < (thisStep - 4); i++)
                {
                    if (
                        (0x43 == testbuf[i]) &&
                        (0x57 == testbuf[i + 1]) &&
                        (0x5F == testbuf[i + 2]) &&
                        (0x42 == testbuf[i + 3]))
                    {
                        // 
                        // We are reading chunks of "thisStep" size, the current one already being
                        // in testbuf and the file pointer being after the chunk. Now we have to go
                        // ( _current - i ) bytes backwards to read the actual string.
                        //
                        _binRead.BaseStream.Seek((long) _current + i, SeekOrigin.Begin);
                        string testLine = _binRead.ReadStringZeroTerminated();

                        if (testLine.StartsWith("CW_BEGIN"))
                        {
                            _sigStartDetected = true;
                            _sigStartOffset = _current + (UInt64) i;
                            _current = _sigStartOffset + (UInt64) testLine.Length;
                            break;
                        }
                    }
                }

                if (! _sigStartDetected)
                {
                    _current += (thisStep - 5);
                }
            }
            else
            {
                string element;
                _binRead.BaseStream.Seek((long) _current, SeekOrigin.Begin);
                element = _binRead.ReadStringZeroTerminated();

                if (element.StartsWith("CW_IMAGE"))
                {
                    _sig.Image = GetDollarElements(element)[1];
                    _current += (UInt64) element.Length;
                }
                else if (element.StartsWith("CW_FAMILY"))
                {
                    _sig.Family = GetDollarElements(element)[1];
                    _current += (UInt64) element.Length;
                }
                else if (element.StartsWith("CW_FEATURE"))
                {
                    _sig.FeatureSet = GetDollarElements(element)[1];
                    _current += (UInt64) element.Length;
                }
                else if (element.StartsWith("CW_VERSION"))
                {
                    _sig.Version = GetDollarElements(element)[1];
                    _current += (UInt64) element.Length;
                }
                else if (element.StartsWith("CW_MEDIA"))
                {
                    _sig.Media = GetDollarElements(element)[1];
                    _current += (UInt64) element.Length;
                }
                else if (element.StartsWith("CW_END"))
                {
                    done(_sig);
                    return 100;
                }
                else
                {
                    _current++;
                }

                //
                // safety check
                //
                if (_current > (_sigStartOffset + 2048))
                {
                    //
                    // signature may not be bigger than 2k 
                    //
                    // reset everything as if we didn't find CW_END
                    //
                    _sigStartDetected = false;
                    _current = _sigStartOffset + 1;
                    _sig = new IOSSignatureCore(GetType().ToString());

                    throw new NotImplementedException("Signature finding safety check triggered");
                }
            }

            return (uint) (((float) (_current)/(float) _fileStream.Length)*(float) 100);
        }

        protected override void CancelInternal()
        {
            // nothing
        }

        private string[] GetDollarElements(string input)
        {
            char[] delimiter = "$".ToCharArray();
            string[] lines = input.Split(delimiter);
            return lines;
        }
    }
}