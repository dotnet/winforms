// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging;

/// <summary>
///  Algorithm for dithering images with a reduced color palette.
/// </summary>
public enum DitherType
{
    /// <summary>
    ///  No dithering is performed. Pixels in the source bitmap are mapped to the nearest color in the palette specified
    ///  by the palette parameter of the
    ///  <see cref="Bitmap.ConvertFormat(PixelFormat, DitherType, PaletteType, ColorPalette?, float)"/>
    ///  method. This algorithm can be used with any palette other than <see cref="PaletteType.Custom"/>.
    /// </summary>
    None = GdiPlus.DitherType.DitherTypeNone,

    /// <summary>
    ///  No dithering is performed. Pixels in the source bitmap are mapped to the nearest color in the palette specified
    ///  by the palette parameter of the
    ///  <see cref="Bitmap.ConvertFormat(PixelFormat, DitherType, PaletteType, ColorPalette?, float)"/> method.
    ///  This algorithm can be used with any palette.
    /// </summary>
    Solid = GdiPlus.DitherType.DitherTypeSolid,

    /// <summary>
    ///  You can use this algorithm to perform dithering based on the colors in one of the standard fixed palettes. You
    ///  can also use this algorithm to convert a bitmap to a 16-bits-per-pixel format that has no palette.
    /// </summary>
    Ordered4x4 = GdiPlus.DitherType.DitherTypeOrdered4x4,

    /// <summary>
    ///  Dithering is performed using the colors in one of the standard fixed palettes.
    /// </summary>
    Ordered8x8 = GdiPlus.DitherType.DitherTypeOrdered8x8,

    /// <summary>
    ///  Dithering is performed using the colors in one of the standard fixed palettes.
    /// </summary>
    Ordered16x16 = GdiPlus.DitherType.DitherTypeOrdered16x16,

    /// <summary>
    ///  Dithering is performed using the colors in one of the standard fixed palettes.
    /// </summary>
    Spiral4x4 = GdiPlus.DitherType.DitherTypeSpiral4x4,

    /// <summary>
    ///  Dithering is performed using the colors in one of the standard fixed palettes.
    /// </summary>
    Spiral8x8 = GdiPlus.DitherType.DitherTypeSpiral8x8,

    /// <summary>
    ///  Dithering is performed using the colors in one of the standard fixed palettes.
    /// </summary>
    DualSpiral4x4 = GdiPlus.DitherType.DitherTypeDualSpiral4x4,

    /// <summary>
    /// Dithering is performed using the colors in one of the standard fixed palettes.
    /// </summary>
    DualSpiral8x8 = GdiPlus.DitherType.DitherTypeDualSpiral8x8,

    /// <summary>
    ///  Dithering is performed based on the palette specified by the palette parameter of the
    ///  <see cref="Bitmap.ConvertFormat(PixelFormat, DitherType, PaletteType, ColorPalette?, float)"/>  method.
    ///  This algorithm can be used with any palette.
    /// </summary>
    ErrorDiffusion = GdiPlus.DitherType.DitherTypeErrorDiffusion
}
#endif
