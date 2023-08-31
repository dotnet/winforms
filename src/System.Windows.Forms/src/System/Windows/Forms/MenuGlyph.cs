// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Enum to be used with the drawMenuGlyph function.
/// </summary>
public enum MenuGlyph
{
    /// <summary>
    ///  Draws a submenu arrow.
    /// </summary>
    Arrow = (int)DFCS_STATE.DFCS_MENUARROW,

    /// <summary>
    ///  Draws a menu checkmark.
    /// </summary>
    Checkmark = (int)DFCS_STATE.DFCS_MENUCHECK,

    /// <summary>
    ///  Draws a menu bullet.
    /// </summary>
    Bullet = (int)DFCS_STATE.DFCS_MENUBULLET,

    Min = (int)DFCS_STATE.DFCS_MENUARROW,
    Max = (int)DFCS_STATE.DFCS_MENUBULLET,
}
