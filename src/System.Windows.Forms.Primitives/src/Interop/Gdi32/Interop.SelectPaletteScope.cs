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
        public readonly ref struct SelectPaletteScope
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
                => new SelectPaletteScope(hdc, (HPALETTE)Graphics.GetHalftonePalette(), forceBackground, realizePalette);

            public static implicit operator HPALETTE(in SelectPaletteScope paletteScope) => paletteScope.HPalette;

            public void Dispose()
            {
                if (!HPalette.IsNull)
                {
                    SelectPalette(HDC, HPalette, bForceBkgd: BOOL.FALSE);
                }
            }
        }
    }
}
