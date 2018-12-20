// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;


namespace System.Windows.Forms
{
    /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument"]/*' />
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    public sealed class HtmlDocument
    {
        internal static object EventClick = new object();
        internal static object EventContextMenuShowing = new object();
        internal static object EventFocusing = new object();
        internal static object EventLosingFocus = new object();
        internal static object EventMouseDown = new object();
        internal static object EventMouseLeave = new object();
        internal static object EventMouseMove = new object();
        internal static object EventMouseOver = new object();
        internal static object EventMouseUp = new object();
        internal static object EventStop = new object();

        private UnsafeNativeMethods.IHTMLDocument2 htmlDocument2;
        private HtmlShimManager shimManager;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        internal HtmlDocument(HtmlShimManager shimManager, UnsafeNativeMethods.IHTMLDocument doc)
        {
            this.htmlDocument2 = (UnsafeNativeMethods.IHTMLDocument2)doc;
            Debug.Assert(this.NativeHtmlDocument2 != null, "The document should implement IHtmlDocument2");

            this.shimManager = shimManager;

        }

        internal UnsafeNativeMethods.IHTMLDocument2 NativeHtmlDocument2
        {
            get
            {
                return this.htmlDocument2;
            }
        }

        private HtmlDocumentShim DocumentShim
        {
            get
            {
                if (ShimManager != null)
                {
                    HtmlDocumentShim shim = ShimManager.GetDocumentShim(this);
                    if (shim == null)
                    {
                        shimManager.AddDocumentShim(this);
                        shim = ShimManager.GetDocumentShim(this);
                    }
                    return shim;
                }
                return null;
            }
        }

        private HtmlShimManager ShimManager
        {
            get
            {
                return this.shimManager;
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.ActiveElement"]/*' />
        public HtmlElement ActiveElement
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement iHtmlElement = this.NativeHtmlDocument2.GetActiveElement();
                return iHtmlElement != null ? new HtmlElement(ShimManager, iHtmlElement) : null;
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Body"]/*' />
        public HtmlElement Body
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement iHtmlElement = this.NativeHtmlDocument2.GetBody();
                return iHtmlElement != null ? new HtmlElement(ShimManager, iHtmlElement) : null;
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Domain"]/*' />
        public string Domain
        {
            get
            {
                return this.NativeHtmlDocument2.GetDomain();
            }
            set
            {
                try
                {
                    this.NativeHtmlDocument2.SetDomain(value);
                }
                catch (ArgumentException)
                {
                    // Give a better message describing the error
                    throw new ArgumentException(SR.HtmlDocumentInvalidDomain);
                }
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Title"]/*' />
        public string Title
        {
            get
            {
                return this.NativeHtmlDocument2.GetTitle();
            }
            set
            {
                this.NativeHtmlDocument2.SetTitle(value);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Location"]/*' />
        public Uri Url
        {
            get
            {
                UnsafeNativeMethods.IHTMLLocation iHtmlLocation = this.NativeHtmlDocument2.GetLocation();
                string stringLocation = (iHtmlLocation == null) ? "" : iHtmlLocation.GetHref();
                return string.IsNullOrEmpty(stringLocation) ? null : new Uri(stringLocation);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Window"]/*' />
        public HtmlWindow Window
        {
            get
            {
                UnsafeNativeMethods.IHTMLWindow2 iHTMLWindow2 = this.NativeHtmlDocument2.GetParentWindow();
                return iHTMLWindow2 != null ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.BackColor"]/*' />
        public Color BackColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = this.ColorFromObject(this.NativeHtmlDocument2.GetBgColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsSecurityOrCriticalException(ex))
                    {
                        throw;
                    }
                }
                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                this.NativeHtmlDocument2.SetBgColor(color);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.ForeColor"]/*' />
        public Color ForeColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = this.ColorFromObject(this.NativeHtmlDocument2.GetFgColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsSecurityOrCriticalException(ex))
                    {
                        throw;
                    }
                }
                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                this.NativeHtmlDocument2.SetFgColor(color);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.LinkColor"]/*' />
        public Color LinkColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = this.ColorFromObject(this.NativeHtmlDocument2.GetLinkColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsSecurityOrCriticalException(ex))
                    {
                        throw;
                    }
                }
                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                this.NativeHtmlDocument2.SetLinkColor(color);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.ActiveLinkColor"]/*' />
        public Color ActiveLinkColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = this.ColorFromObject(this.NativeHtmlDocument2.GetAlinkColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsSecurityOrCriticalException(ex))
                    {
                        throw;
                    }
                }
                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                this.NativeHtmlDocument2.SetAlinkColor(color);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.VisitedLinkColor"]/*' />
        public Color VisitedLinkColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = this.ColorFromObject(this.NativeHtmlDocument2.GetVlinkColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsSecurityOrCriticalException(ex))
                    {
                        throw;
                    }
                }
                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                this.NativeHtmlDocument2.SetVlinkColor(color);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Focused"]/*' />
        public bool Focused
        {
            get
            {
                return ((UnsafeNativeMethods.IHTMLDocument4)this.NativeHtmlDocument2).HasFocus();
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.DomDocument"]/*' />
        public object DomDocument
        {
            get
            {
                return this.NativeHtmlDocument2;
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Cookie"]/*' />
        public string Cookie
        {
            get
            {
                return this.NativeHtmlDocument2.GetCookie();
            }
            set
            {
                this.NativeHtmlDocument2.SetCookie(value);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.RightToLeft"]/*' />
        public bool RightToLeft
        {
            get
            {
                return ((UnsafeNativeMethods.IHTMLDocument3)this.NativeHtmlDocument2).GetDir() == "rtl";
            }
            set
            {
                ((UnsafeNativeMethods.IHTMLDocument3)this.NativeHtmlDocument2).SetDir(value ? "rtl" : "ltr");
            }
        }

        public string Encoding
        {
            get
            {
                return this.NativeHtmlDocument2.GetCharset();
            }
            set
            {
                this.NativeHtmlDocument2.SetCharset(value);
            }
        }

        public string DefaultEncoding
        {
            get
            {
                return this.NativeHtmlDocument2.GetDefaultCharset();
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.All"]/*' />
        public HtmlElementCollection All
        {
            get
            {
                UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = this.NativeHtmlDocument2.GetAll();
                return iHTMLElementCollection != null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Links"]/*' />
        public HtmlElementCollection Links
        {
            get
            {
                UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = this.NativeHtmlDocument2.GetLinks();
                return iHTMLElementCollection != null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Images"]/*' />
        public HtmlElementCollection Images
        {
            get
            {
                UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = this.NativeHtmlDocument2.GetImages();
                return iHTMLElementCollection != null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Forms"]/*' />
        public HtmlElementCollection Forms
        {
            get
            {
                UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = this.NativeHtmlDocument2.GetForms();
                return iHTMLElementCollection != null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Write"]/*' />
        public void Write(string text)
        {
            object[] strs = new object[] { (object)text };
            this.NativeHtmlDocument2.Write(strs);
        }

        /// <devdoc>
        ///    <para>Executes a command on the document</para>
        /// </devdoc>
        public void ExecCommand(string command, bool showUI, object value)
        {
            this.NativeHtmlDocument2.ExecCommand(command, showUI, value);
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Focus"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Focus()
        {
            ((UnsafeNativeMethods.IHTMLDocument4)this.NativeHtmlDocument2).Focus();
            // Seems to have a problem in really setting focus the first time
            ((UnsafeNativeMethods.IHTMLDocument4)this.NativeHtmlDocument2).Focus();
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.GetElementById"]/*' />
        public HtmlElement GetElementById(string id)
        {
            UnsafeNativeMethods.IHTMLElement iHTMLElement = ((UnsafeNativeMethods.IHTMLDocument3)this.NativeHtmlDocument2).GetElementById(id);
            return iHTMLElement != null ? new HtmlElement(ShimManager, iHTMLElement) : null;
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.GetElementFromPoint"]/*' />
        public HtmlElement GetElementFromPoint(Point point)
        {
            UnsafeNativeMethods.IHTMLElement iHTMLElement = this.NativeHtmlDocument2.ElementFromPoint(point.X, point.Y);
            return iHTMLElement != null ? new HtmlElement(ShimManager, iHTMLElement) : null;
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.GetElementsByTagName"]/*' />
        public HtmlElementCollection GetElementsByTagName(string tagName)
        {
            UnsafeNativeMethods.IHTMLElementCollection iHTMLElementCollection = ((UnsafeNativeMethods.IHTMLDocument3)this.NativeHtmlDocument2).GetElementsByTagName(tagName);
            return iHTMLElementCollection != null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.OpenNew"]/*' />
        public HtmlDocument OpenNew(bool replaceInHistory)
        {
            object name = (object)(replaceInHistory ? "replace" : "");
            object nullObject = null;
            object ohtmlDocument = this.NativeHtmlDocument2.Open("text/html", name, nullObject, nullObject);
            UnsafeNativeMethods.IHTMLDocument iHTMLDocument = ohtmlDocument as UnsafeNativeMethods.IHTMLDocument;
            return iHTMLDocument != null ? new HtmlDocument(ShimManager, iHTMLDocument) : null;
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.CreateElement"]/*' />
        public HtmlElement CreateElement(string elementTag)
        {
            UnsafeNativeMethods.IHTMLElement iHTMLElement = this.NativeHtmlDocument2.CreateElement(elementTag);
            return iHTMLElement != null ? new HtmlElement(ShimManager, iHTMLElement) : null;
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.InvokeScript"]/*' />
        public object InvokeScript(string scriptName, object[] args)
        {
            object retVal = null;
            NativeMethods.tagDISPPARAMS dp = new NativeMethods.tagDISPPARAMS();
            dp.rgvarg = IntPtr.Zero;
            try
            {
                UnsafeNativeMethods.IDispatch scriptObject = this.NativeHtmlDocument2.GetScript() as UnsafeNativeMethods.IDispatch;
                if (scriptObject != null)
                {
                    Guid g = Guid.Empty;
                    string[] names = new string[] { scriptName };
                    int[] dispids = new int[] { NativeMethods.ActiveX.DISPID_UNKNOWN };
                    int hr = scriptObject.GetIDsOfNames(ref g, names, 1,
                                                   SafeNativeMethods.GetThreadLCID(), dispids);
                    if (NativeMethods.Succeeded(hr) && (dispids[0] != NativeMethods.ActiveX.DISPID_UNKNOWN))
                    {
                        if (args != null)
                        {
                            // Reverse the arg order so that parms read naturally after IDispatch. (
                            Array.Reverse(args);
                        }
                        dp.rgvarg = (args == null) ? IntPtr.Zero : HtmlDocument.ArrayToVARIANTVector(args);
                        dp.cArgs = (args == null) ? 0 : args.Length;
                        dp.rgdispidNamedArgs = IntPtr.Zero;
                        dp.cNamedArgs = 0;

                        object[] retVals = new object[1];

                        hr = scriptObject.Invoke(dispids[0], ref g, SafeNativeMethods.GetThreadLCID(),
                                NativeMethods.DISPATCH_METHOD, dp,
                                retVals, new NativeMethods.tagEXCEPINFO(), null);
                        if (hr == NativeMethods.S_OK)
                        {
                            retVal = retVals[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }
            finally
            {
                if (dp.rgvarg != IntPtr.Zero)
                {
                    HtmlDocument.FreeVARIANTVector(dp.rgvarg, args.Length);
                }
            }
            return retVal;
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.InvokeScript1"]/*' />
        public object InvokeScript(string scriptName)
        {
            return InvokeScript(scriptName, null);
        }


        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.AttachEventHandler"]/*' />
        public void AttachEventHandler(string eventName, EventHandler eventHandler)
        {
            HtmlDocumentShim shim = DocumentShim;
            if (shim != null)
            {
                shim.AttachEventHandler(eventName, eventHandler);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.DetachEventHandler"]/*' />
        public void DetachEventHandler(string eventName, EventHandler eventHandler)
        {
            HtmlDocumentShim shim = DocumentShim;
            if (shim != null)
            {
                shim.DetachEventHandler(eventName, eventHandler);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Click"]/*' />
        public event HtmlElementEventHandler Click
        {
            add
            {
                DocumentShim.AddHandler(EventClick, value);
            }
            remove
            {
                DocumentShim.RemoveHandler(EventClick, value);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.ContextMenuShowing"]/*' />
        public event HtmlElementEventHandler ContextMenuShowing
        {
            add
            {
                DocumentShim.AddHandler(EventContextMenuShowing, value);
            }
            remove
            {
                DocumentShim.RemoveHandler(EventContextMenuShowing, value);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Focusing"]/*' />
        public event HtmlElementEventHandler Focusing
        {
            add
            {
                DocumentShim.AddHandler(EventFocusing, value);
            }
            remove
            {
                DocumentShim.RemoveHandler(EventFocusing, value);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.LosingFocus"]/*' />
        public event HtmlElementEventHandler LosingFocus
        {
            add
            {
                DocumentShim.AddHandler(EventLosingFocus, value);
            }
            remove
            {
                DocumentShim.RemoveHandler(EventLosingFocus, value);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.MouseDown"]/*' />
        public event HtmlElementEventHandler MouseDown
        {
            add
            {
                DocumentShim.AddHandler(EventMouseDown, value);
            }
            remove
            {
                DocumentShim.RemoveHandler(EventMouseDown, value);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.MouseLeave"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the mouse leaves the document</para>
        /// </devdoc>
        public event HtmlElementEventHandler MouseLeave
        {
            add
            {
                DocumentShim.AddHandler(EventMouseLeave, value);
            }
            remove
            {
                DocumentShim.RemoveHandler(EventMouseLeave, value);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.MouseMove"]/*' />
        public event HtmlElementEventHandler MouseMove
        {
            add
            {
                DocumentShim.AddHandler(EventMouseMove, value);
            }
            remove
            {
                DocumentShim.RemoveHandler(EventMouseMove, value);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.MouseOver"]/*' />
        public event HtmlElementEventHandler MouseOver
        {
            add
            {
                DocumentShim.AddHandler(EventMouseOver, value);
            }
            remove
            {
                DocumentShim.RemoveHandler(EventMouseOver, value);
            }

        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.MouseUp"]/*' />
        public event HtmlElementEventHandler MouseUp
        {
            add
            {
                DocumentShim.AddHandler(EventMouseUp, value);
            }
            remove
            {
                DocumentShim.RemoveHandler(EventMouseUp, value);
            }
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.Stop"]/*' />
        public event HtmlElementEventHandler Stop
        {
            add
            {
                DocumentShim.AddHandler(EventStop, value);
            }
            remove
            {
                DocumentShim.RemoveHandler(EventStop, value);
            }
        }

        //
        // Private helper methods:
        //
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FindSizeOfVariant
        {
            [MarshalAs(UnmanagedType.Struct)]
            public object var;
            public byte b;
        }
        private static readonly int VariantSize = (int)Marshal.OffsetOf(typeof(FindSizeOfVariant), "b");
        //
        // Convert a object[] into an array of VARIANT, allocated with CoTask allocators.
        internal unsafe static IntPtr ArrayToVARIANTVector(object[] args)
        {
            int len = args.Length;
            IntPtr mem = Marshal.AllocCoTaskMem(len * VariantSize);
            byte* a = (byte*)(void*)mem;
            for (int i = 0; i < len; ++i)
            {
                Marshal.GetNativeVariantForObject(args[i], (IntPtr)(a + VariantSize * i));
            }
            return mem;
        }

        //
        // Free a Variant array created with the above function
        internal unsafe static void FreeVARIANTVector(IntPtr mem, int len)
        {
            byte* a = (byte*)(void*)mem;
            for (int i = 0; i < len; ++i)
            {
                SafeNativeMethods.VariantClear(new HandleRef(null, (IntPtr)(a + VariantSize * i)));
            }
            Marshal.FreeCoTaskMem(mem);
        }

        private Color ColorFromObject(object oColor)
        {
            try
            {
                if (oColor is string)
                {
                    string strColor = oColor as String;
                    int index = strColor.IndexOf('#');
                    if (index >= 0)
                    {
                        // The string is of the form: #ff00a0. Skip past the #
                        string hexColor = strColor.Substring(index + 1);
                        // The actual color is non-transparent. So set alpha = 255.
                        return Color.FromArgb(255, Color.FromArgb(int.Parse(hexColor, NumberStyles.HexNumber, CultureInfo.InvariantCulture)));
                    }
                    else
                    {
                        return Color.FromName(strColor);
                    }
                }
                else if (oColor is int)
                {
                    // The actual color is non-transparent. So set alpha = 255.
                    return Color.FromArgb(255, Color.FromArgb((int)oColor));
                }
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }
            }

            return Color.Empty;
        }


        ///<devdoc>
        /// HtmlDocumentShim - this is the glue between the DOM eventing mechanisms
        ///                    and our CLR callbacks.  
        ///             
        ///  There are two kinds of events: HTMLWindowEvents2 and IHtmlWindow3.AttachHandler style
        ///     HTMLDocumentEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
        ///                        on our an instance of HTMLDocumentEvents2.  The HTMLDocumentEvents2 class then fires the event.
        ///
        ///     IHTMLDocument3.AttachHandler: MSHML calls back on an HtmlToClrEventProxy that we've created, looking
        ///                                 for a method named DISPID=0.  For each event that's subscribed, we create 
        ///                                 a new HtmlToClrEventProxy, detect the callback and fire the corresponding
        ///                                 CLR event.
        ///</devdoc>      
        internal class HtmlDocumentShim : HtmlShim
        {
            private AxHost.ConnectionPointCookie cookie;
            private HtmlDocument htmlDocument;
            private UnsafeNativeMethods.IHTMLWindow2 associatedWindow = null;

            internal HtmlDocumentShim(HtmlDocument htmlDocument)
            {
                this.htmlDocument = htmlDocument;
                // snap our associated window so we know when to disconnect.
                if (this.htmlDocument != null)
                {
                    HtmlWindow window = htmlDocument.Window;
                    if (window != null)
                    {
                        associatedWindow = window.NativeHtmlWindow;
                    }
                }
            }

            public override UnsafeNativeMethods.IHTMLWindow2 AssociatedWindow
            {
                get { return associatedWindow; }
            }

            public UnsafeNativeMethods.IHTMLDocument2 NativeHtmlDocument2
            {
                get { return htmlDocument.NativeHtmlDocument2; }
            }

            internal HtmlDocument Document
            {
                get { return htmlDocument; }
            }

            /// Support IHtmlDocument3.AttachHandler
            public override void AttachEventHandler(string eventName, EventHandler eventHandler)
            {

                // IE likes to call back on an IDispatch of DISPID=0 when it has an event, 
                // the HtmlToClrEventProxy helps us fake out the CLR so that we can call back on 
                // our EventHandler properly.

                HtmlToClrEventProxy proxy = AddEventProxy(eventName, eventHandler);
                bool success = ((UnsafeNativeMethods.IHTMLDocument3)this.NativeHtmlDocument2).AttachEvent(eventName, proxy);
                Debug.Assert(success, "failed to add event");
            }

            /// Support IHtmlDocument3.DetachHandler
            public override void DetachEventHandler(string eventName, EventHandler eventHandler)
            {
                HtmlToClrEventProxy proxy = RemoveEventProxy(eventHandler);
                if (proxy != null)
                {
                    ((UnsafeNativeMethods.IHTMLDocument3)this.NativeHtmlDocument2).DetachEvent(eventName, proxy);
                }

            }

            //
            // Connect to standard events
            //
            public override void ConnectToEvents()
            {
                if (cookie == null || !cookie.Connected)
                {
                    this.cookie = new AxHost.ConnectionPointCookie(this.NativeHtmlDocument2,
                                                                          new HTMLDocumentEvents2(htmlDocument),
                                                                          typeof(UnsafeNativeMethods.DHTMLDocumentEvents2),
                                                                          /*throwException*/ false);
                    if (!cookie.Connected) 
                    {
                        cookie = null;
                    }
                }
            }

            //
            // Disconnect from standard events
            //
            public override void DisconnectFromEvents()
            {
                if (this.cookie != null)
                {
                    this.cookie.Disconnect();
                    this.cookie = null;
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    if (htmlDocument != null)
                    {
                        Marshal.FinalReleaseComObject(htmlDocument.NativeHtmlDocument2);
                    }
                    htmlDocument = null;
                }

            }

            protected override object GetEventSender()
            {
                return htmlDocument;
            }
        }

        //
        // Private classes:
        //
        [ClassInterface(ClassInterfaceType.None)]
        private class HTMLDocumentEvents2 : StandardOleMarshalObject, /*Enforce calling back on the same thread*/
                                            UnsafeNativeMethods.DHTMLDocumentEvents2
        {
            private HtmlDocument parent;

            public HTMLDocumentEvents2(HtmlDocument htmlDocument)
            {
                this.parent = htmlDocument;
            }

            private void FireEvent(object key, EventArgs e)
            {
                if (this.parent != null)
                {
                    parent.DocumentShim.FireEvent(key, e);
                }
            }

            public bool onclick(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventClick, e);

                return e.ReturnValue;
            }

            public bool oncontextmenu(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventContextMenuShowing, e);
                return e.ReturnValue;
            }

            public void onfocusin(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventFocusing, e);
            }

            public void onfocusout(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventLosingFocus, e);
            }

            public void onmousemove(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseMove, e);
            }

            public void onmousedown(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseDown, e);
            }

            public void onmouseout(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseLeave, e);
            }

            public void onmouseover(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseOver, e);
            }

            public void onmouseup(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseUp, e);
            }

            public bool onstop(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventStop, e);
                return e.ReturnValue;
            }

            public bool onhelp(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool ondblclick(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onkeydown(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void onkeyup(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public bool onkeypress(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
            
            public void onreadystatechange(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public bool onbeforeupdate(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
            
            public void onafterupdate(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public bool onrowexit(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
            
            public void onrowenter(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public bool ondragstart(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
            
            public bool onselectstart(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
            
            public bool onerrorupdate(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
            
            public void onrowsdelete(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void onrowsinserted(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void oncellchange(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void onpropertychange(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void ondatasetchanged(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void ondataavailable(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void ondatasetcomplete(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void onbeforeeditfocus(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void onselectionchange(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public bool oncontrolselect(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
            
            public bool onmousewheel(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
            
            public void onactivate(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void ondeactivate(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public bool onbeforeactivate(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
            
            public bool onbeforedeactivate(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
        }

            #region operators

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlElement.operatorEQ"]/*' />
        [SuppressMessage("Microsoft.Design", "CA1046:DoNotOverrideOperatorEqualsOnReferenceTypes")]
        public static bool operator ==(HtmlDocument left, HtmlDocument right)
        {
            //Not equal if only one's null.
            if (object.ReferenceEquals(left, null) != object.ReferenceEquals(right, null))
            {
                return false;
            }

            //Equal if both are null.
            if (object.ReferenceEquals(left, null))
            {
                return true;
            }

            //Neither are null.  Get the IUnknowns and compare them.
            IntPtr leftPtr = IntPtr.Zero;
            IntPtr rightPtr = IntPtr.Zero;
            try
            {
                leftPtr = Marshal.GetIUnknownForObject(left.NativeHtmlDocument2);
                rightPtr = Marshal.GetIUnknownForObject(right.NativeHtmlDocument2);
                return leftPtr == rightPtr;
            }
            finally
            {
                if (leftPtr != IntPtr.Zero)
                {
                    Marshal.Release(leftPtr);
                }
                if (rightPtr != IntPtr.Zero)
                {
                    Marshal.Release(rightPtr);
                }
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.operatorNE"]/*' />
        public static bool operator !=(HtmlDocument left, HtmlDocument right)
        {
            return !(left == right);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.GetHashCode"]/*' />
        public override int GetHashCode()
        {
            return htmlDocument2 == null ? 0 : htmlDocument2.GetHashCode();
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Equals"]/*' />
        public override bool Equals(object obj)
        {
            return (this == (HtmlDocument)obj);
        }
            #endregion

    }
}

