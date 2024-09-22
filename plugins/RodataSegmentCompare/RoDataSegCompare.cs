// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using PluginHost;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.PluginResults;
using Recurity.CIR.Engine.PluginResults.Xml;

namespace Recurity.CIR.Plugins.RoDataSegmentCompare
{
    internal class ComparisonDiff
    {
        public UInt64 virtualAddress;
        public UInt64 offsetElf;
        public UInt64 offsetCore;
        public UInt64 diffLength;
    }

    public class RoDataSegCompare : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        private IElfMemory _elf;
        private ICiscoCoreMemory _core;
        private IElfMemoryMap _elfmap;

        private UInt64 _compareStep;
        private UInt64 _compareCurrent;
        private UInt64 _dataStart;
        private UInt64 _dataEnd;
        private UInt64 _dataStartCore;

        private List<ComparisonDiff> _diffList;
        private bool _lastByteDiffers;

        public override string Name
        {
            get { return "ReadOnly Data Segment Validation"; }
        }

        public override string Description
        {
            get { return "Validates the .rodata segment of an IOS Core file using a known-to-be-good IOS image"; }
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
                Type[] ret = {typeof (SegmentCompareResult)};
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
                Type[] ret = {typeof (IElfMemory), typeof (ICiscoCoreMemory), typeof (IElfMemoryMap)};
                return ret;
            }
        }

        public void FulFill(object[] requirements)
        {
            _elf = (IElfMemory) requirements[0];
            _core = (ICiscoCoreMemory) requirements[1];
            _elfmap = (IElfMemoryMap) requirements[2];
        }

        protected override void InitializeInternal()
        {
            bool sectionFound = false;

            List<IVirtualMemorySection> sections = _elfmap.GetKnownMemorySections();

            foreach (IVirtualMemorySection s in sections)
            {
                if (s.Name.StartsWith(".rodata"))
                {
                    _dataStart = s.Address;
                    _dataEnd = s.Address + s.Size;
                    _compareStep = (UInt64) ((float) (_dataEnd - _dataStart)/100.0);
                    _compareCurrent = 0;
                    sectionFound = true;
                }
            }

            if (!sectionFound)
                throw new Exception("Could not find .rodata segment in ELF file");

            //
            // we know the section exists if we get here, so we just search for 
            // it in the adjusted map
            //
            foreach (IVirtualMemorySection s in _elfmap.GetPlatformAdjusted().GetKnownMemorySections())
            {
                if (s.Name.StartsWith(".rodata"))
                {
                    _dataStartCore = s.Address;
                }
            }

            _diffList = new List<ComparisonDiff>();
            _lastByteDiffers = false;
        }

        protected override uint doStep()
        {
            UInt64 i;
            UInt64 lengthInThisStep =
                (_dataStart + _compareCurrent + _compareStep) > _dataEnd
                    ?
                        (_dataEnd - (_dataStart + _compareCurrent))
                    :
                        (_compareStep);

            byte[] elfStep = _elf.GetBytes(_dataStart + _compareCurrent, (uint) lengthInThisStep);
            byte[] coreStep = _core.GetBytes(_dataStartCore + _compareCurrent, (uint) lengthInThisStep);

            for (i = 0; i < (lengthInThisStep - 1); i++)
            {
                //
                // in each doStep(), we compare lengthInThisStep (normally being _compareStep)
                // bytes individually. Once we hit a difference, we create an entry in _diffList
                // and keep counting it's length field up until we stop seeing a difference. 
                // This allows for clear statements of punctual patches in the .text segment (one
                // conditional jump) vs. changes that offset the whole image.
                //
                if (elfStep[(int) i] != coreStep[(int) i])
                {
                    /*
                    SegmentCompareResult result = new SegmentCompareResult(this.GetType().ToString(), false, "Read-Only data segment validation shows differences.");
                    SegmentDiffResult diff = new SegmentDiffResult();
                    diff.SetAddress("Start", ( (UInt64)( _dataStartCore + _compareCurrent + i ) ), "start of difference"); 
                    diff.SetElfOffset("Offset In Image", _elf.VirtualAddress2FileOffset( _dataStart + _compareCurrent + i ), "start of difference in image");
                    diff.SetCoreOffset("Offset In Core", _core.VirtualAddress2FileOffset( _dataStartCore + _compareCurrent + i ), "start of difference in core");
                    result.Diff = diff;

                    done( result );
                    break;
                     * */
                    if (_lastByteDiffers)
                    {
                        _diffList[_diffList.Count - 1].diffLength++;
                    }
                    else
                    {
                        ComparisonDiff diff = new ComparisonDiff();
                        diff.diffLength = 1;
                        diff.offsetCore = _core.VirtualAddress2FileOffset(_dataStartCore + _compareCurrent + i);
                        diff.offsetElf = _elf.VirtualAddress2FileOffset(_dataStart + _compareCurrent + i);
                        diff.virtualAddress = (UInt64) (_dataStartCore + _compareCurrent + i);
                        _diffList.Add(diff);
                    }

                    _lastByteDiffers = true;
                }
                else
                {
                    _lastByteDiffers = false;
                }
            }

            _compareCurrent += (i + 1);

            if ((_dataStart + _compareCurrent) == _dataEnd)
            {
                //
                // once we are done, the existence of _diffList entries 
                // decides if this was a successful comparison or if we 
                // found a modification
                //
                if (_diffList.Count == 0)
                {
                    SegmentCompareResult result =
                        new SegmentCompareResult(GetType().ToString(), true, "Read-Only data segment validation passed");
                    done(result);
                }
                else
                {
                    SegmentCompareResult result =
                        new SegmentCompareResult(GetType().ToString(), false,
                                                 "Read-Only data segment validation shows differences.");

                    foreach (ComparisonDiff diffEntry in _diffList)
                    {
                        SegmentDiffResult segDiff = new SegmentDiffResult();
                        segDiff.virtualAddress = diffEntry.virtualAddress;
                        segDiff.offsetElf = diffEntry.offsetElf;
                        segDiff.offsetCore = diffEntry.offsetCore;
                        segDiff.diffLength = diffEntry.diffLength;
                        segDiff.SegmentName = "RoData";

                        result.Diff.Add(segDiff);
                    }

                    done(result);
                }
                return 100;
            }
            else
            {
                return (uint) (
                                  ((float) ((_compareCurrent))/(float) (_dataEnd - _dataStart))*100.0);
            }
        }

        protected override void CancelInternal()
        {
            // nothing
        }
    }
}