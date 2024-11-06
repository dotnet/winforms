// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace System.Drawing.Tests;

public class TextureBrushTests
{
    public static IEnumerable<object[]> Ctor_Bitmap_TestData()
    {
        yield return new object[] { new Bitmap(10, 10), PixelFormat.Format32bppPArgb, new Size(10, 10) };
        yield return new object[] { new Metafile(Helpers.GetTestBitmapPath("telescope_01.wmf")), PixelFormat.Format32bppArgb, new Size(490, 654) };
    }

    [Theory]
    [MemberData(nameof(Ctor_Bitmap_TestData))]
    public void Ctor_Bitmap(Image bitmap, PixelFormat expectedPixelFormat, Size expectedSize)
    {
        try
        {
            using TextureBrush brush = new(bitmap);
            using Matrix matrix = new();
            Bitmap brushImage = Assert.IsType<Bitmap>(brush.Image);
            Assert.NotSame(bitmap, brushImage);
            Assert.Equal(expectedPixelFormat, brushImage.PixelFormat);
            Assert.Equal(expectedSize, brushImage.Size);
            Assert.Equal(matrix, brush.Transform);
            Assert.Equal(WrapMode.Tile, brush.WrapMode);
        }
        finally
        {
            bitmap.Dispose();
        }
    }

    [Fact]
    public void Ctor_BitmapFromIconHandle_Success()
    {
        using Icon icon = new(Helpers.GetTestBitmapPath("10x16_one_entry_32bit.ico"));
        using var image = Bitmap.FromHicon(icon.Handle);
        Ctor_Bitmap(image, PixelFormat.Format32bppPArgb, new Size(11, 22));
    }

    public static IEnumerable<object[]> Ctor_Image_WrapMode_TestData()
    {
        foreach (object[] data in Ctor_Bitmap_TestData())
        {
            yield return new object[] { ((Image)data[0]).Clone(), WrapMode.Clamp, data[1], data[2] };
            yield return new object[] { ((Image)data[0]).Clone(), WrapMode.Tile, data[1], data[2] };
            yield return new object[] { ((Image)data[0]).Clone(), WrapMode.TileFlipX, data[1], data[2] };
            yield return new object[] { ((Image)data[0]).Clone(), WrapMode.TileFlipXY, data[1], data[2] };
            yield return new object[] { ((Image)data[0]).Clone(), WrapMode.TileFlipY, data[1], data[2] };
        }
    }

    [Theory]
    [MemberData(nameof(Ctor_Image_WrapMode_TestData))]
    public void Ctor_Image_WrapMode(Image image, WrapMode wrapMode, PixelFormat expectedPixelFormat, Size expectedSize)
    {
        try
        {
            using TextureBrush brush = new(image, wrapMode);
            using Matrix matrix = new();
            Bitmap brushImage = Assert.IsType<Bitmap>(brush.Image);
            Assert.NotSame(image, brushImage);
            Assert.Equal(expectedPixelFormat, brushImage.PixelFormat);
            Assert.Equal(expectedSize, brushImage.Size);
            Assert.Equal(matrix, brush.Transform);
            Assert.Equal(wrapMode, brush.WrapMode);
        }
        finally
        {
            image.Dispose();
        }
    }

    public static IEnumerable<object[]> Ctor_Image_Rectangle_TestData()
    {
        yield return new object[] { new Bitmap(10, 10), new Rectangle(0, 0, 10, 10) };
        yield return new object[] { new Bitmap(10, 10), new Rectangle(5, 5, 5, 5) };
    }

    [Theory]
    [MemberData(nameof(Ctor_Image_Rectangle_TestData))]
    public void Ctor_Image_Rectangle(Image image, Rectangle rectangle)
    {
        try
        {
            using TextureBrush brush = new(image, rectangle);
            using Matrix matrix = new();
            Bitmap brushImage = Assert.IsType<Bitmap>(brush.Image);
            Assert.NotSame(image, brushImage);
            Assert.Equal(PixelFormat.Format32bppPArgb, brushImage.PixelFormat);
            Assert.Equal(rectangle.Size, brushImage.Size);
            Assert.Equal(matrix, brush.Transform);
            Assert.Equal(WrapMode.Tile, brush.WrapMode);
        }
        finally
        {
            image.Dispose();
        }
    }

    [Theory]
    [MemberData(nameof(Ctor_Image_Rectangle_TestData))]
    public void Ctor_Image_RectangleF(Image image, Rectangle rectangle)
    {
        try
        {
            using TextureBrush brush = new(image, (RectangleF)rectangle);
            using Matrix matrix = new();
            Bitmap brushImage = Assert.IsType<Bitmap>(brush.Image);
            Assert.NotSame(image, brushImage);
            Assert.Equal(PixelFormat.Format32bppPArgb, brushImage.PixelFormat);
            Assert.Equal(rectangle.Size, brushImage.Size);
            Assert.Equal(matrix, brush.Transform);
            Assert.Equal(WrapMode.Tile, brush.WrapMode);
        }
        finally
        {
            image.Dispose();
        }
    }

    public static IEnumerable<object[]> Ctor_Image_WrapMode_Rectangle_TestData()
    {
        foreach (object[] data in Ctor_Image_Rectangle_TestData())
        {
            yield return new object[] { ((Image)data[0]).Clone(), WrapMode.Clamp, data[1] };
            yield return new object[] { ((Image)data[0]).Clone(), WrapMode.Tile, data[1] };
            yield return new object[] { ((Image)data[0]).Clone(), WrapMode.TileFlipX, data[1] };
            yield return new object[] { ((Image)data[0]).Clone(), WrapMode.TileFlipXY, data[1] };
            yield return new object[] { ((Image)data[0]).Clone(), WrapMode.TileFlipY, data[1] };
        }
    }

    [Theory]
    [MemberData(nameof(Ctor_Image_WrapMode_Rectangle_TestData))]
    public void Ctor_Image_WrapMode_Rectangle(Image image, WrapMode wrapMode, Rectangle rectangle)
    {
        try
        {
            using TextureBrush brush = new(image, wrapMode, rectangle);
            using Matrix matrix = new();
            Bitmap brushImage = Assert.IsType<Bitmap>(brush.Image);
            Assert.NotSame(image, brushImage);
            Assert.Equal(PixelFormat.Format32bppPArgb, brushImage.PixelFormat);
            Assert.Equal(rectangle.Size, brushImage.Size);
            Assert.Equal(matrix, brush.Transform);
            Assert.Equal(wrapMode, brush.WrapMode);
        }
        finally
        {
            image.Dispose();
        }
    }

    [Theory]
    [MemberData(nameof(Ctor_Image_WrapMode_Rectangle_TestData))]
    public void Ctor_Image_WrapMode_RectangleF(Image image, WrapMode wrapMode, Rectangle rectangle)
    {
        try
        {
            using TextureBrush brush = new(image, wrapMode, (RectangleF)rectangle);
            using Matrix matrix = new();
            Bitmap brushImage = Assert.IsType<Bitmap>(brush.Image);
            Assert.NotSame(image, brushImage);
            Assert.Equal(PixelFormat.Format32bppPArgb, brushImage.PixelFormat);
            Assert.Equal(rectangle.Size, brushImage.Size);
            Assert.Equal(matrix, brush.Transform);
            Assert.Equal(wrapMode, brush.WrapMode);
        }
        finally
        {
            image.Dispose();
        }
    }

    public static IEnumerable<object[]> Ctor_Image_Rectangle_ImageAttributes_TestData()
    {
        foreach (object[] data in Ctor_Image_Rectangle_TestData())
        {
            yield return new object[] { ((Image)data[0]).Clone(), data[1], null, WrapMode.Tile };
            yield return new object[] { ((Image)data[0]).Clone(), data[1], new ImageAttributes(), WrapMode.Clamp };

            ImageAttributes customWrapMode = new();
            customWrapMode.SetWrapMode(WrapMode.TileFlipXY);
            yield return new object[] { ((Image)data[0]).Clone(), data[1], customWrapMode, WrapMode.TileFlipXY };
        }
    }

    [Theory]
    [MemberData(nameof(Ctor_Image_Rectangle_ImageAttributes_TestData))]
    public void Ctor_Image_Rectangle_ImageAttributes(Image image, Rectangle rectangle, ImageAttributes attributes, WrapMode expectedWrapMode)
    {
        try
        {
            using TextureBrush brush = new(image, rectangle, attributes);
            using Matrix matrix = new();
            Bitmap brushImage = Assert.IsType<Bitmap>(brush.Image);
            Assert.NotSame(image, brushImage);
            Assert.Equal(PixelFormat.Format32bppPArgb, brushImage.PixelFormat);
            Assert.Equal(rectangle.Size, brushImage.Size);
            Assert.Equal(matrix, brush.Transform);
            Assert.Equal(expectedWrapMode, brush.WrapMode);
        }
        finally
        {
            image.Dispose();
            attributes?.Dispose();
        }
    }

    [Theory]
    [MemberData(nameof(Ctor_Image_Rectangle_ImageAttributes_TestData))]
    public void Ctor_Image_RectangleF_ImageAttributes(Image image, Rectangle rectangle, ImageAttributes attributes, WrapMode expectedWrapMode)
    {
        try
        {
            using TextureBrush brush = new(image, (RectangleF)rectangle, attributes);
            using Matrix matrix = new();
            Bitmap brushImage = Assert.IsType<Bitmap>(brush.Image);
            Assert.NotSame(image, brushImage);
            Assert.Equal(PixelFormat.Format32bppPArgb, brushImage.PixelFormat);
            Assert.Equal(rectangle.Size, brushImage.Size);
            Assert.Equal(matrix, brush.Transform);
            Assert.Equal(expectedWrapMode, brush.WrapMode);
        }
        finally
        {
            image.Dispose();
            attributes?.Dispose();
        }
    }

    [Fact]
    public void Ctor_NullImage_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("image", () => new TextureBrush((Image)null));
        AssertExtensions.Throws<ArgumentNullException>("image", () => new TextureBrush(null, WrapMode.Tile));
        AssertExtensions.Throws<ArgumentNullException>("image", () => new TextureBrush(null, RectangleF.Empty));
        AssertExtensions.Throws<ArgumentNullException>("image", () => new TextureBrush(null, Rectangle.Empty));
        AssertExtensions.Throws<ArgumentNullException>("image", () => new TextureBrush(null, RectangleF.Empty, null));
        AssertExtensions.Throws<ArgumentNullException>("image", () => new TextureBrush(null, Rectangle.Empty, null));
        AssertExtensions.Throws<ArgumentNullException>("image", () => new TextureBrush(null, WrapMode.Tile, RectangleF.Empty));
        AssertExtensions.Throws<ArgumentNullException>("image", () => new TextureBrush(null, WrapMode.Tile, Rectangle.Empty));
    }

    [Fact]
    public void Ctor_DisposedImage_ThrowsArgumentException()
    {
        Bitmap image = new(10, 10);
        image.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => new TextureBrush(image));
        AssertExtensions.Throws<ArgumentException>(null, () => new TextureBrush(image, WrapMode.Tile));
        AssertExtensions.Throws<ArgumentException>(null, () => new TextureBrush(image, RectangleF.Empty));
        AssertExtensions.Throws<ArgumentException>(null, () => new TextureBrush(image, Rectangle.Empty));
        AssertExtensions.Throws<ArgumentException>(null, () => new TextureBrush(image, RectangleF.Empty, null));
        AssertExtensions.Throws<ArgumentException>(null, () => new TextureBrush(image, Rectangle.Empty, null));
        AssertExtensions.Throws<ArgumentException>(null, () => new TextureBrush(image, WrapMode.Tile, RectangleF.Empty));
        AssertExtensions.Throws<ArgumentException>(null, () => new TextureBrush(image, WrapMode.Tile, Rectangle.Empty));
    }

    [Theory]
    [InlineData(WrapMode.Tile - 1)]
    [InlineData(WrapMode.Clamp + 1)]
    public void Ctor_InvalidWrapMode_ThrowsInvalidEnumArgumentException(WrapMode wrapMode)
    {
        using Bitmap image = new(10, 10);
        Assert.ThrowsAny<ArgumentException>(() => new TextureBrush(image, wrapMode));
        Assert.ThrowsAny<ArgumentException>(() => new TextureBrush(image, wrapMode, RectangleF.Empty));
        Assert.ThrowsAny<ArgumentException>(() => new TextureBrush(image, wrapMode, Rectangle.Empty));
    }

    [Theory]
    [InlineData(-1, 0, 1, 1)]
    [InlineData(10, 0, 1, 1)]
    [InlineData(5, 0, 6, 1)]
    [InlineData(0, -1, 1, 1)]
    [InlineData(0, 10, 1, 1)]
    [InlineData(0, 5, 1, 6)]
    [InlineData(0, 0, 1, 0)]
    [InlineData(0, 0, 0, 1)]
    public void Ctor_InvalidRectangle_ThrowsOutOfMemoryException(int x, int y, int width, int height)
    {
        Rectangle rectangle = new(x, y, width, height);
        using Bitmap image = new(10, 10);
        Assert.Throws<OutOfMemoryException>(() => new TextureBrush(image, rectangle));
        Assert.Throws<OutOfMemoryException>(() => new TextureBrush(image, (RectangleF)rectangle));
        Assert.Throws<OutOfMemoryException>(() => new TextureBrush(image, WrapMode.Tile, rectangle));
        Assert.Throws<OutOfMemoryException>(() => new TextureBrush(image, WrapMode.Tile, (RectangleF)rectangle));
        Assert.Throws<OutOfMemoryException>(() => new TextureBrush(image, rectangle, null));
        Assert.Throws<OutOfMemoryException>(() => new TextureBrush(image, (RectangleF)rectangle, null));
    }

    [Fact]
    public void Clone_Invoke_Success()
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image, WrapMode.Clamp);
        TextureBrush clone = Assert.IsType<TextureBrush>(brush.Clone());
        Assert.NotSame(brush, clone);

        Assert.Equal(new Size(10, 10), brush.Image.Size);
        Assert.Equal(WrapMode.Clamp, clone.WrapMode);
    }

    [Fact]
    public void Clone_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        TextureBrush brush = new(image);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, brush.Clone);
    }

    [Fact]
    public void Image_GetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        TextureBrush brush = new(image);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.Image);
    }

    public static IEnumerable<object[]> MultiplyTransform_TestData()
    {
        yield return new object[] { new Matrix(), new Matrix(1, 2, 3, 4, 5, 6), MatrixOrder.Prepend };
        yield return new object[] { new Matrix(), new Matrix(1, 2, 3, 4, 5, 6), MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), new Matrix(2, 3, 4, 5, 6, 7), MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), new Matrix(2, 3, 4, 5, 6, 7), MatrixOrder.Append };
    }

    [Theory]
    [MemberData(nameof(MultiplyTransform_TestData))]
    public void MultiplyTransform_Matrix_SetsTransformToExpected(Matrix originalTransform, Matrix matrix, MatrixOrder matrixOrder)
    {
        try
        {
            using Bitmap image = new(10, 10);
            using TextureBrush brush = new(image);
            using var expected = originalTransform.Clone();
            expected.Multiply(matrix, matrixOrder);
            brush.Transform = originalTransform;

            if (matrixOrder == MatrixOrder.Prepend)
            {
                TextureBrush clone = (TextureBrush)brush.Clone();
                clone.MultiplyTransform(matrix);
                Assert.Equal(expected, clone.Transform);
            }

            brush.MultiplyTransform(matrix, matrixOrder);
            Assert.Equal(expected, brush.Transform);
        }
        finally
        {
            originalTransform.Dispose();
            matrix.Dispose();
        }
    }

    [Fact]
    public void MultiplyTransform_NullMatrix_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => brush.MultiplyTransform(null));
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => brush.MultiplyTransform(null, MatrixOrder.Prepend));
    }

    [Fact]
    public void MultiplyTransform_NotInvertibleMatrix_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        using Matrix matrix = new(123, 24, 82, 16, 47, 30);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(matrix));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(matrix, MatrixOrder.Prepend));
    }

    [Fact]
    public void MultiplyTransform_DisposedMatrix_Nop()
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        brush.Transform = transform;

        Matrix matrix = new();
        matrix.Dispose();

        brush.MultiplyTransform(matrix);
        brush.MultiplyTransform(matrix, MatrixOrder.Append);

        Assert.Equal(transform, brush.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void MultiplyTransform_InvalidOrder_Nop(MatrixOrder matrixOrder)
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        using Matrix matrix = new();
        brush.Transform = transform;

        brush.MultiplyTransform(matrix, matrixOrder);
        Assert.Equal(transform, brush.Transform);
    }

    [Fact]
    public void MultiplyTransform_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Matrix matrix = new();
        TextureBrush brush = new(image);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(matrix));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.MultiplyTransform(matrix, MatrixOrder.Prepend));
    }

    [Fact]
    public void ResetTransform_Invoke_SetsTransformToZero()
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        using Matrix matrix = new();
        brush.Transform = transform;
        brush.ResetTransform();
        Assert.Equal(matrix, brush.Transform);

        brush.ResetTransform();
        Assert.Equal(matrix, brush.Transform);
    }

    [Fact]
    public void ResetTransform_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        TextureBrush brush = new(image);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, brush.ResetTransform);
    }

    public static IEnumerable<object[]> RotateTransform_TestData()
    {
        yield return new object[] { new Matrix(), 90, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(), 90, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 0, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 0, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 360, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 360, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), -45, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), -45, MatrixOrder.Append };
    }

    [Theory]
    [MemberData(nameof(RotateTransform_TestData))]
    public void RotateTransform_Invoke_SetsTransformToExpected(Matrix originalTransform, float angle, MatrixOrder matrixOrder)
    {
        try
        {
            using Bitmap image = new(10, 10);
            using TextureBrush brush = new(image);
            using Matrix expected = originalTransform.Clone();
            expected.Rotate(angle, matrixOrder);
            brush.Transform = originalTransform;

            if (matrixOrder == MatrixOrder.Prepend)
            {
                TextureBrush clone = (TextureBrush)brush.Clone();
                clone.RotateTransform(angle);
                Assert.Equal(expected, clone.Transform);
            }

            brush.RotateTransform(angle, matrixOrder);
            Assert.Equal(expected, brush.Transform);
        }
        finally
        {
            originalTransform.Dispose();
        }
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void RotateTransform_InvalidOrder_ThrowsArgumentException(MatrixOrder matrixOrder)
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.RotateTransform(10, matrixOrder));
    }

    [Fact]
    public void RotateTransform_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Matrix matrix = new();
        TextureBrush brush = new(image);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.RotateTransform(1));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.RotateTransform(1, MatrixOrder.Prepend));
    }

    public static IEnumerable<object[]> ScaleTransform_TestData()
    {
        yield return new object[] { new Matrix(), 2, 3, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(), 2, 3, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 0, 0, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 0, 0, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 1, 1, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 1, 1, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), -2, -3, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), -2, -3, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 0.5, 0.75, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 0.5, 0.75, MatrixOrder.Append };
    }

    [Theory]
    [MemberData(nameof(ScaleTransform_TestData))]
    public void ScaleTransform_Invoke_SetsTransformToExpected(Matrix originalTransform, float scaleX, float scaleY, MatrixOrder matrixOrder)
    {
        try
        {
            using Bitmap image = new(10, 10);
            using TextureBrush brush = new(image);
            using Matrix expected = originalTransform.Clone();
            expected.Scale(scaleX, scaleY, matrixOrder);
            brush.Transform = originalTransform;

            if (matrixOrder == MatrixOrder.Prepend)
            {
                TextureBrush clone = (TextureBrush)brush.Clone();
                clone.ScaleTransform(scaleX, scaleY);
                Assert.Equal(expected, clone.Transform);
            }

            brush.ScaleTransform(scaleX, scaleY, matrixOrder);
            Assert.Equal(expected, brush.Transform);
        }
        finally
        {
            originalTransform.Dispose();
        }
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void ScaleTransform_InvalidOrder_ThrowsArgumentException(MatrixOrder matrixOrder)
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.ScaleTransform(1, 2, matrixOrder));
    }

    [Fact]
    public void ScaleTransform_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Matrix matrix = new();
        TextureBrush brush = new(image);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.ScaleTransform(1, 2));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.ScaleTransform(1, 2, MatrixOrder.Prepend));
    }

    [Fact]
    public void Transform_SetValid_GetReturnsExpected()
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        using Matrix matrix = new(1, 2, 3, 4, 5, 6);
        brush.Transform = matrix;
        Assert.Equal(matrix, brush.Transform);
    }

    [Fact]
    public void Transform_SetNull_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        AssertExtensions.Throws<ArgumentNullException>("value", () => brush.Transform = null);
    }

    [Fact]
    public void Transform_SetDisposedMatrix_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        Matrix matrix = new();
        matrix.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.Transform = matrix);
    }

    [Fact]
    public void Transform_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Matrix matrix = new();
        TextureBrush brush = new(image);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.Transform);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.Transform = matrix);
    }

    public static IEnumerable<object[]> TranslateTransform_TestData()
    {
        yield return new object[] { new Matrix(), 2, 3, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(), 2, 3, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 0, 0, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 0, 0, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 1, 1, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 1, 1, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), -2, -3, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), -2, -3, MatrixOrder.Append };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 0.5, 0.75, MatrixOrder.Prepend };
        yield return new object[] { new Matrix(1, 2, 3, 4, 5, 6), 0.5, 0.75, MatrixOrder.Append };
    }

    [Theory]
    [MemberData(nameof(TranslateTransform_TestData))]
    public void TranslateTransform_Invoke_SetsTransformToExpected(Matrix originalTransform, float dX, float dY, MatrixOrder matrixOrder)
    {
        try
        {
            using Bitmap image = new(10, 10);
            using TextureBrush brush = new(image);
            using Matrix expected = originalTransform.Clone();
            expected.Translate(dX, dY, matrixOrder);
            brush.Transform = originalTransform;

            if (matrixOrder == MatrixOrder.Prepend)
            {
                TextureBrush clone = (TextureBrush)brush.Clone();
                clone.TranslateTransform(dX, dY);
                Assert.Equal(expected, clone.Transform);
            }

            brush.TranslateTransform(dX, dY, matrixOrder);
            Assert.Equal(expected, brush.Transform);
        }
        finally
        {
            originalTransform.Dispose();
        }
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void TranslateTransform_InvalidOrder_ThrowsArgumentException(MatrixOrder matrixOrder)
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.TranslateTransform(1, 2, matrixOrder));
    }

    [Fact]
    public void TranslateTransform_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Matrix matrix = new();
        TextureBrush brush = new(image);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.TranslateTransform(1, 2));
        AssertExtensions.Throws<ArgumentException>(null, () => brush.TranslateTransform(1, 2, MatrixOrder.Prepend));
    }

    [Theory]
    [InlineData(WrapMode.Clamp)]
    [InlineData(WrapMode.Tile)]
    [InlineData(WrapMode.TileFlipX)]
    [InlineData(WrapMode.TileFlipXY)]
    [InlineData(WrapMode.TileFlipY)]
    public void WrapMode_SetValid_GetReturnsExpected(WrapMode wrapMode)
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        brush.WrapMode = wrapMode;
        Assert.Equal(wrapMode, brush.WrapMode);
    }

    [Theory]
    [InlineData(WrapMode.Tile - 1)]
    [InlineData(WrapMode.Clamp + 1)]
    public void WrapMode_SetInvalid_ThrowsInvalidEnumArgumentException(WrapMode wrapMode)
    {
        using Bitmap image = new(10, 10);
        using TextureBrush brush = new(image);
        Assert.ThrowsAny<ArgumentException>(() => brush.WrapMode = wrapMode);
    }

    [Fact]
    public void WrapMode_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        TextureBrush brush = new(image);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => brush.WrapMode);
        AssertExtensions.Throws<ArgumentException>(null, () => brush.WrapMode = WrapMode.Tile);
    }

    [Fact]
    public void WrapMode_Clamp_ReturnsExpected()
    {
        // R|G|_|_
        // B|Y|_|_
        // _|_|_|_
        // _|_|_|_
        Color empty = Color.FromArgb(0, 0, 0, 0);
        VerifyFillRect(WrapMode.Clamp,
        [
            [Color.Red,    Color.Green,    empty,  empty],
            [Color.Blue,   Color.Yellow,   empty,  empty],
            [empty,        empty,          empty,  empty],
            [empty,        empty,          empty,  empty]
        ]);
    }

    [Fact]
    public void WrapMode_Tile_ReturnsExpected()
    {
        // R|G|R|G
        // B|Y|B|Y
        // R|G|R|G
        // B|Y|B|Y
        VerifyFillRect(WrapMode.Tile,
        [
            [Color.Red,  Color.Green,  Color.Red,  Color.Green],
            [Color.Blue, Color.Yellow, Color.Blue, Color.Yellow],
            [Color.Red,  Color.Green,  Color.Red,  Color.Green],
            [Color.Blue, Color.Yellow, Color.Blue, Color.Yellow]
        ]);
    }

    [Fact]
    public void WrapMode_TileFlipX_ReturnsExpected()
    {
        // R|G|G|R
        // B|Y|Y|B
        // R|G|G|R
        // B|Y|Y|B
        VerifyFillRect(WrapMode.TileFlipX,
        [
            [Color.Red,    Color.Green,    Color.Green,    Color.Red],
            [Color.Blue,   Color.Yellow,   Color.Yellow,   Color.Blue],
            [Color.Red,    Color.Green,    Color.Green,    Color.Red],
            [Color.Blue,   Color.Yellow,   Color.Yellow,   Color.Blue]
        ]);
    }

    [Fact]
    public void WrapMode_TileFlipY_ReturnsExpected()
    {
        // R|G|R|G
        // B|Y|B|Y
        // B|Y|B|Y
        // R|G|R|G
        VerifyFillRect(WrapMode.TileFlipY,
        [
            [Color.Red,    Color.Green,    Color.Red,    Color.Green],
            [Color.Blue,   Color.Yellow,   Color.Blue,   Color.Yellow],
            [Color.Blue,   Color.Yellow,   Color.Blue,   Color.Yellow],
            [Color.Red,    Color.Green,    Color.Red,    Color.Green]
        ]);
    }

    [Fact]
    public void WrapMode_TileFlipXY_ReturnsExpected()
    {
        // R|G|G|R
        // B|Y|Y|B
        // B|Y|Y|B
        // R|G|G|R
        VerifyFillRect(WrapMode.TileFlipXY,
        [
            [Color.Red,    Color.Green,    Color.Green,    Color.Red],
            [Color.Blue,   Color.Yellow,   Color.Yellow,   Color.Blue],
            [Color.Blue,   Color.Yellow,   Color.Yellow,   Color.Blue],
            [Color.Red,    Color.Green,    Color.Green,    Color.Red]
        ]);
    }

    private static void VerifyFillRect(WrapMode wrapMode, Color[][] expectedColors)
    {
        using Bitmap brushBitmap = new(2, 2);
        brushBitmap.SetPixel(0, 0, Color.Red);
        brushBitmap.SetPixel(1, 0, Color.Green);
        brushBitmap.SetPixel(0, 1, Color.Blue);
        brushBitmap.SetPixel(1, 1, Color.Yellow);

        using TextureBrush brush = new(brushBitmap, wrapMode);
        using Bitmap targetImage = new(4, 4);
        using Graphics targetGraphics = Graphics.FromImage(targetImage);
        targetGraphics.FillRectangle(brush, new Rectangle(0, 0, 4, 4));

        Helpers.VerifyBitmap(targetImage, expectedColors);
    }
}
