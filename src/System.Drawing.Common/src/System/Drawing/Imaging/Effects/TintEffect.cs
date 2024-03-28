// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Allows you to apply a tint to an image.
/// </summary>
public unsafe class TintEffect : Effect
{
    private readonly TintParams _tintParams;

    /// <summary>
    ///  Creates a new <see cref="TintEffect"/> with the given parameters.
    /// </summary>
    /// <param name="color">
    ///  The color to be applied to the image.
    /// </param>
    /// <param name="amount">
    ///  Integer in the range -100 through 100 that specifies how much the hue (given by the hue parameter) is
    ///  strengthened or weakened. A value of 0 specifies no change. Positive values specify that the hue is
    ///  strengthened and negative values specify that the hue is weakened
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="amount"/> is less than -100 or greater than 100.</exception>
    public TintEffect(Color color, int amount) : this(
        (int)color.GetHue(),
        // If there is no color, the hue is 0, which is red. Set amount to 0 to replicate "no tint".
        color.IsEmpty || color == Color.White ? 0 : amount)
    {
    }

    /// <inheritdoc cref="TintEffect(Color, int)"/>
    /// <param name="hue">
    ///  Integer in the range of 0 to 360 that specifies the hue to be strengthened or weakened.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="hue"/> is less than 0 or greater than 360.</exception>
    /// <exception cref="ArgumentException"><paramref name="amount"/> is less than -100 or greater than 100.</exception>
    public TintEffect(int hue, int amount) : base(PInvoke.TintEffectGuid)
    {
        if (hue is < 0 or > 360)
        {
            throw new ArgumentOutOfRangeException(nameof(hue));
        }

        if (hue > 180)
        {
            hue -= 360;
        }

        _tintParams = new() { hue = hue, amount = amount };
        SetParameters(ref _tintParams);
    }

    /// <summary>
    ///  Integer in the range of 0 to 360 that specifies the hue to be strengthened or weakened.
    /// </summary>
    public int Hue => _tintParams.hue < 0 ? _tintParams.hue + 360 : _tintParams.hue;

    /// <summary>
    ///  Integer in the range -100 through 100 that specifies how much the hue (given by the hue parameter) is
    ///  strengthened or weakened. A value of 0 specifies no change. Positive values specify that the hue is
    ///  strengthened and negative values specify that the hue is weakened
    /// </summary>
    public int Amount => _tintParams.amount;
}
#endif
