// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Effect that makes colors more vivid.
/// </summary>
public sealed class VividEffect : ColorMatrixEffect
{
    /// <summary>
    ///  Creates a new <see cref="ColorMatrixEffect"/> that makes colors more vivid.
    /// </summary>
    public VividEffect() : base(
        new(
        [
            1.2f, -0.1f, -0.1f, 0, 0,
            -0.1f, 1.2f, -0.1f, 0, 0,
            -0.1f, -0.1f, 1.2f, 0, 0,
            0, 0, 0, 1, 0,
            0, 0, 0, 0, 1
        ]))
    { }
}
#endif
