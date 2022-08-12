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
    [Obsolete(
       Obsoletions.WebBrowserMessage,
       error: false,
       DiagnosticId = Obsoletions.WebBrowserDiagnosticId,
       UrlFormat = Obsoletions.SharedUrlFormat)]
    public sealed partial class HtmlWindow
    {
        internal static readonly object s_eventError = new();
        internal static readonly object s_eventGotFocus = new();
        internal static readonly object s_eventLoad = new();
        internal static readonly object s_eventLostFocus = new();
        internal static readonly object s_eventResize = new();
        internal static readonly object s_eventScroll = new();
        internal static readonly object s_eventUnload = new();

        private readonly HtmlShimManager _shimManager;
        private readonly IHTMLWindow2 _htmlWindow2;

        internal HtmlWindow(HtmlShimManager shimManager, IHTMLWindow2 win)
        {
            _htmlWindow2 = win;
            Debug.Assert(NativeHtmlWindow is not null, "The window object should implement IHTMLWindow2");

            _shimManager = shimManager;
        }

        internal IHTMLWindow2 NativeHtmlWindow
        {
            get
            {
                return _htmlWindow2;
            }
        }

        private HtmlShimManager ShimManager
        {
            get { return _shimManager; }
        }

        private HtmlWindowShim WindowShim
        {
            get
            {
                if (ShimManager is not null)
                {
                    HtmlWindowShim shim = ShimManager.GetWindowShim(this);
                    if (shim is null)
                    {
                        _shimManager.AddWindowShim(this);
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
                return (iHTMLFramesCollection2 is not null) ? new HtmlWindowCollection(ShimManager, iHTMLFramesCollection2) : null;
            }
        }

        public HtmlHistory History
        {
            get
            {
                IOmHistory iOmHistory = NativeHtmlWindow.GetHistory();
                return iOmHistory is not null ? new HtmlHistory(iOmHistory) : null;
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
                return (iHTMLWindow2 is not null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
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
            return (iHTMLWindow2 is not null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
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
            return (iHTMLWindow2 is not null) ? new HtmlWindow(ShimManager, iHTMLWindow2) : null;
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
            add => WindowShim.AddHandler(s_eventError, value);
            remove => WindowShim.RemoveHandler(s_eventError, value);
        }

        public event HtmlElementEventHandler GotFocus
        {
            add => WindowShim.AddHandler(s_eventGotFocus, value);
            remove => WindowShim.RemoveHandler(s_eventGotFocus, value);
        }

        public event HtmlElementEventHandler Load
        {
            add => WindowShim.AddHandler(s_eventLoad, value);
            remove => WindowShim.RemoveHandler(s_eventLoad, value);
        }

        public event HtmlElementEventHandler LostFocus
        {
            add => WindowShim.AddHandler(s_eventLostFocus, value);
            remove => WindowShim.RemoveHandler(s_eventLostFocus, value);
        }

        public event HtmlElementEventHandler Resize
        {
            add => WindowShim.AddHandler(s_eventResize, value);
            remove => WindowShim.RemoveHandler(s_eventResize, value);
        }

        public event HtmlElementEventHandler Scroll
        {
            add => WindowShim.AddHandler(s_eventScroll, value);
            remove => WindowShim.RemoveHandler(s_eventScroll, value);
        }

        public event HtmlElementEventHandler Unload
        {
            add => WindowShim.AddHandler(s_eventUnload, value);
            remove => WindowShim.RemoveHandler(s_eventUnload, value);
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

        public override int GetHashCode() => _htmlWindow2?.GetHashCode() ?? 0;

        public override bool Equals(object obj)
        {
            return (this == (HtmlWindow)obj);
        }
        #endregion

    }
}
