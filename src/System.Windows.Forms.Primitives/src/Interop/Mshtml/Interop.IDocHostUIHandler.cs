// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("BD3F23C0-D43E-11CF-893B-00AA00BDCE1A")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IDocHostUIHandler
        {
            [PreserveSig]
            HRESULT ShowContextMenu(
                uint dwID,
                Point* pt,
                [MarshalAs(UnmanagedType.Interface)] object pcmdtReserved,
                [MarshalAs(UnmanagedType.Interface)] object pdispReserved);

            [PreserveSig]
            HRESULT GetHostInfo(
                DOCHOSTUIINFO* pInfo);

            [PreserveSig]
            HRESULT ShowUI(
                uint dwID,
                Ole32.IOleInPlaceActiveObject activeObject,
                Ole32.IOleCommandTarget commandTarget,
                Ole32.IOleInPlaceFrame frame,
                Ole32.IOleInPlaceUIWindow doc);

            [PreserveSig]
            HRESULT HideUI();

            [PreserveSig]
            HRESULT UpdateUI();

            [PreserveSig]
            HRESULT EnableModeless(
                BOOL fEnable);

            [PreserveSig]
            HRESULT OnDocWindowActivate(
                BOOL fActivate);

            [PreserveSig]
            HRESULT OnFrameWindowActivate(
                BOOL fActivate);

            [PreserveSig]
            HRESULT ResizeBorder(
                RECT* rect,
                Ole32.IOleInPlaceUIWindow doc,
                BOOL fFrameWindow);

            [PreserveSig]
            HRESULT TranslateAccelerator(
                User32.MSG* lpMsg,
                Guid* pguidCmdGroup,
                uint nCmdID);

            [PreserveSig]
            HRESULT GetOptionKeyPath(
                [Out, MarshalAs(UnmanagedType.LPArray)] string[] pbstrKey,
                uint dw);

            [PreserveSig]
            HRESULT GetDropTarget(
                Ole32.IDropTarget pDropTarget,
                out Ole32.IDropTarget ppDropTarget);

            [PreserveSig]
            HRESULT GetExternal(
                [MarshalAs(UnmanagedType.Interface)] out object ppDispatch);

            [PreserveSig]
            HRESULT TranslateUrl(
                uint dwTranslate,
                [MarshalAs(UnmanagedType.LPWStr)] string strURLIn,
                [MarshalAs(UnmanagedType.LPWStr)] out string pstrURLOut);

            [PreserveSig]
            HRESULT FilterDataObject(
                IDataObject pDO,
                out IDataObject ppDORet);
        }
    }
}
