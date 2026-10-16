// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Layout;

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
internal static class PopupButtonKeyCapRenderer
{
    /// <summary>
    ///  Renders the key into the given <see cref="Graphics"/>.
    /// </summary>
    /// <param name="graphics">The target graphics.</param>
    /// <param name="context">The complete render context.</param>
    /// <param name="paintImage">
    ///  Optional callback used to paint an image onto the key surface. It is invoked after the key chrome and
    ///  before the caption, and receives the bowl (content) rectangle.
    /// </param>
    public static void Render(
        Graphics graphics,
        PopupButtonRenderContext context,
        Action<Rectangle>? paintImage = null,
        (Rectangle TextBounds, Rectangle ImageBounds)? contentLayout = null,
        Action<Rectangle>? paintBackgroundImage = null)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(context);

        Rectangle bounds = context.Bounds;
        Color surfaceBackColor = GetSurfaceBackColor(context);

        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        // Degenerate bounds: just fill, never throw.
        if (bounds.Width < 8 || bounds.Height < 8)
        {
            using SolidBrush tinyBrush = new(surfaceBackColor);
            graphics.FillRectangle(tinyBrush, bounds);

            return;
        }

        if (context.HighContrast)
        {
            RenderHighContrast(
                graphics,
                context,
                surfaceBackColor,
                paintImage,
                contentLayout,
                paintBackgroundImage);

            return;
        }

        Metrics metrics = Metrics.Create(context);
        Palette palette = Palette.Create(context, metrics, surfaceBackColor);
        (Rectangle textBounds, Rectangle imageBounds) = contentLayout
            ?? CreateContentLayout(
                context,
                metrics.BowlRect,
                applySurfaceInset: false);

        GraphicsState state = graphics.Save();

        try
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            DrawAmbientShadow(graphics, metrics, palette);
            DrawKeyBody(graphics, metrics, palette);
            DrawBowl(graphics, metrics, palette);
            PaintBackgroundImage(
                graphics,
                metrics,
                paintBackgroundImage);
            DrawBorder(graphics, metrics, palette);
            DrawStateCues(graphics, context, metrics, palette);

            if (imageBounds.Width > 0 && imageBounds.Height > 0)
            {
                paintImage?.Invoke(imageBounds);
            }
        }
        finally
        {
            graphics.Restore(state);
        }

        DrawText(graphics, context, metrics, palette, textBounds);
    }

    internal static Rectangle GetContentBounds(PopupButtonRenderContext context)
    {
        if (!context.HighContrast)
        {
            return Metrics.Create(context).BowlRect;
        }

        float scale = context.DeviceDpi / 96f;
        Rectangle contentBounds = Rectangle.Inflate(
            context.Bounds,
            -(int)(4 * scale),
            -(int)(4 * scale));

        if (context.Pressed)
        {
            contentBounds.Offset((int)scale, (int)scale);
        }

        return contentBounds;
    }

    internal static Size GetPreferredSizeChrome(int deviceDpi, int borderWidth)
    {
        float scale = Math.Max(0.5f, deviceDpi / 96f);
        int sideClearance = Math.Max(1, (int)MathF.Round(Metrics.SideClearanceDip * scale));
        int pressTravel = Math.Max(1, (int)MathF.Round(Metrics.PressTravelDip * scale));
        int defaultBorderIncrease = Math.Max(1, (int)MathF.Round(scale));
        int stableBorderWidth = Math.Max(0, borderWidth + defaultBorderIncrease);
        int rim = Math.Max(2, (int)MathF.Round(3f * scale));
        int bowlInset = stableBorderWidth + rim;

        return new Size(
            2 * (sideClearance + bowlInset),
            (2 * sideClearance) + pressTravel + (2 * bowlInset));
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
        if (context.Focused)
        {
            int inset = metrics.BorderWidth + Math.Max(2, (int)MathF.Round(2f * metrics.Scale));
            Rectangle focusRect = Rectangle.Inflate(metrics.KeyRect, -inset, -inset);

            if (focusRect.Width > 4 && focusRect.Height > 4)
            {
                using GraphicsPath focusPath = CreateRoundedPath(
                    focusRect,
                    Math.Max(1f, metrics.CornerRadius - inset));
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
        Palette palette,
        Rectangle textRect)
    {
        string? text = context.Text;

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (textRect.Width <= 0 || textRect.Height <= 0)
        {
            return;
        }

        TextFormatFlags flags = GetTextFormatFlags(context);
        int reliefOffset = metrics.TextReliefOffset;

        PopupButtonTextEffect effect = GetTextEffect(context);

        if (!palette.TextOutline.IsEmpty)
        {
            for (int y = -reliefOffset; y <= reliefOffset; y += reliefOffset)
            {
                for (int x = -reliefOffset; x <= reliefOffset; x += reliefOffset)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    Rectangle outlineRect = textRect;
                    outlineRect.Offset(x, y);
                    TextRenderer.DrawText(
                        graphics,
                        text,
                        context.Font,
                        outlineRect,
                        palette.TextOutline,
                        flags);
                }
            }
        }

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

    internal static PopupButtonTextEffect GetTextEffect(
        PopupButtonRenderContext context)
        => context.Enabled
            ? context.TextEffect
            : PopupButtonTextEffect.Flat;

    private static void PaintBackgroundImage(
        Graphics graphics,
        Metrics metrics,
        Action<Rectangle>? paintBackgroundImage)
    {
        if (paintBackgroundImage is null)
        {
            return;
        }

        int inset = Math.Max(
            1,
            (int)MathF.Ceiling(metrics.Scale));
        Rectangle backgroundBounds = Rectangle.Inflate(
            metrics.BowlRect,
            -inset,
            -inset);
        if (backgroundBounds.Width <= 0
            || backgroundBounds.Height <= 0)
        {
            return;
        }

        using GraphicsStateScope state = new(graphics);
        using GraphicsPath clipPath = CreateRoundedPath(
            backgroundBounds,
            Math.Max(1f, metrics.BowlRadius - inset));
        graphics.SetClip(clipPath, CombineMode.Intersect);
        paintBackgroundImage(backgroundBounds);
    }

    private static void RenderHighContrast(
        Graphics graphics,
        PopupButtonRenderContext context,
        Color surfaceBackColor,
        Action<Rectangle>? paintImage,
        (Rectangle TextBounds, Rectangle ImageBounds)? contentLayout,
        Action<Rectangle>? paintBackgroundImage)
    {
        Rectangle bounds = context.Bounds;
        bool pressed = context.Pressed;
        float scale = context.DeviceDpi / 96f;

        Color back = surfaceBackColor;
        Color fore = context.Enabled ? context.ForeColor : SystemColors.GrayText;
        Color border = context.Enabled ? context.ForeColor : SystemColors.GrayText;
        Rectangle contentRect = GetContentBounds(context);

        using (SolidBrush backBrush = new(back))
        {
            graphics.FillRectangle(backBrush, bounds);
        }

        paintBackgroundImage?.Invoke(contentRect);

        int defaultBorderIncrease = context.IsDefault && context.Enabled
            ? Math.Max(1, (int)MathF.Round(scale))
            : 0;
        int borderWidth = Math.Max(
            Math.Max(1, context.BorderWidth + defaultBorderIncrease),
            pressed ? (int)(2 * scale) : 1);
        Rectangle borderRect = bounds;
        borderRect.Width -= 1;
        borderRect.Height -= 1;

        using (Pen borderPen = new(border, borderWidth) { Alignment = PenAlignment.Inset })
        {
            graphics.DrawRectangle(borderPen, borderRect);
        }

        (Rectangle textRect, Rectangle imageRect) = contentLayout
            ?? CreateContentLayout(
                context,
                contentRect,
                applySurfaceInset: false);
        if (imageRect.Width > 0 && imageRect.Height > 0)
        {
            paintImage?.Invoke(imageRect);
        }

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

    private static Color GetSurfaceBackColor(PopupButtonRenderContext context)
    {
        if (context.HighContrast || !context.IsDefault || !context.Enabled)
        {
            return context.BackColor;
        }

        return context.IsDarkMode
            ? PopupButtonColorMath.Lighten(context.BackColor, 0.1f)
            : PopupButtonColorMath.Darken(context.BackColor, 0.1f);
    }

    private static (Rectangle TextBounds, Rectangle ImageBounds) CreateContentLayout(
        PopupButtonRenderContext context,
        Rectangle contentBounds,
        bool applySurfaceInset)
    {
        if (applySurfaceInset)
        {
            int inset = Math.Max(1, (int)MathF.Round(1.5f * context.DeviceDpi / 96f));
            contentBounds = Rectangle.Inflate(contentBounds, -inset, -inset);
        }

        contentBounds = ApplyPadding(contentBounds, context.Padding);
        bool hasText = !string.IsNullOrEmpty(context.Text);
        bool hasImage = !context.ImageSize.IsEmpty;
        ContentAlignment imageAlign = context.RightToLeft == RightToLeft.Yes
            ? MirrorAlignment(context.ImageAlign)
            : context.ImageAlign;

        if (!hasImage)
        {
            return (hasText ? contentBounds : Rectangle.Empty, Rectangle.Empty);
        }

        Size imageSize = new(
            Math.Min(contentBounds.Width, context.ImageSize.Width),
            Math.Min(contentBounds.Height, context.ImageSize.Height));

        if (!hasText || context.TextImageRelation == TextImageRelation.Overlay)
        {
            return (
                hasText ? contentBounds : Rectangle.Empty,
                AlignInRectangle(contentBounds, imageSize, imageAlign));
        }

        TextImageRelation relation = context.RightToLeft == RightToLeft.Yes
            ? LayoutUtils.GetOppositeTextImageRelation(context.TextImageRelation)
            : context.TextImageRelation;
        int gap = Math.Max(2, (int)MathF.Round(4f * context.DeviceDpi / 96f));

        return relation switch
        {
            TextImageRelation.ImageBeforeText => CreateHorizontalLayout(imageFirst: true),
            TextImageRelation.TextBeforeImage => CreateHorizontalLayout(imageFirst: false),
            TextImageRelation.ImageAboveText => CreateVerticalLayout(imageFirst: true),
            TextImageRelation.TextAboveImage => CreateVerticalLayout(imageFirst: false),
            _ => (contentBounds, AlignInRectangle(contentBounds, imageSize, imageAlign))
        };

        (Rectangle TextBounds, Rectangle ImageBounds) CreateHorizontalLayout(bool imageFirst)
        {
            int imageWidth = Math.Min(imageSize.Width, Math.Max(0, contentBounds.Width - gap));
            Rectangle imageSlot = imageFirst
                ? new Rectangle(contentBounds.Left, contentBounds.Top, imageWidth, contentBounds.Height)
                : new Rectangle(contentBounds.Right - imageWidth, contentBounds.Top, imageWidth, contentBounds.Height);
            Rectangle textBounds = imageFirst
                ? Rectangle.FromLTRB(imageSlot.Right + gap, contentBounds.Top, contentBounds.Right, contentBounds.Bottom)
                : Rectangle.FromLTRB(contentBounds.Left, contentBounds.Top, imageSlot.Left - gap, contentBounds.Bottom);

            return (
                textBounds,
                AlignInRectangle(imageSlot, imageSize with { Width = imageWidth }, imageAlign));
        }

        (Rectangle TextBounds, Rectangle ImageBounds) CreateVerticalLayout(bool imageFirst)
        {
            int imageHeight = Math.Min(imageSize.Height, Math.Max(0, contentBounds.Height - gap));
            Rectangle imageSlot = imageFirst
                ? new Rectangle(contentBounds.Left, contentBounds.Top, contentBounds.Width, imageHeight)
                : new Rectangle(contentBounds.Left, contentBounds.Bottom - imageHeight, contentBounds.Width, imageHeight);
            Rectangle textBounds = imageFirst
                ? Rectangle.FromLTRB(contentBounds.Left, imageSlot.Bottom + gap, contentBounds.Right, contentBounds.Bottom)
                : Rectangle.FromLTRB(contentBounds.Left, contentBounds.Top, contentBounds.Right, imageSlot.Top - gap);

            return (
                textBounds,
                AlignInRectangle(imageSlot, imageSize with { Height = imageHeight }, imageAlign));
        }
    }

    private static Rectangle AlignInRectangle(
        Rectangle container,
        Size size,
        ContentAlignment alignment)
    {
        int x = alignment switch
        {
            ContentAlignment.TopLeft or ContentAlignment.MiddleLeft or ContentAlignment.BottomLeft => container.Left,
            ContentAlignment.TopRight or ContentAlignment.MiddleRight or ContentAlignment.BottomRight
                => container.Right - size.Width,
            _ => container.Left + ((container.Width - size.Width) / 2)
        };

        int y = alignment switch
        {
            ContentAlignment.TopLeft or ContentAlignment.TopCenter or ContentAlignment.TopRight => container.Top,
            ContentAlignment.BottomLeft or ContentAlignment.BottomCenter or ContentAlignment.BottomRight
                => container.Bottom - size.Height,
            _ => container.Top + ((container.Height - size.Height) / 2)
        };

        return new Rectangle(x, y, size.Width, size.Height);
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
        internal const float SideClearanceDip = 1f;
        internal const float PressTravelDip = 1.5f;

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

            int sideClearance = Math.Max(1, (int)MathF.Round(SideClearanceDip * scale));
            int pressTravel = Math.Max(1, (int)MathF.Round(PressTravelDip * scale));
            int ambient = sideClearance + pressTravel;
            int maxBorder = Math.Max(0, (Math.Min(bounds.Width, bounds.Height) / 4) - 1);
            int defaultBorderIncrease = context.IsDefault && context.Enabled
                ? Math.Max(1, (int)MathF.Round(scale))
                : 0;
            int borderWidth = Math.Clamp(context.BorderWidth + defaultBorderIncrease, 0, maxBorder);

            Rectangle keyRect = new(
                bounds.X + sideClearance,
                bounds.Y + sideClearance,
                Math.Max(1, bounds.Width - (2 * sideClearance)),
                Math.Max(1, bounds.Height - sideClearance - ambient));

            // Pressing translates the complete key top into the space released by its shortening shadow.
            // Keeping the key height constant avoids moving the bowl, border, and content independently.
            int pressOffset = Math.Min(
                (int)MathF.Round(press * PressTravelDip * scale),
                Math.Max(0, bounds.Bottom - sideClearance - keyRect.Bottom));
            keyRect.Offset(0, pressOffset);

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
        public Color DarkestTextBackground { get; init; }
        public Color LightestTextBackground { get; init; }
        public Color LipLight { get; init; }
        public Color LipDark { get; init; }
        public Color Border { get; init; }
        public Color Text { get; init; }
        public Color TextOutline { get; init; }
        public Color TextHighlight { get; init; }
        public Color TextShadow { get; init; }
        public Color Focus { get; init; }
        public int AmbientAlpha { get; init; }
        public int InnerShadowAlpha { get; init; }
        public int InnerLightAlpha { get; init; }

        public static Palette Create(PopupButtonRenderContext context, Metrics metrics, Color surfaceBackColor)
        {
            bool enabled = context.Enabled;

            // Disabled keys mute the material but keep the concave form readable - reduced contrast rather than
            // flat gray.
            Color back = enabled ? surfaceBackColor : PopupButtonColorMath.Mute(surfaceBackColor, 0.55f);
            float contrast = enabled ? 1f : 0.35f;
            float luminance = PopupButtonColorMath.GetLuminance(back);

            Color border = enabled
                ? context.BorderColor
                : PopupButtonColorMath.Blend(context.BorderColor, back, 0.45f);

            // Keep the border visible even if the user picked one too close to the face color.
            border = PopupButtonColorMath.EnsureContrast(border, back, 0.08f);

            float reliefStrength = (enabled ? 1f : 0.4f) + (metrics.Press * 0.25f);
            Color bodyLight = PopupButtonColorMath.Lighten(back, metrics.HighlightAmount * 0.9f * contrast);
            Color bodyDark = PopupButtonColorMath.Darken(back, metrics.ShadowAmount * 0.9f * contrast);
            Color bowlCenter = PopupButtonColorMath.Darken(back, (metrics.ShadowAmount * 0.75f * contrast) + (enabled ? 0f : 0.02f));
            Color bowlEdge = PopupButtonColorMath.Lighten(back, metrics.HighlightAmount * 0.55f * contrast);
            int innerShadowAlpha = Math.Clamp(
                (int)((30f + (metrics.ShadowAmount * 380f)) * contrast), 0, 120);
            int innerLightAlpha = Math.Clamp(
                (int)((20f + (metrics.HighlightAmount * 300f)) * contrast), 0, 100);
            Color darkestBowl = Color.White;
            Color lightestBowl = Color.Black;
            float darkestLuminance = 1f;
            float lightestLuminance = 0f;
            EvaluateBowlExtremes(bodyLight, bowlCenter);
            EvaluateBowlExtremes(bodyLight, bowlEdge);
            EvaluateBowlExtremes(bodyDark, bowlCenter);
            EvaluateBowlExtremes(bodyDark, bowlEdge);
            Color text = enabled
                ? context.UseAutomaticForeColor
                    ? PopupButtonColorMath.GetReadableForeColor(darkestBowl, lightestBowl)
                    : context.ForeColor
                : ModernControlColorMath.GetDisabledTextColor(
                    context.ForeColor,
                    darkestBowl,
                    lightestBowl);
            float textContrast = Math.Min(
                PopupButtonColorMath.GetContrastRatio(text, darkestBowl),
                PopupButtonColorMath.GetContrastRatio(text, lightestBowl));
            Color textOutline = enabled
                && context.UseAutomaticForeColor
                && textContrast < PopupButtonColorMath.MinimumReadableContrastRatio
                    ? text == Color.Black ? Color.White : Color.Black
                    : Color.Empty;

            return new Palette
            {
                BodyLight = bodyLight,
                BodyDark = bodyDark,
                BowlEdge = bowlEdge,
                BowlCenter = bowlCenter,
                DarkestTextBackground = darkestBowl,
                LightestTextBackground = lightestBowl,
                LipLight = PopupButtonColorMath.Lighten(back, metrics.HighlightAmount * 1.3f * contrast),
                LipDark = PopupButtonColorMath.Darken(back, metrics.ShadowAmount * 1.2f * contrast),
                Border = border,
                Text = text,
                TextOutline = textOutline,
                TextHighlight = PopupButtonColorMath.GetTextHighlight(bowlCenter, reliefStrength),
                TextShadow = PopupButtonColorMath.GetTextShadow(bowlCenter, reliefStrength),
                Focus = PopupButtonColorMath.EnsureContrast(
                    PopupButtonColorMath.TowardsContrast(back, 0.55f),
                    back,
                    0.45f),
                AmbientAlpha = (int)((luminance > 0.5f ? 55f : 85f)
                    * (1f - (metrics.Press * 0.6f))
                    * (enabled ? 1f : 0.5f)),
                InnerShadowAlpha = innerShadowAlpha,
                InnerLightAlpha = innerLightAlpha
            };

            void EvaluateBowlExtremes(Color bodyColor, Color bowlColor)
            {
                Color renderedBody = PopupButtonColorMath.Composite(bodyColor, context.SurfaceColor);
                Color renderedBowl = PopupButtonColorMath.Composite(bowlColor, renderedBody);
                EvaluateLuminance(renderedBowl);
                EvaluateLuminance(
                    PopupButtonColorMath.Composite(
                        Color.FromArgb(innerShadowAlpha, Color.Black),
                        renderedBowl));
                EvaluateLuminance(
                    PopupButtonColorMath.Composite(
                        Color.FromArgb(innerLightAlpha, Color.White),
                        renderedBowl));
            }

            void EvaluateLuminance(Color color)
            {
                float luminance = PopupButtonColorMath.GetRelativeLuminance(color);
                if (luminance < darkestLuminance)
                {
                    darkestBowl = color;
                    darkestLuminance = luminance;
                }

                if (luminance > lightestLuminance)
                {
                    lightestBowl = color;
                    lightestLuminance = luminance;
                }
            }
        }
    }
}
