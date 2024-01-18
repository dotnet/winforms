// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum PenType
{
    SolidColor = GdiPlus.PenType.PenTypeSolidColor,
    HatchFill = GdiPlus.PenType.PenTypeHatchFill,
    TextureFill = GdiPlus.PenType.PenTypeTextureFill,
    PathGradient = GdiPlus.PenType.PenTypePathGradient,
    LinearGradient = GdiPlus.PenType.PenTypeLinearGradient
}
