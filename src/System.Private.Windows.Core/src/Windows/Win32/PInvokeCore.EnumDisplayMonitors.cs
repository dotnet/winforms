// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static unsafe partial class PInvokeCore
{
    public delegate bool EnumDisplayMonitorsCallback(HMONITOR monitor, HDC hdc);

#if NETFRAMEWORK
    private delegate BOOL EnumDisplayMonitorsNativeCallback(HMONITOR monitor, HDC hdc, RECT* lprcMonitor, LPARAM lParam);
    private static readonly EnumDisplayMonitorsNativeCallback s_enumDisplayMonitorsNativeCallback = HandleEnumDisplayMonitorsNativeCallback;
    private static readonly delegate* unmanaged[Stdcall]<HMONITOR, HDC, RECT*, LPARAM, BOOL> s_enumDisplayMonitorsNativeCallbackFunctionPointer =
        (delegate* unmanaged[Stdcall]<HMONITOR, HDC, RECT*, LPARAM, BOOL>)Marshal.GetFunctionPointerForDelegate(s_enumDisplayMonitorsNativeCallback);
#endif

    public static BOOL EnumDisplayMonitors(EnumDisplayMonitorsCallback callBack)
    {
        GCHandle gcHandle = GCHandle.Alloc(callBack);
        try
        {
#if NET
            return EnumDisplayMonitors(default, (RECT*)null, &HandleEnumDisplayMonitorsNativeCallback, (LPARAM)(nint)gcHandle);
#else
            return EnumDisplayMonitors(default, (RECT*)null, s_enumDisplayMonitorsNativeCallbackFunctionPointer, (LPARAM)(nint)gcHandle);
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
    private static BOOL HandleEnumDisplayMonitorsNativeCallback(HMONITOR monitor, HDC hdc, RECT* lprcMonitor, LPARAM lParam)
    {
        return ((EnumDisplayMonitorsCallback)((GCHandle)(nint)lParam).Target!)(monitor, hdc);
    }
}
