// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.BinaryFormat.Serializer;

namespace System.Private.Windows.BinaryFormat;

internal static class TypeExtensions
{
    /// <summary>
    ///  Returns the <see cref="PrimitiveType"/> for the given <paramref name="type"/>.
    /// </summary>
    /// <returns><see cref="PrimitiveType"/> or <see langword="default"/> if not a <see cref="PrimitiveType"/>.</returns>
    internal static PrimitiveType GetPrimitiveType(this Type? type) => type is null || type.IsEnum ? default : Type.GetTypeCode(type) switch
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
}
