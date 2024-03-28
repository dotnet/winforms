// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

/// <summary>
///  Defines an Placeable Metafile.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public sealed class WmfPlaceableFileHeader
{
    internal GdiPlus.WmfPlaceableFileHeader _header;

    public WmfPlaceableFileHeader() => _header.Key = 0x9aC6CDD7;

    /// <summary>
    ///  Indicates the presence of a placeable metafile header.
    /// </summary>
    public int Key
    {
        get => (int)_header.Key;
        set => _header.Key = (uint)value;
    }

    /// <summary>
    ///  Stores the handle of the metafile in memory.
    /// </summary>
    public short Hmf
    {
        get => _header.Hmf;
        set => _header.Hmf = value;
    }

    /// <summary>
    ///  The x-coordinate of the upper-left corner of the bounding rectangle of the metafile image on the output device.
    /// </summary>
    public short BboxLeft
    {
        get => _header.BoundingBox.Left;
        set => _header.BoundingBox.Left = value;
    }

    /// <summary>
    ///  The y-coordinate of the upper-left corner of the bounding rectangle of the metafile image on the output device.
    /// </summary>
    public short BboxTop
    {
        get => _header.BoundingBox.Top;
        set => _header.BoundingBox.Top = value;
    }

    /// <summary>
    ///  The x-coordinate of the lower-right corner of the bounding rectangle of the metafile image on the output device.
    /// </summary>
    public short BboxRight
    {
        get => _header.BoundingBox.Right;
        set => _header.BoundingBox.Right = value;
    }

    /// <summary>
    ///  The y-coordinate of the lower-right corner of the bounding rectangle of the metafile image on the output device.
    /// </summary>
    public short BboxBottom
    {
        get => _header.BoundingBox.Bottom;
        set => _header.BoundingBox.Bottom = value;
    }

    /// <summary>
    ///  Indicates the number of twips per inch.
    /// </summary>
    public short Inch
    {
        get => _header.Inch;
        set => _header.Inch = value;
    }

    /// <summary>
    ///  Reserved. Do not use.
    /// </summary>
    public int Reserved
    {
        get => (int)_header.Reserved;
        set => _header.Reserved = (uint)value;
    }

    /// <summary>
    ///  Indicates the checksum value for the previous ten WORDs in the header.
    /// </summary>
    public short Checksum
    {
        get => _header.Checksum;
        set => _header.Checksum = value;
    }
}
