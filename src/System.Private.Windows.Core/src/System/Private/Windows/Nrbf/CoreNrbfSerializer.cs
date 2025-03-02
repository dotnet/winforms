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

    public static bool IsFullySupportedType(Type type) =>
        // Do not include NotSupportedException, Hashtable, or ArrayList here. See interface docs for details.
        type == typeof(byte)
            || type == typeof(sbyte)
            || type == typeof(short)
            || type == typeof(ushort)
            || type == typeof(int)
            || type == typeof(uint)
            || type == typeof(long)
            || type == typeof(ulong)
            || type == typeof(double)
            || type == typeof(float)
            || type == typeof(char)
            || type == typeof(bool)
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(TimeSpan)
            || type == typeof(IntPtr)
            || type == typeof(UIntPtr)
            || type == typeof(List<byte>)
            || type == typeof(List<sbyte>)
            || type == typeof(List<short>)
            || type == typeof(List<ushort>)
            || type == typeof(List<int>)
            || type == typeof(List<uint>)
            || type == typeof(List<long>)
            || type == typeof(List<ulong>)
            || type == typeof(List<float>)
            || type == typeof(List<double>)
            || type == typeof(List<char>)
            || type == typeof(List<bool>)
            || type == typeof(List<string>)
            || type == typeof(List<decimal>)
            || type == typeof(List<DateTime>)
            || type == typeof(List<TimeSpan>)
            || type == typeof(byte[])
            || type == typeof(sbyte[])
            || type == typeof(short[])
            || type == typeof(ushort[])
            || type == typeof(int[])
            || type == typeof(uint[])
            || type == typeof(long[])
            || type == typeof(ulong[])
            || type == typeof(float[])
            || type == typeof(double[])
            || type == typeof(char[])
            || type == typeof(bool[])
            || type == typeof(string[])
            || type == typeof(decimal[])
            || type == typeof(DateTime[])
            || type == typeof(TimeSpan[])
            || type == typeof(RectangleF)
            || type == typeof(PointF)
            || type == typeof(SizeF)
            || type == typeof(Rectangle)
            || type == typeof(Point)
            || type == typeof(Size)
            || type == typeof(Color);
}
