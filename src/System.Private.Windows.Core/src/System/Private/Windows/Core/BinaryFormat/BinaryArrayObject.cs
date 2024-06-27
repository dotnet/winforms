// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat;

/// <summary>
///  <see cref="BinaryArray"/> of objects.
/// </summary>
/// <inheritdoc cref="BinaryArray"/>
internal sealed class BinaryArrayObject : BinaryArray<object?>
{
    internal BinaryArrayObject(
        Count rank,
        BinaryArrayType arrayType,
        IReadOnlyList<int> lengths,
        ArrayInfo arrayInfo,
        MemberTypeInfo typeInfo,
        BinaryFormattedObject.IParseState state) : base(
        rank,
        arrayType,
        lengths,
        arrayInfo,
        typeInfo,
        ReadObjectArrayValues(state, typeInfo, arrayInfo.Length))
    {
    }

    private protected override void Write(BinaryWriter writer) => throw new NotSupportedException();
}
