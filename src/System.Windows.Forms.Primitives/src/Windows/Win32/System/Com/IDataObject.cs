// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ComWrappers = Interop.WinFormsComWrappers;

namespace Windows.Win32.System.Com;
internal unsafe partial struct IDataObject : IVTable<IDataObject, IDataObject.Vtbl>
{
    static void IVTable<IDataObject, Vtbl>.PopulateComInterfaceVTable(Vtbl* vtable)
    {
        vtable->GetData_4 = &GetData;
        vtable->GetDataHere_5 = &GetDataHere;
        vtable->QueryGetData_6 = &QueryGetData;
        vtable->GetCanonicalFormatEtc_7 = &GetCanonicalFormatEtc;
        vtable->SetData_8 = &SetData;
        vtable->EnumFormatEtc_9 = &EnumFormatEtc;
        vtable->DAdvise_10 = &DAdvise;
        vtable->DUnadvise_11 = &DUnadvise;
        vtable->EnumDAdvise_12 = &EnumDAdvise;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT GetData(IDataObject* @this, FORMATETC* format, STGMEDIUM* pMedium)
        => ComWrappers.UnwrapAndInvoke<IDataObject, Interface>(@this, o => o.GetData(format, pMedium));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe HRESULT GetDataHere(IDataObject* @this, FORMATETC* format, STGMEDIUM* pMedium)
        => ComWrappers.UnwrapAndInvoke<IDataObject, Interface>(@this, o => o.GetDataHere(format, pMedium));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe HRESULT QueryGetData(IDataObject* @this, FORMATETC* format)
        => ComWrappers.UnwrapAndInvoke<IDataObject, Interface>(@this, o => o.QueryGetData(format));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe HRESULT GetCanonicalFormatEtc(IDataObject* @this, FORMATETC* formatIn, FORMATETC* formatOut)
        => ComWrappers.UnwrapAndInvoke<IDataObject, Interface>(@this, o => o.GetCanonicalFormatEtc(formatIn, formatOut));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT SetData(IDataObject* @this, FORMATETC* format, STGMEDIUM* pMedium, BOOL release)
        => ComWrappers.UnwrapAndInvoke<IDataObject, Interface>(@this, o => o.SetData(format, pMedium, release));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT EnumFormatEtc(IDataObject* @this, uint direction, IEnumFORMATETC** pEnumFormatC)
        => ComWrappers.UnwrapAndInvoke<IDataObject, Interface>(@this, o => o.EnumFormatEtc(direction, pEnumFormatC));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe HRESULT DAdvise(IDataObject* @this, FORMATETC* pFormatetc, uint advf, IAdviseSink* pAdviseSink, uint* connection)
        => ComWrappers.UnwrapAndInvoke<IDataObject, Interface>(@this, o => o.DAdvise(pFormatetc, advf, pAdviseSink, connection));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe HRESULT DUnadvise(IDataObject* @this, uint connection)
        => ComWrappers.UnwrapAndInvoke<IDataObject, Interface>(@this, o => o.DUnadvise(connection));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static unsafe HRESULT EnumDAdvise(IDataObject* @this, IEnumSTATDATA** pEnumAdvise)
        => ComWrappers.UnwrapAndInvoke<IDataObject, Interface>(@this, o => o.EnumDAdvise(pEnumAdvise));
}
