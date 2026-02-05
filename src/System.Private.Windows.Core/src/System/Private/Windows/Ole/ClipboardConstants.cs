// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

internal static class ClipboardConstants
{
    /// <summary>
    ///  The number of times to retry OLE clipboard operations.
    /// </summary>
    internal const int OleRetryCount = 10;

    /// <summary>
    ///  The amount of time in milliseconds to sleep between retrying OLE clipboard operations.
    /// </summary>
    internal const int OleRetryDelay = 100;
}
