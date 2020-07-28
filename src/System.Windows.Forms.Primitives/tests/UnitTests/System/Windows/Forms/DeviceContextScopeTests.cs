// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class DeviceContextScopeTests
    {
        [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true)]
        private static extern int GetRandomRgn(Gdi32.HDC hdc, Gdi32.HRGN hrgn, int i);

        [Fact]
        public void Scope_ApplyGraphicsProperties()
        {
            // Create a bitmap using the screen's stats
            using var dcScope = new Gdi32.CreateDcScope(default);
            using var bitmapScope = new Gdi32.CreateBitmapScope(dcScope, 20, 20);
            Gdi32.SelectObject(dcScope, bitmapScope);

            // Select a clipping region into the DC
            using var dcRegion = new Gdi32.RegionScope(2, 1, 4, 7);
            RegionType type = Gdi32.SelectClipRgn(dcScope, dcRegion);
            Assert.Equal(RegionType.SIMPLEREGION, type);

            using var test = new Gdi32.RegionScope(0, 0, 0, 0);
            int result = GetRandomRgn(dcScope, test, 1);
            RECT rect2 = default;
            type = Gdi32.GetRgnBox(test, ref rect2);

            // Create a Graphics object and set it's clipping region
            using Graphics graphics = dcScope.CreateGraphics();
            using Region oldRegion = graphics.Clip;

            // The existing region should be infinite (no clipping)
            Assert.True(oldRegion.IsInfinite(graphics));

            // Add a new region and transform
            using Region region = new Region(new Rectangle(1, 1, 2, 3));
            graphics.Clip = region;
            graphics.Transform = new Matrix(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f);

            using (var hdcScope = new DeviceContextHdcScope(graphics))
            {
                using var regionScope = new Gdi32.RegionScope(hdcScope);
                Assert.False(regionScope.IsNull);
                RECT rect = default;
                type = Gdi32.GetRgnBox(regionScope, ref rect);

                // Our clipping region should be the intersection
                Assert.Equal(new Rectangle(2, 1, 1, 3), (Rectangle)rect);
            }
        }

        [Fact]
        public void Graphics_HdcStatePersistence()
        {
            // Create a bitmap using the screen's stats
            using var dcScope = new Gdi32.CreateDcScope(default);
            using var bitmapScope = new Gdi32.CreateBitmapScope(dcScope, 20, 20);
            Gdi32.MM originalMapMode = Gdi32.SetMapMode(dcScope, Gdi32.MM.HIMETRIC);
            Gdi32.SelectObject(dcScope, bitmapScope);

            Gdi32.OBJ type = Gdi32.GetObjectType(dcScope);

            using var blueBrush = new Gdi32.CreateBrushScope(Color.Blue);
            using var redBrush = new Gdi32.CreateBrushScope(Color.Red);
            Gdi32.SelectObject(dcScope, blueBrush);

            using Graphics graphics = dcScope.CreateGraphics();
            Gdi32.HGDIOBJ current = Gdi32.GetCurrentObject(dcScope, Gdi32.OBJ.BRUSH);

            Gdi32.MM currentMode = Gdi32.GetMapMode(dcScope);
            Assert.Equal(Gdi32.MM.HIMETRIC, currentMode);

            IntPtr hdc = graphics.GetHdc();
            currentMode = Gdi32.SetMapMode(dcScope, Gdi32.MM.TEXT);
            Assert.Equal(Gdi32.MM.HIMETRIC, currentMode);
            try
            {
                // We get the same HDC out
                Assert.Equal(dcScope.HDC.Handle, hdc);
                current = Gdi32.GetCurrentObject(dcScope, Gdi32.OBJ.BRUSH);
                Assert.Equal(blueBrush.HBrush.Handle, current.Handle);
                Gdi32.SelectObject(dcScope, redBrush);
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
                currentMode = Gdi32.GetMapMode(dcScope);
                Assert.Equal(Gdi32.MM.TEXT, currentMode);
                current = Gdi32.GetCurrentObject(dcScope, Gdi32.OBJ.BRUSH);

                graphics.GetHdc();
                try
                {
                    currentMode = Gdi32.GetMapMode(dcScope);
                    Assert.Equal(Gdi32.MM.TEXT, currentMode);
                }
                finally
                {
                    graphics.ReleaseHdc(hdc);
                }
            }

            current = Gdi32.GetCurrentObject(dcScope, Gdi32.OBJ.BRUSH);
        }
    }
}
