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
    }
}
