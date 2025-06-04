// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Drawing2D;

namespace System.Drawing;

public sealed unsafe class Region : MarshalByRefObject, IDisposable, IPointer<GpRegion>
{
    internal GpRegion* NativeRegion { get; private set; }

    nint IPointer<GpRegion>.Pointer => (nint)NativeRegion;

    public Region()
    {
        GpRegion* region;
        CheckStatus(PInvokeGdiPlus.GdipCreateRegion(&region));
        SetNativeRegion(region);
    }

    public Region(RectangleF rect)
    {
        GpRegion* region = default;
        RectF rectF = rect;
        CheckStatus(PInvokeGdiPlus.GdipCreateRegionRect(&rectF, &region));
        SetNativeRegion(region);
    }

    public Region(Rectangle rect) : this((RectangleF)rect)
    {
    }

    public Region(GraphicsPath path)
    {
        ArgumentNullException.ThrowIfNull(path);

        GpRegion* region = default;
        CheckStatus(PInvokeGdiPlus.GdipCreateRegionPath(path._nativePath, &region));
        GC.KeepAlive(path);
        SetNativeRegion(region);
    }

    public Region(RegionData rgnData)
    {
        ArgumentNullException.ThrowIfNull(rgnData);

        GpRegion* region = default;
        fixed (byte* data = rgnData.Data)
        {
            CheckStatus(PInvokeGdiPlus.GdipCreateRegionRgnData(data, rgnData.Data.Length, &region));
        }

        SetNativeRegion(region);
    }

    internal Region(GpRegion* nativeRegion) => SetNativeRegion(nativeRegion);

    public static Region FromHrgn(IntPtr hrgn)
    {
        GpRegion* region = default;
        Gdip.CheckStatus(PInvokeGdiPlus.GdipCreateRegionHrgn((HRGN)hrgn, &region));
        return new Region(region);
    }

    private void SetNativeRegion(GpRegion* nativeRegion)
    {
        if (nativeRegion is null)
            throw new ArgumentNullException(nameof(nativeRegion));

        NativeRegion = nativeRegion;
    }

    public Region Clone()
    {
        GpRegion* region = default;
        CheckStatus(PInvokeGdiPlus.GdipCloneRegion(NativeRegion, &region));
        return new Region(region);
    }

    public void ReleaseHrgn(IntPtr regionHandle)
    {
        if (regionHandle == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(regionHandle));
        }

        PInvokeCore.DeleteObject((HRGN)regionHandle);
        GC.KeepAlive(this);
    }

    public void Dispose()
    {
        if (NativeRegion is not null)
        {
            Status status = !Gdip.Initialized ? Status.Ok : PInvokeGdiPlus.GdipDeleteRegion(NativeRegion);
            NativeRegion = null;
            Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
        }

        GC.SuppressFinalize(this);
    }

    ~Region() => Dispose();

    public void MakeInfinite() => CheckStatus(PInvokeGdiPlus.GdipSetInfinite(NativeRegion));

    public void MakeEmpty() => CheckStatus(PInvokeGdiPlus.GdipSetEmpty(NativeRegion));

    public void Intersect(RectangleF rect) =>
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionRect(NativeRegion, (RectF*)&rect, GdiPlus.CombineMode.CombineModeIntersect));

    public void Intersect(Rectangle rect) => Intersect((RectangleF)rect);

    public void Intersect(GraphicsPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionPath(NativeRegion, path._nativePath, GdiPlus.CombineMode.CombineModeIntersect));
        GC.KeepAlive(path);
    }

    public void Intersect(Region region)
    {
        ArgumentNullException.ThrowIfNull(region);
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionRegion(NativeRegion, region.NativeRegion, GdiPlus.CombineMode.CombineModeIntersect));
        GC.KeepAlive(region);
    }

    public void Union(RectangleF rect) =>
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionRect(NativeRegion, (RectF*)&rect, GdiPlus.CombineMode.CombineModeUnion));

    public void Union(Rectangle rect) => Union((RectangleF)rect);

    public void Union(GraphicsPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionPath(NativeRegion, path._nativePath, GdiPlus.CombineMode.CombineModeUnion));
        GC.KeepAlive(path);
    }

    public void Union(Region region)
    {
        ArgumentNullException.ThrowIfNull(region);
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionRegion(NativeRegion, region.NativeRegion, GdiPlus.CombineMode.CombineModeUnion));
        GC.KeepAlive(region);
    }

    public void Xor(RectangleF rect) =>
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionRect(NativeRegion, (RectF*)&rect, GdiPlus.CombineMode.CombineModeXor));

    public void Xor(Rectangle rect) => Xor((RectangleF)rect);

    public void Xor(GraphicsPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionPath(NativeRegion, path._nativePath, GdiPlus.CombineMode.CombineModeXor));
        GC.KeepAlive(path);
    }

    public void Xor(Region region)
    {
        ArgumentNullException.ThrowIfNull(region);
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionRegion(NativeRegion, region.NativeRegion, GdiPlus.CombineMode.CombineModeXor));
        GC.KeepAlive(region);
    }

    public void Exclude(RectangleF rect) =>
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionRect(NativeRegion, (RectF*)&rect, GdiPlus.CombineMode.CombineModeExclude));

    public void Exclude(Rectangle rect) => Exclude((RectangleF)rect);

    public void Exclude(GraphicsPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionPath(NativeRegion, path._nativePath, GdiPlus.CombineMode.CombineModeExclude));
        GC.KeepAlive(path);
    }

    public void Exclude(Region region)
    {
        ArgumentNullException.ThrowIfNull(region);
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionRegion(NativeRegion, region.NativeRegion, GdiPlus.CombineMode.CombineModeExclude));
        GC.KeepAlive(region);
    }

    public void Complement(RectangleF rect) =>
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionRect(NativeRegion, (RectF*)&rect, GdiPlus.CombineMode.CombineModeComplement));

    public void Complement(Rectangle rect) => Complement((RectangleF)rect);

    public void Complement(GraphicsPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionPath(NativeRegion, path._nativePath, GdiPlus.CombineMode.CombineModeComplement));
        GC.KeepAlive(path);
    }

    public void Complement(Region region)
    {
        ArgumentNullException.ThrowIfNull(region);
        CheckStatus(PInvokeGdiPlus.GdipCombineRegionRegion(NativeRegion, region.NativeRegion, GdiPlus.CombineMode.CombineModeComplement));
        GC.KeepAlive(region);
    }

    public void Translate(float dx, float dy) => CheckStatus(PInvokeGdiPlus.GdipTranslateRegion(NativeRegion, dx, dy));

    public void Translate(int dx, int dy) => Translate((float)dx, dy);

    public void Transform(Matrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        CheckStatus(PInvokeGdiPlus.GdipTransformRegion(NativeRegion, matrix.NativeMatrix));
        GC.KeepAlive(matrix);
    }

    public RectangleF GetBounds(Graphics g)
    {
        ArgumentNullException.ThrowIfNull(g);
        RectF bounds;
        CheckStatus(PInvokeGdiPlus.GdipGetRegionBounds(NativeRegion, g.NativeGraphics, &bounds));
        GC.KeepAlive(g);
        return bounds;
    }

    public IntPtr GetHrgn(Graphics g)
    {
        ArgumentNullException.ThrowIfNull(g);
        HRGN hrgn;
        CheckStatus(PInvokeGdiPlus.GdipGetRegionHRgn(NativeRegion, g.NativeGraphics, &hrgn));
        GC.KeepAlive(g);
        return hrgn;
    }

    public bool IsEmpty(Graphics g)
    {
        ArgumentNullException.ThrowIfNull(g);
        BOOL isEmpty;
        CheckStatus(PInvokeGdiPlus.GdipIsEmptyRegion(NativeRegion, g.NativeGraphics, &isEmpty));
        GC.KeepAlive(g);
        return isEmpty;
    }

    public bool IsInfinite(Graphics g)
    {
        ArgumentNullException.ThrowIfNull(g);
        BOOL isInfinite;
        CheckStatus(PInvokeGdiPlus.GdipIsInfiniteRegion(NativeRegion, g.NativeGraphics, &isInfinite));
        return isInfinite;
    }

    public bool Equals(Region region, Graphics g)
    {
        ArgumentNullException.ThrowIfNull(region);
        ArgumentNullException.ThrowIfNull(g);
        BOOL isEqual;
        CheckStatus(PInvokeGdiPlus.GdipIsEqualRegion(NativeRegion, region.NativeRegion, g.NativeGraphics, &isEqual));
        GC.KeepAlive(g);
        GC.KeepAlive(region);
        return isEqual;
    }

    public RegionData? GetRegionData()
    {
        uint regionSize;
        CheckStatus(PInvokeGdiPlus.GdipGetRegionDataSize(NativeRegion, &regionSize));

        if (regionSize == 0)
            return null;

        byte[] regionData = new byte[regionSize];
        fixed (byte* rd = regionData)
        {
            CheckStatus(PInvokeGdiPlus.GdipGetRegionData(NativeRegion, rd, regionSize, &regionSize));
        }

        return new RegionData(regionData);
    }

    public bool IsVisible(float x, float y) => IsVisible(new PointF(x, y), null);

    public bool IsVisible(PointF point) => IsVisible(point, null);

    public bool IsVisible(float x, float y, Graphics? g) => IsVisible(new PointF(x, y), g);

    public bool IsVisible(PointF point, Graphics? g)
    {
        BOOL isVisible;
        CheckStatus(PInvokeGdiPlus.GdipIsVisibleRegionPoint(
            NativeRegion,
            point.X,
            point.Y,
            g is null ? null : g.NativeGraphics,
            &isVisible));

        GC.KeepAlive(g);
        return isVisible;
    }

    public bool IsVisible(float x, float y, float width, float height) => IsVisible(new RectangleF(x, y, width, height), null);

    public bool IsVisible(RectangleF rect) => IsVisible(rect, null);

    public bool IsVisible(float x, float y, float width, float height, Graphics? g) => IsVisible(new RectangleF(x, y, width, height), g);

    public bool IsVisible(RectangleF rect, Graphics? g)
    {
        BOOL isVisible;
        CheckStatus(PInvokeGdiPlus.GdipIsVisibleRegionRect(
            NativeRegion,
            rect.X, rect.Y, rect.Width, rect.Height,
            g is null ? null : g.NativeGraphics,
            &isVisible));

        GC.KeepAlive(g);
        return isVisible;
    }

    public bool IsVisible(int x, int y, Graphics? g) => IsVisible(new Point(x, y), g);

    public bool IsVisible(Point point) => IsVisible(point, null);

    public bool IsVisible(Point point, Graphics? g) => IsVisible((PointF)point, g);

    public bool IsVisible(int x, int y, int width, int height) => IsVisible(new Rectangle(x, y, width, height), null);

    public bool IsVisible(Rectangle rect) => IsVisible(rect, null);

    public bool IsVisible(int x, int y, int width, int height, Graphics? g) => IsVisible(new Rectangle(x, y, width, height), g);

    public bool IsVisible(Rectangle rect, Graphics? g) => IsVisible((RectangleF)rect, g);

    public RectangleF[] GetRegionScans(Matrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        uint count;
        CheckStatus(PInvokeGdiPlus.GdipGetRegionScansCount(
            NativeRegion,
            &count,
            matrix.NativeMatrix));

        if (count == 0)
        {
            return [];
        }

        RectangleF[] rectangles = new RectangleF[count];

        fixed (RectangleF* r = rectangles)
        {
            CheckStatus(PInvokeGdiPlus.GdipGetRegionScans(
                NativeRegion,
                (RectF*)r,
                (int*)&count,
                matrix.NativeMatrix));
        }

        GC.KeepAlive(matrix);
        return rectangles;
    }

    private void CheckStatus(Status status)
    {
        Gdip.CheckStatus(status);
        GC.KeepAlive(this);
    }
}
