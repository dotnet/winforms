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
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            ClearNativeFrame(
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
                    DrawFlatUnderline(
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

        private static void ClearNativeFrame(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle bounds)
        {
            int thickness = Math.Max(
                1,
                ScaleHelper.ScaleToDpi(
                    ModernControlVisualStyles.Fixed3DBorderPadding,
                    comboBox.DeviceDpiInternal));
            Color background = GetEffectiveBackColor(comboBox);
            using var brush = background.GetCachedSolidBrushScope();
            graphics.FillRectangle(
                brush,
                bounds.Left,
                bounds.Top,
                bounds.Width,
                Math.Min(thickness, bounds.Height));
            graphics.FillRectangle(
                brush,
                bounds.Left,
                Math.Max(bounds.Top, bounds.Bottom - thickness),
                bounds.Width,
                Math.Min(thickness, bounds.Height));
            graphics.FillRectangle(
                brush,
                bounds.Left,
                bounds.Top,
                Math.Min(thickness, bounds.Width),
                bounds.Height);
            graphics.FillRectangle(
                brush,
                Math.Max(bounds.Left, bounds.Right - thickness),
                bounds.Top,
                Math.Min(thickness, bounds.Width),
                bounds.Height);
        }

        private static void DrawStandardFrame(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle clientBounds,
            Rectangle borderBounds)
        {
            Color background = GetEffectiveBackColor(comboBox);
            Color borderColor = PopupButtonColorMath.TowardsContrast(
                background,
                0.2f);
            if (!comboBox.Enabled)
            {
                borderColor = PopupButtonColorMath.Mute(
                    borderColor,
                    0.55f);
            }

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

        private static void DrawFlatUnderline(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle bounds)
        {
            if (!comboBox.Enabled || !comboBox.ContainsFocus)
            {
                return;
            }

            int thickness = Math.Max(
                ScaleHelper.ScaleToDpi(
                    ModernControlVisualStyles.BorderThickness,
                    comboBox.DeviceDpiInternal),
                ModernControlVisualStyles.GetFocusBorderMetrics(
                    Application.SystemVisualSettings.FocusBorderMetrics,
                    Application.SystemVisualSettings.TextScaleFactor,
                    comboBox.DeviceDpiInternal).Height);
            using var pen = Application.SystemVisualSettings.AccentColor
                .GetCachedPenScope(thickness);
            int y = bounds.Bottom - Math.Max(1, thickness / 2);
            graphics.DrawLine(
                pen,
                bounds.Left,
                y,
                bounds.Right,
                y);
        }

        private static void DrawPopupFrame(
            ComboBox comboBox,
            Graphics graphics,
            Rectangle clientBounds,
            Rectangle borderBounds)
        {
            if (!comboBox.Enabled
                || (!comboBox.ContainsFocus && !comboBox.MouseIsOver))
            {
                return;
            }

            Color background = GetEffectiveBackColor(comboBox);
            Color borderColor = comboBox.ContainsFocus
                ? Application.SystemVisualSettings.AccentColor
                : PopupButtonColorMath.TowardsContrast(
                    background,
                    0.22f);
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
            int thickness = Math.Max(
                1,
                ScaleHelper.ScaleToDpi(
                    ModernControlVisualStyles.BorderThickness,
                    comboBox.DeviceDpiInternal));
            using var pen = borderColor.GetCachedPenScope(thickness);
            graphics.DrawPath(pen, path);
        }

        private static GraphicsPath CreateFieldPath(
            ComboBox comboBox,
            Rectangle bounds)
        {
            GraphicsPath path = new();
            int radius = Math.Clamp(
                ScaleHelper.ScaleToDpi(
                    ModernControlVisualStyles.FieldCornerRadius,
                    comboBox.DeviceDpiInternal),
                1,
                Math.Max(
                    1,
                    Math.Min(bounds.Width, bounds.Height)));
            path.AddRoundedRectangle(
                bounds,
                new Size(radius, radius));

            return path;
        }

        private static Color GetEffectiveBackColor(ComboBox comboBox)
            => comboBox.BackColor.A == byte.MaxValue
                ? comboBox.BackColor
                : comboBox.ParentInternal?.BackColor
                    ?? SystemColors.Window;
    }
}
