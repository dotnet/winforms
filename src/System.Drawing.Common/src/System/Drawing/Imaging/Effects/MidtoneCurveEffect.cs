// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Lightens or darkens an image. Color channel values in the middle of the intensity range are altered more than
///  color channel values near the minimum or maximum intensity. You can use this effect to lighten (or darken)
///  an image without loosing the contrast between the darkest and lightest portions of the image.
/// </summary>
public class MidtoneCurveEffect : ColorCurveEffect
{
    /// <summary>
    ///  Creates a new <see cref="MidtoneCurveEffect"/> with the given <paramref name="midtone"/> adjustment.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="midtone">
    ///  A value in the range of -100 through 100. A value of 0 specifies no change. Positive values specify that the
    ///  mid-tones are made lighter, and negative values specify that the mid-tones are made darker.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="midtone"/> is less than -100 or greater than 100.</exception>
    public MidtoneCurveEffect(CurveChannel channel, int midtone)
        : base(CurveAdjustments.AdjustMidtone, channel, midtone)
    {
    }

    /// <summary>
    ///  A value in the range of -100 through 100. A value of 0 specifies no change. Positive values specify that the
    ///  mid-tones are made lighter, and negative values specify that the mid-tones are made darker.
    /// </summary>
    public int Midtone => AdjustValue;
}
#endif
