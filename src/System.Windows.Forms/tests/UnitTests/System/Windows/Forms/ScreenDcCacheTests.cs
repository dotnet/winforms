// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ScreenDcCacheTests
{
    [Fact(Skip = "Run manually, takes a few minutes and is very resource intensive.")]
    public void StressTest()
    {
        Random random = new();
        using ScreenDcCache cache = new();

        for (int i = 0; i < 10000; i++)
        {
            Thread.Sleep(random.Next(5));
            Task.Run(() =>
            {
                using var screen = cache.Acquire();
                Assert.False(screen.HDC.IsNull);
                Thread.Sleep(random.Next(5));
            });
        }
    }
}
