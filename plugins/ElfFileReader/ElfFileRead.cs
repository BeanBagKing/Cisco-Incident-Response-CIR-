// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using PluginHost;
using Recurity.CIR.ELF;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;

// FUUUUUCK SVN !!1!

namespace Recurity.CIR.Plugins.ElfFileRead
{
    public class ElfFileRead : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        private ElfFileMemory _elfResult;
        private IElfUncompressedFile _file;
        private IPlatformPlugin _platform;

        #region IAnalysisPlugin Members

        public override string Name
        {
            get { return "ELF File Parser (uncompressed)"; }
        }

        public override string Description
        {
            get { return "Parses uncompressed ELF files"; }
        }

        protected override void InitializeInternal()
        {
            // nothing to be done
        }

        public CiscoPlatforms[] Platforms
        {
            get
            {
                CiscoPlatforms[] ret = new CiscoPlatforms[1];
                ret[0] = CiscoPlatforms.ANY;
                return ret;
            }
        }

        public Type[] ResultTypes
        {
            get { return ArrayMaker.Make(typeof (IElfMemory), typeof (IElfMemoryMap)); }
        }

        public IPluginResult[] Results
        {
            get
            {
                //return ArrayMaker.Make( (IElfMemory)Result, (IElfMemoryMap)Result );
                return ArrayMaker.Make(Result);
            }
        }

        public Type[] Requirements
        {
            get { return ArrayMaker.Make(typeof (IElfUncompressedFile), typeof (IPlatformPlugin)); }
        }

        public void FulFill(object[] requirements)
        {
            _file = (IElfUncompressedFile) requirements[0];
            _platform = (IPlatformPlugin) requirements[1];
        }

        #endregion

        protected override uint doStep()
        {
            _elfResult = new ElfFileMemory(_file, _platform.VirtualAddressMapper);
            done(_elfResult);
            return 100;
        }

        protected override void CancelInternal()
        {
            // nothing
        }
    }
}