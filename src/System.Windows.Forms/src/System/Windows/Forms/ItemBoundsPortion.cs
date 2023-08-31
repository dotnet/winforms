// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the display bounds of a ListItem.
/// </summary>
public enum ItemBoundsPortion
{
    /// <summary>
    ///  Both the icon and label portions. In Report View, this includes subitems.
    /// </summary>
    Entire = (int)PInvoke.LVIR_BOUNDS,

    /// <summary>
    ///  Only the icon portion.
    /// </summary>
    Icon = (int)PInvoke.LVIR_ICON,

    /// <summary>
    ///  Only the label portion.
    /// </summary>
    Label = (int)PInvoke.LVIR_LABEL,

    /// <summary>
    ///  Both the icon and label portions. In Report view, this <see cref="Entire"/>.
    /// </summary>
    ItemOnly = (int)PInvoke.LVIR_SELECTBOUNDS,
}
