// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Configuration;

namespace Recurity.CIR.Engine.Configuration.Section
{
    public class CirPluginCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CirPlugin();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
           if (!(element is CirPlugin))
               throw new ArgumentException("element must be an instance of CirPlugin");
            CirPlugin plugin = element as CirPlugin;
            return plugin.Assembly;
           
        }

        public CirPlugin this[int index]
        {
            get
            {
                return (CirPlugin)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public CirPlugin this[string Name]
        {
            get
            {
                return (CirPlugin)BaseGet(Name);
            }
        }
    }
}
