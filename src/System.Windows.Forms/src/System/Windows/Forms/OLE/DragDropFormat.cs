// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Com;

namespace System.Windows.Forms;

/// <summary>
///  Represents a private format used for data transfer by the drag-and-drop helpers.
/// </summary>
internal class DragDropFormat : IDisposable
{
    private ushort _format;
    private STGMEDIUM _medium;

    public STGMEDIUM Medium => _medium;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DragDropFormat"/> class using the specified format, storage medium, and owner.
    /// </summary>
    public DragDropFormat(ushort format, STGMEDIUM medium, bool copyData)
    {
        _format = format;

        // Handle whether the data object or the caller owns the storage medium.
        _medium = copyData ? CopyData(format, medium) : medium;
    }

    /// <summary>
    ///  Returns a copy of the storage medium in this instance.
    /// </summary>
    public STGMEDIUM GetData() => CopyData(_format, _medium);

    /// <summary>
    ///  Refreshes the storage medium in this instance.
    /// </summary>
    public void RefreshData(ushort format, STGMEDIUM medium, bool copyData)
    {
        ReleaseData();
        _format = format;

        // Handle whether the data object or the caller owns the storage medium.
        _medium = copyData ? CopyData(format, medium) : medium;
    }

    /// <summary>
    ///  Copies a given storage medium.
    /// </summary>
    /// <returns>
    ///  A copy of <paramref name="mediumSource"/>.
    /// </returns>
    private static unsafe STGMEDIUM CopyData(ushort format, STGMEDIUM mediumSource)
    {
        STGMEDIUM mediumDestination = default;

        try
        {
            switch (mediumSource.tymed)
            {
                case TYMED.TYMED_HGLOBAL:
                case TYMED.TYMED_FILE:
                case TYMED.TYMED_ENHMF:
                case TYMED.TYMED_GDI:
                case TYMED.TYMED_MFPICT:

                    mediumDestination.hGlobal = (HGLOBAL)(nint)PInvoke.OleDuplicateData(
                        (HANDLE)(nint)mediumSource.hGlobal,
                        (CLIPBOARD_FORMAT)format,
                        // Note that GMEM_DDESHARE is ignored
                        GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT);

                    if (mediumDestination.hGlobal.IsNull)
                    {
                        return default;
                    }

                    break;

                case TYMED.TYMED_ISTORAGE:
                case TYMED.TYMED_ISTREAM:

                    mediumDestination.hGlobal = mediumSource.hGlobal;
                    Marshal.AddRef(mediumSource.hGlobal);
                    break;

                case TYMED.TYMED_NULL:
                default:

                    mediumDestination.hGlobal = HGLOBAL.Null;
                    break;
            }

            mediumDestination.tymed = mediumSource.tymed;
            mediumDestination.pUnkForRelease = mediumSource.pUnkForRelease;

            if (mediumSource.pUnkForRelease is not null)
            {
                mediumSource.pUnkForRelease->AddRef();
            }

            return mediumDestination;
        }
        catch
        {
            PInvoke.ReleaseStgMedium(ref mediumDestination);
            return default;
        }
    }

    /// <summary>
    ///  Frees the storage medium in this instance.
    /// </summary>
    private unsafe void ReleaseData()
    {
        PInvoke.ReleaseStgMedium(ref _medium);
        _medium.pUnkForRelease = null;
        _medium.tymed = TYMED.TYMED_NULL;
        _medium.hGlobal = HGLOBAL.Null;
    }

    public void Dispose()
    {
        ReleaseData();
        GC.SuppressFinalize(this);
    }

    ~DragDropFormat()
    {
        Dispose();
    }
}
