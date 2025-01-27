// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

internal static partial class Gdip
{
    [ThreadStatic]
    private static IDictionary<object, object>? t_threadData;

    internal static bool Initialized => GdiPlusInitialization.IsInitialized;

    /// <summary>
    ///  This property will give us back a dictionary we can use to store all of our static brushes and pens on
    ///  a per-thread basis. This way we can avoid 'object in use' crashes when different threads are
    ///  referencing the same drawing object.
    /// </summary>
    internal static IDictionary<object, object> ThreadData => t_threadData ??= new Dictionary<object, object>();

    internal static void CheckStatus(Status status) => status.ThrowIfFailed();

    internal static Exception StatusException(Status status) => status.GetException();
}
