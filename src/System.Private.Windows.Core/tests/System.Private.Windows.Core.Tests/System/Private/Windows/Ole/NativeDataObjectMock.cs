// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Foundation;
using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

internal abstract unsafe class NativeDataObjectMock : DisposableBase, IDataObject.Interface, IManagedWrapper<IDataObject>
{
    public virtual HRESULT GetData(FORMATETC* pformatetcIn, STGMEDIUM* pmedium) => throw new NotImplementedException();
    public virtual HRESULT GetDataHere(FORMATETC* pformatetc, STGMEDIUM* pmedium) => throw new NotImplementedException();
    public virtual HRESULT QueryGetData(FORMATETC* pformatetc) => throw new NotImplementedException();
    public virtual HRESULT GetCanonicalFormatEtc(FORMATETC* pformatectIn, FORMATETC* pformatetcOut) => throw new NotImplementedException();
    public virtual HRESULT SetData(FORMATETC* pformatetc, STGMEDIUM* pmedium, BOOL fRelease) => throw new NotImplementedException();
    public virtual HRESULT EnumFormatEtc(uint dwDirection, IEnumFORMATETC** ppenumFormatEtc) => throw new NotImplementedException();
    public virtual HRESULT DAdvise(FORMATETC* pformatetc, uint advf, IAdviseSink* pAdvSink, uint* pdwConnection) => throw new NotImplementedException();
    public virtual HRESULT DUnadvise(uint dwConnection) => throw new NotImplementedException();
    public virtual HRESULT EnumDAdvise(IEnumSTATDATA** ppenumAdvise) => throw new NotImplementedException();
}
