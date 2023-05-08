﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Specifies that the given record type can be created from a <see cref="BinaryReader"/>.
/// </summary>
internal interface IBinaryFormatParseable<T> where T : IRecord
{
    /// <summary>
    ///  Creates the type utilizaing the given <see cref="BinaryReader"/>.
    /// </summary>
    /// <param name="recordMap">
    ///  Record map for looking up referenced records. If this record has an id it will be added to the map.
    /// </param>
    static abstract T Parse(BinaryReader reader, RecordMap recordMap);
}
