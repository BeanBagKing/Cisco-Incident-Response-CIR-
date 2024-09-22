// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.PluginResults.Xml;
using Recurity.CIR.Engine.Report;

namespace Recurity.CIR.Engine.PluginResults
{
    public class HeapStructureIO :  AbstractHeapStructure, IHeapStructureIO ,IPluginReporter
    {
        private readonly string pluginName;

        public HeapStructureIO(string pluginName)
        {
            this.pluginName = pluginName;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine( "IO Heap blocks:" );
            foreach ( IHeapBlock32 block in _struct)            
            {
                sb.AppendFormat( "{0,8:X}: {1,8:X} {2} by {3,8:X} for '{4}'",
                    block.Address,
                    block.Size,
                    block.Used?"used":"free",
                    block.Used?block.AllocPC:block.FreePC,
                    block.AllocNameString );
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public IReport Report
        {
            get
            {
                IReport report = ReportFactory.Instance.CreatePluginReport();
                report.Details = 1;
                report.State = 0;
                // TODO: (simonw) add description
                report.Description = "";
                
                report.Author = this.pluginName;
                uint free = 0;
                foreach (IHeapBlock32 block in _struct)
                {
                    HeapBlockResult heapBlock = new HeapBlockResult();
                    heapBlock.address = block.Address;
                    heapBlock.@for = block.AllocNameString;
                    if (block.Used)
                    {
                     
                        heapBlock.state= "used";
                        heapBlock.by = block.AllocPC;
                    }
                    else
                    {
                        ++free;
                        heapBlock.state= "free";
                        heapBlock.by = block.FreePC;
                    }
                                    
                    uint count = (uint)this._struct.Count;
                    report.Summary = String.Format("Found {0} IO heap blocks. {1} in use; {2} free", count, count-free, free);
                    report.AddReportNode(heapBlock);

                }

                return report;
            }
        }
        public bool Errors { get { return false; } }
    }
}
