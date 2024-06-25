// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms.ButtonInternal;

namespace System.Windows.Forms.PropertyGridInternal;

internal sealed partial class DropDownButton : Button
{
    internal class DropDownButtonAdapter : ButtonStandardAdapter
    {
        internal DropDownButtonAdapter(ButtonBase control) : base(control) { }

        private void DDB_Draw3DBorder(PaintEventArgs e, Rectangle r, bool raised)
        {
            if (Control.BackColor != Application.ApplicationColors.Control && SystemInformation.HighContrast)
            {
                if (raised)
                {
                    Color c = ControlPaint.LightLight(Control.BackColor);
                    ControlPaint.DrawBorder(
                        e, r,
                        c, 1, ButtonBorderStyle.Outset,
                        c, 1, ButtonBorderStyle.Outset,
                        c, 2, ButtonBorderStyle.Inset,
                        c, 2, ButtonBorderStyle.Inset);
                }
                else
                {
                    ControlPaint.DrawBorderSimple(e, r, ControlPaint.Dark(Control.BackColor));
                }
            }
            else
            {
                if (raised)
                {
                    Color c = ControlPaint.Light(Control.BackColor);
                    ControlPaint.DrawBorder(
                        e, r,
                        c, 1, ButtonBorderStyle.Solid,
                        c, 1, ButtonBorderStyle.Solid,
                        Control.BackColor, 2, ButtonBorderStyle.Outset,
                        Control.BackColor, 2, ButtonBorderStyle.Outset);

                    Rectangle inside = r;
                    inside.Offset(1, 1);
                    inside.Width -= 3;
                    inside.Height -= 3;
                    c = ControlPaint.LightLight(Control.BackColor);
                    ControlPaint.DrawBorder(
                        e, inside,
                        c, 1, ButtonBorderStyle.Solid,
                        c, 1, ButtonBorderStyle.Solid,
                        c, 1, ButtonBorderStyle.None,
                        c, 1, ButtonBorderStyle.None);
                }
                else
                {
                    ControlPaint.DrawBorderSimple(e, r, ControlPaint.Dark(Control.BackColor));
                }
            }
        }

        internal override void PaintUp(PaintEventArgs pevent, CheckState state)
        {
            base.PaintUp(pevent, state);
            if (!Application.RenderWithVisualStyles)
            {
                DDB_Draw3DBorder(pevent, Control.ClientRectangle, raised: true);
            }
            else
            {
                Color c = (ARGB)SystemColors.Window;
                Rectangle rect = Control.ClientRectangle;
                rect.Inflate(0, -1);
                ControlPaint.DrawBorder(
                    pevent, rect,
                    c, 1, ButtonBorderStyle.None,
                    c, 1, ButtonBorderStyle.None,
                    c, 1, ButtonBorderStyle.Solid,
                    c, 1, ButtonBorderStyle.None);
            }
        }

        internal override void DrawImageCore(Graphics graphics, Image image, Rectangle imageBounds, Point imageStart, LayoutData layout)
        {
            bool isHighContrastHighlighted = !Control.MouseIsDown && IsHighContrastHighlighted();
            Color backgroundColor = isHighContrastHighlighted ? Application.ApplicationColors.Highlight : Control.BackColor;
            if (ControlPaint.IsDark(backgroundColor) && image is Bitmap bitmap)
            {
                using Image invertedImage = ControlPaint.CreateBitmapWithInvertedForeColor(bitmap, Control.BackColor);
                graphics.DrawImage(invertedImage, imageBounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, new ImageAttributes());
            }
            else
            {
                graphics.DrawImage(image, imageBounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, new ImageAttributes());
            }
        }
    }
}
