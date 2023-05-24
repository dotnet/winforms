// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

internal static class AccessibleOwnerExtensions
{
    /// <summary>
    ///  Tries to get the owner of the accessible object.
    /// </summary>
    public static bool TryGetOwnerAs<TOwner, TAs>(
        this IAccessibleOwner<TOwner> accessibleOwner,
        [NotNullWhen(true)] out TAs? owner)
        where TOwner : class
        where TAs : class
    {
        owner = accessibleOwner.Owner as TAs;
        return owner is not null;
    }

    /// <summary>
    ///  Gets the owner's accessible role.
    /// </summary>
    public static AccessibleRole GetOwnerAccessibleRole<TOwner>(
        this IAccessibleOwner<TOwner> accessibleOwner,
        AccessibleRole defaultRole = AccessibleRole.Default)
        where TOwner : Control
    {
        AccessibleRole role = accessibleOwner.Owner?.AccessibleRole ?? AccessibleRole.Default;
        return role == AccessibleRole.Default ? defaultRole : role;
    }

    /// <summary>
    ///  Gets the owner's accessible name.
    /// </summary>
    public static string GetOwnerAccessibleName<TOwner>(
        this IAccessibleOwner<TOwner> accessibleOwner,
        string defaultName = "") where TOwner : Control
        => accessibleOwner.Owner?.AccessibleName ?? defaultName;

    /// <summary>
    ///  Gets the owner's text.
    /// </summary>
    public static string GetOwnerText<TOwner>(this IAccessibleOwner<TOwner> accessibleOwner, string defaultText = "")
        where TOwner : Control
        => accessibleOwner.Owner?.Text ?? defaultText;
}
