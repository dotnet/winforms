// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

/// <summary>
///  Non-generic record base interface.
/// </summary>
internal interface IRecord
{
    /// <summary>
    ///  Id for the record, or null if the record has no id.
    /// </summary>
    Id Id => Id.Null;

    static virtual RecordType RecordType { get; }
}

/// <summary>
///  Typed record interface.
/// </summary>
internal interface IRecord<T> : IRecord where T : class, IRecord
{
}
