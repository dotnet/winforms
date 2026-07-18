// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Rendering.Button;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

/// <summary>
///  Resolves the modern button palette and state colors shared by button renderers and appearance defaults.
/// </summary>
internal static class ModernButtonColorMath
{
    internal const float AccentBlendAmount = 0.2f;
    internal const float MinimumTextContrastRatio = PopupButtonColorMath.MinimumReadableContrastRatio;
    private const float DefaultHoverLightenAmount = 0.08f;
    private const float DefaultPressedDarkenAmount = 0.12f;

    internal static Color GetMouseDownColor()
        => Application.SystemVisualSettings.AccentColor;

    internal static Color GetMouseOverColor(ButtonBase owner, FlatButtonAppearance appearance)
    {
        Color baseColor = GetRenderedBaseColor(owner, appearance);

        return BlendWithAccent(baseColor);
    }

    internal static Color BlendWithAccent(Color baseColor)
        => PopupButtonColorMath.Blend(baseColor, Application.SystemVisualSettings.AccentColor, AccentBlendAmount);

    internal static Color GetDefaultButtonColor(PushButtonState state)
        => GetDefaultButtonColor(Application.SystemVisualSettings.AccentColor, state);

    internal static Color GetDefaultButtonColor(Color accentColor, PushButtonState state)
        => state switch
        {
            PushButtonState.Hot => PopupButtonColorMath.Lighten(accentColor, DefaultHoverLightenAmount),
            PushButtonState.Pressed => PopupButtonColorMath.Darken(accentColor, DefaultPressedDarkenAmount),
            _ => accentColor
        };

    internal static Color GetReadableForeColor(Color backColor)
    {
        float blackContrast = PopupButtonColorMath.GetContrastRatio(Color.Black, backColor);
        float whiteContrast = PopupButtonColorMath.GetContrastRatio(Color.White, backColor);
        Color foreColor = PopupButtonColorMath.GetReadableForeColor(backColor);

        Debug.Assert(
            Math.Max(blackContrast, whiteContrast) >= MinimumTextContrastRatio,
            "Either black or white should meet WCAG AA contrast against an opaque background.");

        return foreColor;
    }

    internal static Color GetStateColor(
        ButtonDarkModeRendererBase renderer,
        PushButtonState state,
        bool isDefault,
        Color customBaseColor)
    {
        if (customBaseColor.IsEmpty && isDefault)
        {
            return GetDefaultButtonColor(state);
        }

        Color baseColor = customBaseColor.IsEmpty
            ? renderer.GetBackgroundColor(PushButtonState.Normal, isDefault)
            : customBaseColor;

        return state switch
        {
            PushButtonState.Pressed => GetMouseDownColor(),
            PushButtonState.Hot => BlendWithAccent(baseColor),
            _ => baseColor
        };
    }

    internal static Color GetRenderedBaseColor(ButtonBase owner, FlatButtonAppearance appearance)
    {
        ButtonDarkModeRendererBase renderer = owner.FlatStyle switch
        {
            FlatStyle.Standard or FlatStyle.Popup => new ModernButtonDarkModeRenderer(),
            FlatStyle.Flat => new ModernFlatButtonRenderer(),
            FlatStyle.System => new SystemButtonDarkModeRenderer(),
            _ => throw new ArgumentOutOfRangeException(nameof(owner))
        };

        renderer.DeviceDpi = owner.DeviceDpi;
        renderer.FlatAppearance = appearance;

        if (owner.BackColor != Control.DefaultBackColor)
        {
            return owner.BackColor;
        }

        return renderer.GetBackgroundColor(PushButtonState.Normal, owner.IsDefault);
    }
}
