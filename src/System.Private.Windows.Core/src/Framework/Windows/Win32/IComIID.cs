// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

/// <summary>
///  Common interface for COM interface wrapping structs.
/// </summary>
/// <remarks>
///  <para>
///   On .NET 6 and later, this is provided by CSWin32 as an abstract static.
///  </para>
/// </remarks>
internal interface IComIID
{
    /// <summary>
    ///  The identifier (IID) GUID for this interface.
    /// </summary>
    ref readonly Guid Guid { get; }
}
