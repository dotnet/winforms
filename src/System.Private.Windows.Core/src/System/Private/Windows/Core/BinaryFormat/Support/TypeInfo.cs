// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.BinaryFormat.Serializer;
using System.Reflection;

namespace System.Private.Windows.Core.BinaryFormat;

internal static class TypeInfo
{
    public const string BooleanType = "System.Boolean";
    public const string CharType = "System.Char";
    public const string StringType = "System.String";
    public const string SByteType = "System.SByte";
    public const string ByteType = "System.Byte";
    public const string Int16Type = "System.Int16";
    public const string UInt16Type = "System.UInt16";
    public const string Int32Type = "System.Int32";
    public const string UInt32Type = "System.UInt32";
    public const string Int64Type = "System.Int64";
    public const string DecimalType = "System.Decimal";
    public const string UInt64Type = "System.UInt64";
    public const string SingleType = "System.Single";
    public const string DoubleType = "System.Double";
    public const string TimeSpanType = "System.TimeSpan";
    public const string DateTimeType = "System.DateTime";
    public const string IntPtrType = "System.IntPtr";
    public const string UIntPtrType = "System.UIntPtr";

    public const string HashtableType = "System.Collections.Hashtable";
    public const string IDictionaryType = "System.Collections.IDictionary";
    public const string ExceptionType = "System.Exception";
    public const string NotSupportedExceptionType = "System.NotSupportedException";

    public const string MscorlibAssemblyName
        = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
    public const string SystemDrawingAssemblyName
        = "System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    private static Assembly? s_mscorlibFacadeAssembly;

    internal static Assembly MscorlibAssembly => s_mscorlibFacadeAssembly
        ??= Assembly.Load("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

    internal static Assembly CorelibAssembly { get; } = typeof(string).Assembly;
    internal static string CorelibAssemblyString { get; } = CorelibAssembly.FullName!;

    /// <summary>
    ///  Returns the <see cref="PrimitiveType"/> for the given <paramref name="type"/>.
    /// </summary>
    /// <returns><see cref="PrimitiveType"/> or <see langword="default"/> if not a <see cref="PrimitiveType"/>.</returns>
    internal static PrimitiveType GetPrimitiveType(Type type) => type.IsEnum ? default : Type.GetTypeCode(type) switch
    {
        TypeCode.Boolean => PrimitiveType.Boolean,
        TypeCode.Char => PrimitiveType.Char,
        TypeCode.SByte => PrimitiveType.SByte,
        TypeCode.Byte => PrimitiveType.Byte,
        TypeCode.Int16 => PrimitiveType.Int16,
        TypeCode.UInt16 => PrimitiveType.UInt16,
        TypeCode.Int32 => PrimitiveType.Int32,
        TypeCode.UInt32 => PrimitiveType.UInt32,
        TypeCode.Int64 => PrimitiveType.Int64,
        TypeCode.UInt64 => PrimitiveType.UInt64,
        TypeCode.Single => PrimitiveType.Single,
        TypeCode.Double => PrimitiveType.Double,
        TypeCode.Decimal => PrimitiveType.Decimal,
        TypeCode.DateTime => PrimitiveType.DateTime,
        TypeCode.String => PrimitiveType.String,
        // TypeCode.Empty => 0,
        // TypeCode.Object => 0,
        // TypeCode.DBNull => 0,
        _ => type == typeof(TimeSpan) ? PrimitiveType.TimeSpan : default,
    };

    /// <summary>
    ///  Returns the <see cref="PrimitiveType"/> for the given <paramref name="type"/> if it is a simple primitive array.
    /// </summary>
    /// <returns><see cref="PrimitiveType"/> or <see langword="default"/> if not a primitive array.</returns>
    internal static PrimitiveType GetPrimitiveArrayType(Type type) => type.IsSZArray
        ? GetPrimitiveType(type.GetElementType()!)
        : default;

    /// <summary>
    ///  Get the proper <see cref="BinaryType"/> for the given <paramref name="type"/>.
    /// </summary>
    internal static BinaryType GetBinaryType(Type type)
    {
        if (type == typeof(string))
        {
            return BinaryType.String;
        }
        else if (type == typeof(object))
        {
            return BinaryType.Object;
        }
        else if (type == typeof(object[]))
        {
            return BinaryType.ObjectArray;
        }
        else if (type == typeof(string[]))
        {
            return BinaryType.StringArray;
        }
        else if (GetPrimitiveArrayType(type) != default)
        {
            return BinaryType.PrimitiveArray;
        }
        else
        {
            return GetPrimitiveType(type) == default
                ? type.Assembly == MscorlibAssembly ? BinaryType.SystemClass : BinaryType.Class
                : BinaryType.Primitive;
        }
    }
}
