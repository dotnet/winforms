// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging;

/// <summary>
///  Specifies the type of color data in the system palette. The data can be color data with alpha, grayscale only,
///  or halftone data.
/// </summary>
[Flags]
public enum PaletteFlags
{
    /// <summary>
    ///  Specifies alpha data.
    /// </summary>
    HasAlpha = GdiPlus.PaletteFlags.PaletteFlagsHasAlpha,

    /// <summary>
    ///  Specifies grayscale data.
    /// </summary>
    GrayScale = GdiPlus.PaletteFlags.PaletteFlagsGrayScale,

    /// <summary>
    ///  Specifies halftone data.
    /// </summary>
    Halftone = GdiPlus.PaletteFlags.PaletteFlagsHalftone
}
