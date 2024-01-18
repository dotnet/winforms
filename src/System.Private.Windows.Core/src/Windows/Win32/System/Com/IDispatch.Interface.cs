// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Variant;
using static WinFormsComWrappers;

namespace Windows.Win32.System.Com;

internal unsafe partial struct IDispatch : IVTable<IDispatch, IDispatch.Vtbl>
{
    static void IVTable<IDispatch, Vtbl>.PopulateVTable(Vtbl* vtable)
    {
        vtable->GetTypeInfoCount_4 = &GetTypeInfoCount;
        vtable->GetTypeInfo_5 = &GetTypeInfo;
        vtable->GetIDsOfNames_6 = &GetIDsOfNames;
        vtable->Invoke_7 = &Invoke;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT GetTypeInfoCount(IDispatch* @this, uint* pctinfo)
        => UnwrapAndInvoke<IDispatch, Interface>(@this, o => o.GetTypeInfoCount(pctinfo));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT GetTypeInfo(IDispatch* @this, uint iTInfo, uint lcid, ITypeInfo** ppTInfo)
        => UnwrapAndInvoke<IDispatch, Interface>(@this, o => o.GetTypeInfo(iTInfo, lcid, ppTInfo));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT GetIDsOfNames(IDispatch* @this, Guid* riid, PWSTR* rgszNames, uint cNames, uint lcid, int* rgDispId)
        => UnwrapAndInvoke<IDispatch, Interface>(@this, o => o.GetIDsOfNames(riid, rgszNames, cNames, lcid, rgDispId));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT Invoke(
        IDispatch* @this,
        int dispIdMember,
        Guid* riid,
        uint lcid,
        DISPATCH_FLAGS dwFlags,
        DISPPARAMS* pDispParams,
        VARIANT* pVarResult,
        EXCEPINFO* pExcepInfo,
        uint* pArgErr)
        => UnwrapAndInvoke<IDispatch, Interface>(
            @this,
            o => o.Invoke(dispIdMember, riid, lcid, dwFlags, pDispParams, pVarResult, pExcepInfo, pArgErr));

    // Marking this as [ComImport] does not work with legacy COM interop. When asking for IDispatch explicitly it
    // will not look for this interface if it is applied.
    [Guid("00020400-0000-0000-C000-000000000046")]
    public unsafe interface Interface
    {
        [PreserveSig]
        HRESULT GetTypeInfoCount(
            uint* pctinfo);

        [PreserveSig]
        HRESULT GetTypeInfo(
            uint iTInfo,
            uint lcid,
            ITypeInfo** ppTInfo);

        [PreserveSig]
        HRESULT GetIDsOfNames(
            Guid* riid,
            PWSTR* rgszNames,
            uint cNames,
            uint lcid,
            int* rgDispId);

        [PreserveSig]
        HRESULT Invoke(
            int dispIdMember,
            Guid* riid,
            uint lcid,
            DISPATCH_FLAGS dwFlags,
            DISPPARAMS* pDispParams,
            VARIANT* pVarResult,
            EXCEPINFO* pExcepInfo,
            uint* pArgErr);
    }
}
