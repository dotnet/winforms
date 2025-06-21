// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests;

public class TabRendererTests : IDisposable
{
    private readonly Bitmap _bitmap;
    private readonly Graphics _graphics;
    private readonly Rectangle _bounds;

    public TabRendererTests()
    {
        _bitmap = new(100, 30);
        _graphics = Graphics.FromImage(_bitmap);
        _bounds = new(0, 0, 100, 30);
    }

    public void Dispose()
    {
        _graphics.Dispose();
        _bitmap.Dispose();
    }

    [Fact]
    public void IsSupported_Matches_VisualStyleRenderer_IsSupported() =>
        TabRenderer.IsSupported.Should().Be(VisualStyleRenderer.IsSupported);

    [WinFormsFact]
    public void DrawTabItem_Basic_ChangesTabStateSuccessfully()
    {
        Color colorBeforeDraw = _bitmap.GetPixel(10, 10);

        foreach (TabItemState state in Enum.GetValues<TabItemState>())
        {
            TabRenderer.DrawTabItem(_graphics, _bounds, state);
            TabRenderer.DrawTabItem(_graphics, _bounds, focused: true, state);
        }

        Color colorAfterDraw = _bitmap.GetPixel(10, 10);
        colorAfterDraw.Should().NotBe(colorBeforeDraw);
    }

    [WinFormsFact]
    public void DrawTabItem_WithText_ChangesTabStateSuccessfully()
    {
        using Font font = new("Arial", 8);
        Color colorBeforeDraw = _bitmap.GetPixel(10, 10);

        foreach (TabItemState state in Enum.GetValues<TabItemState>())
        {
            TabRenderer.DrawTabItem(_graphics, _bounds, "TabText", font, state);
        }

        Color colorAfterDraw = _bitmap.GetPixel(10, 10);
        colorAfterDraw.Should().NotBe(colorBeforeDraw);
    }

    [WinFormsFact]
    public void DrawTabItem_WithText_Focused_ChangesTabStateSuccessfully()
    {
        using Font font = new("Arial", 8);
        Color colorBeforeDraw = _bitmap.GetPixel(10, 10);

        foreach (TabItemState state in Enum.GetValues<TabItemState>())
        {
            TabRenderer.DrawTabItem(_graphics, _bounds, "TabText", font, focused: true, state);
        }

        Color colorAfterDraw = _bitmap.GetPixel(10, 10);
        colorAfterDraw.Should().NotBe(colorBeforeDraw);
    }

    [WinFormsFact]
    public void DrawTabItem_WithText_TextFormatFlags_Focused_ChangesTabStateSuccessfully()
    {
        using Font font = new("Arial", 8);
        Color colorBeforeDraw = _bitmap.GetPixel(10, 10);

        foreach (TabItemState state in Enum.GetValues<TabItemState>())
        {
            TabRenderer.DrawTabItem(_graphics, _bounds, "TabText", font, TextFormatFlags.Default, focused: true, state);
        }

        Color colorAfterDraw = _bitmap.GetPixel(10, 10);
        colorAfterDraw.Should().NotBe(colorBeforeDraw);
    }

    [WinFormsFact]
    public void DrawTabItem_WithImage_ChangesTabStateSuccessfully()
    {
        using Bitmap image = new(16, 16);
        Rectangle imageRect = new(5, 5, 16, 16);
        Color colorBeforeDraw = _bitmap.GetPixel(10, 10);

        foreach (TabItemState state in Enum.GetValues<TabItemState>())
        {
            TabRenderer.DrawTabItem(_graphics, _bounds, image, imageRect, focused: false, state);
            TabRenderer.DrawTabItem(_graphics, _bounds, image, imageRect, focused: true, state);
        }

        Color colorAfterDraw = _bitmap.GetPixel(10, 10);
        colorAfterDraw.Should().NotBe(colorBeforeDraw);
    }

    [WinFormsFact]
    public void DrawTabItem_WithTextAndImage_ChangesTabStateSuccessfully()
    {
        using Font font = new("Arial", 8);
        using Bitmap image = new(16, 16);
        Rectangle imageRect = new(5, 5, 16, 16);
        Color colorBeforeDraw = _bitmap.GetPixel(10, 10);

        foreach (TabItemState state in Enum.GetValues<TabItemState>())
        {
            TabRenderer.DrawTabItem(_graphics, _bounds, "TabText", font, image, imageRect, focused: false, state);
            TabRenderer.DrawTabItem(_graphics, _bounds, "TabText", font, TextFormatFlags.Default, image, imageRect, focused: true, state);
        }

        Color colorAfterDraw = _bitmap.GetPixel(10, 10);
        colorAfterDraw.Should().NotBe(colorBeforeDraw);
    }

    [WinFormsFact]
    public void DrawTabPage_ChangesTabPageStateSuccessfully()
    {
        using Bitmap bmp = new(100, 100);
        using Graphics g = Graphics.FromImage(bmp);
        Rectangle bounds = new(0, 0, 100, 100);
        Color colorBeforeDraw = bmp.GetPixel(10, 10);

        TabRenderer.DrawTabPage(g, bounds);

        Color colorAfterDraw = bmp.GetPixel(10, 10);
        colorAfterDraw.Should().NotBe(colorBeforeDraw);
    }
}
