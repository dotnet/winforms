// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.Internal;

namespace System.Windows.Forms.ButtonInternal
{
    internal abstract class RadioButtonBaseAdapter : CheckableControlBaseAdapter
    {
        internal RadioButtonBaseAdapter(ButtonBase control) : base(control) { }

        protected new RadioButton Control
        {
            get
            {
                return ((RadioButton)base.Control);
            }
        }

        #region Drawing helpers
        protected void DrawCheckFlat(PaintEventArgs e, LayoutData layout, Color checkColor, Color checkBackground, Color checkBorder)
        {
            DrawCheckBackgroundFlat(e, layout.checkBounds, checkBorder, checkBackground);
            DrawCheckOnly(e, layout, checkColor, checkBackground, true);
        }

        protected void DrawCheckBackground3DLite(PaintEventArgs e, Rectangle bounds, Color checkColor, Color checkBackground, ColorData colors, bool disabledColors)
        {
            Graphics g = e.Graphics;

            Color field = checkBackground;
            if (!Control.Enabled && disabledColors)
            {
                field = SystemColors.Control;
            }

            using (Brush fieldBrush = new SolidBrush(field))
            {
                using (Pen dark = new Pen(colors.buttonShadow),
                       light = new Pen(colors.buttonFace),
                       lightlight = new Pen(colors.highlight))
                {

                    bounds.Width--;
                    bounds.Height--;
                    // fall a little short of SW, NW, NE, SE because corners come out nasty
                    g.DrawPie(dark, bounds, (float)(135 + 1), (float)(90 - 2));
                    g.DrawPie(dark, bounds, (float)(225 + 1), (float)(90 - 2));
                    g.DrawPie(lightlight, bounds, (float)(315 + 1), (float)(90 - 2));
                    g.DrawPie(lightlight, bounds, (float)(45 + 1), (float)(90 - 2));
                    bounds.Inflate(-1, -1);
                    g.FillEllipse(fieldBrush, bounds);
                    g.DrawEllipse(light, bounds);
                }
            }
        }

        protected void DrawCheckBackgroundFlat(PaintEventArgs e, Rectangle bounds, Color borderColor, Color checkBackground)
        {
            Color field = checkBackground;
            Color border = borderColor;

            if (!Control.Enabled)
            {
                // if we are not in HighContrast mode OR we opted into the legacy behavior
                if (!SystemInformation.HighContrast)
                {
                    border = ControlPaint.ContrastControlDark;
                }
                // otherwise we are in HighContrast mode
                field = SystemColors.Control;
            }

            double scale = GetDpiScaleRatio(e.Graphics);

            using (WindowsGraphics wg = WindowsGraphics.FromGraphics(e.Graphics))
            {
                using (WindowsPen borderPen = new WindowsPen(wg.DeviceContext, border))
                {
                    using (WindowsBrush fieldBrush = new WindowsSolidBrush(wg.DeviceContext, field))
                    {
                        // In high DPI mode when we draw ellipse as three rectantles,
                        // the quality of ellipse is poor. Draw it directly as ellipse
                        if (scale > 1.1)
                        {
                            bounds.Width--;
                            bounds.Height--;
                            wg.DrawAndFillEllipse(borderPen, fieldBrush, bounds);
                            bounds.Inflate(-1, -1);
                        }
                        else
                        {
                            DrawAndFillEllipse(wg, borderPen, fieldBrush, bounds);
                        }
                    }
                }
            }
        }

        // Helper method to overcome the poor GDI ellipse drawing routine		
        private static void DrawAndFillEllipse(WindowsGraphics wg, WindowsPen borderPen, WindowsBrush fieldBrush, Rectangle bounds)
        {
            Debug.Assert(wg != null, "Calling DrawAndFillEllipse with null wg");
            if (wg == null)
            {
                return;
            }

            wg.FillRectangle(fieldBrush, new Rectangle(bounds.X + 2, bounds.Y + 2, 8, 8));
            wg.FillRectangle(fieldBrush, new Rectangle(bounds.X + 4, bounds.Y + 1, 4, 10));
            wg.FillRectangle(fieldBrush, new Rectangle(bounds.X + 1, bounds.Y + 4, 10, 4));

            wg.DrawLine(borderPen, new Point(bounds.X + 4, bounds.Y + 0), new Point(bounds.X + 8, bounds.Y + 0));
            wg.DrawLine(borderPen, new Point(bounds.X + 4, bounds.Y + 11), new Point(bounds.X + 8, bounds.Y + 11));

            wg.DrawLine(borderPen, new Point(bounds.X + 2, bounds.Y + 1), new Point(bounds.X + 4, bounds.Y + 1));
            wg.DrawLine(borderPen, new Point(bounds.X + 8, bounds.Y + 1), new Point(bounds.X + 10, bounds.Y + 1));

            wg.DrawLine(borderPen, new Point(bounds.X + 2, bounds.Y + 10), new Point(bounds.X + 4, bounds.Y + 10));
            wg.DrawLine(borderPen, new Point(bounds.X + 8, bounds.Y + 10), new Point(bounds.X + 10, bounds.Y + 10));

            wg.DrawLine(borderPen, new Point(bounds.X + 0, bounds.Y + 4), new Point(bounds.X + 0, bounds.Y + 8));
            wg.DrawLine(borderPen, new Point(bounds.X + 11, bounds.Y + 4), new Point(bounds.X + 11, bounds.Y + 8));

            wg.DrawLine(borderPen, new Point(bounds.X + 1, bounds.Y + 2), new Point(bounds.X + 1, bounds.Y + 4));
            wg.DrawLine(borderPen, new Point(bounds.X + 1, bounds.Y + 8), new Point(bounds.X + 1, bounds.Y + 10));

            wg.DrawLine(borderPen, new Point(bounds.X + 10, bounds.Y + 2), new Point(bounds.X + 10, bounds.Y + 4));
            wg.DrawLine(borderPen, new Point(bounds.X + 10, bounds.Y + 8), new Point(bounds.X + 10, bounds.Y + 10));
        }

        private static int GetScaledNumber(int n, double scale)
        {
            return (int)(n * scale);
        }

        protected void DrawCheckOnly(PaintEventArgs e, LayoutData layout, Color checkColor, Color checkBackground, bool disabledColors)
        {
            // check
            //
            if (Control.Checked)
            {
                if (!Control.Enabled && disabledColors)
                {
                    checkColor = SystemColors.ControlDark;
                }

                double scale = GetDpiScaleRatio(e.Graphics);
                using (WindowsGraphics wg = WindowsGraphics.FromGraphics(e.Graphics))
                {
                    using (WindowsBrush brush = new WindowsSolidBrush(wg.DeviceContext, checkColor))
                    {
                        // circle drawing doesn't work at this size
                        int offset = 5;
                        Rectangle vCross = new Rectangle(layout.checkBounds.X + GetScaledNumber(offset, scale), layout.checkBounds.Y + GetScaledNumber(offset - 1, scale), GetScaledNumber(2, scale), GetScaledNumber(4, scale));
                        wg.FillRectangle(brush, vCross);
                        Rectangle hCross = new Rectangle(layout.checkBounds.X + GetScaledNumber(offset - 1, scale), layout.checkBounds.Y + GetScaledNumber(offset, scale), GetScaledNumber(4, scale), GetScaledNumber(2, scale));
                        wg.FillRectangle(brush, hCross);
                    }
                }
            }
        }

        protected ButtonState GetState()
        {
            ButtonState style = (ButtonState)0;

            if (Control.Checked)
            {
                style |= ButtonState.Checked;
            }
            else
            {
                style |= ButtonState.Normal;
            }

            if (!Control.Enabled)
            {
                style |= ButtonState.Inactive;
            }

            if (Control.MouseIsDown)
            {
                style |= ButtonState.Pushed;
            }

            return style;

        }

        protected void DrawCheckBox(PaintEventArgs e, LayoutData layout)
        {
            Graphics g = e.Graphics;

            Rectangle check = layout.checkBounds;
            if (!Application.RenderWithVisualStyles)
            {
                check.X--;      // compensate for Windows drawing slightly offset to right
            }

            ButtonState style = GetState();

            if (Application.RenderWithVisualStyles)
            {
                RadioButtonRenderer.DrawRadioButton(g, new Point(check.Left, check.Top), RadioButtonRenderer.ConvertFromButtonState(style, Control.MouseIsOver), Control.HandleInternal);
            }
            else
            {
                ControlPaint.DrawRadioButton(g, check, style);
            }
        }

        #endregion

        protected void AdjustFocusRectangle(LayoutData layout)
        {
            if (string.IsNullOrEmpty(Control.Text))
            {
                // When a RadioButton has no text, AutoSize sets the size to zero
                // and thus there's no place around which to draw the focus rectangle.
                // So, when AutoSize == true we want the focus rectangle to be rendered around the circle area.
                // Otherwise, it should encircle all the available space next to the box (like it's done in WPF and ComCtl32).
                layout.focus = Control.AutoSize ? layout.checkBounds : layout.field;
            }
        }

        internal override LayoutOptions CommonLayout()
        {
            LayoutOptions layout = base.CommonLayout();
            layout.checkAlign = Control.CheckAlign;

            return layout;
        }

    }
}
