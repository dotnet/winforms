// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.Rendering.Button;

/// <summary>
///  Drives and renders a <see cref="Forms.Button"/> whose <see cref="Forms.ButtonBase.FlatStyle"/> is
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

    private float _hoverCurrent;
    private float _hoverStart;
    private float _hoverTarget;

    private float _pressCurrent;
    private float _pressStart;
    private float _pressTarget;

    public AnimatedPopupButtonRenderer(Forms.Button button)
        : base(button)
    {
    }

    private Forms.Button Button => (Forms.Button)Control;

    /// <summary>
    ///  Updates the hover and press targets from the current interaction state and, if anything changed,
    ///  (re)starts the interpolation from the current channel values towards the new targets.
    /// </summary>
    public void SetInteractionState(bool hovered, bool pressed)
    {
        float hoverTarget = hovered ? 1f : 0f;
        float pressTarget = pressed ? 1f : 0f;

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
        Forms.Button button = Button;

        Color faceColor = button.BackColor;
        FlatButtonAppearance flatAppearance = button.FlatAppearance;

        Color borderColor = flatAppearance.BorderColor.IsEmpty
            ? PopupButtonColorMath.TowardsContrast(faceColor, 0.35f)
            : flatAppearance.BorderColor;

        PopupButtonRenderContext context = new()
        {
            Bounds = button.ClientRectangle,
            Text = button.Text,
            Font = button.Font,
            BackColor = faceColor,
            ForeColor = button.ForeColor,
            BorderColor = borderColor,
            BorderWidth = flatAppearance.BorderSize,
            Enabled = button.Enabled,
            Focused = button.Focused && button.ShowFocusCues,
            Pressed = button.MouseIsDown,
            IsDefault = button.IsDefault,
            AnimationState = new PopupButtonAnimationState(_hoverCurrent, _pressCurrent),
            TextAlign = button.TextAlign,
            RightToLeft = button.RightToLeft,
            Padding = button.Padding,
            DeviceDpi = button.DeviceDpi,
            ShowKeyboardCues = button.ShowKeyboardCues,
        };

        Action<Rectangle>? paintImage = null;
        Image? image = button.Image;

        if (image is not null)
        {
            paintImage = contentBounds =>
            {
                Rectangle imageBounds = AlignInRectangle(contentBounds, image.Size, button.ImageAlign);

                if (button.Enabled)
                {
                    graphics.DrawImage(image, imageBounds);
                }
                else
                {
                    ControlPaint.DrawImageDisabled(graphics, image, imageBounds.X, imageBounds.Y, faceColor);
                }
            };
        }

        PopupButtonKeyCapRenderer.Render(graphics, context, paintImage);
    }

    private static float Lerp(float start, float end, float amount) => start + ((end - start) * amount);

    private static Rectangle AlignInRectangle(Rectangle container, Size size, ContentAlignment alignment)
    {
        int x = alignment switch
        {
            ContentAlignment.TopLeft or ContentAlignment.MiddleLeft or ContentAlignment.BottomLeft => container.Left,
            ContentAlignment.TopRight or ContentAlignment.MiddleRight or ContentAlignment.BottomRight => container.Right - size.Width,
            _ => container.Left + ((container.Width - size.Width) / 2)
        };

        int y = alignment switch
        {
            ContentAlignment.TopLeft or ContentAlignment.TopCenter or ContentAlignment.TopRight => container.Top,
            ContentAlignment.BottomLeft or ContentAlignment.BottomCenter or ContentAlignment.BottomRight => container.Bottom - size.Height,
            _ => container.Top + ((container.Height - size.Height) / 2)
        };

        return new Rectangle(x, y, size.Width, size.Height);
    }
}
