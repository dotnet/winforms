// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static unsafe partial class PInvokeCore
{
    public delegate BOOL EnumThreadWindowsCallback(HWND hWnd);

#if NETFRAMEWORK
    private delegate BOOL EnumThreadWindowsNativeCallback(HWND hWnd, LPARAM lParam);
    private static readonly EnumThreadWindowsNativeCallback s_enumThreadWindowsNativeCallback = HandleEnumThreadWindowsNativeCallback;
    private static readonly delegate* unmanaged[Stdcall]<HWND, LPARAM, BOOL> s_enumThreadWindowsNativeCallbackFunctionPointer =
        (delegate* unmanaged[Stdcall]<HWND, LPARAM, BOOL>)Marshal.GetFunctionPointerForDelegate(s_enumThreadWindowsNativeCallback);
#endif

    /// <summary>
    ///  Enumerates all nonchild windows in the current thread.
    /// </summary>
    public static BOOL EnumCurrentThreadWindows(EnumThreadWindowsCallback callback)
    {
        // We pass a function pointer to the native function and supply the callback as
        // reference data, so that the CLR doesn't need to generate a native code block for
        // each callback delegate instance (for storing the closure pointer).
        GCHandle gcHandle = GCHandle.Alloc(callback);
        try
        {
#if NET
            return EnumThreadWindows(GetCurrentThreadId(), &HandleEnumThreadWindowsNativeCallback, (LPARAM)(nint)gcHandle);
#else
            return EnumThreadWindows(
                GetCurrentThreadId(),
                s_enumThreadWindowsNativeCallbackFunctionPointer,
                (LPARAM)(nint)gcHandle);
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
    private static BOOL HandleEnumThreadWindowsNativeCallback(HWND hWnd, LPARAM lParam)
    {
        return ((EnumThreadWindowsCallback)((GCHandle)(nint)lParam).Target!)(hWnd);
    }
}
