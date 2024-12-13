// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Ole;
using static Interop.Mshtml;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using MsHtml = Windows.Win32.Web.MsHtml;
using Ole = Windows.Win32.System.Ole;

namespace System.Windows.Forms;

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
        public WebBrowserSite(WebBrowser host)
            : base(host)
        {
        }

        // IDocHostUIHandler Implementation
        unsafe HRESULT IDocHostUIHandler.ShowContextMenu(uint dwID, Point* pt, object pcmdtReserved, object pdispReserved)
        {
            WebBrowser wb = (WebBrowser)Host;

            if (wb.IsWebBrowserContextMenuEnabled)
            {
                // let MSHTML display its UI
                return HRESULT.S_FALSE;
            }

            if (pt is null)
            {
                return HRESULT.E_INVALIDARG;
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
            return HRESULT.S_OK;
        }

        unsafe HRESULT IDocHostUIHandler.GetHostInfo(MsHtml.DOCHOSTUIINFO* pInfo)
        {
            if (pInfo is null)
            {
                return HRESULT.E_POINTER;
            }

            WebBrowser wb = (WebBrowser)Host;

            pInfo->dwDoubleClick = MsHtml.DOCHOSTUIDBLCLK.DOCHOSTUIDBLCLK_DEFAULT;
            pInfo->dwFlags = MsHtml.DOCHOSTUIFLAG.DOCHOSTUIFLAG_NO3DOUTERBORDER |
                           MsHtml.DOCHOSTUIFLAG.DOCHOSTUIFLAG_DISABLE_SCRIPT_INACTIVE;

            if (wb.ScrollBarsEnabled)
            {
                pInfo->dwFlags |= MsHtml.DOCHOSTUIFLAG.DOCHOSTUIFLAG_FLAT_SCROLLBAR;
            }
            else
            {
                pInfo->dwFlags |= MsHtml.DOCHOSTUIFLAG.DOCHOSTUIFLAG_SCROLL_NO;
            }

            if (Application.RenderWithVisualStyles)
            {
                pInfo->dwFlags |= MsHtml.DOCHOSTUIFLAG.DOCHOSTUIFLAG_THEME;
            }
            else
            {
                pInfo->dwFlags |= MsHtml.DOCHOSTUIFLAG.DOCHOSTUIFLAG_NOTHEME;
            }

            return HRESULT.S_OK;
        }

        HRESULT IDocHostUIHandler.EnableModeless(BOOL fEnable)
        {
            return HRESULT.E_NOTIMPL;
        }

        HRESULT IDocHostUIHandler.ShowUI(
            uint dwID,
            IOleInPlaceActiveObject.Interface activeObject,
            IOleCommandTarget.Interface commandTarget,
            IOleInPlaceFrame.Interface frame,
            IOleInPlaceUIWindow.Interface doc)
        {
            return HRESULT.S_FALSE;
        }

        HRESULT IDocHostUIHandler.HideUI()
        {
            return HRESULT.E_NOTIMPL;
        }

        HRESULT IDocHostUIHandler.UpdateUI()
        {
            return HRESULT.E_NOTIMPL;
        }

        HRESULT IDocHostUIHandler.OnDocWindowActivate(BOOL fActivate)
        {
            return HRESULT.E_NOTIMPL;
        }

        HRESULT IDocHostUIHandler.OnFrameWindowActivate(BOOL fActivate)
        {
            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT IDocHostUIHandler.ResizeBorder(RECT* rect, IOleInPlaceUIWindow.Interface doc, BOOL fFrameWindow)
        {
            return HRESULT.E_NOTIMPL;
        }

        HRESULT IDocHostUIHandler.GetOptionKeyPath(string[] pbstrKey, uint dw)
        {
            return HRESULT.E_NOTIMPL;
        }

        HRESULT IDocHostUIHandler.GetDropTarget(Ole.IDropTarget.Interface pDropTarget, out Ole.IDropTarget.Interface? ppDropTarget)
        {
            // Set to null no matter what we return, to prevent the marshaller
            // from having issues if the pointer points to random stuff.
            ppDropTarget = null;
            return HRESULT.E_NOTIMPL;
        }

        HRESULT IDocHostUIHandler.GetExternal(out object? ppDispatch)
        {
            WebBrowser wb = (WebBrowser)Host;
            ppDispatch = wb.ObjectForScripting;
            return HRESULT.S_OK;
        }

        unsafe HRESULT IDocHostUIHandler.TranslateAccelerator(MSG* lpMsg, Guid* pguidCmdGroup, uint nCmdID)
        {
            if (lpMsg is null || pguidCmdGroup is null)
            {
                return HRESULT.E_POINTER;
            }

            // Returning S_FALSE will allow the native control to do default processing,
            // i.e., execute the shortcut key. Returning S_OK will cancel the shortcut key.
            WebBrowser wb = (WebBrowser)Host;
            if (!wb.WebBrowserShortcutsEnabled)
            {
                int keyCode = (int)(uint)lpMsg->wParam | (int)ModifierKeys;
                if (lpMsg->message != PInvokeCore.WM_CHAR && Enum.IsDefined((Shortcut)keyCode))
                {
                    return HRESULT.S_OK;
                }
            }

            return HRESULT.S_FALSE;
        }

        HRESULT IDocHostUIHandler.TranslateUrl(uint dwTranslate, string strUrlIn, out string? pstrUrlOut)
        {
            // Set to null no matter what we return, to prevent the marshaller
            // from having issues if the pointer points to random stuff.
            pstrUrlOut = null;
            return HRESULT.S_FALSE;
        }

        HRESULT IDocHostUIHandler.FilterDataObject(ComTypes.IDataObject pDO, out ComTypes.IDataObject? ppDORet)
        {
            // Set to null no matter what we return, to prevent the marshaller
            // from having issues if the pointer points to random stuff.
            ppDORet = null;
            return HRESULT.S_FALSE;
        }

        internal override void OnPropertyChanged(int dispid)
        {
            if (dispid != PInvokeCore.DISPID_READYSTATE)
            {
                base.OnPropertyChanged(dispid);
            }
        }
    }
}
