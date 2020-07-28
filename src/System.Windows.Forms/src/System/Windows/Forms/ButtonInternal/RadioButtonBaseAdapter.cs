// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms.ButtonInternal
{
    internal abstract class RadioButtonBaseAdapter : CheckableControlBaseAdapter
    {
        internal RadioButtonBaseAdapter(ButtonBase control) : base(control) { }

        protected new RadioButton Control => ((RadioButton)base.Control);

        protected void DrawCheckFlat(
            PaintEventArgs e,
            LayoutData layout,
            Color checkColor,
            Color checkBackground,
            Color checkBorder)
        {
            DrawCheckBackgroundFlat(e, layout.checkBounds, checkBorder, checkBackground);
            DrawCheckOnly(e, layout, checkColor, checkBackground, true);
        }

        protected void DrawCheckBackground3DLite(
            PaintEventArgs e,
            Rectangle bounds,
            Color checkBackground,
            ColorData colors,
            bool disabledColors)
        {
            Graphics g = e.GraphicsInternal;

            Color field = checkBackground;
            if (!Control.Enabled && disabledColors)
            {
                field = SystemColors.Control;
            }

            using var fieldBrush = field.GetCachedSolidBrushScope();
            using var dark = colors.buttonShadow.GetCachedPenScope();
            using var light = colors.buttonFace.GetCachedPenScope();
            using var lightlight = colors.highlight.GetCachedPenScope();

            bounds.Width--;
            bounds.Height--;

            // Fall a little short of SW, NW, NE, SE because corners come out nasty
            g.DrawPie(dark, bounds, 135 + 1, 90 - 2);
            g.DrawPie(dark, bounds, 225 + 1, 90 - 2);
            g.DrawPie(lightlight, bounds, 315 + 1, 90 - 2);
            g.DrawPie(lightlight, bounds, 45 + 1, 90 - 2);
            bounds.Inflate(-1, -1);
            g.FillEllipse(fieldBrush, bounds);
            g.DrawEllipse(light, bounds);
        }

        protected void DrawCheckBackgroundFlat(PaintEventArgs e, Rectangle bounds, Color borderColor, Color checkBackground)
        {
            Color field = checkBackground;
            Color border = borderColor;

            if (!Control.Enabled)
            {
                // If we are not in HighContrast mode OR we opted into the legacy behavior
                if (!SystemInformation.HighContrast)
                {
                    border = ControlPaint.ContrastControlDark;
                }

                // Otherwise we are in HighContrast mode
                field = SystemColors.Control;
            }

            double scale = GetDpiScaleRatio();

            using var hdc = new DeviceContextHdcScope(e);
            using var borderPen = new Gdi32.CreatePenScope(border);
            using var fieldBrush = new Gdi32.CreateBrushScope(field);

            if (scale > 1.1)
            {
                // In high DPI mode when we draw an ellipse as three rectangles, the quality of ellipse is poor. Draw
                // it directly as an ellipse.
                bounds.Width--;
                bounds.Height--;
                hdc.DrawAndFillEllipse(borderPen, fieldBrush, bounds);
                bounds.Inflate(-1, -1);
            }
            else
            {
                DrawAndFillEllipse(hdc, borderPen, fieldBrush, bounds);
            }
        }

        // Helper method to overcome the poor GDI ellipse drawing routine
        private static void DrawAndFillEllipse(Gdi32.HDC hdc, Gdi32.HPEN borderPen, Gdi32.HBRUSH fieldBrush, Rectangle bounds)
        {
            Debug.Assert(!hdc.IsNull, "Calling DrawAndFillEllipse with null wg");
            if (hdc.IsNull)
            {
                return;
            }

            hdc.FillRectangle(fieldBrush, new Rectangle(bounds.X + 2, bounds.Y + 2, 8, 8));
            hdc.FillRectangle(fieldBrush, new Rectangle(bounds.X + 4, bounds.Y + 1, 4, 10));
            hdc.FillRectangle(fieldBrush, new Rectangle(bounds.X + 1, bounds.Y + 4, 10, 4));

            hdc.DrawLine(borderPen, new Point(bounds.X + 4, bounds.Y + 0), new Point(bounds.X + 8, bounds.Y + 0));
            hdc.DrawLine(borderPen, new Point(bounds.X + 4, bounds.Y + 11), new Point(bounds.X + 8, bounds.Y + 11));

            hdc.DrawLine(borderPen, new Point(bounds.X + 2, bounds.Y + 1), new Point(bounds.X + 4, bounds.Y + 1));
            hdc.DrawLine(borderPen, new Point(bounds.X + 8, bounds.Y + 1), new Point(bounds.X + 10, bounds.Y + 1));

            hdc.DrawLine(borderPen, new Point(bounds.X + 2, bounds.Y + 10), new Point(bounds.X + 4, bounds.Y + 10));
            hdc.DrawLine(borderPen, new Point(bounds.X + 8, bounds.Y + 10), new Point(bounds.X + 10, bounds.Y + 10));

            hdc.DrawLine(borderPen, new Point(bounds.X + 0, bounds.Y + 4), new Point(bounds.X + 0, bounds.Y + 8));
            hdc.DrawLine(borderPen, new Point(bounds.X + 11, bounds.Y + 4), new Point(bounds.X + 11, bounds.Y + 8));

            hdc.DrawLine(borderPen, new Point(bounds.X + 1, bounds.Y + 2), new Point(bounds.X + 1, bounds.Y + 4));
            hdc.DrawLine(borderPen, new Point(bounds.X + 1, bounds.Y + 8), new Point(bounds.X + 1, bounds.Y + 10));

            hdc.DrawLine(borderPen, new Point(bounds.X + 10, bounds.Y + 2), new Point(bounds.X + 10, bounds.Y + 4));
            hdc.DrawLine(borderPen, new Point(bounds.X + 10, bounds.Y + 8), new Point(bounds.X + 10, bounds.Y + 10));
        }

        private static int GetScaledNumber(int n, double scale)
        {
            return (int)(n * scale);
        }

        protected void DrawCheckOnly(PaintEventArgs e, LayoutData layout, Color checkColor, Color checkBackground, bool disabledColors)
        {
            if (!Control.Checked)
            {
                return;
            }

            if (!Control.Enabled && disabledColors)
            {
                checkColor = SystemColors.ControlDark;
            }

            double scale = GetDpiScaleRatio();
            using var hdc = new DeviceContextHdcScope(e);
            using var brush = new Gdi32.CreateBrushScope(checkColor);

            // Circle drawing doesn't work at this size
            int offset = 5;

            Rectangle vCross = new Rectangle(
                layout.checkBounds.X + GetScaledNumber(offset, scale),
                layout.checkBounds.Y + GetScaledNumber(offset - 1, scale),
                GetScaledNumber(2, scale),
                GetScaledNumber(4, scale));
            hdc.FillRectangle(vCross, brush);

            Rectangle hCross = new Rectangle(
                layout.checkBounds.X + GetScaledNumber(offset - 1, scale),
                layout.checkBounds.Y + GetScaledNumber(offset, scale),
                GetScaledNumber(4, scale), GetScaledNumber(2, scale));
            hdc.FillRectangle(hCross, brush);
        }

        protected ButtonState GetState()
        {
            ButtonState style = default;

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
            Rectangle check = layout.checkBounds;
            if (!Application.RenderWithVisualStyles)
            {
                check.X--;      // compensate for Windows drawing slightly offset to right
            }

            ButtonState style = GetState();

            if (Application.RenderWithVisualStyles)
            {
                using var hdc = new DeviceContextHdcScope(e);
                RadioButtonRenderer.DrawRadioButtonWithVisualStyles(
                    hdc,
                    new Point(check.Left, check.Top),
                    RadioButtonRenderer.ConvertFromButtonState(style, Control.MouseIsOver),
                    Control.HandleInternal);
            }
            else
            {
                ControlPaint.DrawRadioButton(e.GraphicsInternal, check, style);
            }
        }

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
