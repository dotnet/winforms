// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Text;

namespace System.IO;

internal static class BinaryReaderExtensions
{
    /// <summary>
    ///  Reads a binary formatted <see cref="DateTime"/> from the given <paramref name="reader"/>.
    /// </summary>
    /// <exception cref="SerializationException">The data was invalid.</exception>
    public static unsafe DateTime ReadDateTime(this BinaryReader reader)
    {
        // Copied from System.Runtime.Serialization.Formatters.Binary.BinaryParser

        long data = reader.ReadInt64();

        // Use DateTime's public constructor to validate the input, but we
        // can't return that result as it strips off the kind. To address
        // that, store the value directly into a DateTime via an unsafe cast.
        // See BinaryFormatterWriter.WriteDateTime for details.

        try
        {
            const long TicksMask = 0x3FFFFFFFFFFFFFFF;
            _ = new DateTime(data & TicksMask);
        }
        catch (ArgumentException ex)
        {
            // Bad data
            throw new SerializationException(ex.Message, ex);
        }

        return *(DateTime*)&data;
    }

    /// <summary>
    ///  Reads a UTF-8 string from the given <paramref name="reader"/> prefixed with a 7 bit encoded length.
    /// </summary>
    public static string ReadLengthPrefixedString(this BinaryReader reader)
        => reader.ReadUtf8String(reader.Read7BitEncodedInt());

    /// <summary>
    ///  Reads a UTF-8 string of byte length <paramref name="lengthInBytes"/> from the given <paramref name="reader"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="lengthInBytes"/> is negative.</exception>
    private static unsafe string ReadUtf8String(this BinaryReader reader, int lengthInBytes)
    {
        if (lengthInBytes == 0)
        {
            return string.Empty;
        }

        if (lengthInBytes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lengthInBytes));
        }

        if (reader.Remaining() < lengthInBytes)
        {
            throw new EndOfStreamException();
        }

        using BufferScope<byte> buffer = new(stackalloc byte[256], lengthInBytes);
        Span<byte> bytes = buffer[..lengthInBytes];
        return reader.Read(bytes) != lengthInBytes
            ? throw new EndOfStreamException()
            : Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    ///  Returns the remaining amount of bytes in the given <paramref name="reader"/>.
    /// </summary>
    public static long Remaining(this BinaryReader reader)
    {
        Stream stream = reader.BaseStream;
        return stream.Length - stream.Position;
    }
}
