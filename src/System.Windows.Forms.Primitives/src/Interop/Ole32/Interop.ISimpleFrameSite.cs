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
        [Guid("742B0E01-14E6-101B-914E-00AA00300CAB")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface ISimpleFrameSite
        {
            [PreserveSig]
            HRESULT PreMessageFilter(
                IntPtr hWnd,
                uint msg,
                IntPtr wp,
                IntPtr lp,
                IntPtr* plResult,
                uint* pdwCookie);

            [PreserveSig]
            HRESULT PostMessageFilter(
                IntPtr hWnd,
                uint msg,
                IntPtr wp,
                IntPtr lp,
                IntPtr* plResult,
                uint dwCookie);
        }
    }
}
