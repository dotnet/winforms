// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Effect that inverts the colors in an image.
/// </summary>
public sealed class InvertEffect : ColorMatrixEffect
{
    /// <summary>
    ///  Creates a <see cref="ColorMatrixEffect"/> that inverts the colors in an image.
    /// </summary>
    public InvertEffect() : base(
        new(
        [
            -1.0f, 0, 0, 0, 0,
            0, -1.0f, 0, 0, 0,
            0, 0, -1.0f, 0, 0,
            0, 0, 0, 1, 0,
            1, 1, 1, 1, 1
        ]))
    { }
}
#endif
