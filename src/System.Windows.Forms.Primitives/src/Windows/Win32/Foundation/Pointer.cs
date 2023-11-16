// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

/// <summary>
///  Simple wrapper for a pointer so it can be used as a type argument. This should only be used in very limited
///  scenarios, notably as a generic type argument.
/// </summary>
internal readonly unsafe struct Pointer<T> where T : unmanaged
{
    private Pointer(T* pointer) => Value = pointer;

    public T* Value { get; }

    public static implicit operator T*(in Pointer<T> pointer) => pointer.Value;
    public static implicit operator Pointer<T>(T* pointer) => new(pointer);
}
