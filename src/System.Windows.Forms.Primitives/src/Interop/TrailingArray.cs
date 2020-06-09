// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

/// <summary>
/// Used for trailing native unsized (ANYSIZE) arrays of <typeparamref name="T"/>. Native example:
/// UCHAR  UniqueId[1];
/// </summary>
/// <remarks>
/// Accessing the values is only safe when you have a pointer to the containing struct in
/// a buffer. If you have an actual struct (Foo, not Foo*), the trailing array will have been
/// truncated as the values aren't actually part of the struct.
/// </remarks>
internal readonly struct TrailingArray<T> where T : unmanaged
{
    private readonly T _firstItem;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static ReadOnlySpan<T> GetBuffer(ref T first, uint count, uint offset = 0)
        => Unsafe.As<T, TrailingArray<T>>(ref first).GetBuffer(count, offset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static ReadOnlySpan<T> GetBufferInBytes(ref T first, uint countInBytes, uint offsetInBytes = 0)
        => Unsafe.As<T, TrailingArray<T>>(ref first).GetBuffer(countInBytes / (uint)sizeof(T), offsetInBytes / (uint)sizeof(T));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe ReadOnlySpan<T> GetBuffer(uint count, uint offset = 0)
    {
        if (count == 0)
        {
            return new ReadOnlySpan<T>();
        }

        fixed (T* t = &_firstItem)
        {
            return new ReadOnlySpan<T>(t + offset, (int)(count));
        }
    }
}
