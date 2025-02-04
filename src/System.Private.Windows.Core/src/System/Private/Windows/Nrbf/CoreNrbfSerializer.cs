// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Formats.Nrbf;
using System.Private.Windows.BinaryFormat;
using System.Reflection.Metadata;

namespace System.Private.Windows.Nrbf;

/// <summary>
///  Core NRBF serializer. Supports common .NET types.
/// </summary>
internal class CoreNrbfSerializer : INrbfSerializer
{
    private static Dictionary<TypeName, Type>? s_knownTypes;

    // These types are read from and written to serialized stream manually, accessing record field by field.
    // Thus they are re-hydrated with no formatters and are safe. The default resolver should recognize them
    // to resolve primitive types or fields of the specified type T.
    private static readonly Type[] s_intrinsicTypes =
    [
        // Primitive types.
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(double),
        typeof(float),
        typeof(char),
        typeof(bool),
        typeof(string),
        typeof(decimal),
        typeof(DateTime),
        typeof(TimeSpan),
        typeof(IntPtr),
        typeof(UIntPtr),
        // Special type we use to report that binary formatting is disabled.
        typeof(NotSupportedException),

        // Lists of primitive types
        typeof(List<byte>),
        typeof(List<sbyte>),
        typeof(List<short>),
        typeof(List<ushort>),
        typeof(List<int>),
        typeof(List<uint>),
        typeof(List<long>),
        typeof(List<ulong>),
        typeof(List<float>),
        typeof(List<double>),
        typeof(List<char>),
        typeof(List<bool>),
        typeof(List<string>),
        typeof(List<decimal>),
        typeof(List<DateTime>),
        typeof(List<TimeSpan>),

        // Arrays of primitive types.
        typeof(byte[]),
        typeof(sbyte[]),
        typeof(short[]),
        typeof(ushort[]),
        typeof(int[]),
        typeof(uint[]),
        typeof(long[]),
        typeof(ulong[]),
        typeof(float[]),
        typeof(double[]),
        typeof(char[]),
        typeof(bool[]),
        typeof(string[]),
        typeof(decimal[]),
        typeof(DateTime[]),
        typeof(TimeSpan[]),

        // Exchange types, they are serialized with the .NET Framework assembly name.
        // In .NET they are located in System.Drawing.Primitives.
        typeof(RectangleF),
        typeof(PointF),
        typeof(SizeF),
        typeof(Rectangle),
        typeof(Point),
        typeof(Size),
        typeof(Color),

        // Hashtable and ArrayList are supported if they contain primitives and no custom comparers.
        typeof(Hashtable),
        typeof(ArrayList)
    ];

    public static bool TryWriteObject(Stream stream, object value) =>
        BinaryFormatWriter.TryWriteFrameworkObject(stream, value)
        || BinaryFormatWriter.TryWriteJsonData(stream, value)
        || BinaryFormatWriter.TryWriteDrawingPrimitivesObject(stream, value);

    public static bool TryGetObject(SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        record.TryGetFrameworkObject(out value)
        // While these shouldn't normally be in a ResX file, it doesn't hurt to read them and simplifies the code.
        || record.TryGetDrawingPrimitivesObject(out value);

    public static bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type)
    {
        if (s_knownTypes is null)
        {
            s_knownTypes = new(s_intrinsicTypes.Length, TypeNameComparer.Default);
            foreach (Type intrinsic in s_intrinsicTypes)
            {
                s_knownTypes.Add(intrinsic.ToTypeName(), intrinsic);
            }
        }

        return s_knownTypes.TryGetValue(typeName, out type);
    }

    public static bool IsSupportedType<T>() =>
        typeof(T) == typeof(byte)
            || typeof(T) == typeof(sbyte)
            || typeof(T) == typeof(short)
            || typeof(T) == typeof(ushort)
            || typeof(T) == typeof(int)
            || typeof(T) == typeof(uint)
            || typeof(T) == typeof(long)
            || typeof(T) == typeof(ulong)
            || typeof(T) == typeof(double)
            || typeof(T) == typeof(float)
            || typeof(T) == typeof(char)
            || typeof(T) == typeof(bool)
            || typeof(T) == typeof(string)
            || typeof(T) == typeof(decimal)
            || typeof(T) == typeof(DateTime)
            || typeof(T) == typeof(TimeSpan)
            || typeof(T) == typeof(IntPtr)
            || typeof(T) == typeof(UIntPtr)
            || typeof(T) == typeof(NotSupportedException)
            || typeof(T) == typeof(List<byte>)
            || typeof(T) == typeof(List<sbyte>)
            || typeof(T) == typeof(List<short>)
            || typeof(T) == typeof(List<ushort>)
            || typeof(T) == typeof(List<int>)
            || typeof(T) == typeof(List<uint>)
            || typeof(T) == typeof(List<long>)
            || typeof(T) == typeof(List<ulong>)
            || typeof(T) == typeof(List<float>)
            || typeof(T) == typeof(List<double>)
            || typeof(T) == typeof(List<char>)
            || typeof(T) == typeof(List<bool>)
            || typeof(T) == typeof(List<string>)
            || typeof(T) == typeof(List<decimal>)
            || typeof(T) == typeof(List<DateTime>)
            || typeof(T) == typeof(List<TimeSpan>)
            || typeof(T) == typeof(byte[])
            || typeof(T) == typeof(sbyte[])
            || typeof(T) == typeof(short[])
            || typeof(T) == typeof(ushort[])
            || typeof(T) == typeof(int[])
            || typeof(T) == typeof(uint[])
            || typeof(T) == typeof(long[])
            || typeof(T) == typeof(ulong[])
            || typeof(T) == typeof(float[])
            || typeof(T) == typeof(double[])
            || typeof(T) == typeof(char[])
            || typeof(T) == typeof(bool[])
            || typeof(T) == typeof(string[])
            || typeof(T) == typeof(decimal[])
            || typeof(T) == typeof(DateTime[])
            || typeof(T) == typeof(TimeSpan[])
            || typeof(T) == typeof(RectangleF)
            || typeof(T) == typeof(PointF)
            || typeof(T) == typeof(SizeF)
            || typeof(T) == typeof(Rectangle)
            || typeof(T) == typeof(Point)
            || typeof(T) == typeof(Size)
            || typeof(T) == typeof(Color);
}
