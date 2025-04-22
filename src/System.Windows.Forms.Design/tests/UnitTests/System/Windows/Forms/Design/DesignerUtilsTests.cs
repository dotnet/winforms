// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Imaging;

namespace System.Windows.Forms.Design.Tests;

public class DesignerUtilsTests
{
    public static Image BoxImage => DesignerUtils.BoxImage;
    public static Brush HoverBrush => DesignerUtils.HoverBrush;

    [Fact]
    public void BoxImage_ShouldReturnNonNullImage() =>
        BoxImage.Should().NotBeNull();

    [Fact]
    public void BoxImage_ShouldHaveExpectedDimensions()
    {
        BoxImage.Width.Should().Be(DesignerUtils.s_boxImageSize);
        BoxImage.Height.Should().Be(DesignerUtils.s_boxImageSize);
    }

    [Fact]
    public void BoxImage_ShouldHaveExpectedPixelFormat() =>
        ((Bitmap)BoxImage).PixelFormat.Should().Be(PixelFormat.Format32bppPArgb);

    [Fact]
    public void BoxImage_ShouldBeCached()
    {
        Image firstCall = DesignerUtils.BoxImage;
        Image secondCall = DesignerUtils.BoxImage;
        firstCall.Should().BeSameAs(secondCall);
    }

    [Fact]
    public void HoverBrush_ShouldReturnNonNullBrush() =>
        HoverBrush.Should().NotBeNull();

    [Fact]
    public void HoverBrush_ShouldBeSolidBrush() =>
        HoverBrush.Should().BeOfType<SolidBrush>();

    [Fact]
    public void HoverBrush_ShouldHaveExpectedColor() =>
        ((SolidBrush)HoverBrush).Color.Should().Be(Color.FromArgb(50, SystemColors.Highlight));

    [Fact]
    public void MinDragSize_ShouldReturnNonEmptySize()
    {
        Size minDragSize = DesignerUtils.MinDragSize;
        minDragSize.Should().NotBe(Size.Empty);
    }

    [Fact]
    public void MinDragSize_ShouldBeCached()
    {
        Size firstCall = DesignerUtils.MinDragSize;
        Size secondCall = DesignerUtils.MinDragSize;
        firstCall.Should().Be(secondCall);
    }

    [Theory]
    [MemberData(nameof(ResizeBorderTestData))]
    public void DrawResizeBorder_ShouldUseCorrectBrushBasedOnBackColor(int width, int height, Color backColor, Color expectedColor)
    {
        using Bitmap bitmap = new(width, height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using Region region = new(new Rectangle(0, 0, width, height));
        DesignerUtils.DrawResizeBorder(graphics, region, backColor);
        Color pixelColor = bitmap.GetPixel(width / 2, height / 2);
        pixelColor.ToArgb().Should().Be(expectedColor.ToArgb());
    }

    public static TheoryData<int, int, Color, Color> ResizeBorderTestData => new()
    {
        { 10, 10, Color.White, SystemColors.ControlDarkDark },
        { 10, 10, Color.Black, SystemColors.ControlLight }
    };

    [Fact]
    public void DrawGrabHandle_ShouldNotThrow_WhenCalledWithValidParameters()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(2, 2, 6, 6);
        Exception exception = Record.Exception(() => DesignerUtils.DrawGrabHandle(graphics, bounds, isPrimary: true));
        exception.Should().BeNull();
    }

    [Theory]
    [BoolData]
    public void DrawGrabHandle_ShouldDrawHandle_BasedOnSelectionType(bool isPrimary)
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(2, 2, 6, 6);

        DesignerUtils.DrawGrabHandle(graphics, bounds, isPrimary);

        Color pixelColor = bitmap.GetPixel(3, 3);
        pixelColor.ToArgb().Should().NotBe(Color.Empty.ToArgb());
    }

    [Theory]
    [BoolData]
    public void DrawLockedHandle_ShouldDrawHandle_BasedOnSelectionType(bool isPrimary)
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(5, 5, 10, 10);

        DesignerUtils.DrawLockedHandle(graphics, bounds, isPrimary);

        Color pixelColor = bitmap.GetPixel(6, 6);
        pixelColor.ToArgb().Should().NotBe(Color.Empty.ToArgb());
    }

    [Fact]
    public void DrawLockedHandle_ShouldDrawUpperRect_WhenCalledWithPrimarySelection()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(5, 5, 10, 10);
        DesignerUtils.DrawLockedHandle(graphics, bounds, isPrimary: true);
        Color pixelColor = bitmap.GetPixel(bounds.Left + 1, bounds.Top + 1);
        pixelColor.ToArgb().Should().NotBe(Color.Empty.ToArgb());
    }

    [Fact]
    public void DrawLockedHandle_ShouldDrawLowerRect_WhenCalledWithNonPrimarySelection()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(5, 5, 10, 10);
        DesignerUtils.DrawLockedHandle(graphics, bounds, isPrimary: false);
        Color pixelColor = bitmap.GetPixel(bounds.Left + 1, bounds.Top + DesignerUtils.s_lockedHandleLowerOffset + 1);
        pixelColor.ToArgb().Should().NotBe(Color.Empty.ToArgb());
    }

    [Fact]
    public void DrawSelectionBorder_ShouldNotThrow_WhenCalledWithValidParameters()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(5, 5, 10, 10);
        Exception exception = Record.Exception(() => DesignerUtils.DrawSelectionBorder(graphics, bounds));
        exception.Should().BeNull();
    }

    [Fact]
    public void DrawSelectionBorder_ShouldFillRectangle_WhenCalled()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle bounds = new(5, 5, 10, 10);
        DesignerUtils.DrawSelectionBorder(graphics, bounds);
        Color pixelColor = bitmap.GetPixel(6, 6);
        pixelColor.ToArgb().Should().NotBe(Color.Empty.ToArgb());
    }
}
