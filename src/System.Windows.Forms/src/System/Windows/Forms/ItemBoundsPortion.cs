// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;




    /// <devdoc>
    ///    <para>
    ///       Specifies the display bounds of a ListItem.
    ///    </para>
    /// </devdoc>
    public enum ItemBoundsPortion {


        /// <devdoc>
        ///    <para>
        ///       Both the icon and label
        ///       portions. In Report View, this includes subitems.
        ///
        ///    </para>
        /// </devdoc>
        Entire = NativeMethods.LVIR_BOUNDS,


        /// <devdoc>
        ///    <para>
        ///       Only the icon portion.
        ///    </para>
        /// </devdoc>
        Icon = NativeMethods.LVIR_ICON,


        /// <devdoc>
        ///    <para>
        ///       Only the label portion.
        ///    </para>
        /// </devdoc>
        Label = NativeMethods.LVIR_LABEL,


        /// <devdoc>
        ///    <para>
        ///       Both the icon and label portions. In Report view, this
        ///       does not include subitems. In all other views, this is the same as
        ///    <see langword='Entire'/>
        ///    .
        /// </para>
        /// </devdoc>
        ItemOnly = NativeMethods.LVIR_SELECTBOUNDS,

    }
}
