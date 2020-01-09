// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

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
        Arrow = (int)User32.DFCS.MENUARROW,

        /// <summary>
        ///  Draws a menu checkmark.
        /// </summary>
        Checkmark = (int)User32.DFCS.MENUCHECK,

        /// <summary>
        ///  Draws a menu bullet.
        /// </summary>
        Bullet = (int)User32.DFCS.MENUBULLET,

        Min = (int)User32.DFCS.MENUARROW,
        Max = (int)User32.DFCS.MENUBULLET,
    }
}
