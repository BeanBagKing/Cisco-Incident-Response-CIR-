// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Configuration;

namespace Recurity.CIR.Engine.Configuration.Section
{
    public class CirPlatform : ConfigurationElement
    {
        [ConfigurationProperty("assembly", IsRequired = true, IsKey = true)]
        public String Assembly
        {
            get { return (String) this["assembly"]; }
            set { this["assembly"] = value; }
        }
        [ConfigurationProperty("path", IsRequired = true, IsKey = false)]
        public String Path
        {
            get { return ((String)this["path"]); }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = false, IsKey = false)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

    
    
    }
}
