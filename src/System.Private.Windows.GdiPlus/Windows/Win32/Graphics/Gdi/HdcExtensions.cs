// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Graphics.GdiPlus;

namespace Windows.Win32.Graphics.Gdi;

internal static class HdcExtensions
{
    private static HPALETTE s_halftonePalette;

    /// <inheritdoc cref="HalftonePalette{T}(T, bool, bool)"/>
    public static SelectPaletteScope HalftonePalette(this GetDcScope hdc, bool forceBackground, bool realizePalette) =>
        HalftonePalette(hdc.HDC, forceBackground, realizePalette);

    /// <summary>
    ///  Uses the GDI+ halftone palette for the given <paramref name="hdc"/> if the color depth is 8 bpp or less.
    /// </summary>
    public static SelectPaletteScope HalftonePalette<T>(this T hdc, bool forceBackground, bool realizePalette)
        where T : IHandle<HDC>
    {
        if (PInvokeCore.GetDeviceCaps(hdc.Handle, GET_DEVICE_CAPS_INDEX.BITSPIXEL) > 8)
        {
            // https://docs.microsoft.com/windows/win32/api/Gdiplusgraphics/nf-gdiplusgraphics-graphics-gethalftonepalette
            // The purpose of the Graphics::GetHalftonePalette method is to enable GDI+ to produce a better
            // quality halftone when the display uses 8 bits per pixel. This method allocates a palette of
            // 256 entries (each of which are 4 bytes a piece).
            //
            // Doing this is a bit pointless when the color depth is much higher (the normal scenario). As such
            // we'll skip doing this unless we see 8bpp or less.
#if DEBUG
            return new SelectPaletteScope();
#else
            return default;
#endif
        }

        if (s_halftonePalette.IsNull)
        {
            GdiPlusInitialization.EnsureInitialized();
            s_halftonePalette = PInvokeGdiPlus.GdipCreateHalftonePalette();
        }

        return new SelectPaletteScope(
            hdc.Handle,
            s_halftonePalette,
            forceBackground,
            realizePalette);
    }
}
