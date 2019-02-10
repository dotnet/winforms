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
    ///     Enum to be used with the drawMenuGlyph function.
    ///
    /// </devdoc>
    public enum MenuGlyph {

        /// <devdoc>
        ///     Draws a submenu arrow.
        /// </devdoc>
        Arrow = NativeMethods.DFCS_MENUARROW,

        /// <devdoc>
        ///     Draws a menu checkmark.
        /// </devdoc>
        Checkmark = NativeMethods.DFCS_MENUCHECK,

        /// <devdoc>
        ///     Draws a menu bullet.
        /// </devdoc>
        Bullet = NativeMethods.DFCS_MENUBULLET,

        Min = NativeMethods.DFCS_MENUARROW,
        Max = NativeMethods.DFCS_MENUBULLET,

    }
}
