// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using static Interop.Ole32;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [ComImport]
        [Guid("376BD3AA-3845-101B-84ED-08002B2EC713")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IPerPropertyBrowsing
        {
            [PreserveSig]
            HRESULT GetDisplayString(
                DispatchID dispID,
                out string pBstr);

            [PreserveSig]
            HRESULT MapPropertyToPage(
                DispatchID dispID,
                Guid* pGuid);

            [PreserveSig]
            HRESULT GetPredefinedStrings(
                DispatchID dispID,
                Ole32.CA* pCaStringsOut,
                Ole32.CA* pCaCookiesOut);

            [PreserveSig]
            HRESULT GetPredefinedValue(
                DispatchID dispID,
                uint dwCookie,
                VARIANT* pVarOut);
        }
    }
}
