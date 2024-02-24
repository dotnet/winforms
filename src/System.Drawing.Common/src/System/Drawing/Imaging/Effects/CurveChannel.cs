// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  The <see cref="CurveChannel"/> enumeration specifies which color channels are affected by a <see cref="ColorCurveEffect"/>.
/// </summary>
public enum CurveChannel
{
    /// <summary>
    ///  Specifies that the color adjustment applies to all channels.
    /// </summary>
    All = GdiPlus.CurveChannel.CurveChannelAll,

    /// <summary>
    ///  Specifies that the color adjustment applies only to the red channel.
    /// </summary>
    Red = GdiPlus.CurveChannel.CurveChannelRed,

    /// <summary>
    ///  Specifies that the color adjustment applies only to the green channel.
    /// </summary>
    Green = GdiPlus.CurveChannel.CurveChannelGreen,

    /// <summary>
    ///  Specifies that the color adjustment applies only to the blue channel.
    /// </summary>
    Blue = GdiPlus.CurveChannel.CurveChannelBlue
}
#endif
