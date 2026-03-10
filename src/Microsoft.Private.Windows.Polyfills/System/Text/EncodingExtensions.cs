// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Text;

internal static class EncodingExtensions
{
    extension(Encoding encoding)
    {
        /// <summary>
        ///  Encodes into a span of bytes a set of characters from the specified read-only span.
        /// </summary>
        /// <param name="source">The span containing the set of characters to encode.</param>
        /// <param name="destination">The byte span to hold the encoded bytes.</param>
        /// <returns>The number of encoded bytes.</returns>
        public unsafe int GetBytes(ReadOnlySpan<char> source, Span<byte> destination)
        {
            if (source.IsEmpty)
            {
                return 0;
            }

            fixed (char* sourcePointer = source)
            fixed (byte* destinationPointer = destination)
            {
                return encoding.GetBytes(sourcePointer, source.Length, destinationPointer, destination.Length);
            }
        }

        /// <summary>
        ///  Decodes all the bytes in the specified byte span into a string.
        /// </summary>
        /// <param name="source">A read-only byte span to decode to a string.</param>
        /// <returns>A string that contains the decoded bytes from the provided read-only span.</returns>
        public unsafe string GetString(ReadOnlySpan<byte> source)
        {
            if (source.IsEmpty)
            {
                return string.Empty;
            }

            fixed (byte* sourcePointer = source)
            {
                return encoding.GetString(sourcePointer, source.Length);
            }
        }
    }
}
