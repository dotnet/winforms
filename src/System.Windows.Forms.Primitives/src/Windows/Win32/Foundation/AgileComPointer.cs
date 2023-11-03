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
///  <para>
///   Fields should be nulled out before calling <see cref="Dispose()"/>. Releasing the COM pointer during disposal
///   can result in callbacks to containing classes. Rather than evaluate the risk of this for every class, always
///   follow this pattern. <see cref="DisposeHelper.NullAndDispose"/> facilitates doing this safely.
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
    private readonly IUnknown* _originalObject;

#if DEBUG
    public AgileComPointer(TInterface* @interface, bool takeOwnership, bool trackDisposal = true)
        : base(trackDisposal)
#else
    public AgileComPointer(TInterface* @interface, bool takeOwnership)
#endif
    {
        try
        {
            _cookie = GlobalInterfaceTable.RegisterInterface(@interface);
            fixed (IUnknown** ppUnknown = &_originalObject)
            {
                ((IUnknown*)@interface)->QueryInterface(IID.Get<IUnknown>(), (void**)ppUnknown).ThrowOnFailure();
            }

            _originalObject->Release();
        }
        finally
        {
            if (takeOwnership)
            {
                // The GIT will add a ref to the given interface, release to effectively give ownership to the GIT.
                uint count = ((IUnknown*)@interface)->Release();
                Debug.Assert(count >= 0);
            }
        }
    }

    /// <summary>
    ///  Returns <see langword="true"/> if the given <paramref name="interface"/> is the same pointer this
    ///  <see cref="AgileComPointer{TInterface}"/> was created from.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is useful in avoiding recreating a new <see cref="AgileComPointer{TInterface}"/> for the same
    ///   object.
    ///  </para>
    /// </remarks>
    public bool IsSameNativeObject(TInterface* @interface)
    {
        using ComScope<IUnknown> unknownScope = new(null);
        ((IUnknown*)@interface)->QueryInterface(IID.Get<IUnknown>(), unknownScope);
        return _cookie != 0 && unknownScope.Value == _originalObject;
    }

    /// <summary>
    ///  Returns <see langword="true"/> if <paramref name="other"/> has the same pointer this
    ///  <see cref="AgileComPointer{TInterface}"/> was created from.
    /// </summary>
    public bool IsSameNativeObject(AgileComPointer<TInterface> other)
        => _originalObject == other._originalObject;

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

    /// <summary>
    ///  Gets the managed object using the pointer
    ///  this <see cref="AgileComPointer{TInterface}"/> was created from.
    /// </summary>
    public object GetManagedObject()
    {
        using var scope = GetInterface();
        return ComHelpers.GetObjectForIUnknown(scope.AsUnknown);
    }

    public override int GetHashCode() => HashCode.Combine((nint)_originalObject);

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
        // Clear the cookie before revoking the interface to guard against re-entry.

        uint cookie = Interlocked.Exchange(ref _cookie, 0);
        if (cookie == 0)
        {
            return;
        }

        HRESULT hr = GlobalInterfaceTable.RevokeInterface(cookie);

        if (disposing)
        {
            // Don't assert from the finalizer thread.
            Debug.Assert(hr.Succeeded);
        }
    }
}
