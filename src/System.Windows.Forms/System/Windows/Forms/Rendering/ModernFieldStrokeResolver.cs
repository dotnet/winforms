// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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

        return GetThemedStroke(state, context);
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
        Color surface = state switch
        {
            ModernFieldStrokeState.Disabled => ModernControlColorMath.GetDisabledSurfaceColor(),
            ModernFieldStrokeState.ReadOnly => ModernControlColorMath.GetFieldReadOnlySurface(context.BackColor, dark),
            _ => context.BackColor,
        };

        // Hover keeps the Rest surface so the editable area does not appear to shrink; it reads as Hover
        // through its border color alone (#14997). All strokes composite over the normal background.
        Color strokeBackground = context.BackColor;

        Color sideTop;
        Color bottom;
        float bottomDip = BaseStrokeDip;

        switch (state)
        {
            case ModernFieldStrokeState.Focused:
                sideTop = ModernControlColorMath.GetFieldStrokeHover(strokeBackground, dark);
                bottom = context.AccentColor;
                bottomDip = FocusBottomStrokeDip;
                break;

            case ModernFieldStrokeState.Hover:
                sideTop = ModernControlColorMath.GetFieldStrokeHover(strokeBackground, dark);
                bottom = ModernControlColorMath.GetFieldStrokeStrong(strokeBackground, dark);
                break;

            case ModernFieldStrokeState.Disabled:
                sideTop = ModernControlColorMath.GetDisabledBorderColor();
                bottom = ModernControlColorMath.GetDisabledStrongBorderColor();
                break;

            default:
                // Rest and ReadOnly share the resting strokes; ReadOnly differs only by its surface.
                sideTop = ModernControlColorMath.GetFieldStrokeDefault(strokeBackground, dark);
                bottom = ModernControlColorMath.GetFieldStrokeStrong(strokeBackground, dark);
                break;
        }

        return new ModernFieldStroke(
            sideTop,
            bottom,
            surface,
            BaseStrokeDip,
            bottomDip,
            HasFocusIndicator: state == ModernFieldStrokeState.Focused);
    }
}
