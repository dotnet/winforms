// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed partial class HtmlWindow
    {
        /// <summary>
        ///  HtmlWindowShim - this is the glue between the DOM eventing mechanisms
        ///        and our CLR callbacks.
        ///
        ///  There are two kinds of events: HTMLWindowEvents2 and IHtmlWindow3.AttachHandler style
        ///  HTMLWindowEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
        ///              on an instance of HTMLWindowEvents2.  The HTMLWindowEvents2 class then fires the event.
        ///
        ///  IHTMLWindow3.AttachHandler: MSHTML calls back on an HtmlToClrEventProxy that we've created, looking
        ///                       for a method named DISPID=0.  For each event that's subscribed, we create
        ///                       a new HtmlToClrEventProxy, detect the callback and fire the corresponding
        ///                       CLR event.
        /// </summary>
        internal class HtmlWindowShim : HtmlShim
        {
            private AxHost.ConnectionPointCookie _cookie;
            private HtmlWindow _htmlWindow;

            public HtmlWindowShim(HtmlWindow window)
            {
                _htmlWindow = window;
            }

            public override IHTMLWindow2 AssociatedWindow
            {
                get { return _htmlWindow.NativeHtmlWindow; }
            }

            public IHTMLWindow2 NativeHtmlWindow
            {
                get { return _htmlWindow.NativeHtmlWindow; }
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
                if (_cookie is null || !_cookie.Connected)
                {
                    _cookie = new AxHost.ConnectionPointCookie(NativeHtmlWindow,
                                                                              new HTMLWindowEvents2(_htmlWindow),
                                                                              typeof(DHTMLWindowEvents2),
                                                                              /*throwException*/ false);
                    if (!_cookie.Connected)
                    {
                        _cookie = null;
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
                if (_cookie != null)
                {
                    _cookie.Disconnect();
                    _cookie = null;
                }
            }

            public void OnWindowUnload()
            {
                if (_htmlWindow != null)
                {
                    _htmlWindow.ShimManager.OnWindowUnloaded(_htmlWindow);
                }
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    if (_htmlWindow != null && _htmlWindow.NativeHtmlWindow != null)
                    {
                        Marshal.FinalReleaseComObject(_htmlWindow.NativeHtmlWindow);
                    }

                    _htmlWindow = null;
                }
            }

            protected override object GetEventSender()
            {
                return _htmlWindow;
            }
        }
    }
}
