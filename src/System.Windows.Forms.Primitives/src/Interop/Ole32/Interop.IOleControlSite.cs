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
        [Guid("B196B289-BAB4-101A-B69C-00AA00341D07")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleControlSite
        {
            [PreserveSig]
            HRESULT OnControlInfoChanged();

            [PreserveSig]
            HRESULT LockInPlaceActive(
                BOOL fLock);

            [PreserveSig]
            HRESULT GetExtendedControl(
                IntPtr* ppDisp);

            [PreserveSig]
            HRESULT TransformCoords(
                Point *pPtlHimetric,
                PointF *pPtfContainer,
                XFORMCOORDS dwFlags);

            [PreserveSig]
            HRESULT TranslateAccelerator(
                User32.MSG* pMsg,
                KEYMODIFIERS grfModifiers);

            [PreserveSig]
            HRESULT OnFocus(
                BOOL fGotFocus);

            [PreserveSig]
            HRESULT ShowPropertyFrame();
        }
    }
}
