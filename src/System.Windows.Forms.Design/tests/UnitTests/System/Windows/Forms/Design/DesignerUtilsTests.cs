// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Imaging;

namespace System.Windows.Forms.Design.Tests;

public class DesignerUtilsTests :IDisposable
{
    private readonly Bitmap _bitmap;
    private readonly Graphics _graphics;
    private readonly Rectangle _bounds;

    public DesignerUtilsTests()
    {
        _bitmap = new(20, 20);
        _graphics = Graphics.FromImage(_bitmap);
        _bounds = new(5, 5, 20, 20);
    }

    public void Dispose()
    {
        _graphics.Dispose();
        _bitmap.Dispose();
    }

    [WinFormsFact]
    public void BoxImage_ShouldReturnNonNullImage() =>
        DesignerUtils.BoxImage.Should().BeOfType<Bitmap>();

    [WinFormsFact]
    public void BoxImage_ShouldHaveExpectedDimensions()
    {
        DesignerUtils.BoxImage.Width.Should().Be(DesignerUtils.s_boxImageSize);
        DesignerUtils.BoxImage.Height.Should().Be(DesignerUtils.s_boxImageSize);
    }

    [WinFormsFact]
    public void BoxImage_ShouldHaveExpectedPixelFormat() =>
        ((Bitmap)DesignerUtils.BoxImage).PixelFormat.Should().Be(PixelFormat.Format32bppPArgb);

    [WinFormsFact]
    public void BoxImage_ShouldBeCached()
    {
        Image firstCall = DesignerUtils.BoxImage;
        Image secondCall = DesignerUtils.BoxImage;
        firstCall.Should().BeSameAs(secondCall);
    }

    [Fact]
    public void HoverBrush_ShouldBeSolidBrush() =>
        DesignerUtils.HoverBrush.Should().BeOfType<SolidBrush>();

    [Fact]
    public void HoverBrush_ShouldHaveExpectedColor() =>
        ((SolidBrush)DesignerUtils.HoverBrush).Color.Should().Be(Color.FromArgb(50, SystemColors.Highlight));

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

    [WinFormsTheory]
    [MemberData(nameof(ResizeBorderTestData))]
    public void DrawResizeBorder_ShouldUseCorrectBrushBasedOnBackColor(Color backColor, Color expectedColor)
    {
        using Region region = new(new Rectangle(0, 0, _bounds.Width, _bounds.Height));
        DesignerUtils.DrawResizeBorder(_graphics, region, backColor);
        Color pixelColor = _bitmap.GetPixel(_bounds.Width / 2, _bounds.Height / 2);
        pixelColor.ToArgb().Should().Be(expectedColor.ToArgb());
    }

    public static TheoryData<Color, Color> ResizeBorderTestData => new()
    {
        { Color.White, SystemColors.ControlDarkDark },
        { Color.Black, SystemColors.ControlLight }
    };

    [WinFormsFact]
    public void DrawGrabHandle_ShouldNotThrow_WhenCalledWithValidParameters()
    {
        Exception exception = Record.Exception(() => DesignerUtils.DrawGrabHandle(_graphics, _bounds, isPrimary: true));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [BoolData]
    public void DrawGrabHandle_ShouldDrawHandle_BasedOnSelectionType(bool isPrimary)
    {
        DesignerUtils.DrawGrabHandle(_graphics, _bounds, isPrimary);

        Color pixelColor = _bitmap.GetPixel(6, 6);
        pixelColor.ToArgb().Should().NotBe(Color.Empty.ToArgb());
    }

    [WinFormsTheory]
    [BoolData]
    public void DrawLockedHandle_ShouldDrawHandle_BasedOnSelectionType(bool isPrimary)
    {
        DesignerUtils.DrawLockedHandle(_graphics, _bounds, isPrimary);

        Color pixelColor = _bitmap.GetPixel(6, 6);
        pixelColor.ToArgb().Should().NotBe(Color.Empty.ToArgb());
    }

    [WinFormsFact]
    public void DrawLockedHandle_ShouldDrawUpperRect_WhenCalledWithPrimarySelection()
    {
        DesignerUtils.DrawLockedHandle(_graphics, _bounds, isPrimary: true);
        Color pixelColor = _bitmap.GetPixel(_bounds.Left + 1, _bounds.Top + 1);
        pixelColor.ToArgb().Should().NotBe(Color.Empty.ToArgb());
    }

    [WinFormsFact]
    public void DrawLockedHandle_ShouldDrawLowerRect_WhenCalledWithNonPrimarySelection()
    {
        DesignerUtils.DrawLockedHandle(_graphics, _bounds, isPrimary: false);
        Color pixelColor = _bitmap.GetPixel(_bounds.Left + 1, _bounds.Top + DesignerUtils.s_lockedHandleLowerOffset + 1);
        pixelColor.ToArgb().Should().NotBe(Color.Empty.ToArgb());
    }

    [WinFormsFact]
    public void DrawSelectionBorder_ShouldNotThrow_WhenCalledWithValidParameters()
    {
        Exception exception = Record.Exception(() => DesignerUtils.DrawSelectionBorder(_graphics, _bounds));
        exception.Should().BeNull();
    }

    [WinFormsFact]
    public void DrawSelectionBorder_ShouldFillRectangle_WhenCalled()
    {
        DesignerUtils.DrawSelectionBorder(_graphics, _bounds);
        Color pixelColor = _bitmap.GetPixel(6, 6);
        pixelColor.ToArgb().Should().NotBe(Color.Empty.ToArgb());
    }
}
