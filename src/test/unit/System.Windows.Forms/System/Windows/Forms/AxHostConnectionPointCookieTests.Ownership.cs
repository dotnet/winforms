// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Checks caller-owned sink references separately from the connection point's subscription reference.
/// </summary>
public partial class AxHostConnectionPointCookieTests
{
    [StaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public unsafe void ConnectionPointCookie_Ownership_BalancesTemporarySink(bool failAdvise)
    {
        using OwnershipConnectionPoint source = new() { FailAdvise = failAdvise };
        OwnershipNotifySink sink = new();
        using var nativeSink = ComHelpers.GetComScope<IPropertyNotifySink>(sink);
        using var observer = nativeSink.Query<IUnknown>();
        uint before = GetSinkReferenceCount(observer.Value);
        AxHost.ConnectionPointCookie cookie = new(source, sink, typeof(IPropertyNotifySink.Interface), throwException: false);
        try
        {
            Assert.Equal(!failAdvise, cookie.Connected);
            Assert.Equal(failAdvise ? before : before + 1, GetSinkReferenceCount(observer.Value));
            if (!failAdvise)
            {
                source.Notify();
                Assert.Equal(1, sink.Calls);
            }
        }
        finally
        {
            cookie.Disconnect();
        }

        Assert.Equal(before, GetSinkReferenceCount(observer.Value));
        Assert.Equal(HRESULT.S_OK, nativeSink.Value->OnChanged(0));
        Assert.Equal(failAdvise ? 1 : 2, sink.Calls);
        cookie.Disconnect();
        Assert.Equal(before, GetSinkReferenceCount(observer.Value));
    }

    private static unsafe uint GetSinkReferenceCount(IUnknown* unknown)
    {
        unknown->AddRef();
        return unknown->Release();
    }

    /// <summary>
    ///  Provides a controlled subscription owner, including failure without retaining a sink.
    /// </summary>
    private sealed unsafe class OwnershipConnectionPoint :
        IConnectionPoint.Interface,
        IConnectionPointContainer.Interface,
        IManagedWrapper<IConnectionPoint, IConnectionPointContainer>,
        IDisposable
    {
        private AgileComPointer<IUnknown>? _sink;
        public bool FailAdvise { get; init; }

        public HRESULT GetConnectionInterface(Guid* iid)
        {
            *iid = IID.GetRef<IPropertyNotifySink>();
            return HRESULT.S_OK;
        }

        public HRESULT GetConnectionPointContainer(IConnectionPointContainer** container)
        {
            *container = ComHelpers.GetComPointer<IConnectionPointContainer>(this);
            return HRESULT.S_OK;
        }

        public HRESULT FindConnectionPoint(Guid* iid, IConnectionPoint** connectionPoint)
        {
            *connectionPoint = ComHelpers.GetComPointer<IConnectionPoint>(this);
            return HRESULT.S_OK;
        }

        public HRESULT Advise(IUnknown* sink, uint* cookie)
        {
            *cookie = 0;
            if (FailAdvise)
            {
                return HRESULT.E_FAIL;
            }

            _sink = new AgileComPointer<IUnknown>(sink, takeOwnership: false);
            *cookie = 1;
            return HRESULT.S_OK;
        }

        public HRESULT Unadvise(uint cookie)
        {
            DisposeHelper.NullAndDispose(ref _sink);
            return HRESULT.S_OK;
        }

        public HRESULT EnumConnections(IEnumConnections** connections)
        {
            *connections = null;
            return HRESULT.E_NOTIMPL;
        }

        public HRESULT EnumConnectionPoints(IEnumConnectionPoints** connectionPoints)
        {
            *connectionPoints = null;
            return HRESULT.E_NOTIMPL;
        }

        public void Notify()
        {
            using var sink = _sink!.GetInterface<IPropertyNotifySink>();
            sink.Value->OnChanged(0).ThrowOnFailure();
        }

        public void Dispose() => DisposeHelper.NullAndDispose(ref _sink);
    }

    /// <summary>
    ///  Exposes a managed sink whose native reference count is not affected by RCW caching.
    /// </summary>
    private sealed class OwnershipNotifySink : IPropertyNotifySink.Interface, IManagedWrapper<IPropertyNotifySink>
    {
        public int Calls { get; private set; }

        public HRESULT OnChanged(int dispID)
        {
            Calls++;
            return HRESULT.S_OK;
        }

        public HRESULT OnRequestEdit(int dispID) => HRESULT.S_OK;
    }
}
