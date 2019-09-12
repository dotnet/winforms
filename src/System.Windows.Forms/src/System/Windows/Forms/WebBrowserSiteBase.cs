// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        UnsafeNativeMethods.IOleClientSite,
        UnsafeNativeMethods.IOleInPlaceSite,
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

        //
        // Interface implementations:
        //

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

        HRESULT Ole32.IOleControlSite.OnFocus(BOOL fGotFocus)
        {
            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleControlSite.ShowPropertyFrame()
        {
            return HRESULT.E_NOTIMPL;
        }

        //
        // IOleClientSite methods:
        //
        int UnsafeNativeMethods.IOleClientSite.SaveObject()
        {
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleClientSite.GetMoniker(int dwAssign, int dwWhichMoniker, out object moniker)
        {
            moniker = null;
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleClientSite.GetContainer(out UnsafeNativeMethods.IOleContainer container)
        {
            container = Host.GetParentContainer();
            return NativeMethods.S_OK;
        }

        unsafe int UnsafeNativeMethods.IOleClientSite.ShowObject()
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
                            OnActiveXRectChange(new NativeMethods.COMRECT(Host.Bounds));
                        }
                    }
                }
                else if (Host.AXInPlaceObject is UnsafeNativeMethods.IOleInPlaceObjectWindowless)
                {
                    throw new InvalidOperationException(SR.AXWindowlessControl);
                }
            }
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleClientSite.OnShowWindow(int fShow)
        {
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleClientSite.RequestNewObjectLayout()
        {
            return NativeMethods.E_NOTIMPL;
        }

        // IOleInPlaceSite methods:
        unsafe HRESULT UnsafeNativeMethods.IOleInPlaceSite.GetWindow(IntPtr* phwnd)
        {
            if (phwnd == null)
            {
                return HRESULT.E_POINTER;
            }

            *phwnd = UnsafeNativeMethods.GetParent(new HandleRef(Host, Host.Handle));
            return HRESULT.S_OK;
        }

        HRESULT UnsafeNativeMethods.IOleInPlaceSite.ContextSensitiveHelp(BOOL fEnterMode)
        {
            return HRESULT.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleInPlaceSite.CanInPlaceActivate()
        {
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleInPlaceSite.OnInPlaceActivate()
        {
            Host.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
            OnActiveXRectChange(new NativeMethods.COMRECT(Host.Bounds));
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleInPlaceSite.OnUIActivate()
        {
            Host.ActiveXState = WebBrowserHelper.AXState.UIActive;
            Host.GetParentContainer().OnUIActivate(Host);
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleInPlaceSite.GetWindowContext(out UnsafeNativeMethods.IOleInPlaceFrame ppFrame, out UnsafeNativeMethods.IOleInPlaceUIWindow ppDoc,
                                             NativeMethods.COMRECT lprcPosRect, NativeMethods.COMRECT lprcClipRect, NativeMethods.tagOIFI lpFrameInfo)
        {
            ppDoc = null;
            ppFrame = Host.GetParentContainer();

            lprcPosRect.left = Host.Bounds.X;
            lprcPosRect.top = Host.Bounds.Y;
            lprcPosRect.right = Host.Bounds.Width + Host.Bounds.X;
            lprcPosRect.bottom = Host.Bounds.Height + Host.Bounds.Y;

            lprcClipRect = WebBrowserHelper.GetClipRect();
            if (lpFrameInfo != null)
            {
                lpFrameInfo.cb = Marshal.SizeOf<NativeMethods.tagOIFI>();
                lpFrameInfo.fMDIApp = false;
                lpFrameInfo.hAccel = IntPtr.Zero;
                lpFrameInfo.cAccelEntries = 0;
                lpFrameInfo.hwndFrame = (Host.ParentInternal == null) ? IntPtr.Zero : Host.ParentInternal.Handle;
            }
            return NativeMethods.S_OK;
        }

        Interop.HRESULT UnsafeNativeMethods.IOleInPlaceSite.Scroll(Size scrollExtant)
        {
            return Interop.HRESULT.S_FALSE;
        }

        int UnsafeNativeMethods.IOleInPlaceSite.OnUIDeactivate(int fUndoable)
        {
            Host.GetParentContainer().OnUIDeactivate(Host);
            if (Host.ActiveXState > WebBrowserHelper.AXState.InPlaceActive)
            {
                Host.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
            }
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleInPlaceSite.OnInPlaceDeactivate()
        {
            if (Host.ActiveXState == WebBrowserHelper.AXState.UIActive)
            {
                ((UnsafeNativeMethods.IOleInPlaceSite)this).OnUIDeactivate(0);
            }

            Host.GetParentContainer().OnInPlaceDeactivate(Host);
            Host.ActiveXState = WebBrowserHelper.AXState.Running;
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleInPlaceSite.DiscardUndoState()
        {
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleInPlaceSite.DeactivateAndUndo()
        {
            return Host.AXInPlaceObject.UIDeactivate();
        }

        int UnsafeNativeMethods.IOleInPlaceSite.OnPosRectChange(NativeMethods.COMRECT lprcPosRect)
        {
            return OnActiveXRectChange(lprcPosRect);
        }

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

        //
        // Internal helper methods:
        //
        internal WebBrowserBase GetAXHost()
        {
            return Host;
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

        private int OnActiveXRectChange(NativeMethods.COMRECT lprcPosRect)
        {
            Host.AXInPlaceObject.SetObjectRects(
                NativeMethods.COMRECT.FromXYWH(0, 0, lprcPosRect.right - lprcPosRect.left, lprcPosRect.bottom - lprcPosRect.top),
                WebBrowserHelper.GetClipRect());
            Host.MakeDirty();
            return NativeMethods.S_OK;
        }
    }
}

