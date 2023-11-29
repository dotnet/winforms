// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

    /// <summary>
    ///  If the owner object exists, then verify if the handle is created, if owner does not exist, return false.
    /// </summary>
    public static bool IsOwnerHandleCreated<TOwner, TAs>(
        this IOwnedObject<TOwner> ownedObject,
        [NotNullWhen(true)] out TAs? ownerAs)
        where TOwner : class
        where TAs : Control
    {
        ownerAs = ownedObject.Owner as TAs;
        if (ownerAs is null)
        {
            return false;
        }

        if (!ownerAs.IsHandleCreated)
        {
            ownerAs = null;
            return false;
        }

        return true;
    }
}
