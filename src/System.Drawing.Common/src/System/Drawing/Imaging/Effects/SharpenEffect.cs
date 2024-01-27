﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

using System.Runtime.Versioning;

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Sharpens an image.
/// </summary>
[RequiresPreviewFeatures]
public sealed unsafe class SharpenEffect : Effect
{
    private readonly SharpenParams _sharpenParams;

    /// <summary>
    ///  Initializes a new instance of the <see cref="SharpenEffect"/> class.
    /// </summary>
    /// <param name="radius">
    ///  Real number that specifies the sharpening radius (the radius of the convolution kernel) in pixels. The radius
    ///  must be in the range 0 through 255. As the radius increases, more surrounding pixels are involved in calculating
    ///  the new value of a given pixel.
    /// </param>
    /// <param name="amount">
    ///  Real number in the range 0 through 100 that specifies the amount of sharpening to be applied. A value of 0
    ///  specifies no sharpening. As the value of amount increases, the sharpness increases.
    /// </param>
    public SharpenEffect(float radius, float amount) : base(PInvoke.SharpenEffectGuid)
    {
        _sharpenParams = new() { radius = radius, amount = amount };
        SetParameters(ref _sharpenParams);
    }

    /// <summary>
    ///  Real number that specifies the sharpening radius (the radius of the convolution kernel) in pixels. The radius
    ///  must be in the range 0 through 255. As the radius increases, more surrounding pixels are involved in calculating
    ///  the new value of a given pixel.
    /// </summary>
    public float Radius => _sharpenParams.radius;

    /// <summary>
    ///  Real number in the range 0 through 100 that specifies the amount of sharpening to be applied. A value of 0
    ///  specifies no sharpening. As the value of amount increases, the sharpness increases.
    /// </summary>
    public float Amount => _sharpenParams.amount;
}
#endif
