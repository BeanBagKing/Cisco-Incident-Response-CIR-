// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.Engine.PluginResults;
using Recurity.CIR.Engine.PluginResults.Xml;

namespace Recurity.CIR.Engine.Interfaces
{
    abstract public class AbstractIOSSignature : IIOSSignature, IPluginReporter
    {
        protected string _image;
        protected string _family;
        protected string _featureSet;
        protected string _version;
        protected string _media;

        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public string Family
        {
            get { return _family; }
            set { _family = value; }
        }

        public CiscoPlatforms KnownPlatform
        {
            get 
            {
                if ( ( null == _family ) || ( _family.Length < 2 ) )
                    return CiscoPlatforms.NONE;

                if ( _family.StartsWith( "C2600" ) )
                    return CiscoPlatforms.C2600;
                else if ( _family.StartsWith( "C1700" ) )
                    return CiscoPlatforms.C1700;
                //TODO: if this works, move the above to this method
                else if ( _family.Equals( "C2691", StringComparison.InvariantCulture ) )
                    return CiscoPlatforms.C2691;

                else
                    return CiscoPlatforms.NONE;
            }            
        }

        public string FeatureSet
        {
            get { return _featureSet; }
            set { _featureSet = value; }
        }

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public IOSVersion IOSVersion
        {
            get
            {
                IOSVersion iosv = new IOSVersion( _version );
                return iosv;
            }
        }

        public string Media
        {
            get { return _media; }
            set { _media = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( base.ToString() );
            sb.Append( "Image: " );
            sb.AppendLine( _image );
            sb.Append( "Family: " );
            sb.AppendLine( _family );
            sb.Append( "Feature Set: ");
            sb.AppendLine( _featureSet );
            sb.Append( "Version: " );
            sb.AppendLine( _version );
            sb.Append( "Media: " );
            sb.AppendLine( _media );
            sb.Append( "Detected platform: " );
            sb.AppendLine( KnownPlatform.ToString() );
            return sb.ToString();
        }

        #region IPluginReporter Members

        public Recurity.CIR.Engine.Report.IReport Report
        {
            get { 
                Recurity.CIR.Engine.Report.IReport retVal = CreateReportInstance();
                
                SignatureResult signature = new SignatureResult();
                signature.detectedplatform = KnownPlatform.ToString();
                signature.family = _family;
                signature.featureset = _featureSet;
                signature.image = _image;
                signature.media = _media;
                signature.version = _version;

                retVal.Summary = String.Format("IOSSignature  Platform [{0}]" , signature.detectedplatform);
                retVal.AddReportNode(signature);
                return retVal;
            }
        }

        #endregion
        protected abstract Report.IReport CreateReportInstance();
        public bool Errors { get { return Report.State == 2; } }
    }
}
