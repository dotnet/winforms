// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Applies a Gaussian blur.
/// </summary>
public unsafe class BlurEffect : Effect
{
    private readonly BlurParams _blurParams;

    /// <summary>
    ///  Creates a new <see cref="BlurEffect"/> with the given parameters.
    /// </summary>
    /// <param name="radius">
    ///  Real number that specifies the blur radius (the radius of the Gaussian convolution kernel) in pixels. The
    ///  radius must be in the range 0 through 256. As the radius increases, the resulting bitmap becomes more blurry.
    /// </param>
    /// <param name="expandEdge">
    ///  Boolean value that specifies whether the bitmap expands by an amount equal to the blur radius. If
    ///  <see langword="true"/>, the bitmap expands by an amount equal to the radius so that it can have soft edges. If
    ///  <see langword="false"/>, the bitmap remains the same size and the soft edges are clipped.
    /// </param>
    /// <exception cref="ArgumentException"><paramref name="radius"/> is less than 0 or greater than 256.</exception>
    public BlurEffect(float radius, bool expandEdge) : base(PInvoke.BlurEffectGuid)
    {
        _blurParams = new() { radius = radius, expandEdge = expandEdge };
        SetParameters(ref _blurParams);
    }

    /// <summary>
    ///  Real number that specifies the blur radius (the radius of the Gaussian convolution kernel) in pixels. The
    ///  radius must be in the range 0 through 255. As the radius increases, the resulting bitmap becomes more blurry.
    /// </summary>
    public float Radius => _blurParams.radius;

    /// <summary>
    ///  Boolean value that specifies whether the bitmap expands by an amount equal to the blur radius. If
    ///  <see langword="true"/>, the bitmap expands by an amount equal to the radius so that it can have soft edges. If
    ///  <see langword="false"/>, the bitmap remains the same size and the soft edges are clipped.
    /// </summary>
    public bool ExpandEdge => _blurParams.expandEdge;
}
#endif
