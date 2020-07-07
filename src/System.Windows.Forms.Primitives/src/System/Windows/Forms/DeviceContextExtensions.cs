// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            => DrawRectangle(hdc.HDC, rectangle, hpen);

        internal static void DrawRectangle(this Gdi32.CreateDcScope hdc, Rectangle rectangle, Gdi32.HPEN hpen)
            => DrawRectangle(hdc.HDC, rectangle, hpen);

        internal static void DrawRectangle(this User32.GetDcScope hdc, Rectangle rectangle, Gdi32.HPEN hpen)
            => DrawRectangle(hdc.HDC, rectangle, hpen);

        internal static void DrawRectangle(this Gdi32.HDC hdc, Rectangle rectangle, Gdi32.HPEN hpen)
        {
            using var penScope = new Gdi32.SelectObjectScope(hdc, hpen);
            using var ropScope = new Gdi32.SetRop2Scope(hdc, Gdi32.R2.COPYPEN);
            using var brushScope = new Gdi32.SelectObjectScope(hdc, Gdi32.GetStockObject(Gdi32.StockObject.HOLLOW_BRUSH));

            Gdi32.Rectangle(hdc, rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);
        }

        internal static void FillRectangle(this DeviceContextHdcScope hdc, Rectangle rectangle, Gdi32.HBRUSH hbrush)
            => FillRectangle(hdc.HDC, rectangle, hbrush);

        internal static void FillRectangle(this Gdi32.CreateDcScope hdc, Rectangle rectangle, Gdi32.HBRUSH hbrush)
            => FillRectangle(hdc.HDC, rectangle, hbrush);

        internal static void FillRectangle(this User32.GetDcScope hdc, Rectangle rectangle, Gdi32.HBRUSH hbrush)
            => FillRectangle(hdc.HDC, rectangle, hbrush);

        internal static void FillRectangle(this Gdi32.HDC hdc, Rectangle rectangle, Gdi32.HBRUSH hbrush)
        {
            RECT rect = rectangle;
            User32.FillRect(
                hdc,
                ref rect,
                hbrush);
        }

        internal static void DrawLine(this DeviceContextHdcScope hdc, Gdi32.HPEN pen, int x1, int y1, int x2, int y2)
            => DrawLine(hdc.HDC, pen, x1, y1, x2, y2);

        internal static void DrawLine(this Gdi32.CreateDcScope hdc, Gdi32.HPEN pen, int x1, int y1, int x2, int y2)
            => DrawLine(hdc.HDC, pen, x1, y1, x2, y2);

        internal static void DrawLine(this User32.GetDcScope hdc, Gdi32.HPEN pen, int x1, int y1, int x2, int y2)
            => DrawLine(hdc.HDC, pen, x1, y1, x2, y2);

        internal unsafe static void DrawLine(this Gdi32.HDC hdc, Gdi32.HPEN pen, int x1, int y1, int x2, int y2)
        {
            using var ropScope = new Gdi32.SetRop2Scope(hdc, Gdi32.R2.COPYPEN);
            using var bkScope = new Gdi32.SetBkModeScope(hdc, Gdi32.BKMODE.TRANSPARENT);
            using var selection = new Gdi32.SelectObjectScope(hdc, pen);

            Point oldPoint = new Point();
            Gdi32.MoveToEx(hdc, x1, y1, &oldPoint);
            Gdi32.LineTo(hdc, x2, y2);
            Gdi32.MoveToEx(hdc, oldPoint.X, oldPoint.Y, &oldPoint);
        }

        internal static Color GetNearestColor(this DeviceContextHdcScope hdc, Color color)
            => GetNearestColor(hdc.HDC, color);

        internal static Color GetNearestColor(this Gdi32.CreateDcScope hdc, Color color)
            => GetNearestColor(hdc.HDC, color);

        internal static Color GetNearestColor(this User32.GetDcScope hdc, Color color)
            => GetNearestColor(hdc.HDC, color);

        internal static Color GetNearestColor(this Gdi32.HDC hdc, Color color)
            => ColorTranslator.FromWin32(Gdi32.GetNearestColor(hdc, ColorTranslator.ToWin32(color)));

        internal static Graphics CreateGraphics(this Gdi32.HDC hdc) => Graphics.FromHdcInternal(hdc.Handle);
        internal static Graphics CreateGraphics(this Gdi32.CreateDcScope hdc) => Graphics.FromHdcInternal(hdc.HDC.Handle);
        internal static Graphics CreateGraphics(this User32.GetDcScope hdc) => Graphics.FromHdcInternal(hdc.HDC.Handle);

        /// <summary>
        ///  Get the number of pixels per logical inch along the device width. In a system with multiple display
        ///  monitors, this value is always from the primary display.
        /// </summary>
        internal static int GetDpiX(this Gdi32.HDC hdc) => Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCapability.LOGPIXELSX);

        /// <summary>
        ///  Get the number of pixels per logical inch along the device (screen) height. In a system with multiple
        ///  display monitors, this value is always from the primary display.
        /// </summary>
        internal static int GetDpiY(this Gdi32.HDC hdc) => Gdi32.GetDeviceCaps(hdc, Gdi32.DeviceCapability.LOGPIXELSY);
    }
}
