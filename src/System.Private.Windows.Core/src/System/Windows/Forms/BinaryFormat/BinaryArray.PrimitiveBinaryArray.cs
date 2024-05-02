// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

internal static partial class BinaryArray
{
    /// <summary>
    ///  <see cref="BinaryArray"/> of primitive values.
    /// </summary>
    /// <inheritdoc cref="BinaryArray"/>
    private sealed class PrimitiveBinaryArray<T> : ArrayRecord<T>, IRecord<PrimitiveBinaryArray<T>>, IBinaryArray, IPrimitiveTypeRecord where T : unmanaged
    {
        public Count Rank { get; }
        public BinaryArrayType ArrayType { get; }
        public MemberTypeInfo TypeInfo { get; }
        public IReadOnlyList<int> Lengths { get; }
        public PrimitiveType PrimitiveType { get; }

        internal PrimitiveBinaryArray(
            Count rank,
            BinaryArrayType arrayType,
            IReadOnlyList<int> lengths,
            ArrayInfo arrayInfo,
            MemberTypeInfo typeInfo,
            BinaryReader reader)
            : base(arrayInfo, reader.ReadPrimitiveArray<T>(arrayInfo.Length))
        {
            Rank = rank;
            ArrayType = arrayType;
            TypeInfo = typeInfo;
            Lengths = lengths;
            PrimitiveType = (PrimitiveType)typeInfo[0].Info!;
        }

        public override void Write(BinaryWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
