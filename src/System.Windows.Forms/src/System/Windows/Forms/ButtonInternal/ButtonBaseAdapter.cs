﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms.ButtonInternal
{
    /// <summary>
    ///  This class is used for more than just <see cref="Button"/>. It is used for things that derive from
    ///  <see cref="ButtonBase"/>, parts of <see cref="ToolStripItem"/>, and parts of <see cref="DataGridView"/>.
    /// </summary>
    internal abstract partial class ButtonBaseAdapter
    {
        // SystemInformation.Border3DSize + 2 pixels for focus rect
        protected const int ButtonBorderSize = 4;

        internal ButtonBaseAdapter(ButtonBase control) => Control = control;

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
            => SystemInformation.HighContrast
                && Application.RenderWithVisualStyles
                && (Control.Focused || Control.MouseIsOver || (Control.IsDefault && Control.Enabled));

        internal static Brush CreateDitherBrush(Color color1, Color color2)
        {
            // Note: Don't dispose the bitmap here. The texture brush will take ownership
            // of the bitmap. So the bitmap will get disposed by the brush's Dispose().

            using Bitmap b = new Bitmap(2, 2);

            b.SetPixel(0, 0, color1);
            b.SetPixel(0, 1, color2);
            b.SetPixel(1, 1, color1);
            b.SetPixel(1, 0, color2);

            return new TextureBrush(b);
        }

        /// <summary>
        ///  Get StringFormat object for rendering text using GDI+ (Graphics).
        /// </summary>
        internal virtual StringFormat CreateStringFormat()
            => ControlPaint.CreateStringFormat(Control, Control.TextAlign, Control.ShowToolTip, Control.UseMnemonic);

        /// <summary>
        ///  Get TextFormatFlags flags for rendering text using GDI (TextRenderer).
        /// </summary>
        internal virtual TextFormatFlags CreateTextFormatFlags()
            => ControlPaint.CreateTextFormatFlags(Control, Control.TextAlign, Control.ShowToolTip, Control.UseMnemonic);

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
            bool stockColor = colors.ButtonFace.ToKnownColor() == SystemColors.Control.ToKnownColor();
            bool disabledHighContrast = (!Control.Enabled) && SystemInformation.HighContrast;

            using var hdc = new DeviceContextHdcScope(deviceContext);

            // Draw counter-clock-wise
            Point p1 = new Point(bounds.X + bounds.Width - 1, bounds.Y);                        // Upper inner right
            Point p2 = new Point(bounds.X, bounds.Y);                                           // Upper left
            Point p3 = new Point(bounds.X, bounds.Y + bounds.Height - 1);                       // Bottom inner left
            Point p4 = new Point(bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);    // Inner bottom right

            // Top + left
            using var penTopLeft = new Gdi32.CreatePenScope(
                disabledHighContrast ? colors.WindowDisabled : stockColor ? SystemColors.ControlLightLight : colors.Highlight);

            hdc.DrawLine(penTopLeft, p1, p2);           // Top  (right-left)
            hdc.DrawLine(penTopLeft, p2, p3);           // Left (up-down)

            // Bottom + right
            using var penBottomRight = new Gdi32.CreatePenScope(
                disabledHighContrast
                    ? colors.WindowDisabled
                    : stockColor ? SystemColors.ControlDarkDark : colors.ButtonShadowDark);

            p1.Offset(0, -1);                           // Need to paint last pixel too.
            hdc.DrawLine(penBottomRight, p3, p4);       // Bottom (left-right)
            hdc.DrawLine(penBottomRight, p4, p1);       // Right  (bottom-up)

            // Draw inset using the background color to make the top and left lines thinner
            using var insetPen = new Gdi32.CreatePenScope(
                stockColor
                    ? SystemInformation.HighContrast ? SystemColors.ControlLight : SystemColors.Control
                    : SystemInformation.HighContrast ? colors.Highlight : colors.ButtonFace);

            p1.Offset(-1, 2);
            p2.Offset(1, 1);
            p3.Offset(1, -1);
            p4.Offset(-1, -1);

            // Top + left inset
            hdc.DrawLine(insetPen, p1, p2);             // Top  (right-left)
            hdc.DrawLine(insetPen, p2, p3);             // Left (up-down)

            // Bottom + right inset
            using var bottomRightInsetPen = new Gdi32.CreatePenScope(
                disabledHighContrast ? colors.WindowDisabled : stockColor ? SystemColors.ControlDark : colors.ButtonShadow);

            p1.Offset(0, -1);                           // Need to paint last pixel too.
            hdc.DrawLine(bottomRightInsetPen, p3, p4);  // Bottom (left-right)
            hdc.DrawLine(bottomRightInsetPen, p4, p1);  // Right  (bottom-up)
        }

        private void Draw3DBorderNormal(IDeviceContext deviceContext, ref Rectangle bounds, ColorData colors)
        {
            using var hdc = new DeviceContextHdcScope(deviceContext);

            // Draw counter-clock-wise
            Point p1 = new Point(bounds.X + bounds.Width - 1, bounds.Y);                        // Upper inner right
            Point p2 = new Point(bounds.X, bounds.Y);                                           // Upper left
            Point p3 = new Point(bounds.X, bounds.Y + bounds.Height - 1);                       // Bottom inner left
            Point p4 = new Point(bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);    // Inner bottom right

            // Top + left
            using var shadowPen = new Gdi32.CreatePenScope(colors.ButtonShadowDark);
            hdc.DrawLine(shadowPen, p1, p2);                                                    // Top (right-left)
            hdc.DrawLine(shadowPen, p2, p3);                                                    // Left(up-down)

            // Bottom + right
            using var highlightPen = new Gdi32.CreatePenScope(colors.Highlight);
            p1.Offset(0, -1);                       // Need to paint last pixel too.
            hdc.DrawLine(highlightPen, p3, p4);     // Bottom (left-right)
            hdc.DrawLine(highlightPen, p4, p1);     // Right  (bottom-up)

            // Draw inset

            using var facePen = new Gdi32.CreatePenScope(colors.ButtonFace);

            p1.Offset(-1, 2);
            p2.Offset(1, 1);
            p3.Offset(1, -1);
            p4.Offset(-1, -1);

            // Top + left inset
            hdc.DrawLine(facePen, p1, p2);          // Top  (right-left)
            hdc.DrawLine(facePen, p2, p3);          // Left (up-down)

            // Bottom + right inset
            using var insetPen = new Gdi32.CreatePenScope(
                colors.ButtonFace.ToKnownColor() == SystemColors.Control.ToKnownColor()
                    ? SystemColors.ControlLight
                    : colors.ButtonFace);

            p1.Offset(0, -1);                       // Need to paint last pixel too.
            hdc.DrawLine(insetPen, p3, p4);         // Bottom (left-right)
            hdc.DrawLine(insetPen, p4, p1);         // Right  (bottom-up)
        }

        private void Draw3DBorderRaised(IDeviceContext deviceContext, ref Rectangle bounds, ColorData colors)
        {
            bool stockColor = colors.ButtonFace.ToKnownColor() == SystemColors.Control.ToKnownColor();
            bool disabledHighContrast = (!Control.Enabled) && SystemInformation.HighContrast;

            using var hdc = new DeviceContextHdcScope(deviceContext);

            // Draw counter-clock-wise.
            Point p1 = new Point(bounds.X + bounds.Width - 1, bounds.Y);                        // Upper inner right
            Point p2 = new Point(bounds.X, bounds.Y);                                           // Upper left
            Point p3 = new Point(bounds.X, bounds.Y + bounds.Height - 1);                       // Bottom inner left
            Point p4 = new Point(bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);    // Inner bottom right

            // Top + left
            using var topLeftPen = new Gdi32.CreatePenScope(
                disabledHighContrast
                    ? colors.WindowDisabled
                    : stockColor ? SystemColors.ControlLightLight : colors.Highlight);

            hdc.DrawLine(topLeftPen, p1, p2);           // Top  (right-left)
            hdc.DrawLine(topLeftPen, p2, p3);           // Left (up-down)

            // Bottom + right
            using var bottomRightPen = new Gdi32.CreatePenScope(
                disabledHighContrast ? colors.WindowDisabled : stockColor ? SystemColors.ControlDarkDark : colors.ButtonShadowDark);

            p1.Offset(0, -1);                           // Need to paint last pixel too.
            hdc.DrawLine(bottomRightPen, p3, p4);       // Bottom (left-right)
            hdc.DrawLine(bottomRightPen, p4, p1);       // Right  (bottom-up)

            // Draw inset - use the back ground color here to have a thinner border.
            p1.Offset(-1, 2);
            p2.Offset(1, 1);
            p3.Offset(1, -1);
            p4.Offset(-1, -1);

            using var topLeftInsetPen = new Gdi32.CreatePenScope(
                !stockColor
                    ? colors.ButtonFace
                    : SystemInformation.HighContrast ? SystemColors.ControlLight : SystemColors.Control);

            // Top + left inset
            hdc.DrawLine(topLeftInsetPen, p1, p2);      // Top  (right-left)
            hdc.DrawLine(topLeftInsetPen, p2, p3);      // Left (up-down)

            // Bottom + right inset

            using var bottomRightInsetPen = new Gdi32.CreatePenScope(
                disabledHighContrast ? colors.WindowDisabled : stockColor ? SystemColors.ControlDark : colors.ButtonShadow);

            p1.Offset(0, -1);                           // Need to paint last pixel too.
            hdc.DrawLine(bottomRightInsetPen, p3, p4);  // Bottom (left-right)
            hdc.DrawLine(bottomRightInsetPen, p4, p1);  // Right  (bottom-up)
        }

        /// <summary>
        ///  Draws a border for the in the 3D style of the popup button.
        /// </summary>
        protected internal static void Draw3DLiteBorder(IDeviceContext deviceContext, Rectangle r, ColorData colors, bool up)
        {
            using var hdc = new DeviceContextHdcScope(deviceContext);

            // Draw counter-clock-wise.
            Point p1 = new Point(r.Right - 1, r.Top);           // Upper inner right
            Point p2 = new Point(r.Left, r.Top);                // Upper left
            Point p3 = new Point(r.Left, r.Bottom - 1);         // Bottom inner left
            Point p4 = new Point(r.Right - 1, r.Bottom - 1);    // Inner bottom right

            // Top, left
            using var topLeftPen = new Gdi32.CreatePenScope(up ? colors.Highlight : colors.ButtonShadow);

            hdc.DrawLine(topLeftPen, p1, p2);                   // top  (right-left)
            hdc.DrawLine(topLeftPen, p2, p3);                   // left (top-down)

            // Bottom, right
            using var bottomRightPen = new Gdi32.CreatePenScope(up ? colors.ButtonShadow : colors.Highlight);

            p1.Offset(0, -1);                                   // Need to paint last pixel too.
            hdc.DrawLine(bottomRightPen, p3, p4);               // Bottom (left-right)
            hdc.DrawLine(bottomRightPen, p4, p1);               // Right  (bottom-up)
        }

        /// <summary>
        ///  Draws the flat border with specified bordersize.
        /// </summary>
        /// <remarks>
        ///  This function gets called only for Flatstyle == Flatstyle.Flat.
        /// </remarks>
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

            if (!layout.Options.DotNetOneButtonCompat)
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
                if (!layout.Options.DotNetOneButtonCompat)
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
                Graphics graphics = deviceContext.TryGetGraphics(create: true);
                if (graphics is not null)
                {
                    using var pen = color.GetCachedPenScope();
                    graphics.DrawRectangle(pen, r.X, r.Y, r.Width - 1, r.Height - 1);
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
            Rectangle r = layout.TextBounds;
            bool disabledText3D = layout.Options.ShadowedText;

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
                if (disabledText3D && !Control.Enabled && !colors.Options.HighContrast)
                {
                    using var highlightBrush = colors.Highlight.GetCachedSolidBrushScope();
                    r.Offset(1, 1);
                    g.DrawString(Control.Text, Control.Font, highlightBrush, r, stringFormat);

                    r.Offset(-1, -1);
                    using var shadowBrush = colors.ButtonShadow.GetCachedSolidBrushScope();
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
                // Draw text using GDI (.NET Framework 2.0+ feature).
                TextFormatFlags formatFlags = CreateTextFormatFlags();
                if (disabledText3D && !Control.Enabled && !colors.Options.HighContrast)
                {
                    if (Application.RenderWithVisualStyles)
                    {
                        //don't draw chiseled text if themed as win32 app does.
                        TextRenderer.DrawTextInternal(e, Control.Text, Control.Font, r, colors.ButtonShadow, formatFlags);
                    }
                    else
                    {
                        r.Offset(1, 1);
                        TextRenderer.DrawTextInternal(e, Control.Text, Control.Font, r, colors.Highlight, formatFlags);

                        r.Offset(-1, -1);
                        TextRenderer.DrawTextInternal(e, Control.Text, Control.Font, r, colors.ButtonShadow, formatFlags);
                    }
                }
                else
                {
                    TextRenderer.DrawTextInternal(e, Control.Text, Control.Font, r, color, formatFlags);
                }
            }
        }

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
            Rectangle maxFocus = layout.Focus;

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
            if (Control.Image is not null)
            {
                // Setup new clip region & draw
                DrawImageCore(e.GraphicsInternal, Control.Image, layout.ImageBounds, layout.ImageStart, layout);
            }
        }

        internal static LayoutOptions CommonLayout(
            Rectangle clientRectangle,
            Padding padding,
            bool isDefault,
            Font font,
            string text,
            bool enabled,
            ContentAlignment textAlign,
            RightToLeft rtl)
        {
            LayoutOptions layout = new LayoutOptions
            {
                Client = LayoutUtils.DeflateRect(clientRectangle, padding),
                Padding = padding,
                GrowBorderBy1PxWhenDefault = true,
                IsDefault = isDefault,
                BorderSize = 2,
                PaddingSize = 0,
                MaxFocus = true,
                FocusOddEvenFixup = false,
                Font = font,
                Text = text,
                ImageSize = Size.Empty,
                CheckSize = 0,
                CheckPaddingSize = 0,
                CheckAlign = ContentAlignment.TopLeft,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextAlign = textAlign,
                HintTextUp = false,
                ShadowedText = !enabled,
                LayoutRTL = RightToLeft.Yes == rtl,
                TextImageRelation = TextImageRelation.Overlay,
                UseCompatibleTextRendering = false
            };

            return layout;
        }

        internal virtual LayoutOptions CommonLayout()
        {
            LayoutOptions layout = new LayoutOptions
            {
                Client = LayoutUtils.DeflateRect(Control.ClientRectangle, Control.Padding),
                Padding = Control.Padding,
                GrowBorderBy1PxWhenDefault = true,
                IsDefault = Control.IsDefault,
                BorderSize = 2,
                PaddingSize = 0,
                MaxFocus = true,
                FocusOddEvenFixup = false,
                Font = Control.Font,
                Text = Control.Text,
                ImageSize = (Control.Image is null) ? Size.Empty : Control.Image.Size,
                CheckSize = 0,
                CheckPaddingSize = 0,
                CheckAlign = ContentAlignment.TopLeft,
                ImageAlign = Control.ImageAlign,
                TextAlign = Control.TextAlign,
                HintTextUp = false,
                ShadowedText = !Control.Enabled,
                LayoutRTL = RightToLeft.Yes == Control.RightToLeft,
                TextImageRelation = Control.TextImageRelation,
                UseCompatibleTextRendering = Control.UseCompatibleTextRendering
            };

            if (Control.FlatStyle != FlatStyle.System)
            {
                if (layout.UseCompatibleTextRendering)
                {
                    using StringFormat format = Control.CreateStringFormat();
                    layout.StringFormat = format;
                }
                else
                {
                    layout.GdiTextFormatFlags = Control.CreateTextFormatFlags();
                }
            }

            return layout;
        }

        private static ColorOptions CommonRender(
            IDeviceContext deviceContext,
            Color foreColor,
            Color backColor,
            bool enabled)
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
            => CommonRender(deviceContext);

        internal static ColorOptions PaintFlatRender(Graphics g, Color foreColor, Color backColor, bool enabled)
            => CommonRender(g, foreColor, backColor, enabled);

        protected ColorOptions PaintFlatRender(IDeviceContext deviceContext)
            => CommonRender(deviceContext);

        internal static ColorOptions PaintPopupRender(Graphics g, Color foreColor, Color backColor, bool enabled)
            => CommonRender(g, foreColor, backColor, enabled);

        protected ColorOptions PaintPopupRender(IDeviceContext deviceContext)
            => CommonRender(deviceContext);
    }
}
