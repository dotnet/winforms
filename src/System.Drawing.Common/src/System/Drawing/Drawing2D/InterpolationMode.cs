// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum InterpolationMode
{
    Invalid = GdiPlus.InterpolationMode.InterpolationModeInvalid,
    Default = GdiPlus.InterpolationMode.InterpolationModeDefault,
    Low = GdiPlus.InterpolationMode.InterpolationModeLowQuality,
    High = GdiPlus.InterpolationMode.InterpolationModeHighQuality,
    Bilinear = GdiPlus.InterpolationMode.InterpolationModeBilinear,
    Bicubic = GdiPlus.InterpolationMode.InterpolationModeBicubic,
    NearestNeighbor = GdiPlus.InterpolationMode.InterpolationModeNearestNeighbor,
    HighQualityBilinear = GdiPlus.InterpolationMode.InterpolationModeHighQualityBilinear,
    HighQualityBicubic = GdiPlus.InterpolationMode.InterpolationModeHighQualityBicubic
}
