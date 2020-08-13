// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("00000122-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDropTarget
        {
            [PreserveSig]
            HRESULT DragEnter(
                [MarshalAs(UnmanagedType.Interface)]
                object pDataObj,
                uint grfKeyState,
                Point pt,
                ref uint pdwEffect);

            [PreserveSig]
            HRESULT DragOver(
                uint grfKeyState,
                Point pt,
                ref uint pdwEffect);

            [PreserveSig]
            HRESULT DragLeave();

            [PreserveSig]
            HRESULT Drop(
                [MarshalAs(UnmanagedType.Interface)]
                object pDataObj,
                uint grfKeyState,
                Point pt,
                ref uint pdwEffect);
        }
    }
}
