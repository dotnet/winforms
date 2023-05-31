// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

internal static class OwnedObjectExtensions
{
    /// <summary>
    ///  Tries to get the owner as the given type.
    /// </summary>
    public static bool TryGetOwnerAs<TOwner, TAs>(
        this IOwnedObject<TOwner> ownedObject,
        [NotNullWhen(true)] out TAs? ownerAs)
        where TOwner : class
        where TAs : class
    {
        ownerAs = ownedObject.Owner as TAs;
        return ownerAs is not null;
    }
}
