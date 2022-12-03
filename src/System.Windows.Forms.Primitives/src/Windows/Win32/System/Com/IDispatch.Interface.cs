// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ComWrappers = Interop.WinFormsComWrappers;

namespace Windows.Win32.System.Com;

internal unsafe partial struct IDispatch : IVTable<IDispatch, IDispatch.Vtbl>
{
    static void IVTable<IDispatch, Vtbl>.PopulateComInterfaceVTable(Vtbl* vtable)
    {
        throw new NotImplementedException();
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT GetTypeInfoCount(IDispatch* @this, uint* pctinfo)
        => ComWrappers.UnwrapAndInvoke<IDispatch, Interface>(@this, o => o.GetTypeInfoCount(pctinfo));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT GetTypeInfo(IDispatch* @this, uint iTInfo, PInvoke.LCID lcid, ITypeInfo** ppTInfo)
        => ComWrappers.UnwrapAndInvoke<IDispatch, Interface>(@this, o => o.GetTypeInfo(iTInfo, lcid, ppTInfo));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT GetIDsOfNames(IDispatch* @this, Guid* riid, uint cNames, PWSTR* lcid, int* rgDispId)
        => ComWrappers.UnwrapAndInvoke<IDispatch, Interface>(@this, o => o.GetIDsOfNames(riid, cNames, lcid, rgDispId));

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static HRESULT Invoke(
        IDispatch* @this,
        int dispIdMember,
        Guid* riid,
        PInvoke.LCID lcid,
        DISPATCH_FLAGS dwFlags,
        DISPPARAMS* pDispParams,
        VARIANT* pVarResult,
        EXCEPINFO* pExcepInfo,
        uint* pArgErr)
        => ComWrappers.UnwrapAndInvoke<IDispatch, Interface>(
            @this,
            o => o.Invoke(dispIdMember, riid, lcid, dwFlags, pDispParams, pVarResult, pExcepInfo, pArgErr));

    [ComImport]
    [Guid("00020400-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface Interface
    {
        [PreserveSig]
        HRESULT GetTypeInfoCount(
            uint* pctinfo);

        [PreserveSig]
        HRESULT GetTypeInfo(
            uint iTInfo,
            PInvoke.LCID lcid,
            ITypeInfo** ppTInfo);

        [PreserveSig]
        HRESULT GetIDsOfNames(
            Guid* riid,
            uint cNames,
            PWSTR* lcid,
            int* rgDispId);

        [PreserveSig]
        HRESULT Invoke(
            int dispIdMember,
            Guid* riid,
            PInvoke.LCID lcid,
            DISPATCH_FLAGS dwFlags,
            DISPPARAMS* pDispParams,
            VARIANT* pVarResult,
            EXCEPINFO* pExcepInfo,
            uint* pArgErr);
    }
}
