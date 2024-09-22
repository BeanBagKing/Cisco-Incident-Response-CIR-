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
using System.Xml.Serialization;

namespace Recurity.CIR.Engine.Helper
{
    public class XMLEntitySerializer
    {

        public static T Deserialize<T>(FileInfo info)
        {
            if (info == null) throw new ArgumentNullException("info");
            if(!info.Exists)
                throw new ArgumentException(String.Format("file {0} does not exist", info));
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using(Stream stream = info.OpenRead())
            {
                
                return (T) serializer.Deserialize(XmlReader.Create(stream));
            }
            
        }

        public static void Serialize(object entity, FileInfo to)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (to == null) throw new ArgumentNullException("to");
            XmlSerializer serializer = new XmlSerializer(entity.GetType());
            using (Stream stream = to.OpenWrite())
            {
                serializer.Serialize(XmlWriter.Create(stream), entity);
            }
        }
    }
}
