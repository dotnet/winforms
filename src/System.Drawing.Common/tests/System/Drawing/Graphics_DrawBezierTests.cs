// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Tests;

public class Graphics_DrawBezierTests : DrawingTest
{
    [Fact]
    public void DrawBezier_Point()
    {
        using Bitmap bitmap = new(100, 100);
        using Pen pen = new(Color.White);
        using Graphics graphics = Graphics.FromImage(bitmap);

        graphics.DrawBezier(pen, new(10, 10), new(20, 1), new(35, 5), new(50, 10));
        ValidateBitmapContent(
            bitmap,
            0x0E, 0x97, 0x36, 0x25, 0x8B, 0x2E, 0x3C, 0x21, 0xAE, 0x8C, 0xEB, 0x80, 0x0B, 0xA1, 0xCD, 0x05,
            0xD2, 0x34, 0xCE, 0xE5, 0x70, 0x35, 0xC6, 0x57, 0xBA, 0x16, 0xF2, 0xA3, 0xFD, 0x87, 0x0F, 0xB5);
    }

    [Fact]
    public void DrawBezier_Points()
    {
        using Bitmap bitmap = new(100, 100);
        using Pen pen = new(Color.Red);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Point[] points =
        [
            new(10, 10), new(20, 1), new(35, 5), new(50, 10),
            new(60, 15), new(65, 25), new(50, 30)
        ];

        graphics.DrawBeziers(pen, points);
        ValidateBitmapContent(
            bitmap,
            0xDC, 0xDB, 0xFF, 0x36, 0x7F, 0x0D, 0x84, 0xA3, 0x74, 0x35, 0x0A, 0x1E, 0x78, 0xCA, 0x76, 0x25,
            0xDB, 0x28, 0xED, 0x6A, 0x32, 0x3D, 0xB8, 0xD4, 0xBF, 0x19, 0xA0, 0x0F, 0x12, 0xB3, 0x35, 0xA6);
    }

    [Fact]
    public void DrawBezier_PointFs()
    {
        using Bitmap bitmap = new(100, 100);
        using Pen pen = new(Color.Red);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PointF[] points =
        [
            new(10.0f, 10.0f), new(20.0f, 1.0f), new(35.0f, 5.0f), new(50.0f, 10.0f),
            new(60.0f, 15.0f), new(65.0f, 25.0f), new(50.0f, 30.0f)
        ];

        graphics.DrawBeziers(pen, points);
        ValidateBitmapContent(
            bitmap,
            0xDC, 0xDB, 0xFF, 0x36, 0x7F, 0x0D, 0x84, 0xA3, 0x74, 0x35, 0x0A, 0x1E, 0x78, 0xCA, 0x76, 0x25,
            0xDB, 0x28, 0xED, 0x6A, 0x32, 0x3D, 0xB8, 0xD4, 0xBF, 0x19, 0xA0, 0x0F, 0x12, 0xB3, 0x35, 0xA6);
    }

    [Fact]
    public void DrawBezier_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawBezier(null, 1, 2, 3, 4, 5, 6, 7, 8));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawBezier(null, Point.Empty, Point.Empty, Point.Empty, Point.Empty));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawBezier(null, PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty));
    }

    [Fact]
    public void DrawBezier_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBezier(pen, 1, 2, 3, 4, 5, 6, 7, 8));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBezier(pen, Point.Empty, Point.Empty, Point.Empty, Point.Empty));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBezier(pen, PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty));
    }

    [Fact]
    public void DrawBezier_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawBezier(pen, 1, 2, 3, 4, 5, 6, 7, 8));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawBezier(pen, Point.Empty, Point.Empty, Point.Empty, Point.Empty));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawBezier(pen, PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawBezier_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new Rectangle(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0, 0, 1, 1, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new RectangleF(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0f, 0f, 1f, 1f, 0, 90));
    }

    [Fact]
    public void DrawBeziers_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawBeziers(null, new Point[2]));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawBeziers(null, new PointF[2]));
    }

    [Fact]
    public void DrawBeziers_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, new Point[2]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, new PointF[2]));
    }

    [Fact]
    public void DrawBeziers_NullPoints_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawBeziers(pen, (Point[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawBeziers(pen, (PointF[])null));
    }

    [Fact]
    public void DrawBeziers_EmptyPoints_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, Array.Empty<Point>()));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, Array.Empty<PointF>()));
    }

    [Fact]
    public void DrawBeziers_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawBeziers(pen, new Point[2]));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawBeziers(pen, new PointF[2]));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawBeziers_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, new Point[2]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, new PointF[2]));
    }
}
