// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Windows.Win32.System.Ole;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

/// <summary>
///  Represents a private format used for data transfer by the drag-and-drop helpers.
/// </summary>
internal class DragDropFormat : IDisposable
{
    private short _format;
    private STGMEDIUM _medium;

    public STGMEDIUM Medium => _medium;

    /// <summary>
    ///  Initializes a new instance of the <see cref="DragDropFormat"/> class using the specified format, storage medium, and owner.
    /// </summary>
    public DragDropFormat(short format, STGMEDIUM medium, bool copyData)
    {
        _format = format;

        // Handle whether the data object or the caller owns the storage medium.
        _medium = copyData ? CopyData(format, medium) : medium;
    }

    /// <summary>
    ///  Returns a copy of the storage mediumn in this instance.
    /// </summary>
    public STGMEDIUM GetData()
    {
        return CopyData(_format, _medium);
    }

    /// <summary>
    ///  Refreshes the storage medium in this instance.
    /// </summary>
    public void RefreshData(short format, STGMEDIUM medium, bool copyData)
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
    private static STGMEDIUM CopyData(short format, STGMEDIUM mediumSource)
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

                    mediumDestination.unionmember = PInvoke.OleDuplicateData(
                        (HANDLE)mediumSource.unionmember,
                        (CLIPBOARD_FORMAT)format,
                        // Note that GMEM_DDESHARE is ignored
                        GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT);

                    if (mediumDestination.unionmember == IntPtr.Zero)
                    {
                        return default;
                    }

                    break;

                case TYMED.TYMED_ISTORAGE:
                case TYMED.TYMED_ISTREAM:

                    mediumDestination.unionmember = mediumSource.unionmember;
                    Marshal.AddRef(mediumSource.unionmember);
                    break;

                case TYMED.TYMED_NULL:
                default:

                    mediumDestination.unionmember = IntPtr.Zero;
                    break;
            }

            mediumDestination.tymed = mediumSource.tymed;
            mediumDestination.pUnkForRelease = mediumSource.pUnkForRelease;

            // If the object is non-null, perform an indirect AddRef() by requesting the IUnknown.
            if (mediumSource.pUnkForRelease is not null)
            {
                Marshal.GetIUnknownForObject(mediumSource.pUnkForRelease);
            }

            return mediumDestination;
        }
        catch
        {
            var comMedium = (Com.STGMEDIUM)mediumDestination;
            PInvoke.ReleaseStgMedium(ref comMedium);
            return default;
        }
    }

    /// <summary>
    ///  Frees the storage medium in this instance.
    /// </summary>
    private void ReleaseData()
    {
        var comMedium = (Com.STGMEDIUM)_medium;
        PInvoke.ReleaseStgMedium(ref comMedium);
        _medium.pUnkForRelease = null;
        _medium.tymed = TYMED.TYMED_NULL;
        _medium.unionmember = IntPtr.Zero;
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
