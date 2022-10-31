// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.System.Com;

namespace System.Windows.Forms
{
    internal class DataStreamFromComStream : Stream
    {
        private IStream.Interface _comStream;

        public DataStreamFromComStream(IStream.Interface comStream) : base()
        {
            _comStream = comStream;
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
                long curPos = Position;
                long endPos = Seek(0, SeekOrigin.End);
                Position = curPos;
                return endPos - curPos;
            }
        }

        public override void Flush()
        {
        }

        /// <summary>
        ///  Read the data into the given buffer
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
        ///  Read the data into the given buffer
        /// </summary>
        /// <param name="buffer">The buffer receiving the data</param>
        /// <returns>The number of bytes read</returns>
        public override unsafe int Read(Span<byte> buffer)
        {
            uint bytesRead = 0;
            if (!buffer.IsEmpty)
            {
                fixed (byte* ch = &buffer[0])
                {
                    _comStream.Read(ch, (uint)buffer.Length, &bytesRead);
                }
            }

            return (int)bytesRead;
        }

        public override void SetLength(long value)
        {
            _comStream.SetSize((ulong)value);
        }

        public override unsafe long Seek(long offset, SeekOrigin origin)
        {
            ulong newPosition = 0;
            _comStream.Seek(offset, origin, &newPosition);
            return (long)newPosition;
        }

        /// <summary>
        ///  Writes the data contained in the given buffer
        /// </summary>
        /// <param name="buffer">The buffer containing the data to write</param>
        /// <param name="index">The offset from the beginning of the buffer</param>
        /// <param name="count">The number of bytes to write</param>
        public override void Write(byte[] buffer, int index, int count)
        {
            if (count <= 0)
            {
                return;
            }

            if (count > 0 && index >= 0 && (count + index) <= buffer.Length)
            {
                var span = new ReadOnlySpan<byte>(buffer, index, count);
                Write(span);
                return;
            }

            throw new IOException(SR.DataStreamWrite);
        }

        /// <summary>
        ///  Writes the data contained in the given buffer
        /// </summary>
        /// <param name="buffer">The buffer to write</param>
        public override unsafe void Write(ReadOnlySpan<byte> buffer)
        {
            if (buffer.IsEmpty)
            {
                return;
            }

            uint bytesWritten = 0;
            try
            {
                fixed (byte* b = &buffer[0])
                {
                    _comStream.Write(b, (uint)buffer.Length, &bytesWritten);
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
                if (disposing && _comStream is not null)
                {
                    _comStream.Commit(STGC.STGC_DEFAULT);
                }

                // Can't release a COM stream from the finalizer thread.
                _comStream = null!;
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
