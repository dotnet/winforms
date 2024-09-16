// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.GdiPlus;

internal static partial class GdiPlusInitialization
{
    private static readonly nuint s_initToken = Init();

    private static unsafe nuint Init()
    {
        Debug.Assert(s_initToken == 0, "GdiplusInitialization: Initialize should not be called more than once!");

        // GDI+ ref counts multiple calls to Startup in the same process, so calls from multiple
        // domains are ok, just make sure to pair each w/GdiplusShutdown

        nuint token;
        GdiplusStartupInputEx startup = GdiplusStartupInputEx.GetDefault();
        PInvokeCore.GdiplusStartup(&token, (GdiplusStartupInput*)&startup, null).ThrowIfFailed();
        return token;
    }

    /// <summary>
    ///  Returns true if GDI+ has been started.
    /// </summary>
    internal static bool EnsureInitialized() => s_initToken != 0;
}
