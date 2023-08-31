// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

/// <summary>
///  Helper to scope lifetime of an <see cref="HDC"/> retrieved via <see cref="PInvoke.GetDC(HWND)"/> and
///  <see cref="PInvoke.GetDCEx(HWND, HRGN, GET_DCX_FLAGS)"/>. Releases the <see cref="HDC"/> (if any)
///  when disposed.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement. If you must pass this around, always pass by <see langword="ref" />
///   to avoid duplicating the handle and risking a double release.
///  </para>
/// </remarks>
internal readonly ref struct GetDcScope
{
    public HDC HDC { get; }
    public HWND HWND { get; }

    public GetDcScope(HWND hwnd)
    {
        HWND = hwnd;
        HDC = PInvoke.GetDC(hwnd);
    }

    /// <summary>
    ///  Creates a <see cref="HDC"/> using <see cref="PInvoke.GetDCEx(HWND, HRGN, GET_DCX_FLAGS)"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   GetWindowDC calls GetDCEx(hwnd, null, DCX_WINDOW | DCX_USESTYLE).
    ///  </para>
    ///  <para>
    ///   GetDC calls GetDCEx(hwnd, null, DCX_USESTYLE) when given a handle. (When given null it has additional
    ///   logic, and can't be replaced directly by GetDCEx.
    ///  </para>
    /// </remarks>
    public GetDcScope(HWND hwnd, HRGN hrgnClip, GET_DCX_FLAGS flags)
    {
        HWND = hwnd;
        HDC = PInvoke.GetDCEx(hwnd, hrgnClip, flags);
    }

    /// <summary>
    ///  Creates a DC scope for the primary monitor (not the entire desktop).
    /// </summary>
    /// <remarks>
    ///   <para>
    ///    <see cref="PInvoke.CreateDCW(PCWSTR, PCWSTR, PCWSTR, Windows.Win32.Graphics.Gdi.DEVMODEW*)" /> is the
    ///    API to get the DC for the entire desktop.
    ///    </para>
    /// </remarks>
    public static GetDcScope ScreenDC => new(HWND.Null);

    public bool IsNull => HDC.IsNull;

    public static implicit operator nint(in GetDcScope scope) => scope.HDC;
    public static implicit operator HDC(in GetDcScope scope) => scope.HDC;

    public void Dispose()
    {
        if (!HDC.IsNull)
        {
            PInvoke.ReleaseDC(HWND, HDC);
        }
    }
}
