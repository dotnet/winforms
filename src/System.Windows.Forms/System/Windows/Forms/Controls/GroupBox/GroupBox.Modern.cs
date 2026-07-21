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
        int horizontalInset = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxContentHorizontalInset);
        int topInset = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxContentTopInset);
        int bottomInset = FlatStyle == FlatStyle.Standard
            ? ScaleModernMetric(
                ModernControlVisualStyles.GroupBoxContentBottomInset)
            : topInset;
        int captionHeight = ModernCaptionFont.Height;
        int top = FlatStyle switch
        {
            FlatStyle.Standard => captionHeight
                + ScaleModernMetric(ModernControlVisualStyles.GroupBoxCaptionGap)
                + topInset,
            FlatStyle.Flat => captionHeight + topInset,
            FlatStyle.Popup => captionHeight
                + (2 * ScaleModernMetric(ModernControlVisualStyles.GroupBoxHeaderVerticalPadding))
                + topInset,
            _ => captionHeight + topInset
        };

        return new Padding(
            left: Padding.Left + horizontalInset,
            top: Padding.Top + top,
            right: Padding.Right + horizontalInset,
            bottom: Padding.Bottom + bottomInset);
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
        DrawModernCaption(
            e.Graphics,
            captionBounds,
            GetCaptionColor(effectiveBackColor));
    }

    private void DrawModernOutline(PaintEventArgs e, Rectangle bounds)
    {
        int captionHeight = ModernCaptionFont.Height;
        int inset = ScaleModernMetric(
            ModernControlVisualStyles.GroupBoxContentHorizontalInset);
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
        Color borderColor = Application.SystemVisualSettings.AccentColor;
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
                GetModernBorderThickness() + 2,
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

        using (GraphicsStateScope state = new(e.Graphics))
        {
            e.Graphics.SetClip(headerBounds);
            using var headerBrush = headerColor.GetCachedSolidBrushScope();
            e.Graphics.FillPath(headerBrush, path);
        }

        using var borderPen = borderColor.GetCachedPenScope(
            GetModernBorderThickness());
        e.Graphics.DrawPath(borderPen, path);

        Color captionColor = Enabled
            ? PopupButtonColorMath.GetReadableForeColor(headerColor)
            : ModernControlColorMath.GetDisabledTextColor(
                PopupButtonColorMath.GetReadableForeColor(headerColor),
                headerColor);
        DrawModernCaption(e.Graphics, captionBounds, captionColor);
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

    private Rectangle GetStandardCaptionBounds(
        Rectangle bounds,
        int captionHeight)
        => new(
            bounds.Left + Padding.Left,
            bounds.Top + Padding.Top,
            Math.Max(0, bounds.Width - Padding.Horizontal),
            captionHeight);

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
