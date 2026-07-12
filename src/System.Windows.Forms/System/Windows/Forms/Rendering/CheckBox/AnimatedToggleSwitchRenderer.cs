// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.Rendering.CheckBox;

/// <summary>
///  Renders and animates a <see cref="Forms.CheckBox"/> or <see cref="Forms.RadioButton"/>
///  in <see cref="Appearance.ToggleSwitch"/> mode. Used when <see cref="Control.VisualStylesMode"/>
///  is <see cref="VisualStylesMode.Net11"/> or later.
/// </summary>
internal sealed class AnimatedToggleSwitchRenderer : AnimatedControlRenderer
{
    private const int AnimationDuration = 300;

    private readonly ModernCheckBoxStyle _switchStyle;
    private bool _positionInitialized;
    private float _positionCurrent;
    private float _positionStart;
    private float _positionTarget;

    public AnimatedToggleSwitchRenderer(Control control, ModernCheckBoxStyle switchStyle)
        : base(control)
    {
        _switchStyle = switchStyle;
    }

    public override void AnimationProc(float animationProgress)
    {
        base.AnimationProc(animationProgress);
        _positionCurrent = Lerp(_positionStart, _positionTarget, EaseOut(animationProgress));
        Invalidate();
    }

    protected override (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted()
    {
        EnsurePositionInitialized();
        _positionStart = _positionCurrent;
        _positionTarget = IsChecked ? 1f : 0f;
        AnimationProgress = 0;

        return (AnimationDuration, AnimationCycle.Once);
    }

    /// <summary>
    ///  Called from the control's <c>OnPaint</c>. Works both while the animation is running (driven by
    ///  <see cref="AnimationProc"/>) and when it is settled (progress is 1).
    /// </summary>
    /// <param name="graphics">The graphics object to render into.</param>
    public override void RenderControl(Graphics graphics)
    {
        EnsurePositionInitialized();

        ToggleSwitchMetrics metrics = ToggleSwitchMetrics.Create(Control);
        Size textSize = TextRenderer.MeasureText(Control.Text, Control.Font);
        Rectangle contentBounds = ToggleSwitchMetrics.GetContentBounds(Control);
        int totalHeight = Math.Max(textSize.Height, metrics.SwitchHeight);
        int contentTop = contentBounds.Top + Math.Max(0, (contentBounds.Height - totalHeight) / 2);
        int switchY = contentTop + ((totalHeight - metrics.SwitchHeight) / 2);
        int textY = contentTop + ((totalHeight - textSize.Height) / 2);

        graphics.Clear(Control.BackColor);

        if (contentBounds.Width <= 0 || contentBounds.Height <= 0)
        {
            return;
        }

        if (IsSwitchOnRight(RtlTranslatedCheckAlign))
        {
            int switchX = Math.Max(contentBounds.Left, contentBounds.Right - metrics.SwitchWidth);
            int textX = Math.Max(contentBounds.Left, switchX - metrics.TextGap - textSize.Width);
            RenderSwitch(
                graphics,
                new Rectangle(switchX, switchY, metrics.SwitchWidth, metrics.SwitchHeight),
                metrics);
            RenderText(graphics, new Point(textX, textY));
        }
        else
        {
            RenderSwitch(
                graphics,
                new Rectangle(contentBounds.Left, switchY, metrics.SwitchWidth, metrics.SwitchHeight),
                metrics);
            RenderText(graphics, new Point(contentBounds.Left + metrics.SwitchWidth + metrics.TextGap, textY));
        }

        if (Control.Focused && ShowFocusCues)
        {
            Rectangle focusBounds = Rectangle.Inflate(Control.ClientRectangle, -1, -1);
            ControlPaint.DrawFocusRectangle(
                graphics,
                focusBounds,
                Control.ForeColor,
                Control.BackColor);
        }
    }

    private static bool IsSwitchOnRight(ContentAlignment checkAlign) => checkAlign is
        ContentAlignment.TopRight or
        ContentAlignment.MiddleRight or
        ContentAlignment.BottomRight;

    private void RenderText(Graphics graphics, Point position)
        => TextRenderer.DrawText(
            graphics,
            Control.Text,
            Control.Font,
            position,
            Control.Enabled ? Control.ForeColor : SystemColors.GrayText);

    private void RenderSwitch(Graphics graphics, Rectangle rect, ToggleSwitchMetrics metrics)
    {
        if (rect.Width <= 0 || rect.Height <= 0)
        {
            return;
        }

        // The background color flips at 80% of the animation so the thumb travels visibly before the color change.
        Color backgroundColor = !Control.Enabled
            ? SystemColors.Control
            : IsChecked ^ (AnimationProgress < 0.8f)
                ? SystemColors.Highlight
                : SystemColors.ControlDark;

        Color circleColor = Control.Enabled
            ? SystemColors.ControlText
            : SystemColors.GrayText;

        float circlePosition = (rect.Width - metrics.ThumbDiameter) * _positionCurrent;

        using var backgroundBrush = backgroundColor.GetCachedSolidBrushScope();
        using var circleBrush = circleColor.GetCachedSolidBrushScope();
        using var backgroundPen =
            SystemColors.WindowFrame.GetCachedPenScope(metrics.BorderThickness);

        SmoothingMode previousSmoothingMode = graphics.SmoothingMode;
        try
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (_switchStyle == ModernCheckBoxStyle.Rounded)
            {
                float radius = Math.Min(rect.Width, rect.Height) / 2f;

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

            float circleTop = rect.Y + ((rect.Height - metrics.ThumbDiameter) / 2f);
            graphics.FillEllipse(
                circleBrush,
                rect.X + circlePosition,
                circleTop,
                metrics.ThumbDiameter,
                metrics.ThumbDiameter);
        }
        finally
        {
            graphics.SmoothingMode = previousSmoothingMode;
        }
    }

    internal void PrepareStateChange()
    {
        EnsurePositionInitialized();
        StopAnimation();
    }

    internal void SynchronizeState()
    {
        StopAnimation();
        _positionCurrent = IsChecked ? 1f : 0f;
        _positionStart = _positionCurrent;
        _positionTarget = _positionCurrent;
        _positionInitialized = true;
        AnimationProgress = 1;
    }

    protected override void OnAnimationStopped()
    {
    }

    protected override void OnAnimationEnded()
    {
        StopAnimation();
        _positionCurrent = _positionTarget;
        _positionStart = _positionCurrent;
        AnimationProgress = 1;
        Invalidate();
    }

    private void EnsurePositionInitialized()
    {
        if (_positionInitialized)
        {
            return;
        }

        _positionCurrent = IsChecked ? 1f : 0f;
        _positionStart = _positionCurrent;
        _positionTarget = _positionCurrent;
        _positionInitialized = true;
    }

    private static float EaseOut(float value)
        => 1 - ((1 - value) * (1 - value));

    private static float Lerp(float start, float end, float amount)
        => start + ((end - start) * amount);

    private bool IsChecked => Control switch
    {
        Forms.CheckBox checkBox => checkBox.Checked,
        Forms.RadioButton radioButton => radioButton.Checked,
        _ => false
    };

    private ContentAlignment RtlTranslatedCheckAlign => Control switch
    {
        Forms.CheckBox checkBox => checkBox.RtlTranslatedCheckAlign,
        Forms.RadioButton radioButton => radioButton.RtlTranslatedCheckAlign,
        _ => ContentAlignment.MiddleLeft
    };

    private bool ShowFocusCues => Control switch
    {
        Forms.CheckBox checkBox => checkBox.ShowFocusCuesInternal,
        Forms.RadioButton radioButton => radioButton.ShowFocusCuesInternal,
        _ => false
    };
}

/// <summary>
///  Provides the geometry shared by toggle-switch rendering and preferred-size calculations.
/// </summary>
internal readonly struct ToggleSwitchMetrics
{
    private ToggleSwitchMetrics(
        int switchWidth,
        int switchHeight,
        int thumbDiameter,
        int borderThickness,
        int textGap)
    {
        SwitchWidth = switchWidth;
        SwitchHeight = switchHeight;
        ThumbDiameter = thumbDiameter;
        BorderThickness = borderThickness;
        TextGap = textGap;
    }

    internal int SwitchWidth { get; }

    internal int SwitchHeight { get; }

    internal int ThumbDiameter { get; }

    internal int BorderThickness { get; }

    internal int TextGap { get; }

    internal static ToggleSwitchMetrics Create(Control control)
    {
        int switchHeight = Math.Max(1, control.Font.Height);
        int minimumMetric = Math.Max(1, control.LogicalToDeviceUnits(1));
        int borderThickness = Math.Max(minimumMetric, switchHeight / 12);
        int thumbDiameter = Math.Max(1, switchHeight - (2 * (borderThickness + minimumMetric)));
        int textGap = Math.Max(2 * minimumMetric, switchHeight / 3);

        return new(
            switchWidth: 2 * switchHeight,
            switchHeight: switchHeight,
            thumbDiameter: thumbDiameter,
            borderThickness: borderThickness,
            textGap: textGap);
    }

    internal static Rectangle GetContentBounds(Control control)
    {
        Padding padding = control.Padding;
        return new Rectangle(
            padding.Left,
            padding.Top,
            Math.Max(0, control.ClientSize.Width - padding.Horizontal),
            Math.Max(0, control.ClientSize.Height - padding.Vertical));
    }

    internal Size GetPreferredSize(Control control)
    {
        Size textSize = TextRenderer.MeasureText(control.Text, control.Font);
        return new Size(
            SwitchWidth + TextGap + textSize.Width + control.Padding.Horizontal,
            Math.Max(SwitchHeight, textSize.Height) + control.Padding.Vertical);
    }
}
