// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

using System.Runtime.Versioning;

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Simulates increasing or decreasing the exposure of a photograph
/// </summary>
[RequiresPreviewFeatures]
public sealed class ExposureEffect : ColorCurveEffect
{
    /// <summary>
    ///  Creates a new <see cref="ExposureEffect"/> with the given <paramref name="adjustValue"/>.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="adjustValue">
    ///  A value in the range of -255 through 255. A value of 0 specifies no change in exposure. Positive values
    ///  specify increased exposure and negative values specify decreased exposure.
    /// </param>
    public ExposureEffect(CurveChannel channel, int adjustValue)
        : base(CurveAdjustments.AdjustExposure, channel, adjustValue)
    {
    }
}
#endif
