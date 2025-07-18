// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripProfessionalLowResolutionRendererTests
{
    private readonly ToolStripProfessionalLowResolutionRenderer _toolStripProfessionalLowResolutionRenderer = new();

    [Fact]
    public void Ctor_Default_Success()
    {
        _toolStripProfessionalLowResolutionRenderer.Should().NotBeNull();
        _toolStripProfessionalLowResolutionRenderer.Should().BeOfType<ToolStripProfessionalLowResolutionRenderer>();
        _toolStripProfessionalLowResolutionRenderer.Should().BeAssignableTo<ToolStripProfessionalRenderer>();
    }

    [WinFormsFact]
    public void OnRenderToolStripBackground_DoesNotThrow_ForNonDropDown()
    {
        using ToolStrip toolStrip = new();
        using Bitmap bmp = new(10, 10);
        using Graphics g = Graphics.FromImage(bmp);
        ToolStripRenderEventArgs args = new(g, toolStrip);

        Action action = () => _toolStripProfessionalLowResolutionRenderer.TestAccessor().Dynamic.OnRenderToolStripBackground(args);

        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void OnRenderToolStripBackground_CallsBase_ForDropDown()
    {
        using ToolStripDropDown dropDown = new();
        using Bitmap bmp = new(10, 10);
        using Graphics g = Graphics.FromImage(bmp);
        ToolStripRenderEventArgs args = new(g, dropDown);

        Action action = () => _toolStripProfessionalLowResolutionRenderer.TestAccessor().Dynamic.OnRenderToolStripBackground(args);

        action.Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(typeof(MenuStrip))]
    [InlineData(typeof(StatusStrip))]
    [InlineData(typeof(ToolStripDropDown))]
    [InlineData(typeof(ToolStrip))]
    public void OnRenderToolStripBorder_DoesNotThrow_ForAllToolStrips(Type toolStripType)
    {
        using ToolStrip toolStrip = (ToolStrip)Activator.CreateInstance(toolStripType)!;
        using Bitmap bmp = new(10, 10);
        using Graphics g = Graphics.FromImage(bmp);
        ToolStripRenderEventArgs args = new(g, toolStrip);

        Action action = () => _toolStripProfessionalLowResolutionRenderer.TestAccessor().Dynamic.OnRenderToolStripBorder(args);

        action.Should().NotThrow();
    }

    [Fact]
    public void RendererOverride_AlwaysReturnsNull() =>
        _toolStripProfessionalLowResolutionRenderer.RendererOverride.Should().BeNull();
}
