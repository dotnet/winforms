// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

/// <summary>
///  Fast stack based <see cref="Span{T}"/> writer.
/// </summary>
internal unsafe ref struct SpanWriter<T>(Span<T> span) where T : unmanaged, IEquatable<T>
{
    private Span<T> _unwritten = span;
    public Span<T> Span { get; } = span;

    public int Position
    {
        readonly get => Span.Length - _unwritten.Length;
        set => _unwritten = Span[value..];
    }

    public readonly int Length => Span.Length;

    /// <summary>
    ///  Try to write the given value.
    /// </summary>
    public bool TryWrite(T value)
    {
        bool success = false;

        if (!_unwritten.IsEmpty)
        {
            success = true;
            _unwritten[0] = value;
            UnsafeAdvance(1);
        }

        return success;
    }

    /// <summary>
    ///  Try to write the given value.
    /// </summary>
    public bool TryWrite(params ReadOnlySpan<T> values)
    {
        bool success = false;

        if (_unwritten.Length >= values.Length)
        {
            success = true;
            values.CopyTo(_unwritten);
            UnsafeAdvance(values.Length);
        }

        return success;
    }

    /// <summary>
    ///  Try to write the given value <paramref name="count"/> times.
    /// </summary>
    public bool TryWriteCount(int count, T value)
    {
        bool success = false;

        if (_unwritten.Length >= count)
        {
            success = true;
            _unwritten[..count].Fill(value);
            UnsafeAdvance(count);
        }

        return success;
    }

    /// <summary>
    ///  Advance the writer by the given <paramref name="count"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count) => _unwritten = _unwritten[count..];

    /// <summary>
    ///  Rewind the writer by the given <paramref name="count"/>.
    /// </summary>
    public void Rewind(int count) => _unwritten = Span[(Span.Length - _unwritten.Length - count)..];

    /// <summary>
    ///  Reset the reader to the beginning of the span.
    /// </summary>
    public void Reset() => _unwritten = Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UnsafeAdvance(int count)
    {
        Debug.Assert((uint)count <= (uint)_unwritten.Length);
        UncheckedSlice(ref _unwritten, count, _unwritten.Length - count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UncheckedSlice(ref Span<T> span, int start, int length)
    {
        Debug.Assert((uint)start <= (uint)span.Length && (uint)length <= (uint)(span.Length - start));
        span = MemoryMarshal.CreateSpan(ref Unsafe.Add(ref MemoryMarshal.GetReference(span), (nint)(uint)start), length);
    }
}
