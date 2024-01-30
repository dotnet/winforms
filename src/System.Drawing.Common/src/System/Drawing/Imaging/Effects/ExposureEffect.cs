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
    ///  Creates a new <see cref="ExposureEffect"/> with the given <paramref name="exposure"/> adjustment.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="exposure">
    ///  A value in the range of -256 through 256. A value of 0 specifies no change in exposure. Positive values
    ///  specify increased exposure and negative values specify decreased exposure.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="exposure"/> is less than -256 or greater than 256.</exception>
    public ExposureEffect(CurveChannel channel, int exposure)
        : base(CurveAdjustments.AdjustExposure, channel, exposure)
    {
    }

    /// <summary>
    ///  A value in the range of -256 through 256. A value of 0 specifies no change in exposure. Positive values
    ///  specify increased exposure and negative values specify decreased exposure.
    /// </summary>
    public int Exposure => AdjustValue;
}
#endif
