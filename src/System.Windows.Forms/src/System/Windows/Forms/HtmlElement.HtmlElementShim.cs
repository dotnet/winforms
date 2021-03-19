// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop.Mshtml;

namespace System.Windows.Forms
{
    public sealed partial class HtmlElement
    {
        /// <summary>
        ///  HtmlElementShim - this is the glue between the DOM eventing mechanisms
        ///          and our CLR callbacks.
        ///
        ///  HTMLElementEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
        ///              on our an instance of HTMLElementEvents2.  The HTMLElementEvents2 class then fires the event.
        ///
        /// </summary>
        internal class HtmlElementShim : HtmlShim
        {
            private static readonly Type[] dispInterfaceTypes =
            {
                typeof(DHTMLElementEvents2),
                typeof(DHTMLAnchorEvents2),
                typeof(DHTMLAreaEvents2),
                typeof(DHTMLButtonElementEvents2),
                typeof(DHTMLControlElementEvents2),
                typeof(DHTMLFormElementEvents2),
                typeof(DHTMLFrameSiteEvents2),
                typeof(DHTMLImgEvents2),
                typeof(DHTMLInputFileElementEvents2),
                typeof(DHTMLInputImageEvents2),
                typeof(DHTMLInputTextElementEvents2),
                typeof(DHTMLLabelEvents2),
                typeof(DHTMLLinkElementEvents2),
                typeof(DHTMLMapEvents2),
                typeof(DHTMLMarqueeElementEvents2),
                typeof(DHTMLOptionButtonElementEvents2),
                typeof(DHTMLSelectElementEvents2),
                typeof(DHTMLStyleElementEvents2),
                typeof(DHTMLTableEvents2),
                typeof(DHTMLTextContainerEvents2),
                typeof(DHTMLScriptEvents2)
            };

            private AxHost.ConnectionPointCookie cookie;   // To hook up events from the native HtmlElement
            private HtmlElement htmlElement;
            private readonly IHTMLWindow2 associatedWindow;

            public HtmlElementShim(HtmlElement element)
            {
                htmlElement = element;

                // snap our associated window so we know when to disconnect.
                if (htmlElement != null)
                {
                    HtmlDocument doc = htmlElement.Document;
                    if (doc != null)
                    {
                        HtmlWindow window = doc.Window;
                        if (window != null)
                        {
                            associatedWindow = window.NativeHtmlWindow;
                        }
                    }
                }
            }

            public IHTMLElement NativeHtmlElement
            {
                get { return htmlElement.NativeHtmlElement; }
            }

            internal HtmlElement Element
            {
                get { return htmlElement; }
            }

            public override IHTMLWindow2 AssociatedWindow
            {
                get { return associatedWindow; }
            }

            ///  Support IHTMLElement2.AttachEventHandler
            public override void AttachEventHandler(string eventName, EventHandler eventHandler)
            {
                // IE likes to call back on an IDispatch of DISPID=0 when it has an event,
                // the HtmlToClrEventProxy helps us fake out the CLR so that we can call back on
                // our EventHandler properly.

                HtmlToClrEventProxy proxy = AddEventProxy(eventName, eventHandler);
                ((IHTMLElement2)NativeHtmlElement).AttachEvent(eventName, proxy);
            }

            public override void ConnectToEvents()
            {
                if (cookie is null || !cookie.Connected)
                {
                    for (int i = 0; i < dispInterfaceTypes.Length && cookie is null; i++)
                    {
                        cookie = new AxHost.ConnectionPointCookie(NativeHtmlElement,
                                                                                  new HTMLElementEvents2(htmlElement),
                                                                                  dispInterfaceTypes[i],
                                                                                  /*throwException*/ false);
                        if (!cookie.Connected)
                        {
                            cookie = null;
                        }
                    }
                }
            }

            ///  Support IHTMLElement2.DetachHandler
            public override void DetachEventHandler(string eventName, EventHandler eventHandler)
            {
                HtmlToClrEventProxy proxy = RemoveEventProxy(eventHandler);
                if (proxy != null)
                {
                    ((IHTMLElement2)NativeHtmlElement).DetachEvent(eventName, proxy);
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
                if (htmlElement != null)
                {
                    Marshal.FinalReleaseComObject(htmlElement.NativeHtmlElement);
                }

                htmlElement = null;
            }

            protected override object GetEventSender()
            {
                return htmlElement;
            }
        }
    }
}
