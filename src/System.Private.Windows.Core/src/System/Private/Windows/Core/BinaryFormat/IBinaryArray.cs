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
    ///  Additional array element type information. Varies with <see cref="ArrayRecord.ElementType"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   For <see cref="BinaryType.SystemClass"/> this will either be the <see cref="Type"/> or the
    ///   type name string.
    ///  </para>
    ///  <para>
    ///   For <see cref="BinaryType.Class"/> this will either be the <see cref="Type"/> or a tuple
    ///   of the type name string and the library <see cref="Id"/>.
    ///  </para>
    ///  <para>
    ///   For <see cref="BinaryType.PrimitiveArray"/> this will be the <see cref="PrimitiveType"/>.
    ///  </para>
    ///  <para>
    ///   For all other types, this is <see langword="null"/>.
    ///  </para>
    /// </remarks>
    object? ElementTypeInfo { get; }
}
