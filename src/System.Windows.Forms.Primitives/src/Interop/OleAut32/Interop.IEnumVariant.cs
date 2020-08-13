// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [ComImport]
        [Guid("00020404-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IEnumVariant
        {
            [PreserveSig]
            HRESULT Next(
                uint celt,
                IntPtr rgVar,
                uint* pCeltFetched);

            [PreserveSig]
            HRESULT Skip(
                uint celt);

            [PreserveSig]
            HRESULT Reset();

            [PreserveSig]
            HRESULT Clone(
                [Out, MarshalAs(UnmanagedType.LPArray)] IEnumVariant[]? ppEnum);
        }
    }
}
