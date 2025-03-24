// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Lifetime = Windows.Win32.System.Com.Lifetime<Windows.Win32.System.Com.IDataObject.Vtbl, System.Private.Windows.Ole.DataObjectProxy>;

namespace System.Private.Windows.Ole;

/// <summary>
///  Emulates an OLE proxy for unit testing purposes.
/// </summary>
internal unsafe class DataObjectProxy : IDataObject.Interface, IDisposable
{
    // Agile for ensured cleanup
    private readonly AgileComPointer<IDataObject> _agileOriginal;
    private readonly IDataObject* _original;
    private readonly AgileComPointer<IDataObject> _agileProxy;
    public IDataObject* Proxy { get; }

    public DataObjectProxy(IDataObject* original)
    {
        // Don't track disposal, we depend on finalization for testing.

        _original = original;
        _agileOriginal = new(
#if DEBUG
            original, takeOwnership: true, trackDisposal: false
#else
            original, takeOwnership: true
#endif
            );

        Proxy = CCW.Create(this);

        _agileProxy = new(
#if DEBUG
            Proxy, takeOwnership: true, trackDisposal: false
#else
            Proxy, takeOwnership: true
#endif
            );
    }

    public HRESULT GetData(FORMATETC* pformatetcIn, STGMEDIUM* pmedium) => _original->GetData(pformatetcIn, pmedium);
    public HRESULT GetDataHere(FORMATETC* pformatetc, STGMEDIUM* pmedium) => _original->GetDataHere(pformatetc, pmedium);
    public HRESULT QueryGetData(FORMATETC* pformatetc) => _original->QueryGetData(pformatetc);
    public HRESULT GetCanonicalFormatEtc(FORMATETC* pformatectIn, FORMATETC* pformatetcOut) => _original->GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
    public HRESULT SetData(FORMATETC* pformatetc, STGMEDIUM* pmedium, BOOL fRelease) => _original->SetData(pformatetc, pmedium, fRelease);
    public HRESULT EnumFormatEtc(uint dwDirection, IEnumFORMATETC** ppenumFormatEtc) => _original->EnumFormatEtc(dwDirection, ppenumFormatEtc);
    public HRESULT DAdvise(FORMATETC* pformatetc, uint advf, IAdviseSink* pAdvSink, uint* pdwConnection) => _original->DAdvise(pformatetc, advf, pAdvSink, pdwConnection);
    public HRESULT DUnadvise(uint dwConnection) => _original->DUnadvise(dwConnection);
    public HRESULT EnumDAdvise(IEnumSTATDATA** ppenumAdvise) => _original->EnumDAdvise(ppenumAdvise);

    public void Dispose()
    {
        _agileOriginal.Dispose();
        _agileProxy.Dispose();
    }

    internal static class CCW
    {
        private static readonly IDataObject.Vtbl* s_vtable = AllocateVTable();

        private static unsafe IDataObject.Vtbl* AllocateVTable()
        {
            // Allocate and create a singular VTable for this type projection.
            IDataObject.Vtbl* vtable = (IDataObject.Vtbl*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(CCW), sizeof(IDataObject.Vtbl));

            // IUnknown
            vtable->QueryInterface_1 = &QueryInterface;
            vtable->AddRef_2 = &AddRef;
            vtable->Release_3 = &Release;
            vtable->GetData_4 = &GetData;
            vtable->GetDataHere_5 = &GetDataHere;
            vtable->QueryGetData_6 = &QueryGetData;
            vtable->GetCanonicalFormatEtc_7 = &GetCanonicalFormatEtc;
            vtable->SetData_8 = &SetData;
            vtable->EnumFormatEtc_9 = &EnumFormatEtc;
            vtable->DAdvise_10 = &DAdvise;
            vtable->DUnadvise_11 = &DUnadvise;
            vtable->EnumDAdvise_12 = &EnumDAdvise;
            return vtable;
        }

        /// <summary>
        ///  Creates a manual COM Callable Wrapper for the given <paramref name="object"/>.
        /// </summary>
        public static unsafe IDataObject* Create(DataObjectProxy @object) =>
            (IDataObject*)Lifetime.Allocate(@object, s_vtable);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT QueryInterface(IDataObject* @this, Guid* iid, void** ppObject)
        {
            if (iid is null || ppObject is null)
            {
                return HRESULT.E_POINTER;
            }

            if (iid->Equals(IDataObject.IID_Guid) || iid->Equals(IUnknown.IID_Guid))
            {
                *ppObject = @this;
                Lifetime.AddRef(@this);
                return HRESULT.S_OK;
            }

            *ppObject = null;
            DataObjectProxy? proxy = Lifetime.GetObject(@this);
            if (proxy is null)
            {
                return HRESULT.E_NOINTERFACE;
            }

            // Unwrap our "proxy" object by calling the the original object. This should roughly match the
            // OLE proxy behavior which returns it's own pointer for the IID_IUnknown and IID_IDataObject interfaces.
            return proxy._original->QueryInterface(iid, ppObject);
        }

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe uint AddRef(IDataObject* @this) => Lifetime.AddRef(@this);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe uint Release(IDataObject* @this) => Lifetime.Release(@this);

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT GetData(IDataObject* @this, FORMATETC* pFormatetc, STGMEDIUM* pMedium) =>
            Lifetime.GetObject(@this)?.GetData(pFormatetc, pMedium) ?? HRESULT.COR_E_OBJECTDISPOSED;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT GetDataHere(IDataObject* @this, FORMATETC* pFormatetc, STGMEDIUM* pMedium) =>
            Lifetime.GetObject(@this)?.GetDataHere(pFormatetc, pMedium) ?? HRESULT.COR_E_OBJECTDISPOSED;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT QueryGetData(IDataObject* @this, FORMATETC* pFormatetc) =>
            Lifetime.GetObject(@this)?.QueryGetData(pFormatetc) ?? HRESULT.COR_E_OBJECTDISPOSED;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT GetCanonicalFormatEtc(IDataObject* @this, FORMATETC* pFormatetcIn, FORMATETC* pFormatetcOut) =>
            Lifetime.GetObject(@this)?.GetCanonicalFormatEtc(pFormatetcIn, pFormatetcOut) ?? HRESULT.COR_E_OBJECTDISPOSED;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT SetData(IDataObject* @this, FORMATETC* pFormatetc, STGMEDIUM* pMedium, BOOL fRelease) =>
            Lifetime.GetObject(@this)?.SetData(pFormatetc, pMedium, fRelease) ?? HRESULT.COR_E_OBJECTDISPOSED;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT EnumFormatEtc(IDataObject* @this, uint dwDirection, IEnumFORMATETC** ppEnumFormatEtc) =>
            Lifetime.GetObject(@this)?.EnumFormatEtc(dwDirection, ppEnumFormatEtc) ?? HRESULT.COR_E_OBJECTDISPOSED;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT DAdvise(IDataObject* @this, FORMATETC* pFormatetc, uint advf, IAdviseSink* pAdvSink, uint* pdwConnection) =>
            Lifetime.GetObject(@this)?.DAdvise(pFormatetc, advf, pAdvSink, pdwConnection) ?? HRESULT.COR_E_OBJECTDISPOSED;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT DUnadvise(IDataObject* @this, uint dwConnection) =>
            Lifetime.GetObject(@this)?.DUnadvise(dwConnection) ?? HRESULT.COR_E_OBJECTDISPOSED;

        [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
        private static unsafe HRESULT EnumDAdvise(IDataObject* @this, IEnumSTATDATA** ppEnumAdvise) =>
            Lifetime.GetObject(@this)?.EnumDAdvise(ppEnumAdvise) ?? HRESULT.COR_E_OBJECTDISPOSED;
    }
}
