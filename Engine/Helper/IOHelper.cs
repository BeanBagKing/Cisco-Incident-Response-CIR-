// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.Text;

namespace Recurity.CIR.Engine.Helper
{
    public static class IOHelper
    {
        public static String combine(string directory, string file)
        {
            if (file == null)
                throw new ArgumentNullException("File must not be null");
            if (directory == null)
                throw new ArgumentNullException("directory must not be null");
            return (directory.EndsWith(@"\") || directory.EndsWith("/") ? directory : directory + @"\") + file;
           
        }
    }
}
