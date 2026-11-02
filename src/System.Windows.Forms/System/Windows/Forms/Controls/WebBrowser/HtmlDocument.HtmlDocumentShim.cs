// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.Web.MsHtml;
namespace System.Windows.Forms;

public sealed unsafe partial class HtmlDocument
{
    /// <summary>
    ///  HtmlDocumentShim - this is the glue between the DOM eventing mechanisms
    ///  and our CLR callbacks.
    ///
    ///  There are two kinds of events: HTMLWindowEvents2 and IHtmlWindow3.AttachHandler style
    ///  HTMLDocumentEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
    ///  on our an instance of HTMLDocumentEvents2. The HTMLDocumentEvents2 class then fires the event.
    ///
    ///  IHTMLDocument3.AttachHandler: MSHTML calls back on an HtmlToClrEventProxy that we've created, looking
    ///  for a method named DISPID=0. For each event that's subscribed, we create
    ///  a new HtmlToClrEventProxy, detect the callback and fire the corresponding
    ///  CLR event.
    /// </summary>
    internal class HtmlDocumentShim : HtmlShim
    {
        private AgileComPointer<IHTMLWindow2>? _associatedWindow;
        private AxHost.ConnectionPointCookie? _cookie;
        private HtmlDocument _htmlDocument;

        internal HtmlDocumentShim(HtmlDocument htmlDocument)
        {
            _htmlDocument = htmlDocument;

            // Snap our associated window so we know when to disconnect.
            HtmlWindow? window = htmlDocument.Window;
            if (window is not null)
            {
                _associatedWindow = window.NativeHtmlWindow;
            }
        }

        public override IHTMLWindow2.Interface? AssociatedWindow => (IHTMLWindow2.Interface?)_associatedWindow?.GetManagedObject();

        public IHTMLDocument2.Interface NativeHtmlDocument2 => (IHTMLDocument2.Interface)_htmlDocument.NativeHtmlDocument2.GetManagedObject();

        internal HtmlDocument Document => _htmlDocument;

        /// Support IHtmlDocument3.AttachHandler
        protected override bool AttachEventProxy(HtmlToClrEventProxy proxy)
        {
            // IE likes to call back on an IDispatch of DISPID=0 when it has an event,
            // the HtmlToClrEventProxy helps us fake out the CLR so that we can call back on
            // our EventHandler properly.

            using var htmlDoc3 = _htmlDocument.GetHtmlDocument<IHTMLDocument3>();
            using BSTR name = new(proxy.EventName);
            using var dispatch = ComHelpers.GetComScope<IDispatch>(proxy);
            VARIANT_BOOL result = default;
            htmlDoc3.Value->attachEvent(name, dispatch, &result).ThrowOnFailure();
            if (IsDisposed && result)
            {
                // Retain this scoped interface for rollback if native attachment reenters disposal.
                htmlDoc3.Value->detachEvent(name, dispatch).ThrowOnFailure();
                ObjectDisposedException.ThrowIf(IsDisposed, this);
            }

            return result;
        }

        //
        // Connect to standard events
        //
        public override void ConnectToEvents()
        {
            if (_cookie is null || !_cookie.Connected)
            {
                AxHost.ConnectionPointCookie cookie = new(
                    NativeHtmlDocument2,
                    new HTMLDocumentEvents2(_htmlDocument),
                    typeof(Interop.Mshtml.DHTMLDocumentEvents2),
                    throwException: false);

                if (IsDisposed)
                {
                    cookie.Disconnect();
                    return;
                }

                _cookie = cookie.Connected ? cookie : null;
            }
        }

        /// Support IHtmlDocument3.DetachHandler
        protected override void DetachEventProxy(HtmlToClrEventProxy proxy)
        {
            using var htmlDoc3 = _htmlDocument.GetHtmlDocument<IHTMLDocument3>();
            using BSTR name = new(proxy.EventName);
            using var dispatch = ComHelpers.GetComScope<IDispatch>(proxy);
            htmlDoc3.Value->detachEvent(name, dispatch).ThrowOnFailure();
        }

        //
        // Disconnect from standard events
        //
        public override void DisconnectFromEvents()
        {
            AxHost.ConnectionPointCookie? cookie = _cookie;
            _cookie = null;
            cookie?.Disconnect();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            finally
            {
                if (disposing)
                {
                    HtmlDocument? document = _htmlDocument;
                    _htmlDocument = null!;
                    try
                    {
                        document?.NativeHtmlDocument2.Dispose();
                    }
                    finally
                    {
                        // This independent GIT registration keeps the old window alive even after unadvising.
                        DisposeHelper.NullAndDispose(ref _associatedWindow);
                    }
                }
            }
        }

        protected override object GetEventSender() => _htmlDocument;
    }
}
