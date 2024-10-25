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
        using CreateDcScope hdc = new(HDC.Null);
        Assert.False(hdc.IsNull);

        using CreateBitmapScope hbitmap = new(hdc, 20, 20);
        Assert.False(hdc.IsNull);

        PInvokeCore.SelectObject(hdc, hbitmap);
        using RegionScope hregion = new(0, 0, 0, 0);

        Assert.False(hregion.IsNull);
        int result = PInvoke.GetClipRgn(hdc, hregion);

        // We should have no clipping region
        Assert.Equal(0, result);
    }

    [Fact]
    public void RegionScope_NullWithNoClippingRegion()
    {
        // Create a bitmap using the screen's stats
        using CreateDcScope hdc = new(HDC.Null);
        Assert.False(hdc.IsNull);

        using CreateBitmapScope hbitmap = new(hdc, 20, 20);
        Assert.False(hdc.IsNull);

        using RegionScope hregion = new(hdc);
        Assert.True(hregion.IsNull);
    }

    [Fact]
    public unsafe void RegionScope_GetRegion()
    {
        // Create a bitmap using the screen's stats
        using CreateDcScope hdc = new(HDC.Null);
        Assert.False(hdc.IsNull);

        using CreateBitmapScope hbitmap = new(hdc, 20, 20);
        Assert.False(hdc.IsNull);

        Rectangle rectangle = new(1, 2, 3, 4);
        using RegionScope originalRegion = new(rectangle);
        PInvokeCore.SelectClipRgn(hdc, originalRegion);
        using RegionScope retrievedRegion = new(hdc);
        RECT rect = default;
        GDI_REGION_TYPE type = PInvoke.GetRgnBox(retrievedRegion, &rect);
        Assert.Equal(GDI_REGION_TYPE.SIMPLEREGION, type);
        Assert.Equal(rectangle, (Rectangle)rect);
    }
}
