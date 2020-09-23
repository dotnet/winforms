﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

internal partial class Interop
{
    /// <summary>
    ///  Helpers for creating W/LPARAM arguments for messages.
    /// </summary>
    internal static class PARAM
    {
        public static IntPtr FromLowHigh(int low, int high)
            => (IntPtr)ToInt(low, high);

        public static IntPtr FromLowHighUnsigned(int low, int high)
            // Convert the int to an uint before converting it to a pointer type,
            // which ensures the high dword being zero for 64-bit pointers.
            // This corresponds to the logic of the MAKELPARAM/MAKEWPARAM/MAKELRESULT
            // macros.
            => unchecked((nint)(nuint)(uint)ToInt(low, high));

        public static int ToInt(int low, int high)
            => (high << 16) | (low & 0xffff);

        public static int HIWORD(int n)
            => (n >> 16) & 0xffff;

        public static int LOWORD(int n)
            => n & 0xffff;

#pragma warning disable IDE0004 // (nint) cast is not actually redundant
        public static int LOWORD(IntPtr n)
            => LOWORD(unchecked((int)(nint)n));

        public static int HIWORD(IntPtr n)
            => HIWORD(unchecked((int)(nint)n));

        public static int SignedHIWORD(IntPtr n)
            => SignedHIWORD(unchecked((int)(nint)n));

        public static int SignedLOWORD(IntPtr n)
            => SignedLOWORD(unchecked((int)(nint)n));
#pragma warning restore IDE0004

        public static int SignedHIWORD(int n)
            => (int)(short)HIWORD(n);

        public static int SignedLOWORD(int n)
            => (int)(short)LOWORD(n);

        public static IntPtr FromBool(bool value)
            => (IntPtr)(value ? BOOL.TRUE : BOOL.FALSE);

        public static IntPtr FromColor(Color color)
            => (IntPtr)ColorTranslator.ToWin32(color);

#pragma warning disable IDE0004 // (nint) cast is not actually redundant
        /// <summary>
        ///  Hard casts to <see langword="int" /> without bounds checks.
        /// </summary>
        public static int ToInt(IntPtr param) => (int)(nint)param;

        /// <summary>
        ///  Hard casts to <see langword="uint" /> without bounds checks.
        /// </summary>
        public static uint ToUInt(IntPtr param) => (uint)(nint)param;
#pragma warning restore IDE0004

        /// <summary>
        ///  Hard casts to <see langword="long" /> without bounds checks.
        /// </summary>
        /// <remarks>
        ///  Technically not needed, but here for completeness.
        /// </remarks>
        public static long ToLong(IntPtr param) => (long)param;

        /// <summary>
        ///  Hard casts to <see langword="ulong" /> without bounds checks.
        /// </summary>
        public static ulong ToULong(IntPtr param) => (ulong)(long)param;
    }
}
