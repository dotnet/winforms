// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Rendering.Button;

/// <summary>
///  Color utilities used by the <see cref="FlatStyle.Popup"/> key-cap renderer to derive material shading
///  colors from arbitrary effective colors.
/// </summary>
/// <remarks>
///  <para>
///   Nothing in here assumes a light or dark base color; every derivation is relative to the input, which is
///   what makes the material effect work with saturated, neutral, light, dark and (dark-mode remapped) system
///   colors alike.
///  </para>
/// </remarks>
internal static class PopupButtonColorMath
{
    public const float MinimumReadableContrastRatio = 4.5f;

    /// <summary>
    ///  Gets the relative luminance of a color in the range <c>0</c>-<c>1</c>.
    /// </summary>
    public static float GetLuminance(Color color)
        => ((0.299f * color.R) + (0.587f * color.G) + (0.114f * color.B)) / 255f;

    /// <summary>
    ///  Gets the WCAG relative luminance of a color, including the required sRGB linearization.
    /// </summary>
    public static float GetRelativeLuminance(Color color)
        => (0.2126f * Linearize(color.R))
            + (0.7152f * Linearize(color.G))
            + (0.0722f * Linearize(color.B));

    /// <summary>
    ///  Gets the WCAG contrast ratio between two colors.
    /// </summary>
    public static float GetContrastRatio(Color first, Color second)
    {
        float firstLuminance = GetRelativeLuminance(first);
        float secondLuminance = GetRelativeLuminance(second);
        float lighter = Math.Max(firstLuminance, secondLuminance);
        float darker = Math.Min(firstLuminance, secondLuminance);

        return (lighter + 0.05f) / (darker + 0.05f);
    }

    /// <summary>
    ///  Chooses black or white, whichever has the greater WCAG contrast ratio against the background.
    /// </summary>
    public static Color GetReadableForeColor(Color backColor)
        => GetContrastRatio(Color.Black, backColor) >= GetContrastRatio(Color.White, backColor)
            ? Color.Black
            : Color.White;

    /// <summary>
    ///  Chooses black or white by its worst contrast ratio across the darkest and lightest rendered surfaces.
    /// </summary>
    public static Color GetReadableForeColor(Color darkestBackColor, Color lightestBackColor)
    {
        float blackContrast = Math.Min(
            GetContrastRatio(Color.Black, darkestBackColor),
            GetContrastRatio(Color.Black, lightestBackColor));
        float whiteContrast = Math.Min(
            GetContrastRatio(Color.White, darkestBackColor),
            GetContrastRatio(Color.White, lightestBackColor));

        return blackContrast >= whiteContrast ? Color.Black : Color.White;
    }

    /// <summary>
    ///  Composites a foreground color over a background color.
    /// </summary>
    public static Color Composite(Color foreground, Color background)
    {
        float foregroundAlpha = foreground.A / 255f;
        float backgroundAlpha = background.A / 255f;
        float alpha = foregroundAlpha + (backgroundAlpha * (1f - foregroundAlpha));

        if (alpha <= 0f)
        {
            return Color.Transparent;
        }

        return Color.FromArgb(
            (int)MathF.Round(alpha * 255f),
            CompositeChannel(foreground.R, background.R),
            CompositeChannel(foreground.G, background.G),
            CompositeChannel(foreground.B, background.B));

        int CompositeChannel(byte foregroundChannel, byte backgroundChannel)
            => (int)MathF.Round(
                ((foregroundChannel * foregroundAlpha)
                    + (backgroundChannel * backgroundAlpha * (1f - foregroundAlpha)))
                / alpha);
    }

    /// <summary>
    ///  Linearly blends <paramref name="baseColor"/> towards <paramref name="target"/>.
    /// </summary>
    /// <param name="baseColor">The starting color.</param>
    /// <param name="target">The color to blend towards.</param>
    /// <param name="amount">Blend amount, <c>0</c> (base) to <c>1</c> (target).</param>
    public static Color Blend(Color baseColor, Color target, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);

        int r = (int)MathF.Round(baseColor.R + ((target.R - baseColor.R) * amount));
        int g = (int)MathF.Round(baseColor.G + ((target.G - baseColor.G) * amount));
        int b = (int)MathF.Round(baseColor.B + ((target.B - baseColor.B) * amount));

        return Color.FromArgb(baseColor.A, r, g, b);
    }

    /// <summary>
    ///  Blends the color towards white by the given amount.
    /// </summary>
    public static Color Lighten(Color color, float amount)
        => Blend(color, Color.White, amount);

    /// <summary>
    ///  Blends the color towards black by the given amount.
    /// </summary>
    public static Color Darken(Color color, float amount)
        => Blend(color, Color.Black, amount);

    /// <summary>
    ///  Blends the color towards the pole (black or white) that increases contrast against itself - lightens
    ///  dark colors, darkens light colors.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Used for borders and cues that must remain visible on both light and dark surfaces.
    ///  </para>
    /// </remarks>
    public static Color TowardsContrast(Color color, float amount)
        => GetLuminance(color) > 0.5f
            ? Darken(color, amount)
            : Lighten(color, amount);

    /// <summary>
    ///  Desaturates the color towards a gray of the same luminance and slightly pulls it towards a mid tone.
    ///  Used for disabled surfaces.
    /// </summary>
    public static Color Mute(Color color, float amount)
    {
        int gray = (int)MathF.Round(GetLuminance(color) * 255f);
        Color desaturated = Blend(color, Color.FromArgb(color.A, gray, gray, gray), amount);

        return Blend(desaturated, Color.FromArgb(color.A, 128, 128, 128), amount * 0.35f);
    }

    /// <summary>
    ///  Ensures a minimum luminance difference between <paramref name="color"/> and <paramref name="against"/>
    ///  by pushing <paramref name="color"/> away if needed.
    /// </summary>
    /// <param name="color">The color to adjust.</param>
    /// <param name="against">The surface it must stay readable on.</param>
    /// <param name="minDelta">Minimum luminance delta, <c>0</c>-<c>1</c>.</param>
    public static Color EnsureContrast(Color color, Color against, float minDelta)
    {
        float delta = MathF.Abs(GetLuminance(color) - GetLuminance(against));

        if (delta >= minDelta)
        {
            return color;
        }

        float push = minDelta - delta;

        return GetLuminance(against) > 0.5f
            ? Darken(color, push)
            : Lighten(color, push);
    }

    /// <summary>
    ///  Derives an opaque highlight color for text relief on the given surface.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Opaque blended colors are used instead of alpha because GDI text rendering via
    ///   <see cref="TextRenderer"/> ignores the alpha channel.
    ///  </para>
    /// </remarks>
    public static Color GetTextHighlight(Color surface, float strength)
        => Lighten(surface, 0.28f * strength);

    /// <summary>
    ///  Derives an opaque shadow color for text relief on the given surface.
    /// </summary>
    public static Color GetTextShadow(Color surface, float strength)
        => Darken(surface, 0.34f * strength);

    private static float Linearize(byte channel)
    {
        float value = channel / 255f;

        return value <= 0.04045f
            ? value / 12.92f
            : MathF.Pow((value + 0.055f) / 1.055f, 2.4f);
    }
}
