// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 

using System.Collections.Generic;
using System.IO;
using System.Text;
using PluginHost;
using PluginHost.PluginValidation;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.Report;

namespace Recurity.CIR.Engine
{
    public class CirMain
    {
        protected AnalysisHost<IAnalysisPlugin> _analysisPlugins;

        public CirMain(PluginValidator theValidator)
        {
            if (theValidator == null) throw new ArgumentNullException("theValidator");
            _analysisPlugins = new AnalysisHost<IAnalysisPlugin>(theValidator);
        }

        //
        // Platform Plugin Management
        //

        public void LoadPlatformPlugin( string filename )
        {
            if (filename == null)
                throw new ArgumentException("Filename must not be null");
            filename = Path.GetFullPath(filename.Trim());

            if (!File.Exists(filename))
                throw new ArgumentException(String.Format("File {0} does not exists", filename));
            _analysisPlugins.LoadPlatformPlugin(filename);
        }

        public void AddPlatformPlugin(IPlatformPlugin plugin)
        {
            if (plugin == null) throw new ArgumentNullException("plugin");
            _analysisPlugins.AddPlatform(plugin);
        }

        public void AddAnalysisPlugin(IAnalysisPlugin plugin)
        {
            if (plugin == null) throw new ArgumentNullException("plugin");
            _analysisPlugins.Add(plugin);
        }

        public List<IPlatformPlugin> PlatformPlugins
        {
            get
            {
                return _analysisPlugins.PlatformPlugins;
            }
        }

        public CiscoPlatforms Platform
        {
            get
            {
                return _analysisPlugins.Platform;
            }
            set
            {
                _analysisPlugins.Platform = value;
            }
        }
        

        public bool AutoDetectPlatform()
        {
            List<IIOSSignature> sigRes = _analysisPlugins.ResultStore.GetResults<IIOSSignature>();

            if ( 0 >= sigRes.Count )
            {
                return false;
            }
            else if ( 1 == sigRes.Count )
            {
                Platform = sigRes[ 0 ].KnownPlatform ;                
            }
            else
            {
                CiscoPlatforms p = sigRes[ 0 ].KnownPlatform;
                bool allTheSame = true;

                for ( int i = 1; i < sigRes.Count; i++ )
                {
                    allTheSame = allTheSame && ( sigRes[ i ].KnownPlatform == p );
                }

                if ( allTheSame )
                {
                    this.Platform = p;
                }
                else
                {
                    throw new ArgumentException( "IOSSignatures for multiple platforms detected" );
                }                    
            }

            return true;
        }



        //
        // Analysis Plugin Management
        //

        public void LoadPlugin( string filename )
        {
            if (filename == null)
                throw new ArgumentException("Filename must not be null");
            filename = Path.GetFullPath(filename.Trim());
            if(!File.Exists(filename))
            {
                throw new ArgumentException(String.Format("File {0} does not exists", filename));
            }
            _analysisPlugins.Add( filename );
        }

        public void LoadAllPlugins( string directory )
        {
            string[] dlls = Directory.GetFiles( directory, "*.dll" );

            foreach ( string dll in dlls )
            {
                try
                {
                    _analysisPlugins.Add( dll );
                }
                catch ( FormatException )
                {
                    // not a plugin, ignore
                }
            }
        }

        public List<IAnalysisPlugin> Plugins
        {
            get
            {
                return _analysisPlugins.Plugins;
            }
        }

        public List<IAnalysisPlugin> AnalysisModules
        {
            get
            {
                return _analysisPlugins.AvailablePlugins;
            }
        }

        public int PluginsCompleted
        {
            get
            {
                int cnt = 0;
                foreach ( IBackgroundPlugin p in Plugins )
                {
                    cnt += p.Done ? 1 : 0;
                }
                return cnt;
            }
        }
        
        public void UpdateDependencies()
        {
            // while we don't know the platform (it's NONE), try to autodetect it
            if ( _analysisPlugins.Platform == CiscoPlatforms.NONE )
                AutoDetectPlatform();

            _analysisPlugins.UpdateAvailablePlugins( );
        }

        public void AddPrerequisites( params IPluginResult[] list )
        {
            _analysisPlugins.AddResults( list );
        }

        public void AddPrerequisites(ICollection<IPluginResult> results)
        {
            _analysisPlugins.AddResults(results);
        }

        public List<IReport> GetReport()
        {
            List<IReport> reports = new List<IReport>();
            foreach (IPluginReporter o in _analysisPlugins.ResultStore.GetResults<IPluginReporter>())
            {
                reports.Add(o.Report);


            }
            return reports;
        }

        public void WritePluginDebugGraph( string filename )
        {
            _analysisPlugins.WritePluginDebugGraph( filename );
        }

        public override string ToString()
        {            
            StringBuilder sb = new StringBuilder();

            sb.AppendLine( GetType().ToString() );
            sb.AppendLine( "Platform Plugins:" );
            foreach ( IPlatformPlugin p in _analysisPlugins.PlatformPlugins )
            {
                sb.Append( "[$] " );
                sb.Append( p.Name );
                sb.Append( " in " );
                sb.AppendLine( p.ToString() );
            }

            sb.AppendLine( "Analysis Plugins:" );
            foreach ( IBackgroundPlugin p in _analysisPlugins.Plugins )
            {
                sb.Append( "[-] " );
                sb.Append( p.Name );
                sb.Append( " in " );
                sb.AppendLine( p.ToString() );
            }

            sb.AppendLine( "Plugins that don't support this platform:" );
            foreach ( IBackgroundPlugin p in _analysisPlugins.UnsupportedPlugins )
            {
                sb.Append( "[-] " );
                sb.Append( p.Name );
                sb.Append( " in " );
                sb.AppendLine( p.ToString() );
            }

            sb.AppendLine( "ResultStore:" );
            foreach ( IPluginResult o in _analysisPlugins.ResultStore.GetResults() )
            {
                sb.Append( "[+] " );
                sb.AppendLine( o.ToString() );
            }

            return sb.ToString();
        }
    }
}
