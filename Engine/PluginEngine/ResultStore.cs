// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections;
using System.Collections.Generic;
using PluginHost;

namespace Recurity.CIR.Engine.PluginEngine
{
    public class ResultStore
    {
        protected List<IPluginResult> _results;

        public ResultStore()
        {
            _results = new List<IPluginResult>();
        }

        public void Add(IPluginResult result) 
        {
            _results.Add( result );
        }

        public List<IPluginResult> GetResults()
        {
            return _results;
        }

        public List<T> GetResults<T>()
        {
            List<T> ret = new List<T>();

            foreach (IPluginResult o in _results)
            {
                if (typeof(T).IsAssignableFrom(o.GetType()))
                {
                    ret.Add((T)o);
                }
            }

            return ret;
        }

        public List<IPluginResult> GetResults(Type t)
        {
            List<IPluginResult> ret = new List<IPluginResult>();
            
            foreach (IPluginResult o in _results)
            {
                if (t.IsAssignableFrom(o.GetType()))
                {
                    
                    ret.Add(o);
                }
            }

            return ret;

        }
    }
}
