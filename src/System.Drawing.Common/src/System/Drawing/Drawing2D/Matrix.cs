// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Numerics;

namespace System.Drawing.Drawing2D;

public sealed unsafe class Matrix : MarshalByRefObject, IDisposable
{
    internal GdiPlus.Matrix* NativeMatrix { get; private set; }

    public Matrix()
    {
        GdiPlus.Matrix* matrix;
        PInvokeGdiPlus.GdipCreateMatrix(&matrix).ThrowIfFailed();
        NativeMatrix = matrix;
    }

    public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
    {
        GdiPlus.Matrix* matrix;
        PInvokeGdiPlus.GdipCreateMatrix2(m11, m12, m21, m22, dx, dy, &matrix).ThrowIfFailed();
        NativeMatrix = matrix;
    }

    /// <summary>
    ///  Construct a <see cref="Matrix"/> utilizing the given <paramref name="matrix"/>.
    /// </summary>
    /// <param name="matrix">Matrix data to construct from.</param>
    public Matrix(Matrix3x2 matrix) : this(CreateNativeHandle(matrix))
    {
    }

    private Matrix(GdiPlus.Matrix* nativeMatrix) => NativeMatrix = nativeMatrix;

    internal static GdiPlus.Matrix* CreateNativeHandle(Matrix3x2 matrix)
    {
        GdiPlus.Matrix* nativeMatrix;
        PInvokeGdiPlus.GdipCreateMatrix2(
            matrix.M11,
            matrix.M12,
            matrix.M21,
            matrix.M22,
            matrix.M31,
            matrix.M32,
            &nativeMatrix).ThrowIfFailed();

        return nativeMatrix;
    }

    public Matrix(RectangleF rect, params PointF[] plgpts)
    {
        ArgumentNullException.ThrowIfNull(plgpts);
        if (plgpts.Length != 3)
            throw Status.InvalidParameter.GetException();

        fixed (PointF* p = plgpts)
        {
            GdiPlus.Matrix* matrix;
            PInvokeGdiPlus.GdipCreateMatrix3((RectF*)&rect, (GdiPlus.PointF*)p, &matrix).ThrowIfFailed();
            NativeMatrix = matrix;
        }
    }

    public Matrix(Rectangle rect, params Point[] plgpts)
    {
        ArgumentNullException.ThrowIfNull(plgpts);
        if (plgpts.Length != 3)
            throw Status.InvalidParameter.GetException();

        fixed (Point* p = plgpts)
        {
            GdiPlus.Matrix* matrix;
            PInvokeGdiPlus.GdipCreateMatrix3I((Rect*)&rect, (GdiPlus.Point*)p, &matrix).ThrowIfFailed();
            NativeMatrix = matrix;
        }
    }

    public void Dispose()
    {
        DisposeInternal();
        GC.SuppressFinalize(this);
    }

    private void DisposeInternal()
    {
        if (NativeMatrix is not null)
        {
            if (Gdip.Initialized)
            {
                PInvokeGdiPlus.GdipDeleteMatrix(NativeMatrix);
            }

            NativeMatrix = null;
        }
    }

    ~Matrix() => DisposeInternal();

    public Matrix Clone()
    {
        GdiPlus.Matrix* matrix;
        PInvokeGdiPlus.GdipCloneMatrix(NativeMatrix, &matrix).ThrowIfFailed();
        GC.KeepAlive(this);
        return new Matrix(matrix);
    }

    public float[] Elements
    {
        get
        {
            float[] elements = new float[6];
            GetElements(elements);
            return elements;
        }
    }

    /// <summary>
    ///  Gets/sets the elements for the matrix.
    /// </summary>
    public Matrix3x2 MatrixElements
    {
        get
        {
            Matrix3x2 matrix = default;
            PInvokeGdiPlus.GdipGetMatrixElements(NativeMatrix, (float*)&matrix).ThrowIfFailed();
            GC.KeepAlive(this);
            return matrix;
        }
        set
        {
            PInvokeGdiPlus.GdipSetMatrixElements(
                NativeMatrix,
                value.M11,
                value.M12,
                value.M21,
                value.M22,
                value.M31,
                value.M32).ThrowIfFailed();

            GC.KeepAlive(this);
        }
    }

    internal void GetElements(Span<float> elements)
    {
        Debug.Assert(elements.Length >= 6);

        fixed (float* m = elements)
        {
            PInvokeGdiPlus.GdipGetMatrixElements(NativeMatrix, m).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public float OffsetX => Offset.X;

    public float OffsetY => Offset.Y;

    internal PointF Offset
    {
        get
        {
            Span<float> elements = stackalloc float[6];
            GetElements(elements);
            return new PointF(elements[4], elements[5]);
        }
    }

    public void Reset()
    {
        PInvokeGdiPlus.GdipSetMatrixElements(
            NativeMatrix,
            1.0f, 0.0f, 0.0f,
            1.0f, 0.0f, 0.0f).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void Multiply(Matrix matrix) => Multiply(matrix, MatrixOrder.Prepend);

    public void Multiply(Matrix matrix, MatrixOrder order)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        if (matrix.NativeMatrix == NativeMatrix)
            throw new InvalidOperationException(SR.GdiplusObjectBusy);

        PInvokeGdiPlus.GdipMultiplyMatrix(NativeMatrix, matrix.NativeMatrix, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
        GC.KeepAlive(matrix);
    }

    public void Translate(float offsetX, float offsetY) => Translate(offsetX, offsetY, MatrixOrder.Prepend);

    public void Translate(float offsetX, float offsetY, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipTranslateMatrix(NativeMatrix, offsetX, offsetY, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void Scale(float scaleX, float scaleY) => Scale(scaleX, scaleY, MatrixOrder.Prepend);

    public void Scale(float scaleX, float scaleY, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipScaleMatrix(NativeMatrix, scaleX, scaleY, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void Rotate(float angle) => Rotate(angle, MatrixOrder.Prepend);

    public void Rotate(float angle, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipRotateMatrix(NativeMatrix, angle, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void RotateAt(float angle, PointF point) => RotateAt(angle, point, MatrixOrder.Prepend);
    public void RotateAt(float angle, PointF point, MatrixOrder order)
    {
        Status status;
        if (order == MatrixOrder.Prepend)
        {
            status = PInvokeGdiPlus.GdipTranslateMatrix(NativeMatrix, point.X, point.Y, (GdiPlus.MatrixOrder)order);
            status |= PInvokeGdiPlus.GdipRotateMatrix(NativeMatrix, angle, (GdiPlus.MatrixOrder)order);
            status |= PInvokeGdiPlus.GdipTranslateMatrix(NativeMatrix, -point.X, -point.Y, (GdiPlus.MatrixOrder)order);
        }
        else
        {
            status = PInvokeGdiPlus.GdipTranslateMatrix(NativeMatrix, -point.X, -point.Y, (GdiPlus.MatrixOrder)order);
            status |= PInvokeGdiPlus.GdipRotateMatrix(NativeMatrix, angle, (GdiPlus.MatrixOrder)order);
            status |= PInvokeGdiPlus.GdipTranslateMatrix(NativeMatrix, point.X, point.Y, (GdiPlus.MatrixOrder)order);
        }

        status.ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void Shear(float shearX, float shearY)
    {
        PInvokeGdiPlus.GdipShearMatrix(NativeMatrix, shearX, shearY, GdiPlus.MatrixOrder.MatrixOrderPrepend).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void Shear(float shearX, float shearY, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipShearMatrix(NativeMatrix, shearX, shearY, (GdiPlus.MatrixOrder)order).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void Invert()
    {
        PInvokeGdiPlus.GdipInvertMatrix(NativeMatrix).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    /// <inheritdoc cref="TransformPoints(Point[])"/>
    public void TransformPoints(params PointF[] pts)
    {
        ArgumentNullException.ThrowIfNull(pts);
        TransformPoints(pts.AsSpan());
    }

    /// <inheritdoc cref="TransformPoints(Point[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void TransformPoints(params ReadOnlySpan<PointF> pts)
    {
        fixed (PointF* p = pts)
        {
            PInvokeGdiPlus.GdipTransformMatrixPoints(
                NativeMatrix,
                (GdiPlus.PointF*)p,
                pts.Length).ThrowIfFailed();
        }

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Applies the geometric transform this <see cref="Matrix"/> represents to an array of points.
    /// </summary>
    /// <param name="pts">The points to transform.</param>
    public void TransformPoints(params Point[] pts)
    {
        ArgumentNullException.ThrowIfNull(pts);
        TransformPoints(pts.AsSpan());
    }

    /// <inheritdoc cref="TransformPoints(Point[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void TransformPoints(params ReadOnlySpan<Point> pts)
    {
        fixed (Point* p = pts)
        {
            PInvokeGdiPlus.GdipTransformMatrixPointsI(
                NativeMatrix,
                (GdiPlus.Point*)p,
                pts.Length).ThrowIfFailed();
        }

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Multiplies each vector in an array by the matrix. The translation elements of this matrix (third row) are ignored.
    /// </summary>
    /// <param name="pts">The points to transform.</param>
    public void TransformVectors(params PointF[] pts)
    {
        ArgumentNullException.ThrowIfNull(pts);
        TransformVectors(pts.AsSpan());
    }

    /// <inheritdoc cref="TransformVectors(PointF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void TransformVectors(params ReadOnlySpan<PointF> pts)
    {
        fixed (PointF* p = pts)
        {
            PInvokeGdiPlus.GdipVectorTransformMatrixPoints(
                NativeMatrix,
                (GdiPlus.PointF*)p,
                pts.Length).ThrowIfFailed();
        }

        GC.KeepAlive(this);
    }

    /// <inheritdoc cref="TransformVectors(PointF[])"/>
    public void VectorTransformPoints(params Point[] pts) => TransformVectors(pts);

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="TransformVectors(PointF[])"/>
    public void VectorTransformPoints(params ReadOnlySpan<Point> pts) => TransformVectors(pts);
#endif

    /// <inheritdoc cref="TransformVectors(PointF[])"/>
    public void TransformVectors(params Point[] pts)
    {
        ArgumentNullException.ThrowIfNull(pts);
        TransformVectors(pts.AsSpan());
    }

    /// <inheritdoc cref="TransformVectors(PointF[])"/>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void TransformVectors(params ReadOnlySpan<Point> pts)
    {
        fixed (Point* p = pts)
        {
            PInvokeGdiPlus.GdipVectorTransformMatrixPointsI(
                NativeMatrix,
                (GdiPlus.Point*)p,
                pts.Length).ThrowIfFailed();
        }

        GC.KeepAlive(this);
    }

    public bool IsInvertible
    {
        get
        {
            BOOL invertible;
            PInvokeGdiPlus.GdipIsMatrixInvertible(NativeMatrix, &invertible).ThrowIfFailed();
            GC.KeepAlive(this);
            return invertible;
        }
    }

    public bool IsIdentity
    {
        get
        {
            BOOL identity;
            PInvokeGdiPlus.GdipIsMatrixIdentity(NativeMatrix, &identity).ThrowIfFailed();
            GC.KeepAlive(this);
            return identity;
        }
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not Matrix matrix2)
            return false;

        BOOL equal;
        PInvokeGdiPlus.GdipIsMatrixEqual(
            NativeMatrix,
            matrix2.NativeMatrix,
            &equal).ThrowIfFailed();

        GC.KeepAlive(this);
        GC.KeepAlive(matrix2);
        return equal;
    }

    public override int GetHashCode() => base.GetHashCode();
}
