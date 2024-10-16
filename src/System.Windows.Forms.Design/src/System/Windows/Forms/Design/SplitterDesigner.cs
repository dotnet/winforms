// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class draws a visible border on the splitter if it doesn't have a border
///  so the user knows where the boundaries of the splitter lie.
/// </summary>
internal class SplitterDesigner : ControlDesigner
{
    public SplitterDesigner()
    {
        AutoResizeHandles = true;
    }

    /// <summary>
    ///  This draws a nice border around our panel. We need this because the panel can have no border and you can't tell where it is.
    /// </summary>
    private void DrawBorder(Graphics graphics)
    {
        Control control = Control;
        Rectangle rectangle = control.ClientRectangle;
        rectangle.Width--;
        rectangle.Height--;

        // Black or white pen?  Depends on the color of the control.
        Color penColor = control.BackColor.GetBrightness() < .5 ? Color.White : Color.Black;

        using Pen pen = new(penColor);
        pen.DashStyle = DashStyle.Dash;
        graphics.DrawRectangle(pen, rectangle);
    }

    /// <summary>
    ///  Here we check to see if there is no border on the panel.
    ///  If not, we draw one so that the panel shape is visible at design time.
    /// </summary>
    protected override void OnPaintAdornments(PaintEventArgs pe)
    {
        base.OnPaintAdornments(pe);

        if (((Splitter)Component).BorderStyle == BorderStyle.None)
        {
            DrawBorder(pe.Graphics);
        }
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvokeCore.WM_WINDOWPOSCHANGED:
                // Really only care about window size changing
                Control.Invalidate();
                break;
        }

        base.WndProc(ref m);
    }
}
