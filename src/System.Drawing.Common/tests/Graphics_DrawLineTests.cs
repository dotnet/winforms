// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Tests;

public class Graphics_DrawLineTests : DrawingTest
{
    [Fact]
    public void DrawLines_Points()
    {
        using Bitmap bitmap = new(100, 100);
        using Pen pen = new(Color.White);
        using Graphics graphics = Graphics.FromImage(bitmap);

        graphics.DrawLines(pen, new Point[] { new(1, 1), new(1, 10), new(20, 5), new(25, 30) });

        ValidateBitmapContent(
            bitmap,
            0xeb, 0xc6, 0x1e, 0xbf, 0xc0, 0x42, 0xa7, 0xfd, 0xcd, 0x24, 0xbc, 0x1c, 0x79, 0x0a, 0x69, 0x07);
    }

    [Fact]
    public void DrawLines_PointFs()
    {
        using Bitmap bitmap = new(100, 100);
        using Pen pen = new(Color.White);
        using Graphics graphics = Graphics.FromImage(bitmap);

        graphics.DrawLines(pen, new PointF[] { new(1.0f, 1.0f), new(1.0f, 10.0f), new(20.0f, 5.0f), new(25.0f, 30.0f) });

        ValidateBitmapContent(
            bitmap,
            0xeb, 0xc6, 0x1e, 0xbf, 0xc0, 0x42, 0xa7, 0xfd, 0xcd, 0x24, 0xbc, 0x1c, 0x79, 0x0a, 0x69, 0x07);
    }

    [Fact]
    public void DrawLine_NullPen_ThrowsArgumentNullException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawLine(null, Point.Empty, Point.Empty));
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawLine(null, 0, 0, 0, 0));
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawLine(null, PointF.Empty, PointF.Empty));
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawLine(null, 0f, 0f, 0f, 0f));
        }
    }

    [Fact]
    public void DrawLine_DisposedPen_ThrowsArgumentException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            Pen pen = new(Color.Red);
            pen.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLine(pen, Point.Empty, Point.Empty));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLine(pen, 0, 0, 0, 0));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLine(pen, PointF.Empty, PointF.Empty));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLine(pen, 0f, 0f, 0f, 0f));
        }
    }

    [Fact]
    public void DrawLine_Busy_ThrowsInvalidOperationException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Pen pen = new(Color.Red))
        {
            graphics.GetHdc();
            try
            {
                Assert.Throws<InvalidOperationException>(() => graphics.DrawLine(pen, Point.Empty, Point.Empty));
                Assert.Throws<InvalidOperationException>(() => graphics.DrawLine(pen, 0, 0, 0, 0));
                Assert.Throws<InvalidOperationException>(() => graphics.DrawLine(pen, PointF.Empty, PointF.Empty));
                Assert.Throws<InvalidOperationException>(() => graphics.DrawLine(pen, 0f, 0f, 0f, 0f));
            }
            finally
            {
                graphics.ReleaseHdc();
            }
        }
    }

    [Fact]
    public void DrawLine_Disposed_ThrowsArgumentException()
    {
        using (Bitmap image = new(10, 10))
        using (Pen pen = new(Color.Red))
        {
            Graphics graphics = Graphics.FromImage(image);
            graphics.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLine(pen, Point.Empty, Point.Empty));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLine(pen, 0, 0, 0, 0));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLine(pen, PointF.Empty, PointF.Empty));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLine(pen, 0f, 0f, 0f, 0f));
        }
    }

    [Fact]
    public void DrawLines_NullPen_ThrowsArgumentNullException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawLines(null, new Point[2]));
            AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawLines(null, new PointF[2]));
        }
    }

    [Fact]
    public void DrawLines_DisposedPen_ThrowsArgumentException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        {
            Pen pen = new(Color.Red);
            pen.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLines(pen, new Point[2]));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLines(pen, new PointF[2]));
        }
    }

    [Fact]
    public void DrawLines_NullPoints_ThrowsArgumentNullException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Pen pen = new(Color.Red))
        {
            AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawLines(pen, (Point[])null));
            AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawLines(pen, (PointF[])null));
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void DrawLines_InvalidPointsLength_ThrowsArgumentException(int length)
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Pen pen = new(Color.Red))
        {
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLines(pen, new Point[length]));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLines(pen, new PointF[length]));
        }
    }

    [Fact]
    public void DrawLines_Busy_ThrowsInvalidOperationException()
    {
        using (Bitmap image = new(10, 10))
        using (Graphics graphics = Graphics.FromImage(image))
        using (Pen pen = new(Color.Red))
        {
            graphics.GetHdc();
            try
            {
                Assert.Throws<InvalidOperationException>(() => graphics.DrawLines(pen, new Point[2]));
                Assert.Throws<InvalidOperationException>(() => graphics.DrawLines(pen, new PointF[2]));
            }
            finally
            {
                graphics.ReleaseHdc();
            }
        }
    }

    [Fact]
    public void DrawLines_Disposed_ThrowsArgumentException()
    {
        using (Bitmap image = new(10, 10))
        using (Pen pen = new(Color.Red))
        {
            Graphics graphics = Graphics.FromImage(image);
            graphics.Dispose();

            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLines(pen, new Point[2]));
            AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawLines(pen, new PointF[2]));
        }
    }
}
