// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope palette selection.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double pallete reset.
        /// </remarks>
#if DEBUG
        internal class SelectPaletteScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct SelectPaletteScope
#endif
        {
            public HDC HDC { get; }
            public HPALETTE HPalette { get; }

            public SelectPaletteScope(HDC hdc, HPALETTE hpalette, bool forceBackground, bool realizePalette)
            {
                HDC = hdc;
                HPalette = SelectPalette(hdc, hpalette, forceBackground.ToBOOL());
                if (!HPalette.IsNull && realizePalette)
                {
                    RealizePalette((IntPtr)hdc);
                }
            }

            public static SelectPaletteScope HalftonePalette(HDC hdc, bool forceBackground, bool realizePalette)
            {
                if (GetDeviceCaps(hdc, DeviceCapability.BITSPIXEL) > 8)
                {
                    // https://docs.microsoft.com/windows/win32/api/Gdiplusgraphics/nf-gdiplusgraphics-graphics-gethalftonepalette
                    // The purpose of the Graphics::GetHalftonePalette method is to enable GDI+ to produce a better
                    // quality halftone when the display uses 8 bits per pixel. This method allocates a palette of
                    // 256 entries (each of which are 4 bytes a piece).
                    //
                    // Doing this is a bit pointless when the color depth is much higher (the normal scenario). As such
                    // we'll skip doing this unless we see 8bpp or less.

                    return new SelectPaletteScope();
                }

                return new SelectPaletteScope(
                    hdc,
                    (HPALETTE)Graphics.GetHalftonePalette(),
                    forceBackground,
                    realizePalette);
            }

            public static implicit operator HPALETTE(in SelectPaletteScope paletteScope) => paletteScope.HPalette;

            public void Dispose()
            {
                if (!HPalette.IsNull)
                {
                    SelectPalette(HDC, HPalette, bForceBkgd: BOOL.FALSE);
                }

                DisposalTracking.SuppressFinalize(this);
            }

#if DEBUG
            public SelectPaletteScope() { }
#endif
        }
    }
}
