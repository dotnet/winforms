// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.Design;

/// <summary>
///  This class handles all design time behavior for the group box class. Group
///  boxes may contain sub-components and therefore use the frame designer.
/// </summary>
internal class PictureBoxDesigner : ControlDesigner
{
    private DesignerActionListCollection? _actionLists;

    public PictureBoxDesigner()
    {
        AutoResizeHandles = true;
    }

    /// <summary>
    ///  This draws a nice border around our pictureBox. We need
    ///  this because the pictureBox can have no border and you can't
    ///  tell where it is.
    /// </summary>
    /// <internalonly/>
    private void DrawBorder(Graphics graphics)
    {
        Control ctl = Control;
        Rectangle rc = ctl.ClientRectangle;
        Color penColor;

        // Black or white pen?  Depends on the color of the control.
        //
        if (ctl.BackColor.GetBrightness() < .5)
        {
            penColor = ControlPaint.Light(ctl.BackColor);
        }
        else
        {
            penColor = ControlPaint.Dark(ctl.BackColor);
            ;
        }

        Pen pen = new(penColor)
        {
            DashStyle = DashStyle.Dash
        };

        rc.Width--;
        rc.Height--;
        graphics.DrawRectangle(pen, rc);

        pen.Dispose();
    }

    /// <summary>
    ///  Overrides our base class. Here we check to see if there
    ///  is no border on the pictureBox. If not, we draw one so that
    ///  the pictureBox shape is visible at design time.
    /// </summary>
    protected override void OnPaintAdornments(PaintEventArgs pe)
    {
        PictureBox pictureBox = (PictureBox)Component;

        if (pictureBox.BorderStyle == BorderStyle.None)
        {
            DrawBorder(pe.Graphics);
        }

        base.OnPaintAdornments(pe);
    }

    /// <summary>
    ///  Retrieves a set of rules concerning the movement capabilities of a component.
    ///  This should be one or more flags from the SelectionRules class. If no designer
    ///  provides rules for a component, the component will not get any UI services.
    /// </summary>
    public override SelectionRules SelectionRules
    {
        get
        {
            SelectionRules rules = base.SelectionRules;
            PictureBoxSizeMode sizeMode = PictureBoxSizeMode.Normal;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(Component);

            if (properties.TryGetPropertyDescriptorValue(
                "SizeMode",
                Component,
                ref sizeMode))
            {
                if (sizeMode == PictureBoxSizeMode.AutoSize)
                {
                    rules &= ~SelectionRules.AllSizeable;
                }
            }

            return rules;
        }
    }

    public override DesignerActionListCollection ActionLists
    {
        get
        {
            if (_actionLists is null)
            {
                _actionLists = new DesignerActionListCollection();
                _actionLists.Add(new PictureBoxActionList(this));
            }

            return _actionLists;
        }
    }
}
