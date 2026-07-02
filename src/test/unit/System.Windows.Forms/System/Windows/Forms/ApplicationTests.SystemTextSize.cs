// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using Microsoft.DotNet.RemoteExecutor;

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
    public void Application_SystemTextSizeAwareness_DefaultValueIsUnaware()
    {
        RemoteExecutor.Invoke(() =>
        {
            Assert.Equal(SystemTextSizeAwareness.Unaware, Application.SystemTextSizeAwareness);
        }).Dispose();
    }

    [WinFormsTheory]
    [EnumData<SystemTextSizeAwareness>]
    public void Application_SystemTextSizeAwareness_Set_GetReturnsExpected(SystemTextSizeAwareness value)
    {
        SystemTextSizeAwareness originalValue = Application.SystemTextSizeAwareness;

        try
        {
            Application.SetSystemTextSizeAwareness(value);
            Assert.Equal(value, Application.SystemTextSizeAwareness);

            Application.SetSystemTextSizeAwareness(value);
            Assert.Equal(value, Application.SystemTextSizeAwareness);
        }
        finally
        {
            Application.SetSystemTextSizeAwareness(originalValue);
        }
    }

    [WinFormsTheory]
    [InvalidEnumData<SystemTextSizeAwareness>]
    public void Application_SetSystemTextSizeAwareness_InvalidValue_ThrowsInvalidEnumArgumentException(SystemTextSizeAwareness value)
    {
        Assert.Throws<InvalidEnumArgumentException>(
            "awareness",
            () => Application.SetSystemTextSizeAwareness(value));
    }
#endif
}
