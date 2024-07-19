// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Simulates increasing or decreasing the film density of a photograph.
/// </summary>
public class DensityCurveEffect : ColorCurveEffect
{
    /// <summary>
    ///  Creates a new <see cref="DensityCurveEffect"/> with the given <paramref name="density"/>.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="density">
    ///  A value in the range of -256 through 256. A value of 0 specifies no change in density. Positive values specify
    ///  increased density (lighter picture) and negative values specify decreased density (darker picture).
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="density"/> is less than -256 or greater than 256.</exception>
    public DensityCurveEffect(CurveChannel channel, int density)
        : base(CurveAdjustments.AdjustDensity, channel, density)
    {
    }

    /// <summary>
    ///  A value in the range of -256 through 256. A value of 0 specifies no change in density. Positive values specify
    ///  increased density (lighter picture) and negative values specify decreased density (darker picture).
    /// </summary>
    public int Density => AdjustValue;
}
#endif
