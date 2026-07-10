// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.Rendering.CheckBox;

/// <summary>
///  Renders and animates a <see cref="Forms.CheckBox"/> in <see cref="Appearance.ToggleSwitch"/> mode.
///  Used when <see cref="Control.VisualStylesMode"/> is <see cref="VisualStylesMode.Net11"/> or later.
/// </summary>
internal sealed class AnimatedToggleSwitchRenderer : AnimatedControlRenderer
{
    private const int AnimationDuration = 300; // milliseconds
    private const int SwitchWidthLogical = 50;
    private const int SwitchHeightLogical = 25;
    private const int CircleDiameterLogical = 20;
    private const int TextGapLogical = 10;

    private readonly ModernCheckBoxStyle _switchStyle;

    public AnimatedToggleSwitchRenderer(Control control, ModernCheckBoxStyle switchStyle)
        : base(control)
    {
        _switchStyle = switchStyle;
    }

    private Forms.CheckBox CheckBox => (Forms.CheckBox)Control;

    public override void AnimationProc(float animationProgress)
    {
        base.AnimationProc(animationProgress);
        Invalidate();
    }

    protected override (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted()
    {
        AnimationProgress = 1;

        return (AnimationDuration, AnimationCycle.Once);
    }

    /// <summary>
    ///  Called from the control's <c>OnPaint</c>. Works both while the animation is running (driven by
    ///  <see cref="AnimationProc"/>) and when it is settled (progress is 1).
    /// </summary>
    /// <param name="graphics">The graphics object to render into.</param>
    public override void RenderControl(Graphics graphics)
    {
        int switchWidth = Control.LogicalToDeviceUnits(SwitchWidthLogical);
        int switchHeight = Control.LogicalToDeviceUnits(SwitchHeightLogical);
        int circleDiameter = Control.LogicalToDeviceUnits(CircleDiameterLogical);
        int textGap = Control.LogicalToDeviceUnits(TextGapLogical);

        Size textSize = TextRenderer.MeasureText(Control.Text, Control.Font);

        int totalHeight = Math.Max(textSize.Height, switchHeight);
        int switchY = (totalHeight - switchHeight) / 2;
        int textY = (totalHeight - textSize.Height) / 2;

        graphics.Clear(Control.BackColor);

        // The switch position follows CheckAlign (as the caption sits opposite the check), so the default
        // MiddleLeft places the switch on the left and the caption on the right. Only the horizontal component
        // is honored; the switch stays vertically centered.
        if (IsSwitchOnRight(CheckBox.RtlTranslatedCheckAlign))
        {
            int switchX = Math.Max(0, CheckBox.ClientRectangle.Right - switchWidth);
            int textX = Math.Max(0, switchX - textGap - textSize.Width);
            RenderSwitch(graphics, new Rectangle(switchX, switchY, switchWidth, switchHeight), circleDiameter);
            RenderText(graphics, new Point(textX, textY));
        }
        else
        {
            RenderSwitch(graphics, new Rectangle(0, switchY, switchWidth, switchHeight), circleDiameter);
            RenderText(graphics, new Point(switchWidth + textGap, textY));
        }

        if (CheckBox.Focused && CheckBox.ShowFocusCuesInternal)
        {
            Rectangle focusBounds = Rectangle.Inflate(CheckBox.ClientRectangle, -1, -1);
            ControlPaint.DrawFocusRectangle(
                graphics,
                focusBounds,
                CheckBox.ForeColor,
                CheckBox.BackColor);
        }
    }

    // The switch is drawn on the right only for the right-aligned CheckAlign values; left- and center-aligned
    // values (including the MiddleLeft default) keep the switch on the left.
    private static bool IsSwitchOnRight(ContentAlignment checkAlign) => checkAlign is
        ContentAlignment.TopRight or
        ContentAlignment.MiddleRight or
        ContentAlignment.BottomRight;

    private void RenderText(Graphics graphics, Point position)
        => TextRenderer.DrawText(
            graphics,
            CheckBox.Text,
            CheckBox.Font,
            position,
            CheckBox.Enabled ? CheckBox.ForeColor : SystemColors.GrayText);

    private void RenderSwitch(Graphics graphics, Rectangle rect, int circleDiameter)
    {
        // The background color flips at 80% of the animation so the thumb travels visibly before the color change.
        Color backgroundColor = !CheckBox.Enabled
            ? SystemColors.Control
            : CheckBox.Checked ^ (AnimationProgress < 0.8f)
                ? SystemColors.Highlight
                : SystemColors.ControlDark;

        Color circleColor = CheckBox.Enabled
            ? SystemColors.ControlText
            : SystemColors.GrayText;

        // Works both for the running and settled states (settled progress is 1, so the thumb rests in place).
        float circlePosition = CheckBox.Checked
            ? (rect.Width - circleDiameter) * (1 - EaseOut(AnimationProgress))
            : (rect.Width - circleDiameter) * EaseOut(AnimationProgress);

        using var backgroundBrush = backgroundColor.GetCachedSolidBrushScope();
        using var circleBrush = circleColor.GetCachedSolidBrushScope();
        using var backgroundPen =
            SystemColors.WindowFrame.GetCachedPenScope(Control.LogicalToDeviceUnits(2));

        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        if (_switchStyle == ModernCheckBoxStyle.Rounded)
        {
            float radius = rect.Height / 2f;

            using GraphicsPath path = new();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();

            graphics.FillPath(backgroundBrush, path);
            graphics.DrawPath(backgroundPen, path);
        }
        else
        {
            graphics.FillRectangle(backgroundBrush, rect);
            graphics.DrawRectangle(backgroundPen, rect);
        }

        float circleTop = rect.Y + ((rect.Height - circleDiameter) / 2f);
        graphics.FillEllipse(circleBrush, rect.X + circlePosition, circleTop, circleDiameter, circleDiameter);

        static float EaseOut(float t) => (1 - t) * (1 - t);
    }

    protected override void OnAnimationStopped()
    {
        AnimationProgress = 0;
    }

    protected override void OnAnimationEnded()
    {
        StopAnimation();
        AnimationProgress = 1;
        Invalidate();
    }
}
