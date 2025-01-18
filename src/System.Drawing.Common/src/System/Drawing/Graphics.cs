// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
#if NET9_0_OR_GREATER
using System.Drawing.Imaging.Effects;
#endif
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Drawing2DCoordinateSpace = System.Drawing.Drawing2D.CoordinateSpace;

namespace System.Drawing;

/// <summary>
///  Encapsulates a GDI+ drawing surface.
/// </summary>
public sealed unsafe partial class Graphics : MarshalByRefObject, IDisposable, IDeviceContext, IGraphics
{
    /// <summary>
    ///  The context state previous to the current Graphics context (the head of the stack).
    ///  We don't keep a GraphicsContext for the current context since it is available at any time from GDI+ and
    ///  we don't want to keep track of changes in it.
    /// </summary>
    private GraphicsContext? _previousContext;

    private static readonly Lock s_syncObject = new();

    // Object reference used for printing; it could point to a PrintPreviewGraphics to obtain the VisibleClipBounds, or
    // a DeviceContext holding a printer DC.
    private object? _printingHelper;

    // GDI+'s preferred HPALETTE.
    private static HPALETTE s_halftonePalette;

    // pointer back to the Image backing a specific graphic object
    private Image? _backingImage;

    /// <summary>
    ///  Handle to native DC - obtained from the GDI+ graphics object. We need to cache it to implement
    ///  IDeviceContext interface.
    /// </summary>
    private HDC _nativeHdc;

    public delegate bool DrawImageAbort(IntPtr callbackdata);

    /// <summary>
    /// Callback for EnumerateMetafile methods.
    /// This method can then call Metafile.PlayRecord to play the record that was just enumerated.
    /// </summary>
    /// <param name="recordType">if >= MinRecordType, it's an EMF+ record</param>
    /// <param name="flags">always 0 for EMF records</param>
    /// <param name="dataSize">size of the data, or 0 if no data</param>
    /// <param name="data">pointer to the data, or NULL if no data (UINT32 aligned)</param>
    /// <param name="callbackData">pointer to callbackData, if any</param>
    /// <returns>False to abort enumerating, true to continue.</returns>
    public delegate bool EnumerateMetafileProc(
        EmfPlusRecordType recordType,
        int flags,
        int dataSize,
        IntPtr data,
        PlayRecordCallback? callbackData);

    /// <summary>
    ///  Constructor to initialize this object from a native GDI+ Graphics pointer.
    /// </summary>
    private Graphics(GpGraphics* gdipNativeGraphics)
    {
        if (gdipNativeGraphics is null)
            throw new ArgumentNullException(nameof(gdipNativeGraphics));

        NativeGraphics = gdipNativeGraphics;
    }

    /// <summary>
    ///  Creates a new instance of the <see cref='Graphics'/> class from the specified handle to a device context.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHdc(IntPtr hdc)
    {
        return hdc == 0 ? throw new ArgumentNullException(nameof(hdc)) : FromHdcInternal(hdc);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHdcInternal(IntPtr hdc)
    {
        GpGraphics* nativeGraphics;
        Gdip.CheckStatus(PInvokeGdiPlus.GdipCreateFromHDC((HDC)hdc, &nativeGraphics));
        return new Graphics(nativeGraphics);
    }

    /// <summary>
    ///  Creates a new instance of the Graphics class from the specified handle to a device context and handle to a device.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHdc(IntPtr hdc, IntPtr hdevice)
    {
        GpGraphics* nativeGraphics;
        Gdip.CheckStatus(PInvokeGdiPlus.GdipCreateFromHDC2((HDC)hdc, (HANDLE)hdevice, &nativeGraphics));
        return new Graphics(nativeGraphics);
    }

    /// <summary>
    ///  Creates a new instance of the <see cref='Graphics'/> class from a window handle.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHwnd(IntPtr hwnd) => FromHwndInternal(hwnd);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Graphics FromHwndInternal(IntPtr hwnd)
    {
        GpGraphics* nativeGraphics;

        // This is one of the few places we need to manually ensure GDI+ is initialized. Other calls to PInvoke will do
        // this automatically, PInvokeCore cannot and as such needs to be manually initialized if we've never called
        // another PInvoke method.
        GdiPlusInitialization.EnsureInitialized();
        Gdip.CheckStatus(PInvokeGdiPlus.GdipCreateFromHWND((HWND)hwnd, &nativeGraphics));
        return new Graphics(nativeGraphics);
    }

    /// <summary>
    ///  Creates an instance of the <see cref='Graphics'/> class from an existing <see cref='Image'/>.
    /// </summary>
    public static Graphics FromImage(Image image)
    {
        ArgumentNullException.ThrowIfNull(image);

        if ((image.PixelFormat & PixelFormat.Indexed) != 0)
            throw new ArgumentException(SR.GdiplusCannotCreateGraphicsFromIndexedPixelFormat, nameof(image));

        GpGraphics* nativeGraphics;
        Gdip.CheckStatus(PInvokeGdiPlus.GdipGetImageGraphicsContext(image.Pointer(), &nativeGraphics));
        GC.KeepAlive(image);

        return new Graphics(nativeGraphics) { _backingImage = image };
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ReleaseHdcInternal(IntPtr hdc)
    {
        CheckStatus(!Gdip.Initialized ? Status.Ok : PInvokeGdiPlus.GdipReleaseDC(NativeGraphics, (HDC)hdc));
        _nativeHdc = HDC.Null;
    }

    /// <summary>
    ///  Deletes this <see cref='Graphics'/>, and frees the memory allocated for it.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            while (_previousContext is not null)
            {
                // Dispose entire stack.
                GraphicsContext? context = _previousContext.Previous;
                _previousContext.Dispose();
                _previousContext = context;
            }

            if (PrintingHelper is HdcHandle printerDC)
            {
                printerDC.Dispose();
                _printingHelper = null;
            }
        }

        if (!_nativeHdc.IsNull)
        {
            ReleaseHdc();
        }

        if (NativeGraphics is not null)
        {
            Status status = !Gdip.Initialized ? Status.Ok : PInvokeGdiPlus.GdipDeleteGraphics(NativeGraphics);
            NativeGraphics = null;
            Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
        }
    }

    ~Graphics() => Dispose(disposing: false);

    /// <summary>
    ///  Handle to native GDI+ graphics object. This object is created on demand.
    /// </summary>
    internal GpGraphics* NativeGraphics { get; private set; }

    nint IPointer<GpGraphics>.Pointer => (nint)NativeGraphics;

    public Region Clip
    {
        get
        {
            Region region = new();
            CheckStatus(PInvokeGdiPlus.GdipGetClip(NativeGraphics, region.NativeRegion));
            return region;
        }
        set => SetClip(value, Drawing2D.CombineMode.Replace);
    }

    public RectangleF ClipBounds
    {
        get
        {
            RectF rect;
            CheckStatus(PInvokeGdiPlus.GdipGetClipBounds(NativeGraphics, &rect));
            return rect;
        }
    }

    /// <summary>
    /// Gets or sets the <see cref='Drawing2D.CompositingMode'/> associated with this <see cref='Graphics'/>.
    /// </summary>
    public Drawing2D.CompositingMode CompositingMode
    {
        get
        {
            GdiPlus.CompositingMode mode;
            CheckStatus(PInvokeGdiPlus.GdipGetCompositingMode(NativeGraphics, &mode));
            return (Drawing2D.CompositingMode)mode;
        }
        set
        {
            if (value is < Drawing2D.CompositingMode.SourceOver or > Drawing2D.CompositingMode.SourceCopy)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Drawing2D.CompositingMode));

            CheckStatus(PInvokeGdiPlus.GdipSetCompositingMode(NativeGraphics, (GdiPlus.CompositingMode)value));
        }
    }

    public Drawing2D.CompositingQuality CompositingQuality
    {
        get
        {
            GdiPlus.CompositingQuality quality;
            CheckStatus(PInvokeGdiPlus.GdipGetCompositingQuality(NativeGraphics, &quality));
            return (Drawing2D.CompositingQuality)quality;
        }
        set
        {
            if (value is < Drawing2D.CompositingQuality.Invalid or > Drawing2D.CompositingQuality.AssumeLinear)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Drawing2D.CompositingQuality));

            CheckStatus(PInvokeGdiPlus.GdipSetCompositingQuality(NativeGraphics, (GdiPlus.CompositingQuality)value));
        }
    }

    public float DpiX
    {
        get
        {
            float dpi;
            CheckStatus(PInvokeGdiPlus.GdipGetDpiX(NativeGraphics, &dpi));
            return dpi;
        }
    }

    public float DpiY
    {
        get
        {
            float dpi;
            CheckStatus(PInvokeGdiPlus.GdipGetDpiY(NativeGraphics, &dpi));
            return dpi;
        }
    }

    /// <summary>
    /// Gets or sets the interpolation mode associated with this Graphics.
    /// </summary>
    public Drawing2D.InterpolationMode InterpolationMode
    {
        get
        {
            GdiPlus.InterpolationMode mode;
            CheckStatus(PInvokeGdiPlus.GdipGetInterpolationMode(NativeGraphics, &mode));
            return (Drawing2D.InterpolationMode)mode;
        }
        set
        {
            if (value is < Drawing2D.InterpolationMode.Invalid or > Drawing2D.InterpolationMode.HighQualityBicubic)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Drawing2D.InterpolationMode));

            CheckStatus(PInvokeGdiPlus.GdipSetInterpolationMode(NativeGraphics, (GdiPlus.InterpolationMode)value));
        }
    }

    public bool IsClipEmpty
    {
        get
        {
            BOOL isEmpty;
            CheckStatus(PInvokeGdiPlus.GdipIsClipEmpty(NativeGraphics, &isEmpty));
            return isEmpty;
        }
    }

    public bool IsVisibleClipEmpty
    {
        get
        {
            BOOL isEmpty;
            CheckStatus(PInvokeGdiPlus.GdipIsVisibleClipEmpty(NativeGraphics, &isEmpty));
            return isEmpty;
        }
    }

    public float PageScale
    {
        get
        {
            float scale;
            CheckStatus(PInvokeGdiPlus.GdipGetPageScale(NativeGraphics, &scale));
            return scale;
        }
        set => CheckStatus(PInvokeGdiPlus.GdipSetPageScale(NativeGraphics, value));
    }

    public GraphicsUnit PageUnit
    {
        get
        {
            Unit unit;
            CheckStatus(PInvokeGdiPlus.GdipGetPageUnit(NativeGraphics, &unit));
            return (GraphicsUnit)unit;
        }
        set
        {
            if (value is < GraphicsUnit.World or > GraphicsUnit.Millimeter)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(GraphicsUnit));

            CheckStatus(PInvokeGdiPlus.GdipSetPageUnit(NativeGraphics, (Unit)value));
        }
    }

    public PixelOffsetMode PixelOffsetMode
    {
        get
        {
            GdiPlus.PixelOffsetMode mode;
            CheckStatus(PInvokeGdiPlus.GdipGetPixelOffsetMode(NativeGraphics, &mode));
            return (PixelOffsetMode)mode;
        }
        set
        {
            if (value is < PixelOffsetMode.Invalid or > PixelOffsetMode.Half)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(PixelOffsetMode));

            CheckStatus(PInvokeGdiPlus.GdipSetPixelOffsetMode(NativeGraphics, (GdiPlus.PixelOffsetMode)value));
        }
    }

    public Point RenderingOrigin
    {
        get
        {
            int x, y;
            CheckStatus(PInvokeGdiPlus.GdipGetRenderingOrigin(NativeGraphics, &x, &y));
            return new Point(x, y);
        }
        set => CheckStatus(PInvokeGdiPlus.GdipSetRenderingOrigin(NativeGraphics, value.X, value.Y));
    }

    public Drawing2D.SmoothingMode SmoothingMode
    {
        get
        {
            GdiPlus.SmoothingMode mode;
            CheckStatus(PInvokeGdiPlus.GdipGetSmoothingMode(NativeGraphics, &mode));
            return (Drawing2D.SmoothingMode)mode;
        }
        set
        {
            if (value is < Drawing2D.SmoothingMode.Invalid or > Drawing2D.SmoothingMode.AntiAlias)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Drawing2D.SmoothingMode));

            CheckStatus(PInvokeGdiPlus.GdipSetSmoothingMode(NativeGraphics, (GdiPlus.SmoothingMode)value));
        }
    }

    public int TextContrast
    {
        get
        {
            uint textContrast;
            CheckStatus(PInvokeGdiPlus.GdipGetTextContrast(NativeGraphics, &textContrast));
            return (int)textContrast;
        }
        set => CheckStatus(PInvokeGdiPlus.GdipSetTextContrast(NativeGraphics, (uint)value));
    }

    /// <summary>
    ///  Gets or sets the rendering mode for text associated with this <see cref='Graphics'/>.
    /// </summary>
    public TextRenderingHint TextRenderingHint
    {
        get
        {
            GdiPlus.TextRenderingHint hint;
            CheckStatus(PInvokeGdiPlus.GdipGetTextRenderingHint(NativeGraphics, &hint));
            return (TextRenderingHint)hint;
        }
        set
        {
            if (value is < TextRenderingHint.SystemDefault or > TextRenderingHint.ClearTypeGridFit)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TextRenderingHint));

            CheckStatus(PInvokeGdiPlus.GdipSetTextRenderingHint(NativeGraphics, (GdiPlus.TextRenderingHint)value));
        }
    }

    /// <summary>
    ///  Gets or sets the world transform for this <see cref='Graphics'/>.
    /// </summary>
    public Matrix Transform
    {
        get
        {
            Matrix matrix = new();
            CheckStatus(PInvokeGdiPlus.GdipGetWorldTransform(NativeGraphics, matrix.NativeMatrix));
            return matrix;
        }
        set
        {
            CheckStatus(PInvokeGdiPlus.GdipSetWorldTransform(NativeGraphics, value.NativeMatrix));
            GC.KeepAlive(value);
        }
    }

    /// <summary>
    ///  Gets or sets the world transform elements for this <see cref="Graphics"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is a more performant alternative to <see cref="Transform"/> that does not need disposal.
    ///  </para>
    /// </remarks>
    public Matrix3x2 TransformElements
    {
        get
        {
            GdiPlus.Matrix* nativeMatrix;
            CheckStatus(PInvokeGdiPlus.GdipCreateMatrix(&nativeMatrix));

            try
            {
                CheckStatus(PInvokeGdiPlus.GdipGetWorldTransform(NativeGraphics, nativeMatrix));

                Matrix3x2 matrix = default;
                CheckStatus(PInvokeGdiPlus.GdipGetMatrixElements(nativeMatrix, (float*)&matrix));
                return matrix;
            }
            finally
            {
                if (nativeMatrix is not null)
                {
                    PInvokeGdiPlus.GdipDeleteMatrix(nativeMatrix);
                }
            }
        }
        set
        {
            GdiPlus.Matrix* nativeMatrix = Matrix.CreateNativeHandle(value);

            try
            {
                CheckStatus(PInvokeGdiPlus.GdipSetWorldTransform(NativeGraphics, nativeMatrix));
            }
            finally
            {
                if (nativeMatrix is not null)
                {
                    PInvokeGdiPlus.GdipDeleteMatrix(nativeMatrix);
                }
            }
        }
    }

    HDC IHdcContext.GetHdc() => (HDC)GetHdc();

    public IntPtr GetHdc()
    {
        HDC hdc;
        CheckStatus(PInvokeGdiPlus.GdipGetDC(NativeGraphics, &hdc));

        // Need to cache the hdc to be able to release with a call to IDeviceContext.ReleaseHdc().
        _nativeHdc = hdc;
        return _nativeHdc;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void ReleaseHdc(IntPtr hdc) => ReleaseHdcInternal(hdc);

    public void ReleaseHdc() => ReleaseHdcInternal(_nativeHdc);

    /// <summary>
    ///  Forces immediate execution of all operations currently on the stack.
    /// </summary>
    public void Flush() => Flush(Drawing2D.FlushIntention.Flush);

    /// <summary>
    ///  Forces execution of all operations currently on the stack.
    /// </summary>
    public void Flush(Drawing2D.FlushIntention intention) =>
        CheckStatus(PInvokeGdiPlus.GdipFlush(NativeGraphics, (GdiPlus.FlushIntention)intention));

    public void SetClip(Graphics g) => SetClip(g, Drawing2D.CombineMode.Replace);

    public void SetClip(Graphics g, Drawing2D.CombineMode combineMode)
    {
        ArgumentNullException.ThrowIfNull(g);

        CheckStatus(PInvokeGdiPlus.GdipSetClipGraphics(NativeGraphics, g.NativeGraphics, (GdiPlus.CombineMode)combineMode));
        GC.KeepAlive(g);
    }

    public void SetClip(Rectangle rect) => SetClip(rect, Drawing2D.CombineMode.Replace);

    public void SetClip(Rectangle rect, Drawing2D.CombineMode combineMode) => SetClip((RectangleF)rect, combineMode);

    public void SetClip(RectangleF rect) => SetClip(rect, Drawing2D.CombineMode.Replace);

    public void SetClip(RectangleF rect, Drawing2D.CombineMode combineMode) =>
        CheckStatus(PInvokeGdiPlus.GdipSetClipRect(NativeGraphics, rect.X, rect.Y, rect.Width, rect.Height, (GdiPlus.CombineMode)combineMode));

    public void SetClip(GraphicsPath path) => SetClip(path, Drawing2D.CombineMode.Replace);

    public void SetClip(GraphicsPath path, Drawing2D.CombineMode combineMode)
    {
        ArgumentNullException.ThrowIfNull(path);
        CheckStatus(PInvokeGdiPlus.GdipSetClipPath(NativeGraphics, path._nativePath, (GdiPlus.CombineMode)combineMode));
        GC.KeepAlive(path);
    }

    public void SetClip(Region region, Drawing2D.CombineMode combineMode)
    {
        ArgumentNullException.ThrowIfNull(region);
        CheckStatus(PInvokeGdiPlus.GdipSetClipRegion(NativeGraphics, region.NativeRegion, (GdiPlus.CombineMode)combineMode));
        GC.KeepAlive(region);
    }

    public void IntersectClip(Rectangle rect) => IntersectClip((RectangleF)rect);

    public void IntersectClip(RectangleF rect) =>
        CheckStatus(PInvokeGdiPlus.GdipSetClipRect(
            NativeGraphics,
            rect.X, rect.Y, rect.Width, rect.Height,
            GdiPlus.CombineMode.CombineModeIntersect));

    public void IntersectClip(Region region)
    {
        ArgumentNullException.ThrowIfNull(region);
        CheckStatus(PInvokeGdiPlus.GdipSetClipRegion(NativeGraphics, region.NativeRegion, GdiPlus.CombineMode.CombineModeIntersect));
        GC.KeepAlive(region);
    }

    public void ExcludeClip(Rectangle rect) =>
        CheckStatus(PInvokeGdiPlus.GdipSetClipRect(
            NativeGraphics,
            rect.X, rect.Y, rect.Width, rect.Height,
            GdiPlus.CombineMode.CombineModeExclude));

    public void ExcludeClip(Region region)
    {
        ArgumentNullException.ThrowIfNull(region);
        CheckStatus(PInvokeGdiPlus.GdipSetClipRegion(NativeGraphics, region.NativeRegion, GdiPlus.CombineMode.CombineModeExclude));
        GC.KeepAlive(region);
    }

    public void ResetClip() => CheckStatus(PInvokeGdiPlus.GdipResetClip(NativeGraphics));

    public void TranslateClip(float dx, float dy) => CheckStatus(PInvokeGdiPlus.GdipTranslateClip(NativeGraphics, dx, dy));

    public void TranslateClip(int dx, int dy) => CheckStatus(PInvokeGdiPlus.GdipTranslateClip(NativeGraphics, dx, dy));

    public bool IsVisible(int x, int y) => IsVisible((float)x, y);

    public bool IsVisible(Point point) => IsVisible(point.X, point.Y);

    public bool IsVisible(float x, float y)
    {
        BOOL isVisible;
        CheckStatus(PInvokeGdiPlus.GdipIsVisiblePoint(NativeGraphics, x, y, &isVisible));
        return isVisible;
    }

    public bool IsVisible(PointF point) => IsVisible(point.X, point.Y);

    public bool IsVisible(int x, int y, int width, int height) => IsVisible((float)x, y, width, height);

    public bool IsVisible(Rectangle rect) => IsVisible((float)rect.X, rect.Y, rect.Width, rect.Height);

    public bool IsVisible(float x, float y, float width, float height)
    {
        BOOL isVisible;
        CheckStatus(PInvokeGdiPlus.GdipIsVisibleRect(NativeGraphics, x, y, width, height, &isVisible));
        return isVisible;
    }

    public bool IsVisible(RectangleF rect) => IsVisible(rect.X, rect.Y, rect.Width, rect.Height);

    /// <summary>
    ///  Resets the world transform to identity.
    /// </summary>
    public void ResetTransform() => CheckStatus(PInvokeGdiPlus.GdipResetWorldTransform(NativeGraphics));

    /// <summary>
    ///  Multiplies the <see cref='Matrix'/> that represents the world transform and <paramref name="matrix"/>.
    /// </summary>
    public void MultiplyTransform(Matrix matrix) => MultiplyTransform(matrix, MatrixOrder.Prepend);

    /// <summary>
    ///  Multiplies the <see cref='Matrix'/> that represents the world transform and <paramref name="matrix"/>.
    /// </summary>
    public void MultiplyTransform(Matrix matrix, MatrixOrder order)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        CheckStatus(PInvokeGdiPlus.GdipMultiplyWorldTransform(NativeGraphics, matrix.NativeMatrix, (GdiPlus.MatrixOrder)order));
        GC.KeepAlive(matrix);
    }

    public void TranslateTransform(float dx, float dy) => TranslateTransform(dx, dy, MatrixOrder.Prepend);

    public void TranslateTransform(float dx, float dy, MatrixOrder order) =>
        CheckStatus(PInvokeGdiPlus.GdipTranslateWorldTransform(NativeGraphics, dx, dy, (GdiPlus.MatrixOrder)order));

    public void ScaleTransform(float sx, float sy) => ScaleTransform(sx, sy, MatrixOrder.Prepend);

    public void ScaleTransform(float sx, float sy, MatrixOrder order) =>
        CheckStatus(PInvokeGdiPlus.GdipScaleWorldTransform(NativeGraphics, sx, sy, (GdiPlus.MatrixOrder)order));

    public void RotateTransform(float angle) => RotateTransform(angle, MatrixOrder.Prepend);

    public void RotateTransform(float angle, MatrixOrder order) =>
        CheckStatus(PInvokeGdiPlus.GdipRotateWorldTransform(NativeGraphics, angle, (GdiPlus.MatrixOrder)order));

    /// <summary>
    ///  Draws an arc from the specified ellipse.
    /// </summary>
    public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        ArgumentNullException.ThrowIfNull(pen);

        CheckErrorStatus(PInvokeGdiPlus.GdipDrawArc(
            NativeGraphics,
            pen.NativePen,
            x, y, width, height,
            startAngle,
            sweepAngle));

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws an arc from the specified ellipse.
    /// </summary>
    public void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle) =>
        DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    /// <summary>
    /// Draws an arc from the specified ellipse.
    /// </summary>
    public void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle)
        => DrawArc(pen, (float)x, y, width, height, startAngle, sweepAngle);

    /// <summary>
    ///  Draws an arc from the specified ellipse.
    /// </summary>
    public void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle) =>
        DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    /// <summary>
    ///  Draws a cubic Bezier curve defined by four ordered pairs that represent points.
    /// </summary>
    public void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
        ArgumentNullException.ThrowIfNull(pen);

        CheckErrorStatus(PInvokeGdiPlus.GdipDrawBezier(
            NativeGraphics,
            pen.NativePen,
            x1, y1, x2, y2, x3, y3, x4, y4));

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws a cubic Bezier curve defined by four points.
    /// </summary>
    public void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4) =>
        DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);

    /// <summary>
    ///  Draws a cubic Bezier curve defined by four points.
    /// </summary>
    public void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4) =>
        DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);

    /// <summary>
    ///  Draws the outline of a rectangle specified by <paramref name="rect"/>.
    /// </summary>
    /// <param name="pen">A Pen that determines the color, width, and style of the rectangle.</param>
    /// <param name="rect">A Rectangle structure that represents the rectangle to draw.</param>
    public void DrawRectangle(Pen pen, RectangleF rect) => DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

    /// <summary>
    ///  Draws the outline of a rectangle specified by <paramref name="rect"/>.
    /// </summary>
    public void DrawRectangle(Pen pen, Rectangle rect) => DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="DrawRoundedRectangle(Pen, RectangleF, SizeF)"/>
    public void DrawRoundedRectangle(Pen pen, Rectangle rect, Size radius) =>
        DrawRoundedRectangle(pen, (RectangleF)rect, radius);

    /// <summary>
    ///  Draws the outline of the specified rounded rectangle.
    /// </summary>
    /// <param name="pen">The <see cref="Pen"/> to draw the outline with.</param>
    /// <param name="rect">The bounds of the rounded rectangle.</param>
    /// <param name="radius">The radius width and height used to round the corners of the rectangle.</param>
    public void DrawRoundedRectangle(Pen pen, RectangleF rect, SizeF radius)
    {
        using GraphicsPath path = new();
        path.AddRoundedRectangle(rect, radius);
        DrawPath(pen, path);
    }
#endif

    /// <summary>
    ///  Draws the outline of the specified rectangle.
    /// </summary>
    public void DrawRectangle(Pen pen, float x, float y, float width, float height)
    {
        ArgumentNullException.ThrowIfNull(pen);
        CheckErrorStatus(PInvokeGdiPlus.GdipDrawRectangle(NativeGraphics, pen.NativePen, x, y, width, height));
        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws the outline of the specified rectangle.
    /// </summary>
    public void DrawRectangle(Pen pen, int x, int y, int width, int height)
        => DrawRectangle(pen, (float)x, y, width, height);

    /// <inheritdoc cref="DrawRectangles(Pen, Rectangle[])"/>
    public void DrawRectangles(Pen pen, params RectangleF[] rects) => DrawRectangles(pen, rects.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawRectangles(Pen, Rectangle[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawRectangles(Pen pen, params ReadOnlySpan<RectangleF> rects)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (RectangleF* r = rects)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawRectangles(NativeGraphics, pen.NativePen, (RectF*)r, rects.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws the outlines of a series of rectangles.
    /// </summary>
    /// <param name="pen"><see cref="Pen"/> that determines the color, width, and style of the outlines of the rectangles.</param>
    /// <param name="rects">An array of <see cref="Rectangle"/> structures that represents the rectangles to draw.</param>
    public void DrawRectangles(Pen pen, params Rectangle[] rects) => DrawRectangles(pen, rects.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawRectangles(Pen, Rectangle[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawRectangles(Pen pen, params ReadOnlySpan<Rectangle> rects)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (Rectangle* r = rects)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawRectanglesI(NativeGraphics, pen.NativePen, (Rect*)r, rects.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws the outline of an ellipse defined by a bounding rectangle.
    /// </summary>
    public void DrawEllipse(Pen pen, RectangleF rect) => DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);

    /// <summary>
    ///  Draws the outline of an ellipse defined by a bounding rectangle.
    /// </summary>
    public void DrawEllipse(Pen pen, float x, float y, float width, float height)
    {
        ArgumentNullException.ThrowIfNull(pen);
        CheckErrorStatus(PInvokeGdiPlus.GdipDrawEllipse(NativeGraphics, pen.NativePen, x, y, width, height));
        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws the outline of an ellipse specified by a bounding rectangle.
    /// </summary>
    public void DrawEllipse(Pen pen, Rectangle rect) => DrawEllipse(pen, (float)rect.X, rect.Y, rect.Width, rect.Height);

    /// <summary>
    ///  Draws the outline of an ellipse defined by a bounding rectangle.
    /// </summary>
    public void DrawEllipse(Pen pen, int x, int y, int width, int height) => DrawEllipse(pen, (float)x, y, width, height);

    /// <summary>
    ///  Draws the outline of a pie section defined by an ellipse and two radial lines.
    /// </summary>
    public void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle) =>
        DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    /// <summary>
    ///  Draws the outline of a pie section defined by an ellipse and two radial lines.
    /// </summary>
    public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        ArgumentNullException.ThrowIfNull(pen);
        CheckErrorStatus(PInvokeGdiPlus.GdipDrawPie(NativeGraphics, pen.NativePen, x, y, width, height, startAngle, sweepAngle));
        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws the outline of a pie section defined by an ellipse and two radial lines.
    /// </summary>
    public void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle) =>
        DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    /// <summary>
    ///  Draws the outline of a pie section defined by an ellipse and two radial lines.
    /// </summary>
    public void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle) =>
        DrawPie(pen, (float)x, y, width, height, startAngle, sweepAngle);

    /// <inheritdoc cref="DrawPolygon(Pen, Point[])"/>
    public void DrawPolygon(Pen pen, params PointF[] points) => DrawPolygon(pen, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawPolygon(Pen, Point[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawPolygon(Pen pen, params ReadOnlySpan<PointF> points)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawPolygon(NativeGraphics, pen.NativePen, (GdiPlus.PointF*)p, points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws the outline of a polygon defined by an array of points.
    /// </summary>
    /// <param name="pen">The <see cref="Pen"/> to draw the outline with.</param>
    /// <param name="points">An array of <see cref="Point"/> structures that represent the vertices of the polygon.</param>
    public void DrawPolygon(Pen pen, params Point[] points) => DrawPolygon(pen, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawPolygon(Pen, Point[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawPolygon(Pen pen, params ReadOnlySpan<Point> points)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawPolygonI(NativeGraphics, pen.NativePen, (GdiPlus.Point*)p, points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws the lines and curves defined by a <see cref='GraphicsPath'/>.
    /// </summary>
    public void DrawPath(Pen pen, GraphicsPath path)
    {
        ArgumentNullException.ThrowIfNull(pen);
        ArgumentNullException.ThrowIfNull(path);

        CheckErrorStatus(PInvokeGdiPlus.GdipDrawPath(NativeGraphics, pen.NativePen, path._nativePath));

        GC.KeepAlive(pen);
        GC.KeepAlive(path);
    }

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
    public void DrawCurve(Pen pen, params PointF[] points) => DrawCurve(pen, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawCurve(Pen pen, params ReadOnlySpan<PointF> points)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawCurve(NativeGraphics, pen.NativePen, (GdiPlus.PointF*)p, points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
    public void DrawCurve(Pen pen, PointF[] points, float tension) =>
        DrawCurve(pen, points.OrThrowIfNull().AsSpan(), tension);

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawCurve(Pen pen, ReadOnlySpan<PointF> points, float tension)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawCurve2(
                NativeGraphics,
                pen.NativePen,
                (GdiPlus.PointF*)p, points.Length,
                tension));
        }

        GC.KeepAlive(pen);
    }

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
    public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments) =>
        DrawCurve(pen, points, offset, numberOfSegments, 0.5f);

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
    public void DrawCurve(Pen pen, ReadOnlySpan<PointF> points, int offset, int numberOfSegments) =>
        DrawCurve(pen, points, offset, numberOfSegments, 0.5f);
#endif

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
    public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension) =>
        DrawCurve(pen, points.OrThrowIfNull().AsSpan(), offset, numberOfSegments, tension);

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawCurve(Pen pen, ReadOnlySpan<PointF> points, int offset, int numberOfSegments, float tension)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawCurve3(
                NativeGraphics,
                pen.NativePen,
                (GdiPlus.PointF*)p, points.Length,
                offset,
                numberOfSegments,
                tension));
        }

        GC.KeepAlive(pen);
    }

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
    public void DrawCurve(Pen pen, params Point[] points) => DrawCurve(pen, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawCurve(Pen pen, params ReadOnlySpan<Point> points)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawCurveI(NativeGraphics, pen.NativePen, (GdiPlus.Point*)p, points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
    public void DrawCurve(Pen pen, Point[] points, float tension) =>
        DrawCurve(pen, points.OrThrowIfNull().AsSpan(), tension);

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawCurve(Pen pen, ReadOnlySpan<Point> points, float tension)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawCurve2I(
                NativeGraphics,
                pen.NativePen,
                (GdiPlus.Point*)p, points.Length,
                tension));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws a curve defined by an array of points.
    /// </summary>
    /// <param name="pen">The <see cref="Pen"/> to draw the curve with.</param>
    /// <param name="points">An array of points that define the curve.</param>
    /// <param name="offset">The index of the first point in the array to draw.</param>
    /// <param name="numberOfSegments">The number of segments to draw.</param>
    /// <param name="tension">A value greater than, or equal to zero that specifies the tension of the curve.</param>
    public void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension) =>
        DrawCurve(pen, points.OrThrowIfNull().AsSpan(), offset, numberOfSegments, tension);

    /// <inheritdoc cref="DrawCurve(Pen, Point[], int, int, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawCurve(Pen pen, ReadOnlySpan<Point> points, int offset, int numberOfSegments, float tension)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawCurve3I(
                NativeGraphics,
                pen.NativePen,
                (GdiPlus.Point*)p, points.Length,
                offset,
                numberOfSegments,
                tension));
        }

        GC.KeepAlive(pen);
    }

    /// <inheritdoc cref="DrawClosedCurve(Pen, PointF[], float, FillMode)"/>
    public void DrawClosedCurve(Pen pen, params PointF[] points) =>
        DrawClosedCurve(pen, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawClosedCurve(Pen, PointF[], float, FillMode)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawClosedCurve(Pen pen, params ReadOnlySpan<PointF> points)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawClosedCurve(
                NativeGraphics,
                pen.NativePen,
                (GdiPlus.PointF*)p, points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws a closed curve defined by an array of points.
    /// </summary>
    /// <param name="pen">The <see cref="Pen"/> to draw the closed curve with.</param>
    /// <param name="points">An array of points that define the closed curve.</param>
    /// <param name="tension">A value greater than, or equal to zero that specifies the tension of the curve.</param>
    /// <param name="fillmode">A <see cref="FillMode"/> enumeration that specifies the fill mode of the curve.</param>
    public void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode) =>
        DrawClosedCurve(pen, points.OrThrowIfNull().AsSpan(), tension, fillmode);

    /// <inheritdoc cref="DrawClosedCurve(Pen, PointF[], float, FillMode)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawClosedCurve(Pen pen, ReadOnlySpan<PointF> points, float tension, FillMode fillmode)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawClosedCurve2(
                NativeGraphics,
                pen.NativePen,
                (GdiPlus.PointF*)p, points.Length,
                tension));
        }

        GC.KeepAlive(pen);
    }

    /// <inheritdoc cref="DrawClosedCurve(Pen, PointF[], float, FillMode)"/>
    public void DrawClosedCurve(Pen pen, params Point[] points) => DrawClosedCurve(pen, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawClosedCurve(Pen, PointF[], float, FillMode)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawClosedCurve(Pen pen, params ReadOnlySpan<Point> points)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawClosedCurveI(
                NativeGraphics,
                pen.NativePen,
                (GdiPlus.Point*)p, points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <inheritdoc cref="DrawClosedCurve(Pen, PointF[], float, FillMode)"/>

    public void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode) =>
        DrawClosedCurve(pen, points.OrThrowIfNull().AsSpan(), tension, fillmode);

    /// <inheritdoc cref="DrawClosedCurve(Pen, PointF[], float, FillMode)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawClosedCurve(Pen pen, ReadOnlySpan<Point> points, float tension, FillMode fillmode)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawClosedCurve2I(
                NativeGraphics,
                pen.NativePen,
                (GdiPlus.Point*)p, points.Length,
                tension));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Fills the entire drawing surface with the specified color.
    /// </summary>
    public void Clear(Color color) => CheckStatus(PInvokeGdiPlus.GdipGraphicsClear(NativeGraphics, (uint)color.ToArgb()));

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="FillRoundedRectangle(Brush, RectangleF, SizeF)"/>
    public void FillRoundedRectangle(Brush brush, Rectangle rect, Size radius) =>
        FillRoundedRectangle(brush, (RectangleF)rect, radius);

    /// <summary>
    ///  Fills the interior of a rounded rectangle with a <see cref='Brush'/>.
    /// </summary>
    /// <param name="brush">The <see cref="Brush"/> to fill the rounded rectangle with.</param>
    /// <param name="rect">The bounds of the rounded rectangle.</param>
    /// <param name="radius">The radius width and height used to round the corners of the rectangle.</param>
    public void FillRoundedRectangle(Brush brush, RectangleF rect, SizeF radius)
    {
        using GraphicsPath path = new();
        path.AddRoundedRectangle(rect, radius);
        FillPath(brush, path);
    }
#endif

    /// <summary>
    ///  Fills the interior of a rectangle with a <see cref='Brush'/>.
    /// </summary>
    public void FillRectangle(Brush brush, RectangleF rect) => FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);

    /// <summary>
    ///  Fills the interior of a rectangle with a <see cref='Brush'/>.
    /// </summary>
    public void FillRectangle(Brush brush, float x, float y, float width, float height)
    {
        ArgumentNullException.ThrowIfNull(brush);

        CheckErrorStatus(PInvokeGdiPlus.GdipFillRectangle(
            NativeGraphics,
            brush.NativeBrush,
            x, y, width, height));

        GC.KeepAlive(brush);
    }

    /// <summary>
    ///  Fills the interior of a rectangle with a <see cref='Brush'/>.
    /// </summary>
    public void FillRectangle(Brush brush, Rectangle rect) => FillRectangle(brush, (float)rect.X, rect.Y, rect.Width, rect.Height);

    /// <summary>
    ///  Fills the interior of a rectangle with a <see cref='Brush'/>.
    /// </summary>
    public void FillRectangle(Brush brush, int x, int y, int width, int height) => FillRectangle(brush, (float)x, y, width, height);

    /// <summary>
    ///  Fills the interiors of a series of rectangles with a <see cref='Brush'/>.
    /// </summary>
    /// <param name="brush">The <see cref="Brush"/> to fill the rectangles with.</param>
    /// <param name="rects">An array of rectangles to fill.</param>
    public void FillRectangles(Brush brush, params RectangleF[] rects)
    {
        ArgumentNullException.ThrowIfNull(rects);
        FillRectangles(brush, rects.AsSpan());
    }

    /// <inheritdoc cref="FillRectangles(Brush, RectangleF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void FillRectangles(Brush brush, params ReadOnlySpan<RectangleF> rects)
    {
        ArgumentNullException.ThrowIfNull(brush);

        fixed (RectangleF* r = rects)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipFillRectangles(NativeGraphics, brush.NativeBrush, (RectF*)r, rects.Length));
        }

        GC.KeepAlive(brush);
    }

    /// <inheritdoc cref="FillRectangles(Brush, RectangleF[])"/>
    public void FillRectangles(Brush brush, params Rectangle[] rects) =>
        FillRectangles(brush, rects.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="FillRectangles(Brush, RectangleF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void FillRectangles(Brush brush, params ReadOnlySpan<Rectangle> rects)
    {
        ArgumentNullException.ThrowIfNull(brush);

        fixed (Rectangle* r = rects)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipFillRectanglesI(NativeGraphics, brush.NativeBrush, (Rect*)r, rects.Length));
        }

        GC.KeepAlive(brush);
    }

    /// <inheritdoc cref="FillPolygon(Brush, Point[], FillMode)"/>
    public void FillPolygon(Brush brush, params PointF[] points) => FillPolygon(brush, points, FillMode.Alternate);

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="FillPolygon(Brush, Point[], FillMode)"/>
    public void FillPolygon(Brush brush, params ReadOnlySpan<PointF> points) => FillPolygon(brush, points, FillMode.Alternate);
#endif

    /// <inheritdoc cref="FillPolygon(Brush, Point[], FillMode)"/>
    public void FillPolygon(Brush brush, PointF[] points, FillMode fillMode) =>
        FillPolygon(brush, points.OrThrowIfNull().AsSpan(), fillMode);

    /// <inheritdoc cref="FillPolygon(Brush, Point[], FillMode)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void FillPolygon(Brush brush, ReadOnlySpan<PointF> points, FillMode fillMode)
    {
        ArgumentNullException.ThrowIfNull(brush);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipFillPolygon(
                NativeGraphics,
                brush.NativeBrush,
                (GdiPlus.PointF*)p, points.Length,
                (GdiPlus.FillMode)fillMode));
        }

        GC.KeepAlive(brush);
    }

    /// <inheritdoc cref="FillPolygon(Brush, Point[], FillMode)"/>
    public void FillPolygon(Brush brush, Point[] points) => FillPolygon(brush, points, FillMode.Alternate);

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="FillPolygon(Brush, Point[], FillMode)"/>
    public void FillPolygon(Brush brush, params ReadOnlySpan<Point> points) => FillPolygon(brush, points, FillMode.Alternate);
#endif

    /// <summary>
    ///  Fills the interior of a polygon defined by an array of points.
    /// </summary>
    /// <param name="brush">The <see cref="Brush"/> to fill the polygon with.</param>
    /// <param name="points">An array points that represent the vertices of the polygon.</param>
    /// <param name="fillMode">A <see cref="FillMode"/> enumeration that specifies the fill mode of the polygon.</param>
    public void FillPolygon(Brush brush, Point[] points, FillMode fillMode) =>
        FillPolygon(brush, points.OrThrowIfNull().AsSpan(), fillMode);

    /// <inheritdoc cref="FillPolygon(Brush, Point[], FillMode)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void FillPolygon(Brush brush, ReadOnlySpan<Point> points, FillMode fillMode)
    {
        ArgumentNullException.ThrowIfNull(brush);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipFillPolygonI(
                NativeGraphics,
                brush.NativeBrush,
                (GdiPlus.Point*)p, points.Length,
                (GdiPlus.FillMode)fillMode));
        }

        GC.KeepAlive(brush);
    }

    /// <summary>
    ///  Fills the interior of an ellipse defined by a bounding rectangle.
    /// </summary>
    public void FillEllipse(Brush brush, RectangleF rect) => FillEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);

    /// <summary>
    ///  Fills the interior of an ellipse defined by a bounding rectangle.
    /// </summary>
    public void FillEllipse(Brush brush, float x, float y, float width, float height)
    {
        ArgumentNullException.ThrowIfNull(brush);

        CheckErrorStatus(PInvokeGdiPlus.GdipFillEllipse(
            NativeGraphics,
            brush.NativeBrush,
            x, y, width, height));

        GC.KeepAlive(brush);
    }

    /// <summary>
    ///  Fills the interior of an ellipse defined by a bounding rectangle.
    /// </summary>
    public void FillEllipse(Brush brush, Rectangle rect) => FillEllipse(brush, (float)rect.X, rect.Y, rect.Width, rect.Height);

    /// <summary>
    ///  Fills the interior of an ellipse defined by a bounding rectangle.
    /// </summary>
    public void FillEllipse(Brush brush, int x, int y, int width, int height) => FillEllipse(brush, (float)x, y, width, height);

    /// <summary>
    ///  Fills the interior of a pie section defined by an ellipse and two radial lines.
    /// </summary>
    public void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle) =>
        FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    /// <summary>
    ///  Fills the interior of a pie section defined by an ellipse and two radial lines.
    /// </summary>
    /// <param name="brush">A Brush that determines the characteristics of the fill.</param>
    /// <param name="rect">
    ///  A Rectangle structure that represents the bounding rectangle that defines the ellipse from which
    ///  the pie section comes.
    /// </param>
    /// <param name="startAngle">
    ///  Angle in degrees measured clockwise from the x-axis to the first side of the pie section.
    /// </param>
    /// <param name="sweepAngle">
    ///  Angle in degrees measured clockwise from the <paramref name="startAngle"/> parameter
    ///  to the second side of the pie section.
    /// </param>
    public void FillPie(Brush brush, RectangleF rect, float startAngle, float sweepAngle) =>
        FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);

    /// <summary>
    ///  Fills the interior of a pie section defined by an ellipse and two radial lines.
    /// </summary>
    public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
    {
        ArgumentNullException.ThrowIfNull(brush);

        CheckErrorStatus(PInvokeGdiPlus.GdipFillPie(
            NativeGraphics,
            brush.NativeBrush,
            x, y, width, height,
            startAngle,
            sweepAngle));

        GC.KeepAlive(brush);
    }

    /// <summary>
    ///  Fills the interior of a pie section defined by an ellipse and two radial lines.
    /// </summary>
    public void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle)
        => FillPie(brush, (float)x, y, width, height, startAngle, sweepAngle);

    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
    public void FillClosedCurve(Brush brush, params PointF[] points) =>
        FillClosedCurve(brush, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void FillClosedCurve(Brush brush, params ReadOnlySpan<PointF> points)
    {
        ArgumentNullException.ThrowIfNull(brush);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipFillClosedCurve(
                NativeGraphics,
                brush.NativeBrush,
                (GdiPlus.PointF*)p, points.Length));
        }

        GC.KeepAlive(brush);
    }

    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
    public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode) =>
        FillClosedCurve(brush, points, fillmode, 0.5f);

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
    public void FillClosedCurve(Brush brush, ReadOnlySpan<PointF> points, FillMode fillmode) =>
        FillClosedCurve(brush, points, fillmode, 0.5f);
#endif

    /// <summary>
    ///  Fills the interior of a closed curve defined by an array of points.
    /// </summary>
    /// <param name="brush">The <see cref="Brush"/> to fill the closed curve with.</param>
    /// <param name="points">An array of points that make up the closed curve.</param>
    /// <param name="fillmode">A <see cref="FillMode"/> enumeration that specifies the fill mode of the closed curve.</param>
    /// <param name="tension">A value greater than, or equal to zero that specifies the tension of the curve.</param>
    public void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension) =>
        FillClosedCurve(brush, points.OrThrowIfNull().AsSpan(), fillmode, tension);

    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void FillClosedCurve(Brush brush, ReadOnlySpan<PointF> points, FillMode fillmode, float tension)
    {
        ArgumentNullException.ThrowIfNull(brush);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipFillClosedCurve2(
                NativeGraphics,
                brush.NativeBrush,
                (GdiPlus.PointF*)p, points.Length,
                tension,
                (GdiPlus.FillMode)fillmode));
        }

        GC.KeepAlive(brush);
    }

    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
    public void FillClosedCurve(Brush brush, params Point[] points) =>
        FillClosedCurve(brush, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void FillClosedCurve(Brush brush, params ReadOnlySpan<Point> points)
    {
        ArgumentNullException.ThrowIfNull(brush);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipFillClosedCurveI(
                NativeGraphics,
                brush.NativeBrush,
                (GdiPlus.Point*)p, points.Length));
        }

        GC.KeepAlive(brush);
    }

    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
    public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode) =>
        FillClosedCurve(brush, points, fillmode, 0.5f);

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
    public void FillClosedCurve(Brush brush, ReadOnlySpan<Point> points, FillMode fillmode) =>
        FillClosedCurve(brush, points, fillmode, 0.5f);

#endif

    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
    public void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension) =>
        FillClosedCurve(brush, points.OrThrowIfNull().AsSpan(), fillmode, tension);

    /// <inheritdoc cref="FillClosedCurve(Brush, PointF[], FillMode, float)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void FillClosedCurve(Brush brush, ReadOnlySpan<Point> points, FillMode fillmode, float tension)
    {
        ArgumentNullException.ThrowIfNull(brush);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipFillClosedCurve2I(
                NativeGraphics,
                brush.NativeBrush,
                (GdiPlus.Point*)p, points.Length,
                tension,
                (GdiPlus.FillMode)fillmode));
        }

        GC.KeepAlive(brush);
    }

    /// <summary>
    ///  Draws the specified text at the specified location with the specified <see cref="Brush"/> and
    ///  <see cref="Font"/> objects.
    /// </summary>
    /// <param name="s">The text to draw.</param>
    /// <param name="font"><see cref="Font"/> that defines the text format.</param>
    /// <param name="brush"><see cref="Brush"/> that determines the color and texture of the drawn text.</param>
    /// <param name="x">The x-coordinate of the upper-left corner of the drawn text.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the drawn text.</param>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="brush"/> is <see langword="null"/>. -or- <paramref name="font"/> is <see langword="null"/>.
    /// </exception>
    public void DrawString(string? s, Font font, Brush brush, float x, float y) =>
        DrawString(s, font, brush, new RectangleF(x, y, 0, 0), null);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="DrawString(string?, Font, Brush, float, float)"/>
    public void DrawString(ReadOnlySpan<char> s, Font font, Brush brush, float x, float y) =>
        DrawString(s, font, brush, new RectangleF(x, y, 0, 0), null);
#endif

    /// <summary>
    ///  Draws the specified text at the specified location with the specified <see cref="Brush"/> and
    ///  <see cref="Font"/> objects.
    /// </summary>
    /// <param name="s">The text to draw.</param>
    /// <param name="font"><see cref="Font"/> that defines the text format.</param>
    /// <param name="brush"><see cref="Brush"/> that determines the color and texture of the drawn text.</param>
    /// <param name="point"><see cref="PointF"/>structure that specifies the upper-left corner of the drawn text.</param>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="brush"/> is <see langword="null"/>. -or- <paramref name="font"/> is <see langword="null"/>.
    /// </exception>
    public void DrawString(string? s, Font font, Brush brush, PointF point) =>
        DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), null);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="DrawString(string?, Font, Brush, PointF)"/>
    public void DrawString(ReadOnlySpan<char> s, Font font, Brush brush, PointF point) =>
        DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), null);
#endif

    /// <summary>
    ///  Draws the specified text at the specified location with the specified <see cref="Brush"/> and
    ///  <see cref="Font"/> objects using the formatting attributes of the specified <see cref="StringFormat"/>.
    /// </summary>
    /// <param name="s">The text to draw.</param>
    /// <param name="font"><see cref="Font"/> that defines the text format.</param>
    /// <param name="brush"><see cref="Brush"/> that determines the color and texture of the drawn text.</param>
    /// <param name="x">The x-coordinate of the upper-left corner of the drawn text.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the drawn text.</param>
    /// <param name="format">
    ///  <see cref="StringFormat"/> that specifies formatting attributes, such as line spacing and alignment,
    ///  that are applied to the drawn text.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="brush"/> is <see langword="null"/>. -or- <paramref name="font"/> is <see langword="null"/>.
    /// </exception>
    public void DrawString(string? s, Font font, Brush brush, float x, float y, StringFormat? format) =>
        DrawString(s, font, brush, new RectangleF(x, y, 0, 0), format);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="DrawString(string?, Font, Brush, float, float, StringFormat?)"/>
    public void DrawString(ReadOnlySpan<char> s, Font font, Brush brush, float x, float y, StringFormat? format) =>
        DrawString(s, font, brush, new RectangleF(x, y, 0, 0), format);
#endif

    /// <summary>
    ///  Draws the specified text at the specified location with the specified <see cref="Brush"/> and
    ///  <see cref="Font"/> objects using the formatting attributes of the specified <see cref="StringFormat"/>.
    /// </summary>
    /// <param name="s">The text to draw.</param>
    /// <param name="font"><see cref="Font"/> that defines the text format.</param>
    /// <param name="brush"><see cref="Brush"/> that determines the color and texture of the drawn text.</param>
    /// <param name="point"><see cref="PointF"/>structure that specifies the upper-left corner of the drawn text.</param>
    /// <param name="format">
    ///  <see cref="StringFormat"/> that specifies formatting attributes, such as line spacing and alignment,
    ///  that are applied to the drawn text.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="brush"/> is <see langword="null"/>. -or- <paramref name="font"/> is <see langword="null"/>.
    /// </exception>
    public void DrawString(string? s, Font font, Brush brush, PointF point, StringFormat? format) =>
        DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), format);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="DrawString(string?, Font, Brush, PointF, StringFormat?)"/>
    public void DrawString(ReadOnlySpan<char> s, Font font, Brush brush, PointF point, StringFormat? format) =>
        DrawString(s, font, brush, new RectangleF(point.X, point.Y, 0, 0), format);
#endif

    /// <summary>
    ///  Draws the specified text in the specified rectangle with the specified <see cref="Brush"/> and
    ///  <see cref="Font"/> objects.
    /// </summary>
    /// <param name="s">The text to draw.</param>
    /// <param name="font"><see cref="Font"/> that defines the text format.</param>
    /// <param name="brush"><see cref="Brush"/> that determines the color and texture of the drawn text.</param>
    /// <param name="layoutRectangle"><see cref="RectangleF"/>structure that specifies the location of the drawn text.</param>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="brush"/> is <see langword="null"/>. -or- <paramref name="font"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    ///  <para>
    ///   The text represented by the <paramref name="s"/> parameter is drawn inside the rectangle represented by
    ///   the <paramref name="layoutRectangle"/> parameter. If the text does not fit inside the rectangle, it is
    ///   truncated at the nearest word. To further manipulate how the string is drawn inside the rectangle use the
    ///   <see cref="DrawString(string?, Font, Brush, RectangleF, StringFormat?)"/> overload that takes
    ///   a <see cref="StringFormat"/>.
    ///  </para>
    /// </remarks>
    public void DrawString(string? s, Font font, Brush brush, RectangleF layoutRectangle) =>
        DrawString(s, font, brush, layoutRectangle, null);

#if NET8_0_OR_GREATER
    /// <remarks>
    ///  <para>
    ///   The text represented by the <paramref name="s"/> parameter is drawn inside the rectangle represented by
    ///   the <paramref name="layoutRectangle"/> parameter. If the text does not fit inside the rectangle, it is
    ///   truncated at the nearest word. To further manipulate how the string is drawn inside the rectangle use the
    ///   <see cref="DrawString(ReadOnlySpan{char}, Font, Brush, RectangleF, StringFormat?)"/> overload that takes
    ///   a <see cref="StringFormat"/>.
    ///  </para>
    /// </remarks>
    /// <inheritdoc cref="DrawString(string?, Font, Brush, RectangleF)"/>
    public void DrawString(ReadOnlySpan<char> s, Font font, Brush brush, RectangleF layoutRectangle) =>
        DrawString(s, font, brush, layoutRectangle, null);
#endif

    /// <summary>
    ///  Draws the specified text in the specified rectangle with the specified <see cref="Brush"/> and
    ///  <see cref="Font"/> objects using the formatting attributes of the specified <see cref="StringFormat"/>.
    /// </summary>
    /// <param name="s">The text to draw.</param>
    /// <param name="font"><see cref="Font"/> that defines the text format.</param>
    /// <param name="brush"><see cref="Brush"/> that determines the color and texture of the drawn text.</param>
    /// <param name="layoutRectangle"><see cref="RectangleF"/>structure that specifies the location of the drawn text.</param>
    /// <param name="format">
    ///  <see cref="StringFormat"/> that specifies formatting attributes, such as line spacing and alignment,
    ///  that are applied to the drawn text.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="brush"/> is <see langword="null"/>. -or- <paramref name="font"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    ///  <para>
    ///   The text represented by the <paramref name="s"/> parameter is drawn inside the rectangle represented by
    ///   the <paramref name="layoutRectangle"/> parameter. If the text does not fit inside the rectangle, it is
    ///   truncated at the nearest word, unless otherwise specified with the <paramref name="format"/> parameter.
    ///  </para>
    /// </remarks>
    public void DrawString(string? s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat? format) =>
        DrawStringInternal(s, font, brush, layoutRectangle, format);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="DrawString(string?, Font, Brush, RectangleF, StringFormat?)"/>
    public void DrawString(ReadOnlySpan<char> s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat? format) =>
        DrawStringInternal(s, font, brush, layoutRectangle, format);
#endif

    private void DrawStringInternal(ReadOnlySpan<char> s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat? format)
    {
        ArgumentNullException.ThrowIfNull(brush);

        if (s.IsEmpty)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(font);

        fixed (char* c = s)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawString(
                NativeGraphics,
                c, s.Length,
                font.NativeFont,
                (RectF*)&layoutRectangle,
                format.Pointer(),
                brush.NativeBrush));
        }

        GC.KeepAlive(font);
        GC.KeepAlive(brush);
        GC.KeepAlive(format);
    }

    /// <param name="charactersFitted">Number of characters in the text.</param>
    /// <param name="linesFilled">Number of lines in the text.</param>
    /// <inheritdoc cref="MeasureString(string?, Font, SizeF, StringFormat?)"/>
    public SizeF MeasureString(
        string? text,
        Font font,
        SizeF layoutArea,
        StringFormat? stringFormat,
        out int charactersFitted,
        out int linesFilled) =>
        MeasureStringInternal(text, font, new(default, layoutArea), stringFormat, out charactersFitted, out linesFilled);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="MeasureString(string?, Font, SizeF, StringFormat?, out int, out int)"/>
    public SizeF MeasureString(
        ReadOnlySpan<char> text,
        Font font,
        SizeF layoutArea,
        StringFormat? stringFormat,
        out int charactersFitted,
        out int linesFilled) =>
        MeasureStringInternal(text, font, new(default, layoutArea), stringFormat, out charactersFitted, out linesFilled);
#endif

    public SizeF MeasureStringInternal(
        ReadOnlySpan<char> text,
        Font font,
        RectangleF layoutArea,
        StringFormat? stringFormat,
        out int charactersFitted,
        out int linesFilled)
    {
        if (text.IsEmpty)
        {
            charactersFitted = 0;
            linesFilled = 0;
            return SizeF.Empty;
        }

        ArgumentNullException.ThrowIfNull(font);

        RectF boundingBox = default;

        fixed (char* c = text)
        fixed (int* fitted = &charactersFitted)
        fixed (int* filled = &linesFilled)
        {
            CheckStatus(PInvokeGdiPlus.GdipMeasureString(
                NativeGraphics,
                c,
                text.Length,
                font.NativeFont,
                (RectF*)&layoutArea,
                stringFormat.Pointer(),
                &boundingBox,
                fitted,
                filled));
        }

        GC.KeepAlive(font);
        GC.KeepAlive(stringFormat);

        return new SizeF(boundingBox.Width, boundingBox.Height);
    }

    /// <param name="origin"><see cref="PointF"/> structure that represents the upper-left corner of the text.</param>
    /// <inheritdoc cref="MeasureString(string?, Font, SizeF, StringFormat?)"/>
    public SizeF MeasureString(string? text, Font font, PointF origin, StringFormat? stringFormat)
        => MeasureStringInternal(text, font, new(origin, default), stringFormat, out _, out _);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="MeasureString(string?, Font, PointF, StringFormat?)"/>
    public SizeF MeasureString(ReadOnlySpan<char> text, Font font, PointF origin, StringFormat? stringFormat)
        => MeasureStringInternal(text, font, new(origin, default), stringFormat, out _, out _);
#endif

    /// <inheritdoc cref="MeasureString(string?, Font, SizeF, StringFormat?)"/>
    public SizeF MeasureString(string? text, Font font, SizeF layoutArea) => MeasureString(text, font, layoutArea, null);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="MeasureString(string?, Font, SizeF)"/>
    public SizeF MeasureString(ReadOnlySpan<char> text, Font font, SizeF layoutArea) => MeasureString(text, font, layoutArea, null);
#endif

    /// <param name="stringFormat">
    ///  <see cref="StringFormat"/> that represents formatting information, such as line spacing, for the text.
    /// </param>
    /// <param name="layoutArea">
    ///  <see cref="SizeF"/> structure that specifies the maximum layout area for the text.
    /// </param>
    /// <inheritdoc cref="MeasureString(string?, Font, int, StringFormat?)"/>
    public SizeF MeasureString(string? text, Font font, SizeF layoutArea, StringFormat? stringFormat) =>
        MeasureStringInternal(text, font, new(default, layoutArea), stringFormat, out _, out _);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="MeasureString(string?, Font, SizeF, StringFormat?)"/>
    public SizeF MeasureString(ReadOnlySpan<char> text, Font font, SizeF layoutArea, StringFormat? stringFormat) =>
        MeasureStringInternal(text, font, new(default, layoutArea), stringFormat, out _, out _);
#endif

    /// <summary>
    ///  Measures the specified text when drawn with the specified <see cref="Font"/>.
    /// </summary>
    /// <param name="text">Text to measure.</param>
    /// <param name="font"><see cref="Font"/> that defines the text format.</param>
    /// <returns>
    ///  This method returns a <see cref="SizeF"/> structure that represents the size, in the units specified by the
    ///  <see cref="PageUnit"/> property, of the text specified by the <paramref name="text"/> parameter as drawn
    ///  with the <paramref name="font"/> parameter.
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   The <see cref="MeasureString(string?, Font)"/> method is designed for use with individual strings and
    ///   includes a small amount of extra space before and after the string to allow for overhanging glyphs. Also,
    ///   the <see cref="DrawString(string?, Font, Brush, PointF)"/> method adjusts glyph points to optimize display
    ///   quality and might display a string narrower than reported by <see cref="MeasureString(string?, Font)"/>.
    ///   To obtain metrics suitable for adjacent strings in layout (for example, when implementing formatted text),
    ///   use the <see cref="MeasureCharacterRanges(string?, Font, RectangleF, StringFormat?)"/> method or one of
    ///   the <see cref="MeasureString(string?, Font, int, StringFormat?)"/> methods that takes a StringFormat, and
    ///   pass <see cref="StringFormat.GenericTypographic"/>. Also, ensure the <see cref="TextRenderingHint"/> for
    ///   the <see cref="Graphics"/> is <see cref="TextRenderingHint.AntiAlias"/>.
    ///  </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="font"/> is null.</exception>
    public SizeF MeasureString(string? text, Font font) => MeasureString(text, font, new SizeF(0, 0));

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="MeasureString(string?, Font)"/>
    public SizeF MeasureString(ReadOnlySpan<char> text, Font font) => MeasureString(text, font, new SizeF(0, 0));
#endif

    /// <param name="width">Maximum width of the string in pixels.</param>
    /// <inheritdoc cref="MeasureString(string?, Font)"/>
    public SizeF MeasureString(string? text, Font font, int width) => MeasureString(text, font, new SizeF(width, 999999));

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="MeasureString(string?, Font, int)"/>
    public SizeF MeasureString(ReadOnlySpan<char> text, Font font, int width) =>
        MeasureString(text, font, new SizeF(width, 999999));
#endif

    /// <param name="format">
    ///  <see cref="StringFormat"/> that represents formatting information, such as line spacing, for the text.
    /// </param>
    /// <inheritdoc cref="MeasureString(string?, Font, int)"/>
    public SizeF MeasureString(string? text, Font font, int width, StringFormat? format) =>
        MeasureString(text, font, new SizeF(width, 999999), format);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="MeasureString(string?, Font, int, StringFormat?)"/>
    public SizeF MeasureString(ReadOnlySpan<char> text, Font font, int width, StringFormat? format) =>
        MeasureString(text, font, new SizeF(width, 999999), format);
#endif

    /// <summary>
    ///  Gets an array of <see cref="Region"/> objects, each of which bounds a range of character positions within
    ///  the specified text.
    /// </summary>
    /// <param name="text">Text to measure.</param>
    /// <param name="font"><see cref="Font"/> that defines the text format.</param>
    /// <param name="layoutRect"><see cref="RectangleF"/> structure that specifies the layout rectangle for the text.</param>
    /// <param name="stringFormat">
    ///  <see cref="StringFormat"/> that represents formatting information, such as line spacing, for the text.
    /// </param>
    /// <returns>
    ///  This method returns an array of <see cref="Region"/> objects, each of which bounds a range of character
    ///  positions within the specified text.
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   The regions returned by this method are resolution-dependent, so there might be a slight loss of accuracy
    ///   if text is recorded in a metafile at one resolution and later played back at a different resolution.
    ///  </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="font"/> is <see langword="null"/>.</exception>
    public Region[] MeasureCharacterRanges(string? text, Font font, RectangleF layoutRect, StringFormat? stringFormat) =>
        MeasureCharacterRangesInternal(text, font, layoutRect, stringFormat);

#if NET8_0_OR_GREATER
    /// <inheritdoc cref="MeasureCharacterRanges(string?, Font, RectangleF, StringFormat?)"/>
    public Region[] MeasureCharacterRanges(ReadOnlySpan<char> text, Font font, RectangleF layoutRect, StringFormat? stringFormat) =>
        MeasureCharacterRangesInternal(text, font, layoutRect, stringFormat);
#endif

    private Region[] MeasureCharacterRangesInternal(
        ReadOnlySpan<char> text,
        Font font,
        RectF layoutRect,
        StringFormat? stringFormat)
    {
        if (text.IsEmpty)
            return [];

        ArgumentNullException.ThrowIfNull(font);

        int count;
        CheckStatus(PInvokeGdiPlus.GdipGetStringFormatMeasurableCharacterRangeCount(stringFormat.Pointer(), &count));

        if (count == 0)
        {
            return [];
        }

        GpRegion*[] gpRegions = new GpRegion*[count];
        Region[] regions = new Region[count];

        for (int f = 0; f < count; f++)
        {
            regions[f] = new Region();
            gpRegions[f] = regions[f].NativeRegion;
        }

        fixed (char* c = text)
        fixed (GpRegion** r = gpRegions)
        {
            CheckStatus(PInvokeGdiPlus.GdipMeasureCharacterRanges(
                NativeGraphics,
                c,
                text.Length,
                font.NativeFont,
                &layoutRect,
                stringFormat.Pointer(),
                count,
                r));
        }

        GC.KeepAlive(stringFormat);
        GC.KeepAlive(font);

        return regions;
    }

    /// <summary>
    ///  Draws the specified image at the specified location.
    /// </summary>
    public void DrawImage(Image image, PointF point) => DrawImage(image, point.X, point.Y);

    public void DrawImage(Image image, float x, float y)
    {
        ArgumentNullException.ThrowIfNull(image);
        Status status = PInvokeGdiPlus.GdipDrawImage(NativeGraphics, image.Pointer(), x, y);
        IgnoreMetafileErrors(image, ref status);
        CheckErrorStatus(status);
    }

    public void DrawImage(Image image, RectangleF rect) => DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);

    public void DrawImage(Image image, float x, float y, float width, float height)
    {
        ArgumentNullException.ThrowIfNull(image);
        Status status = PInvokeGdiPlus.GdipDrawImageRect(NativeGraphics, image.Pointer(), x, y, width, height);
        IgnoreMetafileErrors(image, ref status);
        CheckErrorStatus(status);
    }

    public void DrawImage(Image image, Point point) => DrawImage(image, (float)point.X, point.Y);

    public void DrawImage(Image image, int x, int y) => DrawImage(image, (float)x, y);

    public void DrawImage(Image image, Rectangle rect) => DrawImage(image, (float)rect.X, rect.Y, rect.Width, rect.Height);

    public void DrawImage(Image image, int x, int y, int width, int height) => DrawImage(image, (float)x, y, width, height);

    public void DrawImageUnscaled(Image image, Point point) => DrawImage(image, point.X, point.Y);

    public void DrawImageUnscaled(Image image, int x, int y) => DrawImage(image, x, y);

    public void DrawImageUnscaled(Image image, Rectangle rect) => DrawImage(image, rect.X, rect.Y);

    public void DrawImageUnscaled(Image image, int x, int y, int width, int height) => DrawImage(image, x, y);

    public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
    {
        ArgumentNullException.ThrowIfNull(image);

        int width = Math.Min(rect.Width, image.Width);
        int height = Math.Min(rect.Height, image.Height);

        // We could put centering logic here too for the case when the image
        // is smaller than the rect.
        DrawImage(image, rect, 0, 0, width, height, GraphicsUnit.Pixel);
    }

    public void DrawImage(Image image, PointF[] destPoints)
    {
        // Affine or perspective blt
        //
        //  destPoints.Length = 3: rect => parallelogram
        //      destPoints[0] <=> top-left corner of the source rectangle
        //      destPoints[1] <=> top-right corner
        //      destPoints[2] <=> bottom-left corner
        //  destPoints.Length = 4: rect => quad
        //      destPoints[3] <=> bottom-right corner
        //
        //  @notes Perspective blt only works for bitmap images.

        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(destPoints);

        int count = destPoints.Length;
        if (count is not 3 and not 4)
            throw new ArgumentException(SR.GdiplusDestPointsInvalidLength);

        fixed (PointF* p = destPoints)
        {
            Status status = PInvokeGdiPlus.GdipDrawImagePoints(NativeGraphics, image.Pointer(), (GdiPlus.PointF*)p, count);
            IgnoreMetafileErrors(image, ref status);
            CheckErrorStatus(status);
        }
    }

    public void DrawImage(Image image, Point[] destPoints)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(destPoints);

        int count = destPoints.Length;
        if (count is not 3 and not 4)
            throw new ArgumentException(SR.GdiplusDestPointsInvalidLength);

        fixed (Point* p = destPoints)
        {
            Status status = PInvokeGdiPlus.GdipDrawImagePointsI(NativeGraphics, image.Pointer(), (GdiPlus.Point*)p, count);
            IgnoreMetafileErrors(image, ref status);
            CheckErrorStatus(status);
        }
    }

    public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit)
    {
        ArgumentNullException.ThrowIfNull(image);

        Status status = PInvokeGdiPlus.GdipDrawImagePointRect(
            NativeGraphics,
            image.Pointer(),
            x, y,
            srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
            (Unit)srcUnit);

        IgnoreMetafileErrors(image, ref status);
        CheckErrorStatus(status);
    }

    public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
        => DrawImage(image, x, y, (RectangleF)srcRect, srcUnit);

    public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit)
    {
        ArgumentNullException.ThrowIfNull(image);

        Status status = PInvokeGdiPlus.GdipDrawImageRectRect(
            NativeGraphics,
            image.Pointer(),
            destRect.X, destRect.Y, destRect.Width, destRect.Height,
            srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
            (Unit)srcUnit,
            imageAttributes: null,
            callback: 0,
            callbackData: null);

        IgnoreMetafileErrors(image, ref status);
        CheckErrorStatus(status);
    }

    public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit)
        => DrawImage(image, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, srcUnit);

    public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(destPoints);

        int count = destPoints.Length;
        if (count is not 3 and not 4)
            throw new ArgumentException(SR.GdiplusDestPointsInvalidLength);

        fixed (PointF* p = destPoints)
        {
            Status status = PInvokeGdiPlus.GdipDrawImagePointsRect(
                NativeGraphics,
                image.Pointer(),
                (GdiPlus.PointF*)p, destPoints.Length,
                srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
                (Unit)srcUnit,
                imageAttributes: null,
                callback: 0,
                callbackData: null);

            IgnoreMetafileErrors(image, ref status);
            CheckErrorStatus(status);
        }
    }

    public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes? imageAttr) =>
        DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, null, 0);

    public void DrawImage(
        Image image,
        PointF[] destPoints,
        RectangleF srcRect,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttr,
        DrawImageAbort? callback) =>
        DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, callback, 0);

    public void DrawImage(
        Image image,
        PointF[] destPoints,
        RectangleF srcRect,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttr,
        DrawImageAbort? callback,
        int callbackData)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(destPoints);

        int count = destPoints.Length;
        if (count is not 3 and not 4)
            throw new ArgumentException(SR.GdiplusDestPointsInvalidLength);

        fixed (PointF* p = destPoints)
        {
            Status status = PInvokeGdiPlus.GdipDrawImagePointsRect(
                NativeGraphics,
                image.Pointer(),
                (GdiPlus.PointF*)p, destPoints.Length,
                srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
                (Unit)srcUnit,
                imageAttr.Pointer(),
                callback is null ? 0 : Marshal.GetFunctionPointerForDelegate(callback),
                (void*)(nint)callbackData);

            GC.KeepAlive(imageAttr);
            GC.KeepAlive(callback);
            IgnoreMetafileErrors(image, ref status);
            CheckErrorStatus(status);
        }
    }

    public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit) =>
        DrawImage(image, destPoints, srcRect, srcUnit, null, null, 0);

    public void DrawImage(
        Image image,
        Point[] destPoints,
        Rectangle srcRect,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttr) => DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, null, 0);

    public void DrawImage(
        Image image,
        Point[] destPoints,
        Rectangle srcRect,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttr,
        DrawImageAbort? callback) => DrawImage(image, destPoints, srcRect, srcUnit, imageAttr, callback, 0);

    public void DrawImage(
        Image image,
        Point[] destPoints,
        Rectangle srcRect,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttr,
        DrawImageAbort? callback,
        int callbackData)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(destPoints);

        int count = destPoints.Length;
        if (count is not 3 and not 4)
            throw new ArgumentException(SR.GdiplusDestPointsInvalidLength);

        fixed (Point* p = destPoints)
        {
            Status status = PInvokeGdiPlus.GdipDrawImagePointsRectI(
                NativeGraphics,
                image.Pointer(),
                (GdiPlus.Point*)p, destPoints.Length,
                srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height,
                (Unit)srcUnit,
                imageAttr.Pointer(),
                callback is null ? 0 : Marshal.GetFunctionPointerForDelegate(callback),
                (void*)(nint)callbackData);

            GC.KeepAlive(imageAttr);
            GC.KeepAlive(callback);

            IgnoreMetafileErrors(image, ref status);
            CheckErrorStatus(status);
        }
    }

    public void DrawImage(
        Image image,
        Rectangle destRect,
        float srcX,
        float srcY,
        float srcWidth,
        float srcHeight,
        GraphicsUnit srcUnit) => DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, null);

    public void DrawImage(
        Image image,
        Rectangle destRect,
        float srcX,
        float srcY,
        float srcWidth,
        float srcHeight,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttrs) => DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, null);

    public void DrawImage(
        Image image,
        Rectangle destRect,
        float srcX,
        float srcY,
        float srcWidth,
        float srcHeight,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttrs,
        DrawImageAbort? callback) => DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, IntPtr.Zero);

    public void DrawImage(
        Image image,
        Rectangle destRect,
        float srcX,
        float srcY,
        float srcWidth,
        float srcHeight,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttrs,
        DrawImageAbort? callback,
        IntPtr callbackData)
    {
        ArgumentNullException.ThrowIfNull(image);

        Status status = PInvokeGdiPlus.GdipDrawImageRectRect(
            NativeGraphics,
            image.Pointer(),
            destRect.X, destRect.Y, destRect.Width, destRect.Height,
            srcX, srcY, srcWidth, srcHeight,
            (Unit)srcUnit,
            imageAttrs.Pointer(),
            callback is null ? 0 : Marshal.GetFunctionPointerForDelegate(callback),
            (void*)callbackData);

        GC.KeepAlive(imageAttrs);
        GC.KeepAlive(callback);
        IgnoreMetafileErrors(image, ref status);
        CheckErrorStatus(status);
    }

    public void DrawImage(
        Image image,
        Rectangle destRect,
        int srcX,
        int srcY,
        int srcWidth,
        int srcHeight,
        GraphicsUnit srcUnit) => DrawImage(image, destRect, (float)srcX, srcY, srcWidth, srcHeight, srcUnit, null);

    public void DrawImage(
        Image image,
        Rectangle destRect,
        int srcX,
        int srcY,
        int srcWidth,
        int srcHeight,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttr) => DrawImage(image, destRect, (float)srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr, null);

    public void DrawImage(
        Image image,
        Rectangle destRect,
        int srcX,
        int srcY,
        int srcWidth,
        int srcHeight,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttr,
        DrawImageAbort? callback) => DrawImage(image, destRect, (float)srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr, callback, IntPtr.Zero);

    public void DrawImage(
        Image image,
        Rectangle destRect,
        int srcX,
        int srcY,
        int srcWidth,
        int srcHeight,
        GraphicsUnit srcUnit,
        ImageAttributes? imageAttrs,
        DrawImageAbort? callback,
        IntPtr callbackData) => DrawImage(image, destRect, (float)srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs, callback, callbackData);

    /// <summary>
    ///  Draws a line connecting the two specified points.
    /// </summary>
    public void DrawLine(Pen pen, PointF pt1, PointF pt2) => DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);

    /// <inheritdoc cref="DrawLines(Pen, Point[])"/>
    public void DrawLines(Pen pen, params PointF[] points) => DrawLines(pen, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawLines(Pen, Point[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawLines(Pen pen, params ReadOnlySpan<PointF> points)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawLines(NativeGraphics, pen.NativePen, (GdiPlus.PointF*)p, points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws a line connecting the two specified points.
    /// </summary>
    public void DrawLine(Pen pen, int x1, int y1, int x2, int y2) =>
        DrawLine(pen, (float)x1, y1, x2, y2);

    /// <summary>
    ///  Draws a line connecting the two specified points.
    /// </summary>
    public void DrawLine(Pen pen, Point pt1, Point pt2) => DrawLine(pen, (float)pt1.X, pt1.Y, pt2.X, pt2.Y);

    /// <summary>
    ///  Draws a series of line segments that connect an array of points.
    /// </summary>
    /// <param name="pen">The <see cref="Pen"/> that determines the color, width, and style of the line segments.</param>
    /// <param name="points">An array of points to connect.</param>
    public void DrawLines(Pen pen, params Point[] points)
    {
        ArgumentNullException.ThrowIfNull(pen);
        ArgumentNullException.ThrowIfNull(points);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawLinesI(NativeGraphics, pen.NativePen, (GdiPlus.Point*)p, points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <inheritdoc cref="DrawLines(Pen, Point[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawLines(Pen pen, params ReadOnlySpan<Point> points)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawLinesI(NativeGraphics, pen.NativePen, (GdiPlus.Point*)p, points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  CopyPixels will perform a gdi "bitblt" operation to the source from the destination with the given size.
    /// </summary>
    public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize) =>
        CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize);

    /// <summary>
    ///  CopyPixels will perform a gdi "bitblt" operation to the source from the destination with the given size.
    /// </summary>
    public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize) =>
        CopyFromScreen(sourceX, sourceY, destinationX, destinationY, blockRegionSize, CopyPixelOperation.SourceCopy);

    /// <summary>
    ///  CopyPixels will perform a gdi "bitblt" operation to the source from the destination with the given size
    ///  and specified raster operation.
    /// </summary>
    public void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation) =>
        CopyFromScreen(upperLeftSource.X, upperLeftSource.Y, upperLeftDestination.X, upperLeftDestination.Y, blockRegionSize, copyPixelOperation);

    public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback) =>
        EnumerateMetafile(metafile, destPoint, callback, IntPtr.Zero);

    public void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData) =>
        EnumerateMetafile(metafile, destPoint, callback, callbackData, null);

    public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback) =>
        EnumerateMetafile(metafile, destPoint, callback, IntPtr.Zero);

    public void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData) =>
        EnumerateMetafile(metafile, destPoint, callback, callbackData, null);

    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback) =>
        EnumerateMetafile(metafile, destRect, callback, IntPtr.Zero);

    public void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData) =>
        EnumerateMetafile(metafile, destRect, callback, callbackData, null);

    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback) =>
        EnumerateMetafile(metafile, destRect, callback, IntPtr.Zero);

    public void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData) =>
        EnumerateMetafile(metafile, destRect, callback, callbackData, null);

    public void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback) =>
        EnumerateMetafile(metafile, destPoints, callback, IntPtr.Zero);

    public void EnumerateMetafile(
        Metafile metafile,
        PointF[] destPoints,
        EnumerateMetafileProc callback,
        IntPtr callbackData) => EnumerateMetafile(metafile, destPoints, callback, IntPtr.Zero, null);

    public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback) =>
        EnumerateMetafile(metafile, destPoints, callback, IntPtr.Zero);

    public void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData) =>
        EnumerateMetafile(metafile, destPoints, callback, callbackData, null);

    public void EnumerateMetafile(
        Metafile metafile,
        PointF destPoint,
        RectangleF srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback) => EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, IntPtr.Zero);

    public void EnumerateMetafile(
        Metafile metafile,
        PointF destPoint,
        RectangleF srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback,
        IntPtr callbackData) => EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, callbackData, null);

    public void EnumerateMetafile(
        Metafile metafile,
        Point destPoint,
        Rectangle srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback) => EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, IntPtr.Zero);

    public void EnumerateMetafile(
        Metafile metafile,
        Point destPoint,
        Rectangle srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback,
        IntPtr callbackData) => EnumerateMetafile(metafile, destPoint, srcRect, srcUnit, callback, callbackData, null);

    public void EnumerateMetafile(
        Metafile metafile,
        RectangleF destRect,
        RectangleF srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback) => EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, IntPtr.Zero);

    public void EnumerateMetafile(
        Metafile metafile,
        RectangleF destRect,
        RectangleF srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback,
        IntPtr callbackData) => EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, callbackData, null);

    public void EnumerateMetafile(
        Metafile metafile,
        Rectangle destRect,
        Rectangle srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback) => EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, IntPtr.Zero);

    public void EnumerateMetafile(
        Metafile metafile,
        Rectangle destRect,
        Rectangle srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback,
        IntPtr callbackData) => EnumerateMetafile(metafile, destRect, srcRect, srcUnit, callback, callbackData, null);

    public void EnumerateMetafile(
        Metafile metafile,
        PointF[] destPoints,
        RectangleF srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback) => EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, IntPtr.Zero);

    public void EnumerateMetafile(
        Metafile metafile,
        PointF[] destPoints,
        RectangleF srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback,
        IntPtr callbackData) => EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, callbackData, null);

    public void EnumerateMetafile(
        Metafile metafile,
        Point[] destPoints,
        Rectangle srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback) => EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, IntPtr.Zero);

    public void EnumerateMetafile(
        Metafile metafile,
        Point[] destPoints,
        Rectangle srcRect,
        GraphicsUnit srcUnit,
        EnumerateMetafileProc callback,
        IntPtr callbackData) => EnumerateMetafile(metafile, destPoints, srcRect, srcUnit, callback, callbackData, null);

    /// <summary>
    ///  Transforms an array of points from one coordinate space to another using the current world and page
    ///  transformations of this <see cref="Graphics"/>.
    /// </summary>
    /// <param name="destSpace">The destination coordinate space.</param>
    /// <param name="srcSpace">The source coordinate space.</param>
    /// <param name="pts">The points to transform.</param>
    public void TransformPoints(Drawing2DCoordinateSpace destSpace, Drawing2DCoordinateSpace srcSpace, params PointF[] pts)
    {
        ArgumentNullException.ThrowIfNull(pts);
        TransformPoints(destSpace, srcSpace, pts.AsSpan());
    }

    /// <inheritdoc cref="TransformPoints(Drawing2DCoordinateSpace, Drawing2DCoordinateSpace, PointF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void TransformPoints(Drawing2DCoordinateSpace destSpace, Drawing2DCoordinateSpace srcSpace, params ReadOnlySpan<PointF> pts)
    {
        fixed (PointF* p = pts)
        {
            CheckStatus(PInvokeGdiPlus.GdipTransformPoints(
                NativeGraphics,
                (GdiPlus.CoordinateSpace)destSpace,
                (GdiPlus.CoordinateSpace)srcSpace,
                (GdiPlus.PointF*)p,
                pts.Length));
        }
    }

    /// <inheritdoc cref="TransformPoints(Drawing2DCoordinateSpace, Drawing2DCoordinateSpace, PointF[])"/>
    public void TransformPoints(Drawing2DCoordinateSpace destSpace, Drawing2DCoordinateSpace srcSpace, params Point[] pts)
    {
        ArgumentNullException.ThrowIfNull(pts);
        TransformPoints(destSpace, srcSpace, pts.AsSpan());
    }

    /// <inheritdoc cref="TransformPoints(Drawing2DCoordinateSpace, Drawing2DCoordinateSpace, PointF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void TransformPoints(Drawing2DCoordinateSpace destSpace, Drawing2DCoordinateSpace srcSpace, params ReadOnlySpan<Point> pts)
    {
        fixed (Point* p = pts)
        {
            CheckStatus(PInvokeGdiPlus.GdipTransformPointsI(
                NativeGraphics,
                (GdiPlus.CoordinateSpace)destSpace,
                (GdiPlus.CoordinateSpace)srcSpace,
                (GdiPlus.Point*)p,
                pts.Length));
        }
    }

    /// <summary>
    ///  GDI+ will return a 'generic error' when we attempt to draw an Emf
    ///  image with width/height == 1. Here, we will hack around this by
    ///  resetting the Status. Note that we don't do simple arg checking
    ///  for height || width == 1 here because transforms can be applied to
    ///  the Graphics object making it difficult to identify this scenario.
    /// </summary>
    private static void IgnoreMetafileErrors(Image image, ref Status errorStatus)
    {
        if (errorStatus != Status.Ok && image.RawFormat.Equals(ImageFormat.Emf))
            errorStatus = Status.Ok;

        GC.KeepAlive(image);
    }

    /// <summary>
    ///  Creates a Region class only if the native region is not infinite.
    /// </summary>
    internal Region? GetRegionIfNotInfinite()
    {
        GpRegion* regionHandle;
        CheckStatus(PInvokeGdiPlus.GdipCreateRegion(&regionHandle));

        try
        {
            BOOL isInfinite;
            PInvokeGdiPlus.GdipGetClip(NativeGraphics, regionHandle);

            CheckStatus(PInvokeGdiPlus.GdipIsInfiniteRegion(
                regionHandle,
                NativeGraphics,
                &isInfinite));

            if (isInfinite)
            {
                // Infinite
                return null;
            }

            Region region = new(regionHandle);
            regionHandle = null;
            return region;
        }
        finally
        {
            if (regionHandle is not null)
            {
                PInvokeGdiPlus.GdipDeleteRegion(regionHandle);
            }
        }
    }

    /// <summary>
    ///  Represents an object used in connection with the printing API, it is used to hold a reference to a
    ///  PrintPreviewGraphics (fake graphics) or a printer DeviceContext (and maybe more in the future).
    /// </summary>
    internal object? PrintingHelper
    {
        get => _printingHelper;
        set
        {
            Debug.Assert(_printingHelper is null, "WARNING: Overwriting the printing helper reference!");
            _printingHelper = value;
        }
    }

    /// <summary>
    ///  CopyPixels will perform a gdi "bitblt" operation to the source from the destination with the given size
    ///  and specified raster operation.
    /// </summary>
    public void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation)
    {
        switch (copyPixelOperation)
        {
            case CopyPixelOperation.Blackness:
            case CopyPixelOperation.NotSourceErase:
            case CopyPixelOperation.NotSourceCopy:
            case CopyPixelOperation.SourceErase:
            case CopyPixelOperation.DestinationInvert:
            case CopyPixelOperation.PatInvert:
            case CopyPixelOperation.SourceInvert:
            case CopyPixelOperation.SourceAnd:
            case CopyPixelOperation.MergePaint:
            case CopyPixelOperation.MergeCopy:
            case CopyPixelOperation.SourceCopy:
            case CopyPixelOperation.SourcePaint:
            case CopyPixelOperation.PatCopy:
            case CopyPixelOperation.PatPaint:
            case CopyPixelOperation.Whiteness:
            case CopyPixelOperation.CaptureBlt:
            case CopyPixelOperation.NoMirrorBitmap:
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(copyPixelOperation), (int)copyPixelOperation, typeof(CopyPixelOperation));
        }

        int destWidth = blockRegionSize.Width;
        int destHeight = blockRegionSize.Height;

        using var screenDC = GetDcScope.ScreenDC;
        if (screenDC == 0)
        {
            // ERROR_INVALID_HANDLE - if you pass an empty handle to BitBlt you'll get this error.
            // Checking here to better describe test failures (and avoids taking the Graphics HDC lock).
            throw new Win32Exception(6);
        }

        HDC targetDC = (HDC)GetHdc();
        try
        {
            BOOL result = PInvokeCore.BitBlt(
                targetDC,
                destinationX,
                destinationY,
                destWidth,
                destHeight,
                screenDC,
                sourceX,
                sourceY,
                (ROP_CODE)copyPixelOperation);

            if (!result)
            {
                throw new Win32Exception();
            }
        }
        finally
        {
            if (!targetDC.IsNull)
            {
                ReleaseHdc();
            }
        }
    }

    public Color GetNearestColor(Color color)
    {
        uint nearest = (uint)color.ToArgb();
        CheckStatus(PInvokeGdiPlus.GdipGetNearestColor(NativeGraphics, &nearest));
        return Color.FromArgb((int)nearest);
    }

    /// <summary>
    ///  Draws a line connecting the two specified points.
    /// </summary>
    public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
    {
        ArgumentNullException.ThrowIfNull(pen);
        CheckErrorStatus(PInvokeGdiPlus.GdipDrawLine(NativeGraphics, pen.NativePen, x1, y1, x2, y2));
        GC.KeepAlive(pen);
    }

    /// <inheritdoc cref="DrawBeziers(Pen, Point[])"/>
    public void DrawBeziers(Pen pen, params PointF[] points) =>
        DrawBeziers(pen, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawBeziers(Pen, Point[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawBeziers(Pen pen, params ReadOnlySpan<PointF> points)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (PointF* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawBeziers(
                NativeGraphics,
                pen.NativePen,
                (GdiPlus.PointF*)p, points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Draws a series of cubic Bézier curves from an array of points.
    /// </summary>
    /// <param name="pen">The <paramref name="pen"/> to draw the Bézier with.</param>
    /// <param name="points">
    ///  Points that represent the points that determine the curve. The number of points in the array
    ///  should be a multiple of 3 plus 1, such as 4, 7, or 10.
    /// </param>
    public void DrawBeziers(Pen pen, params Point[] points) => DrawBeziers(pen, points.OrThrowIfNull().AsSpan());

    /// <inheritdoc cref="DrawBeziers(Pen, Point[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void DrawBeziers(Pen pen, params ReadOnlySpan<Point> points)
    {
        ArgumentNullException.ThrowIfNull(pen);

        fixed (Point* p = points)
        {
            CheckErrorStatus(PInvokeGdiPlus.GdipDrawBeziersI(
                NativeGraphics,
                pen.NativePen,
                (GdiPlus.Point*)p,
                points.Length));
        }

        GC.KeepAlive(pen);
    }

    /// <summary>
    ///  Fills the interior of a path.
    /// </summary>
    public void FillPath(Brush brush, GraphicsPath path)
    {
        ArgumentNullException.ThrowIfNull(brush);
        ArgumentNullException.ThrowIfNull(path);

        CheckErrorStatus(PInvokeGdiPlus.GdipFillPath(
            NativeGraphics,
            brush.NativeBrush,
            path._nativePath));

        GC.KeepAlive(brush);
        GC.KeepAlive(path);
    }

    /// <summary>
    ///  Fills the interior of a <see cref='Region'/>.
    /// </summary>
    public void FillRegion(Brush brush, Region region)
    {
        ArgumentNullException.ThrowIfNull(brush);
        ArgumentNullException.ThrowIfNull(region);

        CheckErrorStatus(PInvokeGdiPlus.GdipFillRegion(
            NativeGraphics,
            brush.NativeBrush,
            region.NativeRegion));

        GC.KeepAlive(brush);
        GC.KeepAlive(region);
    }

    public void DrawIcon(Icon icon, int x, int y)
    {
        ArgumentNullException.ThrowIfNull(icon);

        if (_backingImage is not null)
        {
            // We don't call the icon directly because we want to stay in GDI+ all the time
            // to avoid alpha channel interop issues between gdi and gdi+
            // so we do icon.ToBitmap() and then we call DrawImage. This is probably slower.
            DrawImage(icon.ToBitmap(), x, y);
        }
        else
        {
            icon.Draw(this, x, y);
        }
    }

    /// <summary>
    ///  Draws this image to a graphics object. The drawing command originates on the graphics
    ///  object, but a graphics object generally has no idea how to render a given image. So,
    ///  it passes the call to the actual image. This version crops the image to the given
    ///  dimensions and allows the user to specify a rectangle within the image to draw.
    /// </summary>
    public void DrawIcon(Icon icon, Rectangle targetRect)
    {
        ArgumentNullException.ThrowIfNull(icon);

        if (_backingImage is not null)
        {
            // We don't call the icon directly because we want to stay in GDI+ all the time
            // to avoid alpha channel interop issues between gdi and gdi+
            // so we do icon.ToBitmap() and then we call DrawImage. This is probably slower.
            DrawImage(icon.ToBitmap(), targetRect);
        }
        else
        {
            icon.Draw(this, targetRect);
        }
    }

    /// <summary>
    ///  Draws this image to a graphics object. The drawing command originates on the graphics
    ///  object, but a graphics object generally has no idea how to render a given image. So,
    ///  it passes the call to the actual image. This version stretches the image to the given
    ///  dimensions and allows the user to specify a rectangle within the image to draw.
    /// </summary>
    public void DrawIconUnstretched(Icon icon, Rectangle targetRect)
    {
        ArgumentNullException.ThrowIfNull(icon);

        if (_backingImage is not null)
        {
            DrawImageUnscaled(icon.ToBitmap(), targetRect);
        }
        else
        {
            icon.DrawUnstretched(this, targetRect);
        }
    }

    public void EnumerateMetafile(
        Metafile metafile,
        PointF destPoint,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr)
    {
        using EnumerateMetafileDataAdapter adapter = new(callback);
        PInvokeGdiPlus.GdipEnumerateMetafileDestPoint(
            NativeGraphics,
            metafile.Pointer(),
            (GdiPlus.PointF*)&destPoint,
            adapter.NativeCallback,
            (void*)callbackData,
            imageAttr.Pointer()).ThrowIfFailed();

        GC.KeepAlive(imageAttr);
        GC.KeepAlive(metafile);
        GC.KeepAlive(this);
    }

    public void EnumerateMetafile(
        Metafile metafile,
        Point destPoint,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr)
        => EnumerateMetafile(metafile, (PointF)destPoint, callback, callbackData, imageAttr);

    public void EnumerateMetafile(
        Metafile metafile,
        RectangleF destRect,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr)
    {
        using EnumerateMetafileDataAdapter adapter = new(callback);
        PInvokeGdiPlus.GdipEnumerateMetafileDestRect(
            NativeGraphics,
            metafile.Pointer(),
            (RectF*)&destRect,
            adapter.NativeCallback,
            (void*)callbackData,
            imageAttr.Pointer()).ThrowIfFailed();

        GC.KeepAlive(imageAttr);
        GC.KeepAlive(metafile);
        GC.KeepAlive(this);
    }

    public void EnumerateMetafile(
        Metafile metafile,
        Rectangle destRect,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr) => EnumerateMetafile(metafile, (RectangleF)destRect, callback, callbackData, imageAttr);

    public void EnumerateMetafile(
        Metafile metafile,
        PointF[] destPoints,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr)
    {
        ArgumentNullException.ThrowIfNull(destPoints);

        if (destPoints.Length != 3)
            throw new ArgumentException(SR.GdiplusDestPointsInvalidParallelogram);

        fixed (PointF* p = destPoints)
        {
            using EnumerateMetafileDataAdapter adapter = new(callback);
            PInvokeGdiPlus.GdipEnumerateMetafileDestPoints(
                NativeGraphics,
                metafile.Pointer(),
                (GdiPlus.PointF*)p, destPoints.Length,
                adapter.NativeCallback,
                (void*)callbackData,
                imageAttr.Pointer()).ThrowIfFailed();

            GC.KeepAlive(imageAttr);
            GC.KeepAlive(metafile);
            GC.KeepAlive(this);
        }
    }

    public void EnumerateMetafile(
        Metafile metafile,
        Point[] destPoints,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr)
    {
        ArgumentNullException.ThrowIfNull(destPoints);

        if (destPoints.Length != 3)
            throw new ArgumentException(SR.GdiplusDestPointsInvalidParallelogram);

        fixed (Point* p = destPoints)
        {
            using EnumerateMetafileDataAdapter adapter = new(callback);
            PInvokeGdiPlus.GdipEnumerateMetafileDestPointsI(
                NativeGraphics,
                metafile.Pointer(),
                (GdiPlus.Point*)p, destPoints.Length,
                adapter.NativeCallback,
                (void*)callbackData,
                imageAttr.Pointer()).ThrowIfFailed();

            GC.KeepAlive(imageAttr);
            GC.KeepAlive(metafile);
            GC.KeepAlive(this);
        }
    }

    public void EnumerateMetafile(
        Metafile metafile,
        PointF destPoint,
        RectangleF srcRect,
        GraphicsUnit unit,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr)
    {
        using EnumerateMetafileDataAdapter adapter = new(callback);
        PInvokeGdiPlus.GdipEnumerateMetafileSrcRectDestPoint(
            NativeGraphics,
            metafile.Pointer(),
            (GdiPlus.PointF*)&destPoint,
            (RectF*)&srcRect,
            (Unit)unit,
            adapter.NativeCallback,
            (void*)callbackData,
            imageAttr.Pointer()).ThrowIfFailed();

        GC.KeepAlive(imageAttr);
        GC.KeepAlive(metafile);
        GC.KeepAlive(this);
    }

    public void EnumerateMetafile(
        Metafile metafile,
        Point destPoint,
        Rectangle srcRect,
        GraphicsUnit unit,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr) => EnumerateMetafile(
            metafile,
            (PointF)destPoint,
            (RectangleF)srcRect,
            unit,
            callback,
            callbackData,
            imageAttr);

    public void EnumerateMetafile(
        Metafile metafile,
        RectangleF destRect,
        RectangleF srcRect,
        GraphicsUnit unit,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr)
    {
        using EnumerateMetafileDataAdapter adapter = new(callback);
        PInvokeGdiPlus.GdipEnumerateMetafileSrcRectDestRect(
            NativeGraphics,
            metafile.Pointer(),
            (RectF*)&destRect,
            (RectF*)&srcRect,
            (Unit)unit,
            adapter.NativeCallback,
            (void*)callbackData,
            imageAttr.Pointer()).ThrowIfFailed();

        GC.KeepAlive(imageAttr);
        GC.KeepAlive(metafile);
        GC.KeepAlive(this);
    }

    public void EnumerateMetafile(
        Metafile metafile,
        Rectangle destRect,
        Rectangle srcRect,
        GraphicsUnit unit,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr) => EnumerateMetafile(metafile, (RectangleF)destRect, srcRect, unit, callback, callbackData, imageAttr);

    public void EnumerateMetafile(
        Metafile metafile,
        PointF[] destPoints,
        RectangleF srcRect,
        GraphicsUnit unit,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr)
    {
        ArgumentNullException.ThrowIfNull(destPoints);

        if (destPoints.Length != 3)
            throw new ArgumentException(SR.GdiplusDestPointsInvalidParallelogram);

        fixed (PointF* p = destPoints)
        {
            using EnumerateMetafileDataAdapter adapter = new(callback);
            PInvokeGdiPlus.GdipEnumerateMetafileSrcRectDestPoints(
                NativeGraphics,
                metafile.Pointer(),
                (GdiPlus.PointF*)p, destPoints.Length,
                (RectF*)&srcRect,
                (Unit)unit,
                adapter.NativeCallback,
                (void*)callbackData,
                imageAttr.Pointer()).ThrowIfFailed();

            GC.KeepAlive(imageAttr);
            GC.KeepAlive(metafile);
            GC.KeepAlive(this);
        }
    }

    public void EnumerateMetafile(
        Metafile metafile,
        Point[] destPoints,
        Rectangle srcRect,
        GraphicsUnit unit,
        EnumerateMetafileProc callback,
        IntPtr callbackData,
        ImageAttributes? imageAttr)
    {
        ArgumentNullException.ThrowIfNull(destPoints);

        if (destPoints.Length != 3)
            throw new ArgumentException(SR.GdiplusDestPointsInvalidParallelogram);

        fixed (Point* p = destPoints)
        {
            using EnumerateMetafileDataAdapter adapter = new(callback);
            PInvokeGdiPlus.GdipEnumerateMetafileSrcRectDestPointsI(
                NativeGraphics,
                metafile.Pointer(),
                (GdiPlus.Point*)p, destPoints.Length,
                (Rect*)&srcRect,
                (Unit)unit,
                adapter.NativeCallback,
                (void*)callbackData,
                imageAttr.Pointer()).ThrowIfFailed();

            GC.KeepAlive(imageAttr);
            GC.KeepAlive(metafile);
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Combines current Graphics context with all previous contexts.
    ///  When BeginContainer() is called, a copy of the current context is pushed into the GDI+ context stack, it keeps track of the
    ///  absolute clipping and transform but reset the public properties so it looks like a brand new context.
    ///  When Save() is called, a copy of the current context is also pushed in the GDI+ stack but the public clipping and transform
    ///  properties are not reset (cumulative). Consecutive Save context are ignored with the exception of the top one which contains
    ///  all previous information.
    ///  The return value is an object array where the first element contains the cumulative clip region and the second the cumulative
    ///  translate transform matrix.
    ///  WARNING: This method is for internal FX support only.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the Graphics.GetContextInfo overloads that accept arguments for better performance and fewer allocations.", DiagnosticId = "SYSLIB0016", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    [SupportedOSPlatform("windows")]
    public object GetContextInfo()
    {
        GetContextInfo(out Matrix3x2 cumulativeTransform, calculateClip: true, out Region? cumulativeClip);
        return new object[] { cumulativeClip ?? new Region(), new Matrix(cumulativeTransform) };
    }

    private void GetContextInfo(out Matrix3x2 cumulativeTransform, bool calculateClip, out Region? cumulativeClip)
    {
        cumulativeClip = calculateClip ? GetRegionIfNotInfinite() : null;   // Current context clip.
        cumulativeTransform = TransformElements;                            // Current context transform.
        Vector2 currentOffset = default;                                    // Offset of current context.
        Vector2 totalOffset = default;                                      // Absolute coordinate offset of top context.

        GraphicsContext? context = _previousContext;

        if (!cumulativeTransform.IsIdentity)
        {
            currentOffset = cumulativeTransform.Translation;
        }

        while (context is not null)
        {
            if (!context.TransformOffset.IsEmpty())
            {
                cumulativeTransform.Translate(context.TransformOffset);
            }

            if (!currentOffset.IsEmpty())
            {
                // The location of the GDI+ clip region is relative to the coordinate origin after any translate transform
                // has been applied. We need to intersect regions using the same coordinate origin relative to the previous
                // context.

                // If we don't have a cumulative clip, we're infinite, and translation on infinite regions is a no-op.
                cumulativeClip?.Translate(currentOffset.X, currentOffset.Y);
                totalOffset.X += currentOffset.X;
                totalOffset.Y += currentOffset.Y;
            }

            // Context only stores clips if they are not infinite. Intersecting a clip with an infinite clip is a no-op.
            if (calculateClip && context.Clip is not null)
            {
                // Intersecting an infinite clip with another is just a copy of the second clip.
                if (cumulativeClip is null)
                {
                    cumulativeClip = context.Clip;
                }
                else
                {
                    cumulativeClip.Intersect(context.Clip);
                }
            }

            currentOffset = context.TransformOffset;

            // Ignore subsequent cumulative contexts.
            do
            {
                context = context.Previous;

                if (context is null || !context.Next!.IsCumulative)
                {
                    break;
                }
            }
            while (context.IsCumulative);
        }

        if (!totalOffset.IsEmpty())
        {
            // We need now to reset the total transform in the region so when calling Region.GetHRgn(Graphics)
            // the HRegion is properly offset by GDI+ based on the total offset of the graphics object.

            // If we don't have a cumulative clip, we're infinite, and translation on infinite regions is a no-op.
            cumulativeClip?.Translate(-totalOffset.X, -totalOffset.Y);
        }
    }

    (HDC hdc, int saveState) IGraphicsContextInfo.GetHdc(ApplyGraphicsProperties apply, bool alwaysSaveState)
    {
        bool applyTransform = apply.HasFlag(ApplyGraphicsProperties.TranslateTransform);
        bool applyClipping = apply.HasFlag(ApplyGraphicsProperties.Clipping);

        int saveState = 0;
        HDC hdc = HDC.Null;

        Region? clipRegion = null;
        PointF offset = default;
        if (applyClipping)
        {
            GetContextInfo(out offset, out clipRegion);
        }
        else if (applyTransform)
        {
            GetContextInfo(out offset);
        }

        using (clipRegion)
        {
            applyTransform = applyTransform && !offset.IsEmpty;
            applyClipping = clipRegion is not null;

            using var graphicsRegion = applyClipping ? clipRegion!.GetRegionScope(this) : default;
            applyClipping = applyClipping && !graphicsRegion!.Region.IsNull;

            hdc = (HDC)GetHdc();

            if (alwaysSaveState || applyClipping || applyTransform)
            {
                saveState = PInvokeCore.SaveDC(hdc);
            }

            if (applyClipping)
            {
                // If the Graphics object was created from a native DC the actual clipping region is the intersection
                // between the original DC clip region and the GDI+ one - for display Graphics it is the same as
                // Graphics.VisibleClipBounds.

                GDI_REGION_TYPE type;

                using RegionScope dcRegion = new(hdc);
                if (!dcRegion.IsNull)
                {
                    type = PInvokeCore.CombineRgn(graphicsRegion!, dcRegion, graphicsRegion!, RGN_COMBINE_MODE.RGN_AND);
                    if (type == GDI_REGION_TYPE.RGN_ERROR)
                    {
                        throw new Win32Exception();
                    }
                }

                type = PInvokeCore.SelectClipRgn(hdc, graphicsRegion!);
                if (type == GDI_REGION_TYPE.RGN_ERROR)
                {
                    throw new Win32Exception();
                }
            }

            if (applyTransform)
            {
                PInvokeCore.OffsetViewportOrgEx(hdc, (int)offset.X, (int)offset.Y, lppt: null);
            }
        }

        return (hdc, saveState);
    }

    /// <summary>
    ///  Gets the cumulative offset.
    /// </summary>
    /// <param name="offset">The cumulative offset.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SupportedOSPlatform("windows")]
    public void GetContextInfo(out PointF offset)
    {
        GetContextInfo(out Matrix3x2 cumulativeTransform, calculateClip: false, out _);
        Vector2 translation = cumulativeTransform.Translation;
        offset = new PointF(translation.X, translation.Y);
    }

    /// <summary>
    ///  Gets the cumulative offset and clip region.
    /// </summary>
    /// <param name="offset">The cumulative offset.</param>
    /// <param name="clip">The cumulative clip region or null if the clip region is infinite.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SupportedOSPlatform("windows")]
    public void GetContextInfo(out PointF offset, out Region? clip)
    {
        GetContextInfo(out Matrix3x2 cumulativeTransform, calculateClip: true, out clip);
        Vector2 translation = cumulativeTransform.Translation;
        offset = new PointF(translation.X, translation.Y);
    }

    public RectangleF VisibleClipBounds
    {
        get
        {
            if (PrintingHelper is PrintPreviewGraphics ppGraphics)
                return ppGraphics.VisibleClipBounds;

            RectF rect;
            CheckStatus(PInvokeGdiPlus.GdipGetVisibleClipBounds(NativeGraphics, &rect));
            return rect;
        }
    }

    /// <summary>
    ///  Saves the current context into the context stack.
    /// </summary>
    private void PushContext(GraphicsContext context)
    {
        Debug.Assert(context is not null && context.State != 0, "GraphicsContext object is null or not valid.");

        if (_previousContext is not null)
        {
            // Push context.
            context.Previous = _previousContext;
            _previousContext.Next = context;
        }

        _previousContext = context;
    }

    /// <summary>
    ///  Pops all contexts from the specified one included. The specified context is becoming the current context.
    /// </summary>
    private void PopContext(int currentContextState)
    {
        Debug.Assert(_previousContext is not null, "Trying to restore a context when the stack is empty");
        GraphicsContext? context = _previousContext;

        // Pop all contexts up the stack.
        while (context is not null)
        {
            if (context.State == currentContextState)
            {
                _previousContext = context.Previous;

                // This will dispose all context object up the stack.
                context.Dispose();
                return;
            }

            context = context.Previous;
        }

        Debug.Fail("Warning: context state not found!");
    }

    public GraphicsState Save()
    {
        GraphicsContext context = new(this);
        uint state;
        Status status = PInvokeGdiPlus.GdipSaveGraphics(NativeGraphics, &state);
        GC.KeepAlive(this);

        if (status != Status.Ok)
        {
            context.Dispose();
            throw Gdip.StatusException(status);
        }

        context.State = (int)state;
        context.IsCumulative = true;
        PushContext(context);

        return new GraphicsState((int)state);
    }

    public void Restore(GraphicsState gstate)
    {
        CheckStatus(PInvokeGdiPlus.GdipRestoreGraphics(NativeGraphics, (uint)gstate._nativeState));
        PopContext(gstate._nativeState);
    }

    public GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit)
    {
        GraphicsContext context = new(this);

        uint state;
        Status status = PInvokeGdiPlus.GdipBeginContainer(NativeGraphics, (RectF*)&dstrect, (RectF*)&srcrect, (Unit)unit, &state);
        GC.KeepAlive(this);

        if (status != Status.Ok)
        {
            context.Dispose();
            throw Gdip.StatusException(status);
        }

        context.State = (int)state;
        PushContext(context);

        return new GraphicsContainer((int)state);
    }

    public GraphicsContainer BeginContainer()
    {
        GraphicsContext context = new(this);
        uint state;
        Status status = PInvokeGdiPlus.GdipBeginContainer2(NativeGraphics, &state);
        GC.KeepAlive(this);

        if (status != Status.Ok)
        {
            context.Dispose();
            throw Gdip.StatusException(status);
        }

        context.State = (int)state;
        PushContext(context);

        return new GraphicsContainer((int)state);
    }

    public void EndContainer(GraphicsContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);
        CheckStatus(PInvokeGdiPlus.GdipEndContainer(NativeGraphics, (uint)container._nativeGraphicsContainer));
        PopContext(container._nativeGraphicsContainer);
    }

    public GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit)
        => BeginContainer((RectangleF)dstrect, (RectangleF)srcrect, unit);

    public void AddMetafileComment(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        fixed (byte* b = data)
        {
            CheckStatus(PInvokeGdiPlus.GdipComment(NativeGraphics, (uint)data.Length, b));
        }
    }

    public static IntPtr GetHalftonePalette()
    {
        if (s_halftonePalette.IsNull)
        {
            lock (s_syncObject)
            {
                if (s_halftonePalette.IsNull)
                {
                    GdiPlusInitialization.EnsureInitialized();
                    s_halftonePalette = PInvokeGdiPlus.GdipCreateHalftonePalette();
                }
            }
        }

        return s_halftonePalette;
    }

    /// <summary>
    ///  GDI+ will return a 'generic error' with specific win32 last error codes when
    ///  a terminal server session has been closed, minimized, etc... We don't want
    ///  to throw when this happens, so we'll guard against this by looking at the
    ///  'last win32 error code' and checking to see if it is either 1) access denied
    ///  or 2) proc not found and then ignore it.
    ///
    ///  The problem is that when you lock the machine, the secure desktop is enabled and
    ///  rendering fails which is expected (since the app doesn't have permission to draw
    ///  on the secure desktop). Not sure if there's anything you can do, short of catching
    ///  the desktop switch message and absorbing all the exceptions that get thrown while
    ///  it's the secure desktop.
    /// </summary>
    private void CheckErrorStatus(Status status)
    {
        if (status == Status.Ok)
        {
            return;
        }

        // Generic error from GDI+ can be GenericError or Win32Error.
        if (status is Status.GenericError or Status.Win32Error)
        {
            WIN32_ERROR error = (WIN32_ERROR)Marshal.GetLastWin32Error();
            if (error == WIN32_ERROR.ERROR_ACCESS_DENIED || error == WIN32_ERROR.ERROR_PROC_NOT_FOUND ||
                    // Here, we'll check to see if we are in a terminal services session.
                    (((PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_REMOTESESSION) & 0x00000001) != 0) && (error == 0)))
            {
                return;
            }
        }

        GC.KeepAlive(this);

        // Legitimate error, throw our status exception.
        throw Gdip.StatusException(status);
    }

#if NET8_0_OR_GREATER

    /// <summary>
    ///  Draws the given <paramref name="cachedBitmap"/>.
    /// </summary>
    /// <param name="cachedBitmap">The <see cref="CachedBitmap"/> that contains the image to be drawn.</param>
    /// <param name="x">The x-coordinate of the upper-left corner of the drawn image.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the drawn image.</param>
    /// <exception cref="ArgumentNullException">
    ///  <para><paramref name="cachedBitmap"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///  <para>
    ///   The <paramref name="cachedBitmap"/> is not compatible with the <see cref="Graphics"/> device state.
    ///  </para>
    ///  <para>
    ///  - or -
    ///  </para>
    ///  <para>
    ///   The <see cref="Graphics"/> object has a transform applied other than a translation.
    ///  </para>
    /// </exception>
    public void DrawCachedBitmap(CachedBitmap cachedBitmap, int x, int y)
    {
        ArgumentNullException.ThrowIfNull(cachedBitmap);

        CheckStatus(PInvokeGdiPlus.GdipDrawCachedBitmap(
            NativeGraphics,
            (GpCachedBitmap*)cachedBitmap.Handle,
            x, y));

        GC.KeepAlive(cachedBitmap);
    }
#endif

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="DrawImage(Image, Effect, RectangleF, Matrix?, GraphicsUnit, ImageAttributes?)"/>
    public void DrawImage(
        Image image,
        Effect effect) => DrawImage(image, effect, srcRect: default, transform: default, GraphicsUnit.Pixel, imageAttr: null);

    /// <summary>
    ///  Draws a portion of an image after applying a specified effect.
    /// </summary>
    /// <param name="image"><see cref="Image"/> to be drawn.</param>
    /// <param name="effect">The effect to be applied when drawing.</param>
    /// <param name="srcRect">The portion of the image to be drawn. <see cref="RectangleF.Empty"/> draws the full image.</param>
    /// <param name="transform">The transform to apply to the <paramref name="srcRect"/> to determine the destination.</param>
    /// <param name="srcUnit">Unit of measure of the <paramref name="srcRect"/>.</param>
    /// <param name="imageAttr">Additional adjustments to be applied, if any.</param>
    public void DrawImage(
        Image image,
        Effect effect,
        RectangleF srcRect = default,
        Matrix? transform = default,
        GraphicsUnit srcUnit = GraphicsUnit.Pixel,
        ImageAttributes? imageAttr = default)
    {
        PInvokeGdiPlus.GdipDrawImageFX(
            NativeGraphics,
            image.Pointer(),
            srcRect.IsEmpty ? null : (RectF*)&srcRect,
            transform.Pointer(),
            effect.Pointer(),
            imageAttr.Pointer(),
            (Unit)srcUnit).ThrowIfFailed();

        GC.KeepAlive(effect);
        GC.KeepAlive(imageAttr);
        GC.KeepAlive(image);
        GC.KeepAlive(this);
    }
#endif

    private void CheckStatus(Status status)
    {
        status.ThrowIfFailed();
        GC.KeepAlive(this);
    }
}
