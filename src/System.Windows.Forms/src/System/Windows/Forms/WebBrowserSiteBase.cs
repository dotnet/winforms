// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Configuration.Assemblies;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Reflection;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Drawing;    
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Security;

namespace System.Windows.Forms {
    /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase"]/*' />
    /// <devdoc>
    ///     <para>
    /// This class implements the necessary interfaces required for an ActiveX site.
    ///
    /// This class is public, but has an internal constructor so that external
    /// users can only reference the Type (cannot instantiate it directly).
    /// Other classes have to inherit this class and expose it to the outside world.
    ///
    /// This class does not have any public property/method/event by itself.
    /// All implementations of the site interface methods are private, which
    /// means that inheritors who want to override even a single method of one
    /// of these interfaces will have to implement the whole interface.
    ///     </para>
    /// </devdoc>
    public class WebBrowserSiteBase
        : UnsafeNativeMethods.IOleControlSite, UnsafeNativeMethods.IOleClientSite, UnsafeNativeMethods.IOleInPlaceSite, UnsafeNativeMethods.ISimpleFrameSite, UnsafeNativeMethods.IPropertyNotifySink, IDisposable {

        private WebBrowserBase host;
        private AxHost.ConnectionPointCookie connectionPoint;

        //
        // The constructor takes an WebBrowserBase as a parameter, so unfortunately,
        // this cannot be used as a standalone site. It has to be used in conjunction
        // with WebBrowserBase. Perhaps we can change it in future.
        //
        internal WebBrowserSiteBase(WebBrowserBase h) {
            if (h == null) {
                throw new ArgumentNullException(nameof(h));
            }
            this.host = h;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.Dispose"]/*' />
        /// <devdoc>
        ///     <para>
        /// Dispose(release the cookie)
        ///     </para>
        /// </devdoc>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.Dispose"]/*' />
        /// <devdoc>
        ///     <para>
        /// Release the cookie if we're disposing
        ///     </para>
        /// </devdoc>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopEvents();
            }
        }
        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.Host"]/*' />
        /// <devdoc>
        ///     <para>
        /// Retrieves the WebBrowserBase object set in the constructor.
        ///     </para>
        /// </devdoc>
        internal WebBrowserBase Host {
            get {
                return this.host;
            }
        }
        
        //
        // Interface implementations:
        //
        
        //
        // IOleControlSite methods:
        //
        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleControlSite.OnControlInfoChanged"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControlSite.OnControlInfoChanged() {
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleControlSite.LockInPlaceActive"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControlSite.LockInPlaceActive(int fLock) {
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleControlSite.GetExtendedControl"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControlSite.GetExtendedControl(out object ppDisp) {
            ppDisp = null;
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleControlSite.TransformCoords"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControlSite.TransformCoords(NativeMethods._POINTL pPtlHimetric, NativeMethods.tagPOINTF pPtfContainer, int dwFlags) {
            if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_HIMETRICTOCONTAINER)  != 0) {
                if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_SIZE) != 0) {
                    pPtfContainer.x = (float) WebBrowserHelper.HM2Pix(pPtlHimetric.x, WebBrowserHelper.LogPixelsX);
                    pPtfContainer.y = (float) WebBrowserHelper.HM2Pix(pPtlHimetric.y, WebBrowserHelper.LogPixelsY);
                }
                else if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_POSITION) != 0) {
                    pPtfContainer.x = (float) WebBrowserHelper.HM2Pix(pPtlHimetric.x, WebBrowserHelper.LogPixelsX);
                    pPtfContainer.y = (float) WebBrowserHelper.HM2Pix(pPtlHimetric.y, WebBrowserHelper.LogPixelsY);
                }
                else {
                    return NativeMethods.E_INVALIDARG;
                }
            }
            else if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_CONTAINERTOHIMETRIC) != 0) {
                if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_SIZE) != 0) {
                    pPtlHimetric.x = WebBrowserHelper.Pix2HM((int)pPtfContainer.x, WebBrowserHelper.LogPixelsX);
                    pPtlHimetric.y = WebBrowserHelper.Pix2HM((int)pPtfContainer.y, WebBrowserHelper.LogPixelsY);
                }
                else if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_POSITION) != 0) {
                    pPtlHimetric.x = WebBrowserHelper.Pix2HM((int)pPtfContainer.x, WebBrowserHelper.LogPixelsX);
                    pPtlHimetric.y = WebBrowserHelper.Pix2HM((int)pPtfContainer.y, WebBrowserHelper.LogPixelsY);
                }
                else {
                    return NativeMethods.E_INVALIDARG;
                }
            }
            else {
                return NativeMethods.E_INVALIDARG;
            }

            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleControlSite.TranslateAccelerator"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControlSite.TranslateAccelerator(ref NativeMethods.MSG pMsg, int grfModifiers) {
            Debug.Assert(!this.Host.GetAXHostState(WebBrowserHelper.siteProcessedInputKey), "Re-entering UnsafeNativeMethods.IOleControlSite.TranslateAccelerator!!!");
            this.Host.SetAXHostState(WebBrowserHelper.siteProcessedInputKey, true);

            Message msg = new Message();
            msg.Msg = pMsg.message;
            msg.WParam = pMsg.wParam;
            msg.LParam = pMsg.lParam;
            msg.HWnd = pMsg.hwnd;
            
            try {
                bool f = ((Control)this.Host).PreProcessControlMessage(ref msg) == PreProcessControlState.MessageProcessed;
                return f ? NativeMethods.S_OK : NativeMethods.S_FALSE;
            }
            finally {
                this.Host.SetAXHostState(WebBrowserHelper.siteProcessedInputKey, false);
            }
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleControlSite.OnFocus"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControlSite.OnFocus(int fGotFocus) {
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleControlSite.ShowPropertyFrame"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControlSite.ShowPropertyFrame() {
            return NativeMethods.E_NOTIMPL;
        }

        //
        // IOleClientSite methods:
        //
        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleClientSite.SaveObject"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleClientSite.SaveObject() {
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleClientSite.GetMoniker"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleClientSite.GetMoniker(int dwAssign, int dwWhichMoniker, out Object moniker) {
            moniker = null;
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleClientSite.GetContainer"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleClientSite.GetContainer(out UnsafeNativeMethods.IOleContainer container) {
            container = this.Host.GetParentContainer();
            return NativeMethods.S_OK;
        }


        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleClientSite.ShowObject"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleClientSite.ShowObject() {
            if (this.Host.ActiveXState >= WebBrowserHelper.AXState.InPlaceActive) {
                IntPtr hwnd;
                if (NativeMethods.Succeeded(this.Host.AXInPlaceObject.GetWindow(out hwnd))) {
                    if (this.Host.GetHandleNoCreate() != hwnd) {
                        if (hwnd != IntPtr.Zero) {
                            this.Host.AttachWindow(hwnd);
                            this.OnActiveXRectChange(new NativeMethods.COMRECT(this.Host.Bounds));
                        }
                    }
                }
                else if (this.Host.AXInPlaceObject is UnsafeNativeMethods.IOleInPlaceObjectWindowless) {
                    throw new InvalidOperationException(SR.AXWindowlessControl);
                }
            }
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleClientSite.OnShowWindow"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleClientSite.OnShowWindow(int fShow) {
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleClientSite.RequestNewObjectLayout"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleClientSite.RequestNewObjectLayout() {
            return NativeMethods.E_NOTIMPL;
        }

        //
        // IOleInPlaceSite methods:
        //
        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.GetWindow"]/*' />
        /// <internalonly/>
        IntPtr UnsafeNativeMethods.IOleInPlaceSite.GetWindow() {
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

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.ContextSensitiveHelp"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.ContextSensitiveHelp(int fEnterMode) {
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.CanInPlaceActivate"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.CanInPlaceActivate() {
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.OnInPlaceActivate"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.OnInPlaceActivate() {
            this.Host.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
            this.OnActiveXRectChange(new NativeMethods.COMRECT(this.Host.Bounds));
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.OnUIActivate"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.OnUIActivate() {
            this.Host.ActiveXState = WebBrowserHelper.AXState.UIActive;
            this.Host.GetParentContainer().OnUIActivate(this.Host);
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.GetWindowContext"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.GetWindowContext(out UnsafeNativeMethods.IOleInPlaceFrame ppFrame, out UnsafeNativeMethods.IOleInPlaceUIWindow ppDoc,
                                             NativeMethods.COMRECT lprcPosRect, NativeMethods.COMRECT lprcClipRect, NativeMethods.tagOIFI lpFrameInfo) {
            ppDoc = null;
            ppFrame = this.Host.GetParentContainer();
            
            lprcPosRect.left = this.Host.Bounds.X;
            lprcPosRect.top = this.Host.Bounds.Y;
            lprcPosRect.right = this.Host.Bounds.Width + this.Host.Bounds.X;
            lprcPosRect.bottom = this.Host.Bounds.Height + this.Host.Bounds.Y;
            
            lprcClipRect = WebBrowserHelper.GetClipRect();
            if (lpFrameInfo != null) {
                lpFrameInfo.cb = Marshal.SizeOf(typeof(NativeMethods.tagOIFI));
                lpFrameInfo.fMDIApp = false;
                lpFrameInfo.hAccel = IntPtr.Zero;
                lpFrameInfo.cAccelEntries = 0;
                lpFrameInfo.hwndFrame = (this.Host.ParentInternal == null) ? IntPtr.Zero : this.Host.ParentInternal.Handle;
            }
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.Scroll"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.Scroll(NativeMethods.tagSIZE scrollExtant) {
            return NativeMethods.S_FALSE;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.OnUIDeactivate"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.OnUIDeactivate(int fUndoable) {
            this.Host.GetParentContainer().OnUIDeactivate(this.Host);
            if (this.Host.ActiveXState > WebBrowserHelper.AXState.InPlaceActive) {
                this.Host.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
            }
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.OnInPlaceDeactivate"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.OnInPlaceDeactivate() {
            if (this.Host.ActiveXState == WebBrowserHelper.AXState.UIActive) {
                ((UnsafeNativeMethods.IOleInPlaceSite)this).OnUIDeactivate(0);
            }

            this.Host.GetParentContainer().OnInPlaceDeactivate(this.Host);
            this.Host.ActiveXState = WebBrowserHelper.AXState.Running;
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.DiscardUndoState"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.DiscardUndoState() {
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.DeactivateAndUndo"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.DeactivateAndUndo() {
            return this.Host.AXInPlaceObject.UIDeactivate();
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IOleInPlaceSite.OnPosRectChange"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceSite.OnPosRectChange(NativeMethods.COMRECT lprcPosRect) {
            return this.OnActiveXRectChange(lprcPosRect);
        }

        //
        // ISimpleFrameSite methods:
        //
        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.ISimpleFrameSite.PreMessageFilter"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.ISimpleFrameSite.PreMessageFilter(IntPtr hwnd, int msg, IntPtr wp, IntPtr lp, ref IntPtr plResult, ref int pdwCookie) {
            return NativeMethods.S_OK;
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.ISimpleFrameSite.PostMessageFilter"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.ISimpleFrameSite.PostMessageFilter(IntPtr hwnd, int msg, IntPtr wp, IntPtr lp, ref IntPtr plResult, int dwCookie) {
            return NativeMethods.S_FALSE;
        }

        //
        // IPropertyNotifySink methods:
        //
        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IPropertyNotifySink.OnChanged"]/*' />
        /// <internalonly/>
        void UnsafeNativeMethods.IPropertyNotifySink.OnChanged(int dispid) {
            // Some controls fire OnChanged() notifications when getting values of some properties.
            // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
            //
            if (this.Host.NoComponentChangeEvents != 0)
                return;

            this.Host.NoComponentChangeEvents++;
            try
            {
                OnPropertyChanged(dispid);
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                throw;
            }
            finally {
                this.Host.NoComponentChangeEvents--;
            }
        }

        /// <include file='doc\WebBrowserSiteBase.uex' path='docs/doc[@for="WebBrowserSiteBase.UnsafeNativeMethods.IPropertyNotifySink.OnRequestEdit"]/*' />
        /// <internalonly/>
        int UnsafeNativeMethods.IPropertyNotifySink.OnRequestEdit(int dispid) {
            return NativeMethods.S_OK;
        }


        //
        // Virtual overrides:
        //
        internal virtual void OnPropertyChanged(int dispid) {
            try
            {
                ISite site = this.Host.Site;
                if (site != null)
                {
                    IComponentChangeService changeService = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));

                    if (changeService != null)
                    {
                        try
                        {
                            changeService.OnComponentChanging(this.Host, null);
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
                        changeService.OnComponentChanged(this.Host, null, null, null);
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
        internal WebBrowserBase GetAXHost() {
            return this.Host;
        }

        internal void StartEvents() {
            if (connectionPoint != null)
                return;

            Object nativeObject = this.Host.activeXInstance;
            if (nativeObject != null) {
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

        internal void StopEvents() {
            if (connectionPoint != null) {
                connectionPoint.Disconnect();
                connectionPoint = null;
            }
        }

        private int OnActiveXRectChange(NativeMethods.COMRECT lprcPosRect) {
            this.Host.AXInPlaceObject.SetObjectRects(
                NativeMethods.COMRECT.FromXYWH(0, 0, lprcPosRect.right - lprcPosRect.left, lprcPosRect.bottom - lprcPosRect.top),
                WebBrowserHelper.GetClipRect());
            this.Host.MakeDirty();
            return NativeMethods.S_OK;
        }
    }
}

