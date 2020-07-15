// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.CompilerServices;
using static Interop;

namespace System.Windows.Forms
{
    internal static class SystemDrawingExtensions
    {
        internal static Gdi32.HBITMAP GetHBITMAP(this Bitmap bitmap) => (Gdi32.HBITMAP)bitmap.GetHbitmap();
        internal static Gdi32.HFONT ToHFONT(this Font font) => (Gdi32.HFONT)font.ToHfont();
        internal static bool HasTransparency(this Color color) => color.A != byte.MaxValue;

        internal static void ThrowIfFailed(this GdiPlus.GpStatus status)
        {
            if (status != GdiPlus.GpStatus.Ok)
            {
                throw new InvalidOperationException(status.ToString());
            }
        }

        [SkipLocalsInit]
        internal static Color MixColor(this Color color1, Color color2)
        {
            // Some colors look up their values on every property access so we'll get the value out
            ARGB argb1 = color1;
            ARGB argb2 = color2;

            return Color.FromArgb(
                (argb1.A + argb2.A) / 2,
                (argb1.R + argb2.R) / 2,
                (argb1.G + argb2.G) / 2,
                (argb1.B + argb2.B) / 2);
        }
    }
}
