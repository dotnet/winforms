// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
///  Interface to encapsulate ownership of an accessible object.
/// </summary>
internal interface IAccessibleOwner<T> where T : class
{
    /// <summary>
    ///  The owner of the accessible object. Use <see cref="AccessibleOwnerExtensions.TryGetOwnerAs"/> for all access.
    /// </summary>
    T? Owner { get; }
}
