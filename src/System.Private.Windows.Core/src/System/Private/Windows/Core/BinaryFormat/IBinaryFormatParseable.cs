// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat;

/// <summary>
///  Specifies that the given record type can be created from a <see cref="BinaryReader"/>.
/// </summary>
internal interface IBinaryFormatParseable<T> where T : IRecord
{
    /// <summary>
    ///  Creates the type utilizaing the given <see cref="BinaryReader"/>.
    /// </summary>
    static abstract T Parse(BinaryFormattedObject.IParseState state);
}
