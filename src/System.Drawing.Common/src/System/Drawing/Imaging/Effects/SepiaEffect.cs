// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Effect that converts an image to sepia.
/// </summary>
public sealed class SepiaEffect : ColorMatrixEffect
{
    /// <summary>
    ///  Creates a new <see cref="ColorMatrixEffect"/> that converts an image to sepia.
    /// </summary>
    public SepiaEffect() : base(
        new(
        [
            0.393f, 0.349f, 0.272f, 0, 0,
            0.769f, 0.686f, 0.534f, 0, 0,
            0.189f, 0.168f, 0.131f, 0, 0,
            0, 0, 0, 1, 0,
            0, 0, 0, 0, 1
        ]))
    { }
}
#endif
