// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.Rendering.Animation;
using PushButtonState = System.Windows.Forms.VisualStyles.PushButtonState;

namespace System.Windows.Forms.Rendering.Button;

/// <summary>
///  Drives and renders a <see cref="Forms.ButtonBase"/> whose <see cref="Forms.ButtonBase.FlatStyle"/> is
///  <see cref="FlatStyle.Popup"/> when modern visual styles or dark mode are active, using the concave key-cap
///  look of <see cref="PopupButtonKeyCapRenderer"/>.
/// </summary>
/// <remarks>
///  <para>
///   The renderer owns two interpolated animation channels (hover and press) that are advanced by the shared
///   <see cref="AnimationManager"/> on each high-precision timer tick. When settled it simply renders the
///   current channel values, so the same path serves both the animating and the resting button.
///  </para>
/// </remarks>
internal sealed class AnimatedPopupButtonRenderer : AnimatedControlRenderer
{
    private const int AnimationDurationMilliseconds = 160;
    private const float CustomHoverShadeAmount = 0.05f;
    private const float CustomPressedShadeAmount = 0.12f;

    private float _hoverCurrent;
    private float _hoverStart;
    private float _hoverTarget;

    private float _pressCurrent;
    private float _pressStart;
    private float _pressTarget;
    private readonly ModernButtonDarkModeRenderer _baseColorRenderer = new();
    private readonly ButtonDarkModeAdapter _layoutAdapter;

    public AnimatedPopupButtonRenderer(Forms.ButtonBase button)
        : base(button)
    {
        _layoutAdapter = new ButtonDarkModeAdapter(button);
    }

    private Forms.ButtonBase Button => (Forms.ButtonBase)Control;

    /// <summary>
    ///  Updates the hover and press targets from the current interaction state and, if anything changed,
    ///  (re)starts the interpolation from the current channel values towards the new targets.
    /// </summary>
    public void SetInteractionState(bool hovered, bool pressed, bool selected)
    {
        float hoverTarget = hovered ? 1f : 0f;
        float pressTarget = pressed || selected ? 1f : 0f;

        if (hoverTarget == _hoverTarget && pressTarget == _pressTarget)
        {
            return;
        }

        _hoverStart = _hoverCurrent;
        _pressStart = _pressCurrent;
        _hoverTarget = hoverTarget;
        _pressTarget = pressTarget;

        RestartAnimation();
    }

    protected override (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted()
        => (AnimationDurationMilliseconds, AnimationCycle.Once);

    public override void AnimationProc(float animationProgress)
    {
        base.AnimationProc(animationProgress);

        _hoverCurrent = Lerp(_hoverStart, _hoverTarget, PopupButtonEasing.EaseOutCubic(animationProgress));
        _pressCurrent = Lerp(_pressStart, _pressTarget, PopupButtonEasing.EaseInOutQuad(animationProgress));

        Invalidate();
    }

    protected override void OnAnimationEnded()
    {
        _hoverCurrent = _hoverTarget;
        _pressCurrent = _pressTarget;
        _hoverStart = _hoverCurrent;
        _pressStart = _pressCurrent;

        // The animation has reached its target; stop ticking so the settled button is not repainted every frame.
        // A later interaction change restarts a fresh interpolation via SetInteractionState.
        StopAnimation();

        Invalidate();
    }

    protected override void OnAnimationStopped()
    {
        // Keep the current channel values so a restart interpolates smoothly from where we are.
    }

    /// <summary>
    ///  Called from the button's <c>OnPaint</c>. Works both while the animation is running (driven by
    ///  <see cref="AnimationProc"/>) and when it is settled.
    /// </summary>
    public override void RenderControl(Graphics graphics)
    {
        Forms.ButtonBase button = Button;

        FlatButtonAppearance flatAppearance = button.FlatAppearance;
        _baseColorRenderer.DeviceDpi = button.DeviceDpi;
        _baseColorRenderer.FlatAppearance = flatAppearance;

        PushButtonState state = _pressTarget > 0f
            ? PushButtonState.Pressed
            : _hoverTarget > 0f
                ? PushButtonState.Hot
                : PushButtonState.Normal;
        bool highContrast = SystemInformation.HighContrast;
        Color faceColor;
        Color foreColor;
        Color borderColor;

        if (highContrast)
        {
            bool highlighted = button.Enabled
                && (button.Focused || button.MouseIsOver || button.IsDefault);
            faceColor = highlighted ? SystemColors.Highlight : SystemColors.Control;
            foreColor = !button.Enabled
                ? SystemColors.GrayText
                : highlighted
                    ? SystemColors.HighlightText
                    : SystemColors.ControlText;
            borderColor = button.Enabled ? SystemColors.ControlText : SystemColors.GrayText;
        }
        else
        {
            (Color baseColor, Color hoverColor, Color pressedColor) = GetStateColors();

            faceColor = PopupButtonColorMath.Blend(baseColor, hoverColor, _hoverCurrent);
            faceColor = PopupButtonColorMath.Blend(faceColor, pressedColor, _pressCurrent);
            bool useAutomaticForeColor = button.EffectiveVisualStylesModeInternal >= VisualStylesMode.Net11
                ? !button.ShouldSerializeForeColor()
                : button.ForeColor == Forms.Control.DefaultForeColor;
            foreColor = !useAutomaticForeColor
                ? button.ForeColor
                : _baseColorRenderer.GetTextColor(state, button.IsDefault, faceColor);
            borderColor = flatAppearance.BorderColor.IsEmpty
                ? PopupButtonColorMath.TowardsContrast(faceColor, 0.35f)
                : flatAppearance.BorderColor;
        }

        PopupButtonRenderContext context = new()
        {
            Bounds = button.ClientRectangle,
            Text = button.Text,
            Font = button.Font,
            BackColor = faceColor,
            ForeColor = foreColor,
            SurfaceColor = button.Parent?.BackColor ?? button.BackColor,
            UseAutomaticForeColor = button.EffectiveVisualStylesModeInternal >= VisualStylesMode.Net11
                ? !button.ShouldSerializeForeColor()
                : button.ForeColor == Forms.Control.DefaultForeColor,
            BorderColor = borderColor,
            BorderWidth = flatAppearance.BorderSize,
            Enabled = button.Enabled,
            Focused = button.Focused && button.ShowFocusCues,
            Pressed = button.MouseIsDown || _pressTarget > 0f,
            IsDefault = button.IsDefault,
            IsDarkMode = Application.IsDarkModeEnabled,
            AnimationState = new PopupButtonAnimationState(_hoverCurrent, _pressCurrent),
            TextAlign = button.TextAlign,
            ImageSize = button.Image?.Size ?? Size.Empty,
            ImageAlign = button.ImageAlign,
            TextImageRelation = button.TextImageRelation,
            RightToLeft = button.RightToLeft,
            Padding = button.Padding,
            DeviceDpi = button.DeviceDpi,
            ShowKeyboardCues = button.ShowKeyboardCues,
            HighContrast = highContrast
        };

        if (context.HighContrast || context.Bounds.Width < 8 || context.Bounds.Height < 8)
        {
            using PaintEventArgs paintEventArgs = new(graphics, button.ClientRectangle);
            button.PaintBackground(paintEventArgs, button.ClientRectangle);
        }
        else
        {
            ParentBackgroundRenderer.Paint(
                button,
                graphics,
                button.ClientRectangle,
                button.Parent?.BackColor ?? button.BackColor);
        }

        if (context.Bounds.Width < 8 || context.Bounds.Height < 8)
        {
            PopupButtonKeyCapRenderer.Render(graphics, context);
            return;
        }

        Image? image = button.Image;
        Rectangle contentBounds = PopupButtonKeyCapRenderer.GetContentBounds(context);
        ButtonBaseAdapter.LayoutData layout = _layoutAdapter.GetLayoutData(contentBounds);
        Action<Rectangle>? paintBackgroundImage = null;
        Action<Rectangle>? paintImage = null;

        if (button.BackgroundImage is not null)
        {
            paintBackgroundImage = bounds =>
            {
                using PaintEventArgs paintEventArgs = new(graphics, button.ClientRectangle);
                _layoutAdapter.PaintBackgroundImage(paintEventArgs, bounds);
            };
        }

        if (image is not null)
        {
            paintImage = _ =>
            {
                using PaintEventArgs paintEventArgs = new(graphics, button.ClientRectangle);
                _layoutAdapter.PaintImage(paintEventArgs, layout);
            };
        }

        PopupButtonKeyCapRenderer.Render(
            graphics,
            context,
            paintImage,
            (layout.TextBounds, layout.ImageBounds),
            paintBackgroundImage);
    }

    internal (Color BaseColor, Color HoverColor, Color PressedColor) GetStateColors()
    {
        Forms.ButtonBase button = Button;
        FlatButtonAppearance flatAppearance = button.FlatAppearance;
        _baseColorRenderer.DeviceDpi = button.DeviceDpi;
        _baseColorRenderer.FlatAppearance = flatAppearance;

        bool hasCustomBackColor = button.BackColor != Forms.Control.DefaultBackColor;
        Color baseColor = hasCustomBackColor
            ? button.BackColor
            : _baseColorRenderer.GetBackgroundColor(PushButtonState.Normal, isDefault: false);
        Color hoverColor = !flatAppearance.MouseOverBackColorCore.IsEmpty
            ? flatAppearance.MouseOverBackColorCore
            : hasCustomBackColor
                ? PopupButtonColorMath.TowardsContrast(baseColor, CustomHoverShadeAmount)
                : _baseColorRenderer.GetBackgroundColor(PushButtonState.Hot, isDefault: false);
        Color pressedColor = !flatAppearance.MouseDownBackColorCore.IsEmpty
            ? flatAppearance.MouseDownBackColorCore
            : hasCustomBackColor
                ? PopupButtonColorMath.TowardsContrast(baseColor, CustomPressedShadeAmount)
                : _baseColorRenderer.GetBackgroundColor(PushButtonState.Pressed, isDefault: false);

        return (baseColor, hoverColor, pressedColor);
    }

    private static float Lerp(float start, float end, float amount) => start + ((end - start) * amount);
}
