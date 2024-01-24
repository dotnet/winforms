﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

/// <summary>
///  Fast stack based <see cref="ReadOnlySpan{T}"/> reader.
/// </summary>
/// <remarks>
///  <para>
///   Care must be used when reading struct values that depend on a specific field state for members to work
///   correctly. For example, <see cref="DateTime"/> has a very specific set of valid values for its packed
///   <see langword="ulong"/> field.
///  </para>
///  <para>
///   Inspired by <see cref="SequenceReader{T}"/> patterns.
///  </para>
/// </remarks>
internal unsafe ref struct SpanReader<T>(ReadOnlySpan<T> span) where T : unmanaged, IEquatable<T>
{
    private ReadOnlySpan<T> _unread = span;
    public ReadOnlySpan<T> Span { get; } = span;

    public int Position
    {
        readonly get => Span.Length - _unread.Length;
        set => _unread = Span[value..];
    }

    /// <summary>
    ///  Try to read everything up to the given <paramref name="delimiter"/>. Advances the reader past the
    ///  <paramref name="delimiter"/> if found.
    /// </summary>
    /// <inheritdoc cref="TryReadTo(T, bool, out ReadOnlySpan{T})"/>
    public bool TryReadTo(T delimiter, out ReadOnlySpan<T> span) =>
        TryReadTo(delimiter, advancePastDelimiter: true, out span);

    /// <summary>
    ///  Try to read everything up to the given <paramref name="delimiter"/>.
    /// </summary>
    /// <param name="span">The read data, if any.</param>
    /// <param name="delimiter">The delimiter to look for.</param>
    /// <param name="advancePastDelimiter"><see langword="true"/> to move past the <paramref name="delimiter"/> if found.</param>
    /// <returns><see langword="true"/> if the <paramref name="delimiter"/> was found.</returns>
    public bool TryReadTo(T delimiter, bool advancePastDelimiter, out ReadOnlySpan<T> span)
    {
        bool found = false;
        int index = _unread.IndexOf(delimiter);
        span = default;

        if (index != -1)
        {
            found = true;
            if (index > 0)
            {
                span = _unread;
                UncheckedSliceTo(ref span, index);
                if (advancePastDelimiter)
                {
                    index++;
                }

                UnsafeAdvance(index);
            }
        }

        return found;
    }

    /// <summary>
    ///  Try to read the next value.
    /// </summary>
    public bool TryRead(out T value)
    {
        bool success;

        if (_unread.IsEmpty)
        {
            value = default;
            success = false;
        }
        else
        {
            success = true;
            value = _unread[0];
            UnsafeAdvance(1);
        }

        return success;
    }

    /// <summary>
    ///  Try to read a span of the given <paramref name="count"/>.
    /// </summary>
    public bool TryRead(int count, out ReadOnlySpan<T> span)
    {
        bool success;

        if (count > _unread.Length)
        {
            span = default;
            success = false;
        }
        else
        {
            success = true;
            span = _unread[..count];
            UnsafeAdvance(count);
        }

        return success;
    }

    /// <summary>
    ///  Try to read a value of the given type. The size of the value must be evenly divisible by the size of
    ///  <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is just a straight copy of bits. If <typeparamref name="TValue"/> has methods that depend on
    ///   specific field value constraints this could be unsafe.
    ///  </para>
    ///  <para>
    ///   The compiler will often optimize away the struct copy if you only read from the value.
    ///  </para>
    /// </remarks>
    public bool TryRead<TValue>(out TValue value) where TValue : unmanaged
    {
        if (sizeof(TValue) < sizeof(T) || sizeof(TValue) % sizeof(T) != 0)
        {
            throw new ArgumentException($"The size of {nameof(TValue)} must be evenly divisible by the size of {nameof(T)}.");
        }

        bool success;

        if (sizeof(TValue) > _unread.Length * sizeof(T))
        {
            value = default;
            success = false;
        }
        else
        {
            success = true;
            value = Unsafe.ReadUnaligned<TValue>(ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(_unread)));
            UnsafeAdvance(sizeof(TValue) / sizeof(T));
        }

        return success;
    }

    /// <summary>
    ///  Try to read a span of values of the given type. The size of the value must be evenly divisible by the size of
    ///  <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This effectively does a <see cref="MemoryMarshal.Cast{TFrom, TTo}(ReadOnlySpan{TFrom})"/> and the same
    ///   caveats apply about safety.
    ///  </para>
    /// </remarks>
    public bool TryRead<TValue>(int count, out ReadOnlySpan<TValue> value) where TValue : unmanaged
    {
        if (sizeof(TValue) < sizeof(T) || sizeof(TValue) % sizeof(T) != 0)
        {
            throw new ArgumentException($"The size of {nameof(TValue)} must be evenly divisible by the size of {nameof(T)}.");
        }

        bool success;

        if (sizeof(TValue) * count > _unread.Length * sizeof(T))
        {
            value = default;
            success = false;
        }
        else
        {
            success = true;
            value = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, TValue>(ref MemoryMarshal.GetReference(_unread)), count);
            UnsafeAdvance((sizeof(TValue) / sizeof(T)) * count);
        }

        return success;
    }

    /// <summary>
    ///  Check to see if the given <paramref name="next"/> values are next.
    /// </summary>
    /// <param name="next">The span to compare the next items to.</param>
    public readonly bool IsNext(ReadOnlySpan<T> next) => _unread.StartsWith(next);

    /// <summary>
    ///  Advance the reader if the given <paramref name="next"/> values are next.
    /// </summary>
    /// <param name="next">The span to compare the next items to.</param>
    /// <returns><see langword="true"/> if the values were found and the reader advanced.</returns>
    public bool TryAdvancePast(ReadOnlySpan<T> next)
    {
        bool success = false;
        if (_unread.StartsWith(next))
        {
            UnsafeAdvance(next.Length);
            success = true;
        }

        return success;
    }

    /// <summary>
    ///  Advance the reader past consecutive instances of the given <paramref name="value"/>.
    /// </summary>
    /// <returns>How many positions the reader has been advanced</returns>
    public int AdvancePast(T value)
    {
        int count = 0;

        int index = _unread.IndexOfAnyExcept(value);
        if (index == -1)
        {
            // Everything left is the value
            count = _unread.Length;
            _unread = default;
        }
        else if (index != 0)
        {
            count = index;
            UnsafeAdvance(index);
        }

        return count;
    }

    /// <summary>
    ///  Advance the reader by the given <paramref name="count"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count) => _unread = _unread[count..];

    /// <summary>
    ///  Rewind the reader by the given <paramref name="count"/>.
    /// </summary>
    public void Rewind(int count) => _unread = Span[(Span.Length - _unread.Length - count)..];

    /// <summary>
    ///  Reset the reader to the beginning of the span.
    /// </summary>
    public void Reset() => _unread = Span;

    /// <summary>
    ///  Advance the reader without bounds checking.
    /// </summary>
    /// <param name="count"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UnsafeAdvance(int count)
    {
        Debug.Assert((uint)count <= (uint)_unread.Length);
        UncheckedSlice(ref _unread, count, _unread.Length - count);
    }

    /// <summary>
    ///  Slicing without bounds checking.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UncheckedSliceTo(ref ReadOnlySpan<T> span, int length)
    {
        Debug.Assert((uint)length <= (uint)span.Length);
        span = MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(span), length);
    }

    /// <summary>
    ///  Slicing without bounds checking.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UncheckedSlice(ref ReadOnlySpan<T> span, int start, int length)
    {
        Debug.Assert((uint)start <= (uint)span.Length && (uint)length <= (uint)(span.Length - start));
        span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(span), (nint)(uint)start), length);
    }
}
