// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Map of records.
/// </summary>
internal interface IReadOnlyRecordMap
{
    IRecord this[Id id] { get; }
}
