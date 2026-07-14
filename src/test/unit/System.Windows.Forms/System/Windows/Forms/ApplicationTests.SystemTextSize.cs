// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

namespace System.Windows.Forms.Tests;

public partial class ApplicationTests
{
#if NET11_0_OR_GREATER
    [WinFormsFact]
    public void Application_SystemTextSize_GetReturnsExpected()
    {
        Assert.InRange(Application.SystemTextSize, 1.0, 2.25);
    }

    [WinFormsFact]
    public void Application_SystemTextSizeChanged_AddRemove_Success()
    {
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;

        Application.SystemTextSizeChanged += handler;
        Application.SystemTextSizeChanged -= handler;

        Assert.Equal(0, callCount);
    }
#endif
}
