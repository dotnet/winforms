// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging;

/// <summary>
///  Defines an array of colors that make up a color palette.
/// </summary>
public sealed unsafe class ColorPalette
{
    private readonly int _flags;
    private readonly Color[] _entries;

    // XmlSerializer requires a public constructor with no parameters.
    private ColorPalette() => _entries = new Color[1];

    /// <summary>
    ///  Specifies how to interpret the color information in the array of colors.
    /// </summary>
    public int Flags => _flags;

    /// <summary>
    ///  Specifies an array of <see cref='Color'/> objects.
    /// </summary>
    public Color[] Entries => _entries;

    private ColorPalette(int flags, Color[] entries)
    {
        _flags = flags;
        _entries = entries;
    }

#if NET9_0_OR_GREATER
    /// <summary>
    ///  Create a custom color palette.
    /// </summary>
    /// <param name="customColors">Color entries for the palette.</param>
    public ColorPalette(params Color[] customColors) : this(0, customColors)
    {
    }

    /// <summary>
    ///  Create a standard color palette.
    /// </summary>
    /// <param name="fixedPaletteType">The palette type.</param>
    public ColorPalette(PaletteType fixedPaletteType)
    {
        ColorPalette palette = InitializePalette(fixedPaletteType, 0, useTransparentColor: false, bitmap: null);
        _flags = palette.Flags;
        _entries = palette.Entries;
    }

    /// <summary>
    ///  Create an optimal color palette based on the colors in a given bitmap.
    /// </summary>
    /// <inheritdoc cref="InitializePalette(PaletteType, int, bool, IPointer{GpBitmap}?)"/>
    public static ColorPalette CreateOptimalPalette(int colors, bool useTransparentColor, Bitmap bitmap) =>
        InitializePalette((PaletteType)GdiPlus.PaletteType.PaletteTypeOptimal, colors, useTransparentColor, bitmap);
#endif

    // Memory layout is:
    //    UINT Flags
    //    UINT Count
    //    ARGB Entries[size]

    /// <summary>
    ///  Converts a native <see cref="GdiPlus.ColorPalette"/> buffer.
    /// </summary>
    internal static ColorPalette ConvertFromBuffer(ReadOnlySpan<uint> buffer) =>
        new((int)buffer[0], ARGB.ToColorArray(buffer.Slice(2, (int)buffer[1])));

    internal BufferScope<uint> ConvertToBuffer()
    {
        BufferScope<uint> buffer = new(Entries.Length + 2);
        buffer[0] = (uint)Flags;
        buffer[1] = (uint)Entries.Length;

        for (int i = 0; i < Entries.Length; i++)
        {
            buffer[i + 2] = (ARGB)Entries[i];
        }

        return buffer;
    }

#if NET9_0_OR_GREATER
    /// <summary>
    ///  Initializes a standard, optimal, or custom color palette.
    /// </summary>
    /// <param name="fixedPaletteType">The palette type.</param>
    /// <param name="colorCount">
    ///  The number of colors you want to have in an optimal palette based on a the specified bitmap.
    /// </param>
    /// <param name="useTransparentColor"><see langword="true"/> to include the transparent color in the palette.</param>
    internal static ColorPalette InitializePalette(
        PaletteType fixedPaletteType,
        int colorCount,
        bool useTransparentColor,
        IPointer<GpBitmap>? bitmap)
    {
        // Reserve the largest possible buffer for the palette.
        using BufferScope<uint> buffer = new(256 + sizeof(GdiPlus.ColorPalette) / sizeof(uint));
        buffer[1] = 256;
        fixed (void* b = buffer)
        {
            PInvokeGdiPlus.GdipInitializePalette(
                (GdiPlus.ColorPalette*)b,
                (GdiPlus.PaletteType)fixedPaletteType,
                colorCount,
                useTransparentColor,
                bitmap is null ? null : bitmap.GetPointer()).ThrowIfFailed();
        }

        GC.KeepAlive(bitmap);
        return ConvertFromBuffer(buffer);
    }
#endif
}
