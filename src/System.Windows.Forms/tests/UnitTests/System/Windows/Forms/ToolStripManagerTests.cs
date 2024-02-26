// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests.System.Windows.Forms;

public class ToolStripManagerTests
{
    [Fact]
    public void DefaultFont_Should_Return_Default_Font()
    {
        // Arrange

        // Act
        Font defaultFont = ToolStripManager.DefaultFont;

        // Assert
        defaultFont.Should().NotBeNull();
        // Add more assertions for the properties of the defaultFont if needed
    }

    [Fact]
    public void CurrentDpi_Should_Set_And_Get_Current_Dpi()
    {
        // Arrange
        int dpi = 96;

        // Act
        ToolStripManager.CurrentDpi = dpi;
        int currentDpi = ToolStripManager.CurrentDpi;

        // Assert
        currentDpi.Should().Be(dpi);
    }

    [Fact]
    public void ToolStrips_Should_Return_ToolStrips_Collection()
    {
        // Arrange

        // Act
        WeakRefCollection<ToolStrip> toolStrips = ToolStripManager.ToolStrips;

        // Assert
        toolStrips.Should().NotBeNull();
        // Add more assertions for the properties or methods of the toolStrips collection if needed
    }

    [Fact]
    public void Renderer_Should_Set_And_Get_Renderer()
    {
        // Arrange
        ToolStripRenderer renderer = new ToolStripProfessionalRenderer();

        // Act
        ToolStripManager.Renderer = renderer;
        ToolStripRenderer currentRenderer = ToolStripManager.Renderer;

        // Assert
        currentRenderer.Should().Be(renderer);
    }

    [Fact]
    public void RenderMode_Should_Set_And_Get_RenderMode()
    {
        // Arrange
        ToolStripManagerRenderMode renderMode = ToolStripManagerRenderMode.System;

        // Act
        ToolStripManager.RenderMode = renderMode;
        ToolStripManagerRenderMode currentRenderMode = ToolStripManager.RenderMode;

        // Assert
        currentRenderMode.Should().Be(renderMode);
    }

    [Fact]
    public void VisualStylesEnabled_Should_Set_And_Get_VisualStylesEnabled()
    {
        // Arrange
        bool visualStylesEnabled = true;

        // Act
        ToolStripManager.VisualStylesEnabled = visualStylesEnabled;
        bool currentVisualStylesEnabled = ToolStripManager.VisualStylesEnabled;

        // Assert
        currentVisualStylesEnabled.Should().Be(visualStylesEnabled);
    }

    // Add more test methods for other members of the ToolStripManager class if needed
}
