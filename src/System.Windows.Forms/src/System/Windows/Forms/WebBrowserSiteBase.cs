﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

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
        Ole32.IOleControlSite,
        Ole32.IOleInPlaceSite,
        Ole32.IOleClientSite,
        Ole32.ISimpleFrameSite,
        Ole32.IPropertyNotifySink,
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
            host = h ?? throw new ArgumentNullException(nameof(h));
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
        HRESULT Ole32.IOleControlSite.OnControlInfoChanged()
        {
            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleControlSite.LockInPlaceActive(BOOL fLock)
        {
            return HRESULT.E_NOTIMPL;
        }

        HRESULT Ole32.IOleControlSite.GetExtendedControl(out object ppDisp)
        {
            ppDisp = null;
            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT Ole32.IOleControlSite.TransformCoords(Point *pPtlHimetric, PointF *pPtfContainer, Ole32.XFORMCOORDS dwFlags)
        {
            if (pPtlHimetric == null || pPtfContainer == null)
            {
                return HRESULT.E_INVALIDARG;
            }

            if ((dwFlags & Ole32.XFORMCOORDS.HIMETRICTOCONTAINER) != 0)
            {
                if ((dwFlags & Ole32.XFORMCOORDS.SIZE) != 0)
                {
                    pPtfContainer->X = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->X, WebBrowserHelper.LogPixelsX);
                    pPtfContainer->Y = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->Y, WebBrowserHelper.LogPixelsY);
                }
                else if ((dwFlags & Ole32.XFORMCOORDS.POSITION) != 0)
                {
                    pPtfContainer->X = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->X, WebBrowserHelper.LogPixelsX);
                    pPtfContainer->Y = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->Y, WebBrowserHelper.LogPixelsY);
                }
                else
                {
                    return HRESULT.E_INVALIDARG;
                }
            }
            else if ((dwFlags & Ole32.XFORMCOORDS.CONTAINERTOHIMETRIC) != 0)
            {
                if ((dwFlags & Ole32.XFORMCOORDS.SIZE) != 0)
                {
                    pPtlHimetric->X = WebBrowserHelper.Pix2HM((int)pPtfContainer->X, WebBrowserHelper.LogPixelsX);
                    pPtlHimetric->Y = WebBrowserHelper.Pix2HM((int)pPtfContainer->Y, WebBrowserHelper.LogPixelsY);
                }
                else if ((dwFlags & Ole32.XFORMCOORDS.POSITION) != 0)
                {
                    pPtlHimetric->X = WebBrowserHelper.Pix2HM((int)pPtfContainer->X, WebBrowserHelper.LogPixelsX);
                    pPtlHimetric->Y = WebBrowserHelper.Pix2HM((int)pPtfContainer->Y, WebBrowserHelper.LogPixelsY);
                }
                else
                {
                    return HRESULT.E_INVALIDARG;
                }
            }
            else
            {
                return HRESULT.E_INVALIDARG;
            }

            return HRESULT.S_OK;
        }

        unsafe HRESULT Ole32.IOleControlSite.TranslateAccelerator(User32.MSG* pMsg, Ole32.KEYMODIFIERS grfModifiers)
        {
            if (pMsg == null)
            {
                return HRESULT.E_POINTER;
            }

            Debug.Assert(!Host.GetAXHostState(WebBrowserHelper.siteProcessedInputKey), "Re-entering Ole32.IOleControlSite.TranslateAccelerator!!!");
            Host.SetAXHostState(WebBrowserHelper.siteProcessedInputKey, true);

            Message msg = *pMsg;
            try
            {
                bool f = ((Control)Host).PreProcessControlMessage(ref msg) == PreProcessControlState.MessageProcessed;
                return f ? HRESULT.S_OK : HRESULT.S_FALSE;
            }
            finally
            {
                Host.SetAXHostState(WebBrowserHelper.siteProcessedInputKey, false);
            }
        }

        HRESULT Ole32.IOleControlSite.OnFocus(BOOL fGotFocus) => HRESULT.S_OK;

        HRESULT Ole32.IOleControlSite.ShowPropertyFrame() => HRESULT.E_NOTIMPL;

        // IOleClientSite methods:
        HRESULT Ole32.IOleClientSite.SaveObject() => HRESULT.E_NOTIMPL;

        unsafe HRESULT Ole32.IOleClientSite.GetMoniker(Ole32.OLEGETMONIKER dwAssign, Ole32.OLEWHICHMK dwWhichMoniker, IntPtr* ppmk)
        {
            if (ppmk == null)
            {
                return HRESULT.E_POINTER;
            }

            *ppmk = IntPtr.Zero;
            return HRESULT.E_NOTIMPL;
        }

        HRESULT Ole32.IOleClientSite.GetContainer(out Ole32.IOleContainer container)
        {
            container = Host.GetParentContainer();
            return HRESULT.S_OK;
        }

        unsafe HRESULT Ole32.IOleClientSite.ShowObject()
        {
            if (Host.ActiveXState >= WebBrowserHelper.AXState.InPlaceActive)
            {
                IntPtr hwnd = IntPtr.Zero;
                if (Host.AXInPlaceObject.GetWindow(&hwnd).Succeeded())
                {
                    if (Host.GetHandleNoCreate() != hwnd)
                    {
                        if (hwnd != IntPtr.Zero)
                        {
                            Host.AttachWindow(hwnd);
                            RECT posRect = Host.Bounds;
                            OnActiveXRectChange(&posRect);
                        }
                    }
                }
                else if (Host.AXInPlaceObject is Ole32.IOleInPlaceObjectWindowless)
                {
                    throw new InvalidOperationException(SR.AXWindowlessControl);
                }
            }

            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleClientSite.OnShowWindow(BOOL fShow) => HRESULT.S_OK;

        HRESULT Ole32.IOleClientSite.RequestNewObjectLayout() => HRESULT.E_NOTIMPL;

        // IOleInPlaceSite methods:
        unsafe HRESULT Ole32.IOleInPlaceSite.GetWindow(IntPtr* phwnd)
        {
            if (phwnd == null)
            {
                return HRESULT.E_POINTER;
            }

            *phwnd = User32.GetParent(Host);
            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleInPlaceSite.ContextSensitiveHelp(BOOL fEnterMode) => HRESULT.E_NOTIMPL;

        HRESULT Ole32.IOleInPlaceSite.CanInPlaceActivate() => HRESULT.S_OK;

        unsafe HRESULT Ole32.IOleInPlaceSite.OnInPlaceActivate()
        {
            Host.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
            RECT posRect = Host.Bounds;
            OnActiveXRectChange(&posRect);
            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleInPlaceSite.OnUIActivate()
        {
            Host.ActiveXState = WebBrowserHelper.AXState.UIActive;
            Host.GetParentContainer().OnUIActivate(Host);
            return HRESULT.S_OK;
        }

        unsafe HRESULT Ole32.IOleInPlaceSite.GetWindowContext(
            out Ole32.IOleInPlaceFrame ppFrame,
            out Ole32.IOleInPlaceUIWindow ppDoc,
            RECT* lprcPosRect,
            RECT* lprcClipRect,
            Ole32.OLEINPLACEFRAMEINFO* lpFrameInfo)
        {
            ppDoc = null;
            ppFrame = Host.GetParentContainer();

            if (lprcPosRect == null || lprcClipRect == null)
            {
                return HRESULT.E_POINTER;
            }

            *lprcPosRect = Host.Bounds;
            *lprcClipRect = WebBrowserHelper.GetClipRect();
            if (lpFrameInfo != null)
            {
                lpFrameInfo->cb = (uint)Marshal.SizeOf<Ole32.OLEINPLACEFRAMEINFO>();
                lpFrameInfo->fMDIApp = BOOL.FALSE;
                lpFrameInfo->hAccel = IntPtr.Zero;
                lpFrameInfo->cAccelEntries = 0;
                lpFrameInfo->hwndFrame = (Host.ParentInternal == null) ? IntPtr.Zero : Host.ParentInternal.Handle;
            }

            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleInPlaceSite.Scroll(Size scrollExtant) => HRESULT.S_FALSE;

        HRESULT Ole32.IOleInPlaceSite.OnUIDeactivate(BOOL fUndoable)
        {
            Host.GetParentContainer().OnUIDeactivate(Host);
            if (Host.ActiveXState > WebBrowserHelper.AXState.InPlaceActive)
            {
                Host.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
            }

            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleInPlaceSite.OnInPlaceDeactivate()
        {
            if (Host.ActiveXState == WebBrowserHelper.AXState.UIActive)
            {
                ((Ole32.IOleInPlaceSite)this).OnUIDeactivate(0);
            }

            Host.GetParentContainer().OnInPlaceDeactivate(Host);
            Host.ActiveXState = WebBrowserHelper.AXState.Running;
            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleInPlaceSite.DiscardUndoState() => HRESULT.S_OK;

        HRESULT Ole32.IOleInPlaceSite.DeactivateAndUndo() => Host.AXInPlaceObject.UIDeactivate();

        unsafe HRESULT Ole32.IOleInPlaceSite.OnPosRectChange(RECT* lprcPosRect) => OnActiveXRectChange(lprcPosRect);

        // ISimpleFrameSite methods:
        unsafe HRESULT Ole32.ISimpleFrameSite.PreMessageFilter(IntPtr hWnd, uint msg, IntPtr wp, IntPtr lp, IntPtr* plResult, uint* pdwCookie)
        {
            return HRESULT.S_OK;
        }

        unsafe HRESULT Ole32.ISimpleFrameSite.PostMessageFilter(IntPtr hWnd, uint msg, IntPtr wp, IntPtr lp, IntPtr* plResult, uint dwCookie)
        {
            return HRESULT.S_FALSE;
        }

        // IPropertyNotifySink methods:
        HRESULT Ole32.IPropertyNotifySink.OnChanged(Ole32.DispatchID dispid)
        {
            // Some controls fire OnChanged() notifications when getting values of some properties.
            // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
            if (Host.NoComponentChangeEvents != 0)
            {
                return HRESULT.S_OK;
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

            return HRESULT.S_OK;
        }

        HRESULT Ole32.IPropertyNotifySink.OnRequestEdit(Ole32.DispatchID dispid)
        {
            return HRESULT.S_OK;
        }

        internal virtual void OnPropertyChanged(Ole32.DispatchID dispid)
        {
            try
            {
                ISite site = Host.Site;
                if (site != null)
                {
                    IComponentChangeService changeService = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));

                    if (changeService != null)
                    {
                        try
                        {
                            changeService.OnComponentChanging(Host, null);
                        }
                        catch (CheckoutException coEx)
                        {
                            if (coEx == CheckoutException.Canceled)
                            {
                                return;
                            }
                            throw coEx;
                        }

                        // Now notify the change service that the change was successful.
                        //
                        changeService.OnComponentChanged(Host, null, null, null);
                    }
                }
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                throw;
            }
        }

        internal void StartEvents()
        {
            if (connectionPoint != null)
            {
                return;
            }

            object nativeObject = Host.activeXInstance;
            if (nativeObject != null)
            {
                try
                {
                    connectionPoint = new AxHost.ConnectionPointCookie(nativeObject, this, typeof(Ole32.IPropertyNotifySink));
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
            if (connectionPoint != null)
            {
                connectionPoint.Disconnect();
                connectionPoint = null;
            }
        }

        private unsafe HRESULT OnActiveXRectChange(RECT* lprcPosRect)
        {
            if (lprcPosRect == null)
            {
                return HRESULT.E_INVALIDARG;
            }

            var posRect = new RECT(0, 0, lprcPosRect->right - lprcPosRect->left, lprcPosRect->bottom - lprcPosRect->top);
            var clipRect = WebBrowserHelper.GetClipRect();
            Host.AXInPlaceObject.SetObjectRects(&posRect, &clipRect);
            Host.MakeDirty();
            return HRESULT.S_OK;
        }
    }
}
