// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

internal unsafe class HGlobalNativeDataObject : NativeDataObjectMock
{
    private readonly Stream _stream;
    private readonly ushort _format;

    public HGlobalNativeDataObject(Stream stream, ushort format)
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

        if (pformatetc->tymed != (uint)TYMED.TYMED_HGLOBAL)
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

        result = _stream.SaveStreamToHGLOBAL(ref pmedium->hGlobal);
        if (result.Failed)
        {
            return result;
        }

        pmedium->tymed = TYMED.TYMED_HGLOBAL;
        return HRESULT.S_OK;
    }

    protected override void Dispose(bool disposing) => _stream.Dispose();
}
