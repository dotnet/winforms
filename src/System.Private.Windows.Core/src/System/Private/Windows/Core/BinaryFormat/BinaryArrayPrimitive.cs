// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  <see cref="BinaryArray"/> of primitive values.
/// </summary>
/// <inheritdoc cref="BinaryArray"/>
internal sealed class BinaryArrayPrimitive<T> : BinaryArray<T>, IRecord<BinaryArrayPrimitive<T>>, IPrimitiveTypeRecord where T : unmanaged
{
    public PrimitiveType PrimitiveType { get; }

    internal BinaryArrayPrimitive(
        Count rank,
        BinaryArrayType arrayType,
        IReadOnlyList<int> lengths,
        ArrayInfo arrayInfo,
        MemberTypeInfo typeInfo,
        BinaryReader reader)
        : base(rank, arrayType, lengths, arrayInfo, typeInfo, reader.ReadPrimitiveArray<T>(arrayInfo.Length))
    {
        PrimitiveType = (PrimitiveType)typeInfo.Info!;
    }

    private protected override void Write(BinaryWriter writer) => throw new NotSupportedException();
}
