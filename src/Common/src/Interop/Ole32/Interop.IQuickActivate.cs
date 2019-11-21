// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("CF51ED10-62FE-11CF-BF86-00A0C9034836")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IQuickActivate
        {
            [PreserveSig]
            HRESULT QuickActivate(
                QACONTAINER pQaContainer,
                QACONTROL* pQaControl);

            [PreserveSig]
            HRESULT SetContentExtent(
                Size* pSizel);

            [PreserveSig]
            HRESULT GetContentExtent(
                Size* pSizel);
        }
    }
}
