// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <summary>
    ///  Tries to get system metrics for the specified <paramref name="dpi"/>. If the OS does not support scaling
    ///  <paramref name="dpi"/> is ignored.
    /// </summary>
    public static int GetCurrentSystemMetrics(SYSTEM_METRICS_INDEX nIndex, uint dpi)
        => OsVersion.IsWindows10_1607OrGreater() ? GetSystemMetricsForDpi(nIndex, dpi) : PInvokeCore.GetSystemMetrics(nIndex);
}
