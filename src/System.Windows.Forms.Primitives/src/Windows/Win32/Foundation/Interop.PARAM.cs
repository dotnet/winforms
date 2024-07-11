// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Foundation;

/// <summary>
///  Helpers for creating W/LPARAM arguments for messages.
/// </summary>
internal static class PARAM
{
    public static nint FromLowHigh(int low, int high) => ToInt(low, high);

    public static nint FromLowHighUnsigned(int low, int high)
        // Convert the int to an uint before converting it to a pointer type,
        // which ensures the high DWORD being zero for 64-bit pointers.
        // This corresponds to the logic of the MAKELPARAM/MAKEWPARAM/MAKELRESULT
        // macros.
        => (nint)(uint)ToInt(low, high);

    public static int ToInt(int low, int high)
        => (high << 16) | (low & 0xffff);

    public static int HIWORD(int n)
        => (n >> 16) & 0xffff;

    public static int LOWORD(int n)
        => n & 0xffff;

    public static int LOWORD(nint n)
        => LOWORD((int)n);

    public static int HIWORD(nint n)
        => HIWORD((int)n);

    public static int SignedHIWORD(nint n)
        => SignedHIWORD((int)n);

    public static int SignedLOWORD(nint n)
        => SignedLOWORD(unchecked((int)n));

    public static int SignedHIWORD(int n)
        => (short)HIWORD(n);

    public static int SignedLOWORD(int n)
        => (short)LOWORD(n);

    public static nint FromBool(bool value)
        => (nint)(BOOL)(value);

    /// <summary>
    ///  Hard casts to <see langword="int" /> without bounds checks.
    /// </summary>
    public static int ToInt(nint param) => (int)param;

    /// <summary>
    ///  Hard casts to <see langword="uint" /> without bounds checks.
    /// </summary>
    public static uint ToUInt(nint param) => (uint)param;

    /// <summary>
    ///  Packs a <see cref="Point"/> into a PARAM.
    /// </summary>
    public static nint FromPoint(Point point)
        => FromLowHigh(point.X, point.Y);

    /// <summary>
    ///  Unpacks a <see cref="Point"/> from a PARAM.
    /// </summary>
    public static Point ToPoint(nint param)
        => new(SignedLOWORD(param), SignedHIWORD(param));
}
