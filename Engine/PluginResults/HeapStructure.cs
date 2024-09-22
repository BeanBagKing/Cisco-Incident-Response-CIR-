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
    public class HeapStructure : AbstractHeapStructure, IHeapStructureMain, IPluginReporter
    {

        private readonly string pluginName;

        public HeapStructure(string pluginName)
        {
            this.pluginName = pluginName;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine( "Heap blocks:" );
            foreach ( IHeapBlock32 block in _struct )
            {
                sb.AppendFormat( "{0,8:X}: {1,8:X} {2} by {3,8:X} for '{4}' (PID {5:d})",
                    block.Address,
                    block.Size,
                    block.Used ? "used" : "free",
                    block.Used ? block.AllocPC : block.FreePC,
                    block.AllocNameString,
                    block.PID
                    );
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
                report.Author = pluginName;
                uint free = 0;
                foreach (IHeapBlock32 block in _struct)
                {
                    HeapBlockResult heapBlock = new HeapBlockResult();
                    heapBlock.address = block.Address;
                    heapBlock.@for = block.AllocNameString;
                    heapBlock.pid = block.PID;
                    if (block.Used)
                    {

                        heapBlock.state = "used";
                        heapBlock.by = block.AllocPC;
                    }
                    else
                    {
                        ++free;
                        heapBlock.state = "free";
                        heapBlock.by = block.FreePC;
                    }
                    report.AddReportNode(heapBlock);
                        
                }
                uint count = (uint)this._struct.Count;
                report.Summary = String.Format("Found {0} main heap blocks. {1} in use; {2} free", count, count-free, free);
                return report;
            }
        }

        public bool Errors { get { return Report.State == 2; } }

        
    }
}
