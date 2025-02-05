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
        s_knownTypes ??= new(60, TypeNameComparer.Default)
        {
            // Types are bound to their .NET Framework identities to facilitate interoperability
            { Types.ToTypeName($"{Types.ByteType}, {Assemblies.Mscorlib}"), typeof(byte) },
            { Types.ToTypeName($"{Types.SByteType}, {Assemblies.Mscorlib}"), typeof(sbyte) },
            { Types.ToTypeName($"{Types.Int16Type}, {Assemblies.Mscorlib}"), typeof(short) },
            { Types.ToTypeName($"{Types.UInt16Type}, {Assemblies.Mscorlib}"), typeof(ushort) },
            { Types.ToTypeName($"{Types.Int32Type}, {Assemblies.Mscorlib}"), typeof(int) },
            { Types.ToTypeName($"{Types.UInt32Type}, {Assemblies.Mscorlib}"), typeof(uint) },
            { Types.ToTypeName($"{Types.Int64Type}, {Assemblies.Mscorlib}"), typeof(long) },
            { Types.ToTypeName($"{Types.UInt64Type}, {Assemblies.Mscorlib}"), typeof(ulong) },
            { Types.ToTypeName($"{Types.DoubleType}, {Assemblies.Mscorlib}"), typeof(double) },
            { Types.ToTypeName($"{Types.SingleType}, {Assemblies.Mscorlib}"), typeof(float) },
            { Types.ToTypeName($"{Types.CharType}, {Assemblies.Mscorlib}"), typeof(char) },
            { Types.ToTypeName($"{Types.BooleanType}, {Assemblies.Mscorlib}"), typeof(bool) },
            { Types.ToTypeName($"{Types.StringType}, {Assemblies.Mscorlib}"), typeof(string) },
            { Types.ToTypeName($"{Types.DecimalType}, {Assemblies.Mscorlib}"), typeof(decimal) },
            { Types.ToTypeName($"{Types.DateTimeType}, {Assemblies.Mscorlib}"), typeof(DateTime) },
            { Types.ToTypeName($"{Types.TimeSpanType}, {Assemblies.Mscorlib}"), typeof(TimeSpan) },
            { Types.ToTypeName($"{Types.IntPtrType}, {Assemblies.Mscorlib}"), typeof(IntPtr) },
            { Types.ToTypeName($"{Types.UIntPtrType}, {Assemblies.Mscorlib}"), typeof(UIntPtr) },
            { Types.ToTypeName($"{Types.NotSupportedExceptionType}, {Assemblies.Mscorlib}"), typeof(NotSupportedException) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.BooleanType}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<bool>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.CharType}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<char>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.StringType}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<string>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.SByteType}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<sbyte>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.ByteType}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<byte>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.Int16Type}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<short>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.UInt16Type}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<ushort>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.Int32Type}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<int>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.UInt32Type}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<uint>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.Int64Type}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<long>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.UInt64Type}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<ulong>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.SingleType}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<float>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.DoubleType}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<double>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.DecimalType}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<decimal>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.DateTimeType}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<DateTime>) },
            { Types.ToTypeName($"{Types.ListName}[[{Types.TimeSpanType}, {Assemblies.Mscorlib}]], {Assemblies.Mscorlib}"), typeof(List<TimeSpan>) },
            { Types.ToTypeName($"{Types.ByteType}[], {Assemblies.Mscorlib}"), typeof(byte[]) },
            { Types.ToTypeName($"{Types.SByteType}[], {Assemblies.Mscorlib}"), typeof(sbyte[]) },
            { Types.ToTypeName($"{Types.Int16Type}[], {Assemblies.Mscorlib}"), typeof(short[]) },
            { Types.ToTypeName($"{Types.UInt16Type}[], {Assemblies.Mscorlib}"), typeof(ushort[]) },
            { Types.ToTypeName($"{Types.Int32Type}[], {Assemblies.Mscorlib}"), typeof(int[]) },
            { Types.ToTypeName($"{Types.UInt32Type}[], {Assemblies.Mscorlib}"), typeof(uint[]) },
            { Types.ToTypeName($"{Types.Int64Type}[], {Assemblies.Mscorlib}"), typeof(long[]) },
            { Types.ToTypeName($"{Types.UInt64Type}[], {Assemblies.Mscorlib}"), typeof(ulong[]) },
            { Types.ToTypeName($"{Types.SingleType}[], {Assemblies.Mscorlib}"), typeof(float[]) },
            { Types.ToTypeName($"{Types.DoubleType}[], {Assemblies.Mscorlib}"), typeof(double[]) },
            { Types.ToTypeName($"{Types.CharType}[], {Assemblies.Mscorlib}"), typeof(char[]) },
            { Types.ToTypeName($"{Types.BooleanType}[], {Assemblies.Mscorlib}"), typeof(bool[]) },
            { Types.ToTypeName($"{Types.StringType}[], {Assemblies.Mscorlib}"), typeof(string[]) },
            { Types.ToTypeName($"{Types.DecimalType}[], {Assemblies.Mscorlib}"), typeof(decimal[]) },
            { Types.ToTypeName($"{Types.DateTimeType}[], {Assemblies.Mscorlib}"), typeof(DateTime[]) },
            { Types.ToTypeName($"{Types.TimeSpanType}[], {Assemblies.Mscorlib}"), typeof(TimeSpan[]) },
            { Types.ToTypeName($"{Types.RectangleFType}, {Assemblies.SystemDrawing}"), typeof(RectangleF) },
            { Types.ToTypeName($"{Types.PointFType}, {Assemblies.SystemDrawing}"), typeof(PointF) },
            { Types.ToTypeName($"{Types.SizeFType}, {Assemblies.SystemDrawing}"), typeof(SizeF) },
            { Types.ToTypeName($"{Types.RectangleType}, {Assemblies.SystemDrawing}"), typeof(Rectangle) },
            { Types.ToTypeName($"{Types.PointType}, {Assemblies.SystemDrawing}"), typeof(Point) },
            { Types.ToTypeName($"{Types.SizeType}, {Assemblies.SystemDrawing}"), typeof(Size) },
            { Types.ToTypeName($"{Types.ColorType}, {Assemblies.SystemDrawing}"), typeof(Color) },
            { Types.ToTypeName($"{Types.HashtableType}, {Assemblies.Mscorlib}"), typeof(Hashtable) },
            { Types.ToTypeName($"{Types.ArrayListType}, {Assemblies.Mscorlib}"), typeof(ArrayList) }
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
