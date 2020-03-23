// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the display bounds of a ListItem.
    /// </summary>
    public enum ItemBoundsPortion
    {
        /// <summary>
        ///  Both the icon and label portions. In Report View, this includes subitems.
        /// </summary>
        Entire = (int)LVIR.BOUNDS,

        /// <summary>
        ///  Only the icon portion.
        /// </summary>
        Icon = (int)LVIR.ICON,

        /// <summary>
        ///  Only the label portion.
        /// </summary>
        Label = (int)LVIR.LABEL,

        /// <summary>
        ///  Both the icon and label portions. In Report view, this <see cref='Entire'/>.
        /// </summary>
        ItemOnly = (int)LVIR.SELECTBOUNDS,
    }
}
