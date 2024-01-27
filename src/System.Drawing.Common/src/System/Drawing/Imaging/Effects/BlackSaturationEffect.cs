// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

using System.Runtime.Versioning;

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Sets the black saturation of an image. The black saturation is the point at which the darkest areas of the image
///  are converted to black.
/// </summary>
[RequiresPreviewFeatures]
public sealed class BlackSaturationEffect : ColorCurveEffect
{
    /// <summary>
    ///  Creates a new <see cref="BlackSaturationEffect"/> with the given <paramref name="adjustValue"/>.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="adjustValue">
    ///  A value of t specifies that the interval [t, 255] is mapped linearly to the interval [0, 255]. For example, if
    ///  <paramref name="adjustValue"/> is equal to 15, then color channel values in the interval [15, 255] are adjusted
    ///  so that they spread out over the interval [0, 255]. Color channel values less than 15 are set to 0.
    /// </param>
    public BlackSaturationEffect(CurveChannel channel, int adjustValue)
        : base(CurveAdjustments.AdjustBlackSaturation, channel, adjustValue)
    {
    }
}
#endif
