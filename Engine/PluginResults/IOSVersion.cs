// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Recurity.CIR.Engine.PluginResults
{
    public class IOSVersion
    {
        protected const string VERSION_PATTERN = @"(?<major>\d+)\.(?<minor>\d+)\((?<release>\d+)(?<ext>[A-Za-z]*)\)(?<train>\w*)";

        protected UInt32 _major;
        protected UInt32 _minor;
        protected UInt32 _release;
        protected string _release_extension;
        protected string _train;

        public IOSVersion( string version )
        {
            Parse( version );
        }

        public static bool operator <( IOSVersion a, IOSVersion b )
        {
            return Comparison( a, b ) < 0;
        }

        public static bool operator >( IOSVersion a, IOSVersion b )
        {
            return Comparison( a, b ) > 0;
        }

        public static bool operator ==( IOSVersion a, IOSVersion b )
        {
            return Comparison( a, b ) == 0;
        }

        public static bool operator !=( IOSVersion a, IOSVersion b )
        {
            return Comparison( a, b ) != 0;
        }

        public static bool operator <=( IOSVersion a, IOSVersion b )
        {
            return Comparison( a, b ) <= 0;
        }

        public static bool operator >=( IOSVersion a, IOSVersion b )
        {
            return Comparison( a, b ) >= 0;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals( Object o )
        {
            IOSVersion otherVersion = (IOSVersion)o;

            return (
                ( _major == otherVersion._major )
                && ( _minor == otherVersion._minor )
                && ( _release == otherVersion._release )
                && ( _release_extension.Equals( otherVersion._release_extension ) )
                && ( _train.Equals( otherVersion._train ) )
            );                
        }

        public static int Comparison( IOSVersion a, IOSVersion b )
        {
            if ( a._major < b._major )
            {
                return -1;
            }
            else if ( a._major > b._major )
            {
                return 1;
            }
            else
            {
                if ( a._minor < b._minor )
                {
                    return -1;
                }
                else if ( a._minor > b._minor )
                {
                    return 1;
                }
                else
                {
                    if ( a._release < b._release )
                    {
                        return -1;
                    }
                    else if ( a._release > b._release )
                    {
                        return 1;
                    }
                    else
                    {
                        if ( a._release_extension.CompareTo( b._release_extension ) < 0 )
                        {
                            return -1;
                        }
                        else if ( a._release_extension.CompareTo( b._release_extension ) > 0 )
                        {
                            return 1;
                        }
                        else if ( a._release_extension.Equals( b._release_extension ) )
                        {
                            return a._train.CompareTo(b._train);                            
                        }
                        else 
                        {
                            throw new Exception( a.ToString() + " / " + b.ToString() + " compare exception" );
                        }
                    }
                }
            }
        }

        public UInt32 Major
        {
            get { return _major; }
        }

        public UInt32 Minor
        {
            get { return _minor; }
        }

        public UInt32 Release
        {
            get { return _release; }
        }

        public string ReleaseExtension
        {
            get { return _release_extension; }
        }

        public string Train
        {
            get { return _train; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append( _major );
            sb.Append( "." );
            sb.Append( _minor );
            sb.Append( "(" );
            sb.Append( _release );
            sb.Append( _release_extension );
            sb.Append( ")" );
            sb.Append( _train );
            return sb.ToString();
        }

        protected void Parse( string version )
        {
            Match match = Regex.Match( version, VERSION_PATTERN, RegexOptions.CultureInvariant );

            if ( match.Success )
            {
                GroupCollection gc = match.Groups;

                _major = UInt32.Parse( gc[ "major" ].Value );
                _minor = UInt32.Parse( gc[ "minor" ].Value );
                _release = UInt32.Parse( gc[ "release" ].Value );
                _release_extension = gc[ "ext" ].Value;
                _train = gc[ "train" ].Value;                
            }
            else
            {
                throw new FormatException( "Version '" + version + "' could not be matched using RegEx " + VERSION_PATTERN );
            }
        }
    }
}
