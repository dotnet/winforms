// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Defines shared device-independent metrics for modern control chrome.
/// </summary>
internal static class ModernControlVisualStyles
{
    internal const int BorderThickness = 1;
    internal const int ComboBoxStyleInset = 1;
    internal const int FieldCornerRadius = 15;
    internal const int FocusBandHeight = 4;
    internal const float GroupBoxCaptionFontScale = 1.15f;
    internal const int GroupBoxCaptionGap = 4;
    internal const int GroupBoxContentBottomInset = 4;
    internal const int GroupBoxContentHorizontalInset = 8;
    internal const int GroupBoxContentTopInset = 8;
    internal const int GroupBoxCornerRadius = 8;
    internal const int GroupBoxHeaderHorizontalPadding = 10;
    internal const int GroupBoxHeaderVerticalPadding = 5;
    internal const int InternalChromeInset = 2;
    internal const int Fixed3DBorderPadding = 2;
    internal const int FixedSingleBorderPadding = 1;
    internal const int NoBorderPadding = 1;
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
