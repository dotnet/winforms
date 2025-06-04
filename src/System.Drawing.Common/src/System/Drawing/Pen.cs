// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Internal;

namespace System.Drawing;

/// <summary>
///  Defines an object used to draw lines and curves.
/// </summary>
public sealed unsafe class Pen : MarshalByRefObject, ICloneable, IDisposable, ISystemColorTracker
{
    // Handle to native GDI+ pen object.
    private GpPen* _nativePen;

    // GDI+ doesn't understand system colors, so we need to cache the value here.
    private Color _color;
    private bool _immutable;

    // Tracks whether the dash style has been changed to something else than Solid during the lifetime of this object.
    private bool _dashStyleWasOrIsNotSolid;

    /// <summary>
    ///  Creates a Pen from a native GDI+ object.
    /// </summary>
    private Pen(GpPen* nativePen) => SetNativePen(nativePen);

    internal Pen(Color color, bool immutable) : this(color) => _immutable = immutable;

    /// <summary>
    ///  Initializes a new instance of the Pen class with the specified <see cref='Color'/>.
    /// </summary>
    public Pen(Color color) : this(color, (float)1.0)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Pen'/> class with the specified
    ///  <see cref='Color'/> and <see cref='Width'/>.
    /// </summary>
    public Pen(Color color, float width)
    {
        _color = color;

        GpPen* pen;
        PInvokeGdiPlus.GdipCreatePen1((uint)color.ToArgb(), width, (int)GraphicsUnit.World, &pen).ThrowIfFailed();
        SetNativePen(pen);

        if (_color.IsSystemColor)
        {
            SystemColorTracker.Add(this);
        }
    }

    /// <summary>
    ///  Initializes a new instance of the Pen class with the specified <see cref='Brush'/>.
    /// </summary>
    public Pen(Brush brush) : this(brush, (float)1.0)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='Pen'/> class with the specified <see cref='Drawing.Brush'/> and width.
    /// </summary>
    public Pen(Brush brush, float width)
    {
        ArgumentNullException.ThrowIfNull(brush);
        GpPen* pen;
        PInvokeGdiPlus.GdipCreatePen2(brush.NativeBrush, width, (int)GraphicsUnit.World, &pen).ThrowIfFailed();
        GC.KeepAlive(brush);
        SetNativePen(pen);
    }

    internal void SetNativePen(GpPen* nativePen)
    {
        Debug.Assert(nativePen is not null);
        _nativePen = nativePen;
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    internal GpPen* NativePen => _nativePen;

    /// <summary>
    ///  Creates an exact copy of this <see cref='Pen'/>.
    /// </summary>
    public object Clone()
    {
        GpPen* clonedPen;
        PInvokeGdiPlus.GdipClonePen(NativePen, &clonedPen).ThrowIfFailed();
        GC.KeepAlive(this);
        return new Pen(clonedPen);
    }

    /// <summary>
    ///  Cleans up Windows resources for this <see cref='Pen'/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing)
        {
            // If we are finalizing, then we will be unreachable soon. Finalize calls dispose to
            // release resources, so we must make sure that during finalization we are
            // not immutable.
            _immutable = false;
        }
        else if (_immutable)
        {
            throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
        }

        if (_nativePen is not null)
        {
            Status status = !Gdip.Initialized ? Status.Ok : PInvokeGdiPlus.GdipDeletePen(NativePen);
            _nativePen = null;
            Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
        }
    }

    /// <summary>
    ///  Cleans up Windows resources for this <see cref='Pen'/>.
    /// </summary>
    ~Pen() => Dispose(disposing: false);

    /// <summary>
    ///  Gets or sets the width of this <see cref='Pen'/>.
    /// </summary>
    public float Width
    {
        get
        {
            float width;
            PInvokeGdiPlus.GdipGetPenWidth(NativePen, &width).ThrowIfFailed();
            GC.KeepAlive(this);
            return width;
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenWidth(NativePen, value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Sets the values that determine the style of cap used to end lines drawn by this <see cref='Pen'/>.
    /// </summary>
    public void SetLineCap(LineCap startCap, LineCap endCap, DashCap dashCap)
    {
        if (_immutable)
        {
            throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
        }

        PInvokeGdiPlus.GdipSetPenLineCap197819(
            NativePen,
            (GdiPlus.LineCap)startCap,
            (GdiPlus.LineCap)endCap,
            (GdiPlus.DashCap)dashCap).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Gets or sets the cap style used at the beginning of lines drawn with this <see cref='Pen'/>.
    /// </summary>
    public LineCap StartCap
    {
        get
        {
            LineCap startCap;
            PInvokeGdiPlus.GdipGetPenStartCap(NativePen, (GdiPlus.LineCap*)&startCap).ThrowIfFailed();
            GC.KeepAlive(this);
            return startCap;
        }
        set
        {
            switch (value)
            {
                case LineCap.Flat:
                case LineCap.Square:
                case LineCap.Round:
                case LineCap.Triangle:
                case LineCap.NoAnchor:
                case LineCap.SquareAnchor:
                case LineCap.RoundAnchor:
                case LineCap.DiamondAnchor:
                case LineCap.ArrowAnchor:
                case LineCap.AnchorMask:
                case LineCap.Custom:
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(LineCap));
            }

            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenStartCap(NativePen, (GdiPlus.LineCap)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets the cap style used at the end of lines drawn with this <see cref='Pen'/>.
    /// </summary>
    public LineCap EndCap
    {
        get
        {
            LineCap endCap;
            PInvokeGdiPlus.GdipGetPenEndCap(NativePen, (GdiPlus.LineCap*)&endCap).ThrowIfFailed();
            GC.KeepAlive(this);
            return endCap;
        }
        set
        {
            switch (value)
            {
                case LineCap.Flat:
                case LineCap.Square:
                case LineCap.Round:
                case LineCap.Triangle:
                case LineCap.NoAnchor:
                case LineCap.SquareAnchor:
                case LineCap.RoundAnchor:
                case LineCap.DiamondAnchor:
                case LineCap.ArrowAnchor:
                case LineCap.AnchorMask:
                case LineCap.Custom:
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(LineCap));
            }

            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenEndCap(NativePen, (GdiPlus.LineCap)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets a custom cap style to use at the beginning of lines drawn with this <see cref='Pen'/>.
    /// </summary>
    public CustomLineCap CustomStartCap
    {
        get
        {
            GpCustomLineCap* lineCap;
            PInvokeGdiPlus.GdipGetPenCustomStartCap(NativePen, &lineCap).ThrowIfFailed();
            GC.KeepAlive(this);
            return CustomLineCap.CreateCustomLineCapObject(lineCap);
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenCustomStartCap(NativePen, value is null ? null : value._nativeCap).ThrowIfFailed();
            GC.KeepAlive(value);
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets a custom cap style to use at the end of lines drawn with this <see cref='Pen'/>.
    /// </summary>
    public CustomLineCap CustomEndCap
    {
        get
        {
            GpCustomLineCap* lineCap;
            PInvokeGdiPlus.GdipGetPenCustomEndCap(NativePen, &lineCap).ThrowIfFailed();
            GC.KeepAlive(this);
            return CustomLineCap.CreateCustomLineCapObject(lineCap);
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenCustomEndCap(NativePen, value is null ? null : value._nativeCap).ThrowIfFailed();
            GC.KeepAlive(value);
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets the cap style used at the beginning or end of dashed lines drawn with this <see cref='Pen'/>.
    /// </summary>
    public DashCap DashCap
    {
        get
        {
            DashCap dashCap;
            PInvokeGdiPlus.GdipGetPenDashCap197819(NativePen, (GdiPlus.DashCap*)&dashCap).ThrowIfFailed();
            GC.KeepAlive(this);
            return dashCap;
        }
        set
        {
            if (value is not DashCap.Flat and not DashCap.Round and not DashCap.Triangle)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DashCap));
            }

            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenDashCap197819(NativePen, (GdiPlus.DashCap)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets the join style for the ends of two overlapping lines drawn with this <see cref='Pen'/>.
    /// </summary>
    public LineJoin LineJoin
    {
        get
        {
            LineJoin lineJoin;
            PInvokeGdiPlus.GdipGetPenLineJoin(NativePen, (GdiPlus.LineJoin*)&lineJoin).ThrowIfFailed();
            GC.KeepAlive(this);
            return lineJoin;
        }
        set
        {
            if (value is < LineJoin.Miter or > LineJoin.MiterClipped)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(LineJoin));
            }

            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenLineJoin(NativePen, (GdiPlus.LineJoin)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets the limit of the thickness of the join on a mitered corner.
    /// </summary>
    public float MiterLimit
    {
        get
        {
            float miterLimit;
            PInvokeGdiPlus.GdipGetPenMiterLimit(NativePen, &miterLimit).ThrowIfFailed();
            GC.KeepAlive(this);
            return miterLimit;
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenMiterLimit(NativePen, value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets the alignment for objects drawn with this <see cref='Pen'/>.
    /// </summary>
    public PenAlignment Alignment
    {
        get
        {
            PenAlignment penMode;
            PInvokeGdiPlus.GdipGetPenMode(NativePen, (GdiPlus.PenAlignment*)&penMode).ThrowIfFailed();
            GC.KeepAlive(this);
            return penMode;
        }
        set
        {
            if (value is < PenAlignment.Center or > PenAlignment.Right)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(PenAlignment));
            }

            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenMode(NativePen, (GdiPlus.PenAlignment)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets the geometrical transform for objects drawn with this <see cref='Pen'/>.
    /// </summary>
    public Matrix Transform
    {
        get
        {
            Matrix matrix = new();
            PInvokeGdiPlus.GdipGetPenTransform(NativePen, matrix.NativeMatrix).ThrowIfFailed();
            GC.KeepAlive(this);
            return matrix;
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            ArgumentNullException.ThrowIfNull(value);

            PInvokeGdiPlus.GdipSetPenTransform(NativePen, value.NativeMatrix).ThrowIfFailed();
            GC.KeepAlive(value);
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Resets the geometric transform for this <see cref='Pen'/> to identity.
    /// </summary>
    public void ResetTransform()
    {
        PInvokeGdiPlus.GdipResetPenTransform(NativePen).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Multiplies the transform matrix for this <see cref='Pen'/> by the specified <see cref='Matrix'/>.
    /// </summary>
    public void MultiplyTransform(Matrix matrix) => MultiplyTransform(matrix, MatrixOrder.Prepend);

    /// <summary>
    ///  Multiplies the transform matrix for this <see cref='Pen'/> by the specified <see cref='Matrix'/> in the specified order.
    /// </summary>
    public void MultiplyTransform(Matrix matrix, MatrixOrder order)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        if (matrix.NativeMatrix is null)
        {
            // Disposed matrices should result in a no-op.
            return;
        }

        PInvokeGdiPlus.GdipMultiplyPenTransform(NativePen, matrix.NativeMatrix, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(matrix);
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Translates the local geometrical transform by the specified dimensions. This method prepends the translation
    ///  to the transform.
    /// </summary>
    public void TranslateTransform(float dx, float dy) => TranslateTransform(dx, dy, MatrixOrder.Prepend);

    /// <summary>
    ///  Translates the local geometrical transform by the specified dimensions in the specified order.
    /// </summary>
    public void TranslateTransform(float dx, float dy, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipTranslatePenTransform(NativePen, dx, dy, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Scales the local geometric transform by the specified amounts. This method prepends the scaling matrix to the transform.
    /// </summary>
    public void ScaleTransform(float sx, float sy) => ScaleTransform(sx, sy, MatrixOrder.Prepend);

    /// <summary>
    ///  Scales the local geometric transform by the specified amounts in the specified order.
    /// </summary>
    public void ScaleTransform(float sx, float sy, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipScalePenTransform(NativePen, sx, sy, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Rotates the local geometric transform by the specified amount. This method prepends the rotation to the transform.
    /// </summary>
    public void RotateTransform(float angle) => RotateTransform(angle, MatrixOrder.Prepend);

    /// <summary>
    ///  Rotates the local geometric transform by the specified amount in the specified order.
    /// </summary>
    public void RotateTransform(float angle, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipRotatePenTransform(NativePen, angle, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    private void InternalSetColor(Color value)
    {
        PInvokeGdiPlus.GdipSetPenColor(NativePen, (uint)_color.ToArgb()).ThrowIfFailed();
        GC.KeepAlive(this);
        _color = value;
    }

    /// <summary>
    ///  Gets the style of lines drawn with this <see cref='Pen'/>.
    /// </summary>
    public Drawing2D.PenType PenType
    {
        get
        {
            GdiPlus.PenType type;
            PInvokeGdiPlus.GdipGetPenFillType(NativePen, &type).ThrowIfFailed();
            GC.KeepAlive(this);
            return (Drawing2D.PenType)type;
        }
    }

    /// <summary>
    ///  Gets or sets the color of this <see cref='Pen'/>.
    /// </summary>
    public Color Color
    {
        get
        {
            if (_color == Color.Empty)
            {
                if (PenType != Drawing2D.PenType.SolidColor)
                {
                    throw new ArgumentException(SR.GdiplusInvalidParameter);
                }

                ARGB color;
                PInvokeGdiPlus.GdipGetPenColor(NativePen, (uint*)&color).ThrowIfFailed();
                GC.KeepAlive(this);
                _color = color;
            }

            // GDI+ doesn't understand system colors, so we can't use GdipGetPenColor in the general case.
            return _color;
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            if (value != _color)
            {
                Color oldColor = _color;
                _color = value;
                InternalSetColor(value);

                // NOTE: We never remove pens from the active list, so if someone is
                // changing their pen colors a lot, this could be a problem.
                if (value.IsSystemColor && !oldColor.IsSystemColor)
                {
                    SystemColorTracker.Add(this);
                }
            }
        }
    }

    /// <summary>
    ///  Gets or sets the <see cref='Drawing.Brush'/> that determines attributes of this <see cref='Pen'/>.
    /// </summary>
    public Brush Brush
    {
        get
        {
            Brush? brush = null;

            switch (PenType)
            {
                case Drawing2D.PenType.SolidColor:
                    brush = new SolidBrush((GpSolidFill*)GetNativeBrush());
                    break;

                case Drawing2D.PenType.HatchFill:
                    brush = new HatchBrush((GpHatch*)GetNativeBrush());
                    break;

                case Drawing2D.PenType.TextureFill:
                    brush = new TextureBrush((GpTexture*)GetNativeBrush());
                    break;

                case Drawing2D.PenType.PathGradient:
                    brush = new PathGradientBrush((GpPathGradient*)GetNativeBrush());
                    break;

                case Drawing2D.PenType.LinearGradient:
                    brush = new LinearGradientBrush((GpLineGradient*)GetNativeBrush());
                    break;

                default:
                    break;
            }

            return brush!;
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            ArgumentNullException.ThrowIfNull(value);
            PInvokeGdiPlus.GdipSetPenBrushFill(NativePen, value.Pointer()).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    private GpBrush* GetNativeBrush()
    {
        GpBrush* nativeBrush;
        PInvokeGdiPlus.GdipGetPenBrushFill(NativePen, &nativeBrush).ThrowIfFailed();
        GC.KeepAlive(this);
        return nativeBrush;
    }

    /// <summary>
    ///  Gets or sets the style used for dashed lines drawn with this <see cref='Pen'/>.
    /// </summary>
    public DashStyle DashStyle
    {
        get
        {
            DashStyle dashStyle;
            PInvokeGdiPlus.GdipGetPenDashStyle(NativePen, (GdiPlus.DashStyle*)&dashStyle).ThrowIfFailed();
            GC.KeepAlive(this);
            return dashStyle;
        }
        set
        {
            if (value is < DashStyle.Solid or > DashStyle.Custom)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DashStyle));
            }

            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenDashStyle(NativePen, (GdiPlus.DashStyle)value).ThrowIfFailed();
            GC.KeepAlive(this);

            // If we just set the pen style to Custom without defining the custom dash pattern,
            // make sure that we can return a valid value.
            if (value == DashStyle.Custom)
            {
                EnsureValidDashPattern();
            }

            if (value != DashStyle.Solid)
            {
                _dashStyleWasOrIsNotSolid = true;
            }
        }
    }

    /// <summary>
    ///  This method is called after the user sets the pen's dash style to custom. Here, we make sure that there
    ///  is a default value set for the custom pattern.
    /// </summary>
    private void EnsureValidDashPattern()
    {
        int count;
        PInvokeGdiPlus.GdipGetPenDashCount(NativePen, &count);
        GC.KeepAlive(this);

        if (count == 0)
        {
            // Set to a solid pattern.
            DashPattern = [1];
        }
    }

    /// <summary>
    ///  Gets or sets the distance from the start of a line to the beginning of a dash pattern.
    /// </summary>
    public float DashOffset
    {
        get
        {
            float dashOffset;
            PInvokeGdiPlus.GdipGetPenDashOffset(NativePen, &dashOffset).ThrowIfFailed();
            GC.KeepAlive(this);
            return dashOffset;
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            PInvokeGdiPlus.GdipSetPenDashOffset(NativePen, value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    /// <summary>
    ///  Gets or sets an array of custom dashes and spaces. The dashes are made up of line segments.
    /// </summary>
    public float[] DashPattern
    {
        get
        {
            int count;
            PInvokeGdiPlus.GdipGetPenDashCount(NativePen, &count).ThrowIfFailed();
            GC.KeepAlive(this);

            float[] pattern;

            // Don't call GdipGetPenDashArray with a 0 count
            if (count > 0)
            {
                pattern = new float[count];
                fixed (float* p = pattern)
                {
                    PInvokeGdiPlus.GdipGetPenDashArray(NativePen, p, count).ThrowIfFailed();
                }
            }
            else if (DashStyle == DashStyle.Solid && !_dashStyleWasOrIsNotSolid)
            {
                // Most likely we're replicating an existing System.Drawing bug here, it doesn't make much sense to
                // ask for a dash pattern when using a solid dash.
                throw new InvalidOperationException();
            }
            else if (DashStyle == DashStyle.Solid)
            {
                pattern = [];
            }
            else
            {
                // Special case (not handled inside GDI+)
                pattern = [1.0f];
            }

            GC.KeepAlive(this);
            return pattern;
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            if (value is null || value.Length == 0)
            {
                throw new ArgumentException(SR.InvalidDashPattern);
            }

            fixed (float* f = value)
            {
                PInvokeGdiPlus.GdipSetPenDashArray(NativePen, f, value.Length).ThrowIfFailed();
                GC.KeepAlive(this);
            }
        }
    }

    /// <summary>
    ///  Gets or sets an array of custom dashes and spaces. The dashes are made up of line segments.
    /// </summary>
    public float[] CompoundArray
    {
        get
        {
            int count;
            PInvokeGdiPlus.GdipGetPenCompoundCount(NativePen, &count).ThrowIfFailed();

            if (count == 0)
            {
                return [];
            }

            float[] array = new float[count];
            fixed (float* f = array)
            {
                PInvokeGdiPlus.GdipGetPenCompoundArray(NativePen, f, count).ThrowIfFailed();
                GC.KeepAlive(this);
                return array;
            }
        }
        set
        {
            if (_immutable)
            {
                throw new ArgumentException(SR.Format(SR.CantChangeImmutableObjects, nameof(Pen)));
            }

            ArgumentNullException.ThrowIfNull(value);
            fixed (float* f = value)
            {
                PInvokeGdiPlus.GdipSetPenCompoundArray(NativePen, f, value.Length).ThrowIfFailed();
                GC.KeepAlive(this);
            }
        }
    }

    void ISystemColorTracker.OnSystemColorChanged()
    {
        if (NativePen is not null)
        {
            InternalSetColor(_color);
        }
    }
}
