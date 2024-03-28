// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Adjusts the color balance of an image.
/// </summary>
public class ColorBalanceEffect : Effect
{
    private readonly ColorBalanceParams _colorBalanceParams;

    /// <summary>
    ///  Creates a new <see cref="ColorBalanceEffect"/> with the given parameters.
    /// </summary>
    /// <param name="cyanRed">
    ///  Integer in the range -100 through 100 that specifies a change in the amount of red in the image. If the value
    ///  is 0, there is no change. As the value moves from 0 to 100, the amount of red in the image increases and the
    ///  amount of cyan decreases. As the value moves from 0 to -100, the amount of red in the image decreases and the
    ///  amount of cyan increases.
    /// </param>
    /// <param name="magentaGreen">
    ///  Integer in the range -100 through 100 that specifies a change in the amount of green in the image. If the value
    ///  is 0, there is no change. As the value moves from 0 to 100, the amount of green in the image increases and the
    ///  amount of magenta decreases. As the value moves from 0 to -100, the amount of green in the image decreases and
    ///  and the amount of magenta increases.
    /// </param>
    /// <param name="yellowBlue">
    ///  Integer in the range -100 through 100 that specifies a change in the amount of blue in the image. If the value
    ///  is 0, there is no change. As the value moves from 0 to 100, the amount of blue in the image increases and the
    ///  amount of yellow decreases. As the value moves from 0 to -100, the amount of blue in the image decreases and
    ///  the amount of yellow increases.
    /// </param>
    /// <exception cref="ArgumentException">
    ///  <paramref name="cyanRed"/>, <paramref name="magentaGreen"/>, or <paramref name="yellowBlue"/> is less than
    ///  -100 or greater than 100.
    /// </exception>
    public ColorBalanceEffect(int cyanRed, int magentaGreen, int yellowBlue) : base(PInvoke.ColorBalanceEffectGuid)
    {
        _colorBalanceParams = new() { cyanRed = cyanRed, magentaGreen = magentaGreen, yellowBlue = yellowBlue };
        SetParameters(ref _colorBalanceParams);
    }

    /// <summary>
    ///  Integer in the range -100 through 100 that specifies a change in the amount of red in the image. If the value
    ///  is 0, there is no change. As the value moves from 0 to 100, the amount of red in the image increases and the
    ///  amount of cyan decreases. As the value moves from 0 to -100, the amount of red in the image decreases and the
    ///  amount of cyan increases.
    /// </summary>
    public int CyanRed => _colorBalanceParams.cyanRed;

    /// <summary>
    ///  Integer in the range -100 through 100 that specifies a change in the amount of green in the image. If the value
    ///  is 0, there is no change. As the value moves from 0 to 100, the amount of green in the image increases and the
    ///  amount of magenta decreases. As the value moves from 0 to -100, the amount of green in the image decreases and
    ///  and the amount of magenta increases.
    /// </summary>
    public int MagentaGreen => _colorBalanceParams.magentaGreen;

    /// <summary>
    ///  Integer in the range -100 through 100 that specifies a change in the amount of blue in the image. If the value
    ///  is 0, there is no change. As the value moves from 0 to 100, the amount of blue in the image increases and the
    ///  amount of yellow decreases. As the value moves from 0 to -100, the amount of blue in the image decreases and
    ///  the amount of yellow increases.
    /// </summary>
    public int YellowBlue => _colorBalanceParams.yellowBlue;
}
#endif
