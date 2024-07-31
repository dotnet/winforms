// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Drawing;

internal static partial class Gdip
{
    private static readonly bool s_initialized = Init();

    [ThreadStatic]
    private static IDictionary<object, object>? t_threadData;

    private static unsafe bool Init()
    {
        if (!OperatingSystem.IsWindows())
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (_, _, _) =>
                throw new PlatformNotSupportedException(SR.PlatformNotSupported_Unix));
        }

        return GdiPlusInitialization.EnsureInitialized();
    }

    internal static bool Initialized => s_initialized;

    /// <summary>
    ///  This property will give us back a dictionary we can use to store all of our static brushes and pens on
    ///  a per-thread basis. This way we can avoid 'object in use' crashes when different threads are
    ///  referencing the same drawing object.
    /// </summary>
    internal static IDictionary<object, object> ThreadData => t_threadData ??= new Dictionary<object, object>();

    internal static void CheckStatus(Status status) => status.ThrowIfFailed();

    internal static Exception StatusException(Status status) => status.GetException();
}
