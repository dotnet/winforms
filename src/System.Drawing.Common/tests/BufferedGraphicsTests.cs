// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Tests;

public class BufferedGraphicsTests
{
    [Fact]
    public void Dispose_TempMultipleTimes_Success()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(3, 3);
        using Graphics targetGraphics = Graphics.FromImage(image);
        BufferedGraphics graphics = context.Allocate(targetGraphics, new Rectangle(0, 0, 1, 1));
        Assert.NotNull(graphics.Graphics);

        graphics.Dispose();
        Assert.Null(graphics.Graphics);

        graphics.Dispose();
    }

    [Fact]
    public void Dispose_ActualMultipleTimes_Success()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(3, 3);
        using Graphics targetGraphics = Graphics.FromImage(image);
        BufferedGraphics graphics = context.Allocate(targetGraphics, new Rectangle(0, 0, context.MaximumBuffer.Width + 1, context.MaximumBuffer.Height + 1));
        Assert.NotNull(graphics.Graphics);

        graphics.Dispose();
        Assert.Null(graphics.Graphics);

        graphics.Dispose();
    }

    [Fact]
    public void Render_ParameterlessWithTargetGraphics_Success()
    {
        Color color = Color.FromArgb(255, 0, 0, 0);

        using BufferedGraphicsContext context = new();
        using Bitmap image = new(3, 3);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(Color.Red);
        graphics.FillRectangle(brush, new Rectangle(0, 0, 3, 3));

        using BufferedGraphics bufferedGraphics = context.Allocate(graphics, new Rectangle(0, 0, 3, 3));
        bufferedGraphics.Render();

        Helpers.VerifyBitmap(image,
        [
                    [color, color, color],
                    [color, color, color],
                    [color, color, color]
        ]);
    }

    [Fact]
    public void Render_ParameterlessWithNullTargetGraphics_Success()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(3, 3);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(Color.Red);
        graphics.FillRectangle(brush, new Rectangle(0, 0, 3, 3));
        try
        {
            IntPtr hdc = graphics.GetHdc();

            using BufferedGraphics bufferedGraphics = context.Allocate(hdc, new Rectangle(0, 0, 3, 3));
            bufferedGraphics.Render();
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void Render_TargetGraphics_Success()
    {
        Color color = Color.FromArgb(255, 0, 0, 0);

        using BufferedGraphicsContext context = new();
        using Bitmap originalImage = new(3, 3);
        using Bitmap targetImage = new(3, 3);
        using Graphics originalGraphics = Graphics.FromImage(originalImage);
        using Graphics targetGraphics = Graphics.FromImage(targetImage);
        using SolidBrush brush = new(Color.Red);
        originalGraphics.FillRectangle(brush, new Rectangle(0, 0, 3, 3));

        using BufferedGraphics graphics = context.Allocate(originalGraphics, new Rectangle(0, 0, 3, 3));
        graphics.Render(targetGraphics);

        Helpers.VerifyBitmap(targetImage,
        [
                    [color, color, color],
                    [color, color, color],
                    [color, color, color]
        ]);
    }

    [Fact]
    public void Render_NullGraphics_Nop()
    {
        using BufferedGraphicsContext context = new();
        using Bitmap image = new(3, 3);
        using Graphics graphics = Graphics.FromImage(image);
        using BufferedGraphics bufferedGraphics = context.Allocate(graphics, new Rectangle(0, 0, 1, 1));
        bufferedGraphics.Render(null);
    }

    [Fact]
    public void Render_InvalidTargetDC_Nop()
    {
        using BufferedGraphicsContext context = new();
        using BufferedGraphics graphics = context.Allocate(null, Rectangle.Empty);
        graphics.Render(IntPtr.Zero);
        graphics.Render(-1);
    }
}
