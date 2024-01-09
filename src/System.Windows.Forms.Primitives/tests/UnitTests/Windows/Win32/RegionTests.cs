// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Primitives.Tests.Windows.Win32;

public class RegionTests
{
    [Fact]
    public void GetClipRgn_NoRegion()
    {
        // Create a bitmap using the screen's stats
        HDC hdc = PInvokeCore.CreateCompatibleDC(default);
        Assert.False(hdc.IsNull);

        try
        {
            HBITMAP hbitmap = PInvoke.CreateCompatibleBitmap(hdc, 20, 20);
            Assert.False(hdc.IsNull);

            try
            {
                PInvoke.SelectObject(hdc, hbitmap);
                HRGN hregion = PInvoke.CreateRectRgn(0, 0, 0, 0);

                Assert.False(hregion.IsNull);
                try
                {
                    int result = PInvoke.GetClipRgn(hdc, hregion);

                    // We should have no clipping region
                    Assert.Equal(0, result);
                }
                finally
                {
                    PInvoke.DeleteObject(hregion);
                }
            }
            finally
            {
                PInvoke.DeleteObject(hbitmap);
            }
        }
        finally
        {
            PInvokeCore.DeleteDC(hdc);
        }
    }

    [Fact]
    public void RegionScope_NullWithNoClippingRegion()
    {
        // Create a bitmap using the screen's stats
        HDC hdc = PInvokeCore.CreateCompatibleDC(default);
        Assert.False(hdc.IsNull);

        try
        {
            HBITMAP hbitmap = PInvoke.CreateCompatibleBitmap(hdc, 20, 20);
            Assert.False(hdc.IsNull);

            try
            {
                using RegionScope hregion = new(hdc);
                Assert.True(hregion.IsNull);
            }
            finally
            {
                PInvoke.DeleteObject(hbitmap);
            }
        }
        finally
        {
            PInvokeCore.DeleteDC(hdc);
        }
    }

    [Fact]
    public unsafe void RegionScope_GetRegion()
    {
        // Create a bitmap using the screen's stats
        HDC hdc = PInvokeCore.CreateCompatibleDC(default);
        Assert.False(hdc.IsNull);

        try
        {
            HBITMAP hbitmap = PInvoke.CreateCompatibleBitmap(hdc, 20, 20);
            Assert.False(hdc.IsNull);

            try
            {
                Rectangle rectangle = new(1, 2, 3, 4);
                using RegionScope originalRegion = new(rectangle);
                PInvoke.SelectClipRgn(hdc, originalRegion);
                using RegionScope retrievedRegion = new(hdc);
                RECT rect = default;
                GDI_REGION_TYPE type = PInvoke.GetRgnBox(retrievedRegion, &rect);
                Assert.Equal(GDI_REGION_TYPE.SIMPLEREGION, type);
                Assert.Equal(rectangle, (Rectangle)rect);
            }
            finally
            {
                PInvoke.DeleteObject(hbitmap);
            }
        }
        finally
        {
            PInvokeCore.DeleteDC(hdc);
        }
    }
}
