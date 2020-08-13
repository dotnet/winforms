// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms.ButtonInternal
{
    /// <summary>
    ///  This class is used for more than just <see cref="Button"/>. It is used for things that derive from
    ///  <see cref="ButtonBase"/>, parts of <see cref="ToolStripItem"/>, and parts of <see cref="DataGridView"/>.
    /// </summary>
    internal abstract class ButtonBaseAdapter
    {
        // SystemInformation.Border3DSize + 2 pixels for focus rect
        protected const int ButtonBorderSize = 4;

        internal ButtonBaseAdapter(ButtonBase control)
            => Control = control;

        protected ButtonBase Control { get; }

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
            LayoutOptions options = default;
            using (var screen = GdiCache.GetScreenHdc())
            using (PaintEventArgs pe = new PaintEventArgs(screen, new Rectangle()))
            {
                options = Layout(pe);
            }

            return options.GetPreferredSizeCore(proposedSize);
        }

        protected abstract LayoutOptions Layout(PaintEventArgs e);

        internal abstract void PaintUp(PaintEventArgs e, CheckState state);

        internal abstract void PaintDown(PaintEventArgs e, CheckState state);

        internal abstract void PaintOver(PaintEventArgs e, CheckState state);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool IsHighContrastHighlighted()
            => SystemInformation.HighContrast && Application.RenderWithVisualStyles &&
                (Control.Focused || Control.MouseIsOver || (Control.IsDefault && Control.Enabled));

        #region Drawing Helpers

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
            using Brush brush = CreateDitherBrush(color1, color2);
            g.FillRectangle(brush, bounds);
        }

        protected void Draw3DBorder(IDeviceContext deviceContext, Rectangle bounds, ColorData colors, bool raised)
        {
            if (Control.BackColor != SystemColors.Control && SystemInformation.HighContrast)
            {
                if (raised)
                {
                    Draw3DBorderHighContrastRaised(deviceContext, ref bounds, colors);
                }
                else
                {
                    ControlPaint.DrawBorderSimple(deviceContext, bounds, ControlPaint.Dark(Control.BackColor));
                }
            }
            else
            {
                if (raised)
                {
                    Draw3DBorderRaised(deviceContext, ref bounds, colors);
                }
                else
                {
                    Draw3DBorderNormal(deviceContext, ref bounds, colors);
                }
            }
        }

        private void Draw3DBorderHighContrastRaised(IDeviceContext deviceContext, ref Rectangle bounds, ColorData colors)
        {
            bool stockColor = colors.buttonFace.ToKnownColor() == SystemColors.Control.ToKnownColor();
            bool disabledHighContrast = (!Control.Enabled) && SystemInformation.HighContrast;

            using var hdc = new DeviceContextHdcScope(deviceContext);

            // Draw counter-clock-wise.
            Point p1 = new Point(bounds.X + bounds.Width - 1, bounds.Y);  // upper inner right.
            Point p2 = new Point(bounds.X, bounds.Y);  // upper left.
            Point p3 = new Point(bounds.X, bounds.Y + bounds.Height - 1);  // bottom inner left.
            Point p4 = new Point(bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);  // inner bottom right.

            // top + left
            using var penTopLeft = new Gdi32.CreatePenScope(
                disabledHighContrast ? colors.windowDisabled : stockColor ? SystemColors.ControlLightLight : colors.highlight);

            hdc.DrawLine(penTopLeft, p1, p2); // top  (right-left)
            hdc.DrawLine(penTopLeft, p2, p3); // left (up-down)

            // bottom + right
            using var penBottomRight = new Gdi32.CreatePenScope(
                disabledHighContrast ? colors.windowDisabled : stockColor ? SystemColors.ControlDarkDark : colors.buttonShadowDark);

            p1.Offset(0, -1); // need to paint last pixel too.
            hdc.DrawLine(penBottomRight, p3, p4);  // bottom (left-right)
            hdc.DrawLine(penBottomRight, p4, p1);  // right  (bottom-up )

            // Draw inset using the background color to make the top and left lines thinner
            using var insetPen = new Gdi32.CreatePenScope(
                stockColor
                    ? SystemInformation.HighContrast ? SystemColors.ControlLight : SystemColors.Control
                    : SystemInformation.HighContrast ? colors.highlight : colors.buttonFace);

            p1.Offset(-1, 2);
            p2.Offset(1, 1);
            p3.Offset(1, -1);
            p4.Offset(-1, -1);

            // top + left inset
            hdc.DrawLine(insetPen, p1, p2); // top (right-left)
            hdc.DrawLine(insetPen, p2, p3); // left( up-down)

            // Bottom + right inset
            using var bottomRightInsetPen = new Gdi32.CreatePenScope(
                disabledHighContrast ? colors.windowDisabled : stockColor ? SystemColors.ControlDark : colors.buttonShadow);

            p1.Offset(0, -1); // need to paint last pixel too.
            hdc.DrawLine(bottomRightInsetPen, p3, p4); // bottom (left-right)
            hdc.DrawLine(bottomRightInsetPen, p4, p1); // right  (bottom-up)
        }

        private void Draw3DBorderNormal(IDeviceContext deviceContext, ref Rectangle bounds, ColorData colors)
        {
            using var hdc = new DeviceContextHdcScope(deviceContext);

            // Draw counter-clock-wise.
            Point p1 = new Point(bounds.X + bounds.Width - 1, bounds.Y);  // upper inner right.
            Point p2 = new Point(bounds.X, bounds.Y);  // upper left.
            Point p3 = new Point(bounds.X, bounds.Y + bounds.Height - 1);  // bottom inner left.
            Point p4 = new Point(bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);  // inner bottom right.

            // top + left
            using var shadowPen = new Gdi32.CreatePenScope(colors.buttonShadowDark);
            hdc.DrawLine(shadowPen, p1, p2); // top (right-left)
            hdc.DrawLine(shadowPen, p2, p3); // left(up-down)

            // bottom + right
            using var highlightPen = new Gdi32.CreatePenScope(colors.highlight);
            p1.Offset(0, -1); // need to paint last pixel too.
            hdc.DrawLine(highlightPen, p3, p4); // bottom(left-right)
            hdc.DrawLine(highlightPen, p4, p1); // right (bottom-up)

            // Draw inset

            using var facePen = new Gdi32.CreatePenScope(colors.buttonFace);

            p1.Offset(-1, 2);
            p2.Offset(1, 1);
            p3.Offset(1, -1);
            p4.Offset(-1, -1);

            // top + left inset
            hdc.DrawLine(facePen, p1, p2); // top (right-left)
            hdc.DrawLine(facePen, p2, p3); // left(up-down)

            // bottom + right inset
            using var insetPen = new Gdi32.CreatePenScope(
                colors.buttonFace.ToKnownColor() == SystemColors.Control.ToKnownColor()
                    ? SystemColors.ControlLight
                    : colors.buttonFace);

            p1.Offset(0, -1); // need to paint last pixel too.
            hdc.DrawLine(insetPen, p3, p4); // bottom(left-right)
            hdc.DrawLine(insetPen, p4, p1); // right (bottom-up)
        }

        private void Draw3DBorderRaised(IDeviceContext deviceContext, ref Rectangle bounds, ColorData colors)
        {
            bool stockColor = colors.buttonFace.ToKnownColor() == SystemColors.Control.ToKnownColor();
            bool disabledHighContrast = (!Control.Enabled) && SystemInformation.HighContrast;

            using var hdc = new DeviceContextHdcScope(deviceContext);

            // Draw counter-clock-wise.
            Point p1 = new Point(bounds.X + bounds.Width - 1, bounds.Y);  // upper inner right.
            Point p2 = new Point(bounds.X, bounds.Y);  // upper left.
            Point p3 = new Point(bounds.X, bounds.Y + bounds.Height - 1);  // bottom inner left.
            Point p4 = new Point(bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);  // inner bottom right.

            // Draw counter-clock-wise.

            // top + left
            using var topLeftPen = new Gdi32.CreatePenScope(
                disabledHighContrast ? colors.windowDisabled : stockColor ? SystemColors.ControlLightLight : colors.highlight);

            hdc.DrawLine(topLeftPen, p1, p2);   // top (right-left)
            hdc.DrawLine(topLeftPen, p2, p3);   // left(up-down)

            // bottom + right
            using var bottomRightPen = new Gdi32.CreatePenScope(
                disabledHighContrast ? colors.windowDisabled : stockColor ? SystemColors.ControlDarkDark : colors.buttonShadowDark);

            p1.Offset(0, -1); // need to paint last pixel too.
            hdc.DrawLine(bottomRightPen, p3, p4);    // bottom(left-right)
            hdc.DrawLine(bottomRightPen, p4, p1);    // right (bottom-up)

            // Draw inset - use the back ground color here to have a thinner border
            p1.Offset(-1, 2);
            p2.Offset(1, 1);
            p3.Offset(1, -1);
            p4.Offset(-1, -1);

            using var topLeftInsetPen = new Gdi32.CreatePenScope(
                !stockColor ? colors.buttonFace : SystemInformation.HighContrast ? SystemColors.ControlLight : SystemColors.Control);

            // top + left inset
            hdc.DrawLine(topLeftInsetPen, p1, p2); // top (right-left)
            hdc.DrawLine(topLeftInsetPen, p2, p3); // left(up-down)

            // Bottom + right inset

            using var bottomRightInsetPen = new Gdi32.CreatePenScope(
                disabledHighContrast ? colors.windowDisabled : stockColor ? SystemColors.ControlDark : colors.buttonShadow);

            p1.Offset(0, -1); // need to paint last pixel too.
            hdc.DrawLine(bottomRightInsetPen, p3, p4);  // bottom(left-right)
            hdc.DrawLine(bottomRightInsetPen, p4, p1);  // right (bottom-up)
        }

        /// <summary>
        ///  Draws a border for the in the 3D style of the popup button.
        /// </summary>
        protected internal static void Draw3DLiteBorder(IDeviceContext deviceContext, Rectangle r, ColorData colors, bool up)
        {
            using var hdc = new DeviceContextHdcScope(deviceContext);

            // Draw counter-clock-wise.
            Point p1 = new Point(r.Right - 1, r.Top);  // upper inner right.
            Point p2 = new Point(r.Left, r.Top);  // upper left.
            Point p3 = new Point(r.Left, r.Bottom - 1);  // bottom inner left.
            Point p4 = new Point(r.Right - 1, r.Bottom - 1);  // inner bottom right.

            // top, left
            using var topLeftPen = new Gdi32.CreatePenScope(up ? colors.highlight : colors.buttonShadow);

            hdc.DrawLine(topLeftPen, p1, p2); // top (right-left)
            hdc.DrawLine(topLeftPen, p2, p3); // left (top-down)

            // bottom, right
            using var bottomRightPen = new Gdi32.CreatePenScope(up ? colors.buttonShadow : colors.highlight);

            p1.Offset(0, -1); // need to paint last pixel too.
            hdc.DrawLine(bottomRightPen, p3, p4); // bottom (left-right)
            hdc.DrawLine(bottomRightPen, p4, p1); // right(bottom-up)
        }

        /// <summary>
        ///  Draws the flat border with specified bordersize.
        ///  This function gets called only for Flatstyle == Flatstyle.Flat.
        /// </summary>
        internal static void DrawFlatBorderWithSize(
            PaintEventArgs e,
            Rectangle bounds,
            Color color,
            int size)
        {
            size = Math.Min(size, Math.Min(bounds.Width, bounds.Height));

            var left = new Rectangle(bounds.X, bounds.Y, size, bounds.Height);
            var right = new Rectangle(bounds.X + bounds.Width - size, bounds.Y, size, bounds.Height);
            var top = new Rectangle(bounds.X + size, bounds.Y, bounds.Width - size * 2, size);
            var bottom = new Rectangle(bounds.X + size, bounds.Y + bounds.Height - size, bounds.Width - size * 2, size);

            if (color.HasTransparency())
            {
                Graphics g = e.GraphicsInternal;
                using var brush = color.GetCachedSolidBrushScope();
                g.FillRectangle(brush, left);
                g.FillRectangle(brush, right);
                g.FillRectangle(brush, top);
                g.FillRectangle(brush, bottom);
                return;
            }

            using var hdc = new DeviceContextHdcScope(e);
            using var hbrush = new Gdi32.CreateBrushScope(color);
            hdc.FillRectangle(left, hbrush);
            hdc.FillRectangle(right, hbrush);
            hdc.FillRectangle(top, hbrush);
            hdc.FillRectangle(bottom, hbrush);
        }

        internal static void DrawFlatFocus(IDeviceContext deviceContext, Rectangle r, Color color)
        {
            using var hdc = new DeviceContextHdcScope(deviceContext);
            using var focusPen = new Gdi32.CreatePenScope(color);
            hdc.DrawRectangle(r, focusPen);
        }

        /// <summary>
        ///  Draws the focus rectangle if the control has focus.
        /// </summary>
        private void DrawFocus(Graphics g, Rectangle r)
        {
            if (Control.Focused && Control.ShowFocusCues)
            {
                ControlPaint.DrawFocusRectangle(g, r, Control.ForeColor, Control.BackColor);
            }
        }

        // here for DropDownButton
        internal virtual void DrawImageCore(Graphics graphics, Image image, Rectangle imageBounds, Point imageStart, LayoutData layout)
        {
            Region oldClip = graphics.Clip;

            if (!layout.options.everettButtonCompat)
            {
                Rectangle bounds = new Rectangle(
                    ButtonBorderSize,
                    ButtonBorderSize,
                    Control.Width - (2 * ButtonBorderSize),
                    Control.Height - (2 * ButtonBorderSize));

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
                    // Need to specify width and height
                    ControlPaint.DrawImageDisabled(graphics, image, imageBounds, unscaledImage: true);
                }
                else
                {
                    graphics.DrawImage(image, imageBounds.X, imageBounds.Y, image.Width, image.Height);
                }
            }
            finally
            {
                if (!layout.options.everettButtonCompat)
                {
                    graphics.Clip = oldClip;
                }
            }
        }

        internal static void DrawDefaultBorder(IDeviceContext deviceContext, Rectangle r, Color color, bool isDefault)
        {
            if (!isDefault)
            {
                return;
            }

            r.Inflate(1, 1);

            if (color.HasTransparency())
            {
                Graphics g = deviceContext.TryGetGraphics(create: true);
                if (g != null)
                {
                    using var pen = color.GetCachedPenScope();
                    g.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
                    return;
                }
            }

            using var hpen = new Gdi32.CreatePenScope(color);
            using var hdc = new DeviceContextHdcScope(deviceContext);
            hdc.DrawRectangle(r, hpen);
        }

        /// <summary>
        ///  Draws the button's text. Color c is the foreground color set with enabled/disabled state in mind.
        /// </summary>
        private void DrawText(PaintEventArgs e, LayoutData layout, Color color, ColorData colors)
        {
            Rectangle r = layout.textBounds;
            bool disabledText3D = layout.options.shadowedText;

            if (Control.UseCompatibleTextRendering)
            {
                Graphics g = e.GraphicsInternal;

                // Draw text using GDI+
                using StringFormat stringFormat = CreateStringFormat();

                // DrawString doesn't seem to draw where it says it does
                if ((Control.TextAlign & LayoutUtils.AnyCenter) == 0)
                {
                    r.X -= 1;
                }

                r.Width += 1;
                if (disabledText3D && !Control.Enabled && !colors.options.HighContrast)
                {
                    using var highlightBrush = colors.highlight.GetCachedSolidBrushScope();
                    r.Offset(1, 1);
                    g.DrawString(Control.Text, Control.Font, highlightBrush, r, stringFormat);

                    r.Offset(-1, -1);
                    using var shadowBrush = colors.buttonShadow.GetCachedSolidBrushScope();
                    g.DrawString(Control.Text, Control.Font, shadowBrush, r, stringFormat);
                }
                else
                {
                    using var brush = color.GetCachedSolidBrushScope();

                    g.DrawString(Control.Text, Control.Font, brush, r, stringFormat);
                }
            }
            else
            {
                // Draw text using GDI (.NET 2.0+ feature).
                TextFormatFlags formatFlags = CreateTextFormatFlags();
                if (disabledText3D && !Control.Enabled && !colors.options.HighContrast)
                {
                    if (Application.RenderWithVisualStyles)
                    {
                        //don't draw chiseled text if themed as win32 app does.
                        TextRenderer.DrawTextInternal(e, Control.Text, Control.Font, r, colors.buttonShadow, formatFlags);
                    }
                    else
                    {
                        r.Offset(1, 1);
                        TextRenderer.DrawTextInternal(e, Control.Text, Control.Font, r, colors.highlight, formatFlags);

                        r.Offset(-1, -1);
                        TextRenderer.DrawTextInternal(e, Control.Text, Control.Font, r, colors.buttonShadow, formatFlags);
                    }
                }
                else
                {
                    TextRenderer.DrawTextInternal(e, Control.Text, Control.Font, r, color, formatFlags);
                }
            }
        }

        #endregion Drawing Helpers

        #region Draw Content Helpers

        internal void PaintButtonBackground(PaintEventArgs e, Rectangle bounds, Brush background)
        {
            if (background is null)
            {
                Control.PaintBackground(e, bounds);
            }
            else
            {
                e.GraphicsInternal.FillRectangle(background, bounds);
            }
        }

        internal void PaintField(
            PaintEventArgs e,
            LayoutData layout,
            ColorData colors,
            Color foreColor,
            bool drawFocus)
        {
            Rectangle maxFocus = layout.focus;

            DrawText(e, layout, foreColor, colors);

            if (drawFocus)
            {
                DrawFocus(e.GraphicsInternal, maxFocus);
            }
        }

        /// <summary>
        ///  Draws the button's image.
        /// </summary>
        internal void PaintImage(PaintEventArgs e, LayoutData layout)
        {
            if (Control.Image != null)
            {
                // Setup new clip region & draw
                DrawImageCore(e.GraphicsInternal, Control.Image, layout.imageBounds, layout.imageStart, layout);
            }
        }

        #endregion

        #region Color

        internal class ColorOptions
        {
            private readonly Color _backColor;
            private readonly Color _foreColor;
            public bool Enabled { get; set; }
            public bool HighContrast { get; }
            private readonly IDeviceContext _deviceContext;

            internal ColorOptions(IDeviceContext deviceContext, Color foreColor, Color backColor)
            {
                _deviceContext = deviceContext;
                _backColor = backColor;
                _foreColor = foreColor;
                HighContrast = SystemInformation.HighContrast;
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
                    buttonFace = _backColor
                };

                if (_backColor == SystemColors.Control)
                {
                    colors.buttonShadow = SystemColors.ControlDark;
                    colors.buttonShadowDark = SystemColors.ControlDarkDark;
                    colors.highlight = SystemColors.ControlLightLight;
                }
                else
                {
                    if (!HighContrast)
                    {
                        colors.buttonShadow = ControlPaint.Dark(_backColor);
                        colors.buttonShadowDark = ControlPaint.DarkDark(_backColor);
                        colors.highlight = ControlPaint.LightLight(_backColor);
                    }
                    else
                    {
                        colors.buttonShadow = ControlPaint.Dark(_backColor);
                        colors.buttonShadowDark = ControlPaint.LightLight(_backColor);
                        colors.highlight = ControlPaint.LightLight(_backColor);
                    }
                }
                colors.windowDisabled = HighContrast ? SystemColors.GrayText : colors.buttonShadow;

                const float lowlight = .1f;
                float adjust = 1 - lowlight;

                if (colors.buttonFace.GetBrightness() < .5)
                {
                    adjust = 1 + lowlight * 2;
                }

                colors.lowButtonFace = Color.FromArgb(
                    Adjust255(adjust, colors.buttonFace.R),
                    Adjust255(adjust, colors.buttonFace.G),
                    Adjust255(adjust, colors.buttonFace.B));

                adjust = 1 - lowlight;
                if (colors.highlight.GetBrightness() < .5)
                {
                    adjust = 1 + lowlight * 2;
                }

                colors.lowHighlight = Color.FromArgb(
                    Adjust255(adjust, colors.highlight.R),
                    Adjust255(adjust, colors.highlight.G),
                    Adjust255(adjust, colors.highlight.B));

                if (HighContrast && _backColor != SystemColors.Control)
                {
                    colors.highlight = colors.lowHighlight;
                }

                colors.windowFrame = _foreColor;

                if (colors.buttonFace.GetBrightness() < .5)
                {
                    colors.constrastButtonShadow = colors.lowHighlight;
                }
                else
                {
                    colors.constrastButtonShadow = colors.buttonShadow;
                }

                if (!Enabled)
                {
                    colors.windowText = colors.windowDisabled;
                    if (HighContrast)
                    {
                        colors.windowFrame = colors.windowDisabled;
                        colors.buttonShadow = colors.windowDisabled;
                    }
                }
                else
                {
                    colors.windowText = colors.windowFrame;
                }

                using var hdc = new DeviceContextHdcScope(_deviceContext, applyGraphicsState: false);

                colors.buttonFace = hdc.FindNearestColor(colors.buttonFace);
                colors.buttonShadow = hdc.FindNearestColor(colors.buttonShadow);
                colors.buttonShadowDark = hdc.FindNearestColor(colors.buttonShadowDark);
                colors.constrastButtonShadow = hdc.FindNearestColor(colors.constrastButtonShadow);
                colors.windowText = hdc.FindNearestColor(colors.windowText);
                colors.highlight = hdc.FindNearestColor(colors.highlight);
                colors.lowHighlight = hdc.FindNearestColor(colors.lowHighlight);
                colors.lowButtonFace = hdc.FindNearestColor(colors.lowButtonFace);
                colors.windowFrame = hdc.FindNearestColor(colors.windowFrame);
                colors.windowDisabled = hdc.FindNearestColor(colors.windowDisabled);

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
            internal bool verticalText;
            internal bool useCompatibleTextRendering;
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
                if (imageSize == Size.Empty || text is null || text.Length == 0 || textImageRelation == TextImageRelation.Overlay)
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
                // Set the Prefix field of TextFormatFlags
                proposedSize = LayoutUtils.FlipSizeIf(verticalText, proposedSize);
                Size textSize = Size.Empty;

                if (useCompatibleTextRendering)
                {
                    // GDI+ text rendering.
                    using var screen = GdiCache.GetScreenDCGraphics();
                    using StringFormat gdipStringFormat = StringFormat;
                    textSize = Size.Ceiling(
                        screen.Graphics.MeasureString(text, font, new SizeF(proposedSize.Width, proposedSize.Height),
                        gdipStringFormat));
                }
                else if (!string.IsNullOrEmpty(text))
                {
                    // GDI text rendering (Whidbey feature).
                    textSize = TextRenderer.MeasureText(text, font, proposedSize, TextFormatFlags);
                }

                // Else skip calling MeasureText, it should return 0,0

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
                imageSize = (Control.Image is null) ? Size.Empty : Control.Image.Size,
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
                    using StringFormat format = Control.CreateStringFormat();
                    layout.StringFormat = format;
                }
                else
                {
                    layout.gdiTextFormatFlags = Control.CreateTextFormatFlags();
                }
            }

            return layout;
        }

        // used by the DataGridViewButtonCell
        private static ColorOptions CommonRender(IDeviceContext deviceContext, Color foreColor, Color backColor, bool enabled)
        {
            ColorOptions colors = new ColorOptions(deviceContext, foreColor, backColor)
            {
                Enabled = enabled
            };
            return colors;
        }

        private ColorOptions CommonRender(IDeviceContext deviceContext)
        {
            ColorOptions colors = new ColorOptions(deviceContext, Control.ForeColor, Control.BackColor)
            {
                Enabled = Control.Enabled
            };
            return colors;
        }

        protected ColorOptions PaintRender(IDeviceContext deviceContext)
        {
            return CommonRender(deviceContext);
        }

        // used by the DataGridViewButtonCell
        internal static ColorOptions PaintFlatRender(Graphics g, Color foreColor, Color backColor, bool enabled)
        {
            return CommonRender(g, foreColor, backColor, enabled);
        }

        protected ColorOptions PaintFlatRender(IDeviceContext deviceContext)
        {
            return CommonRender(deviceContext);
        }

        // used by the DataGridViewButtonCell
        internal static ColorOptions PaintPopupRender(Graphics g, Color foreColor, Color backColor, bool enabled)
        {
            return CommonRender(g, foreColor, backColor, enabled);
        }

        protected ColorOptions PaintPopupRender(IDeviceContext deviceContext)
        {
            return CommonRender(deviceContext);
        }

        #endregion

    }
}
