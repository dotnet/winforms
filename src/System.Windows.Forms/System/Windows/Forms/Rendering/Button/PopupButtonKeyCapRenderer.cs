// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.Rendering.Button;

/// <summary>
///  Renders a concave mechanical key top - the classic table-calculator/cash-register key with a raised rim
///  and a shallow bowl - used for the <see cref="FlatStyle.Popup"/> button visual style for arbitrary colors,
///  DPI values and interaction states.
/// </summary>
/// <remarks>
///  <para>
///   The renderer is completely stateless with respect to any control: every input arrives via
///   <see cref="PopupButtonRenderContext"/>. It can therefore render previews, designer adornments or key
///   visuals inside other controls without a live control instance.
///  </para>
///  <para>
///   Visual layers, back to front: ambient drop shadow, key body (rim surface), concave bowl (path gradient
///   plus inner top-shadow and bottom-light overlays), bowl lip stroke, border, default/focus cues, the
///   optional image, and finally the caption with a raised or engraved relief.
///  </para>
///  <para>
///   In high-contrast accessibility modes the renderer falls back to a flat, high-contrast style without any
///   material emulation.
///  </para>
/// </remarks>
internal sealed class PopupButtonKeyCapRenderer
{
    /// <summary>
    ///  Gets a shared default renderer instance. The renderer is stateless and thread-safe.
    /// </summary>
    public static PopupButtonKeyCapRenderer Default { get; } = new();

    /// <summary>
    ///  Renders the key into the given <see cref="Graphics"/>.
    /// </summary>
    /// <param name="graphics">The target graphics.</param>
    /// <param name="context">The complete render context.</param>
    /// <param name="paintImage">
    ///  Optional callback used to paint an image onto the key surface. It is invoked after the key chrome and
    ///  before the caption, and receives the bowl (content) rectangle.
    /// </param>
    public void Render(Graphics graphics, PopupButtonRenderContext context, Action<Rectangle>? paintImage = null)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(context);

        Rectangle bounds = context.Bounds;

        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        // Degenerate bounds: just fill, never throw.
        if (bounds.Width < 8 || bounds.Height < 8)
        {
            using SolidBrush tinyBrush = new(context.BackColor);
            graphics.FillRectangle(tinyBrush, bounds);

            return;
        }

        if (context.HighContrast)
        {
            RenderHighContrast(graphics, context, paintImage);

            return;
        }

        Metrics metrics = Metrics.Create(context);
        Palette palette = Palette.Create(context, metrics);

        GraphicsState state = graphics.Save();

        try
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            DrawAmbientShadow(graphics, metrics, palette);
            DrawKeyBody(graphics, metrics, palette);
            DrawBowl(graphics, metrics, palette);
            DrawBorder(graphics, metrics, palette);
            DrawStateCues(graphics, context, metrics, palette);

            paintImage?.Invoke(metrics.BowlRect);
        }
        finally
        {
            graphics.Restore(state);
        }

        DrawText(graphics, context, metrics, palette);
    }

    private static void DrawAmbientShadow(Graphics graphics, Metrics metrics, Palette palette)
    {
        // The key visually lifts off the surface; pressing it reduces the drop shadow.
        int drop = Math.Max(1, metrics.Ambient - metrics.PressOffset);

        Rectangle outer = metrics.KeyRect;
        outer.Offset(drop / 2, drop);
        outer.Inflate(1, 1);

        Rectangle inner = metrics.KeyRect;
        inner.Offset(drop / 2, drop);

        using (GraphicsPath outerPath = CreateRoundedPath(outer, metrics.CornerRadius + 1f))
        using (SolidBrush softBrush = new(Color.FromArgb(palette.AmbientAlpha / 3, Color.Black)))
        {
            graphics.FillPath(softBrush, outerPath);
        }

        using GraphicsPath innerPath = CreateRoundedPath(inner, metrics.CornerRadius);
        using SolidBrush coreBrush = new(Color.FromArgb(palette.AmbientAlpha, Color.Black));
        graphics.FillPath(coreBrush, innerPath);
    }

    private static void DrawKeyBody(Graphics graphics, Metrics metrics, Palette palette)
    {
        // The rim surface: lit from the upper left, falling into shadow at the lower right.
        using GraphicsPath bodyPath = CreateRoundedPath(metrics.KeyRect, metrics.CornerRadius);
        using LinearGradientBrush bodyBrush = new(
            InflateForGradient(metrics.KeyRect),
            palette.BodyLight,
            palette.BodyDark,
            LinearGradientMode.ForwardDiagonal);

        graphics.FillPath(bodyBrush, bodyPath);
    }

    private static void DrawBowl(Graphics graphics, Metrics metrics, Palette palette)
    {
        Rectangle bowl = metrics.BowlRect;

        if (bowl.Width < 2 || bowl.Height < 2)
        {
            return;
        }

        using GraphicsPath bowlPath = CreateRoundedPath(bowl, metrics.BowlRadius);

        // 1. Radial shading: edges catch light, the center sits lower and darker - the signature concave read.
        //    The dark center is biased towards the upper left, where a top-left light source cannot reach into
        //    a bowl.
        using (PathGradientBrush bowlBrush = new(bowlPath))
        {
            bowlBrush.CenterColor = palette.BowlCenter;
            bowlBrush.SurroundColors = [palette.BowlEdge];
            bowlBrush.CenterPoint = new PointF(
                bowl.Left + (bowl.Width * 0.40f),
                bowl.Top + (bowl.Height * 0.36f));
            bowlBrush.FocusScales = new PointF(0.28f, 0.22f);

            graphics.FillPath(bowlBrush, bowlPath);
        }

        // 2. Inner top shadow and inner bottom light, clipped to the bowl.
        GraphicsState clipState = graphics.Save();

        try
        {
            graphics.SetClip(bowlPath, CombineMode.Intersect);

            int topHeight = Math.Max(2, (int)(bowl.Height * 0.42f));
            Rectangle topRect = bowl with { Height = topHeight };

            using (LinearGradientBrush topShadow = new(
                InflateForGradient(topRect),
                Color.FromArgb(palette.InnerShadowAlpha, Color.Black),
                Color.FromArgb(0, Color.Black),
                LinearGradientMode.Vertical))
            {
                graphics.FillRectangle(topShadow, topRect);
            }

            int bottomHeight = Math.Max(2, (int)(bowl.Height * 0.30f));
            Rectangle bottomRect = new(bowl.Left, bowl.Bottom - bottomHeight, bowl.Width, bottomHeight);

            using LinearGradientBrush bottomLight = new(
                InflateForGradient(bottomRect),
                Color.FromArgb(0, Color.White),
                Color.FromArgb(palette.InnerLightAlpha, Color.White),
                LinearGradientMode.Vertical);

            graphics.FillRectangle(bottomLight, bottomRect);
        }
        finally
        {
            graphics.Restore(clipState);
        }

        // 3. The lip where rim and bowl meet: light on the upper left, shadow lower right - the raised edge of
        //    the surrounding rim.
        using LinearGradientBrush lipBrush = new(
            InflateForGradient(Rectangle.Inflate(bowl, 1, 1)),
            palette.LipLight,
            palette.LipDark,
            LinearGradientMode.ForwardDiagonal);
        using Pen lipPen = new(lipBrush, Math.Max(1f, metrics.Scale));

        graphics.DrawPath(lipPen, bowlPath);
    }

    private static void DrawBorder(Graphics graphics, Metrics metrics, Palette palette)
    {
        if (metrics.BorderWidth <= 0)
        {
            return;
        }

        float half = metrics.BorderWidth / 2f;
        RectangleF borderRect = metrics.KeyRect;
        borderRect.Inflate(-half, -half);

        if (borderRect.Width < 1f || borderRect.Height < 1f)
        {
            return;
        }

        using GraphicsPath borderPath = CreateRoundedPath(
            Rectangle.Round(borderRect),
            Math.Max(1f, metrics.CornerRadius - half));
        using Pen borderPen = new(palette.Border, metrics.BorderWidth);

        graphics.DrawPath(borderPen, borderPath);
    }

    private static void DrawStateCues(
        Graphics graphics,
        PopupButtonRenderContext context,
        Metrics metrics,
        Palette palette)
    {
        if (context.IsDefault && context.Enabled)
        {
            // Default-button cue: an additional discreet ring just inside the border.
            int inset = metrics.BorderWidth + Math.Max(1, (int)metrics.Scale);
            Rectangle cueRect = Rectangle.Inflate(metrics.KeyRect, -inset, -inset);

            if (cueRect.Width > 4 && cueRect.Height > 4)
            {
                using GraphicsPath cuePath = CreateRoundedPath(
                    cueRect,
                    Math.Max(1f, metrics.CornerRadius - inset));
                using Pen cuePen = new(palette.DefaultCue, Math.Max(1f, metrics.Scale * 0.75f));

                graphics.DrawPath(cuePen, cuePath);
            }
        }

        if (context.Focused)
        {
            int inset = Math.Max(2, (int)MathF.Round(2.5f * metrics.Scale));
            Rectangle focusRect = Rectangle.Inflate(metrics.BowlRect, -inset, -inset);

            if (focusRect.Width > 4 && focusRect.Height > 4)
            {
                using GraphicsPath focusPath = CreateRoundedPath(
                    focusRect,
                    Math.Max(1f, metrics.BowlRadius - inset));
                using Pen focusPen = new(palette.Focus, Math.Max(1f, metrics.Scale * 0.75f))
                {
                    DashStyle = DashStyle.Dot
                };

                graphics.DrawPath(focusPen, focusPath);
            }
        }
    }

    private static void DrawText(
        Graphics graphics,
        PopupButtonRenderContext context,
        Metrics metrics,
        Palette palette)
    {
        string? text = context.Text;

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        int inset = Math.Max(1, (int)MathF.Round(1.5f * metrics.Scale));
        Rectangle textRect = Rectangle.Inflate(metrics.BowlRect, -inset, -inset);
        textRect = ApplyPadding(textRect, context.Padding);

        if (textRect.Width <= 0 || textRect.Height <= 0)
        {
            return;
        }

        // The key top already moves with the press; the caption sinks a touch further, which sells the
        // "finger pushes the key down" moment.
        int extraSink = (int)MathF.Round(context.AnimationState.PressProgress * 0.75f * metrics.Scale);
        textRect.Offset(0, extraSink);

        TextFormatFlags flags = GetTextFormatFlags(context);
        int reliefOffset = metrics.TextReliefOffset;

        PopupButtonTextEffect effect = context.TextEffect;

        if (effect is not PopupButtonTextEffect.Flat)
        {
            // GDI text ignores alpha, so the relief colors are opaque blends against the bowl center - see
            // PopupButtonColorMath.GetTextHighlight/GetTextShadow.
            (Color reliefColor, Point reliefShift) = effect switch
            {
                // Raised: light from the upper left casts the glyph's shadow downwards.
                PopupButtonTextEffect.Raised => (palette.TextShadow, new Point(0, reliefOffset)),

                // Engraved (letterpress): the recess's lower edge catches the light.
                _ => (palette.TextHighlight, new Point(0, reliefOffset))
            };

            Rectangle reliefRect = textRect;
            reliefRect.Offset(reliefShift);
            TextRenderer.DrawText(graphics, text, context.Font, reliefRect, reliefColor, flags);

            if (effect is PopupButtonTextEffect.Engraved)
            {
                // A faint dark edge above completes the engraving.
                Rectangle upperRect = textRect;
                upperRect.Offset(0, -reliefOffset);
                Color upperShadow = PopupButtonColorMath.Blend(palette.TextShadow, palette.BowlCenter, 0.55f);
                TextRenderer.DrawText(graphics, text, context.Font, upperRect, upperShadow, flags);
            }
        }

        TextRenderer.DrawText(graphics, text, context.Font, textRect, palette.Text, flags);
    }

    private static void RenderHighContrast(Graphics graphics, PopupButtonRenderContext context, Action<Rectangle>? paintImage)
    {
        Rectangle bounds = context.Bounds;
        bool pressed = context.Pressed;
        float scale = context.DeviceDpi / 96f;

        Color back = context.BackColor;
        Color fore = context.Enabled ? context.ForeColor : SystemColors.GrayText;
        Color border = context.Enabled ? context.ForeColor : SystemColors.GrayText;

        using (SolidBrush backBrush = new(back))
        {
            graphics.FillRectangle(backBrush, bounds);
        }

        int borderWidth = Math.Max(Math.Max(1, context.BorderWidth), pressed ? (int)(2 * scale) : 1);
        Rectangle borderRect = bounds;
        borderRect.Width -= 1;
        borderRect.Height -= 1;

        using (Pen borderPen = new(border, borderWidth) { Alignment = PenAlignment.Inset })
        {
            graphics.DrawRectangle(borderPen, borderRect);
        }

        Rectangle contentRect = Rectangle.Inflate(bounds, -(int)(4 * scale), -(int)(4 * scale));

        if (pressed)
        {
            contentRect.Offset((int)scale, (int)scale);
        }

        paintImage?.Invoke(contentRect);

        Rectangle textRect = ApplyPadding(contentRect, context.Padding);

        if (textRect.Width > 0 && textRect.Height > 0)
        {
            TextRenderer.DrawText(graphics, context.Text, context.Font, textRect, fore, GetTextFormatFlags(context));
        }

        if (context.Focused)
        {
            Rectangle focusRect = Rectangle.Inflate(bounds, -(int)(3 * scale), -(int)(3 * scale));
            ControlPaint.DrawFocusRectangle(graphics, focusRect, fore, back);
        }
    }

    /// <summary>
    ///  Creates a rounded-rectangle path; degrades to a plain rectangle for tiny radii and clamps the radius so
    ///  it never exceeds half of the smaller side.
    /// </summary>
    private static GraphicsPath CreateRoundedPath(Rectangle rect, float radius)
    {
        GraphicsPath path = new();

        radius = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2f);

        if (radius < 1f || rect.Width < 2 || rect.Height < 2)
        {
            path.AddRectangle(rect);

            return path;
        }

        float diameter = radius * 2f;
        RectangleF arc = new(rect.X, rect.Y, diameter, diameter);

        path.AddArc(arc, 180f, 90f);
        arc.X = rect.Right - diameter;
        path.AddArc(arc, 270f, 90f);
        arc.Y = rect.Bottom - diameter;
        path.AddArc(arc, 0f, 90f);
        arc.X = rect.X;
        path.AddArc(arc, 90f, 90f);
        path.CloseFigure();

        return path;
    }

    private static Rectangle ApplyPadding(Rectangle rect, Padding padding)
        => new(
            rect.X + padding.Left,
            rect.Y + padding.Top,
            Math.Max(0, rect.Width - padding.Horizontal),
            Math.Max(0, rect.Height - padding.Vertical));

    /// <summary>
    ///  Grows a gradient rectangle by one pixel to avoid GDI+ edge-seam artifacts and to guarantee non-zero
    ///  dimensions.
    /// </summary>
    private static Rectangle InflateForGradient(Rectangle rect)
    {
        Rectangle result = Rectangle.Inflate(rect, 1, 1);

        if (result.Width < 2)
        {
            result.Width = 2;
        }

        if (result.Height < 2)
        {
            result.Height = 2;
        }

        return result;
    }

    private static TextFormatFlags GetTextFormatFlags(PopupButtonRenderContext context)
    {
        bool rtl = context.RightToLeft == RightToLeft.Yes;
        ContentAlignment align = rtl ? MirrorAlignment(context.TextAlign) : context.TextAlign;

        TextFormatFlags flags = align switch
        {
            ContentAlignment.TopLeft => TextFormatFlags.Top | TextFormatFlags.Left,
            ContentAlignment.TopCenter => TextFormatFlags.Top | TextFormatFlags.HorizontalCenter,
            ContentAlignment.TopRight => TextFormatFlags.Top | TextFormatFlags.Right,
            ContentAlignment.MiddleLeft => TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            ContentAlignment.MiddleRight => TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
            ContentAlignment.BottomLeft => TextFormatFlags.Bottom | TextFormatFlags.Left,
            ContentAlignment.BottomCenter => TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter,
            ContentAlignment.BottomRight => TextFormatFlags.Bottom | TextFormatFlags.Right,
            _ => TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
        };

        flags |= TextFormatFlags.EndEllipsis;

        if (context.Text is not null && !context.Text.Contains('\n'))
        {
            flags |= TextFormatFlags.SingleLine;
        }

        if (rtl)
        {
            flags |= TextFormatFlags.RightToLeft;
        }

        if (!context.ShowKeyboardCues)
        {
            flags |= TextFormatFlags.HidePrefix;
        }

        return flags;
    }

    private static ContentAlignment MirrorAlignment(ContentAlignment alignment)
        => alignment switch
        {
            ContentAlignment.TopLeft => ContentAlignment.TopRight,
            ContentAlignment.TopRight => ContentAlignment.TopLeft,
            ContentAlignment.MiddleLeft => ContentAlignment.MiddleRight,
            ContentAlignment.MiddleRight => ContentAlignment.MiddleLeft,
            ContentAlignment.BottomLeft => ContentAlignment.BottomRight,
            ContentAlignment.BottomRight => ContentAlignment.BottomLeft,
            _ => alignment
        };

    /// <summary>
    ///  Device-resolved geometry and animation-modulated shading amounts for one render pass.
    /// </summary>
    private readonly struct Metrics
    {
        public float Scale { get; init; }
        public int Ambient { get; init; }
        public int BorderWidth { get; init; }
        public int Rim { get; init; }
        public float CornerRadius { get; init; }
        public Rectangle KeyRect { get; init; }
        public Rectangle BowlRect { get; init; }
        public float BowlRadius { get; init; }
        public int PressOffset { get; init; }
        public float Hover { get; init; }
        public float Press { get; init; }
        public float HighlightAmount { get; init; }
        public float ShadowAmount { get; init; }
        public int TextReliefOffset { get; init; }

        public static Metrics Create(PopupButtonRenderContext context)
        {
            PopupButtonRenderOptions options = context.Options;
            Rectangle bounds = context.Bounds;

            float scale = Math.Max(0.5f, context.DeviceDpi / 96f);
            float hover = context.AnimationState.HoverProgress;
            float press = context.AnimationState.PressProgress;

            int ambient = Math.Max(1, (int)MathF.Round(2.5f * scale));
            int maxBorder = Math.Max(0, (Math.Min(bounds.Width, bounds.Height) / 4) - 1);
            int borderWidth = Math.Clamp(context.BorderWidth, 0, maxBorder);

            Rectangle keyRect = Rectangle.Inflate(bounds, -ambient, -ambient);

            // Pressing sinks the key top; the bottom edge stays put, so the cap compresses.
            int pressOffset = (int)MathF.Round(press * 1.5f * scale);
            keyRect.Y += pressOffset;
            keyRect.Height = Math.Max(4, keyRect.Height - pressOffset);

            int rim = Math.Max(2, (int)MathF.Round(3f * scale));
            int bowlInset = borderWidth + rim;

            if (keyRect.Width - (2 * bowlInset) < 8 || keyRect.Height - (2 * bowlInset) < 8)
            {
                rim = Math.Max(1, rim / 2);
                bowlInset = borderWidth + rim;
            }

            Rectangle bowlRect = Rectangle.Inflate(keyRect, -bowlInset, -bowlInset);

            if (bowlRect.Width < 2 || bowlRect.Height < 2)
            {
                bowlRect = Rectangle.Inflate(keyRect, -1, -1);
            }

            float cornerRadius = Math.Clamp(
                options.GetCornerRadiusDip() * scale,
                1f,
                Math.Min(keyRect.Width, keyRect.Height) / 2f);
            float bowlRadius = Math.Max(1f, cornerRadius - (rim * 0.6f));

            // Pressing deepens the bowl; hovering flattens it a touch, as if the key rises to meet the finger.
            // Highlights brighten on hover, shadows deepen on press.
            float depth = options.GetConcavityDepth() * (1f + (press * 0.7f) - (hover * 0.12f));
            float highlight = Math.Clamp(
                depth * options.GetHighlightMultiplier() * (1f + (hover * 0.55f)),
                0.02f,
                0.6f);
            float shadow = Math.Clamp(
                depth * options.GetShadowMultiplier() * (1f + (press * 0.35f)),
                0.02f,
                0.6f);

            return new Metrics
            {
                Scale = scale,
                Ambient = ambient,
                BorderWidth = borderWidth,
                Rim = rim,
                CornerRadius = cornerRadius,
                KeyRect = keyRect,
                BowlRect = bowlRect,
                BowlRadius = bowlRadius,
                PressOffset = pressOffset,
                Hover = hover,
                Press = press,
                HighlightAmount = highlight,
                ShadowAmount = shadow,
                TextReliefOffset = Math.Max(1, (int)MathF.Round(0.8f * scale))
            };
        }
    }

    /// <summary>
    ///  All colors of one render pass, derived from the effective context colors so the material effect adapts
    ///  to any <c>BackColor</c>/<c>ForeColor</c> combination.
    /// </summary>
    private readonly struct Palette
    {
        public Color BodyLight { get; init; }
        public Color BodyDark { get; init; }
        public Color BowlEdge { get; init; }
        public Color BowlCenter { get; init; }
        public Color LipLight { get; init; }
        public Color LipDark { get; init; }
        public Color Border { get; init; }
        public Color Text { get; init; }
        public Color TextHighlight { get; init; }
        public Color TextShadow { get; init; }
        public Color Focus { get; init; }
        public Color DefaultCue { get; init; }
        public int AmbientAlpha { get; init; }
        public int InnerShadowAlpha { get; init; }
        public int InnerLightAlpha { get; init; }

        public static Palette Create(PopupButtonRenderContext context, Metrics metrics)
        {
            bool enabled = context.Enabled;

            // Disabled keys mute the material but keep the concave form readable - reduced contrast rather than
            // flat gray.
            Color back = enabled ? context.BackColor : PopupButtonColorMath.Mute(context.BackColor, 0.55f);
            float contrast = enabled ? 1f : 0.35f;
            float luminance = PopupButtonColorMath.GetLuminance(back);

            Color border = enabled
                ? context.BorderColor
                : PopupButtonColorMath.Blend(context.BorderColor, back, 0.45f);

            // Keep the border visible even if the user picked one too close to the face color.
            border = PopupButtonColorMath.EnsureContrast(border, back, 0.08f);

            Color text = enabled
                ? context.ForeColor
                : PopupButtonColorMath.EnsureContrast(PopupButtonColorMath.Blend(context.ForeColor, back, 0.45f), back, 0.18f);

            float reliefStrength = (enabled ? 1f : 0.4f) + (metrics.Press * 0.25f);
            Color bowlCenter = PopupButtonColorMath.Darken(back, (metrics.ShadowAmount * 0.75f * contrast) + (enabled ? 0f : 0.02f));

            return new Palette
            {
                BodyLight = PopupButtonColorMath.Lighten(back, metrics.HighlightAmount * 0.9f * contrast),
                BodyDark = PopupButtonColorMath.Darken(back, metrics.ShadowAmount * 0.9f * contrast),
                BowlEdge = PopupButtonColorMath.Lighten(back, metrics.HighlightAmount * 0.55f * contrast),
                BowlCenter = bowlCenter,
                LipLight = PopupButtonColorMath.Lighten(back, metrics.HighlightAmount * 1.3f * contrast),
                LipDark = PopupButtonColorMath.Darken(back, metrics.ShadowAmount * 1.2f * contrast),
                Border = border,
                Text = text,
                TextHighlight = PopupButtonColorMath.GetTextHighlight(bowlCenter, reliefStrength),
                TextShadow = PopupButtonColorMath.GetTextShadow(bowlCenter, reliefStrength),
                Focus = PopupButtonColorMath.TowardsContrast(back, 0.55f),
                DefaultCue = PopupButtonColorMath.TowardsContrast(border, 0.3f),
                AmbientAlpha = (int)((luminance > 0.5f ? 55f : 85f)
                    * (1f - (metrics.Press * 0.6f))
                    * (enabled ? 1f : 0.5f)),
                InnerShadowAlpha = Math.Clamp(
                    (int)((30f + (metrics.ShadowAmount * 380f)) * contrast), 0, 120),
                InnerLightAlpha = Math.Clamp(
                    (int)((20f + (metrics.HighlightAmount * 300f)) * contrast), 0, 100)
            };
        }
    }
}
