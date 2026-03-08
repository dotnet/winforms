// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

/// <summary>
///  A native data object mock that returns data via <see cref="TYMED.TYMED_ISTREAM"/>.
/// </summary>
internal unsafe class IStreamNativeDataObject : NativeDataObjectMock
{
    private readonly Stream _stream;
    private readonly ushort _format;

    public IStreamNativeDataObject(Stream stream, ushort format)
    {
        _stream = stream;
        _format = format;
    }

    public override HRESULT QueryGetData(FORMATETC* pformatetc)
    {
        if (pformatetc is null)
        {
            return HRESULT.DV_E_FORMATETC;
        }

        if (pformatetc->cfFormat != _format)
        {
            return HRESULT.DV_E_FORMATETC;
        }

        if (pformatetc->dwAspect != (uint)DVASPECT.DVASPECT_CONTENT)
        {
            return HRESULT.DV_E_DVASPECT;
        }

        if (pformatetc->lindex != -1)
        {
            return HRESULT.DV_E_LINDEX;
        }

        if (pformatetc->tymed != (uint)TYMED.TYMED_ISTREAM)
        {
            return HRESULT.DV_E_TYMED;
        }

        return HRESULT.S_OK;
    }

    public override HRESULT GetData(FORMATETC* pformatetcIn, STGMEDIUM* pmedium)
    {
        HRESULT result = QueryGetData(pformatetcIn);
        if (result.Failed)
        {
            return result;
        }

        if (pmedium is null)
        {
            return HRESULT.E_POINTER;
        }

        // Reset stream position for each GetData call
        _stream.Position = 0;

        // Create a ComManagedStream wrapper
        ComManagedStream comStream = new(_stream);

        // Return the IStream pointer in the STGMEDIUM
        // Note: hGlobal is a union with pstm in STGMEDIUM
        pmedium->hGlobal = (HGLOBAL)(nint)ComHelpers.GetComPointer<IStream>(comStream);
        pmedium->tymed = TYMED.TYMED_ISTREAM;
        pmedium->pUnkForRelease = null;

        return HRESULT.S_OK;
    }

    protected override void Dispose(bool disposing) => _stream.Dispose();
}
