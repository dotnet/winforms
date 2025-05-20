// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests;

public class TabRendererTests
{
    [Fact]
    public void IsSupported_Matches_VisualStyleRenderer_IsSupported() =>
        TabRenderer.IsSupported.Should().Be(VisualStyleRenderer.IsSupported);

    [WinFormsFact]
    public void DrawTabItem_Basic_Overloads_DoNotThrow()
    {
        using Bitmap bmp = new(100, 30);
        using Graphics g = Graphics.FromImage(bmp);
        Rectangle bounds = new(0, 0, 100, 30);
        Color before = bmp.GetPixel(10, 10);

        foreach (TabItemState state in Enum.GetValues<TabItemState>())
        {
            TabRenderer.DrawTabItem(g, bounds, state);
            TabRenderer.DrawTabItem(g, bounds, focused: true, state);
        }

        Color after = bmp.GetPixel(10, 10);
        after.Should().NotBe(before);
    }

    [WinFormsFact]
    public void DrawTabItem_WithText_Overloads_DoNotThrow()
    {
        using Bitmap bmp = new(100, 30);
        using Graphics g = Graphics.FromImage(bmp);
        Rectangle bounds = new(0, 0, 100, 30);
        using Font font = new("Arial", 8);
        Color before = bmp.GetPixel(10, 10);

        foreach (TabItemState state in Enum.GetValues<TabItemState>())
        {
            TabRenderer.DrawTabItem(g, bounds, "TabText", font, state);
            TabRenderer.DrawTabItem(g, bounds, "TabText", font, focused: true, state);
            TabRenderer.DrawTabItem(g, bounds, "TabText", font, TextFormatFlags.Default, focused: true, state);
        }

        Color after = bmp.GetPixel(10, 10);
        after.Should().NotBe(before);
    }

    [WinFormsFact]
    public void DrawTabItem_WithImage_Overloads_DoNotThrow()
    {
        using Bitmap bmp = new(100, 30);
        using Graphics g = Graphics.FromImage(bmp);
        Rectangle bounds = new(0, 0, 100, 30);
        using Bitmap image = new(16, 16);
        Rectangle imageRect = new(5, 5, 16, 16);
        Color before = bmp.GetPixel(10, 10);

        foreach (TabItemState state in Enum.GetValues<TabItemState>())
        {
            TabRenderer.DrawTabItem(g, bounds, image, imageRect, focused: false, state);
            TabRenderer.DrawTabItem(g, bounds, image, imageRect, focused: true, state);
        }

        Color after = bmp.GetPixel(10, 10);
        after.Should().NotBe(before);
    }

    [WinFormsFact]
    public void DrawTabItem_WithTextAndImage_Overloads_DoNotThrow()
    {
        using Bitmap bmp = new(100, 30);
        using Graphics g = Graphics.FromImage(bmp);
        Rectangle bounds = new(0, 0, 100, 30);
        using Font font = new("Arial", 8);
        using Bitmap image = new(16, 16);
        Rectangle imageRect = new(5, 5, 16, 16);
        Color before = bmp.GetPixel(10, 10);

        foreach (TabItemState state in Enum.GetValues<TabItemState>())
        {
            TabRenderer.DrawTabItem(g, bounds, "TabText", font, image, imageRect, focused: false, state);
            TabRenderer.DrawTabItem(g, bounds, "TabText", font, TextFormatFlags.Default, image, imageRect, focused: true, state);
        }

        Color after = bmp.GetPixel(10, 10);
        after.Should().NotBe(before);
    }

    [WinFormsFact]
    public void DrawTabPage_DoesNotThrow()
    {
        using Bitmap bmp = new(100, 100);
        using Graphics g = Graphics.FromImage(bmp);
        Rectangle bounds = new(0, 0, 100, 100);
        Color before = bmp.GetPixel(10, 10);

        TabRenderer.DrawTabPage(g, bounds);

        Color after = bmp.GetPixel(10, 10);
        after.Should().NotBe(before);
    }
}
