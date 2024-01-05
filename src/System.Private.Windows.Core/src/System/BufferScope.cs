// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.Runtime.InteropServices;

namespace System;

/// <summary>
///  Allows renting a buffer from <see cref="ArrayPool{T}"/> with a using statement. Can be used directly as if it
///  were a <see cref="Span{T}"/>.
/// </summary>
/// <remarks>
///  <para>
///   Buffers are not cleared and as such their initial contents will be random.
///  </para>
/// </remarks>
internal ref struct BufferScope<T>
{
    private T[]? _array;
    private Span<T> _span;

    public BufferScope(int minimumLength)
    {
        _array = ArrayPool<T>.Shared.Rent(minimumLength);
        _span = _array;
    }

    /// <summary>
    ///  Create the <see cref="BufferScope{T}"/> with an initial buffer. Useful for creating with an initial stack
    ///  allocated buffer.
    /// </summary>
    public BufferScope(Span<T> initialBuffer)
    {
        _array = null;
        _span = initialBuffer;
    }

    /// <summary>
    ///  Create the <see cref="BufferScope{T}"/> with an initial buffer. Useful for creating with an initial stack
    ///  allocated buffer.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   <example>
    ///    <para>Creating with a stack allocated buffer:</para>
    ///    <code>using BufferScope&lt;char> buffer = new(stackalloc char[64]);</code>
    ///   </example>
    ///  </para>
    ///  <para>
    ///   Stack allocated buffers should be kept small to avoid overflowing the stack.
    ///  </para>
    /// </remarks>
    /// <param name="minimumLength">
    ///  The required minimum length. If the <paramref name="initialBuffer"/> is not large enough, this will rent from
    ///  the shared <see cref="ArrayPool{T}"/>.
    /// </param>
    public BufferScope(Span<T> initialBuffer, int minimumLength)
    {
        if (initialBuffer.Length >= minimumLength)
        {
            _array = null;
            _span = initialBuffer;
        }
        else
        {
            _array = ArrayPool<T>.Shared.Rent(minimumLength);
            _span = _array;
        }
    }

    /// <summary>
    ///  Ensure that the buffer has enough space for <paramref name="capacity"/> number of elements.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Consider if creating new <see cref="BufferScope{T}"/> instances is possible and cleaner than using
    ///   this method.
    ///  </para>
    /// </remarks>
    /// <param name="copy">True to copy the existing elements when new space is allocated.</param>
    public unsafe void EnsureCapacity(int capacity, bool copy = false)
    {
        if (_span!.Length >= capacity)
        {
            return;
        }

        T[] newArray = ArrayPool<T>.Shared.Rent(capacity);
        if (copy)
        {
            _span.CopyTo(newArray);
        }

        if (_array is not null)
        {
            ArrayPool<T>.Shared.Return(_array);
        }

        _array = newArray;
        _span = _array;
    }

    public ref T this[int i] => ref _span[i];

    public readonly Span<T> this[Range range] => _span[range];

    public readonly Span<T> Slice(int start, int length) => _span.Slice(start, length);

    public readonly ref T GetPinnableReference() => ref MemoryMarshal.GetReference(_span);

    public readonly int Length => _span.Length;

    public readonly Span<T> AsSpan() => _span;

    public static implicit operator Span<T>(BufferScope<T> scope) => scope._span;

    public static implicit operator ReadOnlySpan<T>(BufferScope<T> scope) => scope._span;

    public void Dispose()
    {
        if (_array is not null)
        {
            ArrayPool<T>.Shared.Return(_array);
        }

        _array = default;
    }

    public override readonly string ToString() => _span.ToString();
}
