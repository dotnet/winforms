// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Interface to describe ownership by another object.
/// </summary>
internal interface IOwnedObject<TOwner> where TOwner : class
{
    /// <summary>
    ///  The owner of this object. Use <see cref="OwnedObjectExtensions.TryGetOwnerAs"/> for all access.
    /// </summary>
    TOwner? Owner { get; }
}
