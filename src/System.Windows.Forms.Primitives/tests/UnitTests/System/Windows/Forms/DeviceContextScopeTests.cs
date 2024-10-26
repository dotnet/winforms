// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Tests;

public class DeviceContextScopeTests
{
    [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true)]
    private static extern int GetRandomRgn(HDC hdc, HRGN hrgn, int i);

    [Fact]
    public unsafe void Scope_ApplyGraphicsProperties()
    {
        // Create a bitmap using the screen's stats
        using CreateDcScope dcScope = new(default);
        using CreateBitmapScope bitmapScope = new(dcScope, 20, 20);
        PInvokeCore.SelectObject(dcScope, bitmapScope);

        // Select a clipping region into the DC
        using RegionScope dcRegion = new(2, 1, 4, 7);
        GDI_REGION_TYPE type = PInvokeCore.SelectClipRgn(dcScope, dcRegion);
        Assert.Equal(GDI_REGION_TYPE.SIMPLEREGION, type);

        using RegionScope test = new(0, 0, 0, 0);
        int result = GetRandomRgn(dcScope, test, 1);
        RECT rect2 = default;
        type = PInvoke.GetRgnBox(test, &rect2);

        // Create a Graphics object and set it's clipping region
        using Graphics graphics = dcScope.CreateGraphics();
        using Region oldRegion = graphics.Clip;

        // The existing region should be infinite (no clipping)
        Assert.True(oldRegion.IsInfinite(graphics));

        // Add a new region and transform
        using Region region = new(new Rectangle(1, 1, 2, 3));
        graphics.Clip = region;
        graphics.Transform = new Matrix(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f);

        using DeviceContextHdcScope hdcScope = new(graphics);
        using RegionScope regionScope = new(hdcScope);
        Assert.False(regionScope.IsNull);
        RECT rect = default;
        type = PInvoke.GetRgnBox(regionScope, &rect);

        // Our clipping region should be the intersection
        Assert.Equal(new Rectangle(2, 1, 1, 3), (Rectangle)rect);
    }

    [Fact]
    public void Graphics_HdcStatePersistence()
    {
        // Create a bitmap using the screen's stats
        using CreateDcScope dcScope = new(default);
        using CreateBitmapScope bitmapScope = new(dcScope, 20, 20);
        HDC_MAP_MODE originalMapMode = (HDC_MAP_MODE)PInvokeCore.SetMapMode(dcScope, HDC_MAP_MODE.MM_HIMETRIC);
        PInvokeCore.SelectObject(dcScope, bitmapScope);

        using CreateBrushScope blueBrush = new(Color.Blue);
        using CreateBrushScope redBrush = new(Color.Red);
        PInvokeCore.SelectObject(dcScope, blueBrush);

        using Graphics graphics = dcScope.CreateGraphics();
        HGDIOBJ current = PInvoke.GetCurrentObject(dcScope, OBJ_TYPE.OBJ_BRUSH);

        HDC_MAP_MODE currentMode = PInvoke.GetMapMode(dcScope);
        Assert.Equal(HDC_MAP_MODE.MM_HIMETRIC, currentMode);

        IntPtr hdc = graphics.GetHdc();
        currentMode = (HDC_MAP_MODE)PInvokeCore.SetMapMode(dcScope, HDC_MAP_MODE.MM_TEXT);
        Assert.Equal(HDC_MAP_MODE.MM_HIMETRIC, currentMode);
        try
        {
            // We get the same HDC out
            Assert.Equal(dcScope.HDC, (HDC)hdc);
            current = PInvoke.GetCurrentObject(dcScope, OBJ_TYPE.OBJ_BRUSH);
            Assert.Equal(blueBrush.HBRUSH, (HBRUSH)current);
            PInvokeCore.SelectObject(dcScope, redBrush);
        }
        finally
        {
            graphics.ReleaseHdc(hdc);
            currentMode = PInvoke.GetMapMode(dcScope);
            Assert.Equal(HDC_MAP_MODE.MM_TEXT, currentMode);
            current = PInvoke.GetCurrentObject(dcScope, OBJ_TYPE.OBJ_BRUSH);

            graphics.GetHdc();
            try
            {
                currentMode = PInvoke.GetMapMode(dcScope);
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
