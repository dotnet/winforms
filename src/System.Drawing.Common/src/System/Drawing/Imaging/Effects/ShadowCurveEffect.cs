// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Increases or decreases the value of a color channel if that channel already has a value that is below half
///  intensity. You can use this effect to get more definition in the dark areas of an image without affecting
///  the light areas.
/// </summary>
public class ShadowCurveEffect : ColorCurveEffect
{
    /// <summary>
    ///  Creates a new <see cref="ShadowCurveEffect"/> with the given <paramref name="shadow"/>.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="shadow">
    ///  A value in the range of -100 through 100. A value of 0 specifies no change. Positive values specify that the
    ///  dark areas are made lighter, and negative values specify that the dark areas are made darker.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="shadow"/> is less than -100 or greater than 100.</exception>
    public ShadowCurveEffect(CurveChannel channel, int shadow)
        : base(CurveAdjustments.AdjustShadow, channel, shadow)
    {
    }

    /// <summary>
    ///  A value in the range of -100 through 100. A value of 0 specifies no change. Positive values specify that the
    ///  dark areas are made lighter, and negative values specify that the dark areas are made darker.
    /// </summary>
    public int Shadow => AdjustValue;
}
#endif
