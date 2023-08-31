// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the starting position that the system uses to arrange minimized
///  windows.
/// </summary>
[Flags]
public enum ArrangeStartingPosition
{
    /// <summary>
    ///  Starts at the lower-left corner of the screen, which is the default position.
    /// </summary>
    BottomLeft = MINIMIZEDMETRICS_ARRANGE.ARW_BOTTOMLEFT,

    /// <summary>
    ///  Starts at the lower-right corner of the screen.
    /// </summary>
    BottomRight = MINIMIZEDMETRICS_ARRANGE.ARW_BOTTOMRIGHT,

    /// <summary>
    ///  Hides minimized windows by moving them off the visible area of the
    ///  screen.
    /// </summary>
    Hide = PInvoke.ARW_HIDE,

    /// <summary>
    ///  Starts at the upper-left corner of the screen.
    /// </summary>
    TopLeft = MINIMIZEDMETRICS_ARRANGE.ARW_TOPLEFT,

    /// <summary>
    ///  Starts at the upper-right corner of the screen.
    /// </summary>
    TopRight = MINIMIZEDMETRICS_ARRANGE.ARW_TOPRIGHT,
}
