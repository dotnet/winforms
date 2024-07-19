// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum PixelOffsetMode
{
    Invalid = GdiPlus.PixelOffsetMode.PixelOffsetModeInvalid,
    Default = GdiPlus.PixelOffsetMode.PixelOffsetModeDefault,
    HighSpeed = GdiPlus.PixelOffsetMode.PixelOffsetModeHighSpeed,
    HighQuality = GdiPlus.PixelOffsetMode.PixelOffsetModeHighQuality,
    None = GdiPlus.PixelOffsetMode.PixelOffsetModeNone,
    // offset by -0.5, -0.5 for fast anti-alias perf
    Half = GdiPlus.PixelOffsetMode.PixelOffsetModeHalf
}
