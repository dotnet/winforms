// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Allows modification of the color components of an image. Individual color component values are changed to entries
///  in a series of lookup tables.
/// </summary>
[RequiresPreviewFeatures]
public sealed unsafe class ColorLookupTableEffect : Effect
{
    private readonly ColorLUTParams _parameters;

    /// <summary>
    ///  Creates a new <see cref="ColorLookupTableEffect"/> with the given parameters.
    /// </summary>
    /// <param name="redLookupTable">The lookup table for the red channel.</param>
    /// <param name="greenLookupTable">The lookup table for the green channel.</param>
    /// <param name="blueLookupTable">The lookup table for the blue channel.</param>
    /// <param name="alphaLookupTable">The lookup table for the alpha channel.</param>
    /// <exception cref="ArgumentException">A lookup table parameter is longer than 256 bytes.</exception>
    public ColorLookupTableEffect(
        byte[] redLookupTable,
        byte[] greenLookupTable,
        byte[] blueLookupTable,
        byte[] alphaLookupTable) : this(redLookupTable.AsSpan(), greenLookupTable, blueLookupTable, alphaLookupTable)
    {
    }

    /// <inheritdoc cref="ColorLookupTableEffect(byte[], byte[], byte[], byte[])"/>
    public ColorLookupTableEffect(
        ReadOnlySpan<byte> redLookupTable,
        ReadOnlySpan<byte> greenLookupTable,
        ReadOnlySpan<byte> blueLookupTable,
        ReadOnlySpan<byte> alphaLookupTable) : base(PInvoke.ColorLUTEffectGuid)
    {
        // ColorLUTParams will validate that the length fits.

        Unsafe.SkipInit(out _parameters);
        _parameters.lutR = redLookupTable;
        _parameters.lutG = greenLookupTable;
        _parameters.lutB = blueLookupTable;
        _parameters.lutA = alphaLookupTable;
    }

    /// <summary>
    ///  The lookup table for the red channel.
    /// </summary>
    public ReadOnlySpan<byte> RedLookupTable => _parameters.lutR.AsReadOnlySpan();

    /// <summary>
    ///  The lookup table for the green channel.
    /// </summary>
    public ReadOnlySpan<byte> GreenLookupTable => _parameters.lutG.AsReadOnlySpan();

    /// <summary>
    ///  The lookup table for the blue channel.
    /// </summary>
    public ReadOnlySpan<byte> BlueLookupTable => _parameters.lutB.AsReadOnlySpan();

    /// <summary>
    ///  The lookup table for the alpha channel.
    /// </summary>
    public ReadOnlySpan<byte> AlphaLookupTable => _parameters.lutA.AsReadOnlySpan();
}
#endif
