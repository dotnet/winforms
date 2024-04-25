// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO;

internal static class BinaryReaderExtensions
{
    /// <summary>
    ///  Reads a binary formatted <see cref="DateTime"/> from the given <paramref name="reader"/>.
    /// </summary>
    /// <exception cref="SerializationException">The data was invalid.</exception>
    public static unsafe DateTime ReadDateTime(this BinaryReader reader)
        => CreateDateTimeFromData(reader.ReadInt64());

    /// <summary>
    ///  Creates a <see cref="DateTime"/> object from raw data with validation.
    /// </summary>
    /// <exception cref="SerializationException"><paramref name="data"/> was invalid.</exception>
    private static DateTime CreateDateTimeFromData(long data)
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

    /// <summary>
    ///  Returns the remaining amount of bytes in the given <paramref name="reader"/>.
    /// </summary>
    public static long Remaining(this BinaryReader reader)
    {
        Stream stream = reader.BaseStream;
        return stream.Length - stream.Position;
    }

    /// <summary>
    ///  Reads an array of primitives.
    /// </summary>
    public static unsafe T[] ReadPrimitiveArray<T>(this BinaryReader reader, int count)
        where T : unmanaged
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (typeof(T) != typeof(bool)
            && typeof(T) != typeof(byte)
            && typeof(T) != typeof(sbyte)
            && typeof(T) != typeof(char)
            && typeof(T) != typeof(short)
            && typeof(T) != typeof(ushort)
            && typeof(T) != typeof(int)
            && typeof(T) != typeof(uint)
            && typeof(T) != typeof(long)
            && typeof(T) != typeof(ulong)
            && typeof(T) != typeof(float)
            && typeof(T) != typeof(double))
        {
            throw new ArgumentException($"Cannot read primitives of {typeof(T).Name}.", nameof(T));
        }

        if (count > 0 && reader.Remaining() < count * (typeof(T) == typeof(char) ? 1 : sizeof(T)))
        {
            throw new SerializationException("Not enough data to fill array.");
        }

        if (count == 0)
        {
            return [];
        }

        if (typeof(T) == typeof(char))
        {
            // Need to handle different encodings
            return (T[])(object)reader.ReadChars(count);
        }

        T[] array = new T[count];

        fixed (T* a = array)
        {
            Span<byte> arrayData = new(a, array.Length * sizeof(T));

            if (reader.Read(arrayData) != arrayData.Length)
            {
                throw new SerializationException("Not enough data to fill array.");
            }

            if (sizeof(T) != 1 && !BitConverter.IsLittleEndian)
            {
                if (sizeof(T) == 2)
                {
                    Span<ushort> ushorts = MemoryMarshal.Cast<byte, ushort>(arrayData);
                    BinaryPrimitives.ReverseEndianness(ushorts, ushorts);
                }
                else if (sizeof(T) == 4)
                {
                    Span<int> ints = MemoryMarshal.Cast<byte, int>(arrayData);
                    BinaryPrimitives.ReverseEndianness(ints, ints);
                }
                else if (sizeof(T) == 8)
                {
                    Span<long> longs = MemoryMarshal.Cast<byte, long>(arrayData);
                    BinaryPrimitives.ReverseEndianness(longs, longs);
                }
                else
                {
                    throw new InvalidOperationException($"Cannot read primitives of {typeof(T).Name}.");
                }
            }
        }

        return array;
    }
}
