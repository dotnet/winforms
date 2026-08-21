// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Resolves a <see cref="ModernFieldStroke"/> for a modern editable control from its
///  <see cref="ModernFieldStrokeContext"/>. This is the single chokepoint: paint paths call
///  <see cref="GetStroke"/> and receive a completed stroke, while state precedence and all color
///  and thickness selection stay private here. Visual target: see #14906.
/// </summary>
internal static class ModernFieldStrokeResolver
{
    private const float BaseStrokeDip = 2f;
    private const float FocusBottomStrokeDip = 4f;

    /// <summary>Resolves the completed stroke for the given context.</summary>
    internal static ModernFieldStroke GetStroke(in ModernFieldStrokeContext context)
    {
        ModernFieldStrokeState state = ResolveState(context);

        return context.HighContrast
            ? GetHighContrastStroke(state)
            : GetThemedStroke(state, context);
    }

    // Precedence: Disabled > Focused > ReadOnly > Hover > Rest.
    private static ModernFieldStrokeState ResolveState(in ModernFieldStrokeContext context)
    {
        if (!context.Enabled)
        {
            return ModernFieldStrokeState.Disabled;
        }

        if (context.Focused)
        {
            return ModernFieldStrokeState.Focused;
        }

        if (context.ReadOnly)
        {
            return ModernFieldStrokeState.ReadOnly;
        }

        if (context.Hovered)
        {
            return ModernFieldStrokeState.Hover;
        }

        return ModernFieldStrokeState.Rest;
    }

    private static ModernFieldStroke GetThemedStroke(ModernFieldStrokeState state, in ModernFieldStrokeContext context)
    {
        bool dark = context.DarkMode;
        Color surface = state == ModernFieldStrokeState.Disabled
            ? ModernControlColorMath.GetDisabledSurfaceColor()
            : context.BackColor;

        Color sideTop;
        Color bottom;
        float bottomDip = BaseStrokeDip;

        switch (state)
        {
            case ModernFieldStrokeState.Focused:
                sideTop = ModernControlColorMath.GetFieldStrokeSecondary(surface, dark);
                bottom = context.AccentColor;
                bottomDip = FocusBottomStrokeDip;
                break;

            case ModernFieldStrokeState.Hover:
                sideTop = ModernControlColorMath.GetFieldStrokeSecondary(surface, dark);
                bottom = ModernControlColorMath.GetFieldStrokeStrong(surface, dark);
                break;

            case ModernFieldStrokeState.Disabled:
                sideTop = ModernControlColorMath.GetDisabledBorderColor();
                bottom = ModernControlColorMath.GetDisabledBorderColor();
                break;

            default:
                // Rest and ReadOnly share the resting look; ReadOnly differs only by surface.
                sideTop = ModernControlColorMath.GetFieldStrokeDefault(surface, dark);
                bottom = ModernControlColorMath.GetFieldStrokeStrong(surface, dark);
                break;
        }

        return new ModernFieldStroke(sideTop, bottom, surface, BaseStrokeDip, bottomDip);
    }

    private static ModernFieldStroke GetHighContrastStroke(ModernFieldStrokeState state)
    {
        if (state == ModernFieldStrokeState.Disabled)
        {
            Color grayText = SystemColors.GrayText;
            return new ModernFieldStroke(grayText, grayText, SystemColors.Control, BaseStrokeDip, BaseStrokeDip);
        }

        Color frame = SystemColors.WindowFrame;
        bool focused = state == ModernFieldStrokeState.Focused;
        Color bottom = focused ? SystemColors.Highlight : frame;
        float bottomDip = focused ? FocusBottomStrokeDip : BaseStrokeDip;

        return new ModernFieldStroke(frame, bottom, SystemColors.Window, BaseStrokeDip, bottomDip);
    }
}
