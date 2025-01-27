// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing.Drawing2D;

/// <summary>
///  Encapsulates a <see cref="Brush"/> object that fills the interior of a <see cref="GraphicsPath"/> object with a gradient.
/// </summary>
public sealed unsafe class PathGradientBrush : Brush
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="PathGradientBrush"/> class with the specified points.
    /// </summary>
    public PathGradientBrush(params PointF[] points) : this(points, WrapMode.Clamp) { }

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="PathGradientBrush(PointF[])"/>
    public PathGradientBrush(params ReadOnlySpan<PointF> points) : this(WrapMode.Clamp, points) { }
#endif

    /// <summary>
    ///  Initializes a new instance of the <see cref="PathGradientBrush"/> class with the specified points and
    ///  <see cref="WrapMode"/>.
    /// </summary>
    public PathGradientBrush(PointF[] points, WrapMode wrapMode) : this(wrapMode, points.OrThrowIfNull().AsSpan()) { }

    /// <inheritdoc cref="PathGradientBrush(PointF[], WrapMode)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    PathGradientBrush(WrapMode wrapMode, params ReadOnlySpan<PointF> points)
    {
        if (wrapMode is < WrapMode.Tile or > WrapMode.Clamp)
            throw new InvalidEnumArgumentException(nameof(wrapMode), (int)wrapMode, typeof(WrapMode));

        if (points.Length < 2)
            throw new ArgumentException(null, nameof(points));

        fixed (PointF* p = points)
        {
            GpPathGradient* nativeBrush;
            PInvokeGdiPlus.GdipCreatePathGradient(
                (GdiPlus.PointF*)p,
                points.Length,
                (GdiPlus.WrapMode)wrapMode,
                &nativeBrush).ThrowIfFailed();

            SetNativeBrushInternal((GpBrush*)nativeBrush);
        }
    }

    /// <inheritdoc cref="PathGradientBrush(PointF[])"/>
    public PathGradientBrush(params Point[] points) : this(points, WrapMode.Clamp) { }

    /// <inheritdoc cref="PathGradientBrush(PointF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    PathGradientBrush(params ReadOnlySpan<Point> points) : this(WrapMode.Clamp, points) { }

    /// <inheritdoc cref="PathGradientBrush(PointF[], WrapMode)"/>
    public PathGradientBrush(Point[] points, WrapMode wrapMode) : this(wrapMode, points.OrThrowIfNull().AsSpan()) { }

    /// <inheritdoc cref="PathGradientBrush(PointF[], WrapMode)"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    PathGradientBrush(WrapMode wrapMode, params ReadOnlySpan<Point> points)
    {
        if (wrapMode is < WrapMode.Tile or > WrapMode.Clamp)
            throw new InvalidEnumArgumentException(nameof(wrapMode), (int)wrapMode, typeof(WrapMode));

        if (points.Length < 2)
            throw new ArgumentException(null, nameof(points));

        fixed (Point* p = points)
        {
            GpPathGradient* nativeBrush;
            PInvokeGdiPlus.GdipCreatePathGradientI(
                (GdiPlus.Point*)p,
                points.Length,
                (GdiPlus.WrapMode)wrapMode,
                &nativeBrush).ThrowIfFailed();

            SetNativeBrushInternal((GpBrush*)nativeBrush);
        }
    }

    public PathGradientBrush(GraphicsPath path)
    {
        ArgumentNullException.ThrowIfNull(path);
        GpPathGradient* nativeBrush;
        PInvokeGdiPlus.GdipCreatePathGradientFromPath(path._nativePath, &nativeBrush).ThrowIfFailed();
        SetNativeBrushInternal((GpBrush*)nativeBrush);
    }

    internal PathGradientBrush(GpPathGradient* nativeBrush)
    {
        Debug.Assert(nativeBrush is not null, "Initializing native brush with null.");
        SetNativeBrushInternal((GpBrush*)nativeBrush);
    }

    internal GpPathGradient* NativePathGradient => (GpPathGradient*)NativeBrush;

    public override object Clone()
    {
        GpBrush* clonedBrush;
        PInvokeGdiPlus.GdipCloneBrush(NativeBrush, &clonedBrush).ThrowIfFailed();
        GC.KeepAlive(this);
        return new PathGradientBrush((GpPathGradient*)clonedBrush);
    }

    public Color CenterColor
    {
        get
        {
            ARGB argb;
            PInvokeGdiPlus.GdipGetPathGradientCenterColor(NativePathGradient, (uint*)&argb).ThrowIfFailed();
            GC.KeepAlive(this);
            return argb;
        }
        set
        {
            PInvokeGdiPlus.GdipSetPathGradientCenterColor(NativePathGradient, (ARGB)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public Color[] SurroundColors
    {
        get
        {
            int count;
            PInvokeGdiPlus.GdipGetPathGradientSurroundColorCount(NativePathGradient, &count).ThrowIfFailed();

            using ArgbBuffer buffer = new(count);
            fixed (uint* b = buffer)
            {
                PInvokeGdiPlus.GdipGetPathGradientSurroundColorsWithCount(NativePathGradient, b, &count).ThrowIfFailed();
            }

            GC.KeepAlive(this);
            return buffer.ToColorArray(count);
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            using ArgbBuffer buffer = new(value);

            int count = value.Length;
            fixed (uint* b = buffer)
            {
                PInvokeGdiPlus.GdipSetPathGradientSurroundColorsWithCount(
                    NativePathGradient,
                    b,
                    &count).ThrowIfFailed();

                GC.KeepAlive(this);
            }
        }
    }

    public PointF CenterPoint
    {
        get
        {
            PointF point;
            PInvokeGdiPlus.GdipGetPathGradientCenterPoint(NativePathGradient, (GdiPlus.PointF*)&point).ThrowIfFailed();
            GC.KeepAlive(this);
            return point;
        }
        set
        {
            PInvokeGdiPlus.GdipSetPathGradientCenterPoint(NativePathGradient, (GdiPlus.PointF*)&value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public RectangleF Rectangle
    {
        get
        {
            RectangleF rect;
            PInvokeGdiPlus.GdipGetPathGradientRect(NativePathGradient, (RectF*)&rect).ThrowIfFailed();
            GC.KeepAlive(this);
            return rect;
        }
    }

    public Blend Blend
    {
        get
        {
            // Figure out the size of blend factor array
            int count;
            PInvokeGdiPlus.GdipGetPathGradientBlendCount(NativePathGradient, &count).ThrowIfFailed();

            float[] factors = new float[count];
            float[] positions = new float[count];

            // Retrieve horizontal blend factors
            fixed (float* f = factors, p = positions)
            {
                PInvokeGdiPlus.GdipGetPathGradientBlend(NativePathGradient, f, p, count).ThrowIfFailed();
            }

            // Return the result in a managed array

            Blend blend = new(factors.Length)
            {
                Factors = factors,
                Positions = positions
            };

            GC.KeepAlive(this);
            return blend;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(value.Factors);

            if (value.Positions is null || value.Positions.Length != value.Factors.Length)
                throw new ArgumentException(SR.Format(SR.InvalidArgumentValue, "value.Positions", value.Positions), nameof(value));

            int count = value.Factors.Length;

            fixed (float* f = value.Factors, p = value.Positions)
            {
                // Set blend factors
                PInvokeGdiPlus.GdipSetPathGradientBlend(NativePathGradient, f, p, count).ThrowIfFailed();
                GC.KeepAlive(this);
            }
        }
    }

    public void SetSigmaBellShape(float focus) => SetSigmaBellShape(focus, (float)1.0);

    public void SetSigmaBellShape(float focus, float scale)
    {
        PInvokeGdiPlus.GdipSetPathGradientSigmaBlend(NativePathGradient, focus, scale).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void SetBlendTriangularShape(float focus) => SetBlendTriangularShape(focus, (float)1.0);

    public void SetBlendTriangularShape(float focus, float scale)
    {
        PInvokeGdiPlus.GdipSetPathGradientLinearBlend(NativePathGradient, focus, scale).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public ColorBlend InterpolationColors
    {
        get
        {
            // Figure out the size of blend factor array
            int count;
            PInvokeGdiPlus.GdipGetPathGradientPresetBlendCount(NativePathGradient, &count).ThrowIfFailed();

            if (count == 0)
            {
                return new ColorBlend();
            }

            using ArgbBuffer colors = new(count);
            float[] positions = new float[count];

            ColorBlend blend = new(count);

            fixed (uint* c = colors)
            fixed (float* p = positions)
            {
                // Retrieve horizontal blend factors
                PInvokeGdiPlus.GdipGetPathGradientPresetBlend(NativePathGradient, c, p, count).ThrowIfFailed();
            }

            blend.Positions = positions;
            blend.Colors = colors.ToColorArray(count);
            GC.KeepAlive(this);
            return blend;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            int count = value.Colors.Length;

            if (value.Positions is null || value.Colors.Length != value.Positions.Length)
                throw new ArgumentException(SR.Format(SR.InvalidArgumentValue, "value.Positions", value.Positions), nameof(value));

            float[] positions = value.Positions;
            using ArgbBuffer argbColors = new(value.Colors);

            fixed (float* p = positions)
            fixed (uint* a = argbColors)
            {
                // Set blend factors
                PInvokeGdiPlus.GdipSetPathGradientPresetBlend(NativePathGradient, a, p, count).ThrowIfFailed();
                GC.KeepAlive(this);
            }
        }
    }

    public Matrix Transform
    {
        get
        {
            Matrix matrix = new();
            PInvokeGdiPlus.GdipGetPathGradientTransform(NativePathGradient, matrix.NativeMatrix).ThrowIfFailed();
            GC.KeepAlive(this);
            return matrix;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            PInvokeGdiPlus.GdipSetPathGradientTransform(NativePathGradient, value.NativeMatrix).ThrowIfFailed();
            GC.KeepAlive(value);
            GC.KeepAlive(this);
        }
    }

    public void ResetTransform()
    {
        PInvokeGdiPlus.GdipResetPathGradientTransform(NativePathGradient).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void MultiplyTransform(Matrix matrix) => MultiplyTransform(matrix, MatrixOrder.Prepend);

    public void MultiplyTransform(Matrix matrix, MatrixOrder order)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        PInvokeGdiPlus.GdipMultiplyPathGradientTransform(
            NativePathGradient,
            matrix.NativeMatrix,
            (GdiPlus.MatrixOrder)order).ThrowIfFailed();

        GC.KeepAlive(matrix);
        GC.KeepAlive(this);
    }

    public void TranslateTransform(float dx, float dy) => TranslateTransform(dx, dy, MatrixOrder.Prepend);

    public void TranslateTransform(float dx, float dy, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipTranslatePathGradientTransform(NativePathGradient, dx, dy, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void ScaleTransform(float sx, float sy) => ScaleTransform(sx, sy, MatrixOrder.Prepend);

    public void ScaleTransform(float sx, float sy, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipScalePathGradientTransform(NativePathGradient, sx, sy, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void RotateTransform(float angle) => RotateTransform(angle, MatrixOrder.Prepend);

    public void RotateTransform(float angle, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipRotatePathGradientTransform(NativePathGradient, angle, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public PointF FocusScales
    {
        get
        {
            float scaleX;
            float scaleY;
            PInvokeGdiPlus.GdipGetPathGradientFocusScales(NativePathGradient, &scaleX, &scaleY).ThrowIfFailed();
            GC.KeepAlive(this);
            return new PointF(scaleX, scaleY);
        }
        set
        {
            PInvokeGdiPlus.GdipSetPathGradientFocusScales(NativePathGradient, value.X, value.Y).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public WrapMode WrapMode
    {
        get
        {
            WrapMode mode;
            PInvokeGdiPlus.GdipGetPathGradientWrapMode(NativePathGradient, (GdiPlus.WrapMode*)&mode).ThrowIfFailed();
            GC.KeepAlive(this);
            return mode;
        }
        set
        {
            if (value is < WrapMode.Tile or > WrapMode.Clamp)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(WrapMode));

            PInvokeGdiPlus.GdipSetPathGradientWrapMode(NativePathGradient, (GdiPlus.WrapMode)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }
}
