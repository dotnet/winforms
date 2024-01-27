// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

using System.Runtime.Versioning;

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Allows you to apply a tint to an image.
/// </summary>
[RequiresPreviewFeatures]
public sealed unsafe class TintEffect : Effect
{
    private readonly TintParams _tintParams;

    /// <summary>
    ///  Initializes a new instance of the <see cref="TintEffect"/> class.
    /// </summary>
    /// <param name="color">
    ///  The color to be applied to the image.
    /// </param>
    /// <param name="amount">
    ///  Integer in the range -100 through 100 that specifies how much the hue (given by the hue parameter) is
    ///  strengthened or weakened. A value of 0 specifies no change. Positive values specify that the hue is
    ///  strengthened and negative values specify that the hue is weakened
    /// </param>
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
    public TintEffect(int hue, int amount) : base(PInvoke.TintEffectGuid)
    {
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
