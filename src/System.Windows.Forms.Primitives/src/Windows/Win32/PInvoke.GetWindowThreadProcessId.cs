// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    public static unsafe uint GetWindowThreadProcessId<T>(T hWnd, out uint lpdwProcessId)
        where T : IHandle<HWND>
    {
        uint processId;
        uint result = GetWindowThreadProcessId(hWnd.Handle, &processId);
        lpdwProcessId = processId;
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}
