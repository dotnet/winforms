// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class LayoutSettingsTests
{
    [Fact]
    public void LayoutSettings_Ctor_Default()
    {
        SubLayoutSettings settings = new();
        Assert.Null(settings.LayoutEngine);
    }

    private class SubLayoutSettings : LayoutSettings
    {
        public SubLayoutSettings() : base() { }
    }
}
