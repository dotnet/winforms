// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing.Drawing2D;

public sealed unsafe class LinearGradientBrush : Brush
{
    private bool _interpolationColorsWasSet;

    public LinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2)
    {
        GpLineGradient* nativeBrush;
        PInvokeGdiPlus.GdipCreateLineBrush(
            (GdiPlus.PointF*)&point1, (GdiPlus.PointF*)&point2,
            (uint)color1.ToArgb(), (uint)color2.ToArgb(),
            GdiPlus.WrapMode.WrapModeTile,
            &nativeBrush).ThrowIfFailed();

        SetNativeBrushInternal((GpBrush*)nativeBrush);
    }

    public LinearGradientBrush(Point point1, Point point2, Color color1, Color color2)
        : this((PointF)point1, (PointF)point2, color1, color2)
    {
    }

    public LinearGradientBrush(RectangleF rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
    {
        if (linearGradientMode is < LinearGradientMode.Horizontal or > LinearGradientMode.BackwardDiagonal)
            throw new InvalidEnumArgumentException(nameof(linearGradientMode), (int)linearGradientMode, typeof(LinearGradientMode));

        if (rect.Width == 0.0 || rect.Height == 0.0)
            throw new ArgumentException(SR.Format(SR.GdiplusInvalidRectangle, rect.ToString()));

        GpLineGradient* nativeBrush;
        PInvokeGdiPlus.GdipCreateLineBrushFromRect(
            (RectF*)&rect,
            (ARGB)color1,
            (ARGB)color2,
            (GdiPlus.LinearGradientMode)linearGradientMode,
            GdiPlus.WrapMode.WrapModeTile,
            &nativeBrush).ThrowIfFailed();

        SetNativeBrushInternal((GpBrush*)nativeBrush);
    }

    public LinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode linearGradientMode)
        : this((RectangleF)rect, color1, color2, linearGradientMode)
    {
    }

    public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle)
        : this(rect, color1, color2, angle, isAngleScaleable: false)
    {
    }

    public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle, bool isAngleScaleable)
    {
        if (rect.Width == 0.0 || rect.Height == 0.0)
            throw new ArgumentException(SR.Format(SR.GdiplusInvalidRectangle, rect.ToString()));

        GpLineGradient* nativeBrush;
        PInvokeGdiPlus.GdipCreateLineBrushFromRectWithAngle(
            (RectF*)&rect,
            (ARGB)color1,
            (ARGB)color2,
            angle,
            isAngleScaleable,
            GdiPlus.WrapMode.WrapModeTile,
            &nativeBrush).ThrowIfFailed();

        SetNativeBrushInternal((GpBrush*)nativeBrush);
    }

    public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle)
        : this(rect, color1, color2, angle, isAngleScaleable: false)
    {
    }

    public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle, bool isAngleScaleable)
        : this((RectangleF)rect, color1, color2, angle, isAngleScaleable)
    {
    }

    internal LinearGradientBrush(GpLineGradient* nativeBrush)
    {
        Debug.Assert(nativeBrush is not null, "Initializing native brush with null.");
        SetNativeBrushInternal((GpBrush*)nativeBrush);
    }

    internal GpLineGradient* NativeLineGradient => (GpLineGradient*)NativeBrush;

    public override object Clone()
    {
        GpBrush* clonedBrush;
        PInvokeGdiPlus.GdipCloneBrush(NativeBrush, &clonedBrush).ThrowIfFailed();
        GC.KeepAlive(this);
        return new LinearGradientBrush((GpLineGradient*)clonedBrush);
    }

    public Color[] LinearColors
    {
        get
        {
            uint* colors = stackalloc uint[2];
            PInvokeGdiPlus.GdipGetLineColors(NativeLineGradient, colors).ThrowIfFailed();
            GC.KeepAlive(this);

            return
            [
                Color.FromArgb((int)colors[0]),
                Color.FromArgb((int)colors[1])
            ];
        }
        set
        {
            PInvokeGdiPlus.GdipSetLineColors(NativeLineGradient, (uint)value[0].ToArgb(), (uint)value[1].ToArgb()).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public RectangleF Rectangle
    {
        get
        {
            RectangleF rect;
            PInvokeGdiPlus.GdipGetLineRect(NativeLineGradient, (RectF*)&rect).ThrowIfFailed();
            GC.KeepAlive(this);
            return rect;
        }
    }

    public bool GammaCorrection
    {
        get
        {
            BOOL useGammaCorrection;
            PInvokeGdiPlus.GdipGetLineGammaCorrection(NativeLineGradient, &useGammaCorrection).ThrowIfFailed();
            GC.KeepAlive(this);
            return useGammaCorrection;
        }
        set
        {
            PInvokeGdiPlus.GdipSetLineGammaCorrection(NativeLineGradient, value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public Blend? Blend
    {
        get
        {
            // Interpolation colors and blends don't work together very well. Getting the Blend when InterpolationColors
            // is set puts the Brush into an unusable state afterwards.
            // Bail out here to avoid that.
            if (_interpolationColorsWasSet)
                return null;

            int count;
            PInvokeGdiPlus.GdipGetLineBlendCount(NativeLineGradient, &count).ThrowIfFailed();
            GC.KeepAlive(this);

            if (count <= 0)
            {
                return null;
            }

            Blend blend = new(count);

            fixed (float* f = blend.Factors, p = blend.Positions)
            {
                PInvokeGdiPlus.GdipGetLineBlend(NativeLineGradient, f, p, count).ThrowIfFailed();
                GC.KeepAlive(this);
                return blend;
            }
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(value.Factors);

            if (value.Positions is null || value.Positions.Length != value.Factors.Length)
                throw new ArgumentException(SR.Format(SR.InvalidArgumentValue, "value.Positions", value.Positions), nameof(value));

            fixed (float* f = value.Factors, p = value.Positions)
            {
                // Set blend factors.
                PInvokeGdiPlus.GdipSetLineBlend(NativeLineGradient, f, p, value.Factors.Length).ThrowIfFailed();
                GC.KeepAlive(this);
            }
        }
    }

    public void SetSigmaBellShape(float focus) => SetSigmaBellShape(focus, (float)1.0);

    public void SetSigmaBellShape(float focus, float scale)
    {
        PInvokeGdiPlus.GdipSetLineSigmaBlend(NativeLineGradient, focus, scale).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void SetBlendTriangularShape(float focus) => SetBlendTriangularShape(focus, (float)1.0);

    public void SetBlendTriangularShape(float focus, float scale)
    {
        _interpolationColorsWasSet = false;
        PInvokeGdiPlus.GdipSetLineLinearBlend(NativeLineGradient, focus, scale).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public ColorBlend InterpolationColors
    {
        get
        {
            int count;
            PInvokeGdiPlus.GdipGetLinePresetBlendCount(NativeLineGradient, &count).ThrowIfFailed();

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
                PInvokeGdiPlus.GdipGetLinePresetBlend(NativeLineGradient, c, p, count).ThrowIfFailed();
            }

            blend.Positions = positions;
            blend.Colors = colors.ToColorArray(count);
            GC.KeepAlive(this);
            return blend;
        }
        set
        {
            _interpolationColorsWasSet = true;
            ArgumentNullException.ThrowIfNull(value);
            int count = value.Colors.Length;

            if (value.Positions is null)
                throw new ArgumentException(SR.Format(SR.InvalidArgumentValue, "value.Positions", value.Positions), nameof(value));
            if (value.Colors.Length != value.Positions.Length)
                throw new ArgumentException(message: null, nameof(value));

            float[] positions = value.Positions;
            using ArgbBuffer argbValues = new(value.Colors);

            fixed (float* p = positions)
            fixed (uint* a = argbValues)
            {
                // Set blend factors
                PInvokeGdiPlus.GdipSetLinePresetBlend(NativeLineGradient, a, p, count).ThrowIfFailed();
                GC.KeepAlive(this);
            }
        }
    }

    public WrapMode WrapMode
    {
        get
        {
            WrapMode mode;
            PInvokeGdiPlus.GdipGetLineWrapMode(NativeLineGradient, (GdiPlus.WrapMode*)&mode).ThrowIfFailed();
            GC.KeepAlive(this);
            return mode;
        }
        set
        {
            if (value is < WrapMode.Tile or > WrapMode.Clamp)
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(WrapMode));

            PInvokeGdiPlus.GdipSetLineWrapMode(NativeLineGradient, (GdiPlus.WrapMode)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public Matrix Transform
    {
        get
        {
            Matrix matrix = new();
            PInvokeGdiPlus.GdipGetLineTransform(NativeLineGradient, matrix.NativeMatrix).ThrowIfFailed();
            GC.KeepAlive(this);
            return matrix;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            PInvokeGdiPlus.GdipSetLineTransform(NativeLineGradient, value.NativeMatrix).ThrowIfFailed();
            GC.KeepAlive(value);
            GC.KeepAlive(this);
        }
    }

    public void ResetTransform()
    {
        PInvokeGdiPlus.GdipResetLineTransform(NativeLineGradient).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void MultiplyTransform(Matrix matrix) => MultiplyTransform(matrix, MatrixOrder.Prepend);

    public void MultiplyTransform(Matrix matrix, MatrixOrder order)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        PInvokeGdiPlus.GdipMultiplyLineTransform(NativeLineGradient, matrix.NativeMatrix, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void TranslateTransform(float dx, float dy) => TranslateTransform(dx, dy, MatrixOrder.Prepend);

    public void TranslateTransform(float dx, float dy, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipTranslateLineTransform(NativeLineGradient, dx, dy, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void ScaleTransform(float sx, float sy) => ScaleTransform(sx, sy, MatrixOrder.Prepend);

    public void ScaleTransform(float sx, float sy, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipScaleLineTransform(NativeLineGradient, sx, sy, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void RotateTransform(float angle) => RotateTransform(angle, MatrixOrder.Prepend);

    public void RotateTransform(float angle, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipRotateLineTransform(NativeLineGradient, angle, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }
}
