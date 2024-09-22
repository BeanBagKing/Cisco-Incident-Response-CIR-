// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Recurity.CIR.Engine.Helper
{
    public class AssemblyResourceBundle
    {
        private readonly List<AssemblyResource> resources;
        private readonly Assembly assembly;
        
       
        private readonly Dictionary<String, AssemblyResource> dict = new Dictionary<string, AssemblyResource>();
        
        public AssemblyResourceBundle(Type type)
        {

            Assembly asm = Assembly.GetAssembly(type);
            string[] names = asm.GetManifestResourceNames();
            List<AssemblyResource> asmResources = new List<AssemblyResource>();
            foreach (string name in names)
            {
                asmResources.Add(new AssemblyResource(name, asm));
            }
            this.resources = asmResources;
            this.assembly = asm;
         
            BuildDictionary();
        }

        private AssemblyResourceBundle(List<AssemblyResource> resources, Assembly asm)
        {
            this.resources = resources;
            this.assembly = asm;
            BuildDictionary();
        }

        private void BuildDictionary()
        {
            foreach (AssemblyResource res in this.resources)
            {
               
                this.dict[res.Name] = res;
                          
              
            }
        }

        public AssemblyResource this[string index]
        {
            
            get { return this.dict[index]; }
        }

        List<AssemblyResource> Resources
        {
            get
            {
                return this.resources; 
            }
        }

        public AssemblyResourceBundle SubBundle(string subBundle)
        {
            string subbundle = subBundle + ".";
            List<AssemblyResource> newRes = new List<AssemblyResource>();
            foreach(AssemblyResource res in this.resources){
                if(res.FullName.Length > subbundle.Length && res.FullName.StartsWith(subbundle)){
                    
                    string name = res.FullName.Substring(subbundle.Length);
                    if(name.Length > 0)
                        newRes.Add(new AssemblyResource(name, this.assembly, res.FullName));

                }
            }
            if(newRes.Count == 0)
                throw new ArgumentException("no such subbundle");
            return new AssemblyResourceBundle(newRes, this.assembly);
        }

        
    }
}
