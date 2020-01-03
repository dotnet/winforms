// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("00000104-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IEnumOLEVERB
        {
            /// <remarks>
            ///  This method really should take OLEVERB* (i.e. an array of OLEVERB structs).
            ///  However, OLEVERB is defined as a class meaning that we can only support
            ///  celt values of 1.
            /// </remarks>
            [PreserveSig]
            HRESULT Next(
                uint celt,
                OLEVERB rgelt,
                uint* pceltFetched);

            [PreserveSig]
            HRESULT Skip(
                uint celt);

            [PreserveSig]
            HRESULT Reset();

            [PreserveSig]
            HRESULT Clone(
               out IEnumOLEVERB ppenum);
        }
    }
}
