// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using PluginHost;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;

namespace Recurity.CIR.Plugins.IOCoreFileReader
{
    public class IOCoreReader : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        private ICiscoIOCoreFile _file;
        private IPlatformPlugin _platform;

        #region IAnalysisPlugin Members

        public override string Name
        {
            get { return "Core (IO Memory) File Loader"; }
        }

        public override string Description
        {
            get { return "Loads the IOMEM core file dumped by an IOS device"; }
        }

        protected override void InitializeInternal()
        {
            // nothing to be done
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
                Type[] ret = {typeof (ICiscoCoreMemory), typeof (ICiscoCoreMemoryMap)};
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
                Type[] ret = {typeof (ICiscoIOCoreFile), typeof (IPlatformPlugin)};
                return ret;
            }
        }

        public void FulFill(object[] requirements)
        {
            _file = (ICiscoIOCoreFile) requirements[0];
            _platform = (IPlatformPlugin) requirements[1];
        }

        #endregion

        protected override uint doStep()
        {
            IOCoreResult res = new IOCoreResult(_file, _platform.IOCoreMemoryBase, _platform.BigEndian);
            done(res);
            return 100;
        }

        protected override void CancelInternal()
        {
            // nothing
        }
    }
}