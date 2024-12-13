// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Helper to scope palette selection.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement. If you must pass this around, always pass
///   by <see langword="ref" /> to avoid duplicating the handle and risking a double palette reset.
///  </para>
/// </remarks>
#if DEBUG
internal class SelectPaletteScope : DisposalTracking.Tracker, IDisposable
#else
internal readonly ref struct SelectPaletteScope
#endif
{
    public HDC HDC { get; }
    public HPALETTE HPALETTE { get; }

    public SelectPaletteScope(HDC hdc, HPALETTE hpalette, bool forceBackground, bool realizePalette)
    {
        HDC = hdc;
        HPALETTE = PInvokeCore.SelectPalette(hdc, hpalette, forceBackground);
        if (!HPALETTE.IsNull && realizePalette)
        {
            PInvokeCore.RealizePalette(hdc);
        }
    }

    public static implicit operator HPALETTE(in SelectPaletteScope paletteScope) => paletteScope.HPALETTE;

    public void Dispose()
    {
        if (!HPALETTE.IsNull)
        {
            PInvokeCore.SelectPalette(HDC, HPALETTE, bForceBkgd: false);
        }

        DisposalTracking.SuppressFinalize(this);
    }

#if DEBUG
    public SelectPaletteScope() { }
#endif
}
