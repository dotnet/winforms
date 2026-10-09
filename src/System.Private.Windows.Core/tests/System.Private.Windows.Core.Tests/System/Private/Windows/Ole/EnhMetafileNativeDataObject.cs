// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Private.Windows.Ole;

internal sealed unsafe class EnhMetafileNativeDataObject : NativeDataObjectMock
{
    private readonly List<nint> _returnedHandles = [];
    private readonly HRESULT _getDataResult;

    public EnhMetafileNativeDataObject(HRESULT getDataResult = default) =>
        _getDataResult = getDataResult == default ? HRESULT.S_OK : getDataResult;

    public List<(string Format, TYMED Tymed)> QueryRequests { get; } = [];

    public IReadOnlyList<nint> ReturnedHandles => _returnedHandles;

    public int GetDataCallCount { get; private set; }

    public override HRESULT QueryGetData(FORMATETC* pformatetc)
    {
        if (pformatetc is null)
        {
            return HRESULT.DV_E_FORMATETC;
        }

        string format = DataFormatsCore<TestFormat>.GetOrAddFormat(pformatetc->cfFormat).Name;
        TYMED tymed = (TYMED)pformatetc->tymed;
        QueryRequests.Add((format, tymed));

        if (pformatetc->cfFormat != (ushort)CLIPBOARD_FORMAT.CF_ENHMETAFILE)
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

        return tymed.HasFlag(TYMED.TYMED_ENHMF) ? HRESULT.S_OK : HRESULT.DV_E_TYMED;
    }

    public override HRESULT GetData(FORMATETC* pformatetcIn, STGMEDIUM* pmedium)
    {
        GetDataCallCount++;

        if (pmedium is null)
        {
            return HRESULT.E_POINTER;
        }

        *pmedium = default;
        if (pformatetcIn is null
            || pformatetcIn->cfFormat != (ushort)CLIPBOARD_FORMAT.CF_ENHMETAFILE
            || pformatetcIn->dwAspect != (uint)DVASPECT.DVASPECT_CONTENT
            || pformatetcIn->lindex != -1
            || (TYMED)pformatetcIn->tymed != TYMED.TYMED_ENHMF)
        {
            return HRESULT.DV_E_FORMATETC;
        }

        if (_getDataResult.Failed)
        {
            return _getDataResult;
        }

        nint handle = EmfNativeMethods.CreateDeterministicEnhMetafile();

        // IDataObject::GetData transfers this handle through STGMEDIUM; track it only to clean up failed test paths.
        _returnedHandles.Add(handle);
        pmedium->tymed = TYMED.TYMED_ENHMF;
        pmedium->hGlobal = (HGLOBAL)handle;

        return HRESULT.S_OK;
    }

    protected override void Dispose(bool disposing)
    {
        foreach (nint handle in _returnedHandles)
        {
            if (EmfNativeMethods.IsValid(handle))
            {
                EmfNativeMethods.Delete(handle).Should().BeTrue();
            }
        }
    }
}
