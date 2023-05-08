// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Windows.Forms.BinaryFormat;

namespace System.IO;

internal static class BinaryReaderExtensions
{
    /// <summary>
    ///  Reads a binary formatted <see cref="DateTime"/> from the given <paramref name="reader"/>.
    /// </summary>
    /// <exception cref="SerializationException">The data was invalid.</exception>
    public static unsafe DateTime ReadDateTime(this BinaryReader reader)
        => BinaryFormatReader.CreateDateTimeFromData(reader.ReadInt64());

    /// <summary>
    ///  Returns the remaining amount of bytes in the given <paramref name="reader"/>.
    /// </summary>
    public static long Remaining(this BinaryReader reader)
    {
        Stream stream = reader.BaseStream;
        return stream.Length - stream.Position;
    }
}
