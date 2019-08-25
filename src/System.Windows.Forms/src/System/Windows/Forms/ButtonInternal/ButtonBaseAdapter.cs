// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Internal;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms.ButtonInternal
{
    /// <summary>
    ///        PLEASE READ
    ///        -----------
    ///  This class is used for more than just Button:
    ///  it's used for things that derive from ButtonBase,
    ///  parts of ToolStripItem, and parts of the DataGridView.
    /// </summary>
    internal abstract class ButtonBaseAdapter
    {
        private readonly ButtonBase control;

        // SystemInformation.Border3DSize + 2 pixels for focus rect
        protected static int buttonBorderSize = 4;

        internal ButtonBaseAdapter(ButtonBase control)
        {
            this.control = control;
        }

        protected ButtonBase Control
        {
            get { return control; }
        }

        internal void Paint(PaintEventArgs pevent)
        {
            if (Control.MouseIsDown)
            {
                PaintDown(pevent, CheckState.Unchecked);
            }
            else if (Control.MouseIsOver)
            {
                PaintOver(pevent, CheckState.Unchecked);
            }
            else
            {
                PaintUp(pevent, CheckState.Unchecked);
            }
        }

        internal virtual Size GetPreferredSizeCore(Size proposedSize)
        {
            // this is a shared cached graphics, therefore it does not require dispose.
            using (Graphics measurementGraphics = WindowsFormsUtils.CreateMeasurementGraphics())
            {
                using (PaintEventArgs pe = new PaintEventArgs(measurementGraphics, new Rectangle()))
                {
                    LayoutOptions options = Layout(pe);
                    return options.GetPreferredSizeCore(proposedSize);
                }
            }
        }

        protected abstract LayoutOptions Layout(PaintEventArgs e);

        internal abstract void PaintUp(PaintEventArgs e, CheckState state);

        internal abstract void PaintDown(PaintEventArgs e, CheckState state);

        internal abstract void PaintOver(PaintEventArgs e, CheckState state);

        #region Accessibility Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsHighContrastHighlighted()
        {
            return SystemInformation.HighContrast && Application.RenderWithVisualStyles &&
                (Control.Focused || Control.MouseIsOver || (Control.IsDefault && Control.Enabled));
        }

        #endregion

        #region Drawing Helpers

        internal static Color MixedColor(Color color1, Color color2)
        {
            byte a1 = color1.A;
            byte r1 = color1.R;
            byte g1 = color1.G;
            byte b1 = color1.B;

            byte a2 = color2.A;
            byte r2 = color2.R;
            byte g2 = color2.G;
            byte b2 = color2.B;

            int a3 = (a1 + a2) / 2;
            int r3 = (r1 + r2) / 2;
            int g3 = (g1 + g2) / 2;
            int b3 = (b1 + b2) / 2;

            return Color.FromArgb(a3, r3, g3, b3);
        }

        internal static Brush CreateDitherBrush(Color color1, Color color2)
        {
            // Note: Don't dispose the bitmap here. The texture brush will take ownership
            // of the bitmap. So the bitmap will get disposed by the brush's Dispose().

            using (Bitmap b = new Bitmap(2, 2))
            {
                b.SetPixel(0, 0, color1);
                b.SetPixel(0, 1, color2);
                b.SetPixel(1, 1, color1);
                b.SetPixel(1, 0, color2);

                return new TextureBrush(b);
            }
        }

        /// <summary>
        ///  Get StringFormat object for rendering text using GDI+ (Graphics).
        /// </summary>
        internal virtual StringFormat CreateStringFormat()
        {
            return ControlPaint.CreateStringFormat(Control, Control.TextAlign, Control.ShowToolTip, Control.UseMnemonic);
        }

        /// <summary>
        ///  Get TextFormatFlags flags for rendering text using GDI (TextRenderer).
        /// </summary>
        internal virtual TextFormatFlags CreateTextFormatFlags()
        {
            return ControlPaint.CreateTextFormatFlags(Control, Control.TextAlign, Control.ShowToolTip, Control.UseMnemonic);
        }

        internal static void DrawDitheredFill(Graphics g, Color color1, Color color2, Rectangle bounds)
        {
            using (Brush brush = CreateDitherBrush(color1, color2))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        protected void Draw3DBorder(Graphics g, Rectangle bounds, ColorData colors, bool raised)
        {
            if (Control.BackColor != SystemColors.Control && SystemInformation.HighContrast)
            {
                if (raised)
                {
                    Draw3DBorderHighContrastRaised(g, ref bounds, colors);
                }
                else
                {
                    ControlPaint.DrawBorder(g, bounds, ControlPaint.Dark(Control.BackColor), ButtonBorderStyle.Solid);
                }
            }
            else
            {
                if (raised)
                {
                    Draw3DBorderRaised(g, ref bounds, colors);
                }
                else
                {
                    Draw3DBorderNormal(g, ref bounds, colors);
                }
            }
        }

        private void Draw3DBorderHighContrastRaised(Graphics g, ref Rectangle bounds, ColorData colors)
        {
            bool stockColor = colors.buttonFace.ToKnownColor() == SystemColors.Control.ToKnownColor();
            bool disabledHighContrast = (!Control.Enabled) && SystemInformation.HighContrast;

            using (WindowsGraphics wg = WindowsGraphics.FromGraphics(g))
            {

                // Draw counter-clock-wise.
                Point p1 = new Point(bounds.X + bounds.Width - 1, bounds.Y);  // upper inner right.
                Point p2 = new Point(bounds.X, bounds.Y);  // upper left.
                Point p3 = new Point(bounds.X, bounds.Y + bounds.Height - 1);  // bottom inner left.
                Point p4 = new Point(bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);  // inner bottom right.

                WindowsPen penTopLeft = null;
                WindowsPen penBottomRight = null;
                WindowsPen insetPen = null;
                WindowsPen bottomRightInsetPen = null;

                try
                {
                    // top + left
                    if (disabledHighContrast)
                    {
                        penTopLeft = new WindowsPen(wg.DeviceContext, colors.windowDisabled);
                    }
                    else
                    {
                        penTopLeft = stockColor ? new WindowsPen(wg.DeviceContext, SystemColors.ControlLightLight) : new WindowsPen(wg.DeviceContext, colors.highlight);
                    }
                    wg.DrawLine(penTopLeft, p1, p2); // top  (right-left)
                    wg.DrawLine(penTopLeft, p2, p3); // left (up-down)

                    // bottom + right
                    if (disabledHighContrast)
                    {
                        penBottomRight = new WindowsPen(wg.DeviceContext, colors.windowDisabled);
                    }
                    else
                    {
                        penBottomRight = stockColor ? new WindowsPen(wg.DeviceContext, SystemColors.ControlDarkDark) : new WindowsPen(wg.DeviceContext, colors.buttonShadowDark);
                    }
                    p1.Offset(0, -1); // need to paint last pixel too.
                    wg.DrawLine(penBottomRight, p3, p4);  // bottom (left-right)
                    wg.DrawLine(penBottomRight, p4, p1);  // right  (bottom-up )

                    // Draw inset using the background color to make the top and left lines thinner
                    if (stockColor)
                    {
                        if (SystemInformation.HighContrast)
                        {
                            insetPen = new WindowsPen(wg.DeviceContext, SystemColors.ControlLight);
                        }
                        else
                        {
                            insetPen = new WindowsPen(wg.DeviceContext, SystemColors.Control);
                        }
                    }
                    else
                    {
                        if (SystemInformation.HighContrast)
                        {
                            insetPen = new WindowsPen(wg.DeviceContext, colors.highlight);
                        }
                        else
                        {
                            insetPen = new WindowsPen(wg.DeviceContext, colors.buttonFace);
                        }
                    }

                    p1.Offset(-1, 2);
                    p2.Offset(1, 1);
                    p3.Offset(1, -1);
                    p4.Offset(-1, -1);

                    // top + left inset
                    wg.DrawLine(insetPen, p1, p2); // top (right-left)
                    wg.DrawLine(insetPen, p2, p3); // left( up-down)

                    // Bottom + right inset
                    if (disabledHighContrast)
                    {
                        bottomRightInsetPen = new WindowsPen(wg.DeviceContext, colors.windowDisabled);
                    }
                    else
                    {
                        bottomRightInsetPen = stockColor ? new WindowsPen(wg.DeviceContext, SystemColors.ControlDark) : new WindowsPen(wg.DeviceContext, colors.buttonShadow);
                    }
                    p1.Offset(0, -1); // need to paint last pixel too.
                    wg.DrawLine(bottomRightInsetPen, p3, p4); // bottom (left-right)
                    wg.DrawLine(bottomRightInsetPen, p4, p1); // right  (bottom-up)
                }
                finally
                {
                    if (penTopLeft != null)
                    {
                        penTopLeft.Dispose();
                    }

                    if (penBottomRight != null)
                    {
                        penBottomRight.Dispose();
                    }

                    if (insetPen != null)
                    {
                        insetPen.Dispose();
                    }

                    if (bottomRightInsetPen != null)
                    {
                        bottomRightInsetPen.Dispose();
                    }
                }
            }
        }

        private void Draw3DBorderNormal(Graphics g, ref Rectangle bounds, ColorData colors)
        {
            using (WindowsGraphics wg = WindowsGraphics.FromGraphics(g))
            {

                // Draw counter-clock-wise.
                Point p1 = new Point(bounds.X + bounds.Width - 1, bounds.Y);  // upper inner right.
                Point p2 = new Point(bounds.X, bounds.Y);  // upper left.
                Point p3 = new Point(bounds.X, bounds.Y + bounds.Height - 1);  // bottom inner left.
                Point p4 = new Point(bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);  // inner bottom right.

                // top + left
                WindowsPen pen = new WindowsPen(wg.DeviceContext, colors.buttonShadowDark);
                try
                {
                    wg.DrawLine(pen, p1, p2); // top (right-left)
                    wg.DrawLine(pen, p2, p3); // left(up-down)
                }
                finally
                {
                    pen.Dispose();
                }

                // bottom + right
                pen = new WindowsPen(wg.DeviceContext, colors.highlight);
                try
                {
                    p1.Offset(0, -1); // need to paint last pixel too.
                    wg.DrawLine(pen, p3, p4); // bottom(left-right)
                    wg.DrawLine(pen, p4, p1); // right (bottom-up)
                }
                finally
                {
                    pen.Dispose();
                }

                // Draw inset

                pen = new WindowsPen(wg.DeviceContext, colors.buttonFace);

                p1.Offset(-1, 2);
                p2.Offset(1, 1);
                p3.Offset(1, -1);
                p4.Offset(-1, -1);

                // top + left inset
                try
                {
                    wg.DrawLine(pen, p1, p2); // top (right-left)
                    wg.DrawLine(pen, p2, p3); // left(up-down)
                }
                finally
                {
                    pen.Dispose();
                }

                // bottom + right inset
                if (colors.buttonFace.ToKnownColor() == SystemColors.Control.ToKnownColor())
                {
                    pen = new WindowsPen(wg.DeviceContext, SystemColors.ControlLight);
                }
                else
                {
                    pen = new WindowsPen(wg.DeviceContext, colors.buttonFace);
                }

                try
                {
                    p1.Offset(0, -1); // need to paint last pixel too.
                    wg.DrawLine(pen, p3, p4); // bottom(left-right)
                    wg.DrawLine(pen, p4, p1); // right (bottom-up)
                }
                finally
                {
                    pen.Dispose();
                }
            }
        }

        private void Draw3DBorderRaised(Graphics g, ref Rectangle bounds, ColorData colors)
        {
            bool stockColor = colors.buttonFace.ToKnownColor() == SystemColors.Control.ToKnownColor();
            bool disabledHighContrast = (!Control.Enabled) && SystemInformation.HighContrast;

            using (WindowsGraphics wg = WindowsGraphics.FromGraphics(g))
            {

                // Draw counter-clock-wise.
                Point p1 = new Point(bounds.X + bounds.Width - 1, bounds.Y);  // upper inner right.
                Point p2 = new Point(bounds.X, bounds.Y);  // upper left.
                Point p3 = new Point(bounds.X, bounds.Y + bounds.Height - 1);  // bottom inner left.
                Point p4 = new Point(bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);  // inner bottom right.

                // Draw counter-clock-wise.

                // top + left
                WindowsPen pen;
                if (disabledHighContrast)
                {
                    pen = new WindowsPen(wg.DeviceContext, colors.windowDisabled);
                }
                else if (stockColor)
                {
                    pen = new WindowsPen(wg.DeviceContext, SystemColors.ControlLightLight);
                }
                else
                {
                    pen = new WindowsPen(wg.DeviceContext, colors.highlight);
                }

                try
                {
                    wg.DrawLine(pen, p1, p2);   // top (right-left)
                    wg.DrawLine(pen, p2, p3);   // left(up-down)
                }
                finally
                {
                    pen.Dispose();
                }

                // bottom + right
                if (disabledHighContrast)
                {
                    pen = new WindowsPen(wg.DeviceContext, colors.windowDisabled);
                }
                else if (stockColor)
                {
                    pen = new WindowsPen(wg.DeviceContext, SystemColors.ControlDarkDark);
                }
                else
                {
                    pen = new WindowsPen(wg.DeviceContext, colors.buttonShadowDark);
                }

                try
                {
                    p1.Offset(0, -1); // need to paint last pixel too.
                    wg.DrawLine(pen, p3, p4);    // bottom(left-right)
                    wg.DrawLine(pen, p4, p1);    // right (bottom-up)
                }
                finally
                {
                    pen.Dispose();
                }

                // Draw inset - use the back ground color here to have a thinner border
                p1.Offset(-1, 2);
                p2.Offset(1, 1);
                p3.Offset(1, -1);
                p4.Offset(-1, -1);

                if (stockColor)
                {
                    if (SystemInformation.HighContrast)
                    {
                        pen = new WindowsPen(wg.DeviceContext, SystemColors.ControlLight);
                    }
                    else
                    {
                        pen = new WindowsPen(wg.DeviceContext, SystemColors.Control);
                    }
                }
                else
                {
                    pen = new WindowsPen(wg.DeviceContext, colors.buttonFace);
                }

                // top + left inset
                try
                {
                    wg.DrawLine(pen, p1, p2); // top (right-left)
                    wg.DrawLine(pen, p2, p3); // left(up-down)
                }
                finally
                {
                    pen.Dispose();
                }

                // Bottom + right inset
                if (disabledHighContrast)
                {
                    pen = new WindowsPen(wg.DeviceContext, colors.windowDisabled);
                }
                else if (stockColor)
                {
                    pen = new WindowsPen(wg.DeviceContext, SystemColors.ControlDark);
                }
                else
                {
                    pen = new WindowsPen(wg.DeviceContext, colors.buttonShadow);
                }

                try
                {
                    p1.Offset(0, -1); // need to paint last pixel too.
                    wg.DrawLine(pen, p3, p4);  // bottom(left-right)
                    wg.DrawLine(pen, p4, p1);  // right (bottom-up)
                }
                finally
                {
                    pen.Dispose();
                }
            }
        }

        /// <summary>
        ///  Draws a border for the in the 3D style of the popup button.
        /// </summary>
        protected internal static void Draw3DLiteBorder(Graphics g, Rectangle r, ColorData colors, bool up)
        {
            using (WindowsGraphics wg = WindowsGraphics.FromGraphics(g))
            {

                // Draw counter-clock-wise.
                Point p1 = new Point(r.Right - 1, r.Top);  // upper inner right.
                Point p2 = new Point(r.Left, r.Top);  // upper left.
                Point p3 = new Point(r.Left, r.Bottom - 1);  // bottom inner left.
                Point p4 = new Point(r.Right - 1, r.Bottom - 1);  // inner bottom right.

                // top, left
                WindowsPen pen = up ? new WindowsPen(wg.DeviceContext, colors.highlight) : new WindowsPen(wg.DeviceContext, colors.buttonShadow);

                try
                {
                    wg.DrawLine(pen, p1, p2); // top (right-left)
                    wg.DrawLine(pen, p2, p3); // left (top-down)
                }
                finally
                {
                    pen.Dispose();
                }

                // bottom, right
                pen = up ? new WindowsPen(wg.DeviceContext, colors.buttonShadow) : new WindowsPen(wg.DeviceContext, colors.highlight);

                try
                {
                    p1.Offset(0, -1); // need to paint last pixel too.
                    wg.DrawLine(pen, p3, p4); // bottom (left-right)
                    wg.DrawLine(pen, p4, p1); // right(bottom-up)
                }
                finally
                {
                    pen.Dispose();
                }
            }
        }

        internal static void DrawFlatBorder(Graphics g, Rectangle r, Color c)
        {
            ControlPaint.DrawBorder(g, r, c, ButtonBorderStyle.Solid);
        }

        /// <summary>
        ///  Draws the flat border with specified bordersize.
        ///  This function gets called only for Flatstyle == Flatstyle.Flat.
        /// </summary>
        internal static void DrawFlatBorderWithSize(Graphics g, Rectangle r, Color c, int size)
        {
            bool stockBorder = c.IsSystemColor;
            SolidBrush brush = null;

            if (size > 1)
            {
                brush = new SolidBrush(c);
            }
            else
            {
                if (stockBorder)
                {
                    brush = (SolidBrush)SystemBrushes.FromSystemColor(c);
                }
                else
                {
                    brush = new SolidBrush(c);
                }
            }

            try
            {
                size = System.Math.Min(size, System.Math.Min(r.Width, r.Height));
                // ...truncate pen width to button size, to avoid overflow if border size is huge!

                //Left Border
                g.FillRectangle(brush, r.X, r.Y, size, r.Height);

                //Right Border
                g.FillRectangle(brush, (r.X + r.Width - size), r.Y, size, r.Height);

                //Top Border
                g.FillRectangle(brush, (r.X + size), r.Y, (r.Width - size * 2), size);

                //Bottom Border
                g.FillRectangle(brush, (r.X + size), (r.Y + r.Height - size), (r.Width - size * 2), size);
            }
            finally
            {
                if (!stockBorder && brush != null)
                {
                    brush.Dispose();
                }
            }
        }

        internal static void DrawFlatFocus(Graphics g, Rectangle r, Color c)
        {
            using (WindowsGraphics wg = WindowsGraphics.FromGraphics(g))
            {
                using (WindowsPen focus = new WindowsPen(wg.DeviceContext, c))
                {
                    wg.DrawRectangle(focus, r);
                }
            }
        }

        /// <summary>
        ///  Draws the focus rectangle if the control has focus.
        /// </summary>
        void DrawFocus(Graphics g, Rectangle r)
        {
            if (Control.Focused && Control.ShowFocusCues)
            {
                ControlPaint.DrawFocusRectangle(g, r, Control.ForeColor, Control.BackColor);
            }
        }

        /// <summary>
        ///  Draws the button's image.
        /// </summary>
        void DrawImage(Graphics graphics, LayoutData layout)
        {
            if (Control.Image != null)
            {
                //setup new clip region & draw
                DrawImageCore(graphics, Control.Image, layout.imageBounds, layout.imageStart, layout);
            }
        }

        // here for DropDownButton
        internal virtual void DrawImageCore(Graphics graphics, Image image, Rectangle imageBounds, Point imageStart, LayoutData layout)
        {
            Region oldClip = graphics.Clip;

            if (!layout.options.everettButtonCompat)
            { // FOR EVERETT COMPATIBILITY - DO NOT CHANGE
                Rectangle bounds = new Rectangle(buttonBorderSize, buttonBorderSize, Control.Width - (2 * buttonBorderSize), Control.Height - (2 * buttonBorderSize));

                Region newClip = oldClip.Clone();
                newClip.Intersect(bounds);

                // If we don't do this, DrawImageUnscaled will happily draw the entire image, even though imageBounds
                // is smaller than the image size.
                newClip.Intersect(imageBounds);
                graphics.Clip = newClip;
            }
            else
            {
                imageBounds.Width++;
                imageBounds.Height++;
                imageBounds.X = imageStart.X + 1;
                imageBounds.Y = imageStart.Y + 1;
            }

            try
            {
                if (!Control.Enabled)
                {
                    // need to specify width and height
                    ControlPaint.DrawImageDisabled(graphics, image, imageBounds, Control.BackColor, true /* unscaled image*/);
                }
                else
                {
                    graphics.DrawImage(image, imageBounds.X, imageBounds.Y, image.Width, image.Height);
                }
            }

            finally
            {
                if (!layout.options.everettButtonCompat)
                {// FOR EVERETT COMPATIBILITY - DO NOT CHANGE
                    graphics.Clip = oldClip;
                }
            }
        }

        internal static void DrawDefaultBorder(Graphics g, Rectangle r, Color c, bool isDefault)
        {
            if (isDefault)
            {
                r.Inflate(1, 1);

                Pen pen;
                if (c.IsSystemColor)
                {
                    pen = SystemPens.FromSystemColor(c);
                }
                else
                {
                    pen = new Pen(c);
                }
                g.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
                if (!c.IsSystemColor)
                {
                    pen.Dispose();
                }
            }
        }

        /// <summary>
        ///  Draws the button's text. Color c is the foreground color set with enabled/disabled state in mind.
        /// </summary>
        void DrawText(Graphics g, LayoutData layout, Color c, ColorData colors)
        {
            Rectangle r = layout.textBounds;
            bool disabledText3D = layout.options.shadowedText;

            if (Control.UseCompatibleTextRendering)
            { // Draw text using GDI+
                using (StringFormat stringFormat = CreateStringFormat())
                {
                    // DrawString doesn't seem to draw where it says it does
                    if ((Control.TextAlign & LayoutUtils.AnyCenter) == 0)
                    {
                        r.X -= 1;
                    }
                    r.Width += 1;
                    if (disabledText3D && !Control.Enabled && !colors.options.highContrast)
                    {
                        using (SolidBrush brush = new SolidBrush(colors.highlight))
                        {
                            r.Offset(1, 1);
                            g.DrawString(Control.Text, Control.Font, brush, r, stringFormat);

                            r.Offset(-1, -1);
                            brush.Color = colors.buttonShadow;
                            g.DrawString(Control.Text, Control.Font, brush, r, stringFormat);
                        }
                    }
                    else
                    {
                        Brush brush;

                        if (c.IsSystemColor)
                        {
                            brush = SystemBrushes.FromSystemColor(c);
                        }
                        else
                        {
                            brush = new SolidBrush(c);
                        }
                        g.DrawString(Control.Text, Control.Font, brush, r, stringFormat);

                        if (!c.IsSystemColor)
                        {
                            brush.Dispose();
                        }
                    }
                }
            }
            else
            { // Draw text using GDI (Whidbey+ feature).
                TextFormatFlags formatFlags = CreateTextFormatFlags();
                if (disabledText3D && !Control.Enabled && !colors.options.highContrast)
                {
                    if (Application.RenderWithVisualStyles)
                    {
                        //don't draw chiseled text if themed as win32 app does.
                        TextRenderer.DrawText(g, Control.Text, Control.Font, r, colors.buttonShadow, formatFlags);
                    }
                    else
                    {
                        r.Offset(1, 1);
                        TextRenderer.DrawText(g, Control.Text, Control.Font, r, colors.highlight, formatFlags);

                        r.Offset(-1, -1);
                        TextRenderer.DrawText(g, Control.Text, Control.Font, r, colors.buttonShadow, formatFlags);
                    }
                }
                else
                {
                    TextRenderer.DrawText(g, Control.Text, Control.Font, r, c, formatFlags);
                }
            }
        }

        #endregion Drawing Helpers

        #region Draw Content Helpers

        // the DataGridViewButtonCell uses this method
        internal static void PaintButtonBackground(WindowsGraphics wg, Rectangle bounds, WindowsBrush background)
        {
            wg.FillRectangle(background, bounds);
        }

        internal void PaintButtonBackground(PaintEventArgs e, Rectangle bounds, Brush background)
        {
            if (background == null)
            {
                Control.PaintBackground(e, bounds);
            }
            else
            {
                e.Graphics.FillRectangle(background, bounds);
            }
        }

        internal void PaintField(PaintEventArgs e,
                                 LayoutData layout,
                                 ColorData colors,
                                 Color foreColor,
                                 bool drawFocus)
        {
            Graphics g = e.Graphics;

            Rectangle maxFocus = layout.focus;

            DrawText(g, layout, foreColor, colors);

            if (drawFocus)
            {
                DrawFocus(g, maxFocus);
            }
        }

        internal void PaintImage(PaintEventArgs e, LayoutData layout)
        {
            Graphics g = e.Graphics;

            DrawImage(g, layout);
        }

        #endregion

        #region Color

        internal class ColorOptions
        {
            internal Color backColor;
            internal Color foreColor;
            internal bool enabled;
            internal bool highContrast;
            internal Graphics graphics;

            internal ColorOptions(Graphics graphics, Color foreColor, Color backColor)
            {
                this.graphics = graphics;
                this.backColor = backColor;
                this.foreColor = foreColor;
                highContrast = SystemInformation.HighContrast;
            }

            internal static int Adjust255(float percentage, int value)
            {
                int v = (int)(percentage * value);
                if (v > 255)
                {
                    return 255;
                }
                return v;
            }

            internal ColorData Calculate()
            {
                ColorData colors = new ColorData(this)
                {
                    buttonFace = backColor
                };

                if (backColor == SystemColors.Control)
                {
                    colors.buttonShadow = SystemColors.ControlDark;
                    colors.buttonShadowDark = SystemColors.ControlDarkDark;
                    colors.highlight = SystemColors.ControlLightLight;
                }
                else
                {
                    if (!highContrast)
                    {
                        colors.buttonShadow = ControlPaint.Dark(backColor);
                        colors.buttonShadowDark = ControlPaint.DarkDark(backColor);
                        colors.highlight = ControlPaint.LightLight(backColor);
                    }
                    else
                    {
                        colors.buttonShadow = ControlPaint.Dark(backColor);
                        colors.buttonShadowDark = ControlPaint.LightLight(backColor);
                        colors.highlight = ControlPaint.LightLight(backColor);
                    }
                }
                colors.windowDisabled = highContrast ? SystemColors.GrayText : colors.buttonShadow;

                const float lowlight = .1f;
                float adjust = 1 - lowlight;

                if (colors.buttonFace.GetBrightness() < .5)
                {
                    adjust = 1 + lowlight * 2;
                }
                colors.lowButtonFace = Color.FromArgb(Adjust255(adjust, colors.buttonFace.R),
                                                    Adjust255(adjust, colors.buttonFace.G),
                                                    Adjust255(adjust, colors.buttonFace.B));

                adjust = 1 - lowlight;
                if (colors.highlight.GetBrightness() < .5)
                {
                    adjust = 1 + lowlight * 2;
                }
                colors.lowHighlight = Color.FromArgb(Adjust255(adjust, colors.highlight.R),
                                                   Adjust255(adjust, colors.highlight.G),
                                                   Adjust255(adjust, colors.highlight.B));

                if (highContrast && backColor != SystemColors.Control)
                {
                    colors.highlight = colors.lowHighlight;
                }

                colors.windowFrame = foreColor;

                /* debug * /
                colors.buttonFace = Color.Yellow;
                colors.buttonShadow = Color.Blue;
                colors.highlight = Color.Brown;
                colors.lowButtonFace = Color.Beige;
                colors.lowHighlight = Color.Cyan;
                colors.windowFrame = Color.Red;
                colors.windowText = Color.Green;
                / * debug */

                if (colors.buttonFace.GetBrightness() < .5)
                {
                    colors.constrastButtonShadow = colors.lowHighlight;
                }
                else
                {
                    colors.constrastButtonShadow = colors.buttonShadow;
                }

                if (!enabled)
                {
                    colors.windowText = colors.windowDisabled;
                    if (highContrast)
                    {
                        colors.windowFrame = colors.windowDisabled;
                        colors.buttonShadow = colors.windowDisabled;
                    }
                }
                else
                {
                    colors.windowText = colors.windowFrame;
                }

                IntPtr hdc = graphics.GetHdc();

                try
                {
                    using (WindowsGraphics wg = WindowsGraphics.FromHdc(hdc))
                    {
                        colors.buttonFace = wg.GetNearestColor(colors.buttonFace);
                        colors.buttonShadow = wg.GetNearestColor(colors.buttonShadow);
                        colors.buttonShadowDark = wg.GetNearestColor(colors.buttonShadowDark);
                        colors.constrastButtonShadow = wg.GetNearestColor(colors.constrastButtonShadow);
                        colors.windowText = wg.GetNearestColor(colors.windowText);
                        colors.highlight = wg.GetNearestColor(colors.highlight);
                        colors.lowHighlight = wg.GetNearestColor(colors.lowHighlight);
                        colors.lowButtonFace = wg.GetNearestColor(colors.lowButtonFace);
                        colors.windowFrame = wg.GetNearestColor(colors.windowFrame);
                        colors.windowDisabled = wg.GetNearestColor(colors.windowDisabled);
                    }
                }
                finally
                {
                    graphics.ReleaseHdc();
                }

                return colors;
            }
        }

        internal class ColorData
        {
            internal Color buttonFace;
            internal Color buttonShadow;
            internal Color buttonShadowDark;
            internal Color constrastButtonShadow;
            internal Color windowText;
            internal Color windowDisabled;
            internal Color highlight;
            internal Color lowHighlight;
            internal Color lowButtonFace;
            internal Color windowFrame;

            internal ColorOptions options;

            internal ColorData(ColorOptions options)
            {
                this.options = options;
            }
        }

        #endregion

        #region Layout

        internal class LayoutOptions
        {
            internal Rectangle client;
            internal bool growBorderBy1PxWhenDefault;
            internal bool isDefault;
            internal int borderSize;
            internal int paddingSize;
            internal bool maxFocus;
            internal bool focusOddEvenFixup;
            internal Font font;
            internal string text;
            internal Size imageSize;
            internal int checkSize;
            internal int checkPaddingSize;
            internal ContentAlignment checkAlign;
            internal ContentAlignment imageAlign;
            internal ContentAlignment textAlign;
            internal TextImageRelation textImageRelation;
            internal bool hintTextUp;
            internal bool textOffset;
            internal bool shadowedText;
            internal bool layoutRTL;
            internal bool verticalText = false;
            internal bool useCompatibleTextRendering = false;
            internal bool everettButtonCompat = true;
            internal TextFormatFlags gdiTextFormatFlags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;
            internal StringFormatFlags gdipFormatFlags;
            internal StringTrimming gdipTrimming;
            internal HotkeyPrefix gdipHotkeyPrefix;
            internal StringAlignment gdipAlignment; // horizontal alignment.
            internal StringAlignment gdipLineAlignment; // vertical alignment.
            private bool disableWordWrapping;

            /// <summary>
            ///  We don't cache the StringFormat itself because we don't have a deterministic way of disposing it, instead
            ///  we cache the flags that make it up and create it on demand so it can be disposed by calling code.
            /// </summary>
            public StringFormat StringFormat
            {
                get
                {
                    StringFormat format = new StringFormat
                    {
                        FormatFlags = gdipFormatFlags,
                        Trimming = gdipTrimming,
                        HotkeyPrefix = gdipHotkeyPrefix,
                        Alignment = gdipAlignment,
                        LineAlignment = gdipLineAlignment
                    };

                    if (disableWordWrapping)
                    {
                        format.FormatFlags |= StringFormatFlags.NoWrap;
                    }

                    return format;
                }
                set
                {
                    gdipFormatFlags = value.FormatFlags;
                    gdipTrimming = value.Trimming;
                    gdipHotkeyPrefix = value.HotkeyPrefix;
                    gdipAlignment = value.Alignment;
                    gdipLineAlignment = value.LineAlignment;
                }
            }

            /// <summary>
            /// </summary>
            public TextFormatFlags TextFormatFlags
            {
                get
                {
                    if (disableWordWrapping)
                    {
                        return gdiTextFormatFlags & ~TextFormatFlags.WordBreak;
                    }

                    return gdiTextFormatFlags;
                }
                //set {
                //    this.gdiTextFormatFlags = value;
                //}
            }

            // textImageInset compensates for two factors: 3d text when the button is disabled,
            // and moving text on 3d-look buttons. These factors make the text require a couple
            // more pixels of space.  We inset image by the same amount so they line up.
            internal int textImageInset = 2;

            internal Padding padding;

            #region PreferredSize
            private static readonly int combineCheck = BitVector32.CreateMask();
            private static readonly int combineImageText = BitVector32.CreateMask(combineCheck);

            private enum Composition
            {
                NoneCombined = 0x00,
                CheckCombined = 0x01,
                TextImageCombined = 0x02,
                AllCombined = 0x03
            }

            // Uses checkAlign, imageAlign, and textAlign to figure out how to compose
            // checkSize, imageSize, and textSize into the preferredSize.
            private Size Compose(Size checkSize, Size imageSize, Size textSize)
            {
                Composition hComposition = GetHorizontalComposition();
                Composition vComposition = GetVerticalComposition();
                return new Size(
                    xCompose(hComposition, checkSize.Width, imageSize.Width, textSize.Width),
                    xCompose(vComposition, checkSize.Height, imageSize.Height, textSize.Height)
                );
            }

            private int xCompose(Composition composition, int checkSize, int imageSize, int textSize)
            {
                switch (composition)
                {
                    case Composition.NoneCombined:
                        return checkSize + imageSize + textSize;
                    case Composition.CheckCombined:
                        return Math.Max(checkSize, imageSize + textSize);
                    case Composition.TextImageCombined:
                        return Math.Max(imageSize, textSize) + checkSize;
                    case Composition.AllCombined:
                        return Math.Max(Math.Max(checkSize, imageSize), textSize);
                    default:
                        Debug.Fail(string.Format(SR.InvalidArgument, nameof(composition), composition.ToString()));
                        return -7107;
                }
            }

            // Uses checkAlign, imageAlign, and textAlign to figure out how to decompose
            // proposedSize into just the space left over for text.
            private Size Decompose(Size checkSize, Size imageSize, Size proposedSize)
            {
                Composition hComposition = GetHorizontalComposition();
                Composition vComposition = GetVerticalComposition();
                return new Size(
                    xDecompose(hComposition, checkSize.Width, imageSize.Width, proposedSize.Width),
                    xDecompose(vComposition, checkSize.Height, imageSize.Height, proposedSize.Height)
                );
            }

            private int xDecompose(Composition composition, int checkSize, int imageSize, int proposedSize)
            {
                switch (composition)
                {
                    case Composition.NoneCombined:
                        return proposedSize - (checkSize + imageSize);
                    case Composition.CheckCombined:
                        return proposedSize - imageSize;
                    case Composition.TextImageCombined:
                        return proposedSize - checkSize;
                    case Composition.AllCombined:
                        return proposedSize;
                    default:
                        Debug.Fail(string.Format(SR.InvalidArgument, nameof(composition), composition.ToString()));
                        return -7109;
                }
            }

            private Composition GetHorizontalComposition()
            {
                BitVector32 action = new BitVector32();

                // Checks reserve space horizontally if possible, so only AnyLeft/AnyRight prevents combination.
                action[combineCheck] = checkAlign == ContentAlignment.MiddleCenter || !LayoutUtils.IsHorizontalAlignment(checkAlign);
                action[combineImageText] = !LayoutUtils.IsHorizontalRelation(textImageRelation);
                return (Composition)action.Data;
            }

            internal Size GetPreferredSizeCore(Size proposedSize)
            {
                // Get space required for border and padding
                //
                int linearBorderAndPadding = borderSize * 2 + paddingSize * 2;
                if (growBorderBy1PxWhenDefault)
                {
                    linearBorderAndPadding += 2;
                }
                Size bordersAndPadding = new Size(linearBorderAndPadding, linearBorderAndPadding);
                proposedSize -= bordersAndPadding;

                // Get space required for Check
                //
                int checkSizeLinear = FullCheckSize;
                Size checkSize = checkSizeLinear > 0 ? new Size(checkSizeLinear + 1, checkSizeLinear) : Size.Empty;

                // Get space required for Image - textImageInset compensated for by expanding image.
                //
                Size textImageInsetSize = new Size(textImageInset * 2, textImageInset * 2);
                Size requiredImageSize = (imageSize != Size.Empty) ? imageSize + textImageInsetSize : Size.Empty;

                // Pack Text into remaning space
                //
                proposedSize -= textImageInsetSize;
                proposedSize = Decompose(checkSize, requiredImageSize, proposedSize);

                Size textSize = Size.Empty;

                if (!string.IsNullOrEmpty(text))
                {
                    // When Button.AutoSizeMode is set to GrowOnly TableLayoutPanel expects buttons not to automatically wrap on word break. If
                    // there's enough room for the text to word-wrap then it will happen but the layout would not be adjusted to allow text wrapping.
                    // If someone has a carriage return in the text we'll honor that for preferred size, but we wont wrap based on constraints.
                    try
                    {
                        disableWordWrapping = true;
                        textSize = GetTextSize(proposedSize) + textImageInsetSize;
                    }
                    finally
                    {
                        disableWordWrapping = false;
                    }
                }

                // Combine pieces to get final preferred size
                //
                Size requiredSize = Compose(checkSize, imageSize, textSize);
                requiredSize += bordersAndPadding;

                return requiredSize;
            }

            private Composition GetVerticalComposition()
            {
                BitVector32 action = new BitVector32();

                // Checks reserve space horizontally if possible, so only Top/Bottom prevents combination.
                action[combineCheck] = checkAlign == ContentAlignment.MiddleCenter || !LayoutUtils.IsVerticalAlignment(checkAlign);
                action[combineImageText] = !LayoutUtils.IsVerticalRelation(textImageRelation);
                return (Composition)action.Data;
            }
            #endregion PreferredSize

            private int FullBorderSize
            {
                get
                {
                    int result = borderSize;
                    if (OnePixExtraBorder)
                    {
                        borderSize++;
                    }
                    return borderSize;
                }
            }

            private bool OnePixExtraBorder
            {
                get { return growBorderBy1PxWhenDefault && isDefault; }
            }

            internal LayoutData Layout()
            {
                LayoutData layout = new LayoutData(this)
                {
                    client = client
                };

                // subtract border size from layout area
                int fullBorderSize = FullBorderSize;
                layout.face = Rectangle.Inflate(layout.client, -fullBorderSize, -fullBorderSize);

                // checkBounds, checkArea, field
                //
                CalcCheckmarkRectangle(layout);

                // imageBounds, imageLocation, textBounds
                LayoutTextAndImage(layout);

                // focus
                //
                if (maxFocus)
                {
                    layout.focus = layout.field;
                    layout.focus.Inflate(-1, -1);

                    // Adjust for padding.
                    layout.focus = LayoutUtils.InflateRect(layout.focus, padding);
                }
                else
                {
                    Rectangle textAdjusted = new Rectangle(layout.textBounds.X - 1, layout.textBounds.Y - 1,
                                                           layout.textBounds.Width + 2, layout.textBounds.Height + 3);
                    if (imageSize != Size.Empty)
                    {
                        layout.focus = Rectangle.Union(textAdjusted, layout.imageBounds);
                    }
                    else
                    {
                        layout.focus = textAdjusted;
                    }
                }
                if (focusOddEvenFixup)
                {
                    if (layout.focus.Height % 2 == 0)
                    {
                        layout.focus.Y++;
                        layout.focus.Height--;
                    }
                    if (layout.focus.Width % 2 == 0)
                    {
                        layout.focus.X++;
                        layout.focus.Width--;
                    }
                }

                return layout;
            }

            TextImageRelation RtlTranslateRelation(TextImageRelation relation)
            {
                // If RTL, we swap ImageBeforeText and TextBeforeImage
                if (layoutRTL)
                {
                    switch (relation)
                    {
                        case TextImageRelation.ImageBeforeText:
                            return TextImageRelation.TextBeforeImage;
                        case TextImageRelation.TextBeforeImage:
                            return TextImageRelation.ImageBeforeText;
                    }
                }
                return relation;
            }

            internal ContentAlignment RtlTranslateContent(ContentAlignment align)
            {

                if (layoutRTL)
                {
                    ContentAlignment[][] mapping = new ContentAlignment[3][];
                    mapping[0] = new ContentAlignment[2] { ContentAlignment.TopLeft, ContentAlignment.TopRight };
                    mapping[1] = new ContentAlignment[2] { ContentAlignment.MiddleLeft, ContentAlignment.MiddleRight };
                    mapping[2] = new ContentAlignment[2] { ContentAlignment.BottomLeft, ContentAlignment.BottomRight };

                    for (int i = 0; i < 3; ++i)
                    {
                        if (mapping[i][0] == align)
                        {
                            return mapping[i][1];
                        }
                        else if (mapping[i][1] == align)
                        {
                            return mapping[i][0];
                        }
                    }
                }
                return align;
            }

            private int FullCheckSize
            {
                get
                {
                    return checkSize + checkPaddingSize;
                }
            }

            void CalcCheckmarkRectangle(LayoutData layout)
            {
                int checkSizeFull = FullCheckSize;
                layout.checkBounds = new Rectangle(client.X, client.Y, checkSizeFull, checkSizeFull);

                // Translate checkAlign for Rtl applications
                ContentAlignment align = RtlTranslateContent(checkAlign);

                Rectangle field = Rectangle.Inflate(layout.face, -paddingSize, -paddingSize);

                layout.field = field;

                if (checkSizeFull > 0)
                {
                    if ((align & LayoutUtils.AnyRight) != 0)
                    {
                        layout.checkBounds.X = (field.X + field.Width) - layout.checkBounds.Width;
                    }
                    else if ((align & LayoutUtils.AnyCenter) != 0)
                    {
                        layout.checkBounds.X = field.X + (field.Width - layout.checkBounds.Width) / 2;
                    }

                    if ((align & LayoutUtils.AnyBottom) != 0)
                    {
                        layout.checkBounds.Y = (field.Y + field.Height) - layout.checkBounds.Height;
                    }
                    else if ((align & LayoutUtils.AnyTop) != 0)
                    {
                        layout.checkBounds.Y = field.Y + 2; // + 2: this needs to be aligned to the text (
                    }
                    else
                    {
                        layout.checkBounds.Y = field.Y + (field.Height - layout.checkBounds.Height) / 2;
                    }

                    switch (align)
                    {
                        case ContentAlignment.TopLeft:
                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.BottomLeft:
                            layout.checkArea.X = field.X;
                            layout.checkArea.Width = checkSizeFull + 1;

                            layout.checkArea.Y = field.Y;
                            layout.checkArea.Height = field.Height;

                            layout.field.X += checkSizeFull + 1;
                            layout.field.Width -= checkSizeFull + 1;
                            break;
                        case ContentAlignment.TopRight:
                        case ContentAlignment.MiddleRight:
                        case ContentAlignment.BottomRight:
                            layout.checkArea.X = field.X + field.Width - checkSizeFull;
                            layout.checkArea.Width = checkSizeFull + 1;

                            layout.checkArea.Y = field.Y;
                            layout.checkArea.Height = field.Height;

                            layout.field.Width -= checkSizeFull + 1;
                            break;
                        case ContentAlignment.TopCenter:
                            layout.checkArea.X = field.X;
                            layout.checkArea.Width = field.Width;

                            layout.checkArea.Y = field.Y;
                            layout.checkArea.Height = checkSizeFull;

                            layout.field.Y += checkSizeFull;
                            layout.field.Height -= checkSizeFull;
                            break;

                        case ContentAlignment.BottomCenter:
                            layout.checkArea.X = field.X;
                            layout.checkArea.Width = field.Width;

                            layout.checkArea.Y = field.Y + field.Height - checkSizeFull;
                            layout.checkArea.Height = checkSizeFull;

                            layout.field.Height -= checkSizeFull;
                            break;

                        case ContentAlignment.MiddleCenter:
                            layout.checkArea = layout.checkBounds;
                            break;
                    }

                    layout.checkBounds.Width -= checkPaddingSize;
                    layout.checkBounds.Height -= checkPaddingSize;
                }
            }

            // Maps an image align to the set of TextImageRelations that represent the same edge.
            // For example, imageAlign = TopLeft maps to TextImageRelations ImageAboveText (top)
            // and ImageBeforeText (left).
            private static readonly TextImageRelation[] _imageAlignToRelation = new TextImageRelation[] {
                /* TopLeft = */       TextImageRelation.ImageAboveText | TextImageRelation.ImageBeforeText,
                /* TopCenter = */     TextImageRelation.ImageAboveText,
                /* TopRight = */      TextImageRelation.ImageAboveText | TextImageRelation.TextBeforeImage,
                /* Invalid */         0,
                /* MiddleLeft = */    TextImageRelation.ImageBeforeText,
                /* MiddleCenter = */  0,
                /* MiddleRight = */   TextImageRelation.TextBeforeImage,
                /* Invalid */         0,
                /* BottomLeft = */    TextImageRelation.TextAboveImage | TextImageRelation.ImageBeforeText,
                /* BottomCenter = */  TextImageRelation.TextAboveImage,
                /* BottomRight = */   TextImageRelation.TextAboveImage | TextImageRelation.TextBeforeImage
            };

            private static TextImageRelation ImageAlignToRelation(ContentAlignment alignment)
            {
                return _imageAlignToRelation[LayoutUtils.ContentAlignmentToIndex(alignment)];
            }

            private static TextImageRelation TextAlignToRelation(ContentAlignment alignment)
            {
                return LayoutUtils.GetOppositeTextImageRelation(ImageAlignToRelation(alignment));
            }

            internal void LayoutTextAndImage(LayoutData layout)
            {
                // Translate for Rtl applications.  This intentially shadows the member variables.
                ContentAlignment imageAlign = RtlTranslateContent(this.imageAlign);
                ContentAlignment textAlign = RtlTranslateContent(this.textAlign);
                TextImageRelation textImageRelation = RtlTranslateRelation(this.textImageRelation);

                // Figure out the maximum bounds for text & image
                Rectangle maxBounds = Rectangle.Inflate(layout.field, -textImageInset, -textImageInset);
                if (OnePixExtraBorder)
                {
                    maxBounds.Inflate(1, 1);
                }

                // Compute the final image and text bounds.
                if (imageSize == Size.Empty || text == null || text.Length == 0 || textImageRelation == TextImageRelation.Overlay)
                {
                    // Do not worry about text/image overlaying
                    Size textSize = GetTextSize(maxBounds.Size);

                    // FOR EVERETT COMPATIBILITY - DO NOT CHANGE
                    Size size = imageSize;
                    if (layout.options.everettButtonCompat && imageSize != Size.Empty)
                    {
                        size = new Size(size.Width + 1, size.Height + 1);
                    }

                    layout.imageBounds = LayoutUtils.Align(size, maxBounds, imageAlign);
                    layout.textBounds = LayoutUtils.Align(textSize, maxBounds, textAlign);

                }
                else
                {
                    // Rearrage text/image to prevent overlay.  Pack text into maxBounds - space reserved for image
                    Size maxTextSize = LayoutUtils.SubAlignedRegion(maxBounds.Size, imageSize, textImageRelation);
                    Size textSize = GetTextSize(maxTextSize);
                    Rectangle maxCombinedBounds = maxBounds;

                    // Combine text & image into one rectangle that we center within maxBounds.
                    Size combinedSize = LayoutUtils.AddAlignedRegion(textSize, imageSize, textImageRelation);
                    maxCombinedBounds.Size = LayoutUtils.UnionSizes(maxCombinedBounds.Size, combinedSize);
                    Rectangle combinedBounds = LayoutUtils.Align(combinedSize, maxCombinedBounds, ContentAlignment.MiddleCenter);

                    // imageEdge indicates whether the combination of imageAlign and textImageRelation place
                    // the image along the edge of the control.  If so, we can increase the space for text.
                    bool imageEdge = (AnchorStyles)(ImageAlignToRelation(imageAlign) & textImageRelation) != AnchorStyles.None;

                    // textEdge indicates whether the combination of textAlign and textImageRelation place
                    // the text along the edge of the control.  If so, we can increase the space for image.
                    bool textEdge = (AnchorStyles)(TextAlignToRelation(textAlign) & textImageRelation) != AnchorStyles.None;

                    if (imageEdge)
                    {
                        // If imageEdge, just split imageSize off of maxCombinedBounds.
                        LayoutUtils.SplitRegion(maxCombinedBounds, imageSize, (AnchorStyles)textImageRelation, out layout.imageBounds, out layout.textBounds);
                    }
                    else if (textEdge)
                    {
                        // Else if textEdge, just split textSize off of maxCombinedBounds.
                        LayoutUtils.SplitRegion(maxCombinedBounds, textSize, (AnchorStyles)LayoutUtils.GetOppositeTextImageRelation(textImageRelation), out layout.textBounds, out layout.imageBounds);
                    }
                    else
                    {
                        // Expand the adjacent regions to maxCombinedBounds (centered) and split the rectangle into imageBounds and textBounds.
                        LayoutUtils.SplitRegion(combinedBounds, imageSize, (AnchorStyles)textImageRelation, out layout.imageBounds, out layout.textBounds);
                        LayoutUtils.ExpandRegionsToFillBounds(maxCombinedBounds, (AnchorStyles)textImageRelation, ref layout.imageBounds, ref layout.textBounds);
                    }

                    // align text/image within their regions.
                    layout.imageBounds = LayoutUtils.Align(imageSize, layout.imageBounds, imageAlign);
                    layout.textBounds = LayoutUtils.Align(textSize, layout.textBounds, textAlign);
                }

                //Don't call "layout.imageBounds = Rectangle.Intersect(layout.imageBounds, maxBounds);"
                // because that is a breaking change that causes images to be scaled to the dimensions of the control.
                //adjust textBounds so that the text is still visible even if the image is larger than the button's size

                //why do we intersect with layout.field for textBounds while we intersect with maxBounds for imageBounds?
                //this is because there are some legacy code which squeezes the button so small that text will get clipped
                //if we intersect with maxBounds. Have to do this for backward compatibility.

                if (textImageRelation == TextImageRelation.TextBeforeImage || textImageRelation == TextImageRelation.ImageBeforeText)
                {
                    //adjust the vertical position of textBounds so that the text doesn't fall off the boundary of the button
                    int textBottom = Math.Min(layout.textBounds.Bottom, layout.field.Bottom);
                    layout.textBounds.Y = Math.Max(Math.Min(layout.textBounds.Y, layout.field.Y + (layout.field.Height - layout.textBounds.Height) / 2), layout.field.Y);
                    layout.textBounds.Height = textBottom - layout.textBounds.Y;
                }
                if (textImageRelation == TextImageRelation.TextAboveImage || textImageRelation == TextImageRelation.ImageAboveText)
                {
                    //adjust the horizontal position of textBounds so that the text doesn't fall off the boundary of the button
                    int textRight = Math.Min(layout.textBounds.Right, layout.field.Right);
                    layout.textBounds.X = Math.Max(Math.Min(layout.textBounds.X, layout.field.X + (layout.field.Width - layout.textBounds.Width) / 2), layout.field.X);
                    layout.textBounds.Width = textRight - layout.textBounds.X;
                }
                if (textImageRelation == TextImageRelation.ImageBeforeText && layout.imageBounds.Size.Width != 0)
                {
                    //squeezes imageBounds.Width so that text is visible
                    layout.imageBounds.Width = Math.Max(0, Math.Min(maxBounds.Width - layout.textBounds.Width, layout.imageBounds.Width));
                    layout.textBounds.X = layout.imageBounds.X + layout.imageBounds.Width;
                }
                if (textImageRelation == TextImageRelation.ImageAboveText && layout.imageBounds.Size.Height != 0)
                {
                    //squeezes imageBounds.Height so that the text is visible
                    layout.imageBounds.Height = Math.Max(0, Math.Min(maxBounds.Height - layout.textBounds.Height, layout.imageBounds.Height));
                    layout.textBounds.Y = layout.imageBounds.Y + layout.imageBounds.Height;
                }
                //make sure that textBound is contained in layout.field
                layout.textBounds = Rectangle.Intersect(layout.textBounds, layout.field);
                if (hintTextUp)
                {
                    layout.textBounds.Y--;
                }
                if (textOffset)
                {
                    layout.textBounds.Offset(1, 1);
                }

                // FOR EVERETT COMPATIBILITY - DO NOT CHANGE
                if (layout.options.everettButtonCompat)
                {
                    layout.imageStart = layout.imageBounds.Location;
                    layout.imageBounds = Rectangle.Intersect(layout.imageBounds, layout.field);
                }
                else if (!Application.RenderWithVisualStyles)
                {
                    // Not sure why this is here, but we can't remove it, since it might break
                    // ToolStrips on non-themed machines
                    layout.textBounds.X++;
                }

                // clip
                //
                int bottom;
                // If we are using GDI to measure text, then we can get into a situation, where
                // the proposed height is ignore. In this case, we want to clip it against
                // maxbounds.
                if (!useCompatibleTextRendering)
                {
                    bottom = Math.Min(layout.textBounds.Bottom, maxBounds.Bottom);
                    layout.textBounds.Y = Math.Max(layout.textBounds.Y, maxBounds.Y);
                }
                else
                {
                    // If we are using GDI+ (like Everett), then use the old Everett code
                    // This ensures that we have pixel-level rendering compatibility
                    bottom = Math.Min(layout.textBounds.Bottom, layout.field.Bottom);
                    layout.textBounds.Y = Math.Max(layout.textBounds.Y, layout.field.Y);
                }
                layout.textBounds.Height = bottom - layout.textBounds.Y;

                //This causes a breaking change because images get shrunk to the new clipped size instead of clipped.
                //********** bottom = Math.Min(layout.imageBounds.Bottom, maxBounds.Bottom);
                //********** layout.imageBounds.Y = Math.Max(layout.imageBounds.Y, maxBounds.Y);
                //********** layout.imageBounds.Height = bottom - layout.imageBounds.Y;

            }

            protected virtual Size GetTextSize(Size proposedSize)
            {
                //set the Prefix field of TextFormatFlags
                proposedSize = LayoutUtils.FlipSizeIf(verticalText, proposedSize);
                Size textSize = Size.Empty;

                if (useCompatibleTextRendering)
                { // GDI+ text rendering.
                    using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
                    {
                        using (StringFormat gdipStringFormat = StringFormat)
                        {
                            textSize = Size.Ceiling(g.MeasureString(text, font, new SizeF(proposedSize.Width, proposedSize.Height), gdipStringFormat));
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(text))
                { // GDI text rendering (Whidbey feature).
                    textSize = TextRenderer.MeasureText(text, font, proposedSize, TextFormatFlags);
                }
                //else skip calling MeasureText, it should return 0,0

                return LayoutUtils.FlipSizeIf(verticalText, textSize);
            }

#if DEBUG
            public override string ToString()
            {
                return
                    "{ client = " + client + "\n" +
                    "OnePixExtraBorder = " + OnePixExtraBorder + "\n" +
                    "borderSize = " + borderSize + "\n" +
                    "paddingSize = " + paddingSize + "\n" +
                    "maxFocus = " + maxFocus + "\n" +
                    "font = " + font + "\n" +
                    "text = " + text + "\n" +
                    "imageSize = " + imageSize + "\n" +
                    "checkSize = " + checkSize + "\n" +
                    "checkPaddingSize = " + checkPaddingSize + "\n" +
                    "checkAlign = " + checkAlign + "\n" +
                    "imageAlign = " + imageAlign + "\n" +
                    "textAlign = " + textAlign + "\n" +
                    "textOffset = " + textOffset + "\n" +
                    "shadowedText = " + shadowedText + "\n" +
                    "textImageRelation = " + textImageRelation + "\n" +
                    "layoutRTL = " + layoutRTL + " }";
            }
#endif
        }

        internal class LayoutData
        {
            internal Rectangle client;
            internal Rectangle face;
            internal Rectangle checkArea;
            internal Rectangle checkBounds;
            internal Rectangle textBounds;
            internal Rectangle field;
            internal Rectangle focus;
            internal Rectangle imageBounds;
            internal Point imageStart; // FOR EVERETT COMPATIBILITY - DO NOT CHANGE
            internal LayoutOptions options;

            internal LayoutData(LayoutOptions options)
            {
                Debug.Assert(options != null, "must have options");
                this.options = options;
            }
        }

        #endregion

        #region Layout

        // used by the DataGridViewButtonCell
        internal static LayoutOptions CommonLayout(Rectangle clientRectangle, Padding padding, bool isDefault, Font font, string text, bool enabled, ContentAlignment textAlign, RightToLeft rtl)
        {
            LayoutOptions layout = new LayoutOptions
            {
                client = LayoutUtils.DeflateRect(clientRectangle, padding),
                padding = padding,
                growBorderBy1PxWhenDefault = true,
                isDefault = isDefault,
                borderSize = 2,
                paddingSize = 0,
                maxFocus = true,
                focusOddEvenFixup = false,
                font = font,
                text = text,
                imageSize = Size.Empty,
                checkSize = 0,
                checkPaddingSize = 0,
                checkAlign = ContentAlignment.TopLeft,
                imageAlign = ContentAlignment.MiddleCenter,
                textAlign = textAlign,
                hintTextUp = false,
                shadowedText = !enabled,
                layoutRTL = RightToLeft.Yes == rtl,
                textImageRelation = TextImageRelation.Overlay,
                useCompatibleTextRendering = false
            };
            return layout;
        }

        internal virtual LayoutOptions CommonLayout()
        {
            LayoutOptions layout = new LayoutOptions
            {
                client = LayoutUtils.DeflateRect(Control.ClientRectangle, Control.Padding),
                padding = Control.Padding,
                growBorderBy1PxWhenDefault = true,
                isDefault = Control.IsDefault,
                borderSize = 2,
                paddingSize = 0,
                maxFocus = true,
                focusOddEvenFixup = false,
                font = Control.Font,
                text = Control.Text,
                imageSize = (Control.Image == null) ? Size.Empty : Control.Image.Size,
                checkSize = 0,
                checkPaddingSize = 0,
                checkAlign = ContentAlignment.TopLeft,
                imageAlign = Control.ImageAlign,
                textAlign = Control.TextAlign,
                hintTextUp = false,
                shadowedText = !Control.Enabled,
                layoutRTL = RightToLeft.Yes == Control.RightToLeft,
                textImageRelation = Control.TextImageRelation,
                useCompatibleTextRendering = Control.UseCompatibleTextRendering
            };

            if (Control.FlatStyle != FlatStyle.System)
            {
                if (layout.useCompatibleTextRendering)
                {
                    using (StringFormat format = Control.CreateStringFormat())
                    {
                        layout.StringFormat = format;
                    }
                }
                else
                {
                    layout.gdiTextFormatFlags = Control.CreateTextFormatFlags();
                }
            }

            return layout;
        }

        // used by the DataGridViewButtonCell
        static ColorOptions CommonRender(Graphics g, Color foreColor, Color backColor, bool enabled)
        {
            ColorOptions colors = new ColorOptions(g, foreColor, backColor)
            {
                enabled = enabled
            };
            return colors;
        }

        ColorOptions CommonRender(Graphics g)
        {
            ColorOptions colors = new ColorOptions(g, Control.ForeColor, Control.BackColor)
            {
                enabled = Control.Enabled
            };
            return colors;
        }

        protected ColorOptions PaintRender(Graphics g)
        {
            return CommonRender(g);
        }

        // used by the DataGridViewButtonCell
        internal static ColorOptions PaintFlatRender(Graphics g, Color foreColor, Color backColor, bool enabled)
        {
            return CommonRender(g, foreColor, backColor, enabled);
        }

        protected ColorOptions PaintFlatRender(Graphics g)
        {
            return CommonRender(g);
        }

        // used by the DataGridViewButtonCell
        internal static ColorOptions PaintPopupRender(Graphics g, Color foreColor, Color backColor, bool enabled)
        {
            return CommonRender(g, foreColor, backColor, enabled);
        }

        protected ColorOptions PaintPopupRender(Graphics g)
        {
            return CommonRender(g);
        }

        #endregion

    }
}
