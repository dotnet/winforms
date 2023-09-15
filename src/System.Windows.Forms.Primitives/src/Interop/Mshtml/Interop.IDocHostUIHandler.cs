// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Windows.Win32.System.Ole;
using MsHtml = Windows.Win32.Web.MsHtml;

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
                MsHtml.DOCHOSTUIINFO* pInfo);

            [PreserveSig]
            HRESULT ShowUI(
                uint dwID,
                IOleInPlaceActiveObject.Interface activeObject,
                IOleCommandTarget.Interface commandTarget,
                IOleInPlaceFrame.Interface frame,
                IOleInPlaceUIWindow.Interface doc);

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
                IOleInPlaceUIWindow.Interface doc,
                BOOL fFrameWindow);

            [PreserveSig]
            HRESULT TranslateAccelerator(
                MSG* lpMsg,
                Guid* pguidCmdGroup,
                uint nCmdID);

            [PreserveSig]
            HRESULT GetOptionKeyPath(
                [Out, MarshalAs(UnmanagedType.LPArray)] string[] pbstrKey,
                uint dw);

            [PreserveSig]
            HRESULT GetDropTarget(
                IDropTarget.Interface pDropTarget,
                out IDropTarget.Interface? ppDropTarget);

            [PreserveSig]
            HRESULT GetExternal(
                [MarshalAs(UnmanagedType.Interface)] out object? ppDispatch);

            [PreserveSig]
            HRESULT TranslateUrl(
                uint dwTranslate,
                [MarshalAs(UnmanagedType.LPWStr)] string strURLIn,
                [MarshalAs(UnmanagedType.LPWStr)] out string? pstrURLOut);

            [PreserveSig]
            HRESULT FilterDataObject(
                IDataObject pDO,
                out IDataObject? ppDORet);
        }
    }
}
