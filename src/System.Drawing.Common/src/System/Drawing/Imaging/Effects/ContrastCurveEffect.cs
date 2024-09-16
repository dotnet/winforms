// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Increases or decreases the contrast of an image.
/// </summary>
public class ContrastCurveEffect : ColorCurveEffect
{
    /// <summary>
    ///  Creates a new <see cref="ContrastCurveEffect"/> with the given <paramref name="contrast"/> adjustment value.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="contrast">
    ///  A value in the range of -100 through 100. A value of 0 specifies no change in contrast. Positive values
    ///  specify increased contrast and negative values specify decreased contrast.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="contrast"/> is less than -100 or greater than 100.</exception>
    public ContrastCurveEffect(CurveChannel channel, int contrast)
        : base(CurveAdjustments.AdjustContrast, channel, contrast)
    {
    }

    /// <summary>
    ///  A value in the range of -100 through 100. A value of 0 specifies no change in contrast. Positive values
    ///  specify increased contrast and negative values specify decreased contrast.
    /// </summary>
    public int Contrast => AdjustValue;
}
#endif
