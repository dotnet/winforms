// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.GdiPlus;

/// <summary>
///  Helper to ensure GDI+ is initialized before making calls.
/// </summary>
internal static partial class GdiPlusInitialization
{
#if NET
    private static readonly Lock s_initializationLock = new();
#else
    private static readonly object s_initializationLock = new();
#endif
    private static nuint s_initToken;

    private static unsafe nuint Init()
    {
        Debug.Assert(
            Volatile.Read(ref s_initToken) == UIntPtr.Zero,
            "GdiplusInitialization: Initialize should not be called more than once!");

        // GDI+ ref counts multiple calls to Startup in the same process, so calls from multiple
        // domains are ok, just make sure to pair each w/GdiplusShutdown

        nuint token;
        GdiplusStartupInputEx startup = GdiplusStartupInputEx.GetDefault();
        PInvokeCore.GdiplusStartup(&token, (GdiplusStartupInput*)&startup, null).ThrowIfFailed();
        return token;
    }

    /// <summary>
    ///  Attempts to ensure that GGI+ is initialized. Returns <see langword="true"/> if GDI+ has been started.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This should be called anywhere you make <see cref="PInvokeCore"/> calls to GDI+ where you don't
    ///   already have a GDI+ handle. In System.Drawing.Common, this is done in the PInvoke static constructor
    ///   so it is not necessary for methods defined there.
    ///  </para>
    ///  <para>
    ///   https://github.com/microsoft/CsWin32/issues/1308 tracks a proposal to make this more automatic.
    ///  </para>
    /// </remarks>
    internal static bool EnsureInitialized()
    {
        nuint token = Volatile.Read(ref s_initToken);
        if (token == UIntPtr.Zero)
        {
            lock (s_initializationLock)
            {
                token = s_initToken;
                if (token == UIntPtr.Zero)
                {
                    token = Init();
                    Volatile.Write(ref s_initToken, token);
                }
            }
        }

        return token != UIntPtr.Zero;
    }

    /// <summary>
    ///  Returns <see langword="true"/> if GDI+ has been started.
    /// </summary>
    internal static bool IsInitialized
        => Volatile.Read(ref s_initToken) != UIntPtr.Zero;
}
