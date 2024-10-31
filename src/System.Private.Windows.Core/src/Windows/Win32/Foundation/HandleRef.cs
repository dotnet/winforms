// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.Win32.Foundation;

/// <summary>
///  Adapter to use when owning classes cannot directly implement <see cref="IHandle{T}"/>.
/// </summary>
/// <remarks>
///  <para>
///   Whenever you need to keep the owning object from being finalized during interop calls, use
///   <see cref="GC.KeepAlive(object?)"/> on the <see cref="Wrapper"/> <see langword="object"/>.
///  </para>
///  <para>
///   This is the typed equivalent of <see cref="HandleRef"/>. Marshalling doesn't know this, and while one
///   could write a custom marshaler, we want our imports to do no marshalling for performance and trimming reasons.
///  </para>
/// </remarks>
internal readonly struct HandleRef<THandle> : IHandle<THandle>, IEquatable<HandleRef<THandle>>
    where THandle : unmanaged, IEquatable<THandle>
{
    public required object? Wrapper { get; init; }
    public required THandle Handle { get; init; }

    [SetsRequiredMembers]
    public HandleRef(object? wrapper, THandle handle)
    {
        Wrapper = wrapper;
        Handle = handle;
    }

    [SetsRequiredMembers]
    public HandleRef(IHandle<THandle>? handle)
    {
        Wrapper = handle;
        Handle = handle?.Handle ?? default;
    }

    public bool Equals(HandleRef<THandle> other)
        => other.Handle.Equals(Handle) && Equals(other.Wrapper, Wrapper);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is THandle other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Wrapper, Handle);

    public static bool operator ==(HandleRef<THandle> left, HandleRef<THandle> right) => left.Equals(right);
    public static bool operator !=(HandleRef<THandle> left, HandleRef<THandle> right) => !(left == right);

    public static unsafe explicit operator HandleRef(HandleRef<THandle> handle)
    {
        if (sizeof(nint) != sizeof(THandle))
        {
            throw new InvalidCastException();
        }

        THandle local = handle.Handle;
        return new(handle.Wrapper, Unsafe.As<THandle, nint>(ref local));
    }

    public static unsafe explicit operator HandleRef<THandle>(HandleRef handle)
    {
        if (sizeof(nint) != sizeof(THandle))
        {
            throw new InvalidCastException();
        }

        nint local = handle.Handle;
        return new(handle.Wrapper, Unsafe.As<nint, THandle>(ref local));
    }

    public bool IsNull => Handle.Equals(default);
}
