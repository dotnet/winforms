// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <summary>
    ///  Gets the thread DPI awareness context.
    /// </summary>
    /// <returns>
    ///  The thread DPI awareness context if the API is available in this version of OS.
    ///  Otherwise, <see cref="DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT"/>.
    /// </returns>
    public static DPI_AWARENESS_CONTEXT GetThreadDpiAwarenessContextInternal()
    {
        if (OsVersion.IsWindows10_1607OrGreater())
        {
            return GetThreadDpiAwarenessContext();
        }

        // legacy OS that does not have this API available.
        return DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT;
    }
}
