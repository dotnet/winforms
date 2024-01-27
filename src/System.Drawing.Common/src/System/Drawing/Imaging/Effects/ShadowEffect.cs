﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

using System.Runtime.Versioning;

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Increases or decreases the value of a color channel if that channel already has a value that is below half
///  intensity. You can use this effect to get more definition in the dark areas of an image without affecting
///  the light areas.
/// </summary>
[RequiresPreviewFeatures]
public sealed class ShadowEffect : ColorCurveEffect
{
    /// <summary>
    ///  Creates a new <see cref="ShadowEffect"/> with the given <paramref name="adjustValue"/>.
    /// </summary>
    /// <param name="channel">The channel or channels that the effect is applied to.</param>
    /// <param name="adjustValue">
    ///  A value in the range of -100 through 100. A value of 0 specifies no change. Positive values specify that the
    ///  dark areas are made lighter, and negative values specify that the dark areas are made darker.
    /// </param>
    public ShadowEffect(CurveChannel channel, int adjustValue)
        : base(CurveAdjustments.AdjustShadow, channel, adjustValue)
    {
    }
}
#endif
