// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Drawing2D;
using System.Numerics;

namespace System.Drawing.Tests;

public partial class GraphicsTests
{
    private static Matrix3x2 s_testMatrix = Matrix3x2.CreateRotation(45) * Matrix3x2.CreateScale(2) * Matrix3x2.CreateTranslation(new Vector2(10, 20));

    [Fact]
    public void TransformElements_SetNonInvertibleMatrix_ThrowsArgumentException()
    {
        using Bitmap image = new(5, 5);
        using Graphics graphics = Graphics.FromImage(image);
        Matrix3x2 matrix = new(123, 24, 82, 16, 47, 30);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformElements = matrix);
    }

    [Fact]
    public void TransformElements_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.TransformElements);
            Assert.Throws<InvalidOperationException>(() => graphics.TransformElements = Matrix3x2.Identity);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void TransformElements_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformElements);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformElements = Matrix3x2.Identity);
    }

    [Fact]
    public void TransformElements_RoundTrip()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.TransformElements = s_testMatrix;
        Assert.Equal(s_testMatrix, graphics.TransformElements);

        using (Matrix matrix = graphics.Transform)
        {
            Assert.Equal(s_testMatrix, matrix.MatrixElements);
        }

        using (Matrix matrix = new())
        {
            graphics.Transform = matrix;
            Assert.True(graphics.TransformElements.IsIdentity);
        }
    }

    [Fact]
    public void DrawRectangle_NullPen_ThrowsArgumentNullException_Core()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawRectangle(null, new RectangleF(0f, 0f, 1f, 1f)));

        // other DrawRectangle overloads tested in DrawRectangle_NullPen_ThrowsArgumentNullException()
    }

    [Fact]
    public void DrawRectangle_DisposedPen_ThrowsArgumentException_Core()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangle(pen, new RectangleF(0f, 0f, 1f, 1f)));

        // other DrawRectangle overloads tested in DrawRectangle_DisposedPen_ThrowsArgumentException()
    }

    [Fact]
    public void DrawRectangle_Busy_ThrowsInvalidOperationException_Core()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawRectangle(pen, new RectangleF(0f, 0f, 1f, 1f)));
            // other DrawRectangle overloads tested in DrawRectangle_Busy_ThrowsInvalidOperationException()
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawRectangle_Disposed_ThrowsArgumentException_Core()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangle(pen, new RectangleF(0f, 0f, 1f, 1f)));

        // other DrawRectangle overloads tested in DrawRectangle_Disposed_ThrowsArgumentException()
    }

    [Fact]
    public void FillPie_NullPen_ThrowsArgumentNullException_Core()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("brush", () => graphics.FillPie(null, new RectangleF(0, 0, 1, 1), 0, 90));

        // other FillPie overloads tested in FillPie_NullPen_ThrowsArgumentNullException()
    }

    [Fact]
    public void FillPie_DisposedPen_ThrowsArgumentException_Core()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        SolidBrush brush = new(Color.Red);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, new RectangleF(0, 0, 1, 1), 0, 90));

        // other FillPie overloads tested in FillPie_DisposedPen_ThrowsArgumentException()
    }

    [Fact]
    public void FillPie_ZeroWidth_ThrowsArgumentException_Core()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, new RectangleF(0, 0, 0, 1), 0, 90));

        // other FillPie overloads tested in FillPie_ZeroWidth_ThrowsArgumentException()
    }

    [Fact]
    public void FillPie_ZeroHeight_ThrowsArgumentException_Core()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, new RectangleF(0, 0, 1, 0), 0, 90));

        // other FillPie overloads tested in FillPie_ZeroHeight_ThrowsArgumentException()
    }

    [Fact]
    public void FillPie_Busy_ThrowsInvalidOperationException_Core()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.FillPie(brush, new RectangleF(0, 0, 1, 1), 0, 90));
            // other FillPie overloads tested in FillPie_Busy_ThrowsInvalidOperationException()
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void FillPie_Disposed_ThrowsArgumentException_Core()
    {
        using Bitmap image = new(10, 10);
        using SolidBrush brush = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, new RectangleF(0, 0, 1, 1), 0, 90));

        // other FillPie overloads tested in FillPie_Disposed_ThrowsArgumentException()
    }
}
