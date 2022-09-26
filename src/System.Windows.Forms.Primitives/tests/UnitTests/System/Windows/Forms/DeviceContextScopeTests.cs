﻿// Licensed to the .NET Foundation under one or more agreements.
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
        private static extern int GetRandomRgn(HDC hdc, HRGN hrgn, int i);

        [Fact]
        public unsafe void Scope_ApplyGraphicsProperties()
        {
            // Create a bitmap using the screen's stats
            using PInvoke.CreateDcScope dcScope = new(default);
            using PInvoke.CreateBitmapScope bitmapScope = new(dcScope, 20, 20);
            PInvoke.SelectObject(dcScope, bitmapScope);

            // Select a clipping region into the DC
            using var dcRegion = new PInvoke.RegionScope(2, 1, 4, 7);
            RegionType type = (RegionType)PInvoke.SelectClipRgn(dcScope, dcRegion);
            Assert.Equal(RegionType.SIMPLEREGION, type);

            using var test = new PInvoke.RegionScope(0, 0, 0, 0);
            int result = GetRandomRgn(dcScope, test, 1);
            RECT rect2 = default;
            type = (RegionType)PInvoke.GetRgnBox(test, &rect2);

            // Create a Graphics object and set it's clipping region
            using Graphics graphics = dcScope.CreateGraphics();
            using Region oldRegion = graphics.Clip;

            // The existing region should be infinite (no clipping)
            Assert.True(oldRegion.IsInfinite(graphics));

            // Add a new region and transform
            using Region region = new Region(new Rectangle(1, 1, 2, 3));
            graphics.Clip = region;
            graphics.Transform = new Matrix(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f);

            using var hdcScope = new DeviceContextHdcScope(graphics);
            using var regionScope = new PInvoke.RegionScope(hdcScope);
            Assert.False(regionScope.IsNull);
            RECT rect = default;
            type = (RegionType)PInvoke.GetRgnBox(regionScope, &rect);

            // Our clipping region should be the intersection
            Assert.Equal(new Rectangle(2, 1, 1, 3), (Rectangle)rect);
        }

        [Fact]
        public void Graphics_HdcStatePersistence()
        {
            // Create a bitmap using the screen's stats
            using PInvoke.CreateDcScope dcScope = new(default);
            using PInvoke.CreateBitmapScope bitmapScope = new(dcScope, 20, 20);
            HDC_MAP_MODE originalMapMode = (HDC_MAP_MODE)PInvoke.SetMapMode(dcScope, HDC_MAP_MODE.MM_HIMETRIC);
            PInvoke.SelectObject(dcScope, bitmapScope);

            using PInvoke.CreateBrushScope blueBrush = new(Color.Blue);
            using PInvoke.CreateBrushScope redBrush = new(Color.Red);
            PInvoke.SelectObject(dcScope, blueBrush);

            using Graphics graphics = dcScope.CreateGraphics();
            HGDIOBJ current = PInvoke.GetCurrentObject(dcScope, OBJ_TYPE.OBJ_BRUSH);

            HDC_MAP_MODE currentMode = (HDC_MAP_MODE)PInvoke.GetMapMode(dcScope);
            Assert.Equal(HDC_MAP_MODE.MM_HIMETRIC, currentMode);

            IntPtr hdc = graphics.GetHdc();
            currentMode = (HDC_MAP_MODE)PInvoke.SetMapMode(dcScope, HDC_MAP_MODE.MM_TEXT);
            Assert.Equal(HDC_MAP_MODE.MM_HIMETRIC, currentMode);
            try
            {
                // We get the same HDC out
                Assert.Equal(dcScope.HDC, (HDC)hdc);
                current = PInvoke.GetCurrentObject(dcScope, OBJ_TYPE.OBJ_BRUSH);
                Assert.Equal(blueBrush.HBRUSH, (HBRUSH)current);
                PInvoke.SelectObject(dcScope, redBrush);
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
                currentMode = (HDC_MAP_MODE)PInvoke.GetMapMode(dcScope);
                Assert.Equal(HDC_MAP_MODE.MM_TEXT, currentMode);
                current = PInvoke.GetCurrentObject(dcScope, OBJ_TYPE.OBJ_BRUSH);

                graphics.GetHdc();
                try
                {
                    currentMode = (HDC_MAP_MODE)PInvoke.GetMapMode(dcScope);
                    Assert.Equal(HDC_MAP_MODE.MM_TEXT, currentMode);
                }
                finally
                {
                    graphics.ReleaseHdc(hdc);
                }
            }

            current = PInvoke.GetCurrentObject(dcScope, OBJ_TYPE.OBJ_BRUSH);
        }
    }
}
