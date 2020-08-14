// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Drawing helpers for device contexts. All methods restore original state unless otherwise specified.
    /// </summary>
    internal static class DeviceContextExtensions
    {
        internal static void DrawRectangle(this DeviceContextHdcScope hdc, Rectangle rectangle, Gdi32.HPEN hpen)
            => DrawRectangle(hdc.HDC, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, hpen);

        internal static void DrawRectangle(
            this DeviceContextHdcScope hdc,
            int left,
            int top,
            int right,
            int bottom,
            Gdi32.HPEN hpen)
            => DrawRectangle(hdc.HDC, left, top, right, bottom, hpen);

        internal static void DrawRectangle(
            this Gdi32.HDC hdc,
            int left,
            int top,
            int right,
            int bottom,
            Gdi32.HPEN hpen)
        {
            using var penScope = new Gdi32.SelectObjectScope(hdc, hpen);
            using var ropScope = new Gdi32.SetRop2Scope(hdc, Gdi32.R2.COPYPEN);
            using var brushScope = new Gdi32.SelectObjectScope(hdc, Gdi32.GetStockObject(Gdi32.StockObject.NULL_BRUSH));

            Gdi32.Rectangle(hdc, left, top, right, bottom);
        }

        internal static void FillRectangle(this DeviceContextHdcScope hdc, Rectangle rectangle, Gdi32.HBRUSH hbrush)
            => FillRectangle(hdc.HDC, rectangle, hbrush);

        internal static void FillRectangle(this Gdi32.HDC hdc, Rectangle rectangle, Gdi32.HBRUSH hbrush)
        {
            Debug.Assert(!hbrush.IsNull);
            RECT rect = rectangle;
            User32.FillRect(
                hdc,
                ref rect,
                hbrush);
        }

        internal static void DrawLine(this DeviceContextHdcScope hdc, Gdi32.HPEN hpen, Point p1, Point p2)
            => DrawLine(hdc.HDC, hpen, p1.X, p1.Y, p2.X, p2.Y);

        internal static void DrawLine(this Gdi32.HDC hdc, Gdi32.HPEN hpen, Point p1, Point p2)
            => DrawLine(hdc, hpen, p1.X, p1.Y, p2.X, p2.Y);

        internal unsafe static void DrawLine(this DeviceContextHdcScope hdc, Gdi32.HPEN hpen, int x1, int y1, int x2, int y2)
            => DrawLine(hdc.HDC, hpen, x1, y1, x2, y2);

        internal unsafe static void DrawLine(this Gdi32.HDC hdc, Gdi32.HPEN hpen, int x1, int y1, int x2, int y2)
        {
            ReadOnlySpan<int> lines = stackalloc int[] { x1, y1, x2, y2 };
            DrawLines(hdc, hpen, lines);
        }

        /// <summary>
        ///  Draws lines with the <paramref name="hpen"/> using points defined in <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">
        ///  MUST be a mulitple of 4. Each group of 4 represents x1, y1, x2, y2.
        /// </param>
        internal unsafe static void DrawLines(this DeviceContextHdcScope hdc, Gdi32.HPEN hpen, ReadOnlySpan<int> lines)
            => DrawLines(hdc.HDC, hpen, lines);

        /// <summary>
        ///  Draws lines with the <paramref name="hpen"/> using points defined in <paramref name="lines"/>.
        /// </summary>
        /// <param name="lines">
        ///  MUST be a mulitple of 4. Each group of 4 represents x1, y1, x2, y2.
        /// </param>
        internal unsafe static void DrawLines(this Gdi32.HDC hdc, Gdi32.HPEN hpen, ReadOnlySpan<int> lines)
        {
            Debug.Assert((lines.Length % 4) == 0);

            using var ropScope = new Gdi32.SetRop2Scope(hdc, Gdi32.R2.COPYPEN);
            using var bkScope = new Gdi32.SetBkModeScope(hdc, Gdi32.BKMODE.TRANSPARENT);
            using var selection = new Gdi32.SelectObjectScope(hdc, hpen);

            Point oldPoint = new Point();

            for (int i = 0; i < lines.Length; i += 4)
            {
                Gdi32.MoveToEx(hdc, lines[i], lines[i + 1], &oldPoint);
                Gdi32.LineTo(hdc, lines[i + 2], lines[i + 3]);
                Gdi32.MoveToEx(hdc, oldPoint.X, oldPoint.Y, null);
            }
        }

        internal static Color FindNearestColor(this DeviceContextHdcScope hdc, Color color)
            => FindNearestColor(hdc.HDC, color);

        /// <summary>
        ///  Calls <see cref="Gdi32.GetNearestColor(Gdi32.HDC, int)"/> to get the nearest color for the given
        ///  <paramref name="color"/>. Returns the original color if the color didn't actually change, retaining
        ///  the state of the color.
        /// </summary>
        /// <remarks>
        ///  This is important as the color only changes if <paramref name="hdc"/> is a very low color depth. This
        ///  is extremely rare for the normal case of HDC backed Graphics objects. Keeping the original color keeps the
        ///  state that would otherwise be stripped, notably things like <see cref="Color.IsKnownColor"/> which allows
        ///  us to later pull from a the various caches that <see cref="Drawing"/> maintains (saving allocations).
        ///
        ///  Ideally we'd drop checking at all and just support full color drawing to improve performance for the
        ///  expected normal case (more than 8 BITSPIXEL for the HDC).
        /// </remarks>
        internal static Color FindNearestColor(this Gdi32.HDC hdc, Color color)
        {
            Color newColor = ColorTranslator.FromWin32(Gdi32.GetNearestColor(hdc, ColorTranslator.ToWin32(color)));
            return newColor.ToArgb() == color.ToArgb() ? color : newColor;
        }

        internal static Graphics CreateGraphics(this Gdi32.HDC hdc) => Graphics.FromHdcInternal(hdc.Handle);
        internal static Graphics CreateGraphics(this Gdi32.CreateDcScope hdc) => Graphics.FromHdcInternal(hdc.HDC.Handle);
        internal static Graphics CreateGraphics(this User32.GetDcScope hdc) => Graphics.FromHdcInternal(hdc.HDC.Handle);

        internal static void DrawAndFillEllipse(
            this DeviceContextHdcScope hdc,
            Gdi32.HPEN pen,
            Gdi32.HBRUSH brush,
            Rectangle bounds)
            => DrawAndFillEllipse(hdc.HDC, pen, brush, bounds.Left, bounds.Top, bounds.Right, bounds.Bottom);

        internal static void DrawAndFillEllipse(
            this Gdi32.HDC hdc,
            Gdi32.HPEN pen,
            Gdi32.HBRUSH brush,
            int left,
            int top,
            int right,
            int bottom)
        {
            using var penSelection = pen.IsNull ? default : new Gdi32.SelectObjectScope(hdc, pen);
            using var brushSelection = brush.IsNull ? default : new Gdi32.SelectObjectScope(hdc, brush);

            Gdi32.Ellipse(hdc, left, top, right, bottom);
        }

        internal static void FillRectangle(this User32.GetDcScope hdc, Gdi32.HBRUSH hbrush, Rectangle rectangle)
            => FillRectangle(hdc.HDC, hbrush, rectangle);

        internal static void FillRectangle(this Gdi32.HDC hdc, Gdi32.HBRUSH hbrush, Rectangle rectangle)
        {
            Debug.Assert(!hbrush.IsNull, "HBRUSH is null");
            RECT rect = rectangle;
            User32.FillRect(
                hdc,
                ref rect,
                hbrush);
        }

        /// <summary>
        ///  Convert the <paramref name="deviceContext"/> into a <see cref="Graphics"/> object if possible.
        /// </summary>
        /// <param name="create">
        ///  Will create the <see cref="Graphics"/> if possible and it is not already created.
        /// </param>
        /// <remarks>
        ///  Do NOT dispose of the <see cref="Graphics"/> object. If it was created, the object creating it owns it.
        /// </remarks>
        internal static Graphics? TryGetGraphics(this IDeviceContext deviceContext, bool create = false)
            => deviceContext switch
            {
                Graphics graphics => graphics,
                IGraphicsHdcProvider provider => provider.GetGraphics(create),
                _ => AssertNoGraphics(create)
            };

        private static Graphics? AssertNoGraphics(bool create)
        {
            Debug.Assert(!create, "Couldn't get Graphics");
            return null;
        }
    }
}
