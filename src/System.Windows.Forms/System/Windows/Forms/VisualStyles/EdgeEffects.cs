// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.VisualStyles;

[Flags]
public enum EdgeEffects
{
    None = 0,
    FillInterior = (int)DRAW_EDGE_FLAGS.BF_MIDDLE,
    Flat = (int)DRAW_EDGE_FLAGS.BF_FLAT,
    Soft = (int)DRAW_EDGE_FLAGS.BF_SOFT,
    Mono = (int)DRAW_EDGE_FLAGS.BF_MONO,
}
