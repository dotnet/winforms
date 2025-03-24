// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the type of scroll arrow to create on a scroll bar.
/// </summary>
public enum ScrollButton
{
    /// <summary>
    ///  A down-scroll arrow.
    /// </summary>
    Down = (int)DFCS_STATE.DFCS_SCROLLDOWN,

    /// <summary>
    ///  A left-scroll arrow.
    /// </summary>
    Left = (int)DFCS_STATE.DFCS_SCROLLLEFT,

    /// <summary>
    ///  A right-scroll arrow.
    /// </summary>
    Right = (int)DFCS_STATE.DFCS_SCROLLRIGHT,

    /// <summary>
    ///  An up-scroll arrow.
    /// </summary>
    Up = (int)DFCS_STATE.DFCS_SCROLLUP,

    Min = (int)DFCS_STATE.DFCS_SCROLLUP,

    Max = (int)DFCS_STATE.DFCS_SCROLLRIGHT,
}
