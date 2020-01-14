// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [ComImport]
        [Guid("00020400-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IDispatch
        {
            [PreserveSig]
            HRESULT GetTypeInfoCount(
                uint* pctinfo);

            [PreserveSig]
            HRESULT GetTypeInfo(
                uint iTInfo,
                uint lcid,
                out ITypeInfo ppTInfo);

            [PreserveSig]
            HRESULT GetIDsOfNames(
                Guid* riid,
                [MarshalAs(UnmanagedType.LPArray)] string[] rgszNames,
                uint cNames,
                uint lcid,
                Ole32.DispatchID* rgDispId);

            [PreserveSig]
            HRESULT Invoke(
                Ole32.DispatchID dispIdMember,
                Guid* riid,
                uint lcid,
                DISPATCH dwFlags,
                Ole32.DISPPARAMS* pDispParams,
                [Out, MarshalAs(UnmanagedType.LPArray)] object[] pVarResult,
                Ole32.EXCEPINFO* pExcepInfo,
                uint* pArgErr);
        }
    }
}
