// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static unsafe partial class PInvokeCore
{
    public delegate BOOL EnumWindowsCallback(HWND hWnd);

#if NETFRAMEWORK
    private delegate BOOL EnumWindowsNativeCallback(HWND hWnd, LPARAM lParam);
    private static readonly EnumWindowsNativeCallback s_enumWindowsNativeCallback = HandleEnumWindowsNativeCallback;
    private static readonly delegate* unmanaged[Stdcall]<HWND, LPARAM, BOOL> s_enumWindowsNativeCallbackFunctionPointer =
        (delegate* unmanaged[Stdcall]<HWND, LPARAM, BOOL>)Marshal.GetFunctionPointerForDelegate(s_enumWindowsNativeCallback);
#endif

    public static unsafe BOOL EnumWindows(EnumWindowsCallback callback)
    {
        // We pass a function pointer to the native function and supply the callback as
        // reference data, so that the CLR doesn't need to generate a native code block for
        // each callback delegate instance (for storing the closure pointer).
        GCHandle gcHandle = GCHandle.Alloc(callback);
        try
        {
#if NET
            return EnumWindows(&HandleEnumWindowsNativeCallback, (LPARAM)(nint)gcHandle);
#else
            return EnumWindows(s_enumWindowsNativeCallbackFunctionPointer, (LPARAM)(nint)gcHandle);
#endif
        }
        finally
        {
            gcHandle.Free();
        }
    }

#if NET
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
#endif
    private static BOOL HandleEnumWindowsNativeCallback(HWND hWnd, LPARAM lParam)
    {
        return ((EnumWindowsCallback)((GCHandle)(nint)lParam).Target!)(hWnd);
    }
}
