// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.IO;

namespace Recurity.CIR.Engine.Interfaces
{
    /// <summary>
    /// This interface is an abstract representation of a file.
    /// Any class which represents or acts like a file in the 
    /// CIR application should inherit this interface no matter if the file is 
    /// a physical file on the harddisk or just a temporary memory-mapped file.
    /// </summary>
    public interface FileRepresentation : IDisposable
    {
        FileInfo Info { get; }
        string Name { get; }
        Stream Stream(FileMode mode, FileAccess access);
        Stream Stream(FileMode mode);
    }

    /* 
     * Note: An abstract file representation it the basic requirement for unit 
     * testing as we need to mock out the file operations. 
     */
}