// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

using System.Runtime.Versioning;

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Simulates increasing or decreasing the film density of a photograph.
/// </summary>
[RequiresPreviewFeatures]
public sealed class DensityEffect : ColorCurveEffect
{
    /// <summary>
    ///  Creates a new <see cref="DensityEffect"/> with the given <paramref name="adjustValue"/>.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="adjustValue">
    ///  A value in the range of -255 through 255. A value of 0 specifies no change in density. Positive values specify
    ///  increased density (lighter picture) and negative values specify decreased density (darker picture).
    /// </param>
    public DensityEffect(CurveChannel channel, int adjustValue)
        : base(CurveAdjustments.AdjustDensity, channel, adjustValue)
    {
    }
}
#endif
