// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Windows.Forms.Metafiles;

namespace System.Windows.Forms.Tests;

public partial class ToolStripLabelTests
{
    [WinFormsFact]
    public void ToolStripLabel_Selected_RendersBackgroundCorrectly_HighContrast()
    {
        using Form form = new();
        using ToolStrip toolStrip = new();
        toolStrip.Renderer = new ToolStripSystemHighContrastRenderer();
        using ToolStripLabel toolStripLabel = new()
        {
            IsLink = true,
            Text = "Test Link"
        };
        toolStrip.Items.Add(toolStripLabel);
        form.Controls.Add(toolStrip);
        toolStripLabel.Select();

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        Rectangle bounds = toolStripLabel.Bounds;
        using PaintEventArgs e = new(emf, bounds);
        toolStripLabel.TestAccessor().Dynamic.OnPaint(e);

        // In high contrast mode, selected items should have a highlight background
        emf.Validate(
           state,
           Validate.Polygon16(
                bounds: null,
                points: null,
                State.Brush(SystemColors.Highlight, BRUSH_STYLE.BS_SOLID)),
           Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON16), 2));
    }

    [WinFormsFact]
    public void ToolStripLabel_IsLink_Selected_RendersBackgroundCorrectly_HighContrast()
    {
        using Form form = new();
        using ToolStrip toolStrip = new();
        toolStrip.Renderer = new ToolStripSystemHighContrastRenderer();
        using ToolStripLabel toolStripLabel = new()
        {
            IsLink = true,
            Text = "Link Text"
        };
        toolStrip.Items.Add(toolStripLabel);
        form.Controls.Add(toolStrip);
        toolStripLabel.Select();

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        Rectangle bounds = toolStripLabel.Bounds;
        using PaintEventArgs e = new(emf, bounds);
        toolStripLabel.TestAccessor().Dynamic.OnPaint(e);

        // When IsLink is true and item is selected in high contrast mode,
        // the background should be filled with highlight color for proper visibility
        emf.Validate(
           state,
           Validate.Polygon16(
                bounds: null,
                points: null,
                State.Brush(SystemColors.Highlight, BRUSH_STYLE.BS_SOLID)),
           Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON16), 2));
    }

    private class ToolStripSystemHighContrastRenderer : ToolStripSystemRenderer
    {
        internal override ToolStripRenderer RendererOverride => HighContrastRenderer;
    }
}
