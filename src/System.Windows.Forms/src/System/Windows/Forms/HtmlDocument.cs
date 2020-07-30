// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
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

        private readonly IHTMLDocument2 htmlDocument2;
        private readonly HtmlShimManager shimManager;

        internal HtmlDocument(HtmlShimManager shimManager, IHTMLDocument doc)
        {
            htmlDocument2 = (IHTMLDocument2)doc;
            Debug.Assert(NativeHtmlDocument2 != null, "The document should implement IHtmlDocument2");

            this.shimManager = shimManager;
        }

        internal IHTMLDocument2 NativeHtmlDocument2
        {
            get
            {
                return htmlDocument2;
            }
        }

        private HtmlDocumentShim DocumentShim
        {
            get
            {
                if (ShimManager != null)
                {
                    HtmlDocumentShim shim = ShimManager.GetDocumentShim(this);
                    if (shim is null)
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
                return shimManager;
            }
        }

        public HtmlElement ActiveElement
        {
            get
            {
                IHTMLElement iHtmlElement = NativeHtmlDocument2.GetActiveElement();
                return iHtmlElement != null ? new HtmlElement(ShimManager, iHtmlElement) : null;
            }
        }

        public HtmlElement Body
        {
            get
            {
                IHTMLElement iHtmlElement = NativeHtmlDocument2.GetBody();
                return iHtmlElement != null ? new HtmlElement(ShimManager, iHtmlElement) : null;
            }
        }

        public string Domain
        {
            get
            {
                return NativeHtmlDocument2.GetDomain();
            }
            set
            {
                try
                {
                    NativeHtmlDocument2.SetDomain(value);
                }
                catch (ArgumentException)
                {
                    // Give a better message describing the error
                    throw new ArgumentException(SR.HtmlDocumentInvalidDomain);
                }
            }
        }

        public string Title
        {
            get
            {
                return NativeHtmlDocument2.GetTitle();
            }
            set
            {
                NativeHtmlDocument2.SetTitle(value);
            }
        }

        public Uri Url
        {
            get
            {
                IHTMLLocation iHtmlLocation = NativeHtmlDocument2.GetLocation();
                string stringLocation = (iHtmlLocation is null) ? "" : iHtmlLocation.GetHref();
                return string.IsNullOrEmpty(stringLocation) ? null : new Uri(stringLocation);
            }
        }

        public HtmlWindow Window
        {
            get
            {
                IHTMLWindow2 iHTMLWindow2 = NativeHtmlDocument2.GetParentWindow();
                return iHTMLWindow2 != null ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
            }
        }

        public Color BackColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = ColorFromObject(NativeHtmlDocument2.GetBgColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                NativeHtmlDocument2.SetBgColor(color);
            }
        }

        public Color ForeColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = ColorFromObject(NativeHtmlDocument2.GetFgColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                NativeHtmlDocument2.SetFgColor(color);
            }
        }

        public Color LinkColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = ColorFromObject(NativeHtmlDocument2.GetLinkColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                NativeHtmlDocument2.SetLinkColor(color);
            }
        }

        public Color ActiveLinkColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = ColorFromObject(NativeHtmlDocument2.GetAlinkColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                NativeHtmlDocument2.SetAlinkColor(color);
            }
        }

        public Color VisitedLinkColor
        {
            get
            {
                Color c = Color.Empty;
                try
                {
                    c = ColorFromObject(NativeHtmlDocument2.GetVlinkColor());
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
                return c;
            }
            set
            {
                int color = value.R << 16 | value.G << 8 | value.B;
                NativeHtmlDocument2.SetVlinkColor(color);
            }
        }

        public bool Focused
        {
            get
            {
                return ((IHTMLDocument4)NativeHtmlDocument2).HasFocus();
            }
        }

        public object DomDocument
        {
            get
            {
                return NativeHtmlDocument2;
            }
        }

        public string Cookie
        {
            get
            {
                return NativeHtmlDocument2.GetCookie();
            }
            set
            {
                NativeHtmlDocument2.SetCookie(value);
            }
        }

        public bool RightToLeft
        {
            get
            {
                return ((IHTMLDocument3)NativeHtmlDocument2).GetDir() == "rtl";
            }
            set
            {
                ((IHTMLDocument3)NativeHtmlDocument2).SetDir(value ? "rtl" : "ltr");
            }
        }

        public string Encoding
        {
            get
            {
                return NativeHtmlDocument2.GetCharset();
            }
            set
            {
                NativeHtmlDocument2.SetCharset(value);
            }
        }

        public string DefaultEncoding
        {
            get
            {
                return NativeHtmlDocument2.GetDefaultCharset();
            }
        }

        public HtmlElementCollection All
        {
            get
            {
                IHTMLElementCollection iHTMLElementCollection = NativeHtmlDocument2.GetAll();
                return iHTMLElementCollection != null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        public HtmlElementCollection Links
        {
            get
            {
                IHTMLElementCollection iHTMLElementCollection = NativeHtmlDocument2.GetLinks();
                return iHTMLElementCollection != null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        public HtmlElementCollection Images
        {
            get
            {
                IHTMLElementCollection iHTMLElementCollection = NativeHtmlDocument2.GetImages();
                return iHTMLElementCollection != null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        public HtmlElementCollection Forms
        {
            get
            {
                IHTMLElementCollection iHTMLElementCollection = NativeHtmlDocument2.GetForms();
                return iHTMLElementCollection != null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
            }
        }

        public void Write(string text)
        {
            object[] strs = new object[] { (object)text };
            NativeHtmlDocument2.Write(strs);
        }

        /// <summary>
        ///  Executes a command on the document
        /// </summary>
        public void ExecCommand(string command, bool showUI, object value)
        {
            NativeHtmlDocument2.ExecCommand(command, showUI, value);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Focus()
        {
            ((IHTMLDocument4)NativeHtmlDocument2).Focus();
            // Seems to have a problem in really setting focus the first time
            ((IHTMLDocument4)NativeHtmlDocument2).Focus();
        }

        public HtmlElement GetElementById(string id)
        {
            IHTMLElement iHTMLElement = ((IHTMLDocument3)NativeHtmlDocument2).GetElementById(id);
            return iHTMLElement != null ? new HtmlElement(ShimManager, iHTMLElement) : null;
        }

        public HtmlElement GetElementFromPoint(Point point)
        {
            IHTMLElement iHTMLElement = NativeHtmlDocument2.ElementFromPoint(point.X, point.Y);
            return iHTMLElement != null ? new HtmlElement(ShimManager, iHTMLElement) : null;
        }

        public HtmlElementCollection GetElementsByTagName(string tagName)
        {
            IHTMLElementCollection iHTMLElementCollection = ((IHTMLDocument3)NativeHtmlDocument2).GetElementsByTagName(tagName);
            return iHTMLElementCollection != null ? new HtmlElementCollection(ShimManager, iHTMLElementCollection) : new HtmlElementCollection(ShimManager);
        }

        public HtmlDocument OpenNew(bool replaceInHistory)
        {
            object name = (object)(replaceInHistory ? "replace" : "");
            object nullObject = null;
            object ohtmlDocument = NativeHtmlDocument2.Open("text/html", name, nullObject, nullObject);
            return ohtmlDocument is IHTMLDocument iHTMLDocument ? new HtmlDocument(ShimManager, iHTMLDocument) : null;
        }

        public HtmlElement CreateElement(string elementTag)
        {
            IHTMLElement iHTMLElement = NativeHtmlDocument2.CreateElement(elementTag);
            return iHTMLElement != null ? new HtmlElement(ShimManager, iHTMLElement) : null;
        }

        public unsafe object InvokeScript(string scriptName, object[] args)
        {
            try
            {
                if (NativeHtmlDocument2.GetScript() is Oleaut32.IDispatch scriptObject)
                {
                    Guid g = Guid.Empty;
                    string[] names = new string[] { scriptName };
                    Ole32.DispatchID dispid = Ole32.DispatchID.UNKNOWN;
                    HRESULT hr = scriptObject.GetIDsOfNames(&g, names, 1, Kernel32.GetThreadLocale(), &dispid);
                    if (!hr.Succeeded() || dispid == Ole32.DispatchID.UNKNOWN)
                    {
                        return null;
                    }

                    if (args != null)
                    {
                        // Reverse the arg order so that they read naturally after IDispatch.
                        Array.Reverse(args);
                    }

                    using var vectorArgs = new Oleaut32.VARIANTVector(args);
                    fixed (Oleaut32.VARIANT* pVariants = vectorArgs.Variants)
                    {
                        var dispParams = new Oleaut32.DISPPARAMS();
                        dispParams.rgvarg = pVariants;
                        dispParams.cArgs = (uint)vectorArgs.Variants.Length;
                        dispParams.rgdispidNamedArgs = null;
                        dispParams.cNamedArgs = 0;

                        var retVals = new object[1];
                        var excepInfo = new Oleaut32.EXCEPINFO();
                        hr = scriptObject.Invoke(
                            dispid,
                            &g,
                            Kernel32.GetThreadLocale(),
                            Oleaut32.DISPATCH.METHOD,
                            &dispParams,
                            retVals,
                            &excepInfo,
                            null);
                        if (hr == HRESULT.S_OK)
                        {
                            return retVals[0];
                        }
                    }
                }
            }
            catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
            {
            }

            return null;
        }

        public object InvokeScript(string scriptName)
        {
            return InvokeScript(scriptName, null);
        }

        public void AttachEventHandler(string eventName, EventHandler eventHandler)
        {
            HtmlDocumentShim shim = DocumentShim;
            if (shim != null)
            {
                shim.AttachEventHandler(eventName, eventHandler);
            }
        }

        public void DetachEventHandler(string eventName, EventHandler eventHandler)
        {
            HtmlDocumentShim shim = DocumentShim;
            if (shim != null)
            {
                shim.DetachEventHandler(eventName, eventHandler);
            }
        }

        public event HtmlElementEventHandler Click
        {
            add => DocumentShim.AddHandler(EventClick, value);
            remove => DocumentShim.RemoveHandler(EventClick, value);
        }

        public event HtmlElementEventHandler ContextMenuShowing
        {
            add => DocumentShim.AddHandler(EventContextMenuShowing, value);
            remove => DocumentShim.RemoveHandler(EventContextMenuShowing, value);
        }

        public event HtmlElementEventHandler Focusing
        {
            add => DocumentShim.AddHandler(EventFocusing, value);
            remove => DocumentShim.RemoveHandler(EventFocusing, value);
        }

        public event HtmlElementEventHandler LosingFocus
        {
            add => DocumentShim.AddHandler(EventLosingFocus, value);
            remove => DocumentShim.RemoveHandler(EventLosingFocus, value);
        }

        public event HtmlElementEventHandler MouseDown
        {
            add => DocumentShim.AddHandler(EventMouseDown, value);
            remove => DocumentShim.RemoveHandler(EventMouseDown, value);
        }

        /// <summary>
        ///  Occurs when the mouse leaves the document
        /// </summary>
        public event HtmlElementEventHandler MouseLeave
        {
            add => DocumentShim.AddHandler(EventMouseLeave, value);
            remove => DocumentShim.RemoveHandler(EventMouseLeave, value);
        }

        public event HtmlElementEventHandler MouseMove
        {
            add => DocumentShim.AddHandler(EventMouseMove, value);
            remove => DocumentShim.RemoveHandler(EventMouseMove, value);
        }

        public event HtmlElementEventHandler MouseOver
        {
            add => DocumentShim.AddHandler(EventMouseOver, value);
            remove => DocumentShim.RemoveHandler(EventMouseOver, value);
        }

        public event HtmlElementEventHandler MouseUp
        {
            add => DocumentShim.AddHandler(EventMouseUp, value);
            remove => DocumentShim.RemoveHandler(EventMouseUp, value);
        }

        public event HtmlElementEventHandler Stop
        {
            add => DocumentShim.AddHandler(EventStop, value);
            remove => DocumentShim.RemoveHandler(EventStop, value);
        }

        private Color ColorFromObject(object oColor)
        {
            try
            {
                if (oColor is string)
                {
                    string strColor = oColor as string;
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
                if (ClientUtils.IsCriticalException(ex))
                {
                    throw;
                }
            }

            return Color.Empty;
        }

        /// <summary>
        ///  HtmlDocumentShim - this is the glue between the DOM eventing mechanisms
        ///          and our CLR callbacks.
        ///
        ///  There are two kinds of events: HTMLWindowEvents2 and IHtmlWindow3.AttachHandler style
        ///  HTMLDocumentEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
        ///              on our an instance of HTMLDocumentEvents2.  The HTMLDocumentEvents2 class then fires the event.
        ///
        ///  IHTMLDocument3.AttachHandler: MSHML calls back on an HtmlToClrEventProxy that we've created, looking
        ///                       for a method named DISPID=0.  For each event that's subscribed, we create
        ///                       a new HtmlToClrEventProxy, detect the callback and fire the corresponding
        ///                       CLR event.
        /// </summary>
        internal class HtmlDocumentShim : HtmlShim
        {
            private AxHost.ConnectionPointCookie cookie;
            private HtmlDocument htmlDocument;
            private readonly IHTMLWindow2 associatedWindow;

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

            public override IHTMLWindow2 AssociatedWindow
            {
                get { return associatedWindow; }
            }

            public IHTMLDocument2 NativeHtmlDocument2
            {
                get { return htmlDocument.NativeHtmlDocument2; }
            }

            internal HtmlDocument Document
            {
                get { return htmlDocument; }
            }

            ///  Support IHtmlDocument3.AttachHandler
            public override void AttachEventHandler(string eventName, EventHandler eventHandler)
            {
                // IE likes to call back on an IDispatch of DISPID=0 when it has an event,
                // the HtmlToClrEventProxy helps us fake out the CLR so that we can call back on
                // our EventHandler properly.

                HtmlToClrEventProxy proxy = AddEventProxy(eventName, eventHandler);
                ((IHTMLDocument3)NativeHtmlDocument2).AttachEvent(eventName, proxy);
            }

            ///  Support IHtmlDocument3.DetachHandler
            public override void DetachEventHandler(string eventName, EventHandler eventHandler)
            {
                HtmlToClrEventProxy proxy = RemoveEventProxy(eventHandler);
                if (proxy != null)
                {
                    ((IHTMLDocument3)NativeHtmlDocument2).DetachEvent(eventName, proxy);
                }
            }

            //
            // Connect to standard events
            //
            public override void ConnectToEvents()
            {
                if (cookie is null || !cookie.Connected)
                {
                    cookie = new AxHost.ConnectionPointCookie(NativeHtmlDocument2,
                                                                          new HTMLDocumentEvents2(htmlDocument),
                                                                          typeof(DHTMLDocumentEvents2),
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
                if (cookie != null)
                {
                    cookie.Disconnect();
                    cookie = null;
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
                                            DHTMLDocumentEvents2
        {
            private readonly HtmlDocument parent;

            public HTMLDocumentEvents2(HtmlDocument htmlDocument)
            {
                parent = htmlDocument;
            }

            private void FireEvent(object key, EventArgs e)
            {
                if (parent != null)
                {
                    parent.DocumentShim.FireEvent(key, e);
                }
            }

            public bool onclick(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventClick, e);

                return e.ReturnValue;
            }

            public bool oncontextmenu(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventContextMenuShowing, e);
                return e.ReturnValue;
            }

            public void onfocusin(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventFocusing, e);
            }

            public void onfocusout(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventLosingFocus, e);
            }

            public void onmousemove(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseMove, e);
            }

            public void onmousedown(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseDown, e);
            }

            public void onmouseout(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseLeave, e);
            }

            public void onmouseover(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseOver, e);
            }

            public void onmouseup(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventMouseUp, e);
            }

            public bool onstop(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlDocument.EventStop, e);
                return e.ReturnValue;
            }

            public bool onhelp(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool ondblclick(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onkeydown(IHTMLEventObj evtObj) { }

            public void onkeyup(IHTMLEventObj evtObj) { }

            public bool onkeypress(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onreadystatechange(IHTMLEventObj evtObj) { }

            public bool onbeforeupdate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onafterupdate(IHTMLEventObj evtObj) { }

            public bool onrowexit(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onrowenter(IHTMLEventObj evtObj) { }

            public bool ondragstart(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onselectstart(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onerrorupdate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onrowsdelete(IHTMLEventObj evtObj) { }

            public void onrowsinserted(IHTMLEventObj evtObj) { }

            public void oncellchange(IHTMLEventObj evtObj) { }

            public void onpropertychange(IHTMLEventObj evtObj) { }

            public void ondatasetchanged(IHTMLEventObj evtObj) { }

            public void ondataavailable(IHTMLEventObj evtObj) { }

            public void ondatasetcomplete(IHTMLEventObj evtObj) { }

            public void onbeforeeditfocus(IHTMLEventObj evtObj) { }

            public void onselectionchange(IHTMLEventObj evtObj) { }

            public bool oncontrolselect(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onmousewheel(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onactivate(IHTMLEventObj evtObj) { }

            public void ondeactivate(IHTMLEventObj evtObj) { }

            public bool onbeforeactivate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public bool onbeforedeactivate(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }
        }

        #region operators

        public static bool operator ==(HtmlDocument left, HtmlDocument right)
        {
            //Not equal if only one's null.
            if (left is null != right is null)
            {
                return false;
            }

            //Equal if both are null.
            if (left is null)
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

        public static bool operator !=(HtmlDocument left, HtmlDocument right)
        {
            return !(left == right);
        }

        public override int GetHashCode() => htmlDocument2?.GetHashCode() ?? 0;

        public override bool Equals(object obj)
        {
            return (this == (HtmlDocument)obj);
        }
        #endregion

    }
}
