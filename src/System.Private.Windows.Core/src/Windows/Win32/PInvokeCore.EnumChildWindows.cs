// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static unsafe partial class PInvokeCore
{
    public delegate BOOL EnumChildWindowsCallback(HWND hWnd);

#if NETFRAMEWORK
    private delegate BOOL EnumChildWindowsNativeCallback(HWND hWnd, LPARAM lParam);
    private static readonly EnumChildWindowsNativeCallback s_enumChildWindowsNativeCallback = HandleEnumChildWindowsNativeCallback;
    private static readonly delegate* unmanaged[Stdcall]<HWND, LPARAM, BOOL> s_enumChildWindowsNativeCallbackFunctionPointer =
        (delegate* unmanaged[Stdcall]<HWND, LPARAM, BOOL>)Marshal.GetFunctionPointerForDelegate(s_enumChildWindowsNativeCallback);
#endif

    public static BOOL EnumChildWindows<T>(T hwndParent, EnumChildWindowsCallback callback)
        where T : IHandle<HWND>
    {
        // We pass a function pointer to the native function and supply the callback as
        // reference data, so that the CLR doesn't need to generate a native code block for
        // each callback delegate instance (for storing the closure pointer).
        GCHandle gcHandle = GCHandle.Alloc(callback);
        try
        {
#if NET
            return EnumChildWindows(hwndParent.Handle, &HandleEnumChildWindowsNativeCallback, (LPARAM)(nint)gcHandle);
#else
            return EnumChildWindows(hwndParent.Handle, s_enumChildWindowsNativeCallbackFunctionPointer, (LPARAM)(nint)gcHandle);
#endif
        }
        finally
        {
            gcHandle.Free();
            GC.KeepAlive(hwndParent.Wrapper);
        }
    }

#if NET
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
#endif
    private static BOOL HandleEnumChildWindowsNativeCallback(HWND hWnd, LPARAM lParam)
    {
        return ((EnumChildWindowsCallback)((GCHandle)(nint)lParam).Target!)(hWnd);
    }
}
