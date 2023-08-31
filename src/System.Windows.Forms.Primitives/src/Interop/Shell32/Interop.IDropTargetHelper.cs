// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.System.Ole;
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
                IComDataObject pDataObj,
                ref Point ppt,
                DROPEFFECT dwEffect);

            [PreserveSig]
            HRESULT DragLeave();

            [PreserveSig]
            HRESULT DragOver(
                ref Point ppt,
                DROPEFFECT dwEffect);

            [PreserveSig]
            HRESULT Drop(
                IComDataObject pDataObj,
                ref Point ppt,
                DROPEFFECT dwEffect);

            [PreserveSig]
            HRESULT Show(
                BOOL fShow);
        }
    }
}
