// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;
using static Interop.Mshtml;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    public partial class WebBrowser
    {
        /// <summary>
        ///  Provides a default WebBrowserSite implementation for use in the CreateWebBrowserSite
        ///  method in the WebBrowser class.
        /// </summary>
        protected class WebBrowserSite : WebBrowserSiteBase, IDocHostUIHandler
        {
            /// <summary>
            ///  Creates an instance of the <see cref="WebBrowserSite"/> class.
            /// </summary>
            public WebBrowserSite(WebBrowser host) : base(host)
            {
            }

            // IDocHostUIHandler Implementation
            unsafe HRESULT IDocHostUIHandler.ShowContextMenu(uint dwID, Point* pt, object pcmdtReserved, object pdispReserved)
            {
                WebBrowser wb = (WebBrowser)Host;

                if (wb.IsWebBrowserContextMenuEnabled)
                {
                    // let MSHTML display its UI
                    return HRESULT.Values.S_FALSE;
                }

                if (pt is null)
                {
                    return HRESULT.Values.E_INVALIDARG;
                }

                if (pt->X == 0 && pt->Y == 0)
                {
                    // IDocHostUIHandler::ShowContextMenu sends (0,0) when the context menu is invoked via the keyboard
                    // make it (-1, -1) for the WebBrowser::ShowContextMenu method
                    pt->X = -1;
                    pt->Y = -1;
                }

                wb.ShowContextMenu(*pt);

                // MSHTML should not display its context menu because we displayed ours
                return HRESULT.Values.S_OK;
            }

            unsafe HRESULT IDocHostUIHandler.GetHostInfo(DOCHOSTUIINFO* pInfo)
            {
                if (pInfo is null)
                {
                    return HRESULT.Values.E_POINTER;
                }

                WebBrowser wb = (WebBrowser)Host;

                pInfo->dwDoubleClick = DOCHOSTUIDBLCLK.DEFAULT;
                pInfo->dwFlags = DOCHOSTUIFLAG.NO3DOUTERBORDER |
                               DOCHOSTUIFLAG.DISABLE_SCRIPT_INACTIVE;

                if (wb.ScrollBarsEnabled)
                {
                    pInfo->dwFlags |= DOCHOSTUIFLAG.FLAT_SCROLLBAR;
                }
                else
                {
                    pInfo->dwFlags |= DOCHOSTUIFLAG.SCROLL_NO;
                }

                if (Application.RenderWithVisualStyles)
                {
                    pInfo->dwFlags |= DOCHOSTUIFLAG.THEME;
                }
                else
                {
                    pInfo->dwFlags |= DOCHOSTUIFLAG.NOTHEME;
                }

                return HRESULT.Values.S_OK;
            }

            HRESULT IDocHostUIHandler.EnableModeless(BOOL fEnable)
            {
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IDocHostUIHandler.ShowUI(
                uint dwID,
                Ole32.IOleInPlaceActiveObject activeObject,
                Ole32.IOleCommandTarget commandTarget,
                Ole32.IOleInPlaceFrame frame,
                Ole32.IOleInPlaceUIWindow doc)
            {
                return HRESULT.Values.S_FALSE;
            }

            HRESULT IDocHostUIHandler.HideUI()
            {
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IDocHostUIHandler.UpdateUI()
            {
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IDocHostUIHandler.OnDocWindowActivate(BOOL fActivate)
            {
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IDocHostUIHandler.OnFrameWindowActivate(BOOL fActivate)
            {
                return HRESULT.Values.E_NOTIMPL;
            }

            unsafe HRESULT IDocHostUIHandler.ResizeBorder(RECT* rect, Ole32.IOleInPlaceUIWindow doc, BOOL fFrameWindow)
            {
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IDocHostUIHandler.GetOptionKeyPath(string[] pbstrKey, uint dw)
            {
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IDocHostUIHandler.GetDropTarget(Ole32.IDropTarget pDropTarget, out Ole32.IDropTarget ppDropTarget)
            {
                // Set to null no matter what we return, to prevent the marshaller
                // from having issues if the pointer points to random stuff.
                ppDropTarget = null;
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IDocHostUIHandler.GetExternal(out object ppDispatch)
            {
                WebBrowser wb = (WebBrowser)Host;
                ppDispatch = wb.ObjectForScripting;
                return HRESULT.Values.S_OK;
            }

            unsafe HRESULT IDocHostUIHandler.TranslateAccelerator(MSG* lpMsg, Guid* pguidCmdGroup, uint nCmdID)
            {
                if (lpMsg is null || pguidCmdGroup is null)
                {
                    return HRESULT.Values.E_POINTER;
                }

                // Returning S_FALSE will allow the native control to do default processing,
                // i.e., execute the shortcut key. Returning S_OK will cancel the shortcut key.
                WebBrowser wb = (WebBrowser)Host;
                if (!wb.WebBrowserShortcutsEnabled)
                {
                    int keyCode = (int)(uint)lpMsg->wParam | (int)ModifierKeys;
                    if (lpMsg->message != (uint)User32.WM.CHAR && Enum.IsDefined(typeof(Shortcut), (Shortcut)keyCode))
                    {
                        return HRESULT.Values.S_OK;
                    }
                }

                return HRESULT.Values.S_FALSE;
            }

            HRESULT IDocHostUIHandler.TranslateUrl(uint dwTranslate, string strUrlIn, out string pstrUrlOut)
            {
                // Set to null no matter what we return, to prevent the marshaller
                // from having issues if the pointer points to random stuff.
                pstrUrlOut = null;
                return HRESULT.Values.S_FALSE;
            }

            HRESULT IDocHostUIHandler.FilterDataObject(IComDataObject pDO, out IComDataObject ppDORet)
            {
                // Set to null no matter what we return, to prevent the marshaller
                // from having issues if the pointer points to random stuff.
                ppDORet = null;
                return HRESULT.Values.S_FALSE;
            }

            //
            // Internal methods
            //
            internal override void OnPropertyChanged(Ole32.DispatchID dispid)
            {
                if (dispid != Ole32.DispatchID.READYSTATE)
                {
                    base.OnPropertyChanged(dispid);
                }
            }
        }
    }
}
