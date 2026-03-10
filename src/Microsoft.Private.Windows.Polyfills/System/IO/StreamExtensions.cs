// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using Microsoft.Private.Windows.Polyfills.Resources;

namespace System.IO;

internal static partial class StreamExtensions
{
    // The implementations here were lifted from the runtime repository. If there were XML docs on the original APIs,
    // they were also lifted. (Minor style changes to fit WinForms requirements were made.)

    extension(Stream stream)
    {
        /// <summary>
        ///  Reads a sequence of bytes from the current stream and advances the position within the stream
        ///  by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        ///  A region of memory. When this method returns, the contents of this region are replaced
        ///  by the bytes read from the current stream.
        /// </param>
        /// <returns>
        ///  The total number of bytes read into the buffer. This can be less than the size of the buffer if that many
        ///  bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="IOException">The number of bytes read from the stream exceeds the buffer length.</exception>
        public int Read(Span<byte> buffer)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                int numRead = stream.Read(sharedBuffer, 0, buffer.Length);
                if ((uint)numRead > (uint)buffer.Length)
                {
                    throw new IOException(SRF.IO_StreamTooLong);
                }

                new ReadOnlySpan<byte>(sharedBuffer, 0, numRead).CopyTo(buffer);
                return numRead;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        /// <summary>
        ///  Writes a sequence of bytes to the current stream and advances the current position within this stream
        ///  by the number of bytes written.
        /// </summary>
        /// <param name="buffer">A region of memory. This method copies the contents of this region to the current stream.</param>
        public void Write(ReadOnlySpan<byte> buffer)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                buffer.CopyTo(sharedBuffer);
                stream.Write(sharedBuffer, 0, buffer.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }

        /// <summary>
        ///  Reads bytes from the current stream and advances the position within the stream until the <paramref name="buffer"/> is filled.
        /// </summary>
        /// <param name="buffer">A region of memory. When this method returns, the contents of this region are replaced by the bytes read from the current stream.</param>
        /// <exception cref="EndOfStreamException">
        ///  The end of the stream is reached before filling the <paramref name="buffer"/>.
        /// </exception>
        /// <remarks>
        ///  <para>
        ///   When <paramref name="buffer"/> is empty, this read operation will be completed without waiting for available data in the stream.
        ///  </para>
        /// </remarks>
        public void ReadExactly(Span<byte> buffer)
        {
            // Collapsed the helper method as the analyzer couldn't see that it was actually being used.

            int totalRead = 0;
            while (totalRead < buffer.Length)
            {
                int read = stream.Read(buffer[totalRead..]);
                if (read == 0)
                {
                    throw new EndOfStreamException();
                }

                totalRead += read;
            }
        }
    }
}
