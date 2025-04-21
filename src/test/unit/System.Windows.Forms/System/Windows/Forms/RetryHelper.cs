// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class RetryHelper
{
    public static bool ExecuteWithRetry(Func<bool> action, int maxRetries = 3, int delayMilliseconds = 500)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            if (action())
            {
                return true;
            }
            else
            {
                Thread.Sleep(delayMilliseconds);
            }
        }

        return false;
    }
}
