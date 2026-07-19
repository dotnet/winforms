// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Rendering.Button;

namespace System.Windows.Forms;

public partial class GroupBox
{
    private Font? _modernCaptionFont;
    private Font? _modernCaptionSourceFont;
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
                || _modernCaptionTextScale != textScale
                || _modernCaptionDpi != DeviceDpiInternal)
            {
                InvalidateModernCaptionFont();

                FontStyle desiredStyle = Font.Style | FontStyle.Bold;
                FontStyle style = Font.FontFamily.IsStyleAvailable(desiredStyle)
                    ? desiredStyle
                    : Font.Style;
                _modernCaptionFont = new Font(
                    Font.FontFamily,
                    Font.Size * ModernControlVisualStyles.GroupBoxCaptionFontScale * textScale,
                    style,
                    Font.Unit,
                    Font.GdiCharSet,
                    Font.GdiVerticalFont);
                _modernCaptionSourceFont = Font;
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
        int contentInset = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxContentInset);
        int captionHeight = ModernCaptionFont.Height;
        int top = FlatStyle switch
        {
            FlatStyle.Standard => captionHeight
                + ScaleModernMetric(ModernControlVisualStyles.GroupBoxCaptionGap)
                + contentInset,
            FlatStyle.Flat => captionHeight + contentInset,
            FlatStyle.Popup => captionHeight
                + (2 * ScaleModernMetric(ModernControlVisualStyles.GroupBoxHeaderVerticalPadding))
                + contentInset,
            _ => captionHeight + contentInset
        };

        return new Padding(
            left: Padding.Left + contentInset,
            top: Padding.Top + top,
            right: Padding.Right + contentInset,
            bottom: Padding.Bottom + contentInset);
    }

    private void DrawModernGroupBox(PaintEventArgs e)
    {
        Rectangle bounds = ClientRectangle;
        if (bounds.Width <= 1 || bounds.Height <= 1)
        {
            return;
        }

        bounds.Width--;
        bounds.Height--;

        using GraphicsStateScope state = new(e.Graphics);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        switch (FlatStyle)
        {
            case FlatStyle.Standard:
                DrawModernCard(e, bounds);
                break;
            case FlatStyle.Flat:
                DrawModernOutline(e, bounds);
                break;
            case FlatStyle.Popup:
                DrawModernPopup(e, bounds);
                break;
        }
    }

    private void DrawModernCard(PaintEventArgs e, Rectangle bounds)
    {
        int captionHeight = ModernCaptionFont.Height;
        int captionGap = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxCaptionGap);
        int inset = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxContentInset);
        Rectangle frameBounds = new(
            bounds.Left,
            bounds.Top + captionHeight + captionGap,
            bounds.Width,
            Math.Max(0, bounds.Height - captionHeight - captionGap));
        Rectangle captionBounds = new(
            bounds.Left + inset,
            bounds.Top,
            Math.Max(0, bounds.Width - (2 * inset)),
            captionHeight);

        Color effectiveBackColor = DisabledColor;
        Color surfaceColor = PopupButtonColorMath.TowardsContrast(
            effectiveBackColor,
            0.035f);
        Color borderColor = PopupButtonColorMath.TowardsContrast(
            effectiveBackColor,
            0.16f);
        if (!Enabled)
        {
            surfaceColor = PopupButtonColorMath.Mute(surfaceColor, 0.55f);
            borderColor = PopupButtonColorMath.Mute(borderColor, 0.55f);
        }

        FillAndStrokeRoundedFrame(
            e.Graphics,
            frameBounds,
            surfaceColor,
            borderColor);
        DrawModernCaption(
            e.Graphics,
            captionBounds,
            GetCaptionColor(effectiveBackColor));
    }

    private void DrawModernOutline(PaintEventArgs e, Rectangle bounds)
    {
        int captionHeight = ModernCaptionFont.Height;
        int inset = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxContentInset);
        Rectangle frameBounds = new(
            bounds.Left,
            bounds.Top + (captionHeight / 2),
            bounds.Width,
            Math.Max(0, bounds.Height - (captionHeight / 2)));
        Rectangle captionBounds = new(
            bounds.Left + inset,
            bounds.Top,
            Math.Max(0, bounds.Width - (2 * inset)),
            captionHeight);
        Color effectiveBackColor = DisabledColor;
        Color borderColor = PopupButtonColorMath.TowardsContrast(
            effectiveBackColor,
            0.22f);
        if (!Enabled)
        {
            borderColor = PopupButtonColorMath.Mute(borderColor, 0.55f);
        }

        DrawRoundedFrame(e.Graphics, frameBounds, borderColor);

        Rectangle captionBackground = GetCaptionTextBounds(
            e.Graphics,
            captionBounds);
        if (!captionBackground.IsEmpty)
        {
            captionBackground.Inflate(
                ScaleModernMetric(ModernControlVisualStyles.BorderThickness) + 2,
                0);
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
        Rectangle captionBounds = new(
            bounds.Left + horizontalPadding,
            bounds.Top + verticalPadding,
            Math.Max(0, bounds.Width - (2 * horizontalPadding)),
            ModernCaptionFont.Height);

        Color effectiveBackColor = DisabledColor;
        Color bodyColor = BackColor.A == 0
            ? Color.Transparent
            : BackColor;
        Color headerColor = PopupButtonColorMath.Blend(
            effectiveBackColor,
            Application.SystemVisualSettings.AccentColor,
            Application.IsDarkModeEnabled ? 0.4f : 0.28f);
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

        using (GraphicsStateScope state = new(e.Graphics))
        {
            e.Graphics.SetClip(headerBounds);
            using var headerBrush = headerColor.GetCachedSolidBrushScope();
            e.Graphics.FillPath(headerBrush, path);
        }

        using var borderPen = borderColor.GetCachedPenScope(
            ScaleModernMetric(ModernControlVisualStyles.BorderThickness));
        e.Graphics.DrawPath(borderPen, path);

        Color captionColor = Enabled
            ? PopupButtonColorMath.GetReadableForeColor(headerColor)
            : ModernControlColorMath.GetDisabledTextColor(
                PopupButtonColorMath.GetReadableForeColor(headerColor),
                headerColor);
        DrawModernCaption(e.Graphics, captionBounds, captionColor);
    }

    private void FillAndStrokeRoundedFrame(
        Graphics graphics,
        Rectangle bounds,
        Color fillColor,
        Color borderColor)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        using GraphicsPath path = CreateModernFramePath(bounds);
        using var brush = fillColor.GetCachedSolidBrushScope();
        graphics.FillPath(brush, path);
        using var pen = borderColor.GetCachedPenScope(
            ScaleModernMetric(ModernControlVisualStyles.BorderThickness));
        graphics.DrawPath(pen, path);
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
            ScaleModernMetric(ModernControlVisualStyles.BorderThickness));
        graphics.DrawPath(pen, path);
    }

    private GraphicsPath CreateModernFramePath(Rectangle bounds)
    {
        GraphicsPath path = new();
        int radius = Math.Clamp(
            ScaleModernMetric(ModernControlVisualStyles.GroupBoxCornerRadius),
            1,
            Math.Max(1, Math.Min(bounds.Width, bounds.Height)));
        path.AddRoundedRectangle(bounds, new Size(radius, radius));

        return path;
    }

    private Color GetCaptionColor(Color backgroundColor)
        => Enabled
            ? ForeColor
            : ModernControlColorMath.GetDisabledTextColor(
                ForeColor,
                backgroundColor);

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
                graphics.MeasureString(
                    Text,
                    ModernCaptionFont,
                    availableBounds.Width))
            : TextRenderer.MeasureText(
                graphics,
                Text,
                ModernCaptionFont,
                availableBounds.Size,
                TextFormatFlags.SingleLine
                    | TextFormatFlags.NoPadding);
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
            using StringFormat format = new()
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

            graphics.DrawString(Text, ModernCaptionFont, brush, bounds, format);
            return;
        }

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

        TextRenderer.DrawText(
            graphics,
            Text,
            ModernCaptionFont,
            bounds,
            color,
            flags);
    }

    private int ScaleModernMetric(int value)
        => ScaleHelper.ScaleToDpi(value, DeviceDpiInternal);

    private void InvalidateModernCaptionFont()
    {
        _modernCaptionFont?.Dispose();
        _modernCaptionFont = null;
        _modernCaptionSourceFont = null;
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
                | SystemVisualSettingsCategories.AccentColor)) != 0)
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
