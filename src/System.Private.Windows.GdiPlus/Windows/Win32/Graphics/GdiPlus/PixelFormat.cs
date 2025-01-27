// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.GdiPlus;

internal enum PixelFormat
{
    /// <summary>
    ///  Specifies that pixel data contains color indexed values which means they are an index to colors in the
    ///  system color table, as opposed to individual color values.
    /// </summary>
    Indexed = (int)PInvokeGdiPlus.PixelFormatIndexed,

    /// <summary>
    ///  Specifies that pixel data contains GDI colors.
    /// </summary>
    Gdi = (int)PInvokeGdiPlus.PixelFormatGDI,

    /// <summary>
    ///  Specifies that pixel data contains alpha values that are not pre-multiplied.
    /// </summary>
    Alpha = (int)PInvokeGdiPlus.PixelFormatAlpha,

    /// <summary>
    ///  Specifies that pixel format contains pre-multiplied alpha values.
    /// </summary>
    PAlpha = (int)PInvokeGdiPlus.PixelFormatPAlpha,

    /// <summary>
    ///  Specifies that pixel format contains extended color values of 16 bits per channel.
    /// </summary>
    Extended = (int)PInvokeGdiPlus.PixelFormatExtended,

    Canonical = (int)PInvokeGdiPlus.PixelFormatCanonical,

    /// <summary>
    ///  Specifies that pixel format is undefined.
    /// </summary>
    Undefined = (int)PInvokeGdiPlus.PixelFormatUndefined,

    /// <summary>
    ///  Specifies that pixel format doesn't matter.
    /// </summary>
    DontCare = (int)PInvokeGdiPlus.PixelFormatDontCare,

    /// <summary>
    ///  Specifies that pixel format is 1 bit per pixel indexed color. The color table therefore has two colors in it.
    /// </summary>
    Format1bppIndexed = 1 | (1 << 8) | Indexed | Gdi,

    /// <summary>
    ///  Specifies that pixel format is 4 bits per pixel indexed color. The color table therefore has 16 colors in it.
    /// </summary>
    Format4bppIndexed = 2 | (4 << 8) | Indexed | Gdi,

    /// <summary>
    ///  Specifies that pixel format is 8 bits per pixel indexed color. The color table therefore has 256 colors in it.
    /// </summary>
    Format8bppIndexed = 3 | (8 << 8) | Indexed | Gdi,

    /// <summary>
    ///  Specifies that pixel format is 16 bits per pixel. The color information specifies 65536 shades of gray.
    /// </summary>
    Format16bppGrayScale = 4 | (16 << 8) | Extended,

    /// <summary>
    ///  Specifies that pixel format is 16 bits per pixel. The color information specifies 32768 shades of color of
    ///  which 5 bits are red, 5 bits are green and 5 bits are blue.
    /// </summary>
    Format16bppRgb555 = 5 | (16 << 8) | Gdi,

    Format16bppRgb565 = 6 | (16 << 8) | Gdi,

    /// <summary>
    ///  Specifies that pixel format is 16 bits per pixel. The color information specifies 32768 shades of color of
    ///  which 5 bits are red, 5 bits are green, 5 bits are blue and 1 bit is alpha.
    /// </summary>
    Format16bppArgb1555 = 7 | (16 << 8) | Alpha | Gdi,

    /// <summary>
    ///  Specifies that pixel format is 24 bits per pixel. The color information specifies 16777216 shades of color
    ///  of which 8 bits are red, 8 bits are green and 8 bits are blue.
    /// </summary>
    Format24bppRgb = 8 | (24 << 8) | Gdi,

    /// <summary>
    ///  Specifies that pixel format is 24 bits per pixel. The color information specifies 16777216 shades of color
    ///  of which 8 bits are red, 8 bits are green and 8 bits are blue.
    /// </summary>
    Format32bppRgb = 9 | (32 << 8) | Gdi,

    /// <summary>
    ///  Specifies that pixel format is 32 bits per pixel. The color information specifies 16777216 shades of color
    ///  of which 8 bits are red, 8 bits are green and 8 bits are blue. The 8 additional bits are alpha bits.
    /// </summary>
    Format32bppArgb = 10 | (32 << 8) | Alpha | Gdi | Canonical,

    /// <summary>
    ///  Specifies that pixel format is 32 bits per pixel. The color information specifies 16777216 shades of color
    ///  of which 8 bits are red, 8 bits are green and 8 bits are blue. The 8 additional bits are pre-multiplied alpha bits.
    /// </summary>
    Format32bppPArgb = 11 | (32 << 8) | Alpha | PAlpha | Gdi,

    /// <summary>
    ///  Specifies that pixel format is 48 bits per pixel. The color information specifies 16777216 shades of color
    ///  of which 8 bits are red, 8 bits are green and 8 bits are blue. The 8 additional bits are alpha bits.
    /// </summary>
    Format48bppRgb = 12 | (48 << 8) | Extended,

    /// <summary>
    ///  Specifies pixel format is 64 bits per pixel. The color information specifies 16777216 shades of color of
    ///  which 16 bits are red, 16 bits are green and 16 bits are blue. The 16 additional bits are alpha bits.
    /// </summary>
    Format64bppArgb = 13 | (64 << 8) | Alpha | Canonical | Extended,

    /// <summary>
    ///  Specifies that pixel format is 64 bits per pixel. The color information specifies 16777216 shades of color
    ///  of which 16 bits are red, 16 bits are green and 16 bits are blue. The 16 additional bits are pre-multiplied
    ///  alpha bits.
    /// </summary>
    Format64bppPArgb = 14 | (64 << 8) | Alpha | PAlpha | Extended,

    /// <summary>
    ///  Specifies that pixel format is 64 bits per pixel. The color information specifies 16777216 shades of color
    ///  of which 16 bits are red, 16 bits are green and 16 bits are blue. The 16 additional bits are alpha bits.
    /// </summary>
    Max = 15,
}
