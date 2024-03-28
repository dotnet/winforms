// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

/// <summary>
///  Contains attributes of an associated <see cref='Metafile'/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public sealed class MetafileHeader
{
    internal GdiPlus.MetafileHeader _header;

    internal MetafileHeader()
    {
    }

    /// <summary>
    ///  Gets the type of the associated <see cref='Metafile'/>.
    /// </summary>
    public MetafileType Type => (MetafileType)_header.Type;

    /// <summary>
    ///  Gets the size, in bytes, of the associated <see cref='Metafile'/>.
    /// </summary>
    public int MetafileSize => (int)_header.Size;

    /// <summary>
    ///  Gets the version number of the associated <see cref='Metafile'/>.
    /// </summary>
    public int Version => (int)_header.Version;

    /// <summary>
    ///  Gets the horizontal resolution, in dots-per-inch, of the associated <see cref='Metafile'/>.
    /// </summary>
    public float DpiX => _header.DpiX;

    /// <summary>
    ///  Gets the vertical resolution, in dots-per-inch, of the associated <see cref='Metafile'/>.
    /// </summary>
    public float DpiY => _header.DpiY;

    /// <summary>
    ///  Gets a <see cref='Rectangle'/> that bounds the associated <see cref='Metafile'/>.
    /// </summary>
    public Rectangle Bounds => new(_header.X, _header.Y, _header.Width, _header.Height);

    /// <summary>
    ///  Returns a value indicating whether the associated <see cref='Metafile'/> is in the Windows metafile format.
    /// </summary>
    public bool IsWmf() => _header.Type is GdiPlus.MetafileType.MetafileTypeWmf or GdiPlus.MetafileType.MetafileTypeWmfPlaceable;

    /// <summary>
    ///  Returns a value indicating whether the associated <see cref='Metafile'/> is in the Windows Placeable metafile format.
    /// </summary>
    public bool IsWmfPlaceable() => _header.Type is GdiPlus.MetafileType.MetafileTypeWmfPlaceable;

    /// <summary>
    ///  Returns a value indicating whether the associated <see cref='Metafile'/> is in the Windows enhanced metafile format.
    /// </summary>
    public bool IsEmf() => _header.Type is GdiPlus.MetafileType.MetafileTypeEmf;

    /// <summary>
    ///  Returns a value indicating whether the associated <see cref='Metafile'/> is in the Windows enhanced
    ///  metafile format or the Windows enhanced metafile plus.
    /// </summary>
    public bool IsEmfOrEmfPlus() => _header.Type is GdiPlus.MetafileType.MetafileTypeEmf
        or GdiPlus.MetafileType.MetafileTypeEmfPlusOnly
        or GdiPlus.MetafileType.MetafileTypeEmfPlusDual;

    /// <summary>
    ///  Returns a value indicating whether the associated <see cref='Metafile'/> is in the Windows enhanced
    ///  metafile plus format.
    /// </summary>
    public bool IsEmfPlus() => _header.Type is GdiPlus.MetafileType.MetafileTypeEmfPlusOnly
        or GdiPlus.MetafileType.MetafileTypeEmfPlusDual;

    /// <summary>
    ///  Returns a value indicating whether the associated <see cref='Metafile'/> is in the Dual enhanced metafile
    ///  format. This format supports both the enhanced and the enhanced plus format.
    /// </summary>
    public bool IsEmfPlusDual() => _header.Type is GdiPlus.MetafileType.MetafileTypeEmfPlusDual;

    /// <summary>
    ///  Returns a value indicating whether the associated <see cref='Metafile'/> supports only the Windows
    ///  enhanced metafile plus format.
    /// </summary>
    public bool IsEmfPlusOnly() => _header.Type is GdiPlus.MetafileType.MetafileTypeEmfPlusOnly;

    /// <summary>
    ///  Returns a value indicating whether the associated <see cref='Metafile'/> is device-dependent.
    /// </summary>
    public bool IsDisplay() => IsEmfPlus() && ((EmfPlusFlags)_header.EmfPlusFlags).HasFlag(EmfPlusFlags.Display);

    /// <summary>
    ///  Gets the WMF header file for the associated <see cref='Metafile'/>.
    /// </summary>
    public MetaHeader WmfHeader => !IsWmf()
        ? throw Status.InvalidParameter.GetException()
        : new(_header.Anonymous.WmfHeader);

    /// <summary>
    ///  Gets the size, in bytes, of the enhanced metafile plus header file.
    /// </summary>
    public int EmfPlusHeaderSize => _header.EmfPlusHeaderSize;

    /// <summary>
    ///  Gets the logical horizontal resolution, in dots-per-inch, of the associated <see cref='Metafile'/>.
    /// </summary>
    public int LogicalDpiX => _header.LogicalDpiX;

    /// <summary>
    ///  Gets the logical vertical resolution, in dots-per-inch, of the associated <see cref='Metafile'/>.
    /// </summary>
    public int LogicalDpiY => _header.LogicalDpiY;
}
