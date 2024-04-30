// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Binary array record.
/// </summary>
/// <inheritdoc cref="BinaryArray"/>
internal interface IBinaryArray : IRecord
{
    /// <summary>
    ///  Rank (dimensions) of the array.
    /// </summary>
    Count Rank { get; }

    /// <summary>
    ///  Type of the array.
    /// </summary>
    BinaryArrayType ArrayType { get; }

    /// <summary>
    ///  Lengths of the array (for each dimension).
    /// </summary>
    IReadOnlyList<int> Lengths { get; }

    /// <summary>
    ///  Array element type information.
    /// </summary>
    MemberTypeInfo TypeInfo { get; }
}
