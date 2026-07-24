// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Rendering.Button;

namespace System.Windows.Forms;

public partial class GroupBox
{
    private const string MissingSemiBoldFamily = "";

    private static readonly ConcurrentDictionary<string, string> s_semiBoldFamilyNames =
        new(StringComparer.OrdinalIgnoreCase);

    private Font? _modernCaptionFont;
    private Font? _modernCaptionSourceFont;
    private FlatStyle _modernCaptionFlatStyle;
    private float _modernCaptionTextScale;
    private int _modernCaptionDpi;

    private bool UsesModernRenderer
        => OwnerDraw
            && EffectiveVisualStylesMode >= VisualStylesMode.Net11;

    private Font ModernCaptionFont
    {
        get
        {
            float textScale = Math.Clamp(
                Application.SystemVisualSettings.TextScaleFactor,
                1f,
                2.25f);

            if (_modernCaptionFont is null
                || !ReferenceEquals(_modernCaptionSourceFont, Font)
                || _modernCaptionFlatStyle != FlatStyle
                || _modernCaptionTextScale != textScale
                || _modernCaptionDpi != DeviceDpiInternal)
            {
                InvalidateModernCaptionFont();

                _modernCaptionFont = CreateModernCaptionFont(textScale);
                _modernCaptionSourceFont = Font;
                _modernCaptionFlatStyle = FlatStyle;
                _modernCaptionTextScale = textScale;
                _modernCaptionDpi = DeviceDpiInternal;
            }

            return _modernCaptionFont;
        }
    }

    private Rectangle ModernDisplayRectangle
    {
        get
        {
            Padding decoration = GetModernDecorationPadding();
            Size size = ClientSize;

            return new Rectangle(
                decoration.Left,
                decoration.Top,
                Math.Max(size.Width - decoration.Horizontal, 0),
                Math.Max(size.Height - decoration.Vertical, 0));
        }
    }

    private Padding GetModernDecorationPadding()
    {
        int captionHeight = ModernCaptionFont.Height;

        switch (FlatStyle)
        {
            case FlatStyle.Standard:
            {
                // Card: reserve the caption band plus its visual gap to the card. No internal
                // horizontal or bottom inset, so a docked child with Padding = 0 fills the card.
                int gap = ScaleModernMetric(ModernControlVisualStyles.GroupBoxCaptionGap);

                return new Padding(
                    left: Padding.Left,
                    top: Padding.Top + captionHeight + gap,
                    right: Padding.Right,
                    bottom: Padding.Bottom);
            }

            case FlatStyle.Flat:
            {
                // Outline: the top border line runs along the caption baseline, so content clears
                // the descenders that hang below it (descent + 1px leeway). Left/right/bottom sit
                // just inside the border.
                (int ascent, int descent) = GetModernCaptionMetrics();
                int leeway = ScaleModernMetric(
                    ModernControlVisualStyles.GroupBoxFlatBaselineLeeway);
                int borderInset = GetModernBorderThickness() + leeway;

                return new Padding(
                    left: Padding.Left + borderInset,
                    top: Padding.Top + ascent + descent + leeway,
                    right: Padding.Right + borderInset,
                    bottom: Padding.Bottom + borderInset);
            }

            case FlatStyle.Popup:
            {
                // Accent header: content is flush to the bottom of the filled header rectangle
                // (0 top gap) with a 2px inset on the remaining sides.
                int verticalPadding = ScaleModernMetric(
                    ModernControlVisualStyles.GroupBoxHeaderVerticalPadding);
                int headerHeight = captionHeight + (2 * verticalPadding);
                int inset = ScaleModernMetric(
                    ModernControlVisualStyles.GroupBoxPopupContentInset);

                return new Padding(
                    left: Padding.Left + inset,
                    top: Padding.Top + headerHeight,
                    right: Padding.Right + inset,
                    bottom: Padding.Bottom + inset);
            }

            default:
            {
                int gap = ScaleModernMetric(ModernControlVisualStyles.GroupBoxCaptionGap);

                return new Padding(
                    left: Padding.Left,
                    top: Padding.Top + captionHeight + gap,
                    right: Padding.Right,
                    bottom: Padding.Bottom);
            }
        }
    }

    /// <summary>
    ///  Returns the modern caption font's ascent and descent in device pixels.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The values are derived from the font family's design metrics (cell ascent/descent scaled
    ///   against the line spacing) applied to the caption font's device-pixel line height, so they
    ///   track the current DPI and text-scale factor.
    ///  </para>
    /// </remarks>
    private (int Ascent, int Descent) GetModernCaptionMetrics()
    {
        Font font = ModernCaptionFont;
        FontFamily family = font.FontFamily;
        FontStyle style = font.Style;

        int lineSpacingDesignUnits = family.GetLineSpacing(style);
        if (lineSpacingDesignUnits <= 0)
        {
            return (font.Height, 0);
        }

        float lineHeightPixels = font.GetHeight(DeviceDpiInternal);
        int ascent = (int)Math.Ceiling(
            lineHeightPixels * family.GetCellAscent(style) / lineSpacingDesignUnits);
        int descent = (int)Math.Ceiling(
            lineHeightPixels * family.GetCellDescent(style) / lineSpacingDesignUnits);

        return (ascent, descent);
    }

    private void DrawModernGroupBox(PaintEventArgs e)
    {
        Rectangle clientBounds = ClientRectangle;
        if (clientBounds.Width <= 1 || clientBounds.Height <= 1)
        {
            return;
        }

        Rectangle strokeBounds = clientBounds;
        strokeBounds.Width--;
        strokeBounds.Height--;

        using GraphicsStateScope state = new(e.Graphics);
        if (FlatStyle != FlatStyle.Standard)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        switch (FlatStyle)
        {
            case FlatStyle.Standard:
                DrawModernCard(e, clientBounds);
                break;
            case FlatStyle.Flat:
                DrawModernOutline(e, strokeBounds);
                break;
            case FlatStyle.Popup:
                DrawModernPopup(e, strokeBounds);
                break;
        }
    }

    private void DrawModernCard(PaintEventArgs e, Rectangle bounds)
    {
        int captionHeight = ModernCaptionFont.Height;
        int captionGap = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxCaptionGap);
        Rectangle frameBounds = new(
            bounds.Left,
            bounds.Top + captionHeight + captionGap,
            bounds.Width,
            Math.Max(0, bounds.Height - captionHeight - captionGap));
        Rectangle captionBounds = GetStandardCaptionBounds(
            bounds,
            captionHeight);

        Color effectiveBackColor = DisabledColor;
        Color surfaceColor = PopupButtonColorMath.TowardsContrast(
            effectiveBackColor,
            0.035f);
        if (!Enabled)
        {
            surfaceColor = PopupButtonColorMath.Mute(surfaceColor, 0.55f);
        }

        using var surfaceBrush = surfaceColor.GetCachedSolidBrushScope();
        e.Graphics.FillRectangle(surfaceBrush, frameBounds);

        // The modern card fills an opaque surface over the whole body, which would hide a
        // BackgroundImage painted by OnPaintBackground. Composite the image back over the surface
        // so it remains visible (issue #14779).
        PaintModernBackgroundImage(e, frameBounds);

        DrawModernCaption(
            e.Graphics,
            captionBounds,
            GetCaptionColor(effectiveBackColor));
    }

    /// <summary>
    ///  Composites the control's <see cref="Control.BackgroundImage"/> over an already-painted
    ///  modern surface, clipped to <paramref name="clipBounds"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The modern GroupBox renderers fill an opaque body surface that would otherwise hide the
    ///   BackgroundImage that <see cref="Control.OnPaintBackground"/> painted. Using a transparent
    ///   back color keeps the surface visible around a non-tiling image layout while restoring the
    ///   image itself.
    ///  </para>
    /// </remarks>
    private void PaintModernBackgroundImage(PaintEventArgs e, Rectangle clipBounds)
    {
        if (BackgroundImage is null || clipBounds.Width <= 0 || clipBounds.Height <= 0)
        {
            return;
        }

        ControlPaint.DrawBackgroundImage(
            e.Graphics,
            BackgroundImage,
            Color.Transparent,
            BackgroundImageLayout,
            ClientRectangle,
            clipBounds,
            DisplayRectangle.Location,
            RightToLeft);
    }

    private void DrawModernOutline(PaintEventArgs e, Rectangle bounds)
    {
        int captionHeight = ModernCaptionFont.Height;
        (int ascent, _) = GetModernCaptionMetrics();
        int cornerRadius = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxCornerRadius);

        // The top border line runs along the caption text baseline.
        int baseline = bounds.Top + ascent;
        Rectangle frameBounds = new(
            bounds.Left,
            baseline,
            bounds.Width,
            Math.Max(0, bounds.Bottom - baseline));

        Rectangle captionBounds = GetFlatCaptionBounds(
            bounds,
            captionHeight,
            cornerRadius);

        Color effectiveBackColor = DisabledColor;
        Color borderColor = Application.SystemVisualSettings.AccentColor;
        if (!Enabled)
        {
            borderColor = PopupButtonColorMath.Mute(borderColor, 0.55f);
        }

        DrawRoundedFrame(e.Graphics, frameBounds, borderColor);

        Rectangle captionBackground = GetFlatCaptionBackgroundBounds(
            e.Graphics,
            captionBounds,
            frameBounds);
        if (!captionBackground.IsEmpty)
        {
            PaintBackground(e, captionBackground);
        }

        DrawModernCaption(
            e.Graphics,
            captionBounds,
            GetCaptionColor(effectiveBackColor));
    }

    private void DrawModernPopup(PaintEventArgs e, Rectangle bounds)
    {
        int verticalPadding = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxHeaderVerticalPadding);
        int horizontalPadding = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxHeaderHorizontalPadding);
        int headerHeight = ModernCaptionFont.Height + (2 * verticalPadding);
        Rectangle headerBounds = new(
            bounds.Left,
            bounds.Top,
            bounds.Width,
            Math.Min(bounds.Height, headerHeight));

        Rectangle captionBounds = GetPopupCaptionBounds(
            bounds,
            ModernCaptionFont.Height,
            horizontalPadding,
            verticalPadding);

        Color bodyColor = BackColor.A == 0
            ? Color.Transparent
            : BackColor;
        Color headerColor = Application.SystemVisualSettings.AccentColor;
        Color borderColor = PopupButtonColorMath.TowardsContrast(
            headerColor,
            0.2f);
        if (!Enabled)
        {
            bodyColor = PopupButtonColorMath.Mute(bodyColor, 0.55f);
            headerColor = PopupButtonColorMath.Mute(headerColor, 0.55f);
            borderColor = PopupButtonColorMath.Mute(borderColor, 0.55f);
        }

        using GraphicsPath path = CreateModernFramePath(bounds);
        using (var bodyBrush = bodyColor.GetCachedSolidBrushScope())
        {
            e.Graphics.FillPath(bodyBrush, path);
        }

        // Composite the BackgroundImage over the opaque body fill, clipped to the rounded frame,
        // so it stays visible under the modern Popup surface (issue #14779).
        if (BackgroundImage is not null)
        {
            using GraphicsStateScope imageState = new(e.Graphics);
            e.Graphics.SetClip(path);
            PaintModernBackgroundImage(e, bounds);
        }

        using (GraphicsStateScope state = new(e.Graphics))
        {
            e.Graphics.SetClip(headerBounds);
            using var headerBrush = headerColor.GetCachedSolidBrushScope();
            e.Graphics.FillPath(headerBrush, path);
        }

        using var borderPen = borderColor.GetCachedPenScope(
            GetModernBorderThickness());
        e.Graphics.DrawPath(borderPen, path);

        // The rounded frame is clipped with a non-antialiased region; blend the resulting corner
        // artifacts into the parent by tracing the parent color just outside the border.
        int frameRadius = GetModernFrameCornerRadius(bounds);
        ParentBackgroundRenderer.PaintRoundedBorderRegionMitigation(
            e.Graphics,
            bounds,
            new Size(frameRadius, frameRadius),
            GetModernBorderThickness(),
            ParentInternal?.BackColor ?? BackColor);

        Color captionColor = Enabled
            ? PopupButtonColorMath.GetReadableForeColor(headerColor)
            : ModernControlColorMath.GetDisabledTextColor(
                PopupButtonColorMath.GetReadableForeColor(headerColor),
                headerColor);
        DrawModernCaption(e.Graphics, captionBounds, captionColor);
    }

    private Rectangle GetFlatCaptionBounds(
        Rectangle bounds,
        int captionHeight,
        int cornerRadius)
    {
        // The caption is indented by the corner radius so it clears the rounded corner. Padding
        // does not move the Flat caption. In RTL the indent is applied on the right.
        bool rightToLeft = RightToLeft == RightToLeft.Yes;
        int captionX = rightToLeft
            ? bounds.Left
            : bounds.Left + cornerRadius;
        int captionOffset = ScaleModernMetric(
            ModernControlVisualStyles.BorderThickness);

        return new Rectangle(
            captionX,
            bounds.Top + captionOffset,
            Math.Max(0, bounds.Width - cornerRadius),
            captionHeight);
    }

    private Rectangle GetFlatCaptionBackgroundBounds(
        Graphics graphics,
        Rectangle captionBounds,
        Rectangle frameBounds)
    {
        Rectangle textBounds = GetCaptionTextBounds(
            graphics,
            captionBounds);
        if (textBounds.IsEmpty)
        {
            return Rectangle.Empty;
        }

        int gap = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxCaptionGap);
        int left = Math.Max(
            frameBounds.Left,
            textBounds.Left - gap);
        int right = Math.Min(
            frameBounds.Right,
            textBounds.Right + gap);

        return right <= left
            ? Rectangle.Empty
            : new Rectangle(
                left,
                captionBounds.Top,
                right - left,
                captionBounds.Height);
    }

    private Rectangle GetPopupCaptionBounds(
        Rectangle bounds,
        int captionHeight,
        int horizontalPadding,
        int verticalPadding)
    {
        Rectangle captionBounds = GetStandardCaptionBounds(
            bounds,
            captionHeight);

        return new Rectangle(
            captionBounds.Left + horizontalPadding,
            captionBounds.Top + verticalPadding,
            Math.Max(
                0,
                captionBounds.Width - (2 * horizontalPadding)),
            captionHeight);
    }

    private void DrawRoundedFrame(
        Graphics graphics,
        Rectangle bounds,
        Color borderColor)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        using GraphicsPath path = CreateModernFramePath(bounds);
        using var pen = borderColor.GetCachedPenScope(
            GetModernBorderThickness());
        graphics.DrawPath(pen, path);

        // The rounded frame is clipped with a non-antialiased region; blend the resulting corner
        // artifacts into the parent by tracing the parent color just outside the border.
        int frameRadius = GetModernFrameCornerRadius(bounds);
        ParentBackgroundRenderer.PaintRoundedBorderRegionMitigation(
            graphics,
            bounds,
            new Size(frameRadius, frameRadius),
            GetModernBorderThickness(),
            ParentInternal?.BackColor ?? BackColor);
    }

    private GraphicsPath CreateModernFramePath(Rectangle bounds)
    {
        GraphicsPath path = new();
        int radius = GetModernFrameCornerRadius(bounds);
        path.AddRoundedRectangle(bounds, new Size(radius, radius));

        return path;
    }

    private int GetModernFrameCornerRadius(Rectangle bounds)
        => Math.Clamp(
            ScaleModernMetric(ModernControlVisualStyles.GroupBoxCornerRadius),
            1,
            Math.Max(1, Math.Min(bounds.Width, bounds.Height)));

    private Color GetCaptionColor(Color backgroundColor)
        => Enabled
            ? ForeColor
            : ModernControlColorMath.GetDisabledTextColor(
                ForeColor,
                backgroundColor);

    private Rectangle GetStandardCaptionBounds(
        Rectangle bounds,
        int captionHeight)
    {
        // Padding only shifts the caption horizontally: right by Padding.Left in LTR, left by
        // Padding.Right in RTL. It never moves the caption vertically.
        bool rightToLeft = RightToLeft == RightToLeft.Yes;
        int leftOffset = rightToLeft ? 0 : Padding.Left;
        int rightOffset = rightToLeft ? Padding.Right : 0;

        return new Rectangle(
            bounds.Left + leftOffset,
            bounds.Top,
            Math.Max(0, bounds.Width - leftOffset - rightOffset),
            captionHeight);
    }

    private Font CreateModernCaptionFont(float textScale)
    {
        float styleScale = FlatStyle == FlatStyle.Flat
            ? 1f
            : ModernControlVisualStyles.GroupBoxCaptionFontScale;
        string semiBoldFamilyName = Font.Style == FontStyle.Regular
            ? s_semiBoldFamilyNames.GetOrAdd(
                Font.FontFamily.Name,
                FindSemiBoldFamilyName)
            : MissingSemiBoldFamily;

        if (semiBoldFamilyName.Length == 0)
        {
            return new Font(
                Font.FontFamily,
                Font.Size * styleScale * textScale,
                Font.Style,
                Font.Unit,
                Font.GdiCharSet,
                Font.GdiVerticalFont);
        }

        using FontFamily semiBoldFamily = new(semiBoldFamilyName);
        return new Font(
            semiBoldFamily,
            Font.Size * styleScale * textScale,
            FontStyle.Regular,
            Font.Unit,
            Font.GdiCharSet,
            Font.GdiVerticalFont);
    }

    internal static string FindSemiBoldFamilyName(string sourceFamilyName)
    {
        string baseFamilyName = sourceFamilyName.EndsWith(
            " Regular",
            StringComparison.OrdinalIgnoreCase)
            ? sourceFamilyName[..^" Regular".Length]
            : sourceFamilyName;
        string expectedName = IsSemiBoldFamilyName(sourceFamilyName)
            ? sourceFamilyName
            : $"{baseFamilyName} Semibold";

        using InstalledFontCollection installedFonts = new();
        foreach (FontFamily family in installedFonts.Families)
        {
            if (family.Name.Equals(
                expectedName,
                StringComparison.OrdinalIgnoreCase))
            {
                return family.Name;
            }
        }

        return MissingSemiBoldFamily;
    }

    internal static bool IsSemiBoldFamilyName(string familyName)
        => familyName.Contains(
            "Semibold",
            StringComparison.OrdinalIgnoreCase)
            || familyName.Contains(
                "Semi Bold",
                StringComparison.OrdinalIgnoreCase)
            || familyName.Contains(
                "Demibold",
                StringComparison.OrdinalIgnoreCase)
            || familyName.Contains(
                "Demi Bold",
                StringComparison.OrdinalIgnoreCase);

    private int GetModernBorderThickness()
    {
        SystemVisualSettings settings = Application.SystemVisualSettings;
        Size borderMetrics = ModernControlVisualStyles.GetFocusBorderMetrics(
            settings.FocusBorderMetrics,
            settings.TextScaleFactor,
            DeviceDpiInternal);

        return Math.Max(borderMetrics.Width, borderMetrics.Height);
    }

    private Rectangle GetCaptionTextBounds(
        Graphics graphics,
        Rectangle availableBounds)
    {
        if (string.IsNullOrEmpty(Text)
            || availableBounds.Width <= 0
            || availableBounds.Height <= 0)
        {
            return Rectangle.Empty;
        }

        Size measuredSize = UseCompatibleTextRendering
            ? Size.Ceiling(
                MeasureModernCaption(
                    graphics,
                    availableBounds.Size))
            : TextRenderer.MeasureText(
                graphics,
                Text,
                ModernCaptionFont,
                availableBounds.Size,
                GetModernCaptionTextFormatFlags());
        int width = Math.Min(measuredSize.Width, availableBounds.Width);
        int x = RightToLeft == RightToLeft.Yes
            ? availableBounds.Right - width
            : availableBounds.Left;

        return new Rectangle(
            x,
            availableBounds.Top,
            width,
            availableBounds.Height);
    }

    private void DrawModernCaption(
        Graphics graphics,
        Rectangle bounds,
        Color color)
    {
        if (string.IsNullOrEmpty(Text) || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        if (UseCompatibleTextRendering)
        {
            using var brush = color.GetCachedSolidBrushScope();
            using StringFormat format = CreateModernCaptionStringFormat();

            graphics.DrawString(Text, ModernCaptionFont, brush, bounds, format);
            return;
        }

        TextRenderer.DrawText(
            graphics,
            Text,
            ModernCaptionFont,
            bounds,
            color,
            GetModernCaptionTextFormatFlags());
    }

    private StringFormat CreateModernCaptionStringFormat()
    {
        StringFormat format = new()
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center,
            HotkeyPrefix = ShowKeyboardCues
                ? HotkeyPrefix.Show
                : HotkeyPrefix.Hide,
            Trimming = StringTrimming.EllipsisCharacter
        };

        if (RightToLeft == RightToLeft.Yes)
        {
            format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
        }

        return format;
    }

    private TextFormatFlags GetModernCaptionTextFormatFlags()
    {
        TextFormatFlags flags = TextFormatFlags.SingleLine
            | TextFormatFlags.VerticalCenter
            | TextFormatFlags.EndEllipsis
            | TextFormatFlags.PreserveGraphicsClipping
            | TextFormatFlags.PreserveGraphicsTranslateTransform;
        if (!ShowKeyboardCues)
        {
            flags |= TextFormatFlags.HidePrefix;
        }

        if (RightToLeft == RightToLeft.Yes)
        {
            flags |= TextFormatFlags.Right | TextFormatFlags.RightToLeft;
        }

        return flags;
    }

    private SizeF MeasureModernCaption(
        Graphics graphics,
        Size availableSize)
    {
        using StringFormat format = CreateModernCaptionStringFormat();

        return graphics.MeasureString(
            Text,
            ModernCaptionFont,
            availableSize,
            format);
    }

    private int ScaleModernMetric(int value)
        => ScaleHelper.ScaleToDpi(value, DeviceDpiInternal);

    private void InvalidateModernCaptionFont()
    {
        _modernCaptionFont?.Dispose();
        _modernCaptionFont = null;
        _modernCaptionSourceFont = null;
        _modernCaptionFlatStyle = default;
        _modernCaptionTextScale = 0f;
        _modernCaptionDpi = 0;
    }

    /// <inheritdoc/>
    protected override void OnSystemVisualSettingsChanged(
        SystemVisualSettingsChangedEventArgs e)
    {
        base.OnSystemVisualSettingsChanged(e);

        if (!UsesModernRenderer)
        {
            return;
        }

        if ((e.Changed & SystemVisualSettingsCategories.TextScale) != 0)
        {
            InvalidateModernCaptionFont();
            CommonProperties.xClearPreferredSizeCache(this);
            LayoutTransaction.DoLayout(
                this,
                this,
                PropertyNames.SystemVisualSettings);
            if (ParentInternal is { } parent)
            {
                LayoutTransaction.DoLayout(
                    parent,
                    this,
                    PropertyNames.SystemVisualSettings);
            }
        }

        if ((e.Changed
            & (SystemVisualSettingsCategories.TextScale
                | SystemVisualSettingsCategories.AccentColor
                | SystemVisualSettingsCategories.FocusMetrics)) != 0)
        {
            Invalidate();
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    ///  <para>
    ///   Net11 and later share GroupBox metrics. Crossing the classic or disabled boundary
    ///   changes the caption and content geometry, while modern-to-modern transitions repaint.
    ///  </para>
    /// </remarks>
    protected override VisualStylesModeChangeImpact GetVisualStylesModeChangeImpact(
        VisualStylesMode oldMode,
        VisualStylesMode newMode)
    {
        if (FlatStyle == FlatStyle.System)
        {
            return VisualStylesModeChangeImpact.None;
        }

        bool oldUsesModernMetrics = oldMode >= VisualStylesMode.Net11;
        bool newUsesModernMetrics = newMode >= VisualStylesMode.Net11;

        return oldUsesModernMetrics != newUsesModernMetrics
            ? VisualStylesModeChangeImpact.Metrics
            : VisualStylesModeChangeImpact.Repaint;
    }

    /// <inheritdoc/>
    protected override void RescaleConstantsForDpi(
        int deviceDpiOld,
        int deviceDpiNew)
    {
        InvalidateModernCaptionFont();
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            InvalidateModernCaptionFont();
        }

        base.Dispose(disposing);
    }
}
