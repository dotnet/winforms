// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("B196B28F-BAB4-101A-B69C-00AA00341D07")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IClassFactory2
        {
            [PreserveSig]
            HRESULT CreateInstance(
                IntPtr pUnkOuter,
                Guid* riid,
                [Out, MarshalAs(UnmanagedType.LPArray)] object[] ppunk);

            [PreserveSig]
            HRESULT LockServer(
                BOOL fLock);

            [PreserveSig]
            HRESULT GetLicInfo(
                LICINFO* licInfo);

            [PreserveSig]
            HRESULT RequestLicKey(
                uint dwReserved,
                [MarshalAs(UnmanagedType.LPArray)] string[] pBstrKey);

            [PreserveSig]
            HRESULT CreateInstanceLic(
                IntPtr pUnkOuter,
                IntPtr pUnkReserved,
                ref Guid riid,
                string bstrKey,
                [MarshalAs(UnmanagedType.Interface)] out object ppVal);
        }
    }
}
