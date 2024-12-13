// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.GdiPlus;

internal static unsafe class GpRegionExtensions
{
    /// <summary>
    ///  Creates a native region from a GDI+ <see cref="GpRegion"/> and <see cref="GpGraphics"/>.
    /// </summary>
    public static RegionScope GetRegionScope(this IPointer<GpRegion> region, IPointer<GpGraphics> graphics)
    {
        RegionScope scope = new(InitializeFromGdiPlus(region.GetPointer(), graphics.GetPointer()));
        GC.KeepAlive(region);
        GC.KeepAlive(graphics);
        return scope;
    }

    /// <summary>
    ///  Creates a native region from a GDI+ <see cref="GpRegion"/> and <see cref="HWND"/>.
    /// </summary>
    public static RegionScope GetRegionScope(this IPointer<GpRegion> region, HWND hwnd)
    {
        GpGraphics* graphics = null;
        PInvokeGdiPlus.GdipCreateFromHWND(hwnd, &graphics).ThrowIfFailed();
        RegionScope scope = new(InitializeFromGdiPlus(region.GetPointer(), graphics));
        GC.KeepAlive(region);
        return scope;
    }

    private static HRGN InitializeFromGdiPlus(GpRegion* region, GpGraphics* graphics)
    {
        BOOL isInfinite;
        PInvokeGdiPlus.GdipIsInfiniteRegion(region, graphics, &isInfinite).ThrowIfFailed();

        if (isInfinite)
        {
            // An infinite region would cover the entire device region which is the same as
            // not having a clipping region. Observe that this is not the same as having an
            // empty region, which when clipping to it has the effect of excluding the entire
            // device region.
            //
            // To remove the clip region from a dc the SelectClipRgn() function needs to be
            // called with a null region ptr - that's why we use the empty constructor here.
            // GDI+ will return IntPtr.Zero for Region.GetHrgn(Graphics) when the region is
            // Infinite.

            return HRGN.Null;
        }

        HRGN hrgn;
        PInvokeGdiPlus.GdipGetRegionHRgn(region, graphics, &hrgn).ThrowIfFailed();
        return hrgn;
    }
}
