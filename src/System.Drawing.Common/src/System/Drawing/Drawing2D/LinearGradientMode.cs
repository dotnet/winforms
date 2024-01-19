// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

[Runtime.CompilerServices.TypeForwardedFrom(AssemblyRef.SystemDrawing)]
public enum LinearGradientMode
{
    Horizontal = GdiPlus.LinearGradientMode.LinearGradientModeHorizontal,
    Vertical = GdiPlus.LinearGradientMode.LinearGradientModeVertical,
    ForwardDiagonal = GdiPlus.LinearGradientMode.LinearGradientModeForwardDiagonal,
    BackwardDiagonal = GdiPlus.LinearGradientMode.LinearGradientModeBackwardDiagonal
}
