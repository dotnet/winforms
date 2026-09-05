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

    // WinUI control-stroke overlay alphas over the black (light mode) / white (dark mode) pole,
    // verified against Common_themeresources_any.xaml, except light-mode Strong, which is raised
    // above WinUI so the visible bottom edge meets WCAG 1.4.11 (#14906). Composited in linear light.
    private const int StrokeDefaultAlphaLight = 0x0F;    // ControlStrokeColorDefault
    private const int StrokeDefaultAlphaDark = 0x03;     // near-invisible dark rest side, matching the light/classic look where the side effectively vanishes (WinUI value is 0x12, #14919)
    private const int StrokeSecondaryAlphaLight = 0x29;  // ControlStrokeColorSecondary
    private const int StrokeSecondaryAlphaDark = 0x18;
    private const int StrokeStrongAlphaLight = 0xB6;     // resting bottom edge; ~3.1:1, the WCAG 1.4.11 floor, lightened from 0xD1 so it is less heavy than the focus accent (#14906, #14997).
    private const int StrokeStrongAlphaDark = 0x8B;

    // Hover overlay: tuned a step stronger than Secondary for a more noticeable cue (#14906 direction).
    private const int StrokeHoverAlphaLight = 0x40;
    private const int StrokeHoverAlphaDark = 0x28;

    // Read-only surface tint: a subtle fill shift signalling non-editability, per Leaf's #14906 table.
    private const int SurfaceReadOnlyAlphaLight = 0x0A;
    private const int SurfaceReadOnlyAlphaDark = 0x0A;

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
    private static readonly Color s_darkModeDisabledBorderStrong = Color.FromArgb(0x6A, 0x6A, 0x6A);
    private static readonly Color s_lightModeDisabledBorderStrong = Color.FromArgb(0xB0, 0xB0, 0xB0);

    /// <summary>
    ///  Gets the stable border color for modern editable text controls when enabled.
    /// </summary>
    internal static Color TextControlBorderColor
        => Application.IsDarkModeEnabled
            ? SystemColors.WindowText
            : SystemColors.WindowFrame;

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

    /// <summary>Gets the stronger disabled border color used for the disabled bottom (elevation) edge.</summary>
    internal static Color GetDisabledStrongBorderColor()
        => SystemInformation.HighContrast
            ? SystemColors.GrayText
            : Application.IsDarkModeEnabled
                ? s_darkModeDisabledBorderStrong
                : s_lightModeDisabledBorderStrong;

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

    /// <summary>Gets the default, lightest field border stroke composited onto <paramref name="background"/>.</summary>
    internal static Color GetFieldStrokeDefault(Color background, bool darkMode)
        => CompositeStrokeOverlay(background, darkMode ? StrokeDefaultAlphaDark : StrokeDefaultAlphaLight, darkMode);

    /// <summary>Gets the secondary field border stroke, a step stronger than default.</summary>
    internal static Color GetFieldStrokeSecondary(Color background, bool darkMode)
        => CompositeStrokeOverlay(background, darkMode ? StrokeSecondaryAlphaDark : StrokeSecondaryAlphaLight, darkMode);

    /// <summary>Gets the hover field border stroke: a bit stronger than secondary for a noticeable cue.</summary>
    internal static Color GetFieldStrokeHover(Color background, bool darkMode)
        => CompositeStrokeOverlay(background, darkMode ? StrokeHoverAlphaDark : StrokeHoverAlphaLight, darkMode);

    /// <summary>Gets the ReadOnly control surface: a subtle non-editable tint of the background.</summary>
    internal static Color GetFieldReadOnlySurface(Color background, bool darkMode)
        => CompositeStrokeOverlay(background, darkMode ? SurfaceReadOnlyAlphaDark : SurfaceReadOnlyAlphaLight, darkMode);

    /// <summary>Gets the strong field border stroke, used for the resting bottom (elevation) edge.</summary>
    internal static Color GetFieldStrokeStrong(Color background, bool darkMode)
        => CompositeStrokeOverlay(background, darkMode ? StrokeStrongAlphaDark : StrokeStrongAlphaLight, darkMode);

    // Composites a black (light) or white (dark) overlay of the given 0-255 alpha onto an opaque
    // background in linear light, returning an opaque color. A straight sRGB blend is wrong here.
    private static Color CompositeStrokeOverlay(Color background, int overlayAlpha, bool darkMode)
    {
        background = ResolveOpaqueColor(background);
        float alpha = Math.Clamp(overlayAlpha / 255f, 0f, 1f);
        float pole = darkMode ? 1f : 0f;

        return Color.FromArgb(
            byte.MaxValue,
            CompositeChannel(background.R),
            CompositeChannel(background.G),
            CompositeChannel(background.B));

        byte CompositeChannel(byte channel)
        {
            float mixed = (pole * alpha) + (SrgbToLinear(channel) * (1f - alpha));
            return LinearToSrgb(mixed);
        }
    }

    private static float SrgbToLinear(byte channel)
    {
        float value = channel / 255f;
        return value <= 0.04045f
            ? value / 12.92f
            : MathF.Pow((value + 0.055f) / 1.055f, 2.4f);
    }

    private static byte LinearToSrgb(float linear)
    {
        linear = Math.Clamp(linear, 0f, 1f);
        float value = linear <= 0.0031308f
            ? linear * 12.92f
            : (1.055f * MathF.Pow(linear, 1f / 2.4f)) - 0.055f;
        return (byte)MathF.Round(value * 255f);
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
