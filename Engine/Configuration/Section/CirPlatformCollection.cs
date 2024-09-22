// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Configuration;

namespace Recurity.CIR.Engine.Configuration.Section
{
    public class CirPlatformCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CirPlatform();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
           if (!(element is CirPlatform))
               throw new ArgumentException("element must be an instance of CirPlugin");
            CirPlatform plugin = element as CirPlatform;
            return plugin.Assembly;
           
        }

        public CirPlatform this[int index]
        {
            get
            {
                return (CirPlatform)BaseGet(index);
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

        new public CirPlatform this[string Name]
        {
            get
            {
                return (CirPlatform)BaseGet(Name);
            }
        }
    }
   
}
