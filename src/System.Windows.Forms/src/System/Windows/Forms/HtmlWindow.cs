// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.Windows.Forms
{
    /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow"]/*' />
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    public sealed class HtmlWindow
    {
        internal static readonly object EventError = new object();
        internal static readonly object EventGotFocus = new object();
        internal static readonly object EventLoad = new object();
        internal static readonly object EventLostFocus = new object();
        internal static readonly object EventResize = new object();
        internal static readonly object EventScroll = new object();
        internal static readonly object EventUnload = new object();

        private HtmlShimManager shimManager;
        private UnsafeNativeMethods.IHTMLWindow2 htmlWindow2;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        internal HtmlWindow(HtmlShimManager shimManager, UnsafeNativeMethods.IHTMLWindow2 win)
        {
            this.htmlWindow2 = win;
            Debug.Assert(this.NativeHtmlWindow != null, "The window object should implement IHTMLWindow2");

            this.shimManager = shimManager;
        }

        internal UnsafeNativeMethods.IHTMLWindow2 NativeHtmlWindow
        {
            get
            {
                return this.htmlWindow2;
            }
        }

        private HtmlShimManager ShimManager
        {
            get { return shimManager; }
        }

        private HtmlWindowShim WindowShim
        {
            get
            {
                if (ShimManager != null)
                {
                    HtmlWindowShim shim = ShimManager.GetWindowShim(this);
                    if (shim == null)
                    {
                        shimManager.AddWindowShim(this);
                        shim = ShimManager.GetWindowShim(this);
                    }
                    return shim;
                }
                return null;
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Document"]/*' />
        public HtmlDocument Document
        {
            get
            {
                UnsafeNativeMethods.IHTMLDocument iHTMLDocument = this.NativeHtmlWindow.GetDocument() as UnsafeNativeMethods.IHTMLDocument;
                return iHTMLDocument != null ? new HtmlDocument(ShimManager, iHTMLDocument) : null;
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.DomWindow"]/*' />
        public object DomWindow
        {
            get
            {
                return this.NativeHtmlWindow;
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Frames"]/*' />
        public HtmlWindowCollection Frames
        {
            get
            {
                UnsafeNativeMethods.IHTMLFramesCollection2 iHTMLFramesCollection2 = this.NativeHtmlWindow.GetFrames();
                return (iHTMLFramesCollection2 != null) ? new HtmlWindowCollection(ShimManager, iHTMLFramesCollection2) : null;
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.History"]/*' />
        public HtmlHistory History
        {
            get
            {
                UnsafeNativeMethods.IOmHistory iOmHistory = this.NativeHtmlWindow.GetHistory();
                return iOmHistory != null ? new HtmlHistory(iOmHistory) : null;
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.IsClosed"]/*' />
        public bool IsClosed
        {
            get
            {
                return this.NativeHtmlWindow.GetClosed();
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Name"]/*' />
        /// <devdoc>
        ///    <para>Name of the NativeHtmlWindow</para>
        /// </devdoc>
        public string Name
        {
            get
            {
                return this.NativeHtmlWindow.GetName();
            }
            set
            {
                this.NativeHtmlWindow.SetName(value);
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Opener"]/*' />
        public HtmlWindow Opener
        {
            get
            {
                UnsafeNativeMethods.IHTMLWindow2 iHTMLWindow2 = this.NativeHtmlWindow.GetOpener() as UnsafeNativeMethods.IHTMLWindow2;
                return (iHTMLWindow2 != null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Parent"]/*' />
        public HtmlWindow Parent
        {
            get
            {
                UnsafeNativeMethods.IHTMLWindow2 iHTMLWindow2 = this.NativeHtmlWindow.GetParent();
                return (iHTMLWindow2 != null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Position"]/*' />
        public Point Position
        {
            get
            {
                return new Point(((UnsafeNativeMethods.IHTMLWindow3)this.NativeHtmlWindow).GetScreenLeft(),
                        ((UnsafeNativeMethods.IHTMLWindow3)this.NativeHtmlWindow).GetScreenTop());
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Size"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets size for the window</para>
        /// </devdoc>
        public Size Size
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement bodyElement = this.NativeHtmlWindow.GetDocument().GetBody();
                return new Size(bodyElement.GetOffsetWidth(), bodyElement.GetOffsetHeight());
            }
            set
            {
                ResizeTo(value.Width, value.Height);
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.StatusBarText"]/*' />
        public string StatusBarText
        {
            get
            {
                return this.NativeHtmlWindow.GetStatus();
            }
            set
            {
                this.NativeHtmlWindow.SetStatus(value);
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Url"]/*' />
        public Uri Url
        {
            get 
            {
                UnsafeNativeMethods.IHTMLLocation iHtmlLocation = this.NativeHtmlWindow.GetLocation();
                string stringLocation = (iHtmlLocation == null) ? "" : iHtmlLocation.GetHref();
                return string.IsNullOrEmpty(stringLocation) ? null : new Uri(stringLocation);
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.WindowFrameElement"]/*' />
        public HtmlElement WindowFrameElement
        {
            get
            {
                UnsafeNativeMethods.IHTMLElement htmlElement = ((UnsafeNativeMethods.IHTMLWindow4)this.NativeHtmlWindow).frameElement() as UnsafeNativeMethods.IHTMLElement;
                return (htmlElement != null) ? new HtmlElement(ShimManager, htmlElement) : null;
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Alert"]/*' />
        public void Alert(string message)
        {
            this.NativeHtmlWindow.Alert(message);
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.AttachEventHandler"]/*' />
        public void AttachEventHandler(string eventName, EventHandler eventHandler)
        {
            WindowShim.AttachEventHandler(eventName, eventHandler);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Close"]/*' />
        public void Close()
        {
            this.NativeHtmlWindow.Close();
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Confirm"]/*' />
        public bool Confirm(string message)
        {
            return this.NativeHtmlWindow.Confirm(message);
        }

        /// <include file='doc\HtmlDocument.uex' path='docs/doc[@for="HtmlDocument.DetachEventHandler"]/*' />
        public void DetachEventHandler(string eventName, EventHandler eventHandler)
        {
            WindowShim.DetachEventHandler(eventName, eventHandler);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Focus"]/*' />
        public void Focus()
        {
            this.NativeHtmlWindow.Focus();
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.MoveTo"]/*' />
        /// <devdoc>
        ///    <para>Moves the Window to the position requested</para>
        /// </devdoc>
        public void MoveTo(int x, int y)
        {
            this.NativeHtmlWindow.MoveTo(x, y);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.MoveTo2"]/*' />
        /// <devdoc>
        ///    <para>Moves the Window to the point requested</para>
        /// </devdoc>
        public void MoveTo(Point point)
        {
            this.NativeHtmlWindow.MoveTo(point.X, point.Y);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Navigate"]/*' />
        public void Navigate(Uri url)
        {
            this.NativeHtmlWindow.Navigate(url.ToString());
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Navigate"]/*' />
        /// Note: We intentionally have a string overload (apparently Mort wants one).  We don't have 
        /// string overloads call Uri overloads because that breaks Uris that aren't fully qualified 
        /// (things like "www.microsoft.com") that the underlying objects support and we don't want to 
        /// break.
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads")]
        public void Navigate(string urlString)
        {
            this.NativeHtmlWindow.Navigate(urlString);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Open"]/*' />
        /// Note: We intentionally have a string overload (apparently Mort wants one).  We don't have 
        /// string overloads call Uri overloads because that breaks Uris that aren't fully qualified 
        /// (things like "www.microsoft.com") that the underlying objects support and we don't want to 
        /// break.
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads")]
        public HtmlWindow Open(string urlString, string target, string windowOptions, bool replaceEntry)
        {
            UnsafeNativeMethods.IHTMLWindow2 iHTMLWindow2 = this.NativeHtmlWindow.Open(urlString, target, windowOptions, replaceEntry);
            return (iHTMLWindow2 != null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Open"]/*' />
        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public HtmlWindow Open(Uri url, string target, string windowOptions, bool replaceEntry)
        {
            return Open(url.ToString(), target, windowOptions, replaceEntry);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.OpenNew"]/*' />
        /// Note: We intentionally have a string overload (apparently Mort wants one).  We don't have 
        /// string overloads call Uri overloads because that breaks Uris that aren't fully qualified 
        /// (things like "www.microsoft.com") that the underlying objects support and we don't want to 
        /// break.
        [SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads")]
        public HtmlWindow OpenNew(string urlString, string windowOptions)
        {
            UnsafeNativeMethods.IHTMLWindow2 iHTMLWindow2 = this.NativeHtmlWindow.Open(urlString, "_blank", windowOptions, true);
            return (iHTMLWindow2 != null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.OpenNew"]/*' />
        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public HtmlWindow OpenNew(Uri url, string windowOptions)
        {
            return OpenNew(url.ToString(), windowOptions);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Prompt"]/*' />
        public string Prompt(string message, string defaultInputValue)
        {
            return this.NativeHtmlWindow.Prompt(message, defaultInputValue).ToString();
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.RemoveFocus"]/*' />
        public void RemoveFocus()
        {
            this.NativeHtmlWindow.Blur();
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.ResizeTo"]/*' />
        /// <devdoc>
        ///    <para>Resize the window to the width/height requested</para>
        /// </devdoc>
        public void ResizeTo(int width, int height)
        {
            this.NativeHtmlWindow.ResizeTo(width, height);
        }

        /// <devdoc>
        ///    <para>Resize the window to the Size requested</para>
        /// </devdoc>
        public void ResizeTo(Size size)
        {
            this.NativeHtmlWindow.ResizeTo(size.Width, size.Height);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.ScrollTo"]/*' />
        /// <devdoc>
        ///    <para>Scroll the window to the position requested</para>
        /// </devdoc>
        public void ScrollTo(int x, int y)
        {
            this.NativeHtmlWindow.ScrollTo(x, y);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.ScrollTo2"]/*' />
        /// <devdoc>
        ///    <para>Scroll the window to the point requested</para>
        /// </devdoc>
        public void ScrollTo(Point point)
        {
            this.NativeHtmlWindow.ScrollTo(point.X, point.Y);
        }

        //
        // Events
        //

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Error"]/*' />
        public event HtmlElementErrorEventHandler Error
        {
            add
            {
                WindowShim.AddHandler(EventError, value);
            }
            remove
            {
                WindowShim.RemoveHandler(EventError, value);
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.GotFocus"]/*' />
        public event HtmlElementEventHandler GotFocus
        {
            add
            {
                WindowShim.AddHandler(EventGotFocus, value);
            }
            remove
            {
                WindowShim.RemoveHandler(EventGotFocus, value);
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Load"]/*' />
        public event HtmlElementEventHandler Load
        {
            add
            {
                WindowShim.AddHandler(EventLoad, value);
            }
            remove
            {
                WindowShim.RemoveHandler(EventLoad, value);
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.LostFocus"]/*' />
        public event HtmlElementEventHandler LostFocus
        {
            add
            {
                WindowShim.AddHandler(EventLostFocus, value);
            }
            remove
            {
                WindowShim.RemoveHandler(EventLostFocus, value);
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Resize"]/*' />
        public event HtmlElementEventHandler Resize
        {
            add
            {
                WindowShim.AddHandler(EventResize, value);
            }
            remove
            {
                WindowShim.RemoveHandler(EventResize, value);
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Scroll"]/*' />
        public event HtmlElementEventHandler Scroll
        {
            add
            {
                WindowShim.AddHandler(EventScroll, value);
            }
            remove
            {
                WindowShim.RemoveHandler(EventScroll, value);
            }
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Unload"]/*' />
        public event HtmlElementEventHandler Unload
        {
            add
            {
                WindowShim.AddHandler(EventUnload, value);
            }
            remove
            {
                WindowShim.RemoveHandler(EventUnload, value);
            }
        }




        //
        // Private classes:
        //
        [ClassInterface(ClassInterfaceType.None)]
        private class HTMLWindowEvents2 : StandardOleMarshalObject, /*Enforce calling back on the same thread*/
            UnsafeNativeMethods.DHTMLWindowEvents2
        {
            private HtmlWindow parent;

            public HTMLWindowEvents2(HtmlWindow htmlWindow)
            {
                this.parent = htmlWindow;
            }

            private void FireEvent(object key, EventArgs e)
            {
                if (this.parent != null)
                {
                    parent.WindowShim.FireEvent(key, e);
                }
            }

            public void onfocus(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventGotFocus, e);
            }

            public void onblur(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventLostFocus, e);
            }

            public bool onerror(string description, string urlString, int line)
            {
                HtmlElementErrorEventArgs e = new HtmlElementErrorEventArgs(description, urlString, line);
                FireEvent(HtmlWindow.EventError, e);
                return e.Handled;
            }

            public void onload(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventLoad, e);
            }

            public void onunload(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventUnload, e);
                if (parent != null)
                {
                    parent.WindowShim.OnWindowUnload();
                }
            }

            public void onscroll(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventScroll, e);
            }

            public void onresize(UnsafeNativeMethods.IHTMLEventObj evtObj) 
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventResize, e);
            }

            public bool onhelp(UnsafeNativeMethods.IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onbeforeunload(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void onbeforeprint(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
            
            public void onafterprint(UnsafeNativeMethods.IHTMLEventObj evtObj) { }
        }



        ///<devdoc>
        /// HtmlWindowShim - this is the glue between the DOM eventing mechanisms
        ///                  and our CLR callbacks.  
        ///             
        ///  There are two kinds of events: HTMLWindowEvents2 and IHtmlWindow3.AttachHandler style
        ///     HTMLWindowEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
        ///                        on an instance of HTMLWindowEvents2.  The HTMLWindowEvents2 class then fires the event.
        ///
        ///     IHTMLWindow3.AttachHandler: MSHML calls back on an HtmlToClrEventProxy that we've created, looking
        ///                                 for a method named DISPID=0.  For each event that's subscribed, we create 
        ///                                 a new HtmlToClrEventProxy, detect the callback and fire the corresponding
        ///                                 CLR event.
        ///</devdoc>
        internal class HtmlWindowShim : HtmlShim
        {
            private AxHost.ConnectionPointCookie cookie;
            private HtmlWindow htmlWindow;

            public HtmlWindowShim(HtmlWindow window)
            {
                this.htmlWindow = window;
            }

            public UnsafeNativeMethods.IHTMLWindow2 NativeHtmlWindow
            {
                get { return htmlWindow.NativeHtmlWindow; }
            }

            public override UnsafeNativeMethods.IHTMLWindow2 AssociatedWindow
            {
                get { return htmlWindow.NativeHtmlWindow; }
            }

            /// Support IHtmlDocument3.AttachHandler
            public override void AttachEventHandler(string eventName, System.EventHandler eventHandler)
            {

                // IE likes to call back on an IDispatch of DISPID=0 when it has an event, 
                // the HtmlToClrEventProxy helps us fake out the CLR so that we can call back on 
                // our EventHandler properly.

                HtmlToClrEventProxy proxy = AddEventProxy(eventName, eventHandler);
                bool success = ((UnsafeNativeMethods.IHTMLWindow3)this.NativeHtmlWindow).AttachEvent(eventName, proxy);
                Debug.Assert(success, "failed to add event");
            }

            /// Support HTMLWindowEvents2
            public override void ConnectToEvents()
            {
                if (cookie == null || !cookie.Connected)
                {
                    this.cookie = new AxHost.ConnectionPointCookie(NativeHtmlWindow,
                                                                              new HTMLWindowEvents2(htmlWindow),
                                                                              typeof(UnsafeNativeMethods.DHTMLWindowEvents2),
                                                                              /*throwException*/ false);
                    if (!cookie.Connected) 
                    {
                        cookie = null;
                    }
                }
            }

            /// Support IHTMLWindow3.DetachHandler
            public override void DetachEventHandler(string eventName, System.EventHandler eventHandler)
            {
                HtmlToClrEventProxy proxy = RemoveEventProxy(eventHandler);
                if (proxy != null)
                {
                    ((UnsafeNativeMethods.IHTMLWindow3)this.NativeHtmlWindow).DetachEvent(eventName, proxy);
                }
            }

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
                    if (htmlWindow != null && htmlWindow.NativeHtmlWindow != null)
                    {
                        Marshal.FinalReleaseComObject(htmlWindow.NativeHtmlWindow);
                    }
                    htmlWindow = null;
                }
            }

            protected override object GetEventSender()
            {
                return htmlWindow;
            }

            public void OnWindowUnload()
            {
                if (htmlWindow != null)
                {
                    htmlWindow.ShimManager.OnWindowUnloaded(htmlWindow);
                }
            }
        }

            #region operators

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlElement.operatorEQ"]/*' />
        [SuppressMessage("Microsoft.Design", "CA1046:DoNotOverrideOperatorEqualsOnReferenceTypes")]
        public static bool operator ==(HtmlWindow left, HtmlWindow right)
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
                leftPtr = Marshal.GetIUnknownForObject(left.NativeHtmlWindow);
                rightPtr = Marshal.GetIUnknownForObject(right.NativeHtmlWindow);
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
        public static bool operator !=(HtmlWindow left, HtmlWindow right)
        {
            return !(left == right);
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.GetHashCode"]/*' />
        public override int GetHashCode()
        {
            return htmlWindow2 == null ? 0 : htmlWindow2.GetHashCode();
        }

        /// <include file='doc\HtmlWindow.uex' path='docs/doc[@for="HtmlWindow.Equals"]/*' />
        public override bool Equals(object obj)
        {
            return (this == (HtmlWindow)obj);
        }
            #endregion

    }
}
