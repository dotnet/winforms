// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Increases or decreases the value of a color channel if that channel already has a value that is above half
///  intensity. You can use this effect to get more definition in the light areas of an image without affecting
///  the dark areas.
/// </summary>
public class HighlightCurveEffect : ColorCurveEffect
{
    /// <summary>
    ///  Creates a new <see cref="HighlightCurveEffect"/> with the given <paramref name="highlight"/> adjustment.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="highlight">
    ///  A value in the range of -100 through 100. A value of 0 specifies no change. Positive values specify that the
    ///  light areas are made lighter, and negative values specify that the light areas are made darker.
    /// </param>
    public HighlightCurveEffect(CurveChannel channel, int highlight)
        : base(CurveAdjustments.AdjustHighlight, channel, highlight)
    {
    }

    /// <summary>
    ///  A value in the range of -100 through 100. A value of 0 specifies no change. Positive values specify that the
    ///  light areas are made lighter, and negative values specify that the light areas are made darker.
    /// </summary>
    public int Highlight => AdjustValue;
}
#endif
