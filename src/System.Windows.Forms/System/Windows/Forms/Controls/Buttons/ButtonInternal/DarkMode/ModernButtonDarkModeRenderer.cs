// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Modern, WinUI-inspired push button renderer used when <see cref="Control.VisualStylesMode"/> is
///  <see cref="VisualStylesMode.Net11"/> or later. It supports both dark and light color schemes and draws a
///  rounded button area, an optional dark gap ring, and a rounded focus ring for the focused button.
/// </summary>
/// <remarks>
///  <para>
///   The visual is intentionally driven by a small set of DPI-scaled constants so the appearance can be
///   fine-tuned during exploratory testing. All widths are expressed in logical (96-DPI) pixels.
///  </para>
/// </remarks>
internal sealed class ModernButtonDarkModeRenderer : ButtonDarkModeRendererBase
{
    // Logical (96-DPI) layout constants. DPI-scaled via the base Scale() helper.
    private const int FocusRingThicknessLogical = 2;
    private const int FocusGapThicknessLogical = 1;
    private const int FocusedCornerRadiusLogical = 6;
    private const int UnfocusedCornerRadiusLogical = 8;
    private const int BorderThicknessLogical = 1;
    private const int ContentInsetLogical = 4;

    // Dark scheme - normal button area.
    private static readonly Color s_darkNormal = Color.FromArgb(0x2D, 0x2D, 0x2D);
    private static readonly Color s_darkNormalHover = Color.FromArgb(0x32, 0x32, 0x32);
    private static readonly Color s_darkNormalPressed = Color.FromArgb(0x2A, 0x2A, 0x2A);
    private static readonly Color s_darkDisabled = Color.FromArgb(0x25, 0x25, 0x25);

    private static readonly Color s_darkDisabledText = Color.FromArgb(0x88, 0x88, 0x88);

    private static readonly Color s_darkGap = Color.FromArgb(0x0A, 0x0A, 0x0A);
    private static readonly Color s_darkFocusRing = Color.White;

    // Light (WinUI) scheme - normal button area.
    private static readonly Color s_lightNormal = Color.FromArgb(0xFB, 0xFB, 0xFB);
    private static readonly Color s_lightNormalHover = Color.FromArgb(0xF9, 0xF9, 0xF9);
    private static readonly Color s_lightNormalPressed = Color.FromArgb(0xF5, 0xF5, 0xF5);
    private static readonly Color s_lightDisabled = Color.FromArgb(0xFA, 0xFA, 0xFA);
    private static readonly Color s_lightBorder = Color.FromArgb(0xD0, 0xD0, 0xD0);

    private static readonly Color s_lightDisabledText = Color.FromArgb(0xA0, 0xA0, 0xA0);

    private static bool IsDark => Application.IsDarkModeEnabled;

    private int FocusRingThickness
        => ScaleBorderThickness(Math.Max(1, Scale(FocusRingThicknessLogical) - HighDpiCorrection));

    private int FocusGapThickness
        => Math.Max(1, Scale(FocusGapThicknessLogical) - HighDpiCorrection);

    private int FocusBodyInset
        => Math.Max(
            FocusRingThickness + FocusGapThickness,
            Math.Max(1, Scale(FocusRingThicknessLogical))
                + Math.Max(1, Scale(FocusGapThicknessLogical)));

    private int HighDpiCorrection => DeviceDpi > ScaleHelper.OneHundredPercentLogicalDpi ? 1 : 0;

    private int GetCornerRadius(bool focused, bool isDefault)
        => Math.Max(1, Scale(focused || isDefault ? FocusedCornerRadiusLogical : UnfocusedCornerRadiusLogical));

    // When focused, the body is inset just enough to leave room for the focus ring and a single-pixel gap,
    // keeping that gap tight so the rounded body claims as much real estate as possible.
    private protected override Padding PaddingCore
        => new(FocusBodyInset);

    // When the focus ring is not drawn there is nothing to inset for, so the rounded body expands to fill the
    // whole client area - covering the band the ring and its gap would otherwise occupy. This gives the button
    // a more generous, less cramped background without changing the control's own Padding.
    private protected override Padding GetContentPadding(bool focusRingVisible)
        => focusRingVisible ? PaddingCore : Padding.Empty;

    internal override Padding GetPreferredSizePadding()
        => new(FocusBodyInset + Scale(ContentInsetLogical));

    private protected override bool UseModernStateDefaults => true;

    private protected override bool PaintParentBackground => true;

    public override Rectangle DrawButtonBackground(
        Graphics graphics,
        Rectangle bounds,
        PushButtonState state,
        bool isDefault,
        bool focused,
        Color backColor)
    {
        GraphicsState? saved = graphics.Save();
        try
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            RectangleF pathBounds = GetPathBounds(bounds);
            float radius = GetCornerRadius(focused, isDefault);
            int borderThickness = ScaleBorderThickness(
                Math.Max(1, Scale(BorderThicknessLogical)));

            if (!IsDark
                && !isDefault
                && state != PushButtonState.Disabled
                && borderThickness > 0)
            {
                Color borderColor = ResolveBorderColor(s_lightBorder);
                using var borderBrush = borderColor.GetCachedSolidBrushScope();
                using GraphicsPath borderPath = CreateRingPath(
                    pathBounds,
                    radius,
                    borderThickness);
                graphics.FillPath(borderBrush, borderPath);

                pathBounds = Inset(pathBounds, borderThickness);
                radius = Math.Max(1, radius - (2 * borderThickness));
            }

            using var brush = backColor.GetCachedSolidBrushScope();
            using GraphicsPath bodyPath = CreateRoundedPath(pathBounds, radius);
            graphics.FillPath(brush, bodyPath);
        }
        finally
        {
            if (saved is not null)
            {
                graphics.Restore(saved);
            }
        }

        int inset = Scale(ContentInsetLogical);
        return Rectangle.Inflate(bounds, -inset, -inset);
    }

    public override void DrawFocusIndicator(Graphics graphics, Rectangle bounds, bool isDefault)
    {
        GraphicsState? saved = graphics.Save();
        try
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (FocusRingThickness == 0)
            {
                return;
            }

            RectangleF outerBounds = GetPathBounds(bounds);
            int bodyInset = FocusRingThickness + FocusGapThickness;
            float outerRadius = GetCornerRadius(focused: true, isDefault: isDefault)
                + (2 * bodyInset);

            Color gapColor = IsDark ? s_darkGap : SystemColors.Window;
            using (var gapBrush = gapColor.GetCachedSolidBrushScope())
            {
                using GraphicsPath gapPath = CreateRingPath(
                    Inset(outerBounds, FocusRingThickness),
                    outerRadius - (2 * FocusRingThickness),
                    FocusGapThickness);
                graphics.FillPath(gapBrush, gapPath);
            }

            Color ringColor = ResolveBorderColor(IsDark ? s_darkFocusRing : SystemColors.WindowText);
            using var ringBrush = ringColor.GetCachedSolidBrushScope();
            using GraphicsPath ringPath = CreateRingPath(
                outerBounds,
                outerRadius,
                FocusRingThickness);
            graphics.FillPath(ringBrush, ringPath);
        }
        finally
        {
            if (saved is not null)
            {
                graphics.Restore(saved);
            }
        }
    }

    public override Color GetTextColor(PushButtonState state, bool isDefault, Color backColor)
    {
        if (state == PushButtonState.Disabled)
        {
            Color preferredForeColor = IsDark
                ? s_darkDisabledText
                : s_lightDisabledText;

            return ModernControlColorMath.GetDisabledTextColor(
                preferredForeColor,
                backColor);
        }

        return ModernButtonColorMath.GetReadableForeColor(backColor);
    }

    public override Color GetBackgroundColor(PushButtonState state, bool isDefault)
    {
        if (state == PushButtonState.Disabled)
        {
            return IsDark ? s_darkDisabled : s_lightDisabled;
        }

        if (isDefault)
        {
            return ModernButtonColorMath.GetDefaultButtonColor(state);
        }

        return IsDark
            ? state switch
            {
                PushButtonState.Hot => s_darkNormalHover,
                PushButtonState.Pressed => s_darkNormalPressed,
                _ => s_darkNormal
            }
            : state switch
            {
                PushButtonState.Hot => s_lightNormalHover,
                PushButtonState.Pressed => s_lightNormalPressed,
                _ => s_lightNormal
            };
    }

    private static RectangleF GetPathBounds(Rectangle bounds)
        => new(
            bounds.X,
            bounds.Y,
            Math.Max(1, bounds.Width - 1),
            Math.Max(1, bounds.Height - 1));

    private static RectangleF Inset(RectangleF bounds, float inset)
        => new(
            bounds.X + inset,
            bounds.Y + inset,
            Math.Max(1, bounds.Width - (2 * inset)),
            Math.Max(1, bounds.Height - (2 * inset)));

    private static GraphicsPath CreateRoundedPath(RectangleF bounds, float radius)
    {
        GraphicsPath path = new();
        float clampedRadius = Math.Clamp(radius, 1, Math.Min(bounds.Width, bounds.Height));
        path.AddRoundedRectangle(bounds, new SizeF(clampedRadius, clampedRadius));
        return path;
    }

    private static GraphicsPath CreateRingPath(
        RectangleF outerBounds,
        float outerRadius,
        float thickness)
    {
        GraphicsPath path = CreateRoundedPath(outerBounds, outerRadius);
        RectangleF innerBounds = Inset(outerBounds, thickness);
        float innerRadius = Math.Max(1, outerRadius - (2 * thickness));
        using GraphicsPath innerPath = CreateRoundedPath(innerBounds, innerRadius);
        path.FillMode = FillMode.Alternate;
        path.AddPath(innerPath, connect: false);
        return path;
    }
}
