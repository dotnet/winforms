// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed class HtmlWindow
    {
        internal static readonly object EventError = new object();
        internal static readonly object EventGotFocus = new object();
        internal static readonly object EventLoad = new object();
        internal static readonly object EventLostFocus = new object();
        internal static readonly object EventResize = new object();
        internal static readonly object EventScroll = new object();
        internal static readonly object EventUnload = new object();

        private readonly HtmlShimManager shimManager;
        private readonly IHTMLWindow2 htmlWindow2;

        internal HtmlWindow(HtmlShimManager shimManager, IHTMLWindow2 win)
        {
            htmlWindow2 = win;
            Debug.Assert(NativeHtmlWindow != null, "The window object should implement IHTMLWindow2");

            this.shimManager = shimManager;
        }

        internal IHTMLWindow2 NativeHtmlWindow
        {
            get
            {
                return htmlWindow2;
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
                    if (shim is null)
                    {
                        shimManager.AddWindowShim(this);
                        shim = ShimManager.GetWindowShim(this);
                    }
                    return shim;
                }
                return null;
            }
        }

        public HtmlDocument Document
        {
            get
            {
                return NativeHtmlWindow.GetDocument() is IHTMLDocument iHTMLDocument ? new HtmlDocument(ShimManager, iHTMLDocument) : null;
            }
        }

        public object DomWindow
        {
            get
            {
                return NativeHtmlWindow;
            }
        }

        public HtmlWindowCollection Frames
        {
            get
            {
                IHTMLFramesCollection2 iHTMLFramesCollection2 = NativeHtmlWindow.GetFrames();
                return (iHTMLFramesCollection2 != null) ? new HtmlWindowCollection(ShimManager, iHTMLFramesCollection2) : null;
            }
        }

        public HtmlHistory History
        {
            get
            {
                IOmHistory iOmHistory = NativeHtmlWindow.GetHistory();
                return iOmHistory != null ? new HtmlHistory(iOmHistory) : null;
            }
        }

        public bool IsClosed
        {
            get
            {
                return NativeHtmlWindow.GetClosed();
            }
        }

        /// <summary>
        ///  Name of the NativeHtmlWindow
        /// </summary>
        public string Name
        {
            get
            {
                return NativeHtmlWindow.GetName();
            }
            set
            {
                NativeHtmlWindow.SetName(value);
            }
        }

        public HtmlWindow Opener
        {
            get
            {
                return (NativeHtmlWindow.GetOpener() is IHTMLWindow2 iHTMLWindow2) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
            }
        }

        public HtmlWindow Parent
        {
            get
            {
                IHTMLWindow2 iHTMLWindow2 = NativeHtmlWindow.GetParent();
                return (iHTMLWindow2 != null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
            }
        }

        public Point Position
        {
            get
            {
                return new Point(((IHTMLWindow3)NativeHtmlWindow).GetScreenLeft(),
                        ((IHTMLWindow3)NativeHtmlWindow).GetScreenTop());
            }
        }

        /// <summary>
        ///  Gets or sets size for the window
        /// </summary>
        public Size Size
        {
            get
            {
                IHTMLElement bodyElement = NativeHtmlWindow.GetDocument().GetBody();
                return new Size(bodyElement.GetOffsetWidth(), bodyElement.GetOffsetHeight());
            }
            set
            {
                ResizeTo(value.Width, value.Height);
            }
        }

        public string StatusBarText
        {
            get
            {
                return NativeHtmlWindow.GetStatus();
            }
            set
            {
                NativeHtmlWindow.SetStatus(value);
            }
        }

        public Uri Url
        {
            get
            {
                IHTMLLocation iHtmlLocation = NativeHtmlWindow.GetLocation();
                string stringLocation = (iHtmlLocation is null) ? "" : iHtmlLocation.GetHref();
                return string.IsNullOrEmpty(stringLocation) ? null : new Uri(stringLocation);
            }
        }

        public HtmlElement WindowFrameElement
        {
            get
            {
                return (((IHTMLWindow4)NativeHtmlWindow).frameElement() is IHTMLElement htmlElement) ? new HtmlElement(ShimManager, htmlElement) : null;
            }
        }

        public void Alert(string message)
        {
            NativeHtmlWindow.Alert(message);
        }

        public void AttachEventHandler(string eventName, EventHandler eventHandler)
        {
            WindowShim.AttachEventHandler(eventName, eventHandler);
        }

        public void Close()
        {
            NativeHtmlWindow.Close();
        }

        public bool Confirm(string message)
        {
            return NativeHtmlWindow.Confirm(message);
        }

        public void DetachEventHandler(string eventName, EventHandler eventHandler)
        {
            WindowShim.DetachEventHandler(eventName, eventHandler);
        }

        public void Focus()
        {
            NativeHtmlWindow.Focus();
        }

        /// <summary>
        ///  Moves the Window to the position requested
        /// </summary>
        public void MoveTo(int x, int y)
        {
            NativeHtmlWindow.MoveTo(x, y);
        }

        /// <summary>
        ///  Moves the Window to the point requested
        /// </summary>
        public void MoveTo(Point point)
        {
            NativeHtmlWindow.MoveTo(point.X, point.Y);
        }

        public void Navigate(Uri url)
        {
            NativeHtmlWindow.Navigate(url.ToString());
        }

        ///  Note: We intentionally have a string overload (apparently Mort wants one).  We don't have
        ///  string overloads call Uri overloads because that breaks Uris that aren't fully qualified
        ///  (things like "www.microsoft.com") that the underlying objects support and we don't want to
        ///  break.
        public void Navigate(string urlString)
        {
            NativeHtmlWindow.Navigate(urlString);
        }

        ///  Note: We intentionally have a string overload (apparently Mort wants one).  We don't have
        ///  string overloads call Uri overloads because that breaks Uris that aren't fully qualified
        ///  (things like "www.microsoft.com") that the underlying objects support and we don't want to
        ///  break.
        public HtmlWindow Open(string urlString, string target, string windowOptions, bool replaceEntry)
        {
            IHTMLWindow2 iHTMLWindow2 = NativeHtmlWindow.Open(urlString, target, windowOptions, replaceEntry);
            return (iHTMLWindow2 != null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
        }

        public HtmlWindow Open(Uri url, string target, string windowOptions, bool replaceEntry)
        {
            return Open(url.ToString(), target, windowOptions, replaceEntry);
        }

        ///  Note: We intentionally have a string overload (apparently Mort wants one).  We don't have
        ///  string overloads call Uri overloads because that breaks Uris that aren't fully qualified
        ///  (things like "www.microsoft.com") that the underlying objects support and we don't want to
        ///  break.
        public HtmlWindow OpenNew(string urlString, string windowOptions)
        {
            IHTMLWindow2 iHTMLWindow2 = NativeHtmlWindow.Open(urlString, "_blank", windowOptions, true);
            return (iHTMLWindow2 != null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
        }

        public HtmlWindow OpenNew(Uri url, string windowOptions)
        {
            return OpenNew(url.ToString(), windowOptions);
        }

        public string Prompt(string message, string defaultInputValue)
        {
            return NativeHtmlWindow.Prompt(message, defaultInputValue).ToString();
        }

        public void RemoveFocus()
        {
            NativeHtmlWindow.Blur();
        }

        /// <summary>
        ///  Resize the window to the width/height requested
        /// </summary>
        public void ResizeTo(int width, int height)
        {
            NativeHtmlWindow.ResizeTo(width, height);
        }

        /// <summary>
        ///  Resize the window to the Size requested
        /// </summary>
        public void ResizeTo(Size size)
        {
            NativeHtmlWindow.ResizeTo(size.Width, size.Height);
        }

        /// <summary>
        ///  Scroll the window to the position requested
        /// </summary>
        public void ScrollTo(int x, int y)
        {
            NativeHtmlWindow.ScrollTo(x, y);
        }

        /// <summary>
        ///  Scroll the window to the point requested
        /// </summary>
        public void ScrollTo(Point point)
        {
            NativeHtmlWindow.ScrollTo(point.X, point.Y);
        }

        //
        // Events
        //

        public event HtmlElementErrorEventHandler Error
        {
            add => WindowShim.AddHandler(EventError, value);
            remove => WindowShim.RemoveHandler(EventError, value);
        }

        public event HtmlElementEventHandler GotFocus
        {
            add => WindowShim.AddHandler(EventGotFocus, value);
            remove => WindowShim.RemoveHandler(EventGotFocus, value);
        }

        public event HtmlElementEventHandler Load
        {
            add => WindowShim.AddHandler(EventLoad, value);
            remove => WindowShim.RemoveHandler(EventLoad, value);
        }

        public event HtmlElementEventHandler LostFocus
        {
            add => WindowShim.AddHandler(EventLostFocus, value);
            remove => WindowShim.RemoveHandler(EventLostFocus, value);
        }

        public event HtmlElementEventHandler Resize
        {
            add => WindowShim.AddHandler(EventResize, value);
            remove => WindowShim.RemoveHandler(EventResize, value);
        }

        public event HtmlElementEventHandler Scroll
        {
            add => WindowShim.AddHandler(EventScroll, value);
            remove => WindowShim.RemoveHandler(EventScroll, value);
        }

        public event HtmlElementEventHandler Unload
        {
            add => WindowShim.AddHandler(EventUnload, value);
            remove => WindowShim.RemoveHandler(EventUnload, value);
        }

        //
        // Private classes:
        //
        [ClassInterface(ClassInterfaceType.None)]
        private class HTMLWindowEvents2 : StandardOleMarshalObject, /*Enforce calling back on the same thread*/
            DHTMLWindowEvents2
        {
            private readonly HtmlWindow parent;

            public HTMLWindowEvents2(HtmlWindow htmlWindow)
            {
                parent = htmlWindow;
            }

            private void FireEvent(object key, EventArgs e)
            {
                if (parent != null)
                {
                    parent.WindowShim.FireEvent(key, e);
                }
            }

            public void onfocus(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventGotFocus, e);
            }

            public void onblur(IHTMLEventObj evtObj)
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

            public void onload(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventLoad, e);
            }

            public void onunload(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventUnload, e);
                if (parent != null)
                {
                    parent.WindowShim.OnWindowUnload();
                }
            }

            public void onscroll(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventScroll, e);
            }

            public void onresize(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                FireEvent(HtmlWindow.EventResize, e);
            }

            public bool onhelp(IHTMLEventObj evtObj)
            {
                HtmlElementEventArgs e = new HtmlElementEventArgs(parent.ShimManager, evtObj);
                return e.ReturnValue;
            }

            public void onbeforeunload(IHTMLEventObj evtObj) { }

            public void onbeforeprint(IHTMLEventObj evtObj) { }

            public void onafterprint(IHTMLEventObj evtObj) { }
        }

        /// <summary>
        ///  HtmlWindowShim - this is the glue between the DOM eventing mechanisms
        ///        and our CLR callbacks.
        ///
        ///  There are two kinds of events: HTMLWindowEvents2 and IHtmlWindow3.AttachHandler style
        ///  HTMLWindowEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
        ///              on an instance of HTMLWindowEvents2.  The HTMLWindowEvents2 class then fires the event.
        ///
        ///  IHTMLWindow3.AttachHandler: MSHML calls back on an HtmlToClrEventProxy that we've created, looking
        ///                       for a method named DISPID=0.  For each event that's subscribed, we create
        ///                       a new HtmlToClrEventProxy, detect the callback and fire the corresponding
        ///                       CLR event.
        /// </summary>
        internal class HtmlWindowShim : HtmlShim
        {
            private AxHost.ConnectionPointCookie cookie;
            private HtmlWindow htmlWindow;

            public HtmlWindowShim(HtmlWindow window)
            {
                htmlWindow = window;
            }

            public IHTMLWindow2 NativeHtmlWindow
            {
                get { return htmlWindow.NativeHtmlWindow; }
            }

            public override IHTMLWindow2 AssociatedWindow
            {
                get { return htmlWindow.NativeHtmlWindow; }
            }

            ///  Support IHtmlDocument3.AttachHandler
            public override void AttachEventHandler(string eventName, EventHandler eventHandler)
            {
                // IE likes to call back on an IDispatch of DISPID=0 when it has an event,
                // the HtmlToClrEventProxy helps us fake out the CLR so that we can call back on
                // our EventHandler properly.

                HtmlToClrEventProxy proxy = AddEventProxy(eventName, eventHandler);
                bool success = ((IHTMLWindow3)NativeHtmlWindow).AttachEvent(eventName, proxy);
                Debug.Assert(success, "failed to add event");
            }

            ///  Support HTMLWindowEvents2
            public override void ConnectToEvents()
            {
                if (cookie is null || !cookie.Connected)
                {
                    cookie = new AxHost.ConnectionPointCookie(NativeHtmlWindow,
                                                                              new HTMLWindowEvents2(htmlWindow),
                                                                              typeof(DHTMLWindowEvents2),
                                                                              /*throwException*/ false);
                    if (!cookie.Connected)
                    {
                        cookie = null;
                    }
                }
            }

            ///  Support IHTMLWindow3.DetachHandler
            public override void DetachEventHandler(string eventName, EventHandler eventHandler)
            {
                HtmlToClrEventProxy proxy = RemoveEventProxy(eventHandler);
                if (proxy != null)
                {
                    ((IHTMLWindow3)NativeHtmlWindow).DetachEvent(eventName, proxy);
                }
            }

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

        public static bool operator ==(HtmlWindow left, HtmlWindow right)
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

        public static bool operator !=(HtmlWindow left, HtmlWindow right)
        {
            return !(left == right);
        }

        public override int GetHashCode() => htmlWindow2?.GetHashCode() ?? 0;

        public override bool Equals(object obj)
        {
            return (this == (HtmlWindow)obj);
        }
        #endregion

    }
}
