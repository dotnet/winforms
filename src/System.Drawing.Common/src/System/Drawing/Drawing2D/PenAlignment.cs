// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum PenAlignment
{
    Center = GdiPlus.PenAlignment.PenAlignmentCenter,
    Inset = GdiPlus.PenAlignment.PenAlignmentInset,
    Outset = 2,
    Left = 3,
    Right = 4
}
