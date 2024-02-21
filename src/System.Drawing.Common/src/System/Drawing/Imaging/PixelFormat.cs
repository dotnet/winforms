// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging;

/// <summary>
///  Specifies the format of the color data for each pixel in the image.
/// </summary>
public enum PixelFormat
{
    /// <inheritdoc cref="GdiPlus.PixelFormat.Indexed"/>
    Indexed = GdiPlus.PixelFormat.Indexed,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Gdi"/>
    Gdi = GdiPlus.PixelFormat.Gdi,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Alpha"/>
    Alpha = GdiPlus.PixelFormat.Alpha,

    /// <inheritdoc cref="GdiPlus.PixelFormat.PAlpha"/>
    PAlpha = GdiPlus.PixelFormat.PAlpha,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Extended"/>
    Extended = GdiPlus.PixelFormat.Extended,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Canonical"/>
    Canonical = GdiPlus.PixelFormat.Canonical,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Undefined"/>
    Undefined = GdiPlus.PixelFormat.Undefined,

    /// <inheritdoc cref="GdiPlus.PixelFormat.DontCare"/>
    DontCare = GdiPlus.PixelFormat.DontCare,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format1bppIndexed"/>
    Format1bppIndexed = GdiPlus.PixelFormat.Format1bppIndexed,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format4bppIndexed"/>
    Format4bppIndexed = GdiPlus.PixelFormat.Format4bppIndexed,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format8bppIndexed"/>
    Format8bppIndexed = GdiPlus.PixelFormat.Format8bppIndexed,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format16bppGrayScale"/>
    Format16bppGrayScale = GdiPlus.PixelFormat.Format16bppGrayScale,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format16bppRgb555"/>
    Format16bppRgb555 = GdiPlus.PixelFormat.Format16bppRgb555,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format16bppRgb565"/>
    Format16bppRgb565 = GdiPlus.PixelFormat.Format16bppRgb565,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format16bppArgb1555"/>
    Format16bppArgb1555 = GdiPlus.PixelFormat.Format16bppArgb1555,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format24bppRgb"/>
    Format24bppRgb = GdiPlus.PixelFormat.Format24bppRgb,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format32bppRgb"/>
    Format32bppRgb = GdiPlus.PixelFormat.Format32bppRgb,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format32bppArgb"/>
    Format32bppArgb = GdiPlus.PixelFormat.Format32bppArgb,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format32bppPArgb"/>
    Format32bppPArgb = GdiPlus.PixelFormat.Format32bppPArgb,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format48bppRgb"/>
    Format48bppRgb = GdiPlus.PixelFormat.Format48bppRgb,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format64bppArgb"/>
    Format64bppArgb = GdiPlus.PixelFormat.Format64bppArgb,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Format64bppPArgb"/>
    Format64bppPArgb = GdiPlus.PixelFormat.Format64bppPArgb,

    /// <inheritdoc cref="GdiPlus.PixelFormat.Max"/>
    Max = GdiPlus.PixelFormat.Max,
}
