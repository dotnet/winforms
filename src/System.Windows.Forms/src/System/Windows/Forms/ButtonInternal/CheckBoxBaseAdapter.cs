// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ButtonInternal {
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;
    using System.Runtime.InteropServices;

    internal abstract class CheckBoxBaseAdapter : CheckableControlBaseAdapter {

        protected const int flatCheckSize = 11;

        [ThreadStatic]
        private static Bitmap checkImageChecked = null;
        [ThreadStatic]
        private static Color checkImageCheckedBackColor         = Color.Empty;

        [ThreadStatic]
        private static Bitmap checkImageIndeterminate    = null;
        [ThreadStatic]
        private static Color checkImageIndeterminateBackColor   = Color.Empty;

        internal CheckBoxBaseAdapter(ButtonBase control) : base(control) {}

        protected new CheckBox Control {
            get {
                return ((CheckBox)base.Control);
            }
        }

        #region Drawing Helpers

        protected void DrawCheckFlat(PaintEventArgs e, LayoutData layout, Color checkColor, Color checkBackground, Color checkBorder, ColorData colors) {
            Rectangle bounds = layout.checkBounds;
            // Removed subtracting one for Width and Height. In Everett we needed to do this,
            // since we were using GDI+ to draw the border. Now that we are using GDI,
            // we should not do before drawing the border.

            if (!layout.options.everettButtonCompat) {
                bounds.Width--;
                bounds.Height--;                
            }
            using (WindowsGraphics wg = WindowsGraphics.FromGraphics( e.Graphics )) {
                using(WindowsPen pen = new WindowsPen(wg.DeviceContext, checkBorder)){
                    wg.DrawRectangle(pen, bounds);
                }

                // Now subtract, since the rest of the code is like Everett.
                if (layout.options.everettButtonCompat) {
                    bounds.Width--;
                    bounds.Height--;                
                }                
                bounds.Inflate(-1, -1);
            }
            if (Control.CheckState == CheckState.Indeterminate) {
                bounds.Width++;
                bounds.Height++;
                DrawDitheredFill(e.Graphics, colors.buttonFace, checkBackground, bounds);
            }
            else {
                using( WindowsGraphics wg = WindowsGraphics.FromGraphics( e.Graphics )) {
                    using (WindowsBrush brush = new WindowsSolidBrush(wg.DeviceContext, checkBackground)) {
                        // Even though we are using GDI here as opposed to GDI+ in Everett, we still need to add 1.
                        bounds.Width++;
                        bounds.Height++;
                        wg.FillRectangle(brush, bounds);
                    }
                }
            }
            DrawCheckOnly(e, layout, colors, checkColor, checkBackground);
    
        }

        // used by DataGridViewCheckBoxCell
        internal static void DrawCheckBackground(bool controlEnabled, CheckState controlCheckState, Graphics g, Rectangle bounds, Color checkColor, Color checkBackground, bool disabledColors, ColorData colors)
        {                       
            using ( WindowsGraphics wg = WindowsGraphics.FromGraphics( g )) {
                WindowsBrush brush;
                if (!controlEnabled && disabledColors) {
                    brush = new WindowsSolidBrush(wg.DeviceContext, SystemColors.Control);
                }
                else if (controlCheckState == CheckState.Indeterminate && checkBackground == SystemColors.Window && disabledColors) {
                    Color comboColor = SystemInformation.HighContrast ? SystemColors.ControlDark :
                            SystemColors.Control;
                    byte R = (byte)((comboColor.R + SystemColors.Window.R) / 2);
                    byte G = (byte)((comboColor.G + SystemColors.Window.G) / 2);
                    byte B = (byte)((comboColor.B + SystemColors.Window.B) / 2);
                    brush = new WindowsSolidBrush(wg.DeviceContext, Color.FromArgb(R, G, B));
                }
                else {
                    brush = new WindowsSolidBrush(wg.DeviceContext, checkBackground);
                }
                
                try {
                    wg.FillRectangle(brush, bounds);
                }
                finally {
                    if (brush != null) {
                        brush.Dispose();
                    }
                }
            }
        }

        protected void DrawCheckBackground(PaintEventArgs e, Rectangle bounds, Color checkColor, Color checkBackground, bool disabledColors, ColorData colors) {
            // area behind check
            //
            if (Control.CheckState == CheckState.Indeterminate) {
                DrawDitheredFill(e.Graphics, colors.buttonFace, checkBackground, bounds);
            }
            else {
                DrawCheckBackground(Control.Enabled, Control.CheckState, e.Graphics, bounds, checkColor, checkBackground, disabledColors, colors);
            }
        }

        protected void DrawCheckOnly(PaintEventArgs e, LayoutData layout, ColorData colors, Color checkColor, Color checkBackground) {
            DrawCheckOnly(flatCheckSize, Control.Checked, Control.Enabled, Control.CheckState, e.Graphics, layout, colors, checkColor, checkBackground);
        }
        
        // used by DataGridViewCheckBoxCell
        internal static void DrawCheckOnly(int checkSize, bool controlChecked, bool controlEnabled, CheckState controlCheckState, Graphics g, LayoutData layout, ColorData colors, Color checkColor, Color checkBackground) {

            // check
            //
            if (controlChecked) {
                if (!controlEnabled) {
                    checkColor = colors.buttonShadow;
                }
                else if (controlCheckState == CheckState.Indeterminate) {
                    checkColor = SystemInformation.HighContrast ? colors.highlight :
                       colors.buttonShadow;
                }
                    
                Rectangle fullSize = layout.checkBounds;
                
                if (fullSize.Width == checkSize) {
                    fullSize.Width++;
                    fullSize.Height++;
                }

                fullSize.Width++;

                fullSize.Height++;
                Bitmap checkImage = null;
                if (controlCheckState == CheckState.Checked) {
                    checkImage = GetCheckBoxImage(checkColor, fullSize, ref checkImageCheckedBackColor, ref checkImageChecked);
                } else {
                    Debug.Assert(controlCheckState == CheckState.Indeterminate, "we want to paint the check box only if the item is checked or indeterminate");
                    checkImage = GetCheckBoxImage(checkColor, fullSize, ref checkImageIndeterminateBackColor, ref checkImageIndeterminate);
                }

                if (layout.options.everettButtonCompat) {
                    fullSize.Y -= 1;
                }
                else {
                    fullSize.Y -= 2;
                }

                ControlPaint.DrawImageColorized(g, checkImage, fullSize, checkColor);
            }
        }
        
        internal static Rectangle DrawPopupBorder(Graphics g, Rectangle r, ColorData colors) {
            using (WindowsGraphics wg = WindowsGraphics.FromGraphics( g )) {
                
                using( WindowsPen high = new WindowsPen(wg.DeviceContext, colors.highlight),
                   shadow = new WindowsPen(wg.DeviceContext, colors.buttonShadow),
                   face = new WindowsPen(wg.DeviceContext, colors.buttonFace)) {
                   
                    wg.DrawLine(high, r.Right-1 , r.Top, r.Right-1, r.Bottom);
                    wg.DrawLine(high, r.Left, r.Bottom-1, r.Right, r.Bottom-1);

                    wg.DrawLine(shadow, r.Left, r.Top, r.Left , r.Bottom);
                    wg.DrawLine(shadow, r.Left, r.Top, r.Right- 1, r.Top);

                    wg.DrawLine(face, r.Right - 2, r.Top + 1, r.Right - 2, r.Bottom - 1);
                    wg.DrawLine(face, r.Left + 1, r.Bottom - 2, r.Right - 1, r.Bottom - 2);
                }
            }
            r.Inflate(-1, -1);
            return r;
        }

        protected ButtonState GetState() {
            ButtonState style = (ButtonState)0;

            if (Control.CheckState == CheckState.Unchecked) {
                style |= ButtonState.Normal;
            }
            else {
                style |= ButtonState.Checked;
            }

            if (!Control.Enabled) {
                style |= ButtonState.Inactive;
            }

            if (Control.MouseIsDown) {
                style |= ButtonState.Pushed;
            }

            return style;
        }
        
        protected void DrawCheckBox(PaintEventArgs e, LayoutData layout) {
            Graphics g = e.Graphics;
            
            ButtonState style = GetState();

            if (Control.CheckState == CheckState.Indeterminate) {
                if (Application.RenderWithVisualStyles) {
                    CheckBoxRenderer.DrawCheckBox(g, new Point(layout.checkBounds.Left, layout.checkBounds.Top), CheckBoxRenderer.ConvertFromButtonState(style, true, Control.MouseIsOver), Control.HandleInternal);
                }
                else {
                    ControlPaint.DrawMixedCheckBox(g, layout.checkBounds, style);
                }
            }
            else {
                if (Application.RenderWithVisualStyles) {
                    CheckBoxRenderer.DrawCheckBox(g, new Point(layout.checkBounds.Left, layout.checkBounds.Top), CheckBoxRenderer.ConvertFromButtonState(style, false, Control.MouseIsOver), Control.HandleInternal);
                }
                else {
                    ControlPaint.DrawCheckBox(g, layout.checkBounds, style);
                }
            }
        }
        
        #endregion

        private static Bitmap GetCheckBoxImage(Color checkColor, Rectangle fullSize, ref Color cacheCheckColor, ref Bitmap cacheCheckImage)
        {
            if (cacheCheckImage != null &&
                cacheCheckColor.Equals(checkColor) &&
                cacheCheckImage.Width == fullSize.Width &&
                cacheCheckImage.Height == fullSize.Height) {
                return cacheCheckImage;
            }

            if (cacheCheckImage != null)
            {
                cacheCheckImage.Dispose();
                cacheCheckImage = null;
            }

            // We draw the checkmark slightly off center to eliminate 3-D border artifacts,
            // and compensate below
            NativeMethods.RECT rcCheck = NativeMethods.RECT.FromXYWH(0, 0, fullSize.Width, fullSize.Height);
            Bitmap bitmap = new Bitmap(fullSize.Width, fullSize.Height);
            Graphics offscreen = Graphics.FromImage(bitmap);
            offscreen.Clear(Color.Transparent);
            IntPtr dc = offscreen.GetHdc();
            try {
                SafeNativeMethods.DrawFrameControl(new HandleRef(offscreen, dc), ref rcCheck,
                                         NativeMethods.DFC_MENU, NativeMethods.DFCS_MENUCHECK);
            } finally {
                offscreen.ReleaseHdcInternal(dc);
                offscreen.Dispose();
            }

            bitmap.MakeTransparent();
            cacheCheckImage = bitmap;
            cacheCheckColor = checkColor;

            return cacheCheckImage;
        }

        protected void AdjustFocusRectangle(LayoutData layout) { 
            if (AccessibilityImprovements.Level2 && String.IsNullOrEmpty(Control.Text)) {
                // When a CheckBox has no text, AutoSize sets the size to zero 
                // and thus there's no place around which to draw the focus rectangle.
                // So, when AutoSize == true we want the focus rectangle to be rendered inside the box.
                // Otherwise, it should encircle all the available space next to the box (like it's done in WPF and ComCtl32).
                layout.focus = Control.AutoSize ? Rectangle.Inflate(layout.checkBounds, -2, -2) : layout.field;
            }
        }

        internal override LayoutOptions CommonLayout() {
            LayoutOptions layout = base.CommonLayout();
            layout.checkAlign        = Control.CheckAlign;
            layout.textOffset        = false;
            layout.shadowedText      = !Control.Enabled;
            layout.layoutRTL         = RightToLeft.Yes == Control.RightToLeft;

            return layout;
        }
    }
}
