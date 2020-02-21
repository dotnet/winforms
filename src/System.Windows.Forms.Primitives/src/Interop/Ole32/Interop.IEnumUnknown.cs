// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("00000100-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IEnumUnknown
        {
            [PreserveSig]
            HRESULT Next(
                uint celt,
                IntPtr rgelt,
                uint* pceltFetched);

            [PreserveSig]
            HRESULT Skip(
                uint celt);

            [PreserveSig]
            HRESULT Reset();

            [PreserveSig]
            HRESULT Clone(
                out IEnumUnknown ppenum);
        }
    }
}
