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
            0x35, 0xa8, 0x3a, 0x03, 0x1e, 0xbd, 0xd4, 0xc0, 0xa4, 0x70, 0x51, 0xba, 0x09, 0xc1, 0xc1, 0xf9);
    }

    [Fact]
    public void DrawBezier_Points()
    {
        using Bitmap bitmap = new(100, 100);
        using Pen pen = new(Color.Red);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Point[] points =
        {
            new(10, 10), new(20, 1), new(35, 5), new(50, 10),
            new(60, 15), new(65, 25), new(50, 30)
        };

        graphics.DrawBeziers(pen, points);
        ValidateBitmapContent(
            bitmap,
            0x7a, 0x02, 0x29, 0xa0, 0xc5, 0x21, 0x94, 0x31, 0xc8, 0x96, 0x31, 0x09, 0xcc, 0xd6, 0xec, 0x63);
    }

    [Fact]
    public void DrawBezier_PointFs()
    {
        using Bitmap bitmap = new(100, 100);
        using Pen pen = new(Color.Red);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PointF[] points =
        {
            new(10.0f, 10.0f), new(20.0f, 1.0f), new(35.0f, 5.0f), new(50.0f, 10.0f),
            new(60.0f, 15.0f), new(65.0f, 25.0f), new(50.0f, 30.0f)
        };

        graphics.DrawBeziers(pen, points);
        ValidateBitmapContent(
            bitmap,
            0x7a, 0x02, 0x29, 0xa0, 0xc5, 0x21, 0x94, 0x31, 0xc8, 0x96, 0x31, 0x09, 0xcc, 0xd6, 0xec, 0x63);
    }

    [Fact]
    public void DrawBezier_NullPen_ThrowsArgumentNullException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawBezier(null, 1, 2, 3, 4, 5, 6, 7, 8));
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawBezier(null, Point.Empty, Point.Empty, Point.Empty, Point.Empty));
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawBezier(null, PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty));
        }
    }

    [Fact]
    public void DrawBezier_DisposedPen_ThrowsArgumentException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            Pen pen = new(Color.Red);
            pen.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBezier(pen, 1, 2, 3, 4, 5, 6, 7, 8));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBezier(pen, Point.Empty, Point.Empty, Point.Empty, Point.Empty));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBezier(pen, PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty));
        }
    }

    [Fact]
    public void DrawBezier_Busy_ThrowsInvalidOperationException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Pen pen = new(Color.Red))
        {
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
    }

    [Fact]
    public void DrawBezier_Disposed_ThrowsArgumentException()
    {
        using (Bitmap image = new(10, 10))
        using (Pen pen = new(Color.Red))
        {
            Graphics graphics = Graphics.FromImage(image);
            graphics.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new Rectangle(0, 0, 1, 1), 0, 90));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0, 0, 1, 1, 0, 90));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new RectangleF(0, 0, 1, 1), 0, 90));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0f, 0f, 1f, 1f, 0, 90));
        }
    }

    [Fact]
    public void DrawBeziers_NullPen_ThrowsArgumentNullException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawBeziers(null, new Point[2]));
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawBeziers(null, new PointF[2]));
        }
    }

    [Fact]
    public void DrawBeziers_DisposedPen_ThrowsArgumentException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            Pen pen = new(Color.Red);
            pen.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, new Point[2]));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, new PointF[2]));
        }
    }

    [Fact]
    public void DrawBeziers_NullPoints_ThrowsArgumentNullException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Pen pen = new(Color.Red))
        {
            AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawBeziers(pen, (Point[])null));
            AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawBeziers(pen, (PointF[])null));
        }
    }

    [Fact]
    public void DrawBeziers_EmptyPoints_ThrowsArgumentException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Pen pen = new(Color.Red))
        {
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, new Point[0]));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, new PointF[0]));
        }
    }

    [Fact]
    public void DrawBeziers_Busy_ThrowsInvalidOperationException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Pen pen = new(Color.Red))
        {
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
    }

    [Fact]
    public void DrawBeziers_Disposed_ThrowsArgumentException()
    {
        using (Bitmap image = new(10, 10))
        using (Pen pen = new(Color.Red))
        {
            Graphics graphics = Graphics.FromImage(image);
            graphics.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, new Point[2]));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawBeziers(pen, new PointF[2]));
        }
    }
}
