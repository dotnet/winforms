// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

internal static partial class Interop
{
    internal static partial class Oleaut32
    {
        [ComImport]
        [Guid("00020401-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface ITypeInfo
        {
            [PreserveSig]
            HRESULT GetTypeAttr(
                TYPEATTR** ppTypeAttr);

            /// <remarks>
            ///  This method is unused so we do not define the interface ITypeComp
            ///  and its dependencies to avoid maintenance costs and code size.
            /// </remarks>
            [PreserveSig]
            HRESULT GetTypeComp(
                IntPtr* ppTComp);

            [PreserveSig]
            HRESULT GetFuncDesc(
                uint index,
                FUNCDESC** ppFuncDesc);

            [PreserveSig]
            HRESULT GetVarDesc(
                uint index,
                VARDESC** ppVarDesc);

            [PreserveSig]
            HRESULT GetNames(
                Ole32.DispatchID memid,
                BSTR* rgBstrNames,
                uint cMaxNames,
                uint* pcNames);

            [PreserveSig]
            HRESULT GetRefTypeOfImplType(
                uint index,
                uint* pRefType);

            [PreserveSig]
            HRESULT GetImplTypeFlags(
                uint index,
                IMPLTYPEFLAGS* pImplTypeFlags);

            [PreserveSig]
            HRESULT GetIDsOfNames(
                [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] rgszNames,
                uint cNames,
                Ole32.DispatchID* pMemId);

            [PreserveSig]
            HRESULT Invoke(
                [MarshalAs(UnmanagedType.Interface)] object pvInstance,
                Ole32.DispatchID memid,
                DISPATCH_FLAGS wFlags,
                DISPPARAMS* pDispParams,
                [Out, MarshalAs(UnmanagedType.LPArray)] object[] pVarResult,
                EXCEPINFO* pExcepInfo,
                uint* puArgErr);

            [PreserveSig]
            HRESULT GetDocumentation(
                Ole32.DispatchID memid,
                BSTR* pBstrName,
                BSTR* pBstrDocString,
                uint* pdwHelpContext,
                BSTR* pBstrHelpFile);

            [PreserveSig]
            HRESULT GetDllEntry(
                Ole32.DispatchID memid,
                INVOKEKIND invkind,
                BSTR* pBstrDllName,
                BSTR* pBstrName,
                ushort* pwOrdinal);

            [PreserveSig]
            HRESULT GetRefTypeInfo(
                uint hreftype,
                out ITypeInfo pTypeInfo);

            [PreserveSig]
            HRESULT AddressOfMember(
                Ole32.DispatchID memid,
                INVOKEKIND invKind,
                IntPtr* ppv);

            [PreserveSig]
            HRESULT CreateInstance(
                IntPtr pUnkOuter,
                Guid* riid,
                IntPtr* ppvObj);

            [PreserveSig]
            HRESULT GetMops(
                Ole32.DispatchID memid,
                BSTR* pBstrMops);

            /// <remarks>
            ///  This method is unused so we do not define the interface ITypeLib
            ///  and its dependencies to avoid maintenance costs and code size.
            /// </remarks>
            [PreserveSig]
            HRESULT GetContainingTypeLib(
                IntPtr* ppTLib,
                uint* pIndex);

            [PreserveSig]
            void ReleaseTypeAttr(
                TYPEATTR* pTypeAttr);

            [PreserveSig]
            void ReleaseFuncDesc(
                FUNCDESC* pFuncDesc);

            [PreserveSig]
            void ReleaseVarDesc(
                VARDESC* pVarDesc);
        }
    }
}
