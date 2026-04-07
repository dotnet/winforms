// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Windows.Forms.Legacy.Tests;

public class WebBrowserTests
{
    [StaFact]
    public void TestWebBrowserNoMemoryLeak()
    {
        var browserWeakRef = CreateWebBrowser();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var isBrowserGC = !browserWeakRef.IsAlive;
        Assert.True(isBrowserGC, "WebBrowser should be collected after GC.Collect and GC.WaitForPendingFinalizers.");

        // NET CORE 8.0+: isBrowserGC is false;
        // NET CORE 7.0: isBrowserGC is true;
        // NET Framework 4.8: isBrowserGC is true;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateWebBrowser()
    {
        using (var browser = new WebBrowser())
        {
            browser.Navigate("about:blank");
            return new WeakReference(browser);
        }
    }
}
