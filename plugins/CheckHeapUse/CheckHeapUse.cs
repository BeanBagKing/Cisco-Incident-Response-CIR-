// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using PluginHost;
using Recurity.CIR.Engine;
using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginEngine;
using Recurity.CIR.Engine.PluginResults;

namespace Recurity.CIR.Plugins.CheckHeapUse
{
    public class CheckHeapUse : AbstractBackgroundPlugin, IAnalysisPlugin
    {
        private IPluginConfiguration _config;
        private IProcessList _processList;
        private IHeapStructureMain _mainHeap;
        private SortedDictionary<UInt32, List<IHeapBlock32>> _blockListMap;             
        private UInt64 _current;
        private CheckHeapUseResult _result;

        public override string Name
        {
            get { return "Check Heap Usage"; }
        }

        public override string Description
        {
            get { return "Correlation of heap blocks to owning processes"; }
        }

        protected override void InitializeInternal()
        {
            _blockListMap = new SortedDictionary<uint, List<IHeapBlock32>>();
            _result = new CheckHeapUseResult( this.GetType().ToString() );
            _current = 0;
        }

        protected override void CancelInternal()
        {
            // nothing
        }

        protected override uint doStep()
        {
            if ( _current == (ulong)_mainHeap.Blocks.Count )
            {
                finalize();
                done( _result );
                return 100;
            }

            IHeapBlock32 block = _mainHeap.Blocks[ (int)_current ];

            if ( block.Used )
            {
                if ( _blockListMap.ContainsKey( block.PID ) )
                {
                    _blockListMap[ block.PID ].Add( block );
                }
                else
                {
                    List<IHeapBlock32> newList = new List<IHeapBlock32>();
                    newList.Add( block );

                    _blockListMap.Add( block.PID, newList );
                }                
            }

            //
            // advance !
            //
            _current++;

            //
            // percentage
            //
            return (uint)( ( (float)_current / (float)_mainHeap.Blocks.Count ) * 100.0 );
        }

        private void finalize()
        {
            foreach ( UInt32 key in _blockListMap.Keys )
            {
                IProcessInformation proc = _processList.GetProcessByPID( (uint)key );

                if ( null == proc )
                {
                    ProcessInformation nproc = new ProcessInformation();
                    nproc.PID = key;
                    nproc.Name = "UNKNOWN PROCESS";
                    proc = nproc;
                }

                UInt64 sum = 0;
                foreach ( IHeapBlock32 block in _blockListMap[ key ] )
                {
                    sum += block.Size;
                }

                CheckHeapUseResultRecord rec = new CheckHeapUseResultRecord( proc, (ulong)_blockListMap[ key ].Count, sum );
                _result.Results.Add( rec );
            }

            WriteGraph( "HeapUseGraph.tgf" );
        }

        private struct Edge
        {
            public UInt32 source;
            public UInt32 destination;
        }

        private void WriteGraph( string filename )
        {
            StreamWriter sw = new StreamWriter( Path.Combine( _config.RuntimeConfiguration.OutputFolder, filename ) );
            
            UInt32 count = 1;
            List<Edge> edges = new List<Edge>();

            foreach ( UInt32 key in _blockListMap.Keys )
            {
                IProcessInformation proc = _processList.GetProcessByPID( key );                

                sw.WriteLine( "{0:d} {1:d} {2}",
                    count,
                    key,    // PID
                    proc == null ? "UNKNOWN PROCESS" : proc.Name
                );
                
                UInt32 count2 = count + 1;

                if ( key < 0x80000000 )
                {
                    //
                    // we don't want the two fsck'd up processes in here
                    //
                    foreach ( IHeapBlock32 b in _blockListMap[ key ] )
                    {
                        Edge e = new Edge();


                        sw.WriteLine( "{0:d} {1:X} {2}",
                            count2,
                            b.Address,
                            b.AllocNameString
                        );

                        e.source = count;
                        e.destination = count2;

                        edges.Add( e );

                        count2++;
                    }
                }

                count = count2;
            }            

            sw.WriteLine( "#" );

            foreach ( Edge e in edges )
            {
                sw.WriteLine( "{0:d} {1:d}",
                    e.source,
                    e.destination
                );
            }
            sw.Close();
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
                Type[] ret = { typeof( ICheckHeapUseResult ) };
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
                Type[] ret = { typeof( IPluginConfiguration ), typeof( IHeapStructureMain ), typeof( IProcessList ) };
                return ret;
            }
        }

        public void FulFill( object[] requirements )
        {
            _config = (IPluginConfiguration)requirements[ 0 ];
            _mainHeap = (IHeapStructureMain)requirements[ 1 ];
            _processList = (IProcessList)requirements[ 2 ];
        }

        #endregion
    }
}
