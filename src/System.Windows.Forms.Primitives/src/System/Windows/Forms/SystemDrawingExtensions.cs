// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
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

        /// <summary>
        ///  Draws lines with the <paramref name="pen"/> using points defined in <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">
        ///  MUST be a mulitple of 4. Each group of 4 represents x1, y1, x2, y2.
        /// </param>
        internal static void DrawLines(this Graphics graphics, Pen pen, ReadOnlySpan<int> lines)
        {
            Debug.Assert((lines.Length % 4) == 0);

            for (int i = 0; i < lines.Length; i += 4)
            {
                graphics.DrawLine(pen, lines[i], lines[i + 1], lines[i + 2], lines[i + 3]);
            }
        }

        /// <summary>
        ///  Mixes two colors.
        /// </summary>
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

        /// <summary>
        ///  Inverts the color.
        /// </summary>
        internal static Color InvertColor(this Color color)
        {
            ARGB argb = color;
            return Color.FromArgb(argb.A, (byte)~argb.R, (byte)~argb.G, (byte)~argb.B);
        }

        /// <summary>
        ///  Creates a <see cref="Pen"/>. If <paramref name="color"/> is a system color, makes a static copy of the
        ///  current color value to avoid having the pen hook itself against <see cref="SystemEvents"/>.
        /// </summary>
        internal static Pen StaticPen(this Color color, float size = 1.0f)
            => color.IsSystemColor
                ? new Pen(Color.FromArgb(color.ToArgb()))
                : new Pen(color);

        /// <summary>
        ///  Creates a <see cref="SolidBrush"/>. If <paramref name="color"/> is a system color, makes a static copy of
        ///  the current color value to avoid having the pen hook itself against <see cref="SystemEvents"/>.
        /// </summary>
        internal static SolidBrush StaticBrush(this Color color)
            => color.IsSystemColor
                ? new SolidBrush(Color.FromArgb(color.ToArgb()))
                : new SolidBrush(color);
    }
}
