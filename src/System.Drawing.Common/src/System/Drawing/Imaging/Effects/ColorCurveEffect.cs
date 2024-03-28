// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Base class for several effects that can be applied to an image.
/// </summary>
public abstract class ColorCurveEffect : Effect
{
    private readonly ColorCurveParams _parameters;

    private protected ColorCurveEffect(CurveAdjustments adjustment, CurveChannel channel, int adjustValue)
        : base(PInvoke.ColorCurveEffectGuid)
    {
        _parameters = new()
        {
            adjustment = adjustment,
            channel = (GdiPlus.CurveChannel)channel,
            adjustValue = adjustValue
        };

        SetParameters(ref _parameters);
    }

    /// <summary>
    ///  The channel or channels that the effect is applied to.
    /// </summary>
    public CurveChannel Channel => (CurveChannel)_parameters.channel;

    /// <summary>
    ///  The adjustment value.
    /// </summary>
    private protected int AdjustValue => _parameters.adjustValue;
}
#endif
