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
    public class WebBrowserSiteBase
        : UnsafeNativeMethods.IOleControlSite, UnsafeNativeMethods.IOleClientSite, UnsafeNativeMethods.IOleInPlaceSite, UnsafeNativeMethods.ISimpleFrameSite, UnsafeNativeMethods.IPropertyNotifySink, IDisposable
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

        //
        // IOleControlSite methods:
        //
        int UnsafeNativeMethods.IOleControlSite.OnControlInfoChanged()
        {
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleControlSite.LockInPlaceActive(int fLock)
        {
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleControlSite.GetExtendedControl(out object ppDisp)
        {
            ppDisp = null;
            return NativeMethods.E_NOTIMPL;
        }

        unsafe HRESULT UnsafeNativeMethods.IOleControlSite.TransformCoords(Point *pPtlHimetric, PointF *pPtfContainer, uint dwFlags)
        {
            if (pPtlHimetric == null || pPtfContainer == null)
            {
                return HRESULT.E_INVALIDARG;
            }

            if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_HIMETRICTOCONTAINER) != 0)
            {
                if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_SIZE) != 0)
                {
                    pPtfContainer->X = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->X, WebBrowserHelper.LogPixelsX);
                    pPtfContainer->Y = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->Y, WebBrowserHelper.LogPixelsY);
                }
                else if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_POSITION) != 0)
                {
                    pPtfContainer->X = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->X, WebBrowserHelper.LogPixelsX);
                    pPtfContainer->Y = (float)WebBrowserHelper.HM2Pix(pPtlHimetric->Y, WebBrowserHelper.LogPixelsY);
                }
                else
                {
                    return HRESULT.E_INVALIDARG;
                }
            }
            else if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_CONTAINERTOHIMETRIC) != 0)
            {
                if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_SIZE) != 0)
                {
                    pPtlHimetric->X = WebBrowserHelper.Pix2HM((int)pPtfContainer->X, WebBrowserHelper.LogPixelsX);
                    pPtlHimetric->Y = WebBrowserHelper.Pix2HM((int)pPtfContainer->Y, WebBrowserHelper.LogPixelsY);
                }
                else if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_POSITION) != 0)
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

        int UnsafeNativeMethods.IOleControlSite.TranslateAccelerator(ref NativeMethods.MSG pMsg, int grfModifiers)
        {
            Debug.Assert(!Host.GetAXHostState(WebBrowserHelper.siteProcessedInputKey), "Re-entering UnsafeNativeMethods.IOleControlSite.TranslateAccelerator!!!");
            Host.SetAXHostState(WebBrowserHelper.siteProcessedInputKey, true);

            Message msg = new Message
            {
                Msg = pMsg.message,
                WParam = pMsg.wParam,
                LParam = pMsg.lParam,
                HWnd = pMsg.hwnd
            };

            try
            {
                bool f = ((Control)Host).PreProcessControlMessage(ref msg) == PreProcessControlState.MessageProcessed;
                return f ? NativeMethods.S_OK : NativeMethods.S_FALSE;
            }
            finally
            {
                Host.SetAXHostState(WebBrowserHelper.siteProcessedInputKey, false);
            }
        }

        int UnsafeNativeMethods.IOleControlSite.OnFocus(int fGotFocus)
        {
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleControlSite.ShowPropertyFrame()
        {
            return NativeMethods.E_NOTIMPL;
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

        int UnsafeNativeMethods.IOleClientSite.ShowObject()
        {
            if (Host.ActiveXState >= WebBrowserHelper.AXState.InPlaceActive)
            {
                if (NativeMethods.Succeeded(Host.AXInPlaceObject.GetWindow(out IntPtr hwnd)))
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

        //
        // IOleInPlaceSite methods:
        //
        IntPtr UnsafeNativeMethods.IOleInPlaceSite.GetWindow()
        {
            try
            {
                return UnsafeNativeMethods.GetParent(new HandleRef(Host, Host.Handle));
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                throw;
            }
        }

        int UnsafeNativeMethods.IOleInPlaceSite.ContextSensitiveHelp(int fEnterMode)
        {
            return NativeMethods.E_NOTIMPL;
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

        //
        // ISimpleFrameSite methods:
        //
        int UnsafeNativeMethods.ISimpleFrameSite.PreMessageFilter(IntPtr hwnd, int msg, IntPtr wp, IntPtr lp, ref IntPtr plResult, ref int pdwCookie)
        {
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.ISimpleFrameSite.PostMessageFilter(IntPtr hwnd, int msg, IntPtr wp, IntPtr lp, ref IntPtr plResult, int dwCookie)
        {
            return NativeMethods.S_FALSE;
        }

        //
        // IPropertyNotifySink methods:
        //
        void UnsafeNativeMethods.IPropertyNotifySink.OnChanged(int dispid)
        {
            // Some controls fire OnChanged() notifications when getting values of some properties.
            // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
            //
            if (Host.NoComponentChangeEvents != 0)
            {
                return;
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
        }

        int UnsafeNativeMethods.IPropertyNotifySink.OnRequestEdit(int dispid)
        {
            return NativeMethods.S_OK;
        }

        //
        // Virtual overrides:
        //
        internal virtual void OnPropertyChanged(int dispid)
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
                    connectionPoint = new AxHost.ConnectionPointCookie(nativeObject, this, typeof(UnsafeNativeMethods.IPropertyNotifySink));
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

