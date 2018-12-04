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
using System.Diagnostics.CodeAnalysis;
using System;
using System.Reflection;
using System.Globalization;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.IO;
using System.Drawing;    
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.ComponentModel.Design;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Security;

namespace System.Windows.Forms {
    /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase"]/*' />
    /// <devdoc>
    ///     <para>
    /// Wraps ActiveX controls and exposes them as fully featured windows forms controls
    /// (by inheriting from Control). Some of Control's properties that don't make sense
    /// for ActiveX controls are blocked here (by setting Browsable attributes on some and
    /// throwing exceptions from others), to make life easy for the inheritors.
    ///
    /// Inheritors of this class simply need to concentrate on defining & implementing the
    /// properties/methods/events of the specific ActiveX control they are wrapping, the
    /// default properties etc and the code to implement the activation etc. are
    /// encapsulated in the class below.
    ///
    /// The classid of the ActiveX control is specified in the constructor.
    ///     </para>
    /// </devdoc>
    [ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    PermissionSetAttribute(SecurityAction.LinkDemand, Name="FullTrust"),
    PermissionSetAttribute(SecurityAction.InheritanceDemand, Name="FullTrust"),
    DefaultProperty(nameof(Name)), DefaultEvent(nameof(Enter)),
    Designer("System.Windows.Forms.Design.AxDesigner, " + AssemblyRef.SystemDesign)]
    public class WebBrowserBase : Control {
        //
        // Privates fields:
        //
        private WebBrowserHelper.AXState axState = WebBrowserHelper.AXState.Passive;
        private WebBrowserHelper.AXState axReloadingState = WebBrowserHelper.AXState.Passive;
        private WebBrowserHelper.AXEditMode axEditMode = WebBrowserHelper.AXEditMode.None;
        private bool        inRtlRecreate=false;
        private BitVector32 axHostState = new BitVector32();
        private WebBrowserHelper.SelectionStyle selectionStyle = WebBrowserHelper.SelectionStyle.NotSelected;
        private int noComponentChange = 0;
        private WebBrowserSiteBase axSite;
        private ContainerControl containingControl;
        private IntPtr hwndFocus = IntPtr.Zero;
        private EventHandler selectionChangeHandler;
        private Guid   clsid;
        // Pointers to the ActiveX object: Interface pointers are cached for perf.
        private UnsafeNativeMethods.IOleObject axOleObject;
        private UnsafeNativeMethods.IOleInPlaceObject axOleInPlaceObject;
        private UnsafeNativeMethods.IOleInPlaceActiveObject axOleInPlaceActiveObject;
        private UnsafeNativeMethods.IOleControl axOleControl;
        private WebBrowserBaseNativeWindow axWindow;
        // We need to change the size of the inner ActiveX control before the 
        //WebBrowserBase control's size is changed (i.e., before WebBrowserBase.Bounds 
        //is changed) for better visual effect. We use this field to know what size 
        //the WebBrowserBase control is changing to.
        private Size webBrowserBaseChangingSize = Size.Empty;
        private WebBrowserContainer wbContainer = null;

        // This flags the WebBrowser not to process dialog keys when the ActiveX control is doing it
        // and calls back into the WebBrowser for some reason.
        private bool ignoreDialogKeys;

        //
        // Internal fields:
        //
        internal WebBrowserContainer container;
        internal object activeXInstance;    // this is internal to avoid the security demand



        //
        // SECURITY NOTE: This must be internal, because by specifying a
        // clsid here, someone will be able to create a native com object.
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.WebBrowserBase"]/*' />
        /// <devdoc>
        ///     <para>
        /// Creates a new instance of a WinForms control which wraps an ActiveX control
        /// given by the clsid parameter.
        ///     </para>
        /// </devdoc>
        internal WebBrowserBase(string clsidString) : base() {
            if (Application.OleRequired() != ApartmentState.STA) {
                throw new ThreadStateException(string.Format(SR.AXMTAThread, clsidString));
            }

            this.SetStyle(ControlStyles.UserPaint, false);

            this.clsid = new Guid(clsidString);
            this.webBrowserBaseChangingSize.Width = -1;  // Invalid value. Use WebBrowserBase.Bounds instead, when this is the case.
            this.SetAXHostState(WebBrowserHelper.isMaskEdit, this.clsid.Equals(WebBrowserHelper.maskEdit_Clsid));
        }



        //
        // Public properties:
        //

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.ActiveXInstance"]/*' />
        /// <devdoc>
        ///     <para>
        /// Returns the native webbrowser object that this control wraps. Needs FullTrust to access.
        ///     </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object ActiveXInstance {
            get {
                return activeXInstance;
            }
        }

        //
        // Virtual methods:
        //
        // The following are virtual methods that derived-classes can override
        // (only in full-trust though).
        //
        
        //
        // The native ActiveX control QI's for interfaces on it's site to see if
        // it needs to change its behavior. Since the WebBrowserSiteBaseBase class is generic,
        // it only implements site interfaces that are generic to all sites. QI's
        // for any more specific interfaces will fail. This is a problem if anyone
        // wants to support any other interfaces on the site. In order to overcome
        // this, one needs to extend WebBrowserSiteBaseBase and implement any additional interfaces
        // needed.
        //
        // ActiveX wrapper controls that derive from this class should override the
        // below method and return their own WebBrowserSiteBaseBase derived object.
        //
        // This method is protected by an InheritanceDemand because extending a
        // site is strictly an advanced feature for which one needs UnmanagedCode
        // permissions.
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.CreateWebBrowserSiteBase"]/*' />
        /// <devdoc>
        ///     <para>
        /// Returns an object that will be set as the site for the native ActiveX control.
        /// Implementors of the site can derive from <see cref='System.Windows.Forms.WebBrowserSiteBase'/> class.
        ///     </para>
        /// </devdoc>
        protected virtual WebBrowserSiteBase CreateWebBrowserSiteBase() {
            return new WebBrowserSiteBase(this);
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.AttachInterfaces"]/*' />
        /// <devdoc>
        ///     <para>
        /// This will be called when the native ActiveX control has just been created.
        /// Inheritors of this class can override this method to cast the nativeActiveXObject
        /// parameter to the appropriate interface. They can then cache this interface
        /// value in a member variable. However, they must release this value when
        /// DetachInterfaces is called (by setting the cached interface variable to null).
        ///     </para>
        /// </devdoc>
        protected virtual void AttachInterfaces(object nativeActiveXObject) {
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.DetachInterfaces"]/*' />
        /// <devdoc>
        ///     <para>
        /// See AttachInterfaces for a description of when to override DetachInterfaces.
        ///     </para>
        /// </devdoc>
        protected virtual void DetachInterfaces() {
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.CreateSink"]/*' />
        /// <devdoc>
        ///     <para>
        /// This will be called when we are ready to start listening to events.
        /// Inheritors can override this method to hook their own connection points.
        ///     </para>
        /// </devdoc>
        protected virtual void CreateSink() {
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.DetachSink"]/*' />
        /// <devdoc>
        ///     <para>
        /// This will be called when it is time to stop listening to events.
        /// This is where inheritors have to disconnect their connection points.
        ///     </para>
        /// </devdoc>
        protected virtual void DetachSink() {
        }

        //DrawToBitmap doesn't work for this control, so we should hide it.  We'll
        //still call base so that this has a chance to work if it can.
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
        {
            base.DrawToBitmap(bitmap, targetBounds);
        }


        //
        // Overriding methods: Overrides of some of Control's virtual methods.
        //

        //
        // Sets the site of this component. A non-null value indicates that this
        // component has been added to a container, and a null value indicates that
        // this component is being removed from a container.
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Site"]/*' />
        public override ISite Site {
            set {
                bool hadSelectionHandler = this.RemoveSelectionHandler();

                base.Site = value;

                if (hadSelectionHandler) {
                    this.AddSelectionHandler();
                }
            }
        }

        //
        // We have to resize the ActiveX control when our size changes.
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.OnBoundsUpdate"]/*' />
        internal override void OnBoundsUpdate(int x, int y, int width, int height)
        {
            //
            // If the ActiveX control is already InPlaceActive, make sure
            // it's bounds also change.
            if (this.ActiveXState >= WebBrowserHelper.AXState.InPlaceActive) {
                try {
                    this.webBrowserBaseChangingSize.Width = width;
                    this.webBrowserBaseChangingSize.Height = height;
                    this.AXInPlaceObject.SetObjectRects(new NativeMethods.COMRECT(new Rectangle(0, 0, width, height)), WebBrowserHelper.GetClipRect());
                }
                finally {
                    this.webBrowserBaseChangingSize.Width = -1;  // Invalid value. Use WebBrowserBase.Bounds instead, when this is the case.
                }
            }

            base.OnBoundsUpdate(x, y, width, height);
        }

        [SuppressMessage("Microsoft.Security", "CA2114:MethodSecurityShouldBeASupersetOfType")]
        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            return ignoreDialogKeys ? false : base.ProcessDialogKey(keyData);
        }

        //
        // Let us assume that TAB key was pressed. In this case, we should first
        // give a chance to the ActiveX control to see if it wants to change focus
        // to other subitems within it. If we did not give the ActiveX control the
        // first chance to handle the key stroke, and called base.PreProcessMessage,
        // the focus would be changed to the next control on the form! We don't want
        // that!!
        //
        // If the ActiveX control doesn't want to handle the key, it calls back into
        // WebBrowserSiteBase's IOleControlSite.TranslateAccelerator implementation. There, we
        // set a flag and call back into this method. In this method, we first check
        // if this flag is set. If so, we call base.PreProcessMessage.
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.PreProcessMessage"]/*' />
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly")]
        public override bool PreProcessMessage(ref Message msg)
        {
            if (IsUserMode) {
                if (this.GetAXHostState(WebBrowserHelper.siteProcessedInputKey)) {
                    // In this case, the control called us back through IOleControlSite
                    // and is now giving us a chance to see if we want to process it.
                    return base.PreProcessMessage(ref msg);
                }


                // Convert Message to NativeMethods.MSG
                NativeMethods.MSG win32Message = new NativeMethods.MSG();
                win32Message.message = msg.Msg;
                win32Message.wParam = msg.WParam;
                win32Message.lParam = msg.LParam;
                win32Message.hwnd = msg.HWnd;

                this.SetAXHostState(WebBrowserHelper.siteProcessedInputKey, false);
                try 
                {
                    if (axOleInPlaceObject != null)
                    {
                        // Give the ActiveX control a chance to process this key by calling
                        // IOleInPlaceActiveObject::TranslateAccelerator.
                        int hr = axOleInPlaceActiveObject.TranslateAccelerator(ref win32Message);

                        if (hr == NativeMethods.S_OK)
                        {
                            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "\t Message translated to " + win32Message);
                            return true;
                        }
                        else
                        {
                            //
                            // win32Message may have been modified. Lets copy it back.
                            msg.Msg = win32Message.message;
                            msg.WParam = win32Message.wParam;
                            msg.LParam = win32Message.lParam;
                            msg.HWnd = win32Message.hwnd;

                            if (hr == NativeMethods.S_FALSE)
                            {
                                // Same code as in AxHost (ignore dialog keys here).
                                // We have the same problem here
                                bool ret = false;

                                ignoreDialogKeys = true;
                                try
                                {
                                    ret = base.PreProcessMessage(ref msg);
                                }
                                finally
                                {
                                    ignoreDialogKeys = false;
                                }
                                return ret;
                            }
                            else if (this.GetAXHostState(WebBrowserHelper.siteProcessedInputKey))
                            {
                                Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "\t Message processed by site. Calling base.PreProcessMessage() " + msg);
                                return base.PreProcessMessage(ref msg);
                            }
                            else
                            {
                                Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "\t Message not processed by site. Returning false. " + msg);
                                return false;
                            }
                        }
                    }
                }
                finally {
                    this.SetAXHostState(WebBrowserHelper.siteProcessedInputKey, false);
                }
            }
            
            return false;
        }


        //
        // Process a mnemonic character. This is done by manufacturing a
        // WM_SYSKEYDOWN message and passing it to the ActiveX control.
        //
        // We can't decide just by ourselves whether we can process the
        // mnemonic. We have to ask the ActiveX control for it.
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.ProcessMnemonic"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2114:MethodSecurityShouldBeASupersetOfType")]
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal override bool ProcessMnemonic(char charCode) {
            bool processed = false;

            if (CanSelect) {
                try {
                    NativeMethods.tagCONTROLINFO ctlInfo = new NativeMethods.tagCONTROLINFO();
                    int hr = this.axOleControl.GetControlInfo(ctlInfo);
                    if (NativeMethods.Succeeded(hr)) {
                        //
                        // Sadly, we don't have a message so we must fake one ourselves.
                        // The message we are faking is a WM_SYSKEYDOWN with the right
                        // alt key setting.
                        NativeMethods.MSG msg = new NativeMethods.MSG();
                        msg.hwnd = IntPtr.Zero;
                        msg.message = NativeMethods.WM_SYSKEYDOWN;
                        msg.wParam = (IntPtr) Char.ToUpper(charCode, CultureInfo.CurrentCulture);
                        msg.lParam = (IntPtr) 0x20180001;
                        msg.time = SafeNativeMethods.GetTickCount();
                        NativeMethods.POINT p = new NativeMethods.POINT();
                        UnsafeNativeMethods.GetCursorPos(p);
                        msg.pt_x = p.x;
                        msg.pt_y = p.y;
                        if (SafeNativeMethods.IsAccelerator(new HandleRef(ctlInfo, ctlInfo.hAccel), ctlInfo.cAccel, ref msg, null)) {
                            this.axOleControl.OnMnemonic(ref msg);
                            FocusInternal();
                            processed = true;
                        }
                    }
                }
                catch (Exception ex) {
                    if (ClientUtils.IsCriticalException(ex)) {
                        throw;
                    }
                    Debug.Fail("error in processMnemonic");
                }
            }
            return processed;
        }

        //
        // Certain messages are forwarder directly to the ActiveX control,
        // others are first processed by the wndproc of Control
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.WndProc"]/*' />
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
         PermissionSetAttribute(SecurityAction.LinkDemand, Name="FullTrust"),
         PermissionSetAttribute(SecurityAction.InheritanceDemand, Name="FullTrust")]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                //
                // Things we explicitly ignore and pass to the ActiveX's windproc
                //
                case NativeMethods.WM_ERASEBKGND:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_NOTIFYFORMAT:
                case NativeMethods.WM_SETCURSOR:
                case NativeMethods.WM_SYSCOLORCHANGE:
                case NativeMethods.WM_LBUTTONDBLCLK:
                case NativeMethods.WM_LBUTTONUP:
                case NativeMethods.WM_MBUTTONDBLCLK:
                case NativeMethods.WM_MBUTTONUP:
                case NativeMethods.WM_RBUTTONDBLCLK:
                case NativeMethods.WM_RBUTTONUP:
                case NativeMethods.WM_CONTEXTMENU:
                //
                // Some of the MSComCtl controls respond to this message to do some
                // custom painting. So, we should just pass this message through.
                case NativeMethods.WM_DRAWITEM:
                    DefWndProc(ref m);
                    break;

                case NativeMethods.WM_COMMAND:
                    if (!ReflectMessageInternal(m.LParam, ref m))
                        DefWndProc(ref m);
                    break;
                
                case NativeMethods.WM_HELP:
                    // We want to both fire the event, and let the ActiveX have the message...
                    base.WndProc(ref m);
                    DefWndProc(ref m);
                    break;

                case NativeMethods.WM_LBUTTONDOWN:
                case NativeMethods.WM_MBUTTONDOWN:
                case NativeMethods.WM_RBUTTONDOWN:
                case NativeMethods.WM_MOUSEACTIVATE:
                    if (!DesignMode) {
                        if (containingControl != null && containingControl.ActiveControl != this) {
                            FocusInternal();
                        } 
                    }
                    DefWndProc(ref m);
                    break;

                case NativeMethods.WM_KILLFOCUS:
                    hwndFocus = (IntPtr)m.WParam;
                    try {
                        base.WndProc(ref m);
                    }
                    finally {
                         hwndFocus = IntPtr.Zero;
                    }
                    break;    

                case NativeMethods.WM_DESTROY:
                    //
                    // If we are currently in a state of InPlaceActive or above,
                    // we should first reparent the ActiveX control to our parking
                    // window before we transition to a state below InPlaceActive.
                    // Otherwise we face all sorts of problems when we try to
                    // transition back to a state >= InPlaceActive.
                    //
                    if (this.ActiveXState >= WebBrowserHelper.AXState.InPlaceActive) {
                        IntPtr hwndInPlaceObject;
                        if (NativeMethods.Succeeded(this.AXInPlaceObject.GetWindow(out hwndInPlaceObject))) {
                            Application.ParkHandle(new HandleRef(this.AXInPlaceObject, hwndInPlaceObject));
                        }
                    }

                    if (RecreatingHandle) {
                        axReloadingState = axState;
                    }

                    //
                    // If the ActiveX control was holding on to our handle we need
                    // to get it to throw it away. This, we do by transitioning it
                    // down below InPlaceActivate (because it is when transitioning
                    // up to InPlaceActivate that the ActiveX control grabs our handle).
                    TransitionDownTo(WebBrowserHelper.AXState.Running);

                    if (this.axWindow != null) {
                        this.axWindow.ReleaseHandle();
                    }

                    OnHandleDestroyed(EventArgs.Empty);
                    break;

                default:
                    if (m.Msg == WebBrowserHelper.REGMSG_MSG) {
                        m.Result = (IntPtr)WebBrowserHelper.REGMSG_RETVAL;
                    }
                    else {
                        base.WndProc(ref m);
                    }
                    break;
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.OnParentChanged"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnParentChanged(EventArgs e) {
            Control parent = ParentInternal;
            if ((Visible && parent != null && parent.Visible) || this.IsHandleCreated) {
                TransitionUpTo(WebBrowserHelper.AXState.InPlaceActive);
            }
            base.OnParentChanged(e);
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.OnVisibleChanged"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnVisibleChanged(EventArgs e) {
            if (Visible && !Disposing && !IsDisposed) {
                TransitionUpTo(WebBrowserHelper.AXState.InPlaceActive);
            }
            base.OnVisibleChanged(e);
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.OnGotFocus"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnGotFocus(EventArgs e) {
            if (this.ActiveXState < WebBrowserHelper.AXState.UIActive) {
                TransitionUpTo(WebBrowserHelper.AXState.UIActive);
            }
            base.OnGotFocus(e);
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.OnLostFocus"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);

            // If the focus goes from our control window to one of the child windows,
            // we should not deactivate.
            if (!this.ContainsFocus) {
                TransitionDownTo(WebBrowserHelper.AXState.InPlaceActive);
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnRightToLeftChanged(EventArgs e) {
            //Do nothing: no point in recreating the handle when we don't obey RTL
        }

        //
        // Can't select the control until the ActiveX control is InPlaceActive.
        //
        internal override bool CanSelectCore() {
            return this.ActiveXState >= WebBrowserHelper.AXState.InPlaceActive ?
                base.CanSelectCore() : false;
        }

        internal override bool AllowsKeyboardToolTip() {
            return false;
        }

        //
        // We have to inform the ActiveX control that an ambient property
        // has changed.
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.OnFontChanged"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            AmbientChanged(NativeMethods.ActiveX.DISPID_AMBIENT_FONT);
        }

        //
        // We have to inform the ActiveX control that an ambient property
        // has changed.
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.OnForeColorChanged"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnForeColorChanged(EventArgs e) {
            base.OnForeColorChanged(e);
            AmbientChanged(NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR);
        }

        //
        // We have to inform the ActiveX control that an ambient property
        // has changed.
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.OnBackColorChanged"]/*' />
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        protected override void OnBackColorChanged(EventArgs e) {
            base.OnBackColorChanged(e);
            AmbientChanged(NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR);
        }

        internal override void RecreateHandleCore() {
            if (!inRtlRecreate) {
                base.RecreateHandleCore();
            }
        }
        

        //
        // TransitionDownTo Passive when we are being disposed.
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Dispose"]/*' />
        protected override void Dispose(bool disposing) {
            if (disposing) {
                TransitionDownTo(WebBrowserHelper.AXState.Passive);
            }
            base.Dispose(disposing);
        }




        //
        // Internal helper methods:
        //

#if false
        // FxCop: Currently not used
        internal Guid ClassId {
            get {
                return clsid;
            }
        }
#endif

        internal WebBrowserHelper.AXState ActiveXState {
            get {
                return axState;
            }
            set {
                axState = value;
            }
        }

        internal bool GetAXHostState(int mask) {
            return this.axHostState[mask];
        }
        
        internal void SetAXHostState(int mask, bool value) {
            this.axHostState[mask] = value;
        }

        internal IntPtr GetHandleNoCreate() {
            return IsHandleCreated ? Handle : IntPtr.Zero;
        }

        internal void TransitionUpTo(WebBrowserHelper.AXState state) {
            if (!this.GetAXHostState(WebBrowserHelper.inTransition)) {
                this.SetAXHostState(WebBrowserHelper.inTransition, true);

                try {
                    while (state > this.ActiveXState) {
                        switch (this.ActiveXState) {
                            case WebBrowserHelper.AXState.Passive:
                                TransitionFromPassiveToLoaded();
                                Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.Loaded, "Failed transition");
                                break;
                            case WebBrowserHelper.AXState.Loaded:
                                TransitionFromLoadedToRunning();
                                Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.Running, "Failed transition");
                                break;
                            case WebBrowserHelper.AXState.Running:
                                TransitionFromRunningToInPlaceActive();
                                Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.InPlaceActive, "Failed transition");
                                break;
                            case WebBrowserHelper.AXState.InPlaceActive:
                                TransitionFromInPlaceActiveToUIActive();
                                Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.UIActive, "Failed transition");
                                break;
                            default:
                                Debug.Fail("bad state");
                                this.ActiveXState++; // To exit the loop
                                break;
                        }
                    }
                }
                finally {
                        this.SetAXHostState(WebBrowserHelper.inTransition, false);
                }
            }
        }

        internal void TransitionDownTo(WebBrowserHelper.AXState state) {
            if (!this.GetAXHostState(WebBrowserHelper.inTransition)) {
                this.SetAXHostState(WebBrowserHelper.inTransition, true);

                try {
                    while (state < this.ActiveXState) {
                        switch (this.ActiveXState) {
                            case WebBrowserHelper.AXState.UIActive:
                                TransitionFromUIActiveToInPlaceActive();
                                Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.InPlaceActive, "Failed transition");
                                break;
                            case WebBrowserHelper.AXState.InPlaceActive:
                                TransitionFromInPlaceActiveToRunning();
                                Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.Running, "Failed transition");
                                break;
                            case WebBrowserHelper.AXState.Running:
                                TransitionFromRunningToLoaded();
                                Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.Loaded, "Failed transition");
                                break;
                            case WebBrowserHelper.AXState.Loaded:
                                TransitionFromLoadedToPassive();
                                Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.Passive, "Failed transition");
                                break;
                            default:
                                Debug.Fail("bad state");
                                this.ActiveXState--; // To exit the loop
                                break;
                        }
                    }
                }
                finally {
                    this.SetAXHostState(WebBrowserHelper.inTransition, false);
                }
            }
        }

        internal bool DoVerb(int verb) {
            int hr = this.axOleObject.DoVerb(verb, IntPtr.Zero, this.ActiveXSite, 0, this.Handle,
                    new NativeMethods.COMRECT(this.Bounds));

            Debug.Assert(hr == NativeMethods.S_OK, String.Format(CultureInfo.CurrentCulture, "DoVerb call failed for verb 0x{0:X}", verb));
            return hr == NativeMethods.S_OK;
        }

        //
        // Returns this control's logically containing form.
        // At design time this is always the form being designed.
        // At runtime it is the parent form.
        // By default, the parent form performs that function.  It is
        // however possible for another form higher in the parent chain
        // to serve in that role.  The logical container of this
        // control determines the set of logical sibling control.
        // This property exists only to enable some specific
        // behaviors of ActiveX controls.
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        internal ContainerControl ContainingControl {
            get {
                if (containingControl == null ||
                    this.GetAXHostState(WebBrowserHelper.recomputeContainingControl)) {
                    containingControl = FindContainerControlInternal();
                }
                
                return containingControl;
            }
        }

        internal WebBrowserContainer CreateWebBrowserContainer()
        {
            if (wbContainer == null)
            {
                wbContainer = new WebBrowserContainer(this);
            }
            return wbContainer;
        }

        internal WebBrowserContainer GetParentContainer() {
            if (container == null) {
                container = WebBrowserContainer.FindContainerForControl(this);
            }
            if (container == null) {
                container = CreateWebBrowserContainer();
                container.AddControl(this);
            }
            return container;
        }

        internal void SetEditMode(WebBrowserHelper.AXEditMode em) {
            this.axEditMode = em;
        }

        internal void SetSelectionStyle(WebBrowserHelper.SelectionStyle selectionStyle) {
            if (DesignMode) {
                ISelectionService iss = WebBrowserHelper.GetSelectionService(this);
                this.selectionStyle = selectionStyle;
                if (iss != null && iss.GetComponentSelected(this)) {
                    // The ActiveX Host designer will offer an extender property
                    // called "SelectionStyle" 
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(this)["SelectionStyle"];
                    if (prop != null && prop.PropertyType == typeof(int)) {
                        prop.SetValue(this, (int)selectionStyle);
                    }
                }
            }
        }

        internal void AddSelectionHandler() {
            if (!this.GetAXHostState(WebBrowserHelper.addedSelectionHandler)) {
                this.SetAXHostState(WebBrowserHelper.addedSelectionHandler, true);
                
                ISelectionService iss = WebBrowserHelper.GetSelectionService(this);
                if (iss != null) {
                    iss.SelectionChanging += SelectionChangeHandler;
                }
            }
        }

        internal bool RemoveSelectionHandler() {
            bool retVal = this.GetAXHostState(WebBrowserHelper.addedSelectionHandler);
            if (retVal) {
                this.SetAXHostState(WebBrowserHelper.addedSelectionHandler, false);

                ISelectionService iss = WebBrowserHelper.GetSelectionService(this);
                if (iss != null) {
                    iss.SelectionChanging -= SelectionChangeHandler;
                }
            }
            return retVal;
        }

        internal void AttachWindow(IntPtr hwnd) {
            UnsafeNativeMethods.SetParent(new HandleRef(null, hwnd), new HandleRef(this, this.Handle));

            if (this.axWindow != null) {
                this.axWindow.ReleaseHandle();
            }
            this.axWindow = new WebBrowserBaseNativeWindow(this);
            this.axWindow.AssignHandle(hwnd, false);
            
            UpdateZOrder();
            UpdateBounds();

            Size extent = Size;
            extent = SetExtent(extent.Width, extent.Height);

            Point location = Location;
            Bounds = new Rectangle(location.X, location.Y, extent.Width, extent.Height);
        }

        internal bool IsUserMode {
            get {
                return Site == null || !DesignMode;
            }
        }

        internal void MakeDirty() {
            ISite iSite = this.Site;
            if (iSite != null) {
                IComponentChangeService ccs = (IComponentChangeService)iSite.GetService(typeof(IComponentChangeService));
                if (ccs != null) {
                    ccs.OnComponentChanging(this, null);
                    ccs.OnComponentChanged(this, null, null, null);
                }
            }
        }

        internal int NoComponentChangeEvents {
            get {
                return noComponentChange;
            }

            set {
                noComponentChange = value;
            }
        }



        



        //
        // Private helper methods:
        //

        private void StartEvents() {
            if (!this.GetAXHostState(WebBrowserHelper.sinkAttached)) {
                this.SetAXHostState(WebBrowserHelper.sinkAttached, true);
                CreateSink();
            }
            this.ActiveXSite.StartEvents();
        }

        private void StopEvents() {
            if (this.GetAXHostState(WebBrowserHelper.sinkAttached)) {
                this.SetAXHostState(WebBrowserHelper.sinkAttached, false);
                DetachSink();
            }
            this.ActiveXSite.StopEvents();
        }

        private void TransitionFromPassiveToLoaded() {
            Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.Passive, "Wrong start state to transition from");
            if (this.ActiveXState == WebBrowserHelper.AXState.Passive) {
                //
                // First, create the ActiveX control
                Debug.Assert(activeXInstance == null, "activeXInstance must be null");
                this.activeXInstance = UnsafeNativeMethods.CoCreateInstance(ref clsid, null, NativeMethods.CLSCTX_INPROC_SERVER, ref NativeMethods.ActiveX.IID_IUnknown);
                Debug.Assert(activeXInstance != null, "w/o an exception being thrown we must have an object...");

                //
                // We are now Loaded!
                this.ActiveXState = WebBrowserHelper.AXState.Loaded;

                //
                // Lets give them a chance to cast the ActiveX object
                // to the appropriate interfaces.
                this.AttachInterfacesInternal();
            }
        }

        private void TransitionFromLoadedToPassive() {
            Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.Loaded, "Wrong start state to transition from");
            if (this.ActiveXState == WebBrowserHelper.AXState.Loaded) {
                //
                // Need to make sure that we don't handle any PropertyChanged
                // notifications at this point.
                this.NoComponentChangeEvents++;
                try {
                    //
                    // Release the activeXInstance
                    if (activeXInstance != null) {
                        //
                        // Lets first get the cached interface pointers of activeXInstance released.
                        this.DetachInterfacesInternal();

                        Marshal.FinalReleaseComObject(activeXInstance);
                        this.activeXInstance = null;
                    }
                }
                finally {
                    this.NoComponentChangeEvents--;
                }

                //
                // We are now Passive!
                this.ActiveXState = WebBrowserHelper.AXState.Passive;
            }
        }

        private void TransitionFromLoadedToRunning() {
            Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.Loaded, "Wrong start state to transition from");
            if (this.ActiveXState == WebBrowserHelper.AXState.Loaded) {
                //
                // See if the ActiveX control returns OLEMISC_SETCLIENTSITEFIRST
                int bits = 0;
                int hr = this.axOleObject.GetMiscStatus(NativeMethods.ActiveX.DVASPECT_CONTENT, out bits);
                if (NativeMethods.Succeeded(hr) && ((bits & NativeMethods.ActiveX.OLEMISC_SETCLIENTSITEFIRST) != 0)) {
                    //
                    // Simply setting the site to the ActiveX control should activate it.
                    // And this will take us to the Running state.
                    this.axOleObject.SetClientSite(this.ActiveXSite);
                }

                //
                // We start receiving events now (but we do this only if we are not
                // in DesignMode).
                if (!DesignMode) {
                    StartEvents();
                }

                //
                // We are now Running!
                this.ActiveXState = WebBrowserHelper.AXState.Running;
            }
        }

        private void TransitionFromRunningToLoaded() {
            Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.Running, "Wrong start state to transition from");
            if (this.ActiveXState == WebBrowserHelper.AXState.Running) {
                StopEvents();

                //
                // Remove ourselves from our parent container...
                WebBrowserContainer parentContainer = this.GetParentContainer();
                if (parentContainer != null) {
                    parentContainer.RemoveControl(this);
                }

                //
                // Now inform the ActiveX control that it's been un-sited.
                this.axOleObject.SetClientSite(null);

                //
                // We are now Loaded!
                this.ActiveXState = WebBrowserHelper.AXState.Loaded;
            }
        }

        private void TransitionFromRunningToInPlaceActive() {
            Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.Running, "Wrong start state to transition from");
            if (this.ActiveXState == WebBrowserHelper.AXState.Running) {
                try
                {
                    DoVerb(NativeMethods.OLEIVERB_INPLACEACTIVATE);
                }
                catch (Exception t)
                {
                    throw new TargetInvocationException(string.Format(SR.AXNohWnd, GetType().Name), t);
                }

                CreateControl(true);

                //
                // We are now InPlaceActive!
                this.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
            }
        }

        private void TransitionFromInPlaceActiveToRunning() {
            Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.InPlaceActive, "Wrong start state to transition from");
            if (this.ActiveXState == WebBrowserHelper.AXState.InPlaceActive) {
                //
                // First, lets make sure we transfer the ContainingControl's ActiveControl
                // before we InPlaceDeactivate.
                ContainerControl f = ContainingControl;
                if (f != null && f.ActiveControl == this) {
                    f.SetActiveControlInternal(null);
                }

                //
                // Now, InPlaceDeactivate.
                this.AXInPlaceObject.InPlaceDeactivate();

                //
                // We are now Running!
                this.ActiveXState = WebBrowserHelper.AXState.Running;
            }
        }

        private void TransitionFromInPlaceActiveToUIActive() {
            Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.InPlaceActive, "Wrong start state to transition from");
            if (this.ActiveXState == WebBrowserHelper.AXState.InPlaceActive) {
                try {
                    DoVerb(NativeMethods.OLEIVERB_UIACTIVATE);
                }
                catch (Exception t) {
                    throw new TargetInvocationException(string.Format(SR.AXNohWnd, GetType().Name), t);
                }

                //
                // We are now UIActive
                this.ActiveXState = WebBrowserHelper.AXState.UIActive;
            }
        }

        private void TransitionFromUIActiveToInPlaceActive() {
            Debug.Assert(this.ActiveXState == WebBrowserHelper.AXState.UIActive, "Wrong start state to transition from");
            if (this.ActiveXState == WebBrowserHelper.AXState.UIActive) {
                int hr = this.AXInPlaceObject.UIDeactivate();
                Debug.Assert(NativeMethods.Succeeded(hr), "Failed to UIDeactivate");

                // We are now InPlaceActive
                this.ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
            }
        }

        internal WebBrowserSiteBase ActiveXSite {
            get {
                if (axSite == null) {
                    this.axSite = CreateWebBrowserSiteBase();
                }
                return axSite;
            }
        }

        private void AttachInterfacesInternal() {
            Debug.Assert(activeXInstance != null, "The native control is null");
            this.axOleObject = (UnsafeNativeMethods.IOleObject)activeXInstance;
            this.axOleInPlaceObject = (UnsafeNativeMethods.IOleInPlaceObject)activeXInstance;
            this.axOleInPlaceActiveObject = (UnsafeNativeMethods.IOleInPlaceActiveObject)activeXInstance;
            this.axOleControl = (UnsafeNativeMethods.IOleControl)activeXInstance;
            //
            // Lets give the inheriting classes a chance to cast
            // the ActiveX object to the appropriate interfaces.
            AttachInterfaces(activeXInstance);
        }
        
        private void DetachInterfacesInternal() {
            this.axOleObject = null;
            this.axOleInPlaceObject = null;
            this.axOleInPlaceActiveObject = null;
            this.axOleControl = null;
            //
            // Lets give the inheriting classes a chance to release
            // their cached interfaces of the ActiveX object.
            DetachInterfaces();
        }
        
        //
        // We need to change the ActiveX control's state when selection changes.
        private EventHandler SelectionChangeHandler {
            get {
                if (this.selectionChangeHandler == null) {
                    this.selectionChangeHandler = new EventHandler(this.OnNewSelection);
                }
                return this.selectionChangeHandler;
            }
        }

        //
        // We need to do special stuff (convert window messages to interface calls)
        // during design time when selection changes.
        private void OnNewSelection(Object sender, EventArgs e) {
            if (this.DesignMode) {
                ISelectionService iss = WebBrowserHelper.GetSelectionService(this);
                if (iss != null) {
                    // We are no longer selected.
                    if (!iss.GetComponentSelected(this)) {
                        //
                        // We need to exit editmode if we were in one.
                        if (this.EditMode) {
                            this.GetParentContainer().OnExitEditMode(this);
                            this.SetEditMode(WebBrowserHelper.AXEditMode.None);
                        }
                        this.SetSelectionStyle(WebBrowserHelper.SelectionStyle.Selected);
                        this.RemoveSelectionHandler();
                    }
                    else {
                        //
                        // The AX Host designer will offer an extender property called "SelectionStyle"
                        PropertyDescriptor prop = TypeDescriptor.GetProperties(this)["SelectionStyle"];
                        if (prop != null && prop.PropertyType == typeof(int)) {
                            int curSelectionStyle = (int)prop.GetValue(this);
                            if (curSelectionStyle != (int)this.selectionStyle) {
                                prop.SetValue(this, selectionStyle);
                            }
                        }
                    }
                }
            }
        }

        private Size SetExtent(int width, int height) {
            NativeMethods.tagSIZEL sz = new NativeMethods.tagSIZEL();
            sz.cx = width;
            sz.cy = height;
            bool resetExtents = DesignMode;
            try {
                Pixel2hiMetric(sz, sz);
                this.axOleObject.SetExtent(NativeMethods.ActiveX.DVASPECT_CONTENT, sz);
            }
            catch (COMException) {
                resetExtents = true;
            }
            if (resetExtents) {
                this.axOleObject.GetExtent(NativeMethods.ActiveX.DVASPECT_CONTENT, sz);
                try {
                    this.axOleObject.SetExtent(NativeMethods.ActiveX.DVASPECT_CONTENT, sz);
                }
                catch (COMException e) {
                    Debug.Fail(e.ToString());
                }
            }
            return GetExtent();        
        }

        private Size GetExtent() {
            NativeMethods.tagSIZEL sz = new NativeMethods.tagSIZEL();
            this.axOleObject.GetExtent(NativeMethods.ActiveX.DVASPECT_CONTENT, sz);
            HiMetric2Pixel(sz, sz);
            return new Size(sz.cx, sz.cy);
        }

        private void HiMetric2Pixel(NativeMethods.tagSIZEL sz, NativeMethods.tagSIZEL szout) {
            NativeMethods._POINTL phm = new NativeMethods._POINTL();
            phm.x = sz.cx;
            phm.y = sz.cy;
            NativeMethods.tagPOINTF pcont = new NativeMethods.tagPOINTF();
            ((UnsafeNativeMethods.IOleControlSite)this.ActiveXSite).TransformCoords(phm, pcont, NativeMethods.ActiveX.XFORMCOORDS_SIZE | NativeMethods.ActiveX.XFORMCOORDS_HIMETRICTOCONTAINER);
            szout.cx = (int)pcont.x;
            szout.cy = (int)pcont.y;
        }

        private void Pixel2hiMetric(NativeMethods.tagSIZEL sz, NativeMethods.tagSIZEL szout) {
            NativeMethods.tagPOINTF pcont = new NativeMethods.tagPOINTF();
            pcont.x = (float) sz.cx;
            pcont.y = (float) sz.cy;
            NativeMethods._POINTL phm = new NativeMethods._POINTL();
            ((UnsafeNativeMethods.IOleControlSite)this.ActiveXSite).TransformCoords(phm, pcont, NativeMethods.ActiveX.XFORMCOORDS_SIZE | NativeMethods.ActiveX.XFORMCOORDS_CONTAINERTOHIMETRIC);
            szout.cx = phm.x;
            szout.cy = phm.y;
        }

        private bool EditMode {
            get {
                return this.axEditMode != WebBrowserHelper.AXEditMode.None;
            }
        }

        //Find the uppermost ContainerControl that this control lives in
        internal ContainerControl FindContainerControlInternal() {
            if (Site != null) {
                IDesignerHost host = (IDesignerHost)Site.GetService(typeof(IDesignerHost));
                if (host != null) {
                    IComponent comp = host.RootComponent;
                    if (comp != null && comp is ContainerControl) {
                        return (ContainerControl)comp;
                    }
                }
            }

            ContainerControl cc = null;
            for (Control control = this; control != null; control = control.ParentInternal) {
                ContainerControl tempCC = control as ContainerControl;
                if (tempCC != null)
                    cc = tempCC;
            }

            if (cc == null) {
                cc = Control.FromHandle(UnsafeNativeMethods.GetParent(new HandleRef(this, Handle))) as ContainerControl;
            }

            // Never use the parking window for this: its hwnd can be destroyed at any time.
            if (cc is Application.ParkingWindow)
            {
                cc = null;
            }

            this.SetAXHostState(WebBrowserHelper.recomputeContainingControl, cc == null);

            return cc;
        }

        private void AmbientChanged(int dispid) {
            if (activeXInstance != null) {
                try {
                    Invalidate();
                    this.axOleControl.OnAmbientPropertyChange(dispid);
                }
                catch (Exception ex) {
                    if (ClientUtils.IsCriticalException(ex)) {
                        throw;
                    }
                    Debug.Fail(ex.ToString());
                }
            }
        }

#if false
        // FxCop: Currently not used
        private Object GetAmbientProperty(int dispid) {

            Control richParent = ParentInternal;

            switch (dispid) {
                case NativeMethods.ActiveX.DISPID_AMBIENT_USERMODE:
                    return !DesignMode;
                case NativeMethods.ActiveX.DISPID_AMBIENT_AUTOCLIP:
                    return true;
                case NativeMethods.ActiveX.DISPID_AMBIENT_MESSAGEREFLECT:
                    return true;
                case NativeMethods.ActiveX.DISPID_AMBIENT_UIDEAD:
                    return false;
                case NativeMethods.ActiveX.DISPID_AMBIENT_DISPLAYASDEFAULT:
                    return false;
                case NativeMethods.ActiveX.DISPID_AMBIENT_FONT:
                    if (richParent != null) {
                        return WebBrowserHelper.GetIFontFromFont(richParent.Font);
                    }
                    return null;
                case NativeMethods.ActiveX.DISPID_AMBIENT_SHOWGRABHANDLES:
                    return false;
                case NativeMethods.ActiveX.DISPID_AMBIENT_SHOWHATCHING:
                    return false;
                case NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR:
                    if (richParent != null) {
                        return WebBrowserHelper.GetOleColorFromColor(richParent.BackColor);
                    }
                    return null;
                case NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR:
                    if (richParent != null) {
                        return WebBrowserHelper.GetOleColorFromColor(richParent.ForeColor);
                    }
                    return null;
                case NativeMethods.ActiveX.DISPID_AMBIENT_DISPLAYNAME:
                    string rval = this.GetParentContainer().GetNameForControl(this);
                    if (rval == null) rval = "";
                    return rval;
                case NativeMethods.ActiveX.DISPID_AMBIENT_LOCALEID:
                    return Thread.CurrentThread.CurrentCulture.LCID;
                case NativeMethods.ActiveX.DISPID_AMBIENT_RIGHTTOLEFT:
                    Control ctl = this;
                    while (ctl != null) {
                        if (ctl.RightToLeft == System.Windows.Forms.RightToLeft.No)
                            return false;
                        if (ctl.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                            return true;
                        if (ctl.RightToLeft == System.Windows.Forms.RightToLeft.Inherit)
                            ctl = ctl.Parent;
                    }
                    return null;
                default:
                    return null;
            }
        }
#endif

        internal UnsafeNativeMethods.IOleInPlaceObject AXInPlaceObject {
            get {
                return this.axOleInPlaceObject;
            }
        }

        // ---------------------------------------------------------------
        // The following properties implemented in the Control class don't make
        // sense for ActiveX controls. So we block them here.
        // ---------------------------------------------------------------

        //
        // Overridden properties:
        //

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.DefaultSize"]/*' />
        protected override Size DefaultSize {
            get {
                return new Size(75, 23);
            }
        }


        //
        // Overridden methods:
        //

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.IsInputChar"]/*' />
        protected override bool IsInputChar(char charCode) {
            return true;
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.OnHandleCreated"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to find out when the
        ///     handle has been created. Call base.OnHandleCreated first.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnHandleCreated(EventArgs e) {
            //
            // This is needed to prevent some controls (for e.g. Office Web Components) from 
            // failing to InPlaceActivate() when they call RegisterDragDrop() but do not call 
            // OleInitialize(). The EE calls CoInitializeEx() on the thread, but I believe
            // that is not good enough for DragDrop.
            //
            if (Application.OleRequired() != System.Threading.ApartmentState.STA) {
                throw new ThreadStateException(SR.ThreadMustBeSTA);
            }

            base.OnHandleCreated(e);

            // make sure we restore whatever running state whad prior to the handle recreate.
            //
            if (axReloadingState != WebBrowserHelper.AXState.Passive && axReloadingState != axState) {
                if (axState < axReloadingState) {
                    TransitionUpTo(axReloadingState);
                }
                else {
                    TransitionDownTo(axReloadingState);
                }
                axReloadingState = WebBrowserHelper.AXState.Passive;
            }

        }

        
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.BackColor"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor {
            get {
                return base.BackColor;
            }
            set {
                base.BackColor = value;
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Font"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font {
            get {
                return base.Font;
            }
            set {
                base.Font = value;
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.ForeColor"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor {
            get {
                return base.ForeColor;
            }
            set {
                base.ForeColor = value;
            }
        }

        /// <devdoc>
        ///     <para>Hide ImeMode: it doesn't make sense for this control</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public ImeMode ImeMode
        {
            get
            {
                return base.ImeMode;
            }
            set
            {
                base.ImeMode = value;
            }
        }

        //
        // Properties blocked at design time and run time:
        //
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.AllowDrop"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AllowDrop {
            get {
                return base.AllowDrop;
            }
            set {
                throw new NotSupportedException(SR.WebBrowserAllowDropNotSupported);
            }
        }

       /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.BackgroundImage"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                throw new NotSupportedException(SR.WebBrowserBackgroundImageNotSupported);
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.BackgroundImageLayout"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                throw new NotSupportedException(SR.WebBrowserBackgroundImageLayoutNotSupported);
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Cursor"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Cursor Cursor {
            get {
                return base.Cursor;
            }
            set {
                throw new NotSupportedException(SR.WebBrowserCursorNotSupported);
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Enabled"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                throw new NotSupportedException(SR.WebBrowserEnabledNotSupported);
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.RightToLeft"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Localizable(false)]
        public override RightToLeft RightToLeft
        {
            get
            {
                return RightToLeft.No;
            }
            set
            {
                throw new NotSupportedException(SR.WebBrowserRightToLeftNotSupported);
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Text"]/*' />
        // Override this property so that the Bindable attribute can be set to false.
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            Bindable(false)
        ]
        public override string Text {
            get {
                return "";
            }
            set {
                throw new NotSupportedException(SR.WebBrowserTextNotSupported);
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.UseWaitCursor"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool UseWaitCursor {
            get {
                return base.UseWaitCursor;
            }
            set {
                throw new NotSupportedException(SR.WebBrowserUseWaitCursorNotSupported);
            }
        }


        //
        // Unavailable events
        //
        
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.BackgroundImageLayoutChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackgroundImageLayoutChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Enter"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler Enter {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Enter"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Leave"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler Leave {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Leave"));
            }
            remove {
            }
        }
        
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.MouseCaptureChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseCaptureChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseCaptureChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.MouseClick"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseClick {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseClick"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.MouseDoubleClick"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseDoubleClick {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseDoubleClick"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.BackColorChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackColorChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackColorChanged"));
            }
            remove {
            }
        }
        
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.BackgroundImageChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackgroundImageChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.BindingContextChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BindingContextChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BindingContextChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.CursorChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler CursorChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "CursorChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.EnabledChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler EnabledChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "EnabledChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.FontChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler FontChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "FontChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.ForeColorChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ForeColorChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.RightToLeftChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler RightToLeftChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "RightToLeftChanged"));
            }
            remove {
            }
        }
        
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.TextChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "TextChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Click"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler Click {
            add { 
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Click"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.DragDrop"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event DragEventHandler DragDrop {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragDrop"));
            }
            remove {
            }
        }


        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.DragEnter"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event DragEventHandler DragEnter {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragEnter"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.DragOver"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event DragEventHandler DragOver {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragOver"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.DragLeave"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DragLeave {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragLeave"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.GiveFeedback"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event GiveFeedbackEventHandler GiveFeedback {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "GiveFeedback"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.HelpRequested"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")] //Everett
        new public event HelpEventHandler HelpRequested
        {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "HelpRequested"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Paint"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event PaintEventHandler Paint {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Paint"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.QueryContinueDrag"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event QueryContinueDragEventHandler QueryContinueDrag {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "QueryContinueDrag"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.QueryAccessibilityHelp"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "QueryAccessibilityHelp"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.DoubleClick"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DoubleClick {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DoubleClick"));
            }
            remove {
            }
        }
        
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.ImeModeChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ImeModeChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ImeModeChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.KeyDown"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyEventHandler KeyDown {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyDown"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.KeyPress"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyPressEventHandler KeyPress {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyPress"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.KeyUp"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyEventHandler KeyUp {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyUp"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.Layout"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event LayoutEventHandler Layout {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Layout"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.MouseDown"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseDown {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseDown"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.MouseEnter"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseEnter {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseEnter"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.MouseLeave"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseLeave {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseLeave"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.MouseHover"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseHover {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseHover"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.MouseMove"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseMove {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseMove"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.MouseUp"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseUp {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseUp"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.MouseWheel"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseWheel {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseWheel"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.ChangeUICues"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event UICuesEventHandler ChangeUICues {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ChangeUICues"));
            }
            remove {
            }
        }
        
        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.StyleChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler StyleChanged {
            add {
                throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "StyleChanged"));
            }
            remove {
            }
        }

        /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.WebBrowserBaseNativeWindow"]/*' />
        /// <devdoc>
        ///     Defines a window that the ActiveX window is attached to so that we can override it's wndproc.
        /// </devdoc>
        /// <internalonly/>
        private class WebBrowserBaseNativeWindow : NativeWindow {
            private WebBrowserBase WebBrowserBase;

            public WebBrowserBaseNativeWindow(WebBrowserBase ax) {
                this.WebBrowserBase = ax;
            }
            
            /// <include file='doc\WebBrowserBase.uex' path='docs/doc[@for="WebBrowserBase.WebBrowserBaseNativeWindow.WndProc"]/*' />
            /// <devdoc>
            ///     Pass messages on to the NotifyIcon object's wndproc handler.
            /// </devdoc>
            /// <internalonly/>
            protected override void WndProc(ref Message m) {
                switch (m.Msg) {
                    case NativeMethods.WM_WINDOWPOSCHANGING:
                        WmWindowPosChanging(ref m);
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }

            private unsafe void WmWindowPosChanging(ref Message m) {
                NativeMethods.WINDOWPOS* wp = (NativeMethods.WINDOWPOS *)m.LParam;
                wp->x = 0;
                wp->y = 0;
                Size s = WebBrowserBase.webBrowserBaseChangingSize;
                if (s.Width == -1) {   // Invalid value. Use WebBrowserBase.Bounds instead, when this is the case.
                    wp->cx = WebBrowserBase.Width;
                    wp->cy = WebBrowserBase.Height;
                }
                else {
                    wp->cx = s.Width;
                    wp->cy = s.Height;
                }
                m.Result = (IntPtr)0;
            }
        }
    }
}

