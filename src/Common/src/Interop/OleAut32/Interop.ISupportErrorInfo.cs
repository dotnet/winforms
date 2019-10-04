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
        [Guid("DF0B3D60-548F-101B-8E65-08002B2BD119")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface ISupportErrorInfo
        {
            [PreserveSig]
            HRESULT InterfaceSupportsErrorInfo(
                Guid* riid);
        }
    }
}
