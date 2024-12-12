// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Imaging;

namespace System.Drawing;

public sealed unsafe class TextureBrush : Brush
{
    // When creating a texture brush from a metafile image, the dstRect
    // is used to specify the size that the metafile image should be
    // rendered at in the device units of the destination graphics.
    // It is NOT used to crop the metafile image, so only the width
    // and height values matter for metafiles.

    public TextureBrush(Image bitmap) : this(bitmap, Drawing2D.WrapMode.Tile)
    {
    }

    public TextureBrush(Image image, Drawing2D.WrapMode wrapMode)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (wrapMode is < Drawing2D.WrapMode.Tile or > Drawing2D.WrapMode.Clamp)
        {
            throw new InvalidEnumArgumentException(nameof(wrapMode), (int)wrapMode, typeof(Drawing2D.WrapMode));
        }

        GpTexture* brush;
        PInvokeGdiPlus.GdipCreateTexture(image.Pointer(), (WrapMode)wrapMode, &brush).ThrowIfFailed();
        GC.KeepAlive(image);
        SetNativeBrushInternal((GpBrush*)brush);
    }

    public TextureBrush(Image image, Drawing2D.WrapMode wrapMode, RectangleF dstRect)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (wrapMode is < Drawing2D.WrapMode.Tile or > Drawing2D.WrapMode.Clamp)
        {
            throw new InvalidEnumArgumentException(nameof(wrapMode), (int)wrapMode, typeof(Drawing2D.WrapMode));
        }

        GpTexture* brush;
        PInvokeGdiPlus.GdipCreateTexture2(
            image.Pointer(),
            (WrapMode)wrapMode,
            dstRect.X, dstRect.Y, dstRect.Width, dstRect.Height, &brush).ThrowIfFailed();

        GC.KeepAlive(image);
        SetNativeBrushInternal((GpBrush*)brush);
    }

    public TextureBrush(Image image, Drawing2D.WrapMode wrapMode, Rectangle dstRect)
        : this(image, wrapMode, (RectangleF)dstRect)
    {
    }

    public TextureBrush(Image image, RectangleF dstRect) : this(image, dstRect, null) { }

    public TextureBrush(Image image, RectangleF dstRect, ImageAttributes? imageAttr)
    {
        ArgumentNullException.ThrowIfNull(image);

        GpTexture* brush;
        PInvokeGdiPlus.GdipCreateTextureIA(
            image.Pointer(),
            imageAttr is null ? null : imageAttr._nativeImageAttributes,
            dstRect.X,
            dstRect.Y,
            dstRect.Width,
            dstRect.Height,
            &brush).ThrowIfFailed();

        SetNativeBrushInternal((GpBrush*)brush);
        GC.KeepAlive(image);
        GC.KeepAlive(imageAttr);
    }

    public TextureBrush(Image image, Rectangle dstRect) : this(image, dstRect, null) { }

    public TextureBrush(Image image, Rectangle dstRect, ImageAttributes? imageAttr)
        : this(image, (RectangleF)dstRect, imageAttr)
    {
    }

    internal TextureBrush(GpTexture* nativeBrush)
    {
        Debug.Assert(nativeBrush is not null, "Initializing native brush with null.");
        SetNativeBrushInternal((GpBrush*)nativeBrush);
    }

    public override object Clone()
    {
        GpBrush* cloneBrush;
        PInvokeGdiPlus.GdipCloneBrush(NativeBrush, &cloneBrush).ThrowIfFailed();
        GC.KeepAlive(this);

        return new TextureBrush((GpTexture*)cloneBrush);
    }

    public Matrix Transform
    {
        get
        {
            Matrix matrix = new();
            PInvokeGdiPlus.GdipGetTextureTransform((GpTexture*)NativeBrush, matrix.NativeMatrix).ThrowIfFailed();
            GC.KeepAlive(this);
            return matrix;
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            PInvokeGdiPlus.GdipSetTextureTransform((GpTexture*)NativeBrush, value.NativeMatrix).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public Drawing2D.WrapMode WrapMode
    {
        get
        {
            WrapMode mode;
            PInvokeGdiPlus.GdipGetTextureWrapMode((GpTexture*)NativeBrush, &mode).ThrowIfFailed();
            GC.KeepAlive(this);
            return (Drawing2D.WrapMode)mode;
        }
        set
        {
            if (value is < Drawing2D.WrapMode.Tile or > Drawing2D.WrapMode.Clamp)
            {
                throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Drawing2D.WrapMode));
            }

            PInvokeGdiPlus.GdipSetTextureWrapMode((GpTexture*)NativeBrush, (WrapMode)value).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public Image Image
    {
        get
        {
            GpImage* image;
            PInvokeGdiPlus.GdipGetTextureImage((GpTexture*)NativeBrush, &image).ThrowIfFailed();
            GC.KeepAlive(this);
            return Image.CreateImageObject(image);
        }
    }

    public void ResetTransform()
    {
        PInvokeGdiPlus.GdipResetTextureTransform((GpTexture*)NativeBrush).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void MultiplyTransform(Matrix matrix) => MultiplyTransform(matrix, MatrixOrder.Prepend);

    public void MultiplyTransform(Matrix matrix, MatrixOrder order)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        if (matrix.NativeMatrix is null)
        {
            return;
        }

        PInvokeGdiPlus.GdipMultiplyTextureTransform(
            (GpTexture*)NativeBrush,
            matrix.NativeMatrix,
            (GdiPlus.MatrixOrder)order).ThrowIfFailed();

        GC.KeepAlive(this);
        GC.KeepAlive(matrix);
    }

    public void TranslateTransform(float dx, float dy) => TranslateTransform(dx, dy, MatrixOrder.Prepend);

    public void TranslateTransform(float dx, float dy, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipTranslateTextureTransform(
            (GpTexture*)NativeBrush,
            dx, dy,
            (GdiPlus.MatrixOrder)order).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void ScaleTransform(float sx, float sy) => ScaleTransform(sx, sy, MatrixOrder.Prepend);

    public void ScaleTransform(float sx, float sy, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipScaleTextureTransform(
            (GpTexture*)NativeBrush,
            sx, sy,
            (GdiPlus.MatrixOrder)order).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void RotateTransform(float angle) => RotateTransform(angle, MatrixOrder.Prepend);

    public void RotateTransform(float angle, MatrixOrder order)
    {
        PInvokeGdiPlus.GdipRotateTextureTransform(
            (GpTexture*)NativeBrush,
            angle,
            (GdiPlus.MatrixOrder)order).ThrowIfFailed();

        GC.KeepAlive(this);
    }
}
