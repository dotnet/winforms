// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.Web.MsHtml;
using static Interop.Mshtml;

namespace System.Windows.Forms;

public sealed partial class HtmlWindow
{
    /// <summary>
    ///  HtmlWindowShim - this is the glue between the DOM eventing mechanisms
    ///  and our CLR callbacks.
    ///
    ///  There are two kinds of events: HTMLWindowEvents2 and IHtmlWindow3.AttachHandler style
    ///  HTMLWindowEvents2: we create an IConnectionPoint (via ConnectionPointCookie) between us and MSHTML and it calls back
    ///  on an instance of HTMLWindowEvents2. The HTMLWindowEvents2 class then fires the event.
    ///
    ///  IHTMLWindow3.AttachHandler: MSHTML calls back on an HtmlToClrEventProxy that we've created, looking
    ///  for a method named DISPID=0. For each event that's subscribed, we create
    ///  a new HtmlToClrEventProxy, detect the callback and fire the corresponding
    ///  CLR event.
    /// </summary>
    internal unsafe class HtmlWindowShim : HtmlShim
    {
        private AxHost.ConnectionPointCookie? _cookie;
        private HtmlWindow _htmlWindow;
        private bool _observeWindowUnload;

        public HtmlWindowShim(HtmlWindow window)
        {
            _htmlWindow = window;
        }

        public override IHTMLWindow2.Interface AssociatedWindow => NativeHtmlWindow;

        public IHTMLWindow2.Interface NativeHtmlWindow => (IHTMLWindow2.Interface)_htmlWindow.NativeHtmlWindow.GetManagedObject();

        /// Support IHtmlDocument3.AttachHandler
        protected override bool AttachEventProxy(HtmlToClrEventProxy proxy)
        {
            // IE likes to call back on an IDispatch of DISPID=0 when it has an event,
            // the HtmlToClrEventProxy helps us fake out the CLR so that we can call back on
            // our EventHandler properly.

            using var htmlWindow3 = _htmlWindow.GetHtmlWindow<IHTMLWindow3>();
            using BSTR name = new(proxy.EventName);
            using var dispatch = ComHelpers.GetComScope<IDispatch>(proxy);
            VARIANT_BOOL result;
            htmlWindow3.Value->attachEvent(name, dispatch, &result).ThrowOnFailure();
            if (IsDisposed && result)
            {
                htmlWindow3.Value->detachEvent(name, dispatch).ThrowOnFailure();
                ObjectDisposedException.ThrowIf(IsDisposed, this);
            }

            return result;
        }

        internal void EnsureWindowObservation()
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            // The manager needs unload even when the application subscribes only to document/element
            // events. Otherwise its dictionaries root old pages and delegates indefinitely.
            _observeWindowUnload = true;
            ConnectToEvents();
        }

        protected override void OnEventHandlerRemoved()
        {
            if (!_observeWindowUnload)
            {
                base.OnEventHandlerRemoved();
            }
        }

        /// Support HTMLWindowEvents2
        public override void ConnectToEvents()
        {
            if (_cookie is null || !_cookie.Connected)
            {
                AxHost.ConnectionPointCookie cookie = new(
                    NativeHtmlWindow,
                    new HTMLWindowEvents2(_htmlWindow),
                    typeof(DHTMLWindowEvents2),
                    throwException: false);
                if (IsDisposed)
                {
                    cookie.Disconnect();
                    return;
                }

                _cookie = cookie.Connected ? cookie : null;
                if (_observeWindowUnload && _cookie is null)
                {
                    // Not every window exposes this connection point. Preserve the existing
                    // nonthrowing subscription behavior; manager disposal still releases its shims.
                    Debug.WriteLine("HTML window unload observation is unavailable; cleanup is deferred to manager disposal.");
                }
            }
        }

        /// Support IHTMLWindow3.DetachHandler
        protected override void DetachEventProxy(HtmlToClrEventProxy proxy)
        {
            using var htmlWindow3 = _htmlWindow.GetHtmlWindow<IHTMLWindow3>();
            using BSTR name = new(proxy.EventName);
            using var dispatch = ComHelpers.GetComScope<IDispatch>(proxy);
            htmlWindow3.Value->detachEvent(name, dispatch).ThrowOnFailure();
        }

        public override void DisconnectFromEvents()
        {
            AxHost.ConnectionPointCookie? cookie = _cookie;
            _cookie = null;
            cookie?.Disconnect();
        }

        public void OnWindowUnload() => _htmlWindow?.ShimManager.OnWindowUnloaded(_htmlWindow);

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
                    HtmlWindow? window = _htmlWindow;
                    _htmlWindow = null!;
                    _observeWindowUnload = false;
                    window?.NativeHtmlWindow.Dispose();
                }
            }
        }

        protected override object GetEventSender() => _htmlWindow;
    }
}
