// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Foundation;

internal readonly unsafe partial struct WPARAM
{
    public static implicit operator void*(WPARAM value) => (void*)value.Value;
    public static implicit operator WPARAM(void* value) => new((nuint)value);

    public static explicit operator HWND(WPARAM value) => (HWND)(nint)value.Value;
    public static explicit operator WPARAM(HWND value) => new((nuint)value.Value);

    public static explicit operator HDC(WPARAM value) => (HDC)(nint)value.Value;
    public static explicit operator WPARAM(HDC value) => new((nuint)value.Value);

    public static explicit operator WPARAM(HFONT value) => new((nuint)value.Value);

    public static explicit operator BOOL(WPARAM value) => (BOOL)(nint)value.Value;
    public static explicit operator WPARAM(BOOL value) => new((nuint)(nint)value);

    public static explicit operator int(WPARAM value) => (int)(nint)value.Value;
    public static explicit operator uint(WPARAM value) => (uint)value.Value;
    public static explicit operator nint(WPARAM value) => (nint)value.Value;
    public static explicit operator WPARAM(int value) => new((nuint)(nint)value);

    public static explicit operator WPARAM(char value) => new(value);

    public static explicit operator WPARAM(Color value) => new((nuint)ColorTranslator.ToWin32(value));

    // #define HIWORD(l)           ((WORD)((((DWORD_PTR)(l)) >> 16) & 0xffff))
    public ushort HIWORD => (ushort)(((Value) >> 16) & 0xffff);
    public short SIGNEDHIWORD => (short)HIWORD;

    // #define LOWORD(l)           ((WORD)(((DWORD_PTR)(l)) & 0xffff))
    public ushort LOWORD => (ushort)(Value & 0xffff);
    public short SIGNEDLOWORD => (short)LOWORD;

    // #define MAKEWPARAM(l, h)    ((WPARAM)(DWORD)MAKELONG(l, h))
    // #define MAKELONG(a, b)      ((LONG)(((WORD)(((DWORD_PTR)(a)) & 0xffff))
    //   | ((DWORD)((WORD)(((DWORD_PTR)(b)) & 0xffff))) << 16))
    public static WPARAM MAKEWPARAM(int low, int high) =>
        (WPARAM)(uint)((int)(((ushort)(((nuint)low) & 0xffff)) | ((uint)((ushort)(((nuint)high) & 0xffff))) << 16));
}
