// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Rendering.Animation;

namespace System.Windows.Forms.Rendering.TextBox;

internal class AnimatedBorderStyleRenderer : AnimatedControlRenderer
{
    private const int AnimationDuration = 200; // milliseconds
    private readonly TextBoxBase _textBox;

    private readonly HDC _hdc;
    private Graphics? _graphics;

    public AnimatedBorderStyleRenderer(Control control, HDC hdc) : base(control)
    {
        _textBox = (TextBoxBase)control;
        _hdc = hdc;
    }

    protected override (int animationDuration, AnimationCycle animationCycle) OnAnimationStarted()
        => (AnimationDuration, AnimationCycle.Once);

    public override void AnimationProc(float animationProgress)
    {
        AnimationProgress = animationProgress;

        if (_graphics is null)
        {
            _graphics = Graphics.FromHdc(_hdc);
        }

        RenderControl(_graphics);
    }

    public override void RenderControl(Graphics graphics)
    {
        int borderThickness = 2;
        int deflateOffset = borderThickness / 2;
        int cornerRadius = 15;

        Color adornerColor = _textBox.ForeColor;
        Color parentBackColor = _textBox.Parent?.BackColor ?? _textBox.BackColor;
        Color clientBackColor = _textBox.BackColor;

        using var parentBackgroundBrush = parentBackColor.GetCachedSolidBrushScope();
        using var clientBackgroundBrush = clientBackColor.GetCachedSolidBrushScope();
        using var adornerBrush = adornerColor.GetCachedSolidBrushScope();
        using var adornerPen = adornerColor.GetCachedPenScope(borderThickness);
        using var focusPen = SystemColors.MenuHighlight.GetCachedPenScope(borderThickness);

        Rectangle bounds = _textBox.Bounds;

        Rectangle clientBounds = new(
            (bounds.Width - _textBox.ClientRectangle.Width) / 2,
            (bounds.Height - _textBox.ClientRectangle.Height) / 2,
            _textBox.ClientRectangle.Width,
            _textBox.ClientRectangle.Height);

        Rectangle deflatedBounds = new(
            x: bounds.Left + _textBox.Padding.Left + deflateOffset,
            y: bounds.Top + _textBox.Padding.Top + deflateOffset,
            width: bounds.Width - (_textBox.Padding.Horizontal + deflateOffset + deflateOffset),
            height: bounds.Height - (_textBox.Padding.Vertical + deflateOffset + deflateOffset));

        using GraphicsStateScope state = new(graphics);

        // We need Anti-Aliasing:
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        // Translate the origin to the top-left corner of the control:
        graphics.TranslateTransform(-bounds.Left, -bounds.Top);

        bounds.Inflate(1, 1);

        Rectangle translatedClientBounds = new(
            clientBounds.Left,
            clientBounds.Top,
            clientBounds.Width,
            clientBounds.Height);

        // Contrast to Copilot's suggestions, we need to first exclude
        // the clip, then translate the origin.
        graphics.ExcludeClip(translatedClientBounds);

        // Fill the background with the specified brush:
        graphics.FillRectangle(parentBackgroundBrush, bounds);

        switch (_textBox.BorderStyle)
        {
            case BorderStyle.None:

                // Draw a rounded Rectangle with the border thickness
                graphics.FillRectangle(
                    clientBackgroundBrush,
                    deflatedBounds);

                break;

            case BorderStyle.FixedSingle:

                // Draw a rounded Rectangle with the border thickness
                graphics.FillRectangle(
                    clientBackgroundBrush,
                    deflatedBounds);

                // Draw a rounded Rectangle with the border thickness
                graphics.DrawRectangle(
                    adornerPen,
                    deflatedBounds);

                break;

            default:

                // fill a rounded Rectangle
                graphics.FillRoundedRectangle(
                    clientBackgroundBrush,
                    deflatedBounds,
                    new Size(cornerRadius, cornerRadius));

                // Draw a rounded Rectangle with the border thickness
                graphics.DrawRoundedRectangle(
                    adornerPen,
                    deflatedBounds,
                    new Size(cornerRadius, cornerRadius));

                break;
        }

        // We draw an animated line at the bottom of the TextBox. That line starts of in the middle of the TextBox,
        // directly on the border in an 8th of the TextBox's width. It then moves simultaneously to the left and right
        // until it reaches the border on both sides.
        int lineLength = _textBox.Width / 8;
        int lineStartX = (_textBox.Width - lineLength) / 2;
        int lineStartY = _textBox.Height - borderThickness;

        int lineOffset = (int)(lineLength * AnimationProgress);

        int lineLeftX = lineStartX - lineOffset;
        int lineRightX = lineStartX + lineLength + lineOffset;

        graphics.DrawLine(focusPen, lineLeftX, lineStartY, lineRightX, lineStartY);
    }

    protected override void OnAnimationEnded()
    {
    }

    protected override void OnAnimationStopped()
    {
    }
}
