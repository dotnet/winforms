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
        [Guid("4D07FC10-F931-11CE-B001-00AA006884E5")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface ICategorizeProperties
        {
            [PreserveSig]
            HRESULT MapPropertyToCategory(
                Ole32.DispatchID dispid,
                PROPCAT* ppropcat);

            [PreserveSig]
            HRESULT GetCategoryName(
                PROPCAT propcat,
                Kernel32.LCID lcid,
                out string pbstrName);
        }
    }
}
