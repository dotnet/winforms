// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed partial class HtmlDocument
    {
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
    }
}
