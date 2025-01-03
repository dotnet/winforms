// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests;

public class ToolStripSystemRendererTests : IDisposable
{
    private readonly Bitmap _bitmap;
    private readonly Graphics _graphics;
    private readonly ToolStripSystemRenderer _toolStripSystemRenderer;

    public ToolStripSystemRendererTests()
    {
        _bitmap = new(100, 100);
        _graphics = Graphics.FromImage(_bitmap);
        _toolStripSystemRenderer = new();
    }

    public void Dispose()
    {
        _graphics.Dispose();
        _bitmap.Dispose();
    }

    [WinFormsFact]
    public void FillBackground_FillsRectangleWithBackColor()
    {
        Rectangle bounds = new(10, 10, 50, 50);
        Color backColor = Color.Red;

        _toolStripSystemRenderer.TestAccessor().Dynamic.FillBackground(_graphics, bounds, backColor);

        for (int x = bounds.Left; x < bounds.Right; x++)
        {
            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                Color actualColor = _bitmap.GetPixel(x, y);
                actualColor.ToArgb().Should().Be(backColor.ToArgb());
            }
        }
    }

    [WinFormsFact]
    public void FillBackground_EmptyRectangle_DoesNotThrow()
    {
        Rectangle bounds = Rectangle.Empty;
        Color backColor = Color.Red;

        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.FillBackground(_graphics, bounds, backColor);
        action.Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(null, ToolBarState.Normal)]
    [InlineData(false, ToolBarState.Disabled)]
    public void GetSplitButtonDropDownItemState_ReturnsExpectedState(bool? isEnabled, ToolBarState expectedState)
    {
        ToolStripSplitButton? item = isEnabled.HasValue ? new ToolStripSplitButton { Enabled = isEnabled.Value } : null;
        int state = _toolStripSystemRenderer.TestAccessor().Dynamic.GetSplitButtonDropDownItemState(item);
        state.Should().Be((int)expectedState);
    }

    [WinFormsTheory]
    [InlineData(null, ToolBarState.Normal)]
    [InlineData(false, ToolBarState.Disabled)]
    public void GetSplitButtonItemState_ReturnsExpectedState(bool? isEnabled, ToolBarState expectedState)
    {
        ToolStripSplitButton? item = isEnabled.HasValue ? new ToolStripSplitButton { Enabled = isEnabled.Value } : null;
        int state = _toolStripSystemRenderer.TestAccessor().Dynamic.GetSplitButtonItemState(item);
        state.Should().Be((int)expectedState);
    }

    [WinFormsTheory]
    [InlineData(null, ToolBarState.Normal)]
    [InlineData(false, ToolBarState.Disabled)]
    public void GetSplitButtonToolBarState_ReturnsExpectedState(bool? isEnabled, ToolBarState expectedState)
    {
        ToolStripSplitButton? button = isEnabled.HasValue ? new ToolStripSplitButton { Enabled = isEnabled.Value } : null;
        ToolBarState state = _toolStripSystemRenderer.TestAccessor().Dynamic.GetSplitButtonToolBarState(button, false);
        state.Should().Be(expectedState);
    }

    [WinFormsFact]
    public void OnRenderItemBackground_ValidArgs_DoesNotThrow()
    {
        using ToolStripButton item = new();
        ToolStripItemRenderEventArgs args = new(_graphics, item);

        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderItemBackground(args);
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnRenderItemBackground_NullArgs_DoesNotThrow()
    {
        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderItemBackground(null);
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnRenderImageMargin_ValidArgs_DoesNotThrow()
    {
        using ToolStrip toolStrip = new();
        ToolStripRenderEventArgs args = new(_graphics, toolStrip);

        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderImageMargin(args);
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnRenderImageMargin_NullArgs_DoesNotThrow()
    {
        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderImageMargin(null);
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnRenderOverflowButtonBackground_ValidArgs_DoesNotThrow()
    {
        using ToolStripButton item = new();
        ToolStripItemRenderEventArgs args = new(_graphics, item);

        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderOverflowButtonBackground(args);
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnRenderOverflowButtonBackground_VisualStylesDisabled_RendersCorrectly()
    {
        using ToolStripButton item = new();
        ToolStripItemRenderEventArgs args = new(_graphics, item);

        ToolStripManager.VisualStylesEnabled = false;

        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderOverflowButtonBackground(args);
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnRenderMenuItemBackground_ValidArgs_DoesNotThrow()
    {
        using ToolStripMenuItem item = new();
        ToolStripItemRenderEventArgs args = new(_graphics, item);

        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderMenuItemBackground(args);
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnRenderMenuItemBackground_WithBackgroundImage_RendersCorrectly()
    {
        using ToolStripMenuItem item = new() { BackgroundImage = new Bitmap(10, 10) };
        ToolStripItemRenderEventArgs args = new(_graphics, item);

        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderMenuItemBackground(args);
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnRenderMenuItemBackground_WithoutBackgroundImage_RendersCorrectly()
    {
        using ToolStripMenuItem item = new();
        ToolStripItemRenderEventArgs args = new(_graphics, item);

        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderMenuItemBackground(args);
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnRenderToolStripStatusLabelBackground_ValidArgs_DoesNotThrow()
    {
        using ToolStripMenuItem item = new();
        ToolStripItemRenderEventArgs args = new(_graphics, item);

        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderToolStripStatusLabelBackground(args);
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(Border3DStyle.Raised)]
    [InlineData(Border3DStyle.Flat)]
    public void OnRenderToolStripStatusLabelBackground_RendersCorrectly(Border3DStyle borderStyle)
    {
        using ToolStripStatusLabel item = new() { BorderStyle = borderStyle };
        ToolStripItemRenderEventArgs args = new(_graphics, item);

        Action action = () => _toolStripSystemRenderer.TestAccessor().Dynamic.OnRenderToolStripStatusLabelBackground(args);
        action.Should().NotThrow();
    }
}
