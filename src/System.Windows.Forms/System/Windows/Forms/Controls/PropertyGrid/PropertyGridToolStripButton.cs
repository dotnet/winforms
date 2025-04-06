// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

internal partial class PropertyGridToolStripButton : ToolStripButton
{
    private readonly PropertyGrid _owningPropertyGrid;

    private readonly bool _selectItemEnabled;

    internal PropertyGridToolStripButton(PropertyGrid propertyGrid, bool selectItemEnabled)
        : base()
    {
        _owningPropertyGrid = propertyGrid;
        _selectItemEnabled = selectItemEnabled;
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new PropertyGridToolStripButtonAccessibleObject(this);

    private void DrawDashedBorer(Graphics graphics)
    {
        var bounds = ClientBounds;

        // This is necessary so that when HighContrast is off, the size of the dotted borders coincides with the
        // size of the button background. For normal mode we use the "ToolStripSystemRenderer.RenderItemInternal"
        // method which calls the "VisualStyleRenderer.DrawBackground" method for drawing. For high contrast mode
        // we use the "ToolStripHighContrastRenderer.OnRenderButtonBackground" method which calls the
        // "Graphics.DrawRectangle" method for drawing.
        bounds.Height -= 1;

        // We support only one type of settings for all borders since it is consistent with the behavior of the same controls
        ControlPaint.DrawBorder(graphics, bounds,
            leftColor: Color.Black, leftWidth: 1, leftStyle: ButtonBorderStyle.Dashed,
            topColor: Color.Black, topWidth: 1, topStyle: ButtonBorderStyle.Dashed,
            rightColor: Color.Black, rightWidth: 1, rightStyle: ButtonBorderStyle.Dashed,
            bottomColor: Color.Black, bottomWidth: 1, bottomStyle: ButtonBorderStyle.Dashed);
    }

    private void DrawHightContrastDashedBorer(Graphics graphics)
    {
        Rectangle bounds = ClientBounds;
        float[] dashValues = [2, 2];
        int penWidth = 2;

        Pen focusPen1 = new(SystemColors.ControlText, penWidth)
        {
            DashPattern = dashValues
        };

        Pen focusPen2 = new(SystemColors.Control, penWidth)
        {
            DashPattern = dashValues,
            DashOffset = 2
        };

        graphics.DrawRectangle(focusPen1, bounds);
        graphics.DrawRectangle(focusPen2, bounds);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (!Selected)
        {
            return;
        }

        if (SystemInformation.HighContrast)
        {
            DrawHightContrastDashedBorer(e.Graphics);
        }
        else
        {
            DrawDashedBorer(e.Graphics);
        }
    }
}
