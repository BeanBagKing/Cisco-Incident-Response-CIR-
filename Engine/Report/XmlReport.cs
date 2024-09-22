// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Recurity.CIR.Engine.Report
{
    partial class Report : IReport
    {
        private readonly List<XmlElement> elements = new List<XmlElement>();
        private static readonly XmlSerializerNamespaces EMPTY_NAMESPACE = EmptyNamespace;

        private static XmlSerializerNamespaces EmptyNamespace
        {
            get
            {
                XmlSerializerNamespaces nameSpace = new XmlSerializerNamespaces();
                nameSpace.Add("", "");
                return nameSpace;

            }
        }
        private static XmlDocument SerializeToDocument(object entity)
        {
            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();

            /*UTF-8 is a good choice -> should be available on almost every platform.*/
            settings.Encoding = System.Text.Encoding.UTF8;
            using (XmlWriter xmlTextWriter = XmlWriter.Create(builder, settings))
            {
                XmlSerializer serializer = new XmlSerializer(entity.GetType());
                serializer.Serialize(xmlTextWriter, entity, EMPTY_NAMESPACE);

                String xmlString = builder.ToString();
                XmlDocument document = new XmlDocument();
                document.LoadXml(xmlString);
                return document;
            }
        }



        #region IReport Members

        public void AddReportNode(IReportNode node)
        {
            if (node == null)
                throw new ArgumentException("node must not be null");
            XmlDocument doc = SerializeToDocument(node);
            this.elements.Add(doc.DocumentElement);
        }

        public virtual void WriteTo(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentException("writer must not null");
            this.Any = this.elements.ToArray();
            /*UTF-8 is a good choice -> should be available on almost every platform.*/
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.CloseOutput = false;
            settings.ConformanceLevel = ConformanceLevel.Auto;

            XmlWriter xmlTextWriter = XmlWriter.Create(writer, settings);



            //xmlTextWriter.Formatting = Formatting.Indented;
            XmlSerializer serializer = new XmlSerializer(typeof(Report));

            serializer.Serialize(xmlTextWriter, this, EMPTY_NAMESPACE);



            writer.Flush();
        }

        #endregion
    }
}
