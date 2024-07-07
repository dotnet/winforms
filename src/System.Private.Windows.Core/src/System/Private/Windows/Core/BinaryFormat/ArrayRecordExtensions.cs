// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Private.Windows.Core.BinaryFormat;

internal static class ArrayRecordExtensions
{
    internal static Array GetPrimitiveArray(this IPrimitiveTypeRecord primitiveArray) => primitiveArray.PrimitiveType switch
    {
        PrimitiveType.Boolean => ((ArrayRecord<bool>)primitiveArray).AsArray(),
        PrimitiveType.Byte => ((ArrayRecord<byte>)primitiveArray).AsArray(),
        PrimitiveType.Char => ((ArrayRecord<char>)primitiveArray).AsArray(),
        PrimitiveType.Decimal => ((ArrayRecord<decimal>)primitiveArray).AsArray(),
        PrimitiveType.Double => ((ArrayRecord<double>)primitiveArray).AsArray(),
        PrimitiveType.Int16 => ((ArrayRecord<short>)primitiveArray).AsArray(),
        PrimitiveType.Int32 => ((ArrayRecord<int>)primitiveArray).AsArray(),
        PrimitiveType.Int64 => ((ArrayRecord<long>)primitiveArray).AsArray(),
        PrimitiveType.SByte => ((ArrayRecord<sbyte>)primitiveArray).AsArray(),
        PrimitiveType.Single => ((ArrayRecord<float>)primitiveArray).AsArray(),
        PrimitiveType.TimeSpan => ((ArrayRecord<TimeSpan>)primitiveArray).AsArray(),
        PrimitiveType.DateTime => ((ArrayRecord<DateTime>)primitiveArray).AsArray(),
        PrimitiveType.UInt16 => ((ArrayRecord<ushort>)primitiveArray).AsArray(),
        PrimitiveType.UInt32 => ((ArrayRecord<uint>)primitiveArray).AsArray(),
        PrimitiveType.UInt64 => ((ArrayRecord<ulong>)primitiveArray).AsArray(),
        _ => throw new SerializationException($"Unexpected primitive array type: '{primitiveArray.PrimitiveType}'")
    };

    /// <summary>
    ///  Convert the source to an array, if it is not already an array.
    /// </summary>
    private static Array AsArray<T>(this ArrayRecord<T> record) where T : unmanaged
    {
        if (record.ArrayObjects is not T[] rawArray)
        {
            Debug.Fail("Should not have any primitive arrays that are not already arrays.");
            throw new InvalidOperationException();
        }

        if (record is not IBinaryArray binaryArray || binaryArray.ArrayType is BinaryArrayType.Single or BinaryArrayType.Jagged)
        {
            return rawArray;
        }

        if (binaryArray.ArrayType is not BinaryArrayType.Rectangular)
        {
            // This should not be possible.
            throw new NotSupportedException();
        }

        Array array = Array.CreateInstance(typeof(T), binaryArray.Lengths.ToArray());
        Span<T> flatSpan = array.GetArrayData<T>();
        rawArray.AsSpan().CopyTo(flatSpan);
        return array;
    }
}
