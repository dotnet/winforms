// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

internal static class Utilities
{
    /// <summary>
    ///  Executes the given action with retry logic for OLE operations.
    /// </summary>
    /// <param name="action">Execute the action which returns bool value indicating whether to continue retries or stop.</param>
    /// <param name="retryCount">Number of retry attempts.</param>
    /// <param name="retryDelay">Delay in milliseconds between retries.</param>
    internal static void ExecuteWithRetry(
        Func<bool> action,
        int retryCount = ClipboardConstants.OleRetryCount,
        int retryDelay = ClipboardConstants.OleRetryDelay)
    {
        int attempts = 0;

        while (attempts < retryCount)
        {
            if (action())
            {
                attempts++;
                Thread.Sleep(retryDelay);

                continue;
            }

            break;
        }
    }
}
