// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

using System.Runtime.CompilerServices;

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Allows modification of the color components of an image. Individual color component values are changed to entries
///  in a series of lookup tables.
/// </summary>
public unsafe class ColorLookupTableEffect : Effect
{
    private readonly byte[] _bytes = new byte[1024];

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
        ArgumentOutOfRangeException.ThrowIfGreaterThan(redLookupTable.Length, 256, nameof(redLookupTable));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(greenLookupTable.Length, 256, nameof(greenLookupTable));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(blueLookupTable.Length, 256, nameof(blueLookupTable));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(alphaLookupTable.Length, 256, nameof(alphaLookupTable));

        Span<byte> bytes = _bytes;
        blueLookupTable.CopyTo(bytes);
        greenLookupTable.CopyTo(bytes[256..]);
        redLookupTable.CopyTo(bytes[512..]);
        alphaLookupTable.CopyTo(bytes[768..]);

        fixed (byte* b = _bytes)
        {
            SetParameters(ref Unsafe.AsRef<ColorLUTParams>(b));
        }
    }

    /// <summary>
    ///  The lookup table for the blue channel.
    /// </summary>
    public ReadOnlyMemory<byte> BlueLookupTable => new(_bytes, 0, 256);

    /// <summary>
    ///  The lookup table for the green channel.
    /// </summary>
    public ReadOnlyMemory<byte> GreenLookupTable => new(_bytes, 256, 256);

    /// <summary>
    ///  The lookup table for the red channel.
    /// </summary>
    public ReadOnlyMemory<byte> RedLookupTable => new(_bytes, 512, 256);

    /// <summary>
    ///  The lookup table for the alpha channel.
    /// </summary>
    public ReadOnlyMemory<byte> AlphaLookupTable => new(_bytes, 768, 256);
}
#endif
