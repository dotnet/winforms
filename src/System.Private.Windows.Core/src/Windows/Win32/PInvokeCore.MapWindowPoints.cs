// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <inheritdoc cref="MapWindowPoints(HWND, HWND, Point*, uint)"/>
    public static unsafe int MapWindowPoints<TFrom, TTo>(TFrom hWndFrom, TTo hWndTo, ref RECT lpRect)
        where TFrom : IHandle<HWND>
        where TTo : IHandle<HWND>
    {
        fixed (void* p = &lpRect)
        {
            int result = MapWindowPoints(hWndFrom.Handle, hWndTo.Handle, (Point*)p, cPoints: 2);
            GC.KeepAlive(hWndFrom.Wrapper);
            GC.KeepAlive(hWndTo.Wrapper);
            return result;
        }
    }

    /// <inheritdoc cref="MapWindowPoints(HWND, HWND, Point*, uint)"/>
    public static unsafe int MapWindowPoints<TFrom, TTo>(TFrom hWndFrom, TTo hWndTo, ref Point lpPoint)
        where TFrom : IHandle<HWND>
        where TTo : IHandle<HWND>
    {
        fixed (void* p = &lpPoint)
        {
            int result = MapWindowPoints(hWndFrom.Handle, hWndTo.Handle, (Point*)p, cPoints: 1);
            GC.KeepAlive(hWndFrom.Wrapper);
            GC.KeepAlive(hWndTo.Wrapper);
            return result;
        }
    }
}
