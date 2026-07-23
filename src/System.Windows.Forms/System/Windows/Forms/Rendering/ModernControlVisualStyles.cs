// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Defines shared device-independent metrics for modern control chrome. All values are logical
///  (96-DPI) pixels and are scaled to the target device DPI at the point of use.
/// </summary>
internal static class ModernControlVisualStyles
{
    /// <summary>Stroke thickness of the modern rounded control border.</summary>
    internal const int BorderThickness = 1;

    /// <summary>Extra width added to the modern ComboBox drop-down button beyond the native metric.</summary>
    internal const int ComboBoxButtonExtraWidth = 4;

    /// <summary>Horizontal clearance so the flat native edit child's square corners clear the field's rounded arcs.</summary>
    internal const int ComboBoxFieldArcClearance = 2;

    /// <summary>Minimal modern ComboBox field chrome inset (excludes classic 3D-border metrics).</summary>
    internal const int ComboBoxStyleInset = 1;

    /// <summary>Corner radius of a modern text field's rounded frame.</summary>
    internal const int FieldCornerRadius = 15;

    /// <summary>Height of the animated focus underline band drawn beneath a focused modern field.</summary>
    internal const int FocusBandHeight = 4;

    /// <summary>Scale factor applied to the GroupBox caption font in modern mode.</summary>
    internal const float GroupBoxCaptionFontScale = 1.15f;

    /// <summary>Gap between the GroupBox caption text and the surrounding frame line.</summary>
    internal const int GroupBoxCaptionGap = 4;

    /// <summary>Inset from the GroupBox bottom frame to its content area.</summary>
    internal const int GroupBoxContentBottomInset = 4;

    /// <summary>Inset from the GroupBox left/right frame to its content area.</summary>
    internal const int GroupBoxContentHorizontalInset = 8;

    /// <summary>Inset from the GroupBox top frame (below the caption) to its content area.</summary>
    internal const int GroupBoxContentTopInset = 8;

    /// <summary>Corner radius of the GroupBox rounded frame.</summary>
    internal const int GroupBoxCornerRadius = 8;

    /// <summary>Extra baseline leeway added to the modern flat GroupBox border inset.</summary>
    internal const int GroupBoxFlatBaselineLeeway = 1;

    /// <summary>Horizontal padding around the GroupBox header text.</summary>
    internal const int GroupBoxHeaderHorizontalPadding = 10;

    /// <summary>Vertical padding around the GroupBox header text.</summary>
    internal const int GroupBoxHeaderVerticalPadding = 5;

    /// <summary>Extra content inset for the modern Popup-style GroupBox, applied on top of the header height.</summary>
    internal const int GroupBoxPopupContentInset = 2;

    /// <summary>
    ///  Inset between a control's border and its content, shared by modern text fields and the up-down
    ///  control. Added on top of the border-padding component (see <see cref="GetFieldPadding"/>).
    /// </summary>
    internal const int InternalChromeInset = 2;

    /// <summary>Border-padding component for a <see cref="BorderStyle.Fixed3D"/> border.</summary>
    internal const int Fixed3DBorderPadding = 2;

    /// <summary>Border-padding component for a <see cref="BorderStyle.FixedSingle"/> border.</summary>
    internal const int FixedSingleBorderPadding = 1;

    /// <summary>Border-padding component for a control with <see cref="BorderStyle.None"/>.</summary>
    internal const int NoBorderPadding = 1;

    /// <summary>Corner radius of the up-down control's rounded frame.</summary>
    internal const int UpDownCornerRadius = 14;

    internal static Padding GetFieldPadding(
        BorderStyle borderStyle,
        Padding userPadding,
        Size focusBorderMetrics,
        float textScaleFactor,
        int deviceDpi)
    {
        Size scaledFocusBorderMetrics = GetFocusBorderMetrics(
            focusBorderMetrics,
            textScaleFactor,
            deviceDpi);

        int horizontalOffset = scaledFocusBorderMetrics.Width;
        int verticalOffset = scaledFocusBorderMetrics.Height;

        Padding borderPadding = borderStyle switch
        {
            BorderStyle.Fixed3D => new Padding(
                left: ScaleToDpi(Fixed3DBorderPadding, deviceDpi) + horizontalOffset,
                top: ScaleToDpi(Fixed3DBorderPadding, deviceDpi) + verticalOffset,
                right: ScaleToDpi(Fixed3DBorderPadding, deviceDpi) + horizontalOffset,
                bottom: ScaleToDpi(Fixed3DBorderPadding, deviceDpi) + verticalOffset),

            BorderStyle.FixedSingle => new Padding(
                left: ScaleToDpi(FixedSingleBorderPadding, deviceDpi) + horizontalOffset,
                top: ScaleToDpi(FixedSingleBorderPadding, deviceDpi) + verticalOffset,
                right: ScaleToDpi(FixedSingleBorderPadding, deviceDpi) + horizontalOffset,
                bottom: ScaleToDpi(FixedSingleBorderPadding, deviceDpi) + verticalOffset),

            BorderStyle.None => new Padding(
                left: ScaleToDpi(NoBorderPadding, deviceDpi),
                top: ScaleToDpi(NoBorderPadding, deviceDpi),
                right: ScaleToDpi(NoBorderPadding, deviceDpi) + horizontalOffset,
                bottom: ScaleToDpi(NoBorderPadding, deviceDpi) + verticalOffset),
            _ => Padding.Empty
        };

        return borderPadding
            + new Padding(ScaleToDpi(InternalChromeInset, deviceDpi))
            + userPadding;
    }

    internal static Size GetFocusBorderMetrics(
        Size focusBorderMetrics,
        float textScaleFactor,
        int deviceDpi)
        => new(
            ScaleFocusMetric(
                focusBorderMetrics.Width,
                textScaleFactor,
                deviceDpi),
            ScaleFocusMetric(
                focusBorderMetrics.Height,
                textScaleFactor,
                deviceDpi));

    internal static int GetFocusBandHeight(
        Size focusBorderMetrics,
        float textScaleFactor,
        int deviceDpi)
    {
        Size scaledFocusBorderMetrics = GetFocusBorderMetrics(
            focusBorderMetrics,
            textScaleFactor,
            deviceDpi);
        int scaledFocusBandHeight = ScaleFocusMetric(
            FocusBandHeight,
            textScaleFactor,
            deviceDpi);

        return Math.Max(
            scaledFocusBandHeight,
            scaledFocusBorderMetrics.Height);
    }

    internal static int GetPreferredFieldHeight(
        int fontHeight,
        Padding fieldPadding,
        int deviceDpi)
    {
        int preferredHeight = fontHeight + fieldPadding.Vertical;

        int roundedChromeMinimumHeight = ScaleToDpi(
            FieldCornerRadius,
            deviceDpi)
            + ScaleToDpi(BorderThickness, deviceDpi)
            + ScaleToDpi(InternalChromeInset, deviceDpi);

        return Math.Max(preferredHeight, roundedChromeMinimumHeight);
    }

    private static int ScaleFocusMetric(
        int metric,
        float textScaleFactor,
        int deviceDpi)
    {
        float scale = Math.Clamp(textScaleFactor, 1f, 2.25f);

        int dpiScaledMetric = ScaleToDpi(
            Math.Max(metric, BorderThickness),
            deviceDpi);

        return Math.Max(
            ScaleToDpi(BorderThickness, deviceDpi),
            (int)Math.Ceiling(dpiScaledMetric * scale));
    }

    private static int ScaleToDpi(int value, int deviceDpi)
        => ScaleHelper.ScaleToDpi(value, deviceDpi);
}
