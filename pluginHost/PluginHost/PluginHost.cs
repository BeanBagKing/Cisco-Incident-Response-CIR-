//
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
//
// Author: Simon Willnauer (simonw@recurtiy-labs.com)
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using PluginHost.PluginValidation;

namespace PluginHost
{
    public class PluginHost<T> where T : class, IPlugin
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (PluginHost<T>));

        protected readonly List<T> _plugins = new List<T>();
        private readonly PluginValidator validator;

        public PluginHost(PluginValidator theValidator)
        {
            if (theValidator == null) throw new ArgumentNullException("theValidator");
            validator = theValidator;
        }

        public PluginHost(string path, PluginValidator theValidator)
        {
            if (theValidator == null) throw new ArgumentNullException("theValidator");
            validator = theValidator;
            Load(path);
        }

        public List<T> Plugins
        {
            get { return _plugins; }
        }

        public void Add(string path)
        {
            Load(path);
        }

        protected void Load(string file)
        {
            bool found = false;

            Assembly asm = Assembly.LoadFrom(file);

            if (Validate(asm))
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (!type.IsClass || !type.IsPublic)
                        continue;

                    Type[] pluginIfs = type.GetInterfaces();
                    if (((IList) pluginIfs).Contains(typeof (T)))
                    {
                        object obj = Activator.CreateInstance(type);
                        T t = (T) obj;
                        _plugins.Add(t);
                        found = true;
                    }
                }

                if (!found)
                    throw new FormatException(file + " does not implement " + typeof (T).ToString());
            }
        }


        private bool Validate(Assembly asm)
        {
            if (!validator.ValidateVersion(asm))
            {
                LOG.WarnFormat("Assembly Version mismatch -- can not load {0}", asm.FullName);
                return false;
            }
            if (!validator.Validate(asm))
            {
                LOG.WarnFormat("Incompatible Assembly -- can not load {0}", asm.FullName);
                return false;
            }
            return true;
        }

        public void Add(T plugin)
        {
            if (plugin == null) throw new ArgumentNullException("plugin");

            if (Validate(plugin.GetType().Assembly))
                _plugins.Add(plugin);
        }
    }
}