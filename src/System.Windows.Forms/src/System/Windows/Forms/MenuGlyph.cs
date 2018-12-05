// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;



    /// <include file='doc\MenuGlyph.uex' path='docs/doc[@for="MenuGlyph"]/*' />
    /// <devdoc>
    ///     Enum to be used with the drawMenuGlyph function.
    ///
    /// </devdoc>
    public enum MenuGlyph {

        /// <include file='doc\MenuGlyph.uex' path='docs/doc[@for="MenuGlyph.Arrow"]/*' />
        /// <devdoc>
        ///     Draws a submenu arrow.
        /// </devdoc>
        Arrow = NativeMethods.DFCS_MENUARROW,

        /// <include file='doc\MenuGlyph.uex' path='docs/doc[@for="MenuGlyph.Checkmark"]/*' />
        /// <devdoc>
        ///     Draws a menu checkmark.
        /// </devdoc>
        Checkmark = NativeMethods.DFCS_MENUCHECK,

        /// <include file='doc\MenuGlyph.uex' path='docs/doc[@for="MenuGlyph.Bullet"]/*' />
        /// <devdoc>
        ///     Draws a menu bullet.
        /// </devdoc>
        Bullet = NativeMethods.DFCS_MENUBULLET,

        /// <include file='doc\MenuGlyph.uex' path='docs/doc[@for="MenuGlyph.Min"]/*' />
        Min = NativeMethods.DFCS_MENUARROW,
        /// <include file='doc\MenuGlyph.uex' path='docs/doc[@for="MenuGlyph.Max"]/*' />
        Max = NativeMethods.DFCS_MENUBULLET,

    }
}
