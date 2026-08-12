// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Rendering.Button;

namespace System.Windows.Forms;

/// <summary>
///  Provides shared color calculations for modern control renderers.
/// </summary>
internal static class ModernControlColorMath
{
    internal const float MinimumDisabledTextContrastRatio = 3f;

    private const float DisabledMuteAmount = 0.45f;
    private const int ContrastSearchIterations = 10;

    // Shared disabled-state palette for modern renderers. Modern controls do not honor user-set
    // BackColor/ForeColor while disabled, so these fixed surfaces replace them. This is the single
    // source of truth: the modern Button renderers and the modern ComboBox adapter all read from
    // here, so a disabled Button and a disabled ComboBox cannot drift apart.
    private static readonly Color s_darkModeDisabledSurface = Color.FromArgb(0x25, 0x25, 0x25);
    private static readonly Color s_lightModeDisabledSurface = Color.FromArgb(0xFA, 0xFA, 0xFA);
    private static readonly Color s_darkModeDisabledBorder = Color.FromArgb(0x55, 0x55, 0x55);
    private static readonly Color s_lightModeDisabledBorder = Color.FromArgb(0xD0, 0xD0, 0xD0);
    private static readonly Color s_darkModeDisabledForeground = Color.FromArgb(0x88, 0x88, 0x88);
    private static readonly Color s_lightModeDisabledForeground = Color.FromArgb(0xA0, 0xA0, 0xA0);

    /// <summary>
    ///  Gets the surface color for a disabled modern control, honoring the current color mode
    ///  and high contrast settings.
    /// </summary>
    internal static Color GetDisabledSurfaceColor()
        => SystemInformation.HighContrast
            ? SystemColors.Control
            : Application.IsDarkModeEnabled
                ? s_darkModeDisabledSurface
                : s_lightModeDisabledSurface;

    /// <summary>
    ///  Gets the border color for a disabled modern control, honoring the current color mode
    ///  and high contrast settings.
    /// </summary>
    internal static Color GetDisabledBorderColor()
        => SystemInformation.HighContrast
            ? SystemColors.GrayText
            : Application.IsDarkModeEnabled
                ? s_darkModeDisabledBorder
                : s_lightModeDisabledBorder;

    /// <summary>
    ///  Gets the contrast-adjusted foreground color for content drawn on
    ///  <see cref="GetDisabledSurfaceColor"/>.
    /// </summary>
    internal static Color GetDisabledForeColor(Color backColor)
        => GetDisabledTextColor(
            Application.IsDarkModeEnabled
                ? s_darkModeDisabledForeground
                : s_lightModeDisabledForeground,
            backColor);

    internal static Color GetDisabledTextColor(
        Color preferredForeColor,
        Color backColor)
        => GetDisabledTextColor(
            preferredForeColor,
            backColor,
            backColor);

    internal static Color GetDisabledTextColor(
        Color preferredForeColor,
        Color firstBackColor,
        Color secondBackColor)
    {
        if (SystemInformation.HighContrast)
        {
            return SystemColors.GrayText;
        }

        firstBackColor = ResolveOpaqueColor(firstBackColor);
        secondBackColor = ResolveOpaqueColor(secondBackColor);

        Color muteColor = PopupButtonColorMath.Blend(
            firstBackColor,
            secondBackColor,
            0.5f);
        Color mutedForeColor = PopupButtonColorMath.Blend(
            preferredForeColor,
            muteColor,
            DisabledMuteAmount);

        if (HasMinimumContrast(
            mutedForeColor,
            firstBackColor,
            secondBackColor))
        {
            return mutedForeColor;
        }

        Color contrastColor = PopupButtonColorMath.GetReadableForeColor(
            firstBackColor,
            secondBackColor);
        Color result = contrastColor;
        float low = 0f;
        float high = 1f;

        for (int i = 0; i < ContrastSearchIterations; i++)
        {
            float amount = (low + high) / 2f;
            Color candidate = PopupButtonColorMath.Blend(
                mutedForeColor,
                contrastColor,
                amount);

            if (HasMinimumContrast(
                candidate,
                firstBackColor,
                secondBackColor))
            {
                result = candidate;
                high = amount;
            }
            else
            {
                low = amount;
            }
        }

        return result;
    }

    private static bool HasMinimumContrast(
        Color foreColor,
        Color firstBackColor,
        Color secondBackColor)
        => PopupButtonColorMath.GetContrastRatio(
            foreColor,
            firstBackColor) >= MinimumDisabledTextContrastRatio
            && PopupButtonColorMath.GetContrastRatio(
                foreColor,
                secondBackColor) >= MinimumDisabledTextContrastRatio;

    private static Color ResolveOpaqueColor(Color color)
        => color.A == byte.MaxValue
            ? color
            : PopupButtonColorMath.Composite(color, SystemColors.Control);
}
