// 
// Copyright (c) 2008, Recurity Labs GmbH.
// All rights reserved.
// 
// Author: Felix Lindner (fx@recurtiy-labs.com)

using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using Recurity.CIR.Engine.Interfaces;

namespace Recurity.CIR.Engine.PluginResults
{
    /// <summary>
    /// This class is the default implementation of the FileRepresentation interface.
    /// Each class in the CIR project which represents a file is supposed to inherit this
    /// class to ensure that all streams opened on the file are closed e.g. released.
    /// This class tracks all streams opened by one of the Stream methods. Each stream which was not
    /// closed before the <code>Dispose</code> method was called will be closed by the finalizer of
    /// this class. Additionally this class will write a message to the logging API which indicates 
    /// where the pending stream was opened. To see this stack trace you have to enable the DEBUG level of the
    /// Logging API.
    /// 
    /// </summary>
    public abstract class SomeFile : FileRepresentation
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (SomeFile));
        private readonly FileInfo fileinfo;

        private readonly List<KeyValuePair<StreamDecorator, string>> released_streams =
            new List<KeyValuePair<StreamDecorator, string>>();

        public SomeFile(FileInfo a_fileinfo)
        {
            if (a_fileinfo == null) throw new ArgumentNullException("a_fileinfo");
            if (!a_fileinfo.Exists)
                throw new IOException(String.Format("File {0} not found.", a_fileinfo));
            fileinfo = a_fileinfo;
        }

        #region FileRepresentation Members

        public FileInfo Info
        {
            get { return fileinfo; }
        }


        public String Name
        {
            get { return fileinfo.FullName; }
        }


        /// <summary>
        /// Closes all pending streams released by this class.
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<StreamDecorator, String> pair in released_streams)
            {
                if (!pair.Key.Closed)
                {
#if DEBUG
                    LOG.Warn("Close pending stream -- stream should be disposed in the owner class");
#endif
                    if (LOG.IsDebugEnabled)
                    {
                        LOG.DebugFormat("Stream was opened at {0}", pair.Value);
                    }
                    pair.Key.Close();
                }
            }
        }


        public Stream Stream(FileMode mode, FileAccess access)
        {
            StreamDecorator stream = new StreamDecorator(fileinfo.Open(mode, access, FileShare.Read));
            ReleasedStream(stream);
            return stream;
        }

        public Stream Stream(FileMode mode)
        {
            StreamDecorator stream = new StreamDecorator(fileinfo.Open(mode, FileAccess.ReadWrite, FileShare.Read));
            ReleasedStream(stream);
            return stream;
        }

        #endregion

        private void ReleasedStream(StreamDecorator stream)
        {
            released_streams.Add(new KeyValuePair<StreamDecorator, string>(stream, Environment.StackTrace));
        }
    }

    /// <summary>
    /// Simple Stream decorator which adds one handy features
    /// to the Stream Interface, it tells you if the stream has been closed already.
    /// </summary>
    internal class StreamDecorator : Stream
    {
        private readonly Stream stream;
        private volatile bool closed = false;

        internal StreamDecorator(Stream a_stream)
        {
            if (a_stream == null) throw new ArgumentNullException("a_stream");

            stream = a_stream;
        }

        public override bool CanRead
        {
            get { return stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return stream.CanWrite; }
        }

        public override long Length
        {
            get { return stream.Length; }
        }

        public override long Position
        {
            get { return stream.Position; }
            set { stream.Position = value; }
        }

        public bool Closed
        {
            get { return closed; }
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }

        public override void Close()
        {
            stream.Close();
            closed = true;
        }
    }
}