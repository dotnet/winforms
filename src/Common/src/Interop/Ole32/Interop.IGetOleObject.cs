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
        [Guid("8A701DA0-4FEB-101B-A82E-08002B2B2337")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IGetOleObject
        {
            [PreserveSig]
            HRESULT GetOleObject(
                Guid* riid,
                [MarshalAs(UnmanagedType.Interface)] out object ppvObj);
        }
    }
}
