﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Enum to be used with the drawMenuGlyph function.
    /// </devdoc>
    public enum MenuGlyph
    {
        /// <summary>
        /// Draws a submenu arrow.
        /// </devdoc>
        Arrow = NativeMethods.DFCS_MENUARROW,

        /// <summary>
        /// Draws a menu checkmark.
        /// </devdoc>
        Checkmark = NativeMethods.DFCS_MENUCHECK,

        /// <summary>
        /// Draws a menu bullet.
        /// </devdoc>
        Bullet = NativeMethods.DFCS_MENUBULLET,

        Min = NativeMethods.DFCS_MENUARROW,
        Max = NativeMethods.DFCS_MENUBULLET,
    }
}
