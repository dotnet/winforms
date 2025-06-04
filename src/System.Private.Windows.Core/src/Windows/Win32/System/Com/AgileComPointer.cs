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
///   follow this pattern. <see cref="M:System.DisposeHelper.NullAndDispose``1(``0@)"/> facilitates doing this safely.
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
    private readonly uint _memoryPressure;

    /// <summary>
    ///  Creates an <see cref="AgileComPointer{TInterface}"/> for the given <paramref name="interface"/>.
    /// </summary>
    /// <param name="interface">The COM interface pointer.</param>
    /// <param name="takeOwnership">
    ///  Indicates whether to take ownership of the interface. If `<see langword="true"/>` this object will own the ref count.
    /// </param>
    /// <param name="memoryPressure">The amount of memory pressure to add.</param>
    /// <remarks>
    ///  <para>
    ///   <paramref name="memoryPressure"/> is used to help the GC know that this object is holding onto native memory.
    ///   This is most useful for objects that can be created in large quantities, particularly if they are not disposable.
    ///  </para>
    ///  <para>
    ///   Setting <paramref name="takeOwnership"/> to `<see langword="true"/>` will ensure that this object takes
    ///   responsibility for releasing the COM interface when it is no longer needed. This is done by calling
    ///   <see cref="IUnknown.Release"/> after the GIT adds a ref to the interface.
    ///  </para>
    /// </remarks>
    /// <devdoc>
    ///  Other options were explored for ensuring that pending finalizers are not out of control besides
    ///  <paramref name="memoryPressure"/>. Caching the interface pointers in a static collection was considered,
    ///  but this had no impact on the HtmlElement scenario, which would create a new COM object for every element
    ///  access. Tracking creation counts and forcing the Finalizer to run was also considered, but that takes too
    ///  much responsibility away from the GC- better to let it make the decision based on our additional pressure.
    /// </devdoc>
#if DEBUG
    public AgileComPointer(TInterface* @interface, bool takeOwnership, uint memoryPressure = 0, bool trackDisposal = true)
        : base(trackDisposal)
#else
    public AgileComPointer(TInterface* @interface, bool takeOwnership, uint memoryPressure = 0)
#endif
    {
        _memoryPressure = 0;

        try
        {
            _cookie = GlobalInterfaceTable.RegisterInterface(@interface);
            if (memoryPressure > 0)
            {
                _memoryPressure = memoryPressure;
                GC.AddMemoryPressure(_memoryPressure);
            }
        }
        catch
        {
            // No need to clean if we couldn't register the interface.
            GC.SuppressFinalize(this);
            throw;
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
    ///  Returns <see langword="true"/> if <paramref name="other"/> has the same pointer this
    ///  <see cref="AgileComPointer{TInterface}"/> was created from.
    /// </summary>
    public bool IsSameNativeObject(AgileComPointer<TInterface> other)
    {
        // There is a chance that this AgileComPointer or the other has a proxy registered to the GIT.
        // A proxy's value can differ depending on the thread. In order to determine identity between two COM pointers,
        // both must be registered in GIT (this is already done when initializing an AgileComPointer),
        // queried for their IUnknowns on the same thread, and then have their values compared.
        // If two proxies refer to the same native object, querying them for IUnknown
        // on the same thread will always give the same value.
        using var currentUnknown = GetInterface<IUnknown>();
        using var otherUnknown = other.GetInterface<IUnknown>();
        return currentUnknown.Value == otherUnknown.Value;
    }

    /// <inheritdoc cref="IsSameNativeObject(AgileComPointer{TInterface})"/>
    public bool IsSameNativeObject(TInterface* other)
    {
        using var currentUnknown = GetInterface<IUnknown>();
        using ComScope<IUnknown> otherUnknown = ComScope<IUnknown>.QueryFrom(other);
        return currentUnknown.Value == otherUnknown.Value;
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

    /// <summary>
    ///  Gets the managed object using the pointer
    ///  this <see cref="AgileComPointer{TInterface}"/> was created from.
    /// </summary>
    public object GetManagedObject()
    {
        using var scope = GetInterface();
        return ComHelpers.GetObjectForIUnknown(scope);
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
        // Clear the cookie before revoking the interface to guard against re-entry.

        uint cookie = Interlocked.Exchange(ref _cookie, 0);
        if (cookie == 0)
        {
            return;
        }

        if (_memoryPressure > 0)
        {
            GC.RemoveMemoryPressure(_memoryPressure);
        }

        HRESULT hr = GlobalInterfaceTable.RevokeInterface(cookie);

        if (disposing)
        {
            // Don't assert from the finalizer thread.
            Debug.Assert(hr.Succeeded);
        }
    }
}
