// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

internal static partial class BinaryArray
{
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
            BinaryFormattedObject.IParseState state)
            : base(arrayInfo, ReadObjectArrayValues(state, typeInfo[0].Type, typeInfo[0].Info, arrayInfo.Length))
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
