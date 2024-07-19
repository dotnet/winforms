// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Drawing;

internal static partial class Gdip
{
    private static readonly nuint s_initToken = Init();

    [ThreadStatic]
    private static IDictionary<object, object>? t_threadData;

    private static unsafe nuint Init()
    {
        if (!OperatingSystem.IsWindows())
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (_, _, _) =>
                throw new PlatformNotSupportedException(SR.PlatformNotSupported_Unix));
        }

        Debug.Assert(s_initToken == 0, "GdiplusInitialization: Initialize should not be called more than once in the same domain!");

        // GDI+ ref counts multiple calls to Startup in the same process, so calls from multiple
        // domains are ok, just make sure to pair each w/GdiplusShutdown

        nuint token;
        GdiplusStartupInputEx startup = GdiplusStartupInputEx.GetDefault();
        Status status = PInvoke.GdiplusStartup(&token, (GdiplusStartupInput*)&startup, null);
        CheckStatus(status);
        return token;
    }

    /// <summary>
    /// Returns true if GDI+ has been started, but not shut down
    /// </summary>
    internal static bool Initialized => s_initToken != 0;

    /// <summary>
    /// This property will give us back a dictionary we can use to store all of our static brushes and pens on
    /// a per-thread basis. This way we can avoid 'object in use' crashes when different threads are
    /// referencing the same drawing object.
    /// </summary>
    internal static IDictionary<object, object> ThreadData => t_threadData ??= new Dictionary<object, object>();

    // Used to ensure static constructor has run.
    internal static void DummyFunction()
    {
    }

    internal static void CheckStatus(Status status) => status.ThrowIfFailed();

    internal static Exception StatusException(Status status) => status.GetException();
}
