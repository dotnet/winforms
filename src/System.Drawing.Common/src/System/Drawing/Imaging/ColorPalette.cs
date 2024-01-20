// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging;

/// <summary>
///  Defines an array of colors that make up a color palette.
/// </summary>
public unsafe sealed class ColorPalette
{
    private int _flags;
    private Color[] _entries;
    /// <summary>
    ///  Specifies how to interpret the color information in the array of colors.
    /// </summary>
    public int Flags => _flags;

    /// <summary>
    ///  Specifies an array of <see cref='Color'/> objects.
    /// </summary>
    public Color[] Entries => _entries;

    internal ColorPalette(int count) => _entries = new Color[count];

    internal ColorPalette() => _entries = new Color[1];

    private ColorPalette(int flags, Color[] entries)
    {
        _flags = flags;
        _entries = entries;
    }

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
}
