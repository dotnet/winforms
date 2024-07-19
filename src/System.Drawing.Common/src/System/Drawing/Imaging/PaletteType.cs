// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging;

/// <summary>
///  Color palette types.
/// </summary>
public enum PaletteType
{
    /// <summary>
    ///  An arbitrary custom palette specified by the caller.
    /// </summary>
    Custom = GdiPlus.PaletteType.PaletteTypeCustom,

    /// <summary>
    ///  A palette that has two colors. This palette type is suitable for bitmaps that store 1 bit per pixel.
    /// </summary>
    FixedBlackAndWhite = GdiPlus.PaletteType.PaletteTypeFixedBW,

    /// <summary>
    ///  A palette based on two intensities each (off or full) for the red, green, and blue channels. Also contains the
    ///  16 colors of the system palette. Because all eight of the on/off combinations of red, green, and blue are
    ///  already in the system palette, this palette is the same as the system palette. This palette type is suitable
    ///  for bitmaps that store 4 bits per pixel.
    /// </summary>
    FixedHalftone8 = GdiPlus.PaletteType.PaletteTypeFixedHalftone8,

    /// <summary>
    ///  A palette based on three intensities each for the red, green, and blue channels. Also contains the 16 colors of
    ///  the system palette. Eight of the 16 system palette colors are among the 27 three-intensity combinations of red,
    ///  green, and blue, so the total number of colors in the palette is 35. If the palette also includes the transparent
    ///  color, the total number of colors is 36.
    /// </summary>
    FixedHalftone27 = GdiPlus.PaletteType.PaletteTypeFixedHalftone27,

    /// <summary>
    ///  A palette based on four intensities each for the red, green, and blue channels. Also contains the 16 colors of
    ///  the system palette. Eight of the 16 system palette colors are among the 64 four-intensity combinations of red,
    ///  green, and blue, so the total number of colors in the palette is 72. If the palette also includes the transparent
    ///  color, the total number of colors is 73.
    /// </summary>
    FixedHalftone64 = GdiPlus.PaletteType.PaletteTypeFixedHalftone64,

    /// <summary>
    ///  A palette based on five intensities each for the red, green, and blue channels. Also contains the 16 colors of
    ///  the system palette. Eight of the 16 system palette colors are among the 125 five-intensity combinations of red,
    ///  green, and blue, so the total number of colors in the palette is 133. If the palette also includes the transparent
    ///  color, the total number of colors is 134.
    /// </summary>
    FixedHalftone125 = GdiPlus.PaletteType.PaletteTypeFixedHalftone125,

    /// <summary>
    ///  A palette based on six intensities each for the red, green, and blue channels. Also contains the 16 colors of
    ///  the system palette. Eight of the 16 system palette colors are among the 216 six-intensity combinations of red,
    ///  green, and blue, so the total number of colors in the palette is 224. If the palette also includes the transparent
    ///  color, the total number of colors is 225. This palette is sometimes called the Windows halftone palette or
    ///  the Web palette.
    /// </summary>
    FixedHalftone216 = GdiPlus.PaletteType.PaletteTypeFixedHalftone216,

    /// <summary>
    ///  A palette based on 6 intensities of red, 7 intensities of green, and 6 intensities of blue. The system palette
    ///  is not included. The total number of colors is 252. If the palette also includes the transparent color, the
    ///  total number of colors is 253.
    /// </summary>
    FixedHalftone252 = GdiPlus.PaletteType.PaletteTypeFixedHalftone252,

    /// <summary>
    ///  A palette based on 8 intensities of red, 8 intensities of green, and 4 intensities of blue. The system palette
    ///  is not included. The total number of colors is 256. If the transparent color is included in this palette, it
    ///  must replace one of the RGB combinations.
    /// </summary>
    FixedHalftone256 = GdiPlus.PaletteType.PaletteTypeFixedHalftone256
}
#endif
