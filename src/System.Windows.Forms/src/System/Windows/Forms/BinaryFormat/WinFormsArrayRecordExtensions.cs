// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Windows.Forms.BinaryFormat;

internal static class WinFormsArrayRecordExtensions
{
    internal static TypeName GetTypeName(this IPrimitiveTypeRecord primitiveArray)
    {
        return primitiveArray.PrimitiveType switch
        {
            PrimitiveType.Boolean => GetTypeName<bool>(),
            PrimitiveType.Byte => GetTypeName<byte>(),
            PrimitiveType.Char => GetTypeName<char>(),
            PrimitiveType.Decimal => GetTypeName<decimal>(),
            PrimitiveType.Double => GetTypeName<double>(),
            PrimitiveType.Int16 => GetTypeName<short>(),
            PrimitiveType.Int32 => GetTypeName<int>(),
            PrimitiveType.Int64 => GetTypeName<long>(),
            PrimitiveType.SByte => GetTypeName<sbyte>(),
            PrimitiveType.Single => GetTypeName<float>(),
            PrimitiveType.TimeSpan => GetTypeName<TimeSpan>(),
            PrimitiveType.DateTime => GetTypeName<DateTime>(),
            PrimitiveType.UInt16 => GetTypeName<ushort>(),
            PrimitiveType.UInt32 => GetTypeName<uint>(),
            PrimitiveType.UInt64 => GetTypeName<ulong>(),
            _ => throw new InvalidOperationException($"Unexpected primitive array type: '{primitiveArray.PrimitiveType}'")
        };

        static TypeName GetTypeName<T>() where T : unmanaged =>
           TypeName.Parse($"{typeof(T).FullName}[], {typeof(T).Assembly.FullName}");
    }
}
