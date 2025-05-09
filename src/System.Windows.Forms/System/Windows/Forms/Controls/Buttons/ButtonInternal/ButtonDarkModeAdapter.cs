// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.ButtonInternal;

internal class ButtonDarkModeAdapter : ButtonBaseAdapter
{
    private const int BorderSize = 1;
    private const int CornerRadius = 5;

    internal ButtonDarkModeAdapter(ButtonBase control) : base(control) { }

    private static void PaintBackground(PaintEventArgs e, Rectangle r, Color backColor)
    {
        // Save original smoothing mode and set to anti-alias for smooth corners
        SmoothingMode originalMode = e.Graphics.SmoothingMode;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using GraphicsPath path = GetRoundedRectanglePath(r, CornerRadius);
        using SolidBrush brush = new(backColor);
        e.Graphics.FillPath(brush, path);

        // Restore original smoothing mode
        e.Graphics.SmoothingMode = originalMode;
    }

    internal override void PaintUp(PaintEventArgs e, CheckState state)
    {
        bool isDefault = Control.IsDefault;
        ColorData colors = PaintDarkModeRender(e).Calculate();
        LayoutData layout = PaintDarkModeLayout(
            up: true,
            check: state == CheckState.Checked,
            borderSize: BorderSize).Layout();

        Rectangle r = Control.ClientRectangle;

        // Determine background color based on button state
        Color backColor = GetBackgroundColor(state, isDefault, normal: true);

        PaintBackground(e, r, backColor);

        if (isDefault)
        {
            r.Inflate(-1, -1);
        }

        PaintImage(e, layout);
        PaintField(
            e,
            layout,
            colors,
            GetTextColor(isDefault, Control.Enabled),
            drawFocus: false);

        if (Control.Focused && Control.ShowFocusCues)
        {
            DrawDarkModeFocus(e, layout.Focus, isDefault);
        }

        if (!(Control.IsDefault && Control.Focused && (BorderSize == 0)))
        {
            DrawDarkModeBorder(e, r, isDefault, pressed: false);
        }
    }

    internal override void PaintDown(PaintEventArgs e, CheckState state)
    {
        bool isDefault = Control.IsDefault;

        ColorData colors = PaintDarkModeRender(e).Calculate();
        LayoutData layout = PaintDarkModeLayout(
            up: false,
            check: state == CheckState.Checked,
            borderSize: BorderSize).Layout();

        Rectangle r = Control.ClientRectangle;

        // Determine background color based on button state
        Color backColor = GetBackgroundColor(state, isDefault, normal: false);

        PaintBackground(e, r, backColor);

        if (isDefault)
        {
            r.Inflate(-1, -1);
        }

        PaintImage(e, layout);

        PaintField(
            e,
            layout,
            colors,
            GetTextColor(isDefault, Control.Enabled),
            drawFocus: false);

        if (Control.Focused && Control.ShowFocusCues)
        {
            DrawDarkModeFocus(e, layout.Focus, isDefault);
        }

        if (!(Control.IsDefault && Control.Focused && (BorderSize == 0)))
        {
            DrawDarkModeBorder(e, r, isDefault, pressed: true);
        }
    }

    internal override void PaintOver(PaintEventArgs e, CheckState state)
    {
        bool isDefault = Control.IsDefault;
        ColorData colors = PaintDarkModeRender(e).Calculate();

        LayoutData layout = PaintDarkModeLayout(
            up: true,
            check: state == CheckState.Checked,
            borderSize: BorderSize).Layout();

        Rectangle r = Control.ClientRectangle;

        // Get hover background color
        Color backColor = isDefault
            ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultHoverBackgroundColor
            : ButtonDarkModeRenderer.DarkModeButtonColors.HoverBackgroundColor;

        PaintBackground(e, r, backColor);

        if (isDefault)
        {
            r.Inflate(-1, -1);
        }

        PaintImage(e, layout);
        PaintField(
            e,
            layout,
            colors,
            GetTextColor(isDefault, Control.Enabled),
            drawFocus: false);

        if (Control.Focused && Control.ShowFocusCues)
        {
            DrawDarkModeFocus(e, layout.Focus, isDefault);
        }

        if (!(Control.IsDefault && Control.Focused && (BorderSize == 0)))
        {
            DrawDarkModeBorder(e, r, isDefault, pressed: false);
        }
    }

    protected override LayoutOptions Layout(PaintEventArgs e) =>
        PaintDarkModeLayout(up: true, check: false, BorderSize);

    private LayoutOptions PaintDarkModeLayout(bool up, bool check, int borderSize)
    {
        LayoutOptions layout = CommonLayout();
        layout.BorderSize = borderSize + (check ? 1 : 0);
        layout.PaddingSize = check ? 1 : 2;
        layout.FocusOddEvenFixup = false;
        layout.TextOffset = !up;
        layout.ShadowedText = false; // Dark mode doesn't use shadowed text

        return layout;
    }

    private ColorOptions PaintDarkModeRender(IDeviceContext deviceContext) =>
        new(deviceContext, Control.ForeColor, Control.BackColor)
        {
            Enabled = Control.Enabled
        };

    private Color GetBackgroundColor(CheckState state, bool isDefault, bool normal)
    {
        if (!Control.Enabled)
        {
            return isDefault
                ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultDisabledBackgroundColor
                : ButtonDarkModeRenderer.DarkModeButtonColors.DisabledBackgroundColor;
        }

        if (!normal) // Pressed state
        {
            return isDefault
                ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultPressedBackgroundColor
                : ButtonDarkModeRenderer.DarkModeButtonColors.PressedBackgroundColor;
        }

        // Normal state
        switch (state)
        {
            case CheckState.Checked:
                return isDefault
                    ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor
                    : ButtonDarkModeRenderer.DarkModeButtonColors.NormalBackgroundColor;
            case CheckState.Indeterminate:
                Color baseColor = isDefault
                    ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor
                    : ButtonDarkModeRenderer.DarkModeButtonColors.NormalBackgroundColor;

                return Color.FromArgb(
                    baseColor.R + 10,
                    baseColor.G + 10,
                    baseColor.B + 10);
            default:
                return isDefault
                    ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor
                    : ButtonDarkModeRenderer.DarkModeButtonColors.NormalBackgroundColor;
        }
    }

    private static Color GetTextColor(bool isDefault, bool enabled) =>
        !enabled
            ? ButtonDarkModeRenderer.DarkModeButtonColors.DisabledTextColor
            : isDefault
                ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultTextColor
                : ButtonDarkModeRenderer.DarkModeButtonColors.NormalTextColor;

    private static void DrawDarkModeBorder(PaintEventArgs e, Rectangle r, bool isDefault, bool pressed)
    {
        SmoothingMode originalMode = e.Graphics.SmoothingMode;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using GraphicsPath path = GetRoundedRectanglePath(r, CornerRadius);

        Color borderColor = isDefault
            ? Color.FromArgb(
                red: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.R - 20,
                green: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.G - 10,
                blue: ButtonDarkModeRenderer.DarkModeButtonColors.DefaultBackgroundColor.B - 30)
            : pressed
                ? ButtonDarkModeRenderer.DarkModeButtonColors.BottomRightBorderColor
                : ButtonDarkModeRenderer.DarkModeButtonColors.SingleBorderColor;

        using Pen borderPen = new(borderColor);
        e.Graphics.DrawPath(borderPen, path);

        // Restore original smoothing mode
        e.Graphics.SmoothingMode = originalMode;
    }

    private static void DrawDarkModeFocus(PaintEventArgs e, Rectangle focusRect, bool isDefault)
    {
        SmoothingMode originalMode = e.Graphics.SmoothingMode;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Create a slightly smaller rectangle for the focus indicator (2px inside)
        Rectangle innerRect = Rectangle.Inflate(focusRect, -2, -2);

        // Get appropriate focus color
        Color focusColor = isDefault
            ? ButtonDarkModeRenderer.DarkModeButtonColors.DefaultFocusIndicatorColor
            : ButtonDarkModeRenderer.DarkModeButtonColors.FocusIndicatorColor;

        using Pen focusPen = new(focusColor)
        {
            DashStyle = DashStyle.Dot
        };

        // Draw the focus rectangle with rounded corners
        using GraphicsPath focusPath = GetRoundedRectanglePath(innerRect, 3);
        e.Graphics.DrawPath(focusPen, focusPath);

        // Restore original smoothing mode
        e.Graphics.SmoothingMode = originalMode;
    }

    private static GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
    {
        GraphicsPath path = new();
        int diameter = radius * 2;
        Rectangle arcRect = new(bounds.Location, new Size(diameter, diameter));

        // Top left corner
        path.AddArc(arcRect, 180, 90);

        // Top right corner
        arcRect.X = bounds.Right - diameter;
        path.AddArc(arcRect, 270, 90);

        // Bottom right corner
        arcRect.Y = bounds.Bottom - diameter;
        path.AddArc(arcRect, 0, 90);

        // Bottom left corner
        arcRect.X = bounds.Left;
        path.AddArc(arcRect, 90, 90);

        path.CloseFigure();
        return path;
    }
}
