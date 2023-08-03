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
        T* v = (T*)Unsafe.AsPointer(ref value);

        // These conditions get optimized away by the JIT.
        if (sizeof(T) == sizeof(byte))
        {
            if (set)
            {
                *(byte*)v |= *(byte*)&flags;
            }
            else
            {
                *(byte*)v &= (byte)~*(byte*)&flags;
            }
        }
        else if (sizeof(T) == sizeof(ushort))
        {
            if (set)
            {
                *(ushort*)v |= *(ushort*)&flags;
            }
            else
            {
                *(ushort*)v &= (ushort)~*(ushort*)&flags;
            }
        }
        else if (sizeof(T) == sizeof(uint))
        {
            if (set)
            {
                *(uint*)v |= *(uint*)&flags;
            }
            else
            {
                *(uint*)v &= ~*(uint*)&flags;
            }
        }
        else if (sizeof(T) == sizeof(ulong))
        {
            if (set)
            {
                *(ulong*)v |= *(ulong*)&flags;
            }
            else
            {
                *(ulong*)v &= ~*(ulong*)&flags;
            }
        }
    }
}
