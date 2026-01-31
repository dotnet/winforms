// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

/// <summary>
///  A native data object mock that returns success from QueryGetData but failure from GetData.
///  This simulates a race condition where the clipboard changes between QueryGetData and GetData.
/// </summary>
internal unsafe class FailingGetDataNativeDataObject : NativeDataObjectMock
{
    private readonly ushort _format;
    private readonly HRESULT _failureHResult;

    public FailingGetDataNativeDataObject(ushort format, HRESULT failureHResult = default)
    {
        _format = format;

        // Default to CLIPBRD_E_CANT_OPEN which can happen during clipboard contention.
        _failureHResult = failureHResult == default ? HRESULT.CLIPBRD_E_CANT_OPEN : failureHResult;
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

        // QueryGetData succeeds - the data appears to be available
        return HRESULT.S_OK;
    }

    public override HRESULT GetData(FORMATETC* pformatetcIn, STGMEDIUM* pmedium)
    {
        // But GetData fails - simulating clipboard contention or the data being removed
        // between QueryGetData and GetData calls
        return _failureHResult;
    }

    protected override void Dispose(bool disposing) { }
}
