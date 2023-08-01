// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace Windows.Win32.Foundation;

/// <summary>
///  Finalizable wrapper for COM pointers that gives agile access to the specified interface.
/// </summary>
/// <remarks>
///  <para>
///   This class should be used to hold all COM pointers that are stored as fields to ensure that they are
///   safely finalized when needed. Finalization should be avoided whenever possible for performance and timely
///   resource release (that is, this class should be disposed).
///  </para>
/// </remarks>
internal unsafe class AgileComPointer<TInterface> :
#if DEBUG
    DisposalTracking.Tracker,
#endif
    IDisposable
    where TInterface : unmanaged, IComIID
{
    private uint _cookie;

    public TInterface* OriginalHandle { get; }

#if DEBUG
    public AgileComPointer(TInterface* @interface, bool takeOwnership, bool trackDisposal = true)
        : base(trackDisposal)
#else
    public AgileComPointer(TInterface* @interface, bool takeOwnership)
#endif
    {
        _cookie = GlobalInterfaceTable.RegisterInterface(@interface);
        OriginalHandle = @interface;

        if (takeOwnership)
        {
            // The GIT will add a ref to the given interface, release to effectively give ownership to the GIT.
            uint count = ((IUnknown*)@interface)->Release();
            Debug.Assert(count >= 0);
        }
    }

    /// <summary>
    ///  Gets the default interface. Throws if failed.
    /// </summary>
    public ComScope<TInterface> GetInterface()
    {
        var scope = GlobalInterfaceTable.GetInterface<TInterface>(_cookie, out HRESULT hr);
        hr.ThrowOnFailure();
        return scope;
    }

    /// <summary>
    ///  Gets the specified interface. Throws if failed.
    /// </summary>
    public ComScope<TAsInterface> GetInterface<TAsInterface>()
        where TAsInterface : unmanaged, IComIID
    {
        var scope = TryGetInterface<TAsInterface>(out HRESULT hr);
        hr.ThrowOnFailure();
        return scope;
    }

    /// <summary>
    ///  Tries to get the default interface.
    /// </summary>
    public ComScope<TInterface> TryGetInterface(out HRESULT hr)
        => GlobalInterfaceTable.GetInterface<TInterface>(_cookie, out hr);

    /// <summary>
    ///  Tries to get the specified interface.
    /// </summary>
    public ComScope<TAsInterface> TryGetInterface<TAsInterface>(out HRESULT hr)
        where TAsInterface : unmanaged, IComIID
    {
        var scope = GlobalInterfaceTable.GetInterface<TAsInterface>(_cookie, out hr);
        return scope;
    }

    ~AgileComPointer()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_cookie == 0)
        {
            return;
        }

        HRESULT hr = GlobalInterfaceTable.RevokeInterface(_cookie);
        _cookie = 0;

        if (disposing)
        {
            // Don't assert from the finalizer thread.
            Debug.Assert(hr.Succeeded);
        }
    }
}
