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

namespace Recurity.CIR.Plugins.CiscoCore
{
    public class CoreFileRead : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        private CoreResult _coreResult;
        private ICiscoMainCoreFile _file;
        private IPlatformPlugin _platformInfo;

        public override string Name
        {
            get { return "Core (main) File Loader"; }
        }

        public override string Description
        {
            get { return "Loads the main core file dumped by an IOS device"; }
        }

        protected override void InitializeInternal()
        {
            // nothing to be done
        }

        protected override uint doStep()
        {
            _coreResult = new CoreResult( _file, _platformInfo.CoreMemoryBase, _platformInfo.BigEndian );
            done( _coreResult );
            return 100;
        }

        protected override void CancelInternal()
        {
            // nothing
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
                Type[] ret = { typeof( ICiscoCoreMemory ), typeof( ICiscoCoreMemoryMap ) };
                return ret;
            }
        }

        public IPluginResult[] Results
        {
            get
            {
                //return ArrayMaker.Make( (ICiscoCoreMemory)Result, (ICiscoCoreMemoryMap)Result );
                return ArrayMaker.Make( Result );
            }
        }

        public Type[] Requirements
        {
            get 
            {
                Type[] ret = { typeof( ICiscoMainCoreFile ), typeof(IPlatformPlugin) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _file = (ICiscoMainCoreFile)requirements[ 0 ];
            _platformInfo = (IPlatformPlugin)requirements[ 1 ];
        }

        #endregion
    }
}
