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

namespace Recurity.CIR.Plugins.AppleAndPears
{
    public class AppleAndPear : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        protected IIOSSignatureElf _elfSig;
        protected IOSSignatureCore _coreSig;

        public override string Name
        {
            get 
            {
                return "Consistency check image vs. core";
            }
        }

        public override string Description
        {
            get 
            {
                return "Ensures consistency between image and core platform and version";
            }
        }

        protected override void InitializeInternal()
        {
            // nothing
        }

        protected override void CancelInternal()
        {
            // nothing
        }

        protected override uint doStep()
        {
            if ( _elfSig.KnownPlatform != _coreSig.KnownPlatform )
            {
                throw new ApplicationException( "Platform of image and core differ!" );
            }

            if ( ! _elfSig.IOSVersion.Equals( _coreSig.IOSVersion ) )
            {
                throw new ApplicationException( "Version number of image ( " + _elfSig.Version + 
                    ") and core ( " + _coreSig.Version + ") differ!" );
            }
            AppleAndPearsResult result = new AppleAndPearsResult(ToString(), true, "Image and core are of the same version and platform, comparison valid.");

            done( result );
            return 100;
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
                Type[] ret = { typeof( AppleAndPearsResult ) };
                return ret;
            }
        }

        public IPluginResult[] Results
        {
            get 
            {
                return ArrayMaker.Make(Result);
            }
        }

        public Type[] Requirements
        {
            get 
            { 
                Type[] ret = { typeof( IIOSSignatureElf ), typeof( IIOSSignatureCore ) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _elfSig = (IOSSignatureElf)requirements[ 0 ];
            _coreSig = (IOSSignatureCore)requirements[ 1 ];            
        }

        #endregion
    }
}
