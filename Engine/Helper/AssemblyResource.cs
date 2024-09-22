// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Recurity.CIR.Engine.Helper
{
    public class AssemblyResource
    {
        private readonly string resourceName;
        private readonly Assembly assembly;
        private readonly string fullName;

        internal AssemblyResource(string name, Assembly asm):this(name,asm, name)
        {
            
        }
        internal AssemblyResource(string name, Assembly asm, string fullname)
        {
            this.resourceName = name;
            this.assembly = asm;
            this.fullName = fullname;

        }

        public string FullName
        {
            get { return this.fullName; }
        }

        public string Name
        {
            get { return this.resourceName; } 
        }

        public Assembly Assembly
        {
            get { return this.assembly; }
        }

        public void CopyTo(DirectoryInfo info)
        {
            CopyTo(info, this.Name);
        }

        public void CopyTo(DirectoryInfo info, string name){
            if (name == null)
                throw new ArgumentNullException("Can not copy -- filename is null");
            if (info == null)
                throw new ArgumentNullException("Can not copy -- directory info is null");
            uint buffersize = 512;
            using (Stream inStream = new BufferedStream(this.InputStream, (int)buffersize))
            {
                using (Stream outStream = new BufferedStream(new FileStream(IOHelper.combine(info.FullName, name), FileMode.Create, FileAccess.Write), (int)buffersize))
                {
                    byte[] buffer = new byte[buffersize];
                    uint read = 0;
                    while ((read = (uint)inStream.Read(buffer, 0, (int)buffersize)) > 0)
                    {
                        outStream.Write(buffer, 0,(int) read);
                        
                    }
                    outStream.Flush();
                }
            }

        }

        public Stream InputStream
        {
            get
            {
                return this.assembly.GetManifestResourceStream(this.FullName);
            }
        }

    }
}
