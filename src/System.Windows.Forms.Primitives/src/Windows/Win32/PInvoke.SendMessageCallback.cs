// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Windows.Win32;

internal static partial class PInvoke
{
    internal static unsafe BOOL SendMessageCallback<T>(
       T hWnd,
       MessageId Msg,
       Action callback,
       WPARAM wParam = default,
       LPARAM lParam = default)
       where T : IHandle<HWND>
    {
        GCHandle gcHandle = GCHandle.Alloc(callback);
        BOOL result = SendMessageCallback(hWnd.Handle, Msg, wParam, lParam, &NativeCallback, (nuint)(nint)gcHandle);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void NativeCallback(HWND hwnd, uint Msg, nuint dwData, LRESULT lResult)
    {
        GCHandle gcHandle = (GCHandle)(nint)dwData;
        Action action = (Action)gcHandle.Target!;
        gcHandle.Free();
        action();
    }
}
