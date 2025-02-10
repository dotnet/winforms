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
        // As these are all common .NET types, we'll match just by their full name and ignore assembly details.
        // This will handle version to version changes and allow compat with .NET Framework serialization.
        s_knownTypes ??= new(60, TypeNameComparer.FullNameMatch)
        {
            // Types are bound to their .NET Framework identities to facilitate interoperability
            { TypeName.Parse(Types.ByteType), typeof(byte) },
            { TypeName.Parse(Types.SByteType), typeof(sbyte) },
            { TypeName.Parse(Types.Int16Type), typeof(short) },
            { TypeName.Parse(Types.UInt16Type), typeof(ushort) },
            { TypeName.Parse(Types.Int32Type), typeof(int) },
            { TypeName.Parse(Types.UInt32Type), typeof(uint) },
            { TypeName.Parse(Types.Int64Type), typeof(long) },
            { TypeName.Parse(Types.UInt64Type), typeof(ulong) },
            { TypeName.Parse(Types.DoubleType), typeof(double) },
            { TypeName.Parse(Types.SingleType), typeof(float) },
            { TypeName.Parse(Types.CharType), typeof(char) },
            { TypeName.Parse(Types.BooleanType), typeof(bool) },
            { TypeName.Parse(Types.StringType), typeof(string) },
            { TypeName.Parse(Types.DecimalType), typeof(decimal) },
            { TypeName.Parse(Types.DateTimeType), typeof(DateTime) },
            { TypeName.Parse(Types.TimeSpanType), typeof(TimeSpan) },
            { TypeName.Parse(Types.IntPtrType), typeof(IntPtr) },
            { TypeName.Parse(Types.UIntPtrType), typeof(UIntPtr) },
            { TypeName.Parse(Types.NotSupportedExceptionType), typeof(NotSupportedException) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.BooleanType}]]"), typeof(List<bool>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.CharType}]]"), typeof(List<char>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.StringType}]]"), typeof(List<string>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.SByteType}]]"), typeof(List<sbyte>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.ByteType}]]"), typeof(List<byte>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.Int16Type}]]"), typeof(List<short>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.UInt16Type}]]"), typeof(List<ushort>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.Int32Type}]]"), typeof(List<int>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.UInt32Type}]]"), typeof(List<uint>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.Int64Type}]]"), typeof(List<long>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.UInt64Type}]]"), typeof(List<ulong>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.SingleType}]]"), typeof(List<float>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.DoubleType}]]"), typeof(List<double>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.DecimalType}]]"), typeof(List<decimal>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.DateTimeType}]]"), typeof(List<DateTime>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.TimeSpanType}]]"), typeof(List<TimeSpan>) },
            { Types.ToTypeName($"{Types.ByteType}[]"), typeof(byte[]) },
            { Types.ToTypeName($"{Types.SByteType}[]"), typeof(sbyte[]) },
            { Types.ToTypeName($"{Types.Int16Type}[]"), typeof(short[]) },
            { Types.ToTypeName($"{Types.UInt16Type}[]"), typeof(ushort[]) },
            { Types.ToTypeName($"{Types.Int32Type}[]"), typeof(int[]) },
            { Types.ToTypeName($"{Types.UInt32Type}[]"), typeof(uint[]) },
            { Types.ToTypeName($"{Types.Int64Type}[]"), typeof(long[]) },
            { Types.ToTypeName($"{Types.UInt64Type}[]"), typeof(ulong[]) },
            { Types.ToTypeName($"{Types.SingleType}[]"), typeof(float[]) },
            { Types.ToTypeName($"{Types.DoubleType}[]"), typeof(double[]) },
            { Types.ToTypeName($"{Types.CharType}[]"), typeof(char[]) },
            { Types.ToTypeName($"{Types.BooleanType}[]"), typeof(bool[]) },
            { Types.ToTypeName($"{Types.StringType}[]"), typeof(string[]) },
            { Types.ToTypeName($"{Types.DecimalType}[]"), typeof(decimal[]) },
            { Types.ToTypeName($"{Types.DateTimeType}[]"), typeof(DateTime[]) },
            { Types.ToTypeName($"{Types.TimeSpanType}[]"), typeof(TimeSpan[]) },
            { Types.ToTypeName($"{Types.RectangleFType}"), typeof(RectangleF) },
            { Types.ToTypeName($"{Types.PointFType}"), typeof(PointF) },
            { Types.ToTypeName($"{Types.SizeFType}"), typeof(SizeF) },
            { Types.ToTypeName($"{Types.RectangleType}"), typeof(Rectangle) },
            { Types.ToTypeName($"{Types.PointType}"), typeof(Point) },
            { Types.ToTypeName($"{Types.SizeType}"), typeof(Size) },
            { Types.ToTypeName($"{Types.ColorType}"), typeof(Color) },
            { Types.ToTypeName($"{Types.HashtableType}"), typeof(Hashtable) },
            { Types.ToTypeName($"{Types.ArrayListType}"), typeof(ArrayList) }
        };

        Debug.Assert(s_knownTypes.Count == 60);
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
