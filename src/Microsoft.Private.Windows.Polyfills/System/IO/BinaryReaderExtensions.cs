// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;

namespace System.IO;

internal static partial class BinaryReaderExtensions
{
    extension(BinaryReader reader)
    {
        /// <summary>
        ///  Reads a sequence of bytes from the current stream and advances the position within the stream
        ///  by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        ///  A region of memory. When this method returns, the contents of this region are replaced by the
        ///  bytes read from the current stream.
        /// </param>
        /// <returns>
        ///  The total number of bytes read into the buffer. This can be less than the size of the buffer if
        ///  that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        public int Read(Span<byte> buffer)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                int numRead = reader.Read(sharedBuffer, 0, buffer.Length);
                new ReadOnlySpan<byte>(sharedBuffer, 0, numRead).CopyTo(buffer);
                return numRead;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(sharedBuffer);
            }
        }
    }
}
