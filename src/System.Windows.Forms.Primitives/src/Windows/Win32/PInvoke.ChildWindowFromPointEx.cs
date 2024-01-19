// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="ChildWindowFromPointEx(HWND, Point, CWP_FLAGS)"/>
    public static HWND ChildWindowFromPointEx<T>(T hwndParent, Point pt, CWP_FLAGS uFlags)
        where T : IHandle<HWND>
    {
        HWND result = ChildWindowFromPointEx(hwndParent.Handle, pt, uFlags);
        GC.KeepAlive(hwndParent.Wrapper);
        return result;
    }
}
