// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

/// <summary>
///  Adapter to use when owning classes cannot directly implement <see cref="IHandle{T}"/>.
/// </summary>
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

    public bool IsNull => Handle.Equals(default);
}
