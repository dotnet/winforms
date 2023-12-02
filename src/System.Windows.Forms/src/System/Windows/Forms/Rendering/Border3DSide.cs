// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the sides of a rectangle to apply a three-dimensional border to.
/// </summary>
[Flags]
public enum Border3DSide
{
    /// <summary>
    ///  A three-dimensional border on the left edge of the control.
    /// </summary>
    Left = (int)DRAW_EDGE_FLAGS.BF_LEFT,

    /// <summary>
    ///  A three-dimensional border on the top edge of the rectangle.
    /// </summary>
    Top = (int)DRAW_EDGE_FLAGS.BF_TOP,

    /// <summary>
    ///  A three-dimensional border on the right side of the rectangle.
    /// </summary>
    Right = (int)DRAW_EDGE_FLAGS.BF_RIGHT,

    /// <summary>
    ///  A three-dimensional border on the bottom side of the rectangle.
    /// </summary>
    Bottom = (int)DRAW_EDGE_FLAGS.BF_BOTTOM,

    /// <summary>
    ///  The interior of the rectangle is filled with the color defined for
    ///  three-dimensional controls instead of the background color for the form.
    /// </summary>
    Middle = (int)DRAW_EDGE_FLAGS.BF_MIDDLE,

    /// <summary>
    ///  A three-dimensional border on all four edges and fill the middle of
    ///  the rectangle with the color defined for three-dimensional controls.
    /// </summary>
    All = Left | Top | Right | Bottom | Middle,
}
