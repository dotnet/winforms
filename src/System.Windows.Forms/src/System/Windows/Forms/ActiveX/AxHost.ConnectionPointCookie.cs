// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    public unsafe class ConnectionPointCookie
    {
        private ConnectionHandle? _connectionHandle;

        /// <summary>
        ///  Creates a connection point to of the given interface type.
        ///  which will call on a managed code sink that implements that interface.
        /// </summary>
        public ConnectionPointCookie(object source, object sink, Type eventInterface)
            : this(source, sink, eventInterface, true)
        {
        }

        internal ConnectionPointCookie(object? source, object sink, Type eventInterface, bool throwException)
        {
            if (source is not IConnectionPointContainer.Interface cpc)
            {
                if (throwException)
                {
                    throw new InvalidCastException(SR.AXNoConnectionPointContainer);
                }

                return;
            }

            if (sink is null || !eventInterface.IsInstanceOfType(sink))
            {
                if (throwException)
                {
                    throw new InvalidCastException(string.Format(SR.AXNoSinkImplementation, eventInterface.Name));
                }

                return;
            }

            IConnectionPoint* connectionPoint = null;
            try
            {
                Guid riid = eventInterface.GUID;
                HRESULT hr = cpc.FindConnectionPoint(&riid, &connectionPoint);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }

            if (connectionPoint is null)
            {
                if (throwException)
                {
                    throw new ArgumentException(string.Format(SR.AXNoEventInterface, eventInterface.Name));
                }

                return;
            }

            _connectionHandle = new(connectionPoint, sink);

            if (!_connectionHandle.Connected)
            {
                _connectionHandle.Dispose();
                _connectionHandle = null;

                if (throwException)
                {
                    throw new ArgumentException(string.Format(SR.AXNoConnectionPoint, eventInterface.Name));
                }
            }
        }

        /// <summary>
        ///  Disconnect the current connection point. If the object is not connected this method will do nothing.
        /// </summary>
        public void Disconnect()
        {
            GC.SuppressFinalize(this);
            _connectionHandle?.Dispose();
            _connectionHandle = null;
        }

        // Existing API
        ~ConnectionPointCookie() { }

        internal bool Connected => _connectionHandle is not null && _connectionHandle.Connected;

        private sealed class ConnectionHandle : AgileComPointer<IConnectionPoint>
        {
            private readonly uint _cookie;
            public bool Connected { get; private set; }

            public ConnectionHandle(IConnectionPoint* connectionPoint, object sink)
                : base(connectionPoint, takeOwnership: true)
            {
                uint cookie = 0;
                IUnknown* ccw = ComHelpers.TryGetComPointer<IUnknown>(sink, out HRESULT hr);
                if (hr.Failed || connectionPoint->Advise(ccw, &cookie).Failed)
                {
                    Dispose();
                }
                else
                {
                    Connected = true;
                }

                _cookie = cookie;
            }

            protected override void Dispose(bool disposing)
            {
                if (Connected)
                {
                    using var connectionPoint = TryGetInterface(out HRESULT hr);
                    if (hr.Succeeded)
                    {
                        hr = connectionPoint.Value->Unadvise(_cookie);
                    }
                }

                Connected = false;

                base.Dispose(disposing);
            }
        }
    }
}
