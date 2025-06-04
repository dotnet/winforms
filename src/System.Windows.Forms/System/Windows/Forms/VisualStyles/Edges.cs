// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.VisualStyles;

[Flags]
public enum Edges
{
    Left = (int)DRAW_EDGE_FLAGS.BF_LEFT,
    Top = (int)DRAW_EDGE_FLAGS.BF_TOP,
    Right = (int)DRAW_EDGE_FLAGS.BF_RIGHT,
    Bottom = (int)DRAW_EDGE_FLAGS.BF_BOTTOM,
    Diagonal = (int)DRAW_EDGE_FLAGS.BF_DIAGONAL,
}
