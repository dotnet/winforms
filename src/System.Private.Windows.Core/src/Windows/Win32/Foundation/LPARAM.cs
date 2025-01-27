// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Foundation;

internal readonly partial struct LPARAM
{
    public static unsafe implicit operator void*(LPARAM value) => (void*)value.Value;
    public static unsafe implicit operator LPARAM(void* value) => new((nint)value);

    public static explicit operator LPARAM(BOOL value) => new((nint)value);

    public static implicit operator LPARAM(int value) => new(value);

    public static explicit operator int(LPARAM value) => (int)value.Value;
    public static explicit operator uint(LPARAM value) => (uint)(int)value.Value;
    public static explicit operator nuint(LPARAM value) => (nuint)value.Value;
    public static explicit operator LPARAM(uint value) => new((nint)(nuint)value);

    public static explicit operator HWND(LPARAM value) => (HWND)value.Value;

    public static unsafe explicit operator LPARAM(HWND value) => (LPARAM)value.Value;

    public static explicit operator LPARAM(Color value) => (LPARAM)ColorTranslator.ToWin32(value);
    public static explicit operator Point(LPARAM value) => new(value.SIGNEDLOWORD, value.SIGNEDHIWORD);
    public static explicit operator LPARAM(Point value) => MAKELPARAM(value.X, value.Y);

    // #define HIWORD(l)           ((WORD)((((DWORD_PTR)(l)) >> 16) & 0xffff))
    public ushort HIWORD => (ushort)((((nuint)Value) >> 16) & 0xffff);
    public short SIGNEDHIWORD => (short)HIWORD;

    // #define LOWORD(l)           ((WORD)(((DWORD_PTR)(l)) & 0xffff))
    public ushort LOWORD => (ushort)(((nuint)Value) & 0xffff);
    public short SIGNEDLOWORD => (short)LOWORD;

    // #define MAKELPARAM(l, h)    ((LPARAM)(DWORD)MAKELONG(l, h))
    // #define MAKELONG(a, b)      ((LONG)(((WORD)(((DWORD_PTR)(a)) & 0xffff))
    //   | ((DWORD)((WORD)(((DWORD_PTR)(b)) & 0xffff))) << 16))
    public static LPARAM MAKELPARAM(int low, int high) => (LPARAM)(uint)((int)(((ushort)(((nuint)low) & 0xffff))
        | ((uint)((ushort)(((nuint)high) & 0xffff))) << 16));
}
