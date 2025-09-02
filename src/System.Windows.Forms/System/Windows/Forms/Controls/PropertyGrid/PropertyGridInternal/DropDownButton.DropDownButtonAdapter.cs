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
            if (ButtonBaseControl.BackColor != SystemColors.Control && SystemInformation.HighContrast)
            {
                if (raised)
                {
                    Color c = ControlPaint.LightLight(ButtonBaseControl.BackColor);
                    ControlPaint.DrawBorder(
                        e, r,
                        c, 1, ButtonBorderStyle.Outset,
                        c, 1, ButtonBorderStyle.Outset,
                        c, 2, ButtonBorderStyle.Inset,
                        c, 2, ButtonBorderStyle.Inset);
                }
                else
                {
                    ControlPaint.DrawBorderSimple(e, r, ControlPaint.Dark(ButtonBaseControl.BackColor));
                }
            }
            else
            {
                if (raised)
                {
                    Color c = ControlPaint.Light(ButtonBaseControl.BackColor);
                    ControlPaint.DrawBorder(
                        e, r,
                        c, 1, ButtonBorderStyle.Solid,
                        c, 1, ButtonBorderStyle.Solid,
                        ButtonBaseControl.BackColor, 2, ButtonBorderStyle.Outset,
                        ButtonBaseControl.BackColor, 2, ButtonBorderStyle.Outset);

                    Rectangle inside = r;
                    inside.Offset(1, 1);
                    inside.Width -= 3;
                    inside.Height -= 3;
                    c = ControlPaint.LightLight(ButtonBaseControl.BackColor);
                    ControlPaint.DrawBorder(
                        e, inside,
                        c, 1, ButtonBorderStyle.Solid,
                        c, 1, ButtonBorderStyle.Solid,
                        c, 1, ButtonBorderStyle.None,
                        c, 1, ButtonBorderStyle.None);
                }
                else
                {
                    ControlPaint.DrawBorderSimple(e, r, ControlPaint.Dark(ButtonBaseControl.BackColor));
                }
            }
        }

        internal override void PaintUp(PaintEventArgs pevent, CheckState state)
        {
            base.PaintUp(pevent, state);

            if (!Application.RenderWithVisualStyles || Application.IsDarkModeEnabled)
            {
                DDB_Draw3DBorder(pevent, ButtonBaseControl.ClientRectangle, raised: true);
            }
            else
            {
                Color c = (ARGB)SystemColors.Window;
                Rectangle rect = ButtonBaseControl.ClientRectangle;
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
            bool isHighContrastHighlighted = !ButtonBaseControl.MouseIsDown && IsHighContrastHighlighted();
            Color backgroundColor = isHighContrastHighlighted ? SystemColors.Highlight : ButtonBaseControl.BackColor;
            if (ControlPaint.IsDark(backgroundColor) && image is Bitmap bitmap)
            {
                using Image invertedImage = ControlPaint.CreateBitmapWithInvertedForeColor(bitmap, ButtonBaseControl.BackColor);
                graphics.DrawImage(invertedImage, imageBounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, new ImageAttributes());
            }
            else
            {
                graphics.DrawImage(image, imageBounds, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, new ImageAttributes());
            }
        }
    }
}
