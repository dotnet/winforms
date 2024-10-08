// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

internal readonly partial struct LRESULT
{
    public static explicit operator int(LRESULT value) => (int)value.Value;
    public static explicit operator uint(LRESULT value) => (uint)value.Value;
    public static explicit operator nuint(LRESULT value) => (nuint)value.Value;
    public static explicit operator char(LRESULT value) => (char)value.Value;
    public static explicit operator HWND(LRESULT value) => (HWND)value.Value;
    public static explicit operator HFONT(LRESULT value) => (HFONT)value.Value;
    public static explicit operator HICON(LRESULT value) => (HICON)value.Value;

    public static explicit operator BOOL(LRESULT value) => (BOOL)value.Value;
    public static explicit operator LRESULT(BOOL value) => new((nint)value);

    // #define HIWORD(l)  ((WORD)((((DWORD_PTR)(l)) >> 16) & 0xffff))
    public ushort HIWORD => (ushort)((((nuint)Value) >> 16) & 0xffff);
    public short SIGNEDHIWORD => (short)HIWORD;

    // #define LOWORD(l)  ((WORD)(((DWORD_PTR)(l)) & 0xffff))
    public ushort LOWORD => (ushort)(((nuint)Value) & 0xffff);
    public short SIGNEDLOWORD => (short)LOWORD;

    // #define MAKELONG(a, b)  ((LONG)(((WORD)(((DWORD_PTR)(a)) & 0xffff))
    //   | ((DWORD)((WORD)(((DWORD_PTR)(b)) & 0xffff))) << 16))
    public static LRESULT MAKELONG(int low, int high) => (LRESULT)((int)(((ushort)(((nuint)low) & 0xffff))
        | ((uint)((ushort)(((nuint)high) & 0xffff))) << 16));
}
