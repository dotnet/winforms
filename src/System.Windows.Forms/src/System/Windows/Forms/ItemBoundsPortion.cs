// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the display bounds of a ListItem.
    /// </devdoc>
    public enum ItemBoundsPortion
    {
        /// <devdoc>
        /// Both the icon and label portions. In Report View, this includes subitems.
        /// </devdoc>
        Entire = NativeMethods.LVIR_BOUNDS,

        /// <devdoc>
        /// Only the icon portion.
        /// </devdoc>
        Icon = NativeMethods.LVIR_ICON,

        /// <devdoc>
        /// Only the label portion.
        /// </devdoc>
        Label = NativeMethods.LVIR_LABEL,

        /// <devdoc>
        /// Both the icon and label portions. In Report view, this <see langword='Entire'/>.
        /// </devdoc>
        ItemOnly = NativeMethods.LVIR_SELECTBOUNDS,
    }
}
