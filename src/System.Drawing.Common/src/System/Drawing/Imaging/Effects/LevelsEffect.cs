// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Adjusts the light, mid-tone, or dark areas of an image.
/// </summary>
public class LevelsEffect : Effect
{
    private readonly LevelsParams _levelsParams;

    /// <summary>
    ///  Creates a new <see cref="LevelsEffect"/> with the given parameters.
    /// </summary>
    /// <param name="highlight">
    ///  Integer in the range 0 through 100 that specifies which pixels should be lightened. You can use this adjustment
    ///  to lighten pixels that are already lighter than a certain threshold. Setting highlight to 100 specifies no change.
    ///  Setting highlight to t specifies that a color channel value is increased if it is already greater than t percent
    ///  of full intensity. For example, setting highlight to 90 specifies that all color channel values greater than 90
    ///  percent of full intensity are increased.
    /// </param>
    /// <param name="midtone">
    ///  Integer in the range -100 through 100 that specifies how much to lighten or darken an image. Color channel values
    ///  in the middle of the intensity range are altered more than color channel values near the minimum or maximum
    ///  intensity. You can use this adjustment to lighten (or darken) an image without loosing the contrast between the
    ///  darkest and lightest portions of the image. A value of 0 specifies no change. Positive values specify that the
    ///  mid-tones are made lighter, and negative values specify that the mid-tones are made darker.
    /// </param>
    /// <param name="shadow">
    ///  Integer in the range 0 through 100 that specifies which pixels should be darkened. You can use this adjustment
    ///  to darken pixels that are already darker than a certain threshold. Setting shadow to 0 specifies no change.
    ///  Setting shadow to t specifies that a color channel value is decreased if it is already less than t percent of
    ///  full intensity. For example, setting shadow to 10 specifies that all color channel values less than 10 percent
    ///  of full intensity are decreased.
    /// </param>
    /// <exception cref="ArgumentException">
    ///  <paramref name="highlight"/> or <paramref name="shadow"/> is less than 0 or greater than 100 or
    ///  <paramref name="midtone"/> is less than -100 or greater than 100.
    /// </exception>
    public LevelsEffect(int highlight, int midtone, int shadow)
        : base(PInvoke.LevelsEffectGuid)
    {
        _levelsParams = new()
        {
            highlight = highlight,
            midtone = midtone,
            shadow = shadow
        };

        SetParameters(ref _levelsParams);
    }

    /// <summary>
    ///  The highlight threshold.
    /// </summary>
    public int Highlight => _levelsParams.highlight;

    /// <summary>
    ///  The midtone adjustment.
    /// </summary>
    public int Midtone => _levelsParams.midtone;

    /// <summary>
    ///  The shadow threshold.
    /// </summary>
    public int Shadow => _levelsParams.shadow;
}
#endif
