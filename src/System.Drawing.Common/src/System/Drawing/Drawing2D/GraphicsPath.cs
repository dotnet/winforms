// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Internal;

namespace System.Drawing.Drawing2D;

public sealed unsafe class GraphicsPath : MarshalByRefObject, ICloneable, IDisposable
{
    internal GpPath* _nativePath;

    private const float Flatness = (float)2.0 / (float)3.0;

    /// <inheritdoc cref="GraphicsPath(Point[], byte[], FillMode)"/>
    public GraphicsPath() : this(FillMode.Alternate) { }

    /// <inheritdoc cref="GraphicsPath(Point[], byte[], FillMode)"/>
    public GraphicsPath(FillMode fillMode)
    {
        GpPath* path;
        PInvokeGdiPlus.GdipCreatePath((GdiPlus.FillMode)fillMode, &path).ThrowIfFailed();
        _nativePath = path;
    }

    /// <inheritdoc cref="GraphicsPath(Point[], byte[], FillMode)"/>
    public GraphicsPath(PointF[] pts, byte[] types) : this(pts, types, FillMode.Alternate) { }

    /// <inheritdoc cref="GraphicsPath(Point[], byte[], FillMode)"/>
    public GraphicsPath(PointF[] pts, byte[] types, FillMode fillMode)
        : this(pts.OrThrowIfNull().AsSpan(), types.OrThrowIfNull().AsSpan(), fillMode)
    {
    }

    /// <inheritdoc cref="GraphicsPath(Point[], byte[], FillMode)"/>
#if NET9_0_OR_GREATER
    public
#else
    internal
#endif
    GraphicsPath(ReadOnlySpan<PointF> pts, ReadOnlySpan<byte> types, FillMode fillMode = FillMode.Alternate)
    {
        if (pts.Length != types.Length)
        {
            throw Status.InvalidParameter.GetException();
        }

        fixed (PointF* p = pts)
        fixed (byte* t = types)
        {
            GpPath* path;
            PInvokeGdiPlus.GdipCreatePath2((GdiPlus.PointF*)p, t, types.Length, (GdiPlus.FillMode)fillMode, &path).ThrowIfFailed();
            _nativePath = path;
        }
    }

    /// <inheritdoc cref="GraphicsPath(Point[], byte[], FillMode)"/>
    public GraphicsPath(Point[] pts, byte[] types) : this(pts, types, FillMode.Alternate) { }

    /// <summary>
    ///  Initializes a new instance of the <see cref='GraphicsPath'/> class.
    /// </summary>
    /// <param name="pts">Array of points that define the path.</param>
    /// <param name="types">Array of <see cref="PathPointType"/> values that specify the type of <paramref name="pts"/></param>
    /// <param name="fillMode">
    ///  A <see cref="Drawing2D.FillMode"/> enumeration that specifies how the interiors of shapes in this <see cref="GraphicsPath"/>
    /// </param>
    public GraphicsPath(Point[] pts, byte[] types, FillMode fillMode)
        : this(pts.OrThrowIfNull().AsSpan(), types.OrThrowIfNull().AsSpan(), fillMode) { }

    /// <inheritdoc cref="GraphicsPath(Point[], byte[], FillMode)"/>
#if NET9_0_OR_GREATER
    public
#else
    internal
#endif
    GraphicsPath(ReadOnlySpan<Point> pts, ReadOnlySpan<byte> types, FillMode fillMode = FillMode.Alternate)
    {
        if (pts.Length != types.Length)
        {
            throw Status.InvalidParameter.GetException();
        }

        fixed (byte* t = types)
        fixed (Point* p = pts)
        {
            GpPath* path;
            PInvokeGdiPlus.GdipCreatePath2I((GdiPlus.Point*)p, t, types.Length, (GdiPlus.FillMode)fillMode, &path).ThrowIfFailed();
            _nativePath = path;
        }
    }

    public object Clone()
    {
        GpPath* path;
        PInvokeGdiPlus.GdipClonePath(_nativePath, &path).ThrowIfFailed();
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
                PInvokeGdiPlus.GdipDeletePath(_nativePath);
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
        PInvokeGdiPlus.GdipResetPath(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public FillMode FillMode
    {
        get
        {
            GdiPlus.FillMode fillMode;
            PInvokeGdiPlus.GdipGetPathFillMode(_nativePath, &fillMode).ThrowIfFailed();
            GC.KeepAlive(this);
            return (FillMode)fillMode;
        }
        set
        {
            if (value is < FillMode.Alternate or > FillMode.Winding)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FillMode));

            PInvokeGdiPlus.GdipSetPathFillMode(_nativePath, (GdiPlus.FillMode)value).ThrowIfFailed();
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

                PInvokeGdiPlus.GdipGetPathData(_nativePath, (GdiPlus.PathData*)&data).ThrowIfFailed();
                GC.KeepAlive(this);
            }

            return pathData;
        }
    }

    public void StartFigure()
    {
        PInvokeGdiPlus.GdipStartPathFigure(_nativePath);
        GC.KeepAlive(this);
    }

    public void CloseFigure()
    {
        PInvokeGdiPlus.GdipClosePathFigure(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void CloseAllFigures()
    {
        PInvokeGdiPlus.GdipClosePathFigures(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void SetMarkers()
    {
        PInvokeGdiPlus.GdipSetPathMarker(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void ClearMarkers()
    {
        PInvokeGdiPlus.GdipClearPathMarkers(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void Reverse()
    {
        PInvokeGdiPlus.GdipReversePath(_nativePath).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public PointF GetLastPoint()
    {
        PointF point;
        PInvokeGdiPlus.GdipGetPathLastPoint(_nativePath, (GdiPlus.PointF*)&point);
        GC.KeepAlive(this);
        return point;
    }

    public bool IsVisible(float x, float y) => IsVisible(new PointF(x, y), null);

    public bool IsVisible(PointF point) => IsVisible(point, null);

    public bool IsVisible(float x, float y, Graphics? graphics)
    {
        BOOL isVisible;
        PInvokeGdiPlus.GdipIsVisiblePathPoint(
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
        PInvokeGdiPlus.GdipIsOutlineVisiblePathPoint(
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
        PInvokeGdiPlus.GdipAddPathLine(_nativePath, x1, y1, x2, y2).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Appends a series of connected line segments to the end of this <see cref="GraphicsPath"/>.
    /// </summary>
    /// <param name="points">An array of points that define the line segments to add.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <remarks>
    ///  <para>
    ///   If there are previous lines or curves in the figure, a line is added to connect the endpoint
    ///   of the previous segment the starting point of the line. The <paramref name="points"/> parameter
    ///   specifies an array of endpoints. The first two specify the first line. Each additional point
    ///   specifies the endpoint of a line segment whose starting point is the endpoint of the previous line.
    ///  </para>
    /// </remarks>
    public void AddLines(params PointF[] points) => AddLines(points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="AddLines(PointF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void AddLines(params ReadOnlySpan<PointF> points)
    {
        if (points.Length == 0)
        {
            throw new ArgumentException(null, nameof(points));
        }

        fixed (PointF* p = points)
        {
            PInvokeGdiPlus.GdipAddPathLine2(_nativePath, (GdiPlus.PointF*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddLine(Point pt1, Point pt2) => AddLine((float)pt1.X, pt1.Y, pt2.X, pt2.Y);

    public void AddLine(int x1, int y1, int x2, int y2) => AddLine((float)x1, y1, x2, y2);

    /// <inheritdoc cref="AddLines(PointF[])"/>
    public void AddLines(params Point[] points) => AddLines(points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="AddLines(PointF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void AddLines(params ReadOnlySpan<Point> points)
    {
        if (points.Length == 0)
        {
            throw new ArgumentException(null, nameof(points));
        }

        fixed (Point* p = points)
        {
            PInvokeGdiPlus.GdipAddPathLine2I(_nativePath, (GdiPlus.Point*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddArc(RectangleF rect, float startAngle, float sweepAngle) =>
        AddArc(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        PInvokeGdiPlus.GdipAddPathArc(_nativePath, x, y, width, height, startAngle, sweepAngle).ThrowIfFailed();
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
        PInvokeGdiPlus.GdipAddPathBezier(_nativePath, x1, y1, x2, y2, x3, y3, x4, y4).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Adds a sequence of connected cubic Bézier curves to the current figure.
    /// </summary>
    /// <param name="points">An array of points that define the curves.</param>
    public void AddBeziers(params PointF[] points) => AddBeziers(points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="AddBeziers(PointF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    internal
#endif
    void AddBeziers(params ReadOnlySpan<PointF> points)
    {
        fixed (PointF* p = points)
        {
            PInvokeGdiPlus.GdipAddPathBeziers(_nativePath, (GdiPlus.PointF*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4) =>
        AddBezier((float)pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);

    public void AddBezier(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4) =>
        AddBezier((float)x1, y1, x2, y2, x3, y3, x4, y4);

    /// <inheritdoc cref="AddBeziers(PointF[])"/>
    public void AddBeziers(params Point[] points) => AddBeziers(points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="AddBeziers(PointF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    internal
#endif
    void AddBeziers(params ReadOnlySpan<Point> points)
    {
        if (points.Length == 0)
            return;

        fixed (Point* p = points)
        {
            PInvokeGdiPlus.GdipAddPathBeziersI(_nativePath, (GdiPlus.Point*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <inheritdoc cref="AddCurve(PointF[], int, int, float)"/>
    public void AddCurve(params PointF[] points) => AddCurve(points.AsSpan(), 0.5f);

    /// <inheritdoc cref="AddCurve(PointF[], int, int, float)"/>
    public void AddCurve(PointF[] points, float tension) => AddCurve(points.AsSpan(), tension);

    /// <summary>
    ///  Adds a spline curve to the current figure. A cardinal spline curve is used because the
    ///  curve travels through each of the points in the array.
    /// </summary>
    /// <param name="points">An array points that define the curve.</param>
    /// <param name="offset">The index of the first point in the array to use.</param>
    /// <param name="numberOfSegments">
    ///  The number of segments to use when creating the curve. A segment can be thought of as
    ///  a line connecting two points.
    /// </param>
    /// <param name="tension">
    ///  A value that specifies the amount that the curve bends between control points.
    ///  Values greater than 1 produce unpredictable results.
    /// </param>
    public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension)
    {
        fixed (PointF* p = points)
        {
            PInvokeGdiPlus.GdipAddPathCurve3(
                _nativePath,
                (GdiPlus.PointF*)p,
                points.Length,
                offset,
                numberOfSegments,
                tension).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="AddCurve(PointF[], int, int, float)"/>
    public void AddCurve(params ReadOnlySpan<PointF> points) => AddCurve(points, 0.5f);
#endif

    /// <inheritdoc cref="AddCurve(PointF[], int, int, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void AddCurve(ReadOnlySpan<PointF> points, float tension)
    {
        fixed (PointF* p = points)
        {
            PInvokeGdiPlus.GdipAddPathCurve2(
                _nativePath,
                (GdiPlus.PointF*)p,
                points.Length,
                tension).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

    /// <inheritdoc cref="AddCurve(PointF[], int, int, float)"/>
    public void AddCurve(params Point[] points) => AddCurve(points.AsSpan(), 0.5f);

    /// <inheritdoc cref="AddCurve(PointF[], int, int, float)"/>
    public void AddCurve(Point[] points, float tension) => AddCurve(points.AsSpan(), tension);

    /// <inheritdoc cref="AddCurve(PointF[], int, int, float)"/>
    public void AddCurve(Point[] points, int offset, int numberOfSegments, float tension)
    {
        fixed (Point* p = points)
        {
            PInvokeGdiPlus.GdipAddPathCurve3I(
                _nativePath,
                (GdiPlus.Point*)p,
                points.Length,
                offset,
                numberOfSegments,
                tension).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="AddCurve(PointF[], int, int, float)"/>
    public void AddCurve(ReadOnlySpan<Point> points) => AddCurve(points, 0.5f);
#endif

    /// <inheritdoc cref="AddCurve(PointF[], int, int, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void AddCurve(ReadOnlySpan<Point> points, float tension)
    {
        fixed (Point* p = points)
        {
            PInvokeGdiPlus.GdipAddPathCurve2I(
                _nativePath,
                (GdiPlus.Point*)p,
                points.Length,
                tension).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

    /// <inheritdoc cref="AddClosedCurve(Point[], float)"/>
    public void AddClosedCurve(params PointF[] points) => AddClosedCurve(points, 0.5f);

    /// <inheritdoc cref="AddClosedCurve(Point[], float)"/>
    public void AddClosedCurve(PointF[] points, float tension) => AddClosedCurve(points.OrThrowIfNull().AsSpan(), tension);

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="AddClosedCurve(Point[], float)"/>
    public void AddClosedCurve(params ReadOnlySpan<PointF> points) => AddClosedCurve(points, 0.5f);
#endif

    /// <inheritdoc cref="AddClosedCurve(Point[], float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void AddClosedCurve(ReadOnlySpan<PointF> points, float tension)
    {
        fixed (PointF* p = points)
        {
            PInvokeGdiPlus.GdipAddPathClosedCurve2(_nativePath, (GdiPlus.PointF*)p, points.Length, tension).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <inheritdoc cref="AddClosedCurve(Point[], float)"/>
    public void AddClosedCurve(params Point[] points) => AddClosedCurve(points, 0.5f);

    /// <summary>
    ///  Adds a closed spline curve to the current figure. A cardinal spline curve is used because the
    ///  curve travels through each of the points in the array.
    /// </summary>
    /// <inheritdoc cref="AddCurve(PointF[], int, int, float)"/>
    public void AddClosedCurve(Point[] points, float tension) => AddClosedCurve(points.OrThrowIfNull().AsSpan(), tension);

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="AddClosedCurve(Point[], float)"/>
    public void AddClosedCurve(params ReadOnlySpan<Point> points) => AddClosedCurve(points, 0.5f);
#endif

    /// <inheritdoc cref="AddClosedCurve(Point[], float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void AddClosedCurve(ReadOnlySpan<Point> points, float tension)
    {
        fixed (Point* p = points)
        {
            PInvokeGdiPlus.GdipAddPathClosedCurve2I(_nativePath, (GdiPlus.Point*)p, points.Length, tension).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddRectangle(RectangleF rect)
    {
        PInvokeGdiPlus.GdipAddPathRectangle(
            _nativePath,
            rect.X, rect.Y, rect.Width, rect.Height).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Adds a series of rectangles to this path.
    /// </summary>
    /// <param name="rects">Array of rectangles to add.</param>
    public void AddRectangles(params RectangleF[] rects) => AddRectangles(rects.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="AddRectangles(RectangleF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void AddRectangles(params ReadOnlySpan<RectangleF> rects)
    {
        fixed (RectangleF* r = rects)
        {
            PInvokeGdiPlus.GdipAddPathRectangles(_nativePath, (RectF*)r, rects.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddRectangle(Rectangle rect) => AddRectangle((RectangleF)rect);

    /// <inheritdoc cref="AddRectangles(RectangleF[])"/>
    public void AddRectangles(params Rectangle[] rects) => AddRectangles(rects.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="AddRectangles(RectangleF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void AddRectangles(params ReadOnlySpan<Rectangle> rects)
    {
        fixed (Rectangle* r = rects)
        {
            PInvokeGdiPlus.GdipAddPathRectanglesI(_nativePath, (Rect*)r, rects.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="AddRoundedRectangle(RectangleF, SizeF)"/>
    public void AddRoundedRectangle(Rectangle rect, Size radius) =>
        AddRoundedRectangle((RectangleF)rect, radius);

    /// <summary>
    ///  Adds a rounded rectangle to this path.
    /// </summary>
    /// <param name="rect">The bounds of the rectangle to add.</param>
    /// <param name="radius">The radius width and height used to round the corners of the rectangle.</param>
    public void AddRoundedRectangle(RectangleF rect, SizeF radius)
    {
        StartFigure();
        AddArc(
            rect.Right - radius.Width,
            rect.Top,
            radius.Width,
            radius.Height,
            -90.0f, 90.0f);
        AddArc(
            rect.Right - radius.Width,
            rect.Bottom - radius.Height,
            radius.Width,
            radius.Height,
            0.0f, 90.0f);
        AddArc(
            rect.Left,
            rect.Bottom - radius.Height,
            radius.Width,
            radius.Height,
            90.0f, 90.0f);
        AddArc(
            rect.Left,
            rect.Top,
            radius.Width,
            radius.Height,
            180.0f, 90.0f);
        CloseFigure();
    }
#endif

    public void AddEllipse(RectangleF rect) =>
        AddEllipse(rect.X, rect.Y, rect.Width, rect.Height);

    public void AddEllipse(float x, float y, float width, float height)
    {
        PInvokeGdiPlus.GdipAddPathEllipse(_nativePath, x, y, width, height).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void AddEllipse(Rectangle rect) => AddEllipse(rect.X, rect.Y, rect.Width, rect.Height);

    public void AddEllipse(int x, int y, int width, int height) => AddEllipse((float)x, y, width, height);

    public void AddPie(Rectangle rect, float startAngle, float sweepAngle) =>
        AddPie((float)rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        PInvokeGdiPlus.GdipAddPathPie(
            _nativePath,
            x, y, width, height,
            startAngle,
            sweepAngle).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void AddPie(int x, int y, int width, int height, float startAngle, float sweepAngle) =>
        AddPie((float)x, y, width, height, startAngle, sweepAngle);

    /// <inheritdoc cref="AddPolygon(Point[])"/>
    public void AddPolygon(params PointF[] points) => AddPolygon(points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="AddPolygon(Point[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void AddPolygon(params ReadOnlySpan<PointF> points)
    {
        fixed (PointF* p = points)
        {
            PInvokeGdiPlus.GdipAddPathPolygon(_nativePath, (GdiPlus.PointF*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Adds a polygon to this path.
    /// </summary>
    /// <param name="points">The points that define the polygon.</param>
    public void AddPolygon(params Point[] points) => AddPolygon(points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="AddPolygon(Point[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void AddPolygon(params ReadOnlySpan<Point> points)
    {
        fixed (Point* p = points)
        {
            PInvokeGdiPlus.GdipAddPathPolygonI(_nativePath, (GdiPlus.Point*)p, points.Length).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void AddPath(GraphicsPath addingPath, bool connect)
    {
        ArgumentNullException.ThrowIfNull(addingPath);
        PInvokeGdiPlus.GdipAddPathPath(_nativePath, addingPath.Pointer(), connect).ThrowIfFailed();
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
            PInvokeGdiPlus.GdipAddPathString(
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
        PInvokeGdiPlus.GdipTransformPath(_nativePath, matrix.NativeMatrix).ThrowIfFailed();
        GC.KeepAlive(matrix);
        GC.KeepAlive(this);
    }

    public RectangleF GetBounds() => GetBounds(null);

    public RectangleF GetBounds(Matrix? matrix) => GetBounds(matrix, null);

    public RectangleF GetBounds(Matrix? matrix, Pen? pen)
    {
        RectF bounds;
        PInvokeGdiPlus.GdipGetPathWorldBounds(
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
        PInvokeGdiPlus.GdipFlattenPath(_nativePath, matrix.Pointer(), flatness).ThrowIfFailed();
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

        PInvokeGdiPlus.GdipWidenPath(_nativePath, pen.Pointer(), matrix.Pointer(), flatness).ThrowIfFailed();
        GC.KeepAlive(pen);
        GC.KeepAlive(matrix);
        GC.KeepAlive(this);
    }

    /// <inheritdoc cref="Warp(ReadOnlySpan{PointF}, RectangleF, Matrix?, WarpMode, float)"/>
    public void Warp(PointF[] destPoints, RectangleF srcRect) => Warp(destPoints, srcRect, null);

    /// <inheritdoc cref="Warp(ReadOnlySpan{PointF}, RectangleF, Matrix?, WarpMode, float)"/>
    public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix? matrix) =>
        Warp(destPoints, srcRect, matrix, WarpMode.Perspective);

    /// <inheritdoc cref="Warp(ReadOnlySpan{PointF}, RectangleF, Matrix?, WarpMode, float)"/>
    public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix? matrix, WarpMode warpMode) =>
        Warp(destPoints, srcRect, matrix, warpMode, 0.25f);

    /// <inheritdoc cref="Warp(ReadOnlySpan{PointF}, RectangleF, Matrix?, WarpMode, float)"/>
    public void Warp(PointF[] destPoints, RectangleF srcRect, Matrix? matrix, WarpMode warpMode, float flatness) =>
        Warp(destPoints.OrThrowIfNull().AsSpan(), srcRect, matrix, warpMode, flatness);

    /// <summary>
    ///  Applies a warp transform, defined by a rectangle and a parallelogram, to this <see cref="GraphicsPath"/>.
    /// </summary>
    /// <param name="destPoints">
    ///  An array of points that define a parallelogram to which the rectangle defined by <paramref name="srcRect"/>
    ///  is transformed. The array can contain either three or four elements. If the array contains three elements,
    ///  the lower-right corner of the parallelogram is implied by the first three points.
    /// </param>
    /// <param name="srcRect">
    ///  A rectangle that represents the rectangle that is transformed to the parallelogram defined by
    ///  <paramref name="destPoints"/>.
    /// </param>
    /// <param name="matrix">A matrix that specifies a geometric transform to apply to the path.</param>
    /// <param name="warpMode">Specifies whether this warp operation uses perspective or bilinear mode.</param>
    /// <param name="flatness">
    ///  A value from 0 through 1 that specifies how flat the resulting path is. For more information, see the
    ///  <see cref="Flatten(Matrix?, float)"/> methods.
    /// </param>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void Warp(
        ReadOnlySpan<PointF> destPoints,
        RectangleF srcRect,
        Matrix? matrix = default,
        WarpMode warpMode = WarpMode.Perspective,
        float flatness = 0.25f)
    {
        fixed (PointF* p = destPoints)
        {
            PInvokeGdiPlus.GdipWarpPath(
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
            PInvokeGdiPlus.GdipGetPointCount(_nativePath, &count).ThrowIfFailed();
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
            GetPathTypes(types);
            return types;
        }
    }

    /// <summary>
    ///  Gets the <see cref="PathPointType"/> types for the points in the path.
    /// </summary>
    /// <param name="destination">
    ///  Span to copy the types into. This should be at least as long as the <see cref="PointCount"/>.
    /// </param>
    /// <returns>
    ///  The count of types copied into the <paramref name="destination"/>.
    /// </returns>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    int GetPathTypes(Span<byte> destination)
    {
        if (destination.IsEmpty)
        {
            return 0;
        }

        fixed (byte* t = destination)
        {
            PInvokeGdiPlus.GdipGetPathTypes(_nativePath, t, destination.Length).ThrowIfFailed();
            GC.KeepAlive(this);
            return PointCount;
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
            GetPathPoints(points);
            return points;
        }
    }

    /// <summary>
    ///  Gets the points in the path.
    /// </summary>
    /// <param name="destination">
    ///  Span to copy the points into. This should be at least as long as the <see cref="PointCount"/>.
    /// </param>
    /// <returns>
    ///  The count of points copied into the <paramref name="destination"/>.
    /// </returns>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    int GetPathPoints(Span<PointF> destination)
    {
        if (destination.IsEmpty)
        {
            return 0;
        }

        fixed (PointF* p = destination)
        {
            PInvokeGdiPlus.GdipGetPathPoints(_nativePath, (GdiPlus.PointF*)p, destination.Length).ThrowIfFailed();
            GC.KeepAlive(this);
            return PointCount;
        }
    }
}
