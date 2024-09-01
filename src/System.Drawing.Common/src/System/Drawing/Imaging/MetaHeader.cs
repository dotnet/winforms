// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential, Pack = 2)]
public sealed class MetaHeader
{
    // The ENHMETAHEADER structure is defined natively as a union with WmfHeader.
    // Extreme care should be taken if changing the layout of the corresponding managed
    // structures to minimize the risk of buffer overruns. The affected managed classes
    // are the following: ENHMETAHEADER, MetaHeader, MetafileHeaderWmf, MetafileHeaderEmf.
    internal METAHEADER _data;

    public MetaHeader()
    {
    }

    internal MetaHeader(METAHEADER header) => _data = header;

    /// <summary>
    ///  Represents the type of the associated <see cref='Metafile'/>.
    /// </summary>
    public short Type
    {
        get => (short)_data.mtType;
        set => _data.mtType = (ushort)value;
    }

    /// <summary>
    ///  Represents the size, in bytes, of the header file.
    /// </summary>
    public short HeaderSize
    {
        get => (short)_data.mtHeaderSize;
        set => _data.mtHeaderSize = (ushort)value;
    }

    /// <summary>
    ///  Represents the version number of the header format.
    /// </summary>
    public short Version
    {
        get => (short)_data.mtVersion;
        set => _data.mtVersion = (ushort)value;
    }

    /// <summary>
    ///  Represents the size, in bytes, of the associated <see cref='Metafile'/>.
    /// </summary>
    public int Size
    {
        get => (int)_data.mtSize;
        set => _data.mtSize = (uint)value;
    }

    public short NoObjects
    {
        get => (short)_data.mtNoObjects;
        set => _data.mtNoObjects = (ushort)value;
    }

    public int MaxRecord
    {
        get => (int)_data.mtMaxRecord;
        set => _data.mtMaxRecord = (uint)value;
    }

    public short NoParameters
    {
        get => (short)_data.mtNoParameters;
        set => _data.mtNoParameters = (ushort)value;
    }
}
