// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Runtime.InteropServices;

    using System.Diagnostics;
    using System;
    using System.IO;


    /// <internalonly/>
    /// <devdoc>
    /// </devdoc>
    internal class DataStreamFromComStream : Stream
    {

        private UnsafeNativeMethods.IStream comStream;

        public DataStreamFromComStream(UnsafeNativeMethods.IStream comStream) : base()
        {
            this.comStream = comStream;
        }

        public override long Position
        {
            get
            {
                return Seek(0, SeekOrigin.Current);
            }

            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                long curPos = this.Position;
                long endPos = Seek(0, SeekOrigin.End);
                this.Position = curPos;
                return endPos - curPos;
            }
        }

        /*
        private void _NotImpl(string message) {
            NotSupportedException ex = new NotSupportedException(message, new ExternalException(SR.ExternalException, NativeMethods.E_NOTIMPL));
            throw ex;
        }
        */

        private unsafe int _Read(void* handle, int bytes)
        {
            return comStream.Read((IntPtr)handle, bytes);
        }

        private unsafe int _Write(void* handle, int bytes)
        {
            return comStream.Write((IntPtr)handle, bytes);
        }

        public override void Flush()
        {
        }

        /// <summary>
        /// Read the data into the given buffer
        /// </summary>
        /// <param name="buffer">The buffer receiving the data</param>
        /// <param name="index">The offset from the beginning of the buffer</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The number of bytes read</returns>
        public override int Read(byte[] buffer, int index, int count)
        {
            int bytesRead = 0;
            if (count > 0 && index >= 0 && (count + index) <= buffer.Length)
            {
                var span = new Span<byte>(buffer, index, count);
                bytesRead = Read(span);
            }
            return bytesRead;
        }

        /// <summary>
        /// Read the data into the given buffer
        /// </summary>
        /// <param name="buffer">The buffer receiving the data</param>
        /// <returns>The number of bytes read</returns>
        public unsafe override int Read(Span<byte> buffer)
        {
            int bytesRead = 0;
            if (!buffer.IsEmpty)
            {
                fixed (byte* ch = &buffer[0])
                {
                    bytesRead = _Read(ch, buffer.Length);
                }
            }
            return bytesRead;
        }

        public override void SetLength(long value)
        {
            comStream.SetSize(value);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return comStream.Seek(offset, (int)origin);
        }

        /// <summary>
        /// Writes the data contained in the given buffer
        /// </summary>
        /// <param name="buffer">The buffer containing the data to write</param>
        /// <param name="index">The offset from the beginning of the buffer</param>
        /// <param name="count">The number of bytes to write</param>
        public override void Write(byte[] buffer, int index, int count)
        {
            if (count <= 0)
                return;

            if (count > 0 && index >= 0 && (count + index) <= buffer.Length)
            {
                var span = new ReadOnlySpan<byte>(buffer, index, count);
                Write(span);
                return;
            }

            throw new IOException(SR.DataStreamWrite);
        }


        /// <summary>
        /// Writes the data contained in the given buffer
        /// </summary>
        /// <param name="buffer">The buffer to write</param>
        public unsafe override void Write(ReadOnlySpan<byte> buffer)
        {
            if (buffer.IsEmpty)
                return;

            int bytesWritten = 0;
            try
            {
                fixed (byte* b = &buffer[0])
                {
                    bytesWritten = _Write(b, buffer.Length);
                }
            }
            catch
            {
            }
            if (bytesWritten < buffer.Length)
            {
                throw new IOException(SR.DataStreamWrite);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && comStream != null)
                {
                    try
                    {
                        comStream.Commit(NativeMethods.STGC_DEFAULT);
                    }
                    catch (Exception)
                    {
                    }
                }
                // Can't release a COM stream from the finalizer thread.
                comStream = null;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        ~DataStreamFromComStream()
        {
            Dispose(false);
        }
    }
}
