// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

public class UtilitiesTests
{
    [Fact]
    public void ExecuteWithRetry_ActionReturnsFalse_CallsOnce()
    {
        int calls = 0;

        Utilities.ExecuteWithRetry(() =>
        {
            calls++;

            return false;
        },
        retryCount: 3,
        retryDelay: 0);

        Assert.Equal(1, calls);
    }

    [Fact]
    public void ExecuteWithRetry_ActionReturnsTrueThenFalse_RetriesUntilFalse()
    {
        int calls = 0;

        Utilities.ExecuteWithRetry(() =>
        {
            calls++;

            return calls < 3;
        },
        retryCount: 5,
        retryDelay: 0);

        Assert.Equal(3, calls);
    }

    [Fact]
    public void ExecuteWithRetry_ActionAlwaysReturnsTrue_StopsAtRetryCount()
    {
        int calls = 0;

        Utilities.ExecuteWithRetry(() =>
        {
            calls++;

            return true;
        },
        retryCount: 3,
        retryDelay: 0);

        Assert.Equal(3, calls);
    }
}
