// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Rendering.Button;

/// <summary>
///  Easing functions for the <see cref="FlatStyle.Popup"/> key-cap state-change animations.
/// </summary>
internal static class PopupButtonEasing
{
    /// <summary>
    ///  Cubic ease-out. Natural for hover enter/leave - fast start, gentle settle.
    /// </summary>
    public static float EaseOutCubic(float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        float inv = 1f - t;

        return 1f - (inv * inv * inv);
    }

    /// <summary>
    ///  Quadratic ease-in/ease-out. Snappy for press and release.
    /// </summary>
    public static float EaseInOutQuad(float t)
    {
        t = Math.Clamp(t, 0f, 1f);

        return t < 0.5f
            ? 2f * t * t
            : 1f - (MathF.Pow((-2f * t) + 2f, 2f) / 2f);
    }
}
