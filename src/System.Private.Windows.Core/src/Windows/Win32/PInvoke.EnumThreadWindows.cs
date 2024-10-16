// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    public delegate BOOL EnumThreadWindowsCallback(HWND hWnd);

    /// <summary>
    ///  Enumerates all nonchild windows in the current thread.
    /// </summary>
    public static unsafe BOOL EnumCurrentThreadWindows(EnumThreadWindowsCallback callback)
    {
        // We pass a function pointer to the native function and supply the callback as
        // reference data, so that the CLR doesn't need to generate a native code block for
        // each callback delegate instance (for storing the closure pointer).
        GCHandle gcHandle = GCHandle.Alloc(callback);
        try
        {
            return EnumThreadWindows(GetCurrentThreadId(), &HandleEnumThreadWindowsNativeCallback, (LPARAM)(nint)gcHandle);
        }
        finally
        {
            gcHandle.Free();
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static BOOL HandleEnumThreadWindowsNativeCallback(HWND hWnd, LPARAM lParam)
    {
        return ((EnumThreadWindowsCallback)((GCHandle)(nint)lParam).Target!)(hWnd);
    }
}
