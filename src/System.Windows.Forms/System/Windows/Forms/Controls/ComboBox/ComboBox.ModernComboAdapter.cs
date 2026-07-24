// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Rendering.Button;

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Paints the modern ComboBox field and drop-down button over the native control.
    /// </summary>
    internal sealed class ModernComboAdapter : FlatComboAdapter
    {
        private readonly Rectangle _clientBounds;
        private readonly Rectangle _buttonBounds;
        private readonly FlatStyle _flatStyle;
        private readonly ComboBoxStyle _dropDownStyle;
        private readonly int _deviceDpi;

        public ModernComboAdapter(ComboBox comboBox)
            : base(comboBox, smallButton: false)
        {
            _clientBounds = comboBox.ClientRectangle;
            _flatStyle = comboBox.FlatStyle;
            _dropDownStyle = comboBox.DropDownStyle;
            _deviceDpi = comboBox.DeviceDpiInternal;

            if (_dropDownStyle == ComboBoxStyle.Simple)
            {
                _buttonBounds = Rectangle.Empty;
                _dropDownRect = Rectangle.Empty;
                return;
            }

            int buttonWidth = SystemInformation.GetHorizontalScrollBarArrowWidthForDpi(
                _deviceDpi)
                + ScaleHelper.ScaleToDpi(
                    ModernControlVisualStyles.ComboBoxButtonExtraWidth,
                    _deviceDpi);
            _buttonBounds = new Rectangle(
                comboBox.RightToLeft == RightToLeft.Yes
                    ? _clientBounds.Left
                    : Math.Max(_clientBounds.Left, _clientBounds.Right - buttonWidth),
                _clientBounds.Top,
                Math.Min(buttonWidth, _clientBounds.Width),
                _clientBounds.Height);
            _dropDownRect = _buttonBounds;
        }

        public override bool IsValid(ComboBox combo)
            => base.IsValid(combo)
                && combo.ClientRectangle == _clientBounds
                && combo.FlatStyle == _flatStyle
                && combo.DropDownStyle == _dropDownStyle
                && combo.DeviceDpiInternal == _deviceDpi
                && combo.UsesModernComboAdapter;

        public override void DrawFlatCombo(
            ComboBox comboBox,
            Graphics graphics)
        {
            Rectangle clientBounds = comboBox.ClientRectangle;
            if (clientBounds.Width <= 1 || clientBounds.Height <= 1)
            {
                return;
            }

            Rectangle borderBounds = clientBounds;
            borderBounds.Width--;
            borderBounds.Height--;

            using GraphicsStateScope state = new(graphics);
            if (_flatStyle != FlatStyle.Flat)
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
            }

            if (_dropDownStyle != ComboBoxStyle.DropDownList
                || comboBox.DrawMode == DrawMode.Normal)
            {
                PaintFieldSurface(
                    comboBox,
                    graphics,
                    clientBounds);
            }

            DrawDropDownListText(
                comboBox,
                graphics,
                clientBounds);
            DrawDropDownButton(comboBox, graphics);

            switch (_flatStyle)
            {
                case FlatStyle.Standard:
                    DrawStandardFrame(
                        comboBox,
                        graphics,
                        clientBounds,
                        borderBounds);
                    break;
                case FlatStyle.Flat:
                    DrawFlatFrame(
                        comboBox,
                        graphics,
                        borderBounds);
                    break;
                case FlatStyle.Popup:
                    DrawPopupFrame(
                        comboBox,
                        graphics,
                        clientBounds,
                        borderBounds);
                    break;
            }
        }

        private static void PaintFieldSurface(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle bounds)
        {
            // Paint the entire modern field surface in one pass. This covers any client-area
            // frame the native ComboBox drew (the sharp inner rectangle), so only our rounded
            // field, the drop-down button, and the flat edit child remain visible. The rounded
            // corners are restored afterwards by CutOutRoundedCorners, and the single rounded
            // border is drawn last, spanning the full control.
            Color background = GetEffectiveFieldColor(comboBox);
            using var brush = background.GetCachedSolidBrushScope();
            graphics.FillRectangle(brush, bounds);
        }

        private static void DrawStandardFrame(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle clientBounds,
            Rectangle borderBounds)
        {
            Color background = GetEffectiveBackColor(comboBox);
            Color borderColor = GetBorderColor(
                comboBox,
                background,
                useAccent: false);

            CutOutRoundedCorners(
                comboBox,
                graphics,
                clientBounds);
            DrawRoundedBorder(
                comboBox,
                graphics,
                borderBounds,
                borderColor);
        }

        private static void DrawFlatFrame(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            Color background = GetEffectiveBackColor(comboBox);
            Color borderColor = GetBorderColor(
                comboBox,
                background,
                useAccent: false);
            using var pen = borderColor.GetCachedPenScope(
                GetBorderThickness(comboBox));
            graphics.DrawRectangle(pen, bounds);
        }

        private static void DrawPopupFrame(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle clientBounds,
            Rectangle borderBounds)
        {
            Color background = GetEffectiveBackColor(comboBox);
            Color borderColor = GetBorderColor(
                comboBox,
                background,
                useAccent: true);
            CutOutRoundedCorners(
                comboBox,
                graphics,
                clientBounds);
            DrawRoundedBorder(
                comboBox,
                graphics,
                borderBounds,
                borderColor);
        }

        private void DrawDropDownButton(
            ComboBox comboBox,
            Graphics graphics)
        {
            if (_buttonBounds.IsEmpty)
            {
                return;
            }

            Color background = GetEffectiveBackColor(comboBox);
            Color buttonColor = comboBox._mousePressed
                ? PopupButtonColorMath.Blend(
                    background,
                    Application.SystemVisualSettings.AccentColor,
                    0.24f)
                : comboBox.MouseIsOver || comboBox.ContainsFocus
                    ? PopupButtonColorMath.Blend(
                        background,
                        Application.SystemVisualSettings.AccentColor,
                        0.12f)
                    : PopupButtonColorMath.TowardsContrast(
                        background,
                        0.035f);
            if (!comboBox.Enabled)
            {
                buttonColor = PopupButtonColorMath.Mute(
                    buttonColor,
                    0.55f);
            }

            using (var brush = buttonColor.GetCachedSolidBrushScope())
            {
                graphics.FillRectangle(brush, _buttonBounds);
            }

            Color chevronColor = comboBox.Enabled
                ? PopupButtonColorMath.GetReadableForeColor(buttonColor)
                : ModernControlColorMath.GetDisabledTextColor(
                    comboBox.ForeColor,
                    buttonColor);
            int halfWidth = Math.Max(
                2,
                ScaleHelper.ScaleToDpi(3, _deviceDpi));
            int halfHeight = Math.Max(
                1,
                ScaleHelper.ScaleToDpi(2, _deviceDpi));
            int stroke = Math.Max(
                1,
                ScaleHelper.ScaleToDpi(
                    ModernControlVisualStyles.BorderThickness,
                    _deviceDpi));
            Point center = new(
                _buttonBounds.Left + (_buttonBounds.Width / 2),
                _buttonBounds.Top + (_buttonBounds.Height / 2));
            Point[] points =
            [
                new(center.X - halfWidth, center.Y - halfHeight),
                new(center.X, center.Y + halfHeight),
                new(center.X + halfWidth, center.Y - halfHeight)
            ];
            using var pen = chevronColor.GetCachedPenScope(stroke);
            graphics.DrawLines(pen, points);
        }

        private void DrawDropDownListText(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle clientBounds)
        {
            if (_dropDownStyle != ComboBoxStyle.DropDownList
                || comboBox.DrawMode != DrawMode.Normal)
            {
                return;
            }

            Rectangle selectionBounds = clientBounds;
            if (comboBox.RightToLeft == RightToLeft.Yes)
            {
                selectionBounds.X = _buttonBounds.Right;
                selectionBounds.Width = Math.Max(
                    0,
                    clientBounds.Right - selectionBounds.X);
            }
            else
            {
                selectionBounds.Width = Math.Max(
                    0,
                    _buttonBounds.Left - clientBounds.Left);
            }

            bool drawFocusedSelection = comboBox.ContainsFocus;
            Color background = drawFocusedSelection
                ? SystemColors.Highlight
                : GetEffectiveFieldColor(comboBox);
            using (var backgroundBrush = background.GetCachedSolidBrushScope())
            {
                graphics.FillRectangle(
                    backgroundBrush,
                    selectionBounds);
            }

            Rectangle textBounds = GetDropDownListTextBounds(
                comboBox,
                clientBounds);
            if (textBounds.Width <= 0
                || textBounds.Height <= 0
                || string.IsNullOrEmpty(comboBox.Text))
            {
                return;
            }

            Color textColor = drawFocusedSelection
                ? SystemColors.HighlightText
                : comboBox.Enabled
                    ? comboBox.ShouldSerializeForeColor()
                        ? comboBox.ForeColor
                        : PopupButtonColorMath.GetReadableForeColor(
                            background)
                    : ModernControlColorMath.GetDisabledTextColor(
                        comboBox.ForeColor,
                        background);
            TextFormatFlags flags = TextFormatFlags.SingleLine
                | TextFormatFlags.VerticalCenter
                | TextFormatFlags.EndEllipsis
                | TextFormatFlags.NoPrefix
                | TextFormatFlags.PreserveGraphicsClipping
                | TextFormatFlags.PreserveGraphicsTranslateTransform;
            if (comboBox.RightToLeft == RightToLeft.Yes)
            {
                flags |= TextFormatFlags.Right
                    | TextFormatFlags.RightToLeft;
            }

            TextRenderer.DrawText(
                graphics,
                comboBox.Text,
                comboBox.Font,
                textBounds,
                textColor,
                background,
                flags);

            if (drawFocusedSelection
                && comboBox.ShowFocusCues)
            {
                ControlPaint.DrawFocusRectangle(
                    graphics,
                    textBounds,
                    textColor,
                    background);
            }
        }

        private Rectangle GetDropDownListTextBounds(
            ComboBox comboBox,
            Rectangle clientBounds)
        {
            Padding padding = comboBox.GetModernFieldPadding();
            int left = comboBox.RightToLeft == RightToLeft.Yes
                ? _buttonBounds.Right + padding.Left
                : clientBounds.Left + padding.Left;
            int right = comboBox.RightToLeft == RightToLeft.Yes
                ? clientBounds.Right - padding.Right
                : _buttonBounds.Left - padding.Right;

            return new Rectangle(
                left,
                clientBounds.Top + padding.Top,
                Math.Max(0, right - left),
                Math.Max(
                    0,
                    clientBounds.Height - padding.Vertical));
        }

        private static void CutOutRoundedCorners(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle bounds)
        {
            using GraphicsPath path = CreateFieldPath(
                comboBox,
                bounds);
            using Region corners = new(bounds);
            corners.Exclude(path);
            Color parentColor = comboBox.ParentInternal?.BackColor
                ?? SystemColors.Control;
            using GraphicsStateScope state = new(graphics);
            graphics.SetClip(
                corners,
                CombineMode.Intersect);
            ParentBackgroundRenderer.Paint(
                comboBox,
                graphics,
                bounds,
                parentColor);
        }

        private static void DrawRoundedBorder(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle bounds,
            Color borderColor)
        {
            using GraphicsPath path = CreateFieldPath(
                comboBox,
                bounds);
            int thickness = GetBorderThickness(comboBox);
            using var pen = borderColor.GetCachedPenScope(thickness);
            graphics.DrawPath(pen, path);

            // The corners are cut out with a non-antialiased region; blend the resulting corner
            // artifacts into the parent by tracing the parent color just outside the border.
            int radius = GetFieldCornerRadius(comboBox, bounds);
            Color parentColor = comboBox.ParentInternal?.BackColor
                ?? SystemColors.Control;
            ParentBackgroundRenderer.PaintRoundedBorderRegionMitigation(
                graphics,
                bounds,
                new Size(radius, radius),
                thickness,
                parentColor);
        }

        private static Color GetBorderColor(
            ComboBox comboBox,
            Color background,
            bool useAccent)
        {
            Color borderColor = useAccent
                ? Application.SystemVisualSettings.AccentColor
                : comboBox.ForeColor;

            return comboBox.Enabled
                ? borderColor
                : ModernControlColorMath.GetDisabledTextColor(
                    borderColor,
                    background);
        }

        private static int GetBorderThickness(ComboBox comboBox)
        {
            SystemVisualSettings settings = Application.SystemVisualSettings;
            Size borderMetrics = ModernControlVisualStyles.GetFocusBorderMetrics(
                settings.FocusBorderMetrics,
                settings.TextScaleFactor,
                comboBox.DeviceDpiInternal);

            return Math.Max(borderMetrics.Width, borderMetrics.Height);
        }

        private static GraphicsPath CreateFieldPath(
            ComboBox comboBox,
            Rectangle bounds)
        {
            GraphicsPath path = new();
            int radius = GetFieldCornerRadius(comboBox, bounds);
            path.AddRoundedRectangle(
                bounds,
                new Size(radius, radius));

            return path;
        }

        private static int GetFieldCornerRadius(
            ComboBox comboBox,
            Rectangle bounds)
            => Math.Clamp(
                ScaleHelper.ScaleToDpi(
                    ModernControlVisualStyles.FieldCornerRadius,
                    comboBox.DeviceDpiInternal),
                1,
                Math.Max(
                    1,
                    Math.Min(bounds.Width, bounds.Height)));

        private static Color GetEffectiveBackColor(ComboBox comboBox)
            => comboBox.BackColor.A == byte.MaxValue
                ? comboBox.BackColor
                : comboBox.ParentInternal?.BackColor
                    ?? SystemColors.Window;

        /// <summary>
        ///  Returns the field surface color, muted when the ComboBox is disabled so a disabled
        ///  control no longer shows its full custom <see cref="Control.BackColor"/> (issue #14797).
        /// </summary>
        private static Color GetEffectiveFieldColor(ComboBox comboBox)
        {
            Color background = GetEffectiveBackColor(comboBox);

            return comboBox.Enabled
                ? background
                : PopupButtonColorMath.Mute(background, 0.55f);
        }
    }
}
