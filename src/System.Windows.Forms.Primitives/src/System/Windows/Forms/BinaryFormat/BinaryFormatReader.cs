// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Reader that reads specific types in binary format without using the BinaryFormatter.
/// </summary>
internal static class BinaryFormatReader
{
    /// <summary>
    ///  Reads a binary formatted string.
    /// </summary>
    /// <exception cref="SerializationException">Data isn't a valid string.</exception>
    public static string ReadString(Stream stream)
    {
        BinaryFormattedObject format = new(stream, leaveOpen: true);
        if (format.RecordCount < 3 || format[1] is not BinaryObjectString value)
        {
            throw new SerializationException();
        }

        return value.Value;
    }

    /// <summary>
    ///  Reads a binary formatted primitive list.
    /// </summary>
    /// <exception cref="NotSupportedException"><typeparamref name="T"/> was not primitive.</exception>
    /// <exception cref="SerializationException">Data isn't a valid <see cref="List{T}"/>.</exception>
    public static List<T> ReadPrimitiveList<T>(Stream stream)
        where T : unmanaged
    {
        if (!typeof(T).IsPrimitive)
        {
            throw new NotSupportedException($"{nameof(T)} is not primitive.");
        }

        BinaryFormattedObject format = new(stream, leaveOpen: true);
        if (format.RecordCount < 4
            || format[1] is not SystemClassWithMembersAndTypes classInfo
            || !classInfo.Name.StartsWith($"System.Collections.Generic.List`1[[{typeof(T).FullName}")
            || format[2] is not ArraySinglePrimitive array
            || array.PrimitiveType != Record.GetPrimitiveType(typeof(T)))
        {
            throw new SerializationException();
        }

        int count;
        try
        {
            count = (int)classInfo["_size"];
        }
        catch (Exception ex)
        {
            throw ex.ConvertToSerializationException();
        }

        List<T> list = new(count);
        list.AddRange(array.Take(count).Cast<T>());
        return list;
    }

    /// <summary>
    ///  Reads a binary formatted <see cref="Hashtable"/>. Only accepts <see langword="string"/> keys
    ///  and <see langword="string"/>? values.
    /// </summary>
    /// <exception cref="SerializationException">
    ///  Data isn't a valid <see langword="string"/> - <see langword="string"/>? <see cref="Hashtable"/>.
    /// </exception>
    public static Hashtable ReadHashtableOfStrings(Stream stream)
    {
        BinaryFormattedObject format = new(stream, leaveOpen: true);
        if (format.RecordCount < 5
            || format[1] is not SystemClassWithMembersAndTypes classInfo
            || classInfo.Name != "System.Collections.Hashtable"
            || format[2] is not ArraySingleObject keys
            || format[3] is not ArraySingleObject values
            || keys.Length != values.Length)
        {
            throw new SerializationException();
        }

        Hashtable hashtable = new(keys.Length);
        for (int i = 0; i < keys.Length; i++)
        {
            string key = keys[i] switch
            {
                BinaryObjectString keyString => keyString.Value,
                MemberReference reference => format[reference.IdRef] switch
                {
                    BinaryObjectString refString => refString.Value,
                    _ => throw new SerializationException()
                },
                _ => throw new SerializationException()
            };

            hashtable[key] = values[i] switch
            {
                BinaryObjectString valueString => valueString.Value,
                ObjectNull => null,
                MemberReference reference => format[reference.IdRef] switch
                {
                    BinaryObjectString refString => refString.Value,
                    _ => throw new SerializationException()
                },
                _ => throw new SerializationException(),
            };
        }

        return hashtable;
    }

    /// <summary>
    ///  Creates a <see cref="DateTime"/> object from raw data with validation.
    /// </summary>
    /// <exception cref="SerializationException"><paramref name="data"/> was invalid.</exception>
    internal static DateTime CreateDateTimeFromData(long data)
    {
        // Copied from System.Runtime.Serialization.Formatters.Binary.BinaryParser

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

        return Unsafe.As<long, DateTime>(ref data);
    }
}
