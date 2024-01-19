// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Internal;

namespace System.Drawing.Drawing2D;

public unsafe sealed class GraphicsPath : MarshalByRefObject, ICloneable, IDisposable
{
    internal GpPath* _nativePath;

    private const float Flatness = (float)2.0 / (float)3.0;

    public GraphicsPath() : this(FillMode.Alternate) { }

    public GraphicsPath(FillMode fillMode)
    {
        GpPath* path;
        PInvoke.GdipCreatePath((GdiPlus.FillMode)fillMode, &path).ThrowIfFailed();
        _nativePath = path;
    }

    public GraphicsPath(PointF[] pts, byte[] types) : this(pts, types, FillMode.Alternate) { }

    public GraphicsPath(PointF[] pts, byte[] types, FillMode fillMode)
    {
        ArgumentNullException.ThrowIfNull(pts);
        ArgumentNullException.ThrowIfNull(types);

        if (pts.Length != types.Length)
            throw Status.InvalidParameter.GetException();

        fixed (PointF* p = pts)
        fixed (byte* t = types)
        {
            GpPath* path;
            PInvoke.GdipCreatePath2((GdiPlus.PointF*)p, t, types.Length, (GdiPlus.FillMode)fillMode, &path).ThrowIfFailed();
            _nativePath = path;
        }
    }

    public GraphicsPath(Point[] pts, byte[] types) : this(pts, types, FillMode.Alternate) { }

    public GraphicsPath(Point[] pts, byte[] types, FillMode fillMode)
    {
        ArgumentNullException.ThrowIfNull(pts);

        if (pts.Length != types.Length)
            throw Status.InvalidParameter.GetException();

        fixed (byte* t = types)
        fixed (Point* p = pts)
        {
            GpPath* path;
            PInvoke.GdipCreatePath2I((GdiPlus.Point*)p, t, types.Length, (GdiPlus.FillMode)fillMode, &path).ThrowIfFailed();
            _nativePath = path;
        }
    }

    public object Clone()
    {
        GpPath* path;
        PInvoke.GdipClonePath(_nativePath, &path).ThrowIfFailed();
        GC.KeepAlive(this);
        return new GraphicsPath(path);
    }

    private GraphicsPath(GpPath* nativePath)
    {
        if (nativePath is null)
            throw new ArgumentNullException(nameof(nativePath));

        _nativePath = nativePath;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_nativePath is not null)
        {
            try
            {
#if DEBUG
                Status status = !Gdip.Initialized ? Status.Ok :
#endif
                PInvoke.GdipDeletePath(_nativePath);
#if DEBUG
                Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
#endif
            }
            catch (Exception ex) when (!ClientUtils.IsSecurityOrCriticalException(ex))
            {
                Debug.Fail($"Exception thrown during Dispose: {ex}");
            }
            finally
            {
                _nativePath = null;
            }
        }
    }

    ~GraphicsPath() => Dispose(disposing: false);

    public void Reset()
    {
        PInvoke.GdipResetPath(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public FillMode FillMode
    {
        get
        {
            GdiPlus.FillMode fillMode;
            PInvoke.GdipGetPathFillMode(_nativePath, &fillMode).ThrowIfFailed();
            GC.KeepAlive(this);
            return (FillMode)fillMode;
        }
        set
        {
            if (value is < FillMode.Alternate or > FillMode.Winding)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FillMode));

            PInvoke.GdipSetPathFillMode(_nativePath, (GdiPlus.FillMode)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public PathData PathData
    {
        get
        {
            int count = PointCount;

            PathData pathData = new()
            {
                Types = new byte[count],
                Points = new PointF[count]
            };

            if (count == 0)
                return pathData;

            fixed (byte* t = pathData.Types)
            fixed (PointF* p = pathData.Points)
            {
                GpPathData data = new()
                {
                    Count = count,
                    Points = p,
                    Types = t
                };

                PInvoke.GdipGetPathData(_nativePath, (GdiPlus.PathData*)&data).ThrowIfFailed();
                GC.KeepAlive(this);
            }

            return pathData;
        }
    }

    public void StartFigure()
    {
        PInvoke.GdipStartPathFigure(_nativePath);
        GC.KeepAlive(this);
    }

    public void CloseFigure()
    {
        PInvoke.GdipClosePathFigure(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void CloseAllFigures()
    {
        PInvoke.GdipClosePathFigures(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void SetMarkers()
    {
        PInvoke.GdipSetPathMarker(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void ClearMarkers()
    {
        PInvoke.GdipClearPathMarkers(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void Reverse()
    {
        PInvoke.GdipReversePath(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public PointF GetLastPoint()
    {
        PointF point;
        PInvoke.GdipGetPathLastPoint(_nativePath, (GdiPlus.PointF*)&point);
        GC.KeepAlive(this);
        return point;
    }

    public bool IsVisible(float x, float y) => IsVisible(new PointF(x, y), null);

    public bool IsVisible(PointF point) => IsVisible(point, null);

    public bool IsVisible(float x, float y, Graphics? graphics)
    {
        BOOL isVisible;
        PInvoke.GdipIsVisiblePathPoint(
            _nativePath,
            x, y,
            graphics is null ? null : graphics.NativeGraphics,
            &isVisible).ThrowIfFailed();

        GC.KeepAlive(this);
        GC.KeepAlive(graphics);
        return isVisible;
    }

    public bool IsVisible(PointF pt, Graphics? graphics) => IsVisible(pt.X, pt.Y, graphics);

    public bool IsVisible(int x, int y) => IsVisible((float)x, y, null);

    public bool IsVisible(Point point) => IsVisible((PointF)point, null);

    public bool IsVisible(int x, int y, Graphics? graphics) => IsVisible((float)x, y, graphics);

    public bool IsVisible(Point pt, Graphics? graphics) => IsVisible((PointF)pt, graphics);

    public bool IsOutlineVisible(float x, float y, Pen pen) => IsOutlineVisible(new PointF(x, y), pen, null);

    public bool IsOutlineVisible(PointF point, Pen pen) => IsOutlineVisible(point, pen, null);

    public bool IsOutlineVisible(float x, float y, Pen pen, Graphics? graphics)
    {
        ArgumentNullException.ThrowIfNull(pen);
        BOOL isVisible;
        PInvoke.GdipIsOutlineVisiblePathPoint(
            _nativePath,
            x, y,
            pen.NativePen,
            graphics is null ? null : graphics.NativeGraphics,
            &isVisible).ThrowIfFailed();

        GC.KeepAlive(this);
        GC.KeepAlive(pen);
        GC.KeepAlive(graphics);
        return isVisible;
    }

    public bool IsOutlineVisible(PointF pt, Pen pen, Graphics? graphics) => IsOutlineVisible(pt.X, pt.Y, pen, graphics);

    public bool IsOutlineVisible(int x, int y, Pen pen) => IsOutlineVisible(new Point(x, y), pen, null);

    public bool IsOutlineVisible(Point point, Pen pen) => IsOutlineVisible(point, pen, null);

    public bool IsOutlineVisible(int x, int y, Pen pen, Graphics? graphics) => IsOutlineVisible((float)x, y, pen, graphics);

    public bool IsOutlineVisible(Point pt, Pen pen, Graphics? graphics) => IsOutlineVisible((PointF)pt, pen, graphics);

    public void AddLine(PointF pt1, PointF pt2) => AddLine(pt1.X, pt1.Y, pt2.X, pt2.Y);

    public void AddLine(float x1, float y1, float x2, float y2)
    {
        PInvoke.GdipAddPathLine(_nativePath, x1, y1, x2, y2).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void AddLines(PointF[] points)
    {
        ArgumentNullException.ThrowIfNull(points);

        if (points.Length == 0)
            throw new ArgumentException(null, nameof(points));

        fixed (PointF* p = points)
        {
            PInvoke.GdipAddPathLine2(_nativePath, (GdiPlus.PointF*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddLine(Point pt1, Point pt2) => AddLine((float)pt1.X, pt1.Y, pt2.X, pt2.Y);

    public void AddLine(int x1, int y1, int x2, int y2) => AddLine((float)x1, y1, x2, y2);

    public void AddLines(Point[] points)
    {
        ArgumentNullException.ThrowIfNull(points);

        if (points.Length == 0)
            throw new ArgumentException(null, nameof(points));

        fixed (Point* p = points)
        {
            PInvoke.GdipAddPathLine2I(_nativePath, (GdiPlus.Point*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddArc(RectangleF rect, float startAngle, float sweepAngle) =>
        AddArc(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        PInvoke.GdipAddPathArc(_nativePath, x, y, width, height, startAngle, sweepAngle).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void AddArc(Rectangle rect, float startAngle, float sweepAngle) =>
        AddArc(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    public void AddArc(int x, int y, int width, int height, float startAngle, float sweepAngle) =>
        AddArc((float)x, y, width, height, startAngle, sweepAngle);

    public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4) =>
        AddBezier(pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);

    public void AddBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
        PInvoke.GdipAddPathBezier(_nativePath, x1, y1, x2, y2, x3, y3, x4, y4).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void AddBeziers(PointF[] points)
    {
        ArgumentNullException.ThrowIfNull(points);

        fixed (PointF* p = points)
        {
            PInvoke.GdipAddPathBeziers(_nativePath, (GdiPlus.PointF*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4) =>
        AddBezier((float)pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);

    public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4) =>
        AddBezier((float)x1, y1, x2, y2, x3, y3, x4, y4);

    public void AddBeziers(params Point[] points)
    {
        ArgumentNullException.ThrowIfNull(points);

        if (points.Length == 0)
            return;

        fixed (Point* p = points)
        {
            PInvoke.GdipAddPathBeziersI(_nativePath, (GdiPlus.Point*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Add cardinal splines to the path object
    /// </summary>
    public void AddCurve(PointF[] points) => AddCurve(points.AsSpan(), 0.5f);

    public void AddCurve(PointF[] points, float tension) => AddCurve(points.AsSpan(), tension);

    public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
    {
        fixed (PointF* p = points)
        {
            PInvoke.GdipAddPathCurve3(
                _nativePath,
                (GdiPlus.PointF*)p,
                points.Length,
                offset,
                numberOfSegments,
                tension).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

    private void AddCurve(Span<PointF> points, float tension)
    {
        fixed (PointF* p = points)
        {
            PInvoke.GdipAddPathCurve2(
                _nativePath,
                (GdiPlus.PointF*)p,
                points.Length,
                tension).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

    public void AddCurve(Point[] points) => AddCurve(points.AsSpan(), 0.5f);

    public void AddCurve(Point[] points, float tension) => AddCurve(points.AsSpan(), tension);

    public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
    {
        fixed (Point* p = points)
        {
            PInvoke.GdipAddPathCurve3I(
                _nativePath,
                (GdiPlus.Point*)p,
                points.Length,
                offset,
                numberOfSegments,
                tension).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

    private void AddCurve(Span<Point> points, float tension)
    {
        fixed (Point* p = points)
        {
            PInvoke.GdipAddPathCurve2I(
                _nativePath,
                (GdiPlus.Point*)p,
                points.Length,
                tension).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

    public void AddClosedCurve(PointF[] points) => AddClosedCurve(points, 0.5f);

    public void AddClosedCurve(PointF[] points, float tension)
    {
        ArgumentNullException.ThrowIfNull(points);

        fixed (PointF* p = points)
        {
            PInvoke.GdipAddPathClosedCurve2(_nativePath, (GdiPlus.PointF*)p, points.Length, tension).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddClosedCurve(Point[] points) => AddClosedCurve(points, 0.5f);

    public void AddClosedCurve(Point[] points, float tension)
    {
        ArgumentNullException.ThrowIfNull(points);

        fixed (Point* p = points)
        {
            PInvoke.GdipAddPathClosedCurve2I(_nativePath, (GdiPlus.Point*)p, points.Length, tension).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddRectangle(RectangleF rect)
    {
        PInvoke.GdipAddPathRectangle(
            _nativePath,
            rect.X, rect.Y, rect.Width, rect.Height).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void AddRectangles(RectangleF[] rects)
    {
        ArgumentNullException.ThrowIfNull(rects);

        fixed (RectangleF* r = rects)
        {
            PInvoke.GdipAddPathRectangles(_nativePath, (RectF*)r, rects.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddRectangle(Rectangle rect) => AddRectangle((RectangleF)rect);

    public void AddRectangles(Rectangle[] rects)
    {
        ArgumentNullException.ThrowIfNull(rects);

        fixed (Rectangle* r = rects)
        {
            PInvoke.GdipAddPathRectanglesI(_nativePath, (Rect*)r, rects.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddEllipse(RectangleF rect) =>
        AddEllipse(rect.X, rect.Y, rect.Width, rect.Height);

    public void AddEllipse(float x, float y, float width, float height)
    {
        PInvoke.GdipAddPathEllipse(_nativePath, x, y, width, height).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void AddEllipse(Rectangle rect) => AddEllipse(rect.X, rect.Y, rect.Width, rect.Height);

    public void AddEllipse(int x, int y, int width, int height) => AddEllipse((float)x, y, width, height);

    public void AddPie(Rectangle rect, float startAngle, float sweepAngle) =>
        AddPie((float)rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        PInvoke.GdipAddPathPie(
            _nativePath,
            x, y, width, height,
            startAngle,
            sweepAngle).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle) =>
        AddPie((float)x, y, width, height, startAngle, sweepAngle);

    public void AddPolygon(PointF[] points)
    {
        ArgumentNullException.ThrowIfNull(points);

        fixed (PointF* p = points)
        {
            PInvoke.GdipAddPathPolygon(_nativePath, (GdiPlus.PointF*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Adds a polygon to the current figure.
    /// </summary>
    public void AddPolygon(Point[] points)
    {
        ArgumentNullException.ThrowIfNull(points);

        fixed (Point* p = points)
        {
            PInvoke.GdipAddPathPolygonI(_nativePath, (GdiPlus.Point*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddPath(GraphicsPath addingPath, bool connect)
    {
        ArgumentNullException.ThrowIfNull(addingPath);
        PInvoke.GdipAddPathPath(_nativePath, addingPath.Pointer(), connect).ThrowIfFailed();
        GC.KeepAlive(addingPath);
        GC.KeepAlive(this);
    }

    public void AddString(string s, FontFamily family, int style, float emSize, PointF origin, StringFormat? format) =>
        AddString(s, family, style, emSize, new RectangleF(origin.X, origin.Y, 0, 0), format);

    public void AddString(string s, FontFamily family, int style, float emSize, Point origin, StringFormat? format) =>
        AddString(s, family, style, emSize, new Rectangle(origin.X, origin.Y, 0, 0), format);

    public void AddString(string s, FontFamily family, int style, float emSize, RectangleF layoutRect, StringFormat? format)
    {
        ArgumentNullException.ThrowIfNull(s);
        ArgumentNullException.ThrowIfNull(family);

        fixed (char* c = s)
        {
            PInvoke.GdipAddPathString(
                _nativePath,
                c, s.Length,
                family.Pointer(),
                style,
                emSize,
                (RectF*)&layoutRect,
                format.Pointer());
        }

        GC.KeepAlive(family);
        GC.KeepAlive(format);
        GC.KeepAlive(this);
    }

    public void AddString(string s, FontFamily family, int style, float emSize, Rectangle layoutRect, StringFormat? format)
        => AddString(s, family, style, emSize, (RectangleF)layoutRect, format);

    public void Transform(Matrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        PInvoke.GdipTransformPath(_nativePath, matrix.NativeMatrix).ThrowIfFailed();
        GC.KeepAlive(matrix);
        GC.KeepAlive(this);
    }

    public RectangleF GetBounds() => GetBounds(null);

    public RectangleF GetBounds(Matrix? matrix) => GetBounds(matrix, null);

    public RectangleF GetBounds(Matrix? matrix, Pen? pen)
    {
        RectF bounds;
        PInvoke.GdipGetPathWorldBounds(
            _nativePath,
            &bounds,
            matrix.Pointer(),
            pen.Pointer()).ThrowIfFailed();

        GC.KeepAlive(this);
        GC.KeepAlive(matrix);
        GC.KeepAlive(pen);
        return bounds;
    }

    public void Flatten() => Flatten(null);

    public void Flatten(Matrix? matrix) => Flatten(matrix, 0.25f);

    public void Flatten(Matrix? matrix, float flatness)
    {
        PInvoke.GdipFlattenPath(_nativePath, matrix.Pointer(), flatness).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void Widen(Pen pen) => Widen(pen, null, Flatness);

    public void Widen(Pen pen, Matrix? matrix) => Widen(pen, matrix, Flatness);

    public void Widen(Pen pen, Matrix? matrix, float flatness)
    {
        ArgumentNullException.ThrowIfNull(pen);

        // GDI+ wrongly returns an out of memory status when there is nothing in the path, so we have to check
        // before calling the widen method and do nothing if we don't have anything in the path.
        if (PointCount == 0)
            return;

        PInvoke.GdipWidenPath(_nativePath, pen.Pointer(), matrix.Pointer(), flatness).ThrowIfFailed();
        GC.KeepAlive(pen);
        GC.KeepAlive(matrix);
        GC.KeepAlive(this);
    }

    public void Warp(PointF[] destPoints, RectangleF srcRect) => Warp(destPoints, srcRect, null);

    public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix? matrix) =>
        Warp(destPoints, srcRect, matrix, WarpMode.Perspective);

    public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix? matrix, WarpMode warpMode) =>
        Warp(destPoints, srcRect, matrix, warpMode, 0.25f);

    public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix? matrix, WarpMode warpMode, float flatness)
    {
        ArgumentNullException.ThrowIfNull(destPoints);

        fixed (PointF* p = destPoints)
        {
            PInvoke.GdipWarpPath(
                _nativePath,
                matrix.Pointer(),
                (GdiPlus.PointF*)p,
                destPoints.Length,
                srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
                (GdiPlus.WarpMode)warpMode,
                flatness).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

    public int PointCount
    {
        get
        {
            int count;
            PInvoke.GdipGetPointCount(_nativePath, &count).ThrowIfFailed();
            GC.KeepAlive(this);
            return count;
        }
    }

    public byte[] PathTypes
    {
        get
        {
            int count = PointCount;
            if (count == 0)
            {
                return [];
            }

            byte[] types = new byte[count];
            fixed (byte* t = types)
            {
                PInvoke.GdipGetPathTypes(_nativePath, t, types.Length).ThrowIfFailed();
                GC.KeepAlive(this);
                return types;
            }
        }
    }

    public PointF[] PathPoints
    {
        get
        {
            int count = PointCount;
            if (count == 0)
            {
                return [];
            }

            PointF[] points = new PointF[count];
            fixed (PointF* p = points)
            {
                PInvoke.GdipGetPathPoints(_nativePath, (GdiPlus.PointF*)p, points.Length).ThrowIfFailed();
                GC.KeepAlive(this);
                return points;
            }
        }
    }
}
