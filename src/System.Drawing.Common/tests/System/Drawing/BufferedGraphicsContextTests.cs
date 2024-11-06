// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Drawing.Tests;

public class BufferedGraphicsContextTests
{
    [Fact]
    public void Ctor_Default()
    {
        using BufferedGraphicsContext context = new();
        Assert.Equal(new Size(225, 96), context.MaximumBuffer);
    }

    [Fact]
    public void Allocate_ValidTargetGraphics_Success()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using BufferedGraphics bufferedGraphics = context.Allocate(graphics, Rectangle.Empty);
        Assert.NotNull(bufferedGraphics.Graphics);

        context.Invalidate();
    }

    [Fact]
    public void Allocate_SmallRectWithTargetGraphics_Success()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using BufferedGraphics bufferedGraphics = context.Allocate(graphics, new Rectangle(0, 0, context.MaximumBuffer.Width - 1, context.MaximumBuffer.Height - 1));
        Assert.NotNull(bufferedGraphics.Graphics);

        context.Invalidate();
    }

    [Fact]
    public void Allocate_LargeRectWithTargetGraphics_Success()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using BufferedGraphics bufferedGraphics = context.Allocate(graphics, new Rectangle(0, 0, context.MaximumBuffer.Width + 1, context.MaximumBuffer.Height + 1));
        Assert.NotNull(bufferedGraphics.Graphics);

        context.Invalidate();
    }

    [Fact]
    public void Allocate_ValidTargetHdc_Success()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        try
        {
            IntPtr hdc = graphics.GetHdc();
            using (BufferedGraphics bufferedGraphics = context.Allocate(hdc, Rectangle.Empty))
            {
                Assert.NotNull(bufferedGraphics.Graphics);
            }

            context.Invalidate();
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void Allocate_SmallRectWithTargetHdc_Success()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        try
        {
            IntPtr hdc = graphics.GetHdc();
            using (BufferedGraphics bufferedGraphics = context.Allocate(hdc, new Rectangle(0, 0, context.MaximumBuffer.Width - 1, context.MaximumBuffer.Height - 1)))
            {
                Assert.NotNull(bufferedGraphics.Graphics);
            }

            context.Invalidate();
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void Allocate_LargeRectWithTargetHdc_Success()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        try
        {
            IntPtr hdc = graphics.GetHdc();
            using (BufferedGraphics bufferedGraphics = context.Allocate(hdc, new Rectangle(0, 0, context.MaximumBuffer.Width + 1, context.MaximumBuffer.Height + 1)))
            {
                Assert.NotNull(bufferedGraphics.Graphics);
            }

            context.Invalidate();
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void Allocate_InvalidHdc_ThrowsArgumentException()
    {
        using BufferedGraphicsContext context = new();
        AssertExtensions.Throws<ArgumentException>(null, () => context.Allocate(-1, new Rectangle(0, 0, 10, 10)));
    }

    [Fact]
    public void Allocate_NullGraphicsZeroSize_Success()
    {
        using BufferedGraphicsContext context = new();
        using BufferedGraphics graphics = context.Allocate(null, Rectangle.Empty);
        Assert.NotNull(graphics.Graphics);
    }

    [Fact]
    public void Allocate_NullGraphicsNonZeroSize_ThrowsArgumentNullException()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        Assert.Throws<ArgumentNullException>("hdc", () => context.Allocate(null, new Rectangle(0, 0, 10, 10)));
    }

    [Fact]
    public void Allocate_DisposedGraphics_ThrowsArgumentException()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        Rectangle largeRectangle = new(0, 0, context.MaximumBuffer.Width + 1, context.MaximumBuffer.Height + 1);
        AssertExtensions.Throws<ArgumentException>(null, () => context.Allocate(graphics, largeRectangle));
        AssertExtensions.Throws<ArgumentException>(null, () => context.Allocate(graphics, Rectangle.Empty));
    }

    [Fact]
    public void Allocate_BusyGraphics_ThrowsInvalidOperationException()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        try
        {
            graphics.GetHdc();

            Rectangle largeRectangle = new(0, 0, context.MaximumBuffer.Width + 1, context.MaximumBuffer.Height + 1);
            Assert.Throws<InvalidOperationException>(() => context.Allocate(graphics, largeRectangle));
            Assert.Throws<InvalidOperationException>(() => context.Allocate(graphics, Rectangle.Empty));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void Invalidate_CallMultipleTimes_Success()
    {
        using BufferedGraphicsContext context = new();
        context.Invalidate();
        context.Invalidate();
    }

    [Fact]
    public void MaximumBuffer_SetValid_ReturnsExpected()
    {
        using BufferedGraphicsContext context = new();
        context.MaximumBuffer = new Size(10, 10);
        Assert.Equal(new Size(10, 10), context.MaximumBuffer);

        context.MaximumBuffer = new Size(255, 255);
        Assert.Equal(new Size(255, 255), context.MaximumBuffer);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MaximumBuffer_SetInvalidWidth_ThrowsArgumentException(int width)
    {
        using BufferedGraphicsContext context = new();
        AssertExtensions.Throws<ArgumentException>("value", null, () => context.MaximumBuffer = new Size(width, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MaximumBuffer_SetInvalidHeight_ThrowsArgumentException(int height)
    {
        using BufferedGraphicsContext context = new();
        AssertExtensions.Throws<ArgumentException>("value", null, () => context.MaximumBuffer = new Size(1, height));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void AllocateBufferedGraphicsContext() => new BufferedGraphicsContext();

    [Fact]
    public void Finalize_Invoke_Success()
    {
        // This makes sure than finalization doesn't cause any errors or debug assertions.
        AllocateBufferedGraphicsContext();

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [Fact]
    public void Dispose_BusyAndValidated_ThrowsInvalidOperationException()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using (context.Allocate(graphics, Rectangle.Empty))
        {
            Assert.Throws<InvalidOperationException>(context.Dispose);
        }
    }

    [Fact]
    public void Dispose_BusyAndInvalidated_ThrowsInvalidOperationException()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using (context.Allocate(graphics, Rectangle.Empty))
        {
            context.Invalidate();
            Assert.Throws<InvalidOperationException>(context.Dispose);
        }
    }
}
