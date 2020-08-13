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
        [Guid("00000118-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleClientSite
        {
            [PreserveSig]
            HRESULT SaveObject();

            [PreserveSig]
            HRESULT GetMoniker(
                Ole32.OLEGETMONIKER dwAssign,
                Ole32.OLEWHICHMK dwWhichMoniker,
                IntPtr* ppmk);

            IOleContainer GetContainer();

            [PreserveSig]
            HRESULT ShowObject();

            [PreserveSig]
            HRESULT OnShowWindow(
                BOOL fShow);

            [PreserveSig]
            HRESULT RequestNewObjectLayout();
        }
    }
}
