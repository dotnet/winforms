// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Effect that converts an image to grayscale.
/// </summary>
public sealed class GrayScaleEffect : ColorMatrixEffect
{
    /// <summary>
    ///  Creates a <see cref="ColorMatrixEffect"/> that converts an image to grayscale.
    /// </summary>
    public GrayScaleEffect() : base(
        new(
        [
            0.299f, 0.299f, 0.299f, 0, 0,
            0.587f, 0.587f, 0.587f, 0, 0,
            0.114f, 0.114f, 0.114f, 0, 0,
            0, 0, 0, 1, 0,
            0, 0, 0, 0, 1
        ]))
    { }
}
#endif
