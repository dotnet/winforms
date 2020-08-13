// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [ComImport]
        [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellItem
        {
            [PreserveSig]
            HRESULT BindToHandler(
                IntPtr pbc,
                ref Guid bhid,
                ref Guid riid,
                out IntPtr ppv);

            [PreserveSig]
            HRESULT GetParent(
                out IShellItem ppsi);

            [PreserveSig]
            HRESULT GetDisplayName(
                SIGDN sigdnName,
                [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

            [PreserveSig]
            HRESULT GetAttributes(
                SFGAOF sfgaoMask,
                out SFGAOF psfgaoAttribs);

            [PreserveSig]
            HRESULT Compare(
                IShellItem psi,
                uint hint,
                out int piOrder);
        }
    }
}
