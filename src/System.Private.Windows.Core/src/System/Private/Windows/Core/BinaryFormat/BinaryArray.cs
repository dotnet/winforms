// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Private.Windows.Core.BinaryFormat;

/// <summary>
///  Array of objects.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/9c62c928-db4e-43ca-aeba-146256ef67c2">
///    [MS-NRBF] 2.4.3.1
///   </see>
///  </para>
/// </remarks>
internal static partial class BinaryArray
{
    private const int MaxRanks = 32;

    public static RecordType RecordType => RecordType.BinaryArray;

    internal static IBinaryArray Parse(BinaryFormattedObject.IParseState state)
    {
        Id objectId = state.Reader.ReadInt32();
        BinaryArrayType arrayType = (BinaryArrayType)state.Reader.ReadByte();

        if (arrayType is not (BinaryArrayType.Single or BinaryArrayType.Rectangular or BinaryArrayType.Jagged))
        {
            // Values: https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/4dbbf3a8-6bc4-4dfc-aa7e-36a35be6ff58
            if ((int)arrayType is 3 or 4 or 5)
            {
                throw new NotSupportedException("Offset arrays are not supported.");
            }

            throw new SerializationException("Invalid array type.");
        }

        int rank = state.Reader.ReadInt32();

        if (arrayType is BinaryArrayType.Single or BinaryArrayType.Jagged)
        {
            // Jagged array is an array of arrays, there should always be one rank
            // for the "outer" array.
            if (rank != 1)
            {
                throw new SerializationException("Invalid array rank.");
            }
        }
        else if (arrayType is BinaryArrayType.Rectangular)
        {
            // Multidimensional array
            if (rank is < 2 or > MaxRanks)
            {
                throw new SerializationException("Invalid array rank.");
            }
        }

        int[] lengths = new int[rank];
        int length;

        if (arrayType is not BinaryArrayType.Rectangular)
        {
            length = state.Reader.ReadInt32();
            if (length < 0)
            {
                throw new SerializationException("Invalid array length.");
            }

            lengths[0] = length;
        }
        else
        {
            length = 1;

            for (int i = 0; i < rank; i++)
            {
                int rankLength = state.Reader.ReadInt32();
                if (rankLength < 0)
                {
                    throw new SerializationException("Invalid array length.");
                }

                // Rectangular (multidimensional) length is product of lengths.
                //
                // It is technically possible to have multidimensional arrays with
                // a total length over int.MaxValue. Even with just bytes this would
                // be a very large array (2GB). To constrain this reader we'll reject
                // anything that goes over this.
                length = checked(length * rankLength);
                lengths[i] = rankLength;
            }
        }

        MemberTypeInfo typeInfo = MemberTypeInfo.Parse(state.Reader);

        IBinaryArray array = typeInfo.Type is not BinaryType.Primitive
            ? new BinaryArrayObject(rank, arrayType, lengths, new(objectId, length), typeInfo, state)
            : (PrimitiveType)typeInfo.Info! switch
            {
                PrimitiveType.Boolean => new BinaryArrayPrimitive<bool>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Byte => new BinaryArrayPrimitive<byte>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.SByte => new BinaryArrayPrimitive<sbyte>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Char => new BinaryArrayPrimitive<char>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Int16 => new BinaryArrayPrimitive<short>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.UInt16 => new BinaryArrayPrimitive<ushort>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Int32 => new BinaryArrayPrimitive<int>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.UInt32 => new BinaryArrayPrimitive<uint>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Int64 => new BinaryArrayPrimitive<long>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.UInt64 => new BinaryArrayPrimitive<ulong>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Single => new BinaryArrayPrimitive<float>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Double => new BinaryArrayPrimitive<double>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Decimal => new BinaryArrayPrimitive<decimal>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.DateTime => new BinaryArrayPrimitive<DateTime>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.TimeSpan => new BinaryArrayPrimitive<TimeSpan>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                _ => throw new SerializationException($"Invalid primitive type '{(PrimitiveType)typeInfo.Info}'"),
            };

        return array;
    }
}

internal abstract partial class BinaryArray<T> : ArrayRecord<T>, IRecord<BinaryArrayObject>, IBinaryArray
{
    private readonly MemberTypeInfo _memberTypeInfo;

    public Count Rank { get; }
    public BinaryArrayType ArrayType { get; }
    public IReadOnlyList<int> Lengths { get; }

    public override BinaryType ElementType => _memberTypeInfo.Type;

    public object? ElementTypeInfo => _memberTypeInfo.Info;

    internal BinaryArray(
        Count rank,
        BinaryArrayType arrayType,
        IReadOnlyList<int> lengths,
        ArrayInfo arrayInfo,
        MemberTypeInfo typeInfo,
        IReadOnlyList<T> values)
        : base(arrayInfo, values)
    {
        Rank = rank;
        ArrayType = arrayType;
        _memberTypeInfo = typeInfo;
        Lengths = lengths;
    }
}
