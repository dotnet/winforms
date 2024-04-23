// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

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

    internal static IBinaryArray Parse(BinaryFormattedObject.ParseState state)
    {
        Id objectId = state.Reader.ReadInt32();
        BinaryArrayType arrayType = (BinaryArrayType)state.Reader.ReadByte();

        if (arrayType is not (BinaryArrayType.Single or BinaryArrayType.Rectangular or BinaryArrayType.Jagged))
        {
            if (arrayType is BinaryArrayType.SingleOffset or BinaryArrayType.RectangularOffset or BinaryArrayType.JaggedOffset)
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

        MemberTypeInfo typeInfo = MemberTypeInfo.Parse(state.Reader, 1);
        (BinaryType type, object? info) = typeInfo[0];

        IBinaryArray array = type is not BinaryType.Primitive
            ? new ObjectBinaryArray(rank, arrayType, lengths, new(objectId, length), typeInfo, state)
            : (PrimitiveType)info! switch
            {
                PrimitiveType.Boolean => new PrimitiveBinaryArray<bool>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Byte => new PrimitiveBinaryArray<byte>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.SByte => new PrimitiveBinaryArray<sbyte>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Char => new PrimitiveBinaryArray<char>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Int16 => new PrimitiveBinaryArray<short>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.UInt16 => new PrimitiveBinaryArray<ushort>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Int32 => new PrimitiveBinaryArray<int>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.UInt32 => new PrimitiveBinaryArray<uint>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Int64 => new PrimitiveBinaryArray<long>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.UInt64 => new PrimitiveBinaryArray<ulong>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Single => new PrimitiveBinaryArray<float>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Double => new PrimitiveBinaryArray<double>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.Decimal => new PrimitiveBinaryArray<decimal>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.DateTime => new PrimitiveBinaryArray<DateTime>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                PrimitiveType.TimeSpan => new PrimitiveBinaryArray<TimeSpan>(rank, arrayType, lengths, new(objectId, length), typeInfo, state.Reader),
                _ => throw new SerializationException($"Invalid primitive type '{(PrimitiveType)info}'"),
            };

        state.RecordMap[objectId] = array;
        return array;
    }

    /// <summary>
    ///  <see cref="BinaryArray"/> of objects.
    /// </summary>
    /// <inheritdoc cref="BinaryArray"/>
    private sealed class ObjectBinaryArray : ArrayRecord<object?>, IRecord<ObjectBinaryArray>, IBinaryArray
    {
        public Count Rank { get; }
        public BinaryArrayType ArrayType { get; }
        public MemberTypeInfo TypeInfo { get; }
        public IReadOnlyList<int> Lengths { get; }

        internal ObjectBinaryArray(
            Count rank,
            BinaryArrayType type,
            IReadOnlyList<int> lengths,
            ArrayInfo arrayInfo,
            MemberTypeInfo typeInfo,
            BinaryFormattedObject.ParseState state)
            : base(arrayInfo, ReadValues(state, typeInfo[0].Type, typeInfo[0].Info, arrayInfo.Length))
        {
            Rank = rank;
            ArrayType = type;
            TypeInfo = typeInfo;
            Lengths = lengths;
        }

        public override void Write(BinaryWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
