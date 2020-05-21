// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class VSSDK
    {
        [ComImport]
        [Guid("7494683C-37A0-11d2-A273-00C04F8EF4FF")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IVSMDPerPropertyBrowsing
        {
            [PreserveSig]
            HRESULT GetPropertyAttributes(
                Ole32.DispatchID dispid,
                uint* pceltAttrs,
                IntPtr* ppbstrTypeNames,
                Oleaut32.VARIANT** ppvarAttrValues);
        }
    }
}
