﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies how items align in the <see cref='System.Windows.Forms.ListView'/>.
    /// </devdoc>
    public enum ListViewAlignment
    {
        /// <devdoc>
        /// When the user moves an item, it remains where it is dropped.
        /// </devdoc>
        Default = NativeMethods.LVA_DEFAULT,

        /// <devdoc>
        /// Items are aligned to the top of the <see cref='System.Windows.Forms.ListView'/> control.
        /// </devdoc>
        Top = NativeMethods.LVA_ALIGNTOP,

        /// <devdoc>
        /// Items are aligned to the left of the <see cref='System.Windows.Forms.ListView'/> control.
        /// </devdoc>
        Left = NativeMethods.LVA_ALIGNLEFT,

        /// <devdoc>
        /// Items are aligned to an invisible grid in the control. When the user
        /// moves an item, it moves to the closest juncture in the grid.
        /// </devdoc>
        SnapToGrid = NativeMethods.LVA_SNAPTOGRID,
    }
}
