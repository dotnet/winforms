// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Builds a <see cref="ModernFieldStroke"/> from <see cref="ModernFieldStrokeContext"/>.
///  Paint paths call <see cref="GetStroke"/>; this class owns precedence, colors, and thicknesses.
///  Visual target: #14906.
/// </summary>
internal static class ModernFieldStrokeResolver
{
    private const float BaseStrokeDip = 2f;
    private const float FocusBottomStrokeDip = 3f;

    /// <summary>Resolves a stroke from the supplied context.</summary>
    internal static ModernFieldStroke GetStroke(in ModernFieldStrokeContext context)
    {
        ModernFieldStrokeState state = ResolveState(context);

        return GetThemedStroke(state, context);
    }

    // Priority: Disabled > Focused > ReadOnly > Hover > Rest.
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
        // The control surface prevents an inner boundary with the native client area (#14997).
        Color surface = context.BackColor;
        Color strokeBackground = context.BackColor;

        Color sideTop;
        Color bottom;
        float bottomDip = BaseStrokeDip;

        switch (state)
        {
            case ModernFieldStrokeState.Focused:
                // Focus keeps the resting side color; the accent bottom edge supplies the cue.
                sideTop = ModernControlColorMath.GetFieldStrokeDefault(strokeBackground, dark);
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
                // Rest and ReadOnly share edge strokes; only their surfaces differ.
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
