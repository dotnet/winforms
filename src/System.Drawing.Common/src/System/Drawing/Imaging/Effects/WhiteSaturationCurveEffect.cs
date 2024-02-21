// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Sets the white saturation of an image. The white saturation is the point at which the lightest areas of the image
///  are converted to white.
/// </summary>
public class WhiteSaturationCurveEffect : ColorCurveEffect
{
    /// <summary>
    ///  Creates a new <see cref="WhiteSaturationCurveEffect"/> with the given parameters.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="whiteSaturation">
    ///  A value of t specifies that the interval [0, t] is mapped linearly to the interval [0, 255]. For example, if
    ///  <paramref name="whiteSaturation"/> is equal to 240, then color channel values in the interval [0, 240] are adjusted
    ///  so that they spread out over the interval [0, 255]. Color channel values greater than 240 are set to 255.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="whiteSaturation"/> was less than 1 or greater than 255.</exception>
    public WhiteSaturationCurveEffect(CurveChannel channel, int whiteSaturation)
        : base(CurveAdjustments.AdjustWhiteSaturation, channel, whiteSaturation)
    {
    }

    /// <summary>
    ///  A value of t specifies that the interval [0, t] is mapped linearly to the interval [0, 255]. For example, if
    ///  <see cref="WhiteSaturation"/> is equal to 240, then color channel values in the interval [0, 240] are adjusted
    ///  so that they spread out over the interval [0, 255]. Color channel values greater than 240 are set to 255.
    /// </summary>
    public int WhiteSaturation => AdjustValue;
}
#endif
