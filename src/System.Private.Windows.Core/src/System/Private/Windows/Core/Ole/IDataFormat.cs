// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.Ole;

/// <summary>
///  Internal interface for a data format.
/// </summary>
internal interface IDataFormat
{
    /// <summary>
    ///  Specifies the name of this format.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///  Specifies the ID number for this format.
    /// </summary>
    int Id { get; }
}

/// <summary>
///  Typed interface for a data format that allows for creation.
/// </summary>
internal interface IDataFormat<T> : IDataFormat
    where T : IDataFormat
{
    /// <summary>
    ///  Creates a new instance of the data format.
    /// </summary>
    static abstract T Create(string name, int id);
}
