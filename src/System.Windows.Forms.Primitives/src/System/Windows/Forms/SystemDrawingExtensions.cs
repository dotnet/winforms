// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    internal static class SystemDrawingExtensions
    {
        internal static Gdi32.HBITMAP GetHBITMAP(this Bitmap bitmap) => (Gdi32.HBITMAP)bitmap.GetHbitmap();
        internal static Gdi32.HFONT ToHFONT(this Font font) => (Gdi32.HFONT)font.ToHfont();

        /// <summary>
        ///  Similar to <see cref="Graphics.GetNearestColor(Color)"/>, but this retains the original color if the color
        ///  didn't actually change. This retains the state of the color.
        /// </summary>
        /// <remarks>
        ///  This is important as the color only changes if <paramref name="graphics"/> has a very low color depth. This
        ///  is extremely rare for the normal case of HDC backed Graphics objects. Keeping the original color keeps the
        ///  state that would otherwise be stripped, notably things like <see cref="Color.IsKnownColor"/> which allows
        ///  us to later pull from a the various caches that <see cref="Drawing"/> maintains (saving allocations).
        ///
        ///  Ideally we'd drop checking at all and just support full color drawing to improve performance for the
        ///  expected normal case (more than 8 BITSPIXEL for the HDC).
        /// </remarks>
        internal static Color FindNearestColor(this Graphics graphics, Color color)
        {
            Color newColor = graphics.GetNearestColor(color);
            return newColor.ToArgb() == color.ToArgb() ? color : newColor;
        }

        /// <summary>
        ///  Returns true if the color has any transparency. If false, the color is fully opaque.
        /// </summary>
        internal static bool HasTransparency(this Color color) => color.A != byte.MaxValue;

        /// <summary>
        ///  Returns true if the color is fully transparent.
        /// </summary>
        internal static bool IsFullyTransparent(this Color color) => color.A == 0;

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
        internal static Pen CreateStaticPen(this Color color, DashStyle dashStyle, float size = 1.0f)
        {
            if (dashStyle == DashStyle.Solid)
            {
                // Solid is the default and faster than setting the property
                return color.IsSystemColor
                    ? new Pen((ARGB)color)
                    : new Pen(color);
            }

            return color.IsSystemColor
                ? new Pen(Color.FromArgb(color.ToArgb())) { DashStyle = dashStyle }
                : new Pen(color) { DashStyle = dashStyle };
        }

        /// <summary>
        ///  Not strictly needed (yet), but allows using the same pattern for all pens.
        /// </summary>
        internal static Pen CreateStaticPen(this Brush brush, float width = 1.0f)
            => new Pen(brush, width);

        /// <summary>
        ///  Creates a <see cref="SolidBrush"/>. If <paramref name="color"/> is a system color, makes a static copy of
        ///  the current color value to avoid having the pen hook itself against <see cref="SystemEvents"/>.
        /// </summary>
        internal static SolidBrush CreateStaticBrush(this Color color)
            => color.IsSystemColor
                ? new SolidBrush(Color.FromArgb(color.ToArgb()))
                : new SolidBrush(color);
    }
}
