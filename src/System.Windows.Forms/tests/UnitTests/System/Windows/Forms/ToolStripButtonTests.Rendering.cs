// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Metafiles;

namespace System.Windows.Forms.Tests;

public partial class ToolStripButtonTests
{
    [WinFormsFact]
    public void ToolStripButton_RendersTextCorrectly()
    {
        using Form form = new();
        using ToolStrip toolStrip = new();
        using ToolStripButton toolStripButton = new()
        {
            Text = "Hello"
        };
        toolStrip.Items.Add(toolStripButton);
        form.Controls.Add(toolStrip);

        using EmfScope emf = new();

        DeviceContextState state = new(emf);

        Rectangle bounds = toolStripButton.Bounds;

        using PaintEventArgs e = new(emf, bounds);
        toolStripButton.TestAccessor().Dynamic.OnPaint(e);

        emf.Validate(
            state,
             Validate.TextOut("Hello"));
    }

    [WinFormsFact]
    public void ToolStripButton_RendersBackgroundCorrectly()
    {
        using Form form = new();
        using ToolStrip toolStrip = new();
        using ToolStripButton toolStripButton = new()
        {
            BackColor = Color.Blue,
        };
        toolStrip.Items.Add(toolStripButton);
        form.Controls.Add(toolStrip);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        Rectangle bounds = toolStripButton.Bounds;
        using PaintEventArgs e = new(emf, bounds);
        toolStripButton.TestAccessor().Dynamic.OnPaint(e);

        emf.Validate(
           state,
           Validate.Polygon16(
                bounds: null,
                points: null,
                State.Brush(Color.Blue, BRUSH_STYLE.BS_SOLID)));
    }

    [WinFormsFact]
    public void ToolStripButton_Selected_RendersBackgroundCorrectly_HighContrast()
    {
        using Form form = new();
        using ToolStrip toolStrip = new();
        toolStrip.Renderer = new ToolStripSystemHighContrastRenderer();
        using ToolStripButton toolStripButton = new();
        toolStrip.Items.Add(toolStripButton);
        form.Controls.Add(toolStrip);
        toolStripButton.Select();

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        Rectangle bounds = toolStripButton.Bounds;
        using PaintEventArgs e = new(emf, bounds);
        toolStripButton.TestAccessor().Dynamic.OnPaint(e);

        emf.Validate(
           state,
           Validate.Polygon16(
                bounds: null,
                points: null,
                State.Brush(SystemColors.Highlight, BRUSH_STYLE.BS_SOLID)),
           Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON16), 2));
    }

    [WinFormsFact]
    public void ToolStripButton_Indeterminate_RendersBackgroundCorrectly_HighContrast()
    {
        using Form form = new();
        using ToolStrip toolStrip = new();
        toolStrip.Renderer = new ToolStripSystemHighContrastRenderer();
        using ToolStripButton toolStripButton = new();
        toolStrip.Items.Add(toolStripButton);
        form.Controls.Add(toolStrip);
        toolStripButton.CheckState = CheckState.Indeterminate;

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        Rectangle bounds = toolStripButton.Bounds;
        using PaintEventArgs e = new(emf, bounds);
        toolStripButton.TestAccessor().Dynamic.OnPaint(e);

        emf.Validate(
           state,
           Validate.Polygon16(
                bounds: null,
                points: null,
                State.Brush(SystemColors.Highlight, BRUSH_STYLE.BS_SOLID)),
           Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYGON16));
    }

    [WinFormsFact]
    public void ToolStripButton_DropDownButton_Selected_RendersBackgroundCorrectly_HighContrast()
    {
        using Form form = new();
        using ToolStrip toolStrip = new();
        toolStrip.Renderer = new ToolStripSystemHighContrastRenderer();
        using ToolStripDropDownButton toolStripDropDownButton = new();
        toolStrip.Items.Add(toolStripDropDownButton);
        form.Controls.Add(toolStrip);
        toolStripDropDownButton.Select();

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        Rectangle bounds = toolStripDropDownButton.Bounds;
        using PaintEventArgs e = new(emf, bounds);
        toolStripDropDownButton.TestAccessor().Dynamic.OnPaint(e);

        emf.Validate(
           state,
           Validate.Polygon16(
                bounds: null,
                points: null,
                State.Brush(SystemColors.Highlight, BRUSH_STYLE.BS_SOLID)),
           Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON16), 2),
           Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYGON16), 1));
    }

    [WinFormsFact]
    public void ToolStripButton_SplitButton_Selected_RendersBackgroundCorrectly_HighContrast()
    {
        using Form form = new();
        using ToolStrip toolStrip = new();
        toolStrip.Renderer = new ToolStripSystemHighContrastRenderer();
        using ToolStripSplitButton toolStripDropDownButton = new();
        toolStrip.Items.Add(toolStripDropDownButton);
        form.Controls.Add(toolStrip);
        toolStripDropDownButton.Select();

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        Rectangle bounds = toolStripDropDownButton.Bounds;
        using PaintEventArgs e = new(emf, bounds);
        toolStripDropDownButton.TestAccessor().Dynamic.OnPaint(e);

        emf.Validate(
           state,
           Validate.Polygon16(
                bounds: null,
                points: null,
                State.Brush(SystemColors.Highlight, BRUSH_STYLE.BS_SOLID)),
           Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYGON16),
           Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON16), 2),
           Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYGON16));
    }

    private class ToolStripSystemHighContrastRenderer : ToolStripSystemRenderer
    {
        internal override ToolStripRenderer RendererOverride => HighContrastRenderer;
    }
}
