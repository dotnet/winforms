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
        [Guid("B63EA76D-1F85-456F-A19C-48159EFA858B")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellItemArray
        {
            [PreserveSig]
            HRESULT BindToHandler(
                IntPtr pbc,
                ref Guid rbhid,
                ref Guid riid,
                out IntPtr ppvOut);

            [PreserveSig]
            HRESULT GetPropertyStore(
                GETPROPERTYSTOREFLAGS flags,
                ref Guid riid,
                out IntPtr ppv);

            [PreserveSig]
            HRESULT GetPropertyDescriptionList(
                ref PROPERTYKEY keyType,
                ref Guid riid,
                out IntPtr ppv);

            [PreserveSig]
            HRESULT GetAttributes(
                SIATTRIBFLAGS dwAttribFlags,
                uint sfgaoMask,
                out uint psfgaoAttribs);

            void GetCount(
                out uint pdwNumItems);

            void GetItemAt(
                uint dwIndex,
                out IShellItem ppsi);

            [PreserveSig]
            HRESULT EnumItems(
                out IntPtr ppenumShellItems);
        }
    }
}
