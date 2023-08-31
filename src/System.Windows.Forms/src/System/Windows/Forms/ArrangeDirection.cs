// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the direction the system uses to arrange minimized windows.
/// </summary>
[Flags]
public enum ArrangeDirection
{
    /// <summary>
    ///  Arranges vertically, from top to bottom.
    /// </summary>
    Down = PInvoke.ARW_DOWN,

    /// <summary>
    ///  Arranges horizontally, from left to right.
    /// </summary>
    Left = PInvoke.ARW_LEFT,

    /// <summary>
    ///  Arranges horizontally, from right to left.
    /// </summary>
    Right = PInvoke.ARW_RIGHT,

    /// <summary>
    ///  Arranges vertically, from bottom to top.
    /// </summary>
    Up = PInvoke.ARW_UP,
}
