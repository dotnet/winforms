// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Text;

namespace System.IO;

internal static class BinaryWriterExtensions
{
    /// <summary>
    ///  Writes a UTF-8 string prefixed by a 7-bit encoded length. Does not include a trailing null.
    /// </summary>
    public static void WriteLengthPrefixedString(this BinaryWriter writer, string value)
    {
        Encoding encoding = Encoding.UTF8;
        using BufferScope<byte> buffer = new(stackalloc byte[256], encoding.GetMaxByteCount(value.Length));
        int length = encoding.GetBytes(value, buffer);
        writer.Write7BitEncodedInt(length);
        writer.Write(buffer[..length]);
    }

    /// <summary>
    ///  Writes a <see cref="DateTime"/> object to the given <paramref name="writer"/>.
    /// </summary>
    public static void Write(this BinaryWriter writer, DateTime value)
    {
        // Copied from System.Runtime.Serialization.Formatters.Binary.BinaryFormatterWriter

        // In .NET Framework, BinaryFormatter is able to access DateTime's ToBinaryRaw,
        // which just returns the value of its sole Int64 dateData field.  Here, we don't
        // have access to that member (which doesn't even exist anymore, since it was only for
        // BinaryFormatter, which is now in a separate assembly).  To address that,
        // we access the sole field directly via an unsafe cast.
        long dateData = Unsafe.As<DateTime, long>(ref value);
        writer.Write(dateData);
    }
}
