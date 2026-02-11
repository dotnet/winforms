// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System;

internal static class EnumExtensions
{
    /// <summary>
    ///  Sets the <paramref name="flags"/> if <paramref name="set"/> is <see langword="true"/>, otherwise clears them.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is not as succinct as directly setting flags, but the generated code isn't far off.
    ///  </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void ChangeFlags<T>(ref this T value, T flags, bool set) where T : unmanaged, Enum
    {
        // These conditions get optimized away by the JIT.
        if (sizeof(T) == sizeof(byte))
        {
            if (set)
            {
                Unsafe.As<T, byte>(ref value) |= *(byte*)&flags;
            }
            else
            {
                Unsafe.As<T, byte>(ref value) &= (byte)~*(byte*)&flags;
            }
        }
        else if (sizeof(T) == sizeof(ushort))
        {
            if (set)
            {
                Unsafe.As<T, ushort>(ref value) |= *(ushort*)&flags;
            }
            else
            {
                Unsafe.As<T, ushort>(ref value) &= (ushort)~*(ushort*)&flags;
            }
        }
        else if (sizeof(T) == sizeof(uint))
        {
            if (set)
            {
                Unsafe.As<T, uint>(ref value) |= *(uint*)&flags;
            }
            else
            {
                Unsafe.As<T, uint>(ref value) &= ~*(uint*)&flags;
            }
        }
        else if (sizeof(T) == sizeof(ulong))
        {
            if (set)
            {
                Unsafe.As<T, ulong>(ref value) |= *(ulong*)&flags;
            }
            else
            {
                Unsafe.As<T, ulong>(ref value) &= ~*(ulong*)&flags;
            }
        }
    }
}
