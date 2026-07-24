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
