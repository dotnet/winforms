// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Enum to be used with the drawMenuGlyph function.
    /// </summary>
    public enum MenuGlyph
    {
        /// <summary>
        ///  Draws a submenu arrow.
        /// </summary>
        Arrow = NativeMethods.DFCS_MENUARROW,

        /// <summary>
        ///  Draws a menu checkmark.
        /// </summary>
        Checkmark = NativeMethods.DFCS_MENUCHECK,

        /// <summary>
        ///  Draws a menu bullet.
        /// </summary>
        Bullet = NativeMethods.DFCS_MENUBULLET,

        Min = NativeMethods.DFCS_MENUARROW,
        Max = NativeMethods.DFCS_MENUBULLET,
    }
}
