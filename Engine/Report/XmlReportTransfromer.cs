// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Recurity.CIR.Engine.Helper;

namespace Recurity.CIR.Engine.Report
{
    public class XmlReportTransfromer : IReportTransformer
    {
        private const string DETAILED = "detailed";
        private const string HTML_TEMPLATE = "report-{0}.html";
        private const string OUTPUT_XML = "report.xml";
        private const string SUMMARY = "summary";
        private const string XSLT_TEMPLATE = "report-{0}.xsl";
        /* 
         * these files need to be available in the output directory as they are referenced by the
         * generated html files.
         */
        private static readonly string[] LOCAL_RESOURCES = new string[] {"cir.css", "CIRlogo.png"};
        private readonly AssemblyResourceBundle bundle;

        public XmlReportTransfromer()
        {
            AssemblyResourceBundle bun = new AssemblyResourceBundle(GetType());
            bundle = bun.SubBundle("Recurity.CIR.Engine.Report.resources");
        }

        #region IReportTransformer Members

        public void Transform(DirectoryInfo targetDirectory, IReport report)
        {
            XPathDocument doc = CreateDocument(IOHelper.combine(targetDirectory.FullName, OUTPUT_XML), report);
            TransformXPathDocument(targetDirectory, doc);
        }

        #endregion

        private XPathDocument CreateDocument(string filename, IReport report)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                TextWriter writer = new StreamWriter(stream);
                report.WriteTo(writer);
            }
            return new XPathDocument(filename);
        }

        public void TransformXPathDocument(DirectoryInfo targetDirectory, XPathDocument xmlDoc)
        {
            TransformAllStylesheets(targetDirectory, xmlDoc);

            List<AssemblyResource> missingResources = GetMissingResources(targetDirectory, LOCAL_RESOURCES, bundle);
            CopyResources(targetDirectory, missingResources);
        }

        private void TransformAllStylesheets(DirectoryInfo targetDirectory, XPathDocument xmlDoc)
        {
            string detailedXslt = String.Format(XSLT_TEMPLATE, DETAILED);

            using (Stream details_stream = bundle[detailedXslt].InputStream)
            {
                XmlReader reader = XmlReader.Create(details_stream);
                TransformToHtml(reader,
                                IOHelper.combine(targetDirectory.FullName, String.Format(HTML_TEMPLATE, DETAILED)),
                                xmlDoc);
            }

            string summaryXslt = String.Format(XSLT_TEMPLATE, SUMMARY);
            using (Stream summary_stream = bundle[summaryXslt].InputStream)
            {
                XmlReader reader = XmlReader.Create(summary_stream);
                TransformToHtml(reader,
                                IOHelper.combine(targetDirectory.FullName, String.Format(HTML_TEMPLATE, SUMMARY)),
                                xmlDoc);
            }
        }

        private static void TransformToHtml(XmlReader reader, string htmlfile, XPathDocument xmlDoc)
        {
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(htmlfile, Encoding.UTF8))
            {
                XslCompiledTransform xsltTransform = new XslCompiledTransform();
                //load the Xsl 
                xsltTransform.Load(reader);

                XsltArgumentList xslArgs = new XsltArgumentList();
                xslArgs.AddExtensionObject("urn:CirFormatter", ReportFormatter.Instance);
                xsltTransform.Transform(xmlDoc, xslArgs, xmlTextWriter);

                xmlTextWriter.Close();
            }
        }


        private static List<AssemblyResource> GetMissingResources(DirectoryInfo target, String[] resources,
                                                                  AssemblyResourceBundle bundle)
        {
            List<AssemblyResource> retVal = new List<AssemblyResource>();
            foreach (String res in resources)
            {
                FileInfo resInfo = new FileInfo(IOHelper.combine(target.FullName, res));
                if (!resInfo.Exists)
                    retVal.Add(bundle[res]);
            }
            return retVal;
        }

        private static void CopyResources(DirectoryInfo target, List<AssemblyResource> res)
        {
            foreach (AssemblyResource resource in res)
            {
                resource.CopyTo(target);
            }
        }
    }
}