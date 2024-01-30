// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

using System.Runtime.Versioning;

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Color matrix adjustment effect.
/// </summary>
/// <remarks>
///  <para>
///   See <see href="https://learn.microsoft.com/windows/win32/gdiplus/-gdiplus-recoloring-use">Recoloring</see> for
///   examples of using a color matrix to adjust the colors of an image.
///  </para>
/// </remarks>
[RequiresPreviewFeatures]
public sealed unsafe class ColorMatrixEffect : Effect
{
    private readonly ColorMatrix _matrix;

    /// <summary>
    ///  Creates a new <see cref="ColorMatrixEffect"/> with the given <paramref name="matrix"/>.
    /// </summary>
    /// <param name="matrix">Color transform matrix.</param>
    public ColorMatrixEffect(ColorMatrix matrix) : base(PInvoke.ColorMatrixEffectGuid)
    {
        fixed (float* p = &matrix.GetPinnableReference())
        {
            PInvoke.GdipSetEffectParameters(NativeEffect, p, (uint)sizeof(GdiPlus.ColorMatrix)).ThrowIfFailed();
            GC.KeepAlive(this);
        }

        _matrix = matrix;
    }

    /// <summary>
    ///  An effect that converts an image to grayscale.
    /// </summary>
    public static ColorMatrixEffect GrayScaleEffect()
    {
        // Luminance values from ITU-R BT.470-6
        ColorMatrix matrix = new(
        [
            0.299f, 0.299f, 0.299f, 0, 0,
            0.587f, 0.587f, 0.587f, 0, 0,
            0.114f, 0.114f, 0.114f, 0, 0,
            0, 0, 0, 1, 0,
            0, 0, 0, 0, 1
        ]);

        return new ColorMatrixEffect(matrix);
    }

    /// <summary>
    ///  An effect that converts an image to sepia.
    /// </summary>
    public static ColorMatrixEffect SepiaEffect()
    {
        ColorMatrix matrix = new(
        [
            0.393f, 0.349f, 0.272f, 0, 0,
            0.769f, 0.686f, 0.534f, 0, 0,
            0.189f, 0.168f, 0.131f, 0, 0,
            0, 0, 0, 1, 0,
            0, 0, 0, 0, 1
        ]);

        return new ColorMatrixEffect(matrix);
    }

    /// <summary>
    ///  An effect that makes colors more vivid.
    /// </summary>
    public static ColorMatrixEffect VividEffect()
    {
        ColorMatrix matrix = new(
        [
            1.2f, -0.1f, -0.1f, 0, 0,
            -0.1f, 1.2f, -0.1f, 0, 0,
            -0.1f, -0.1f, 1.2f, 0, 0,
            0, 0, 0, 1, 0,
            0, 0, 0, 0, 1
        ]);

        return new ColorMatrixEffect(matrix);
    }

    /// <summary>
    ///  An effect that inverts the colors in an image.
    /// </summary>
    public static ColorMatrixEffect InvertEffect()
    {
        ColorMatrix matrix = new(
        [
            -1.0f, 0, 0, 0, 0,
            0, -1.0f, 0, 0, 0,
            0, 0, -1.0f, 0, 0,
            0, 0, 0, 1, 0,
            1, 1, 1, 1, 1
        ]);

        return new ColorMatrixEffect(matrix);
    }

    public ColorMatrix Matrix => _matrix;
}
#endif
