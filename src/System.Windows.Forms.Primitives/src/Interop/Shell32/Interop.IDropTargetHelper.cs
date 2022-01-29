// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [ComImport]
        [Guid("4657278B-411B-11D2-839A-00C04FD918D0")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDropTargetHelper
        {
            [PreserveSig]
            HRESULT DragEnter(
                IntPtr hwndTarget,
                [MarshalAs(UnmanagedType.Interface)]
                IComDataObject pDataObj,
                ref Point ppt,
                uint dwEffect);

            [PreserveSig]
            HRESULT DragLeave();

            [PreserveSig]
            HRESULT DragOver(
                ref Point ppt,
                uint dwEffect);

            [PreserveSig]
            HRESULT Drop(
                [MarshalAs(UnmanagedType.Interface)]
                IComDataObject pDataObj,
                ref Point ppt,
                uint dwEffect);

            [PreserveSig]
            HRESULT Show(
                BOOL fShow);
        }
    }
}
