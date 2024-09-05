// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms;

public sealed partial class HtmlElement
{
    /// <summary>
    ///  HtmlElementShim - this is the glue between the DOM eventing mechanisms
    ///          and our CLR callbacks.
    ///
    ///  HTMLElementEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
    ///              on our an instance of HTMLElementEvents2. The HTMLElementEvents2 class then fires the event.
    ///
    /// </summary>
    internal unsafe class HtmlElementShim : HtmlShim
    {
        private static readonly Type[] s_dispInterfaceTypes =
        [
            typeof(Interop.Mshtml.DHTMLElementEvents2),
            typeof(Interop.Mshtml.DHTMLAnchorEvents2),
            typeof(Interop.Mshtml.DHTMLAreaEvents2),
            typeof(Interop.Mshtml.DHTMLButtonElementEvents2),
            typeof(Interop.Mshtml.DHTMLControlElementEvents2),
            typeof(Interop.Mshtml.DHTMLFormElementEvents2),
            typeof(Interop.Mshtml.DHTMLFrameSiteEvents2),
            typeof(Interop.Mshtml.DHTMLImgEvents2),
            typeof(Interop.Mshtml.DHTMLInputFileElementEvents2),
            typeof(Interop.Mshtml.DHTMLInputImageEvents2),
            typeof(Interop.Mshtml.DHTMLInputTextElementEvents2),
            typeof(Interop.Mshtml.DHTMLLabelEvents2),
            typeof(Interop.Mshtml.DHTMLLinkElementEvents2),
            typeof(Interop.Mshtml.DHTMLMapEvents2),
            typeof(Interop.Mshtml.DHTMLMarqueeElementEvents2),
            typeof(Interop.Mshtml.DHTMLOptionButtonElementEvents2),
            typeof(Interop.Mshtml.DHTMLSelectElementEvents2),
            typeof(Interop.Mshtml.DHTMLStyleElementEvents2),
            typeof(Interop.Mshtml.DHTMLTableEvents2),
            typeof(Interop.Mshtml.DHTMLTextContainerEvents2),
            typeof(Interop.Mshtml.DHTMLScriptEvents2)
        ];

        private readonly AgileComPointer<IHTMLWindow2>? _associatedWindow;
        private AxHost.ConnectionPointCookie? _cookie;   // To hook up events from the native HtmlElement
        private HtmlElement _htmlElement;

        public HtmlElementShim(HtmlElement element)
        {
            _htmlElement = element;

            // Snap our associated window so we know when to disconnect.
            HtmlDocument? doc = _htmlElement.Document;
            if (doc is not null)
            {
                HtmlWindow? window = doc.Window;
                if (window is not null)
                {
                    _associatedWindow = window.NativeHtmlWindow;
                }
            }
        }

        public override IHTMLWindow2.Interface? AssociatedWindow => (IHTMLWindow2.Interface?)_associatedWindow?.GetManagedObject();

        public IHTMLElement.Interface NativeHtmlElement => (IHTMLElement.Interface)_htmlElement.NativeHtmlElement.GetManagedObject();

        internal HtmlElement Element => _htmlElement;

        ///  Support IHTMLElement2.AttachEventHandler
        public override void AttachEventHandler(string eventName, EventHandler eventHandler)
        {
            // IE likes to call back on an IDispatch of DISPID=0 when it has an event,
            // the HtmlToClrEventProxy helps us fake out the CLR so that we can call back on
            // our EventHandler properly.

            HtmlToClrEventProxy proxy = AddEventProxy(eventName, eventHandler);
            using var htmlElement2 = _htmlElement.GetHtmlElement<IHTMLElement2>();
            using BSTR name = new(eventName);
            using var dispatch = ComHelpers.GetComScope<IDispatch>(proxy);
            VARIANT_BOOL result;
            htmlElement2.Value->attachEvent(name, dispatch, &result).ThrowOnFailure();
        }

        public override void ConnectToEvents()
        {
            if (_cookie is null || !_cookie.Connected)
            {
                for (int i = 0; i < s_dispInterfaceTypes.Length && _cookie is null; i++)
                {
                    _cookie = new AxHost.ConnectionPointCookie(
                        NativeHtmlElement,
                        new HTMLElementEvents2(_htmlElement),
                        s_dispInterfaceTypes[i],
                        throwException: false);
                    if (!_cookie.Connected)
                    {
                        _cookie = null;
                    }
                }
            }
        }

        ///  Support IHTMLElement2.DetachHandler
        public override void DetachEventHandler(string eventName, EventHandler eventHandler)
        {
            HtmlToClrEventProxy? proxy = RemoveEventProxy(eventHandler);
            if (proxy is not null)
            {
                using var htmlElement2 = _htmlElement.GetHtmlElement<IHTMLElement2>();
                using BSTR name = new(eventName);
                using var dispatch = ComHelpers.GetComScope<IDispatch>(proxy);
                htmlElement2.Value->detachEvent(name, dispatch).ThrowOnFailure();
            }
        }

        public override void DisconnectFromEvents()
        {
            if (_cookie is not null)
            {
                _cookie.Disconnect();
                _cookie = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _htmlElement?.NativeHtmlElement?.Dispose();
                _htmlElement = null!;
            }
        }

        protected override object GetEventSender() => _htmlElement;
    }
}
