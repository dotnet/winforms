// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.ButtonInternal
{
    internal abstract class CheckBoxBaseAdapter : CheckableControlBaseAdapter
    {
        protected const int flatCheckSize = 11;

        [ThreadStatic]
        private static Bitmap checkImageChecked = null;
        [ThreadStatic]
        private static Color checkImageCheckedBackColor = Color.Empty;

        [ThreadStatic]
        private static Bitmap checkImageIndeterminate = null;
        [ThreadStatic]
        private static Color checkImageIndeterminateBackColor = Color.Empty;

        internal CheckBoxBaseAdapter(ButtonBase control) : base(control) { }

        protected new CheckBox Control
        {
            get
            {
                return ((CheckBox)base.Control);
            }
        }

        #region Drawing Helpers

        protected void DrawCheckFlat(
            PaintEventArgs e,
            LayoutData layout,
            Color checkColor,
            Color checkBackground,
            Color checkBorder,
            ColorData colors)
        {
            Rectangle bounds = layout.checkBounds;

            // Removed subtracting one for Width and Height. In Everett we needed to do this,
            // since we were using GDI+ to draw the border. Now that we are using GDI,
            // we should not do before drawing the border.

            if (!layout.options.everettButtonCompat)
            {
                bounds.Width--;
                bounds.Height--;
            }

            using (var hdc = new DeviceContextHdcScope(e))
            {
                using var hpen = new Gdi32.CreatePenScope(checkBorder);
                hdc.DrawRectangle(bounds, hpen);

                // Now subtract, since the rest of the code is like Everett.
                if (layout.options.everettButtonCompat)
                {
                    bounds.Width--;
                    bounds.Height--;
                }
                bounds.Inflate(-1, -1);
            }

            if (Control.CheckState == CheckState.Indeterminate)
            {
                bounds.Width++;
                bounds.Height++;
                DrawDitheredFill(e.Graphics, colors.buttonFace, checkBackground, bounds);
            }
            else
            {
                using var hdc = new DeviceContextHdcScope(e);
                using var hbrush = new Gdi32.CreateBrushScope(checkBackground);

                // Even though we are using GDI here as opposed to GDI+ in Everett, we still need to add 1.
                bounds.Width++;
                bounds.Height++;
                hdc.FillRectangle(bounds, hbrush);
            }

            DrawCheckOnly(e, layout, colors, checkColor);
        }

        internal static void DrawCheckBackground(
            bool controlEnabled,
            CheckState controlCheckState,
            IDeviceContext deviceContext,
            Rectangle bounds,
            Color checkBackground,
            bool disabledColors)
        {
            using var hdc = new DeviceContextHdcScope(deviceContext);

            Color color;

            if (!controlEnabled && disabledColors)
            {
                color = SystemColors.Control;
            }
            else if (controlCheckState == CheckState.Indeterminate && checkBackground == SystemColors.Window && disabledColors)
            {
                Color comboColor = SystemInformation.HighContrast ? SystemColors.ControlDark : SystemColors.Control;
                color = Color.FromArgb(
                    (byte)((comboColor.R + SystemColors.Window.R) / 2),
                    (byte)((comboColor.G + SystemColors.Window.G) / 2),
                    (byte)((comboColor.B + SystemColors.Window.B) / 2));
            }
            else
            {
                color = checkBackground;
            }

            using var hbrush = new Gdi32.CreateBrushScope(color);

            RECT rect = bounds;
            User32.FillRect(hdc, ref rect, hbrush);
        }

        protected void DrawCheckBackground(
            PaintEventArgs e,
            Rectangle bounds,
            Color checkBackground,
            bool disabledColors,
            ColorData colors)
        {
            // Area behind check

            if (Control.CheckState == CheckState.Indeterminate)
            {
                DrawDitheredFill(e.GraphicsInternal, colors.buttonFace, checkBackground, bounds);
            }
            else
            {
                DrawCheckBackground(Control.Enabled, Control.CheckState, e, bounds, checkBackground, disabledColors);
            }
        }

        protected void DrawCheckOnly(PaintEventArgs e, LayoutData layout, ColorData colors, Color checkColor)
        {
            DrawCheckOnly(
                flatCheckSize,
                Control.Checked,
                Control.Enabled,
                Control.CheckState,
                e.GraphicsInternal,
                layout,
                colors,
                checkColor);
        }

        internal static void DrawCheckOnly(
            int checkSize,
            bool controlChecked,
            bool controlEnabled,
            CheckState controlCheckState,
            Graphics g,
            LayoutData layout,
            ColorData colors,
            Color checkColor)
        {
            if (!controlChecked)
            {
                return;
            }

            if (!controlEnabled)
            {
                checkColor = colors.buttonShadow;
            }
            else if (controlCheckState == CheckState.Indeterminate)
            {
                checkColor = SystemInformation.HighContrast ? colors.highlight : colors.buttonShadow;
            }

            Rectangle fullSize = layout.checkBounds;

            if (fullSize.Width == checkSize)
            {
                fullSize.Width++;
                fullSize.Height++;
            }

            fullSize.Width++;

            fullSize.Height++;
            Bitmap checkImage;
            if (controlCheckState == CheckState.Checked)
            {
                checkImage = GetCheckBoxImage(checkColor, fullSize, ref checkImageCheckedBackColor, ref checkImageChecked);
            }
            else
            {
                Debug.Assert(
                    controlCheckState == CheckState.Indeterminate,
                    "we want to paint the check box only if the item is checked or indeterminate");
                checkImage = GetCheckBoxImage(checkColor, fullSize, ref checkImageIndeterminateBackColor, ref checkImageIndeterminate);
            }

            fullSize.Y -= layout.options.everettButtonCompat ? 1 : 2;

            ControlPaint.DrawImageColorized(g, checkImage, fullSize, checkColor);
        }

        internal static Rectangle DrawPopupBorder(Graphics g, Rectangle r, ColorData colors)
        {
            using var hdc = new DeviceContextHdcScope(g);
            return DrawPopupBorder(hdc, r, colors);
        }

        internal static Rectangle DrawPopupBorder(PaintEventArgs e, Rectangle r, ColorData colors)
        {
            using var hdc = new DeviceContextHdcScope(e);
            return DrawPopupBorder(hdc, r, colors);
        }

        internal static Rectangle DrawPopupBorder(Gdi32.HDC hdc, Rectangle r, ColorData colors)
        {
            using var high = new Gdi32.CreatePenScope(colors.highlight);
            using var shadow = new Gdi32.CreatePenScope(colors.buttonShadow);
            using var face = new Gdi32.CreatePenScope(colors.buttonFace);

            hdc.DrawLine(high, r.Right - 1, r.Top, r.Right - 1, r.Bottom);
            hdc.DrawLine(high, r.Left, r.Bottom - 1, r.Right, r.Bottom - 1);

            hdc.DrawLine(shadow, r.Left, r.Top, r.Left, r.Bottom);
            hdc.DrawLine(shadow, r.Left, r.Top, r.Right - 1, r.Top);

            hdc.DrawLine(face, r.Right - 2, r.Top + 1, r.Right - 2, r.Bottom - 1);
            hdc.DrawLine(face, r.Left + 1, r.Bottom - 2, r.Right - 1, r.Bottom - 2);

            r.Inflate(-1, -1);
            return r;
        }

        protected ButtonState GetState()
        {
            ButtonState style = (ButtonState)0;

            if (Control.CheckState == CheckState.Unchecked)
            {
                style |= ButtonState.Normal;
            }
            else
            {
                style |= ButtonState.Checked;
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
            ButtonState style = GetState();

            if (Control.CheckState == CheckState.Indeterminate)
            {
                if (Application.RenderWithVisualStyles)
                {
                    CheckBoxRenderer.DrawCheckBoxWithVisualStyles(
                        e,
                        new Point(layout.checkBounds.Left, layout.checkBounds.Top),
                        CheckBoxRenderer.ConvertFromButtonState(style, true, Control.MouseIsOver),
                        Control.HandleInternal);
                }
                else
                {
                    ControlPaint.DrawMixedCheckBox(e.GraphicsInternal, layout.checkBounds, style);
                }
            }
            else
            {
                if (Application.RenderWithVisualStyles)
                {
                    CheckBoxRenderer.DrawCheckBoxWithVisualStyles(
                        e,
                        new Point(layout.checkBounds.Left, layout.checkBounds.Top),
                        CheckBoxRenderer.ConvertFromButtonState(style, false, Control.MouseIsOver),
                        Control.HandleInternal);
                }
                else
                {
                    ControlPaint.DrawCheckBox(e.GraphicsInternal, layout.checkBounds, style);
                }
            }
        }

        #endregion

        private static Bitmap GetCheckBoxImage(Color checkColor, Rectangle fullSize, ref Color cacheCheckColor, ref Bitmap cacheCheckImage)
        {
            if (cacheCheckImage != null &&
                cacheCheckColor.Equals(checkColor) &&
                cacheCheckImage.Width == fullSize.Width &&
                cacheCheckImage.Height == fullSize.Height)
            {
                return cacheCheckImage;
            }

            cacheCheckImage?.Dispose();

            // We draw the checkmark slightly off center to eliminate 3-D border artifacts and compensate below
            RECT rcCheck = new Rectangle(0, 0, fullSize.Width, fullSize.Height);
            Bitmap bitmap = new Bitmap(fullSize.Width, fullSize.Height);

            using (Graphics offscreen = Graphics.FromImage(bitmap))
            {
                offscreen.Clear(Color.Transparent);
                using var hdc = new DeviceContextHdcScope(offscreen, applyGraphicsState: false);
                User32.DrawFrameControl(
                    hdc,
                    ref rcCheck,
                    User32.DFC.MENU,
                    User32.DFCS.MENUCHECK);
            }

            bitmap.MakeTransparent();
            cacheCheckImage = bitmap;
            cacheCheckColor = checkColor;

            return cacheCheckImage;
        }

        protected void AdjustFocusRectangle(LayoutData layout)
        {
            if (string.IsNullOrEmpty(Control.Text))
            {
                // When a CheckBox has no text, AutoSize sets the size to zero and thus there's no place around which
                // to draw the focus rectangle. So, when AutoSize == true we want the focus rectangle to be rendered
                // inside the box. Otherwise, it should encircle all the available space next to the box (like it's
                // done in WPF and ComCtl32).
                layout.focus = Control.AutoSize ? Rectangle.Inflate(layout.checkBounds, -2, -2) : layout.field;
            }
        }

        internal override LayoutOptions CommonLayout()
        {
            LayoutOptions layout = base.CommonLayout();
            layout.checkAlign = Control.CheckAlign;
            layout.textOffset = false;
            layout.shadowedText = !Control.Enabled;
            layout.layoutRTL = RightToLeft.Yes == Control.RightToLeft;

            return layout;
        }
    }
}
