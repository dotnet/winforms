// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Helper to scope lifetime of an HDC retrieved via <see cref="PInvokeCore.BeginPaint(HWND, out PAINTSTRUCT)"/>
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement. If you must pass this around, always pass
///   by <see langword="ref" /> to avoid duplicating the handle and risking a double EndPaint.
///  </para>
/// </remarks>
#if DEBUG
internal class BeginPaintScope : DisposalTracking.Tracker, IDisposable
#else
internal readonly ref struct BeginPaintScope
#endif
{
    private readonly PAINTSTRUCT _paintStruct;

    public HDC HDC { get; }
    public HWND HWND { get; }
    public Rectangle PaintRectangle => _paintStruct.rcPaint;

    public BeginPaintScope(HWND hwnd)
    {
        HDC = PInvokeCore.BeginPaint(hwnd, out _paintStruct);
        HWND = hwnd;
    }

    public static implicit operator HDC(in BeginPaintScope scope) => scope.HDC;

    public void Dispose()
    {
        if (!HDC.IsNull)
        {
            PInvokeCore.EndPaint(HWND, _paintStruct);
        }

#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }
}
