// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("00000117-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IOleInPlaceActiveObject
        {
            [PreserveSig]
            HRESULT GetWindow(
                IntPtr* phwnd);

            [PreserveSig]
            HRESULT ContextSensitiveHelp(
                BOOL fEnterMode);

            [PreserveSig]
            HRESULT TranslateAccelerator(
                User32.MSG* lpmsg);

            [PreserveSig]
            HRESULT OnFrameWindowActivate(
                BOOL fActivate);

            [PreserveSig]
            HRESULT OnDocWindowActivate(
                BOOL fActivate);

            [PreserveSig]
            HRESULT ResizeBorder(
                RECT* prcBorder,
                IOleInPlaceUIWindow pUIWindow,
                BOOL fFrameWindow);

            [PreserveSig]
            HRESULT EnableModeless(
                BOOL fEnable);
        }
    }
}
