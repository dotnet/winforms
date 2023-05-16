﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Writer that writes specific types in binary format without using the BinaryFormatter.
/// </summary>
internal static class BinaryFormatWriter
{
    private static string[]? s_hashtableMemberNames;
    private static string[] HashtableMemberNames => s_hashtableMemberNames ??= new[]
    {
        "LoadFactor", "Version", "Comparer", "HashCodeProvider", "HashSize", "Keys", "Values"
    };

    private static string[]? s_listMemberNames;
    private static string[] ListMemberNames => s_listMemberNames ??= new[] { "_items", "_size", "_version" };

    private static string[]? s_decimalMemberNames;
    private static string[] DecimalMemberNames => s_decimalMemberNames ??= new[] { "flags", "hi", "lo", "mid" };

    private static readonly string[] s_dateTimeMemberNames = new[] { "ticks", "dateData" };
    private static readonly string[] s_primitiveMemberName = new[] { "m_value" };

    private const string Mscorlib = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

    /// <summary>
    ///  Writes a <see langword="string"/> in binary format.
    /// </summary>
    public static void WriteString(Stream stream, string value)
    {
        using var writer = new BinaryFormatWriterScope(stream);
        new BinaryObjectString(1, value).Write(writer);
    }

    /// <summary>
    ///  Writes a <see langword="decimal"/> in binary format.
    /// </summary>
    public static void WriteDecimal(Stream stream, decimal value)
    {
        Span<int> ints = stackalloc int[4];
        decimal.TryGetBits(value, ints, out _);

        using var writer = new BinaryFormatWriterScope(stream);

        new SystemClassWithMembersAndTypes(
            new ClassInfo(1, typeof(decimal).FullName!, DecimalMemberNames),
            new MemberTypeInfo(
                (BinaryType.Primitive, PrimitiveType.Int32),
                (BinaryType.Primitive, PrimitiveType.Int32),
                (BinaryType.Primitive, PrimitiveType.Int32),
                (BinaryType.Primitive, PrimitiveType.Int32)),
            ints[3],
            ints[2],
            ints[0],
            ints[1]).Write(writer);
    }

    /// <summary>
    ///  Writes a <see cref="DateTime"/> in binary format.
    /// </summary>
    public static void WriteDateTime(Stream stream, DateTime value)
    {
        using var writer = new BinaryFormatWriterScope(stream);

        // We could use ISerializable here to get the data, but it is pretty
        // heavy weight, and the internals of DateTime should never change.

        new SystemClassWithMembersAndTypes(
            new ClassInfo(1, typeof(DateTime).FullName!, s_dateTimeMemberNames),
            new MemberTypeInfo(
                (BinaryType.Primitive, PrimitiveType.Int64),
                (BinaryType.Primitive, PrimitiveType.UInt64)),
            value.Ticks,
            Unsafe.As<DateTime, ulong>(ref value)).Write(writer);
    }

    /// <summary>
    ///  Writes a <see cref="TimeSpan"/> in binary format.
    /// </summary>
    public static void WriteTimeSpan(Stream stream, TimeSpan value)
    {
        using var writer = new BinaryFormatWriterScope(stream);

        new SystemClassWithMembersAndTypes(
            new ClassInfo(1, typeof(TimeSpan).FullName!, new string[] { "_ticks" }),
            new MemberTypeInfo((BinaryType.Primitive, PrimitiveType.Int64)),
            value.Ticks).Write(writer);
    }

    /// <summary>
    ///  Writes a nint in binary format.
    /// </summary>
    public static void WriteNativeInt(Stream stream, nint value)
    {
        using var writer = new BinaryFormatWriterScope(stream);

        new SystemClassWithMembersAndTypes(
            new ClassInfo(1, typeof(nint).FullName!, new string[] { "value" }),
            new MemberTypeInfo((BinaryType.Primitive, PrimitiveType.Int64)),
            (long)value).Write(writer);
    }

    /// <summary>
    ///  Writes a nuint in binary format.
    /// </summary>
    public static void WriteNativeUInt(Stream stream, nuint value)
    {
        using var writer = new BinaryFormatWriterScope(stream);

        new SystemClassWithMembersAndTypes(
            new ClassInfo(1, typeof(nuint).FullName!, new string[] { "value" }),
            new MemberTypeInfo((BinaryType.Primitive, PrimitiveType.UInt64)),
            (ulong)value).Write(writer);
    }

    /// <summary>
    ///  Attempts to write a primitive value or string in binary format.
    /// </summary>
    /// <returns><see langword="true"/> if successful.</returns>
    public static bool TryWritePrimitive(Stream stream, object primitive)
    {
        long originalPosition = stream.Position;
        try
        {
            WritePrimitive(stream, primitive);
            return true;
        }
        catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
        {
            stream.Position = originalPosition;
            return false;
        }
    }

    /// <summary>
    ///  Writes a primitive value or string in binary format.
    /// </summary>
    public static void WritePrimitive(Stream stream, object primitive)
    {
        Type type = primitive.GetType();
        PrimitiveType primitiveType = Record.GetPrimitiveType(type);

        if (primitiveType == default)
        {
            // These two are considered primitive by .NET but not the binary format spec
            switch (primitive)
            {
                case nint nativeInt:
                    WritePrimitive(stream, nativeInt);
                    return;
                case nuint nativeUint:
                    WritePrimitive(stream, nativeUint);
                    return;
            }

            throw new NotSupportedException($"{nameof(primitive)} is not primitive.");
        }

        // These are handled differently from the rest of the primitive types when serialized on their own.
        switch (primitiveType)
        {
            case PrimitiveType.String:
                WriteString(stream, (string)primitive);
                return;
            case PrimitiveType.Decimal:
                WriteDecimal(stream, (decimal)primitive);
                return;
            case PrimitiveType.DateTime:
                WriteDateTime(stream, (DateTime)primitive);
                return;
            case PrimitiveType.TimeSpan:
                WriteTimeSpan(stream, (TimeSpan)primitive);
                return;
        }

        using var writer = new BinaryFormatWriterScope(stream);

        new SystemClassWithMembersAndTypes(
            new ClassInfo(1, type.FullName!, s_primitiveMemberName),
            new MemberTypeInfo((BinaryType.Primitive, primitiveType)),
            primitive).Write(writer);
    }

    /// <summary>
    ///  Writes a primitive list in binary format.
    /// </summary>
    public static void WritePrimitiveList<T>(Stream stream, IReadOnlyList<T> list)
        where T : unmanaged
    {
        PrimitiveType primitiveType = Record.GetPrimitiveType(typeof(T));
        if (primitiveType == default)
        {
            throw new NotSupportedException($"{nameof(T)} is not primitive.");
        }

        using var writer = new BinaryFormatWriterScope(stream);

        new SystemClassWithMembersAndTypes(
            new ClassInfo(1, $"System.Collections.Generic.List`1[[{typeof(T).FullName}, {Mscorlib}]]", ListMemberNames),
            new MemberTypeInfo(
                (BinaryType.PrimitiveArray, primitiveType),
                (BinaryType.Primitive, PrimitiveType.Int32),
                (BinaryType.Primitive, PrimitiveType.Int32)),
            new MemberReference(2),
            list.Count,
            // _version doesn't matter
            0).Write(writer);

        new ArraySinglePrimitive(
            new(2, list.Count),
            primitiveType,
            new ListConverter<T, object>(list, (T value) => value)).Write(writer);
    }

    /// <summary>
    ///  Writes a <see cref="Hashtable"/> of primitive to primitive values to the given stream in binary format.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Primitive types are anything in the <see cref="PrimitiveType"/> enum.
    ///  </para>
    /// </remarks>
    /// <exception cref="ArgumentException"><paramref name="hashtable"/> contained non-primitive values.</exception>
    public static void WritePrimitiveHashtable(Stream stream, Hashtable hashtable)
    {
        // Get the ISerializable data from the hashtable. This way we don't have to worry about
        // getting the LoadFactor, Version, etc. wrong.
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        SerializationInfo info = new(typeof(Hashtable), FormatterConverterStub.Instance);
#pragma warning restore SYSLIB0050
#pragma warning disable SYSLIB0051 // Type or member is obsolete
        hashtable.GetObjectData(info, default);
#pragma warning restore SYSLIB0051

        // Build up the key and value data
        object[] keys = info.GetValue<object[]>("Keys")!;
        object?[] values = info.GetValue<object?[]>("Values")!;

        using var writer = new BinaryFormatWriterScope(stream);

        new SystemClassWithMembersAndTypes(
            new ClassInfo(1, "System.Collections.Hashtable", HashtableMemberNames),
            new MemberTypeInfo(
                (BinaryType.Primitive, PrimitiveType.Single),
                (BinaryType.Primitive, PrimitiveType.Int32),
                (BinaryType.SystemClass, "System.Collections.IComparer"),
                (BinaryType.SystemClass, "System.Collections.IHashCodeProvider"),
                (BinaryType.Primitive, PrimitiveType.Int32),
                (BinaryType.ObjectArray, null),
                (BinaryType.ObjectArray, null)),
            info.GetValue<float>("LoadFactor"),
            info.GetValue<int>("Version"),
            // No need to persist the comparer and hashcode provider
            ObjectNull.Instance,
            ObjectNull.Instance,
            info.GetValue<int>("HashSize"),
            // MemberReference to Arrays here
            new MemberReference(2),
            new MemberReference(3)).Write(writer);

        // We've used 1, 2 and 3 for the class id and array ids.
        int currentId = 4;

        // Using an array converter to save a temporary array allocation. By not doing this we come much closer
        // to the memory usage of BinaryFormatter for large hashsets.

        StringRecordsCollection strings = new();

        ListConverter<object, object> keyConverter = new(
            keys,
            (object value) => value switch
            {
                string stringValue => strings.GetStringRecord(stringValue, ref currentId),
                _ => new MemberPrimitiveTyped(value)
            });

        ArraySingleObject array = new(new(2, keys.Length), keyConverter);
        array.Write(writer);

        ListConverter<object?, object> valueConverter = new(
            values,
            (object? value) => value switch
            {
                null => ObjectNull.Instance,
                string stringValue => strings.GetStringRecord(stringValue, ref currentId),
                _ => new MemberPrimitiveTyped(value)
            });

        array = new(new(3, values.Length), valueConverter);
        array.Write(writer);
    }
}
