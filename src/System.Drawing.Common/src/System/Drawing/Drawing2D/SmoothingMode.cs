// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum SmoothingMode
{
    Invalid = GdiPlus.SmoothingMode.SmoothingModeInvalid,
    Default = GdiPlus.SmoothingMode.SmoothingModeDefault,
    HighSpeed = GdiPlus.SmoothingMode.SmoothingModeHighSpeed,
    HighQuality = GdiPlus.SmoothingMode.SmoothingModeHighQuality,
    None = GdiPlus.SmoothingMode.SmoothingModeNone,
    AntiAlias = GdiPlus.SmoothingMode.SmoothingModeAntiAlias
}
