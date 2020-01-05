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
        [Guid("91733A60-3F4C-101B-A3F6-00AA0034E4E9")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IGetVBAObject
        {
            [PreserveSig]
            HRESULT GetObject(
                Guid* riid,
                [Out, MarshalAs(UnmanagedType.LPArray)] IVBFormat[] rval,
                uint dwReserved);
        }
    }
}
