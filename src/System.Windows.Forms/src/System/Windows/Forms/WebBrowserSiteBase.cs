// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop.Ole32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class implements the necessary interfaces required for an ActiveX site.
    ///
    ///  This class is public, but has an internal constructor so that external
    ///  users can only reference the Type (cannot instantiate it directly).
    ///  Other classes have to inherit this class and expose it to the outside world.
    ///
    ///  This class does not have any public property/method/event by itself.
    ///  All implementations of the site interface methods are private, which
    ///  means that inheritors who want to override even a single method of one
    ///  of these interfaces will have to implement the whole interface.
    /// </summary>
    public class WebBrowserSiteBase :
        IOleControlSite,
        IOleInPlaceSite,
        IOleClientSite,
        ISimpleFrameSite,
        IPropertyNotifySink,
        IDisposable
    {
        private readonly WebBrowserBase host;
        private AxHost.ConnectionPointCookie connectionPoint;

        //
        // The constructor takes an WebBrowserBase as a parameter, so unfortunately,
        // this cannot be used as a standalone site. It has to be used in conjunction
        // with WebBrowserBase. Perhaps we can change it in future.
        //
        internal WebBrowserSiteBase(WebBrowserBase h)
        {
            host = h.OrThrowIfNull();
        }

        /// <summary>
        ///  Dispose(release the cookie)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///  Release the cookie if we're disposing
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopEvents();
            }
        }

        /// <summary>
        ///  Retrieves the WebBrowserBase object set in the constructor.
        /// </summary>
        internal WebBrowserBase Host
        {
            get
            {
                return host;
            }
        }

        // IOleControlSite methods:
        HRESULT IOleControlSite.OnControlInfoChanged()
        {
            return HRESULT.Values.S_OK;
        }

        HRESULT IOleControlSite.LockInPlaceActive(BOOL fLock)
        {
            return HRESULT.Values.E_NOTIMPL;
        }

        unsafe HRESULT IOleControlSite.GetExtendedControl(IntPtr* ppDisp)
        {
            if (ppDisp is null)
            {
                return HRESULT.Values.E_POINTER;
            }

            *ppDisp = IntPtr.Zero;
            return HRESULT.Values.E_NOTIMPL;
        }

        unsafe HRESULT IOleControlSite.TransformCoords(Point* pPtlHimetric, PointF* pPtfContainer, XFORMCOORDS dwFlags)
        {
            if (pPtlHimetric is null || pPtfContainer is null)
            {
                return HRESULT.Values.E_POINTER;
            }

            if ((dwFlags & XFORMCOORDS.HIMETRICTOCONTAINER) != 0)
            {
                if ((dwFlags & XFORMCOORDS.SIZE) != 0)
                {
                    pPtfContainer->X = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->X, WebBrowserHelper.LogPixelsX);
                    pPtfContainer->Y = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->Y, WebBrowserHelper.LogPixelsY);
                }
                else if ((dwFlags & XFORMCOORDS.POSITION) != 0)
                {
                    pPtfContainer->X = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->X, WebBrowserHelper.LogPixelsX);
                    pPtfContainer->Y = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->Y, WebBrowserHelper.LogPixelsY);
                }
                else
                {
                    return HRESULT.Values.E_INVALIDARG;
                }
            }
            else if ((dwFlags & XFORMCOORDS.CONTAINERTOHIMETRIC) != 0)
            {
                if ((dwFlags & XFORMCOORDS.SIZE) != 0)
                {
                    pPtlHimetric->X = WebBrowserHelper.Pix2HM((int)pPtfContainer->X, WebBrowserHelper.LogPixelsX);
                    pPtlHimetric->Y = WebBrowserHelper.Pix2HM((int)pPtfContainer->Y, WebBrowserHelper.LogPixelsY);
                }
                else if ((dwFlags & XFORMCOORDS.POSITION) != 0)
                {
                    pPtlHimetric->X = WebBrowserHelper.Pix2HM((int)pPtfContainer->X, WebBrowserHelper.LogPixelsX);
                    pPtlHimetric->Y = WebBrowserHelper.Pix2HM((int)pPtfContainer->Y, WebBrowserHelper.LogPixelsY);
                }
                else
                {
                    return HRESULT.Values.E_INVALIDARG;
                }
            }
            else
            {
                return HRESULT.Values.E_INVALIDARG;
            }

            return HRESULT.Values.S_OK;
        }

        unsafe HRESULT IOleControlSite.TranslateAccelerator(MSG* pMsg, KEYMODIFIERS grfModifiers)
        {
            if (pMsg is null)
            {
                return HRESULT.Values.E_POINTER;
            }

            Debug.Assert(!Host.GetAXHostState(WebBrowserHelper.siteProcessedInputKey), "Re-entering IOleControlSite.TranslateAccelerator!!!");
            Host.SetAXHostState(WebBrowserHelper.siteProcessedInputKey, true);

            Message msg = *pMsg;
            try
            {
                bool f = ((Control)Host).PreProcessControlMessage(ref msg) == PreProcessControlState.MessageProcessed;
                return f ? HRESULT.Values.S_OK : HRESULT.Values.S_FALSE;
            }
            finally
            {
                Host.SetAXHostState(WebBrowserHelper.siteProcessedInputKey, false);
            }
        }

        HRESULT IOleControlSite.OnFocus(BOOL fGotFocus) => HRESULT.Values.S_OK;

        HRESULT IOleControlSite.ShowPropertyFrame() => HRESULT.Values.E_NOTIMPL;

        // IOleClientSite methods:
        HRESULT IOleClientSite.SaveObject() => HRESULT.Values.E_NOTIMPL;

        unsafe HRESULT IOleClientSite.GetMoniker(OLEGETMONIKER dwAssign, OLEWHICHMK dwWhichMoniker, IntPtr* ppmk)
        {
            if (ppmk is null)
            {
                return HRESULT.Values.E_POINTER;
            }

            *ppmk = IntPtr.Zero;
            return HRESULT.Values.E_NOTIMPL;
        }

        IOleContainer IOleClientSite.GetContainer()
        {
            return Host.GetParentContainer();
        }

        unsafe HRESULT IOleClientSite.ShowObject()
        {
            if (Host.ActiveXState >= WebBrowserHelper.AXState.InPlaceActive)
            {
                HWND hwnd = HWND.Null;
                if (Host.AXInPlaceObject.GetWindow(&hwnd).Succeeded)
                {
                    if (Host.GetHandleNoCreate() != hwnd)
                    {
                        if (!hwnd.IsNull)
                        {
                            Host.AttachWindow(hwnd);
                            RECT posRect = Host.Bounds;
                            OnActiveXRectChange(&posRect);
                        }
                    }
                }
                else if (Host.AXInPlaceObject is IOleInPlaceObjectWindowless)
                {
                    throw new InvalidOperationException(SR.AXWindowlessControl);
                }
            }

            return HRESULT.Values.S_OK;
        }

        HRESULT IOleClientSite.OnShowWindow(BOOL fShow) => HRESULT.Values.S_OK;

        HRESULT IOleClientSite.RequestNewObjectLayout() => HRESULT.Values.E_NOTIMPL;

        // IOleInPlaceSite methods:
        unsafe HRESULT IOleInPlaceSite.GetWindow(IntPtr* phwnd)
        {
            if (phwnd is null)
            {
                return HRESULT.Values.E_POINTER;
            }

            *phwnd = PInvoke.GetParent(Host);
            return HRESULT.Values.S_OK;
        }

        HRESULT IOleInPlaceSite.ContextSensitiveHelp(BOOL fEnterMode) => HRESULT.Values.E_NOTIMPL;

        HRESULT IOleInPlaceSite.CanInPlaceActivate() => HRESULT.Values.S_OK;

        unsafe HRESULT IOleInPlaceSite.OnInPlaceActivate()
        {
            Host.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
            RECT posRect = Host.Bounds;
            OnActiveXRectChange(&posRect);
            return HRESULT.Values.S_OK;
        }

        HRESULT IOleInPlaceSite.OnUIActivate()
        {
            Host.ActiveXState = WebBrowserHelper.AXState.UIActive;
            Host.GetParentContainer().OnUIActivate(Host);
            return HRESULT.Values.S_OK;
        }

        unsafe HRESULT IOleInPlaceSite.GetWindowContext(
            out IOleInPlaceFrame ppFrame,
            out IOleInPlaceUIWindow ppDoc,
            RECT* lprcPosRect,
            RECT* lprcClipRect,
            OLEINPLACEFRAMEINFO* lpFrameInfo)
        {
            ppDoc = null;
            ppFrame = Host.GetParentContainer();

            if (lprcPosRect is null || lprcClipRect is null)
            {
                return HRESULT.Values.E_POINTER;
            }

            *lprcPosRect = Host.Bounds;
            *lprcClipRect = WebBrowserHelper.GetClipRect();
            if (lpFrameInfo is not null)
            {
                lpFrameInfo->cb = (uint)Marshal.SizeOf<OLEINPLACEFRAMEINFO>();
                lpFrameInfo->fMDIApp = false;
                lpFrameInfo->hAccel = IntPtr.Zero;
                lpFrameInfo->cAccelEntries = 0;
                lpFrameInfo->hwndFrame = Host.ParentInternal?.Handle ?? IntPtr.Zero;
            }

            return HRESULT.Values.S_OK;
        }

        HRESULT IOleInPlaceSite.Scroll(Size scrollExtant) => HRESULT.Values.S_FALSE;

        HRESULT IOleInPlaceSite.OnUIDeactivate(BOOL fUndoable)
        {
            Host.GetParentContainer().OnUIDeactivate(Host);
            if (Host.ActiveXState > WebBrowserHelper.AXState.InPlaceActive)
            {
                Host.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
            }

            return HRESULT.Values.S_OK;
        }

        HRESULT IOleInPlaceSite.OnInPlaceDeactivate()
        {
            if (Host.ActiveXState == WebBrowserHelper.AXState.UIActive)
            {
                ((IOleInPlaceSite)this).OnUIDeactivate(false);
            }

            Host.GetParentContainer().OnInPlaceDeactivate(Host);
            Host.ActiveXState = WebBrowserHelper.AXState.Running;
            return HRESULT.Values.S_OK;
        }

        HRESULT IOleInPlaceSite.DiscardUndoState() => HRESULT.Values.S_OK;

        HRESULT IOleInPlaceSite.DeactivateAndUndo() => Host.AXInPlaceObject.UIDeactivate();

        unsafe HRESULT IOleInPlaceSite.OnPosRectChange(RECT* lprcPosRect) => OnActiveXRectChange(lprcPosRect);

        // ISimpleFrameSite methods:
        unsafe HRESULT ISimpleFrameSite.PreMessageFilter(IntPtr hWnd, uint msg, IntPtr wp, IntPtr lp, IntPtr* plResult, uint* pdwCookie)
        {
            return HRESULT.Values.S_OK;
        }

        unsafe HRESULT ISimpleFrameSite.PostMessageFilter(IntPtr hWnd, uint msg, IntPtr wp, IntPtr lp, IntPtr* plResult, uint dwCookie)
        {
            return HRESULT.Values.S_FALSE;
        }

        // IPropertyNotifySink methods:
        HRESULT IPropertyNotifySink.OnChanged(DispatchID dispid)
        {
            // Some controls fire OnChanged() notifications when getting values of some properties.
            // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
            if (Host.NoComponentChangeEvents != 0)
            {
                return HRESULT.Values.S_OK;
            }

            Host.NoComponentChangeEvents++;
            try
            {
                OnPropertyChanged(dispid);
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                throw;
            }
            finally
            {
                Host.NoComponentChangeEvents--;
            }

            return HRESULT.Values.S_OK;
        }

        HRESULT IPropertyNotifySink.OnRequestEdit(DispatchID dispid)
        {
            return HRESULT.Values.S_OK;
        }

        internal virtual void OnPropertyChanged(DispatchID dispid)
        {
            if (Host.Site.TryGetService(out IComponentChangeService changeService))
            {
                try
                {
                    changeService.OnComponentChanging(Host);
                    changeService.OnComponentChanged(Host);
                }
                catch (CheckoutException e) when (e == CheckoutException.Canceled)
                {
                    return;
                }
            }
        }

        internal void StartEvents()
        {
            if (connectionPoint is not null)
            {
                return;
            }

            object nativeObject = Host.activeXInstance;
            if (nativeObject is not null)
            {
                try
                {
                    connectionPoint = new AxHost.ConnectionPointCookie(nativeObject, this, typeof(IPropertyNotifySink));
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
            }
        }

        internal void StopEvents()
        {
            if (connectionPoint is not null)
            {
                connectionPoint.Disconnect();
                connectionPoint = null;
            }
        }

        private unsafe HRESULT OnActiveXRectChange(RECT* lprcPosRect)
        {
            if (lprcPosRect is null)
            {
                return HRESULT.Values.E_INVALIDARG;
            }

            var posRect = new RECT(0, 0, lprcPosRect->right - lprcPosRect->left, lprcPosRect->bottom - lprcPosRect->top);
            var clipRect = WebBrowserHelper.GetClipRect();
            Host.AXInPlaceObject.SetObjectRects(&posRect, &clipRect);
            Host.MakeDirty();
            return HRESULT.Values.S_OK;
        }
    }
}
