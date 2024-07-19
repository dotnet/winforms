// Licensed to the.NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Primitives;

namespace System.Windows.Forms.Tests;

public class LocalAppContextSwitchesTest
{
    [WinFormsTheory]
    [InlineData(".NETCoreApp,Version=v8.0", true)]
    [InlineData(".NETCoreApp,Version=v7.0", false)]
    [InlineData(".NET Framework,Version=v4.8", false)]
    public void Validate_Default_Switch_Values(string targetFrameworkName, bool expected)
    {
        using TargetFrameworkNameScope scope = new(targetFrameworkName);
        Assert.Equal(expected, LocalAppContextSwitches.TrackBarModernRendering);
        Assert.Equal(expected, LocalAppContextSwitches.ScaleTopLevelFormMinMaxSizeForDpi);
        Assert.Equal(expected, LocalAppContextSwitches.ServicePointManagerCheckCrl);
    }
}
