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

namespace Recurity.CIR.Plugins.ReportSignature
{
    public class ReportSignature : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        protected IIOSSignatureElf _elfSignature;
        protected IIOSSignatureCore _coreSignature;
       


        public override string Name
        {
            get { return "Signature Report Generator"; }
        }

        public override string Description
        {
            get { return "Generates the Image and Core signature report"; }
        }       

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
                Type[] ret = { typeof( ReportSignatureResult ) };
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
                Type[] ret = { typeof( IIOSSignatureElf ), typeof( IIOSSignatureCore ) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _elfSignature = (IIOSSignatureElf)requirements[ 0 ];
            _coreSignature = (IIOSSignatureCore)requirements[ 1 ];
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
            StringBuilder imageSig = new StringBuilder(_elfSignature.Family).Append(" ");
            imageSig.Append(_elfSignature.Version).Append(" ");
            imageSig.Append(_elfSignature.FeatureSet).Append(" ");
             
            StringBuilder coreSig = new StringBuilder(_coreSignature.Family).Append(" ");
            coreSig.Append(_coreSignature.Version).Append(" ");
            coreSig.Append(_coreSignature.FeatureSet).Append(" ");
            ReportSignatureResult result = new ReportSignatureResult( this.ToString(),
                true,"not implemented",imageSig.ToString(), coreSig.ToString() );
            done( result );
            return 100;
        }
    }
}
