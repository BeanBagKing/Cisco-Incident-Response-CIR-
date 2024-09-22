// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;


using Recurity.CIR.Engine.Interfaces;
using Recurity.CIR.Engine.Report;
namespace Recurity.CIR.Engine.PluginResults
{
    public sealed class PacketHeaderStructure : IPacketHeaderStructure
    {
        private readonly List<IPacketHeader> _list;
        private readonly string pluginName;

        public PacketHeaderStructure(string pluginName)
        {
            this.pluginName = pluginName;
            _list = new List<IPacketHeader>();
        }

        public List<IPacketHeader> Headers
        {
            get { return _list; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine( base.ToString() );
            sb.AppendLine("Packet Headers:");
            foreach ( IPacketHeader p in _list )
            {
                sb.AppendFormat( "{0,8:X}: Next: {1,8:X} Frame: {2,8:X} Size: {3}", 
                    p.Address, 
                    p.Next, 
                    p.Frame,
                    p.Size);
                sb.AppendLine();
            }

            return sb.ToString();
        }

     

        public Recurity.CIR.Engine.Report.IReport Report
        {
            get 
            {
                Recurity.CIR.Engine.Report.IReport report = ReportFactory.Instance.CreatePluginReport();
                report.Details = 1;
                report.Summary = String.Format("Found {0} packet header.", this._list.Count);
                report.Author = this.pluginName;
                // TODO: (simonw) add description
                report.Description = "";
                report.State = 0;
                
                foreach(PacketHeader header in this._list)
                {
                    Recurity.CIR.Engine.PluginResults.Xml.PacketHeaderResult xmlHeader = new Recurity.CIR.Engine.PluginResults.Xml.PacketHeaderResult();
                    xmlHeader.address = header.Address;
                    xmlHeader.frame = header.Frame;
                    xmlHeader.next = header.Next;
                    xmlHeader.size = header.Size;
                    report.AddReportNode(xmlHeader);

                }
                               
                return report;
            }
        }
        public bool Errors { get { return Report.State == 2; } }

     
    }
}
