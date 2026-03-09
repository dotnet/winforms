// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Versioning.Tests;

public class PlatformAttributeTests
{
    [Fact]
    public void SupportedOSPlatformAttribute_PlatformName()
    {
        SupportedOSPlatformAttribute attr = new("windows");
        attr.PlatformName.Should().Be("windows");
    }

    [Fact]
    public void SupportedOSPlatformAttribute_PlatformNameWithVersion()
    {
        SupportedOSPlatformAttribute attr = new("windows10.0");
        attr.PlatformName.Should().Be("windows10.0");
    }

    [Fact]
    public void UnsupportedOSPlatformAttribute_PlatformName()
    {
        UnsupportedOSPlatformAttribute attr = new("linux");
        attr.PlatformName.Should().Be("linux");
    }

    [Fact]
    public void UnsupportedOSPlatformAttribute_WithMessage()
    {
        UnsupportedOSPlatformAttribute attr = new("linux", "Not supported on Linux");
        attr.PlatformName.Should().Be("linux");
        attr.Message.Should().Be("Not supported on Linux");
    }

    [Fact]
    public void TargetPlatformAttribute_PlatformName()
    {
        TargetPlatformAttribute attr = new("windows7.0");
        attr.PlatformName.Should().Be("windows7.0");
    }

    [Fact]
    public void ObsoletedOSPlatformAttribute_PlatformName()
    {
        ObsoletedOSPlatformAttribute attr = new("windows6.0");
        attr.PlatformName.Should().Be("windows6.0");
    }

    [Fact]
    public void ObsoletedOSPlatformAttribute_WithMessage()
    {
        ObsoletedOSPlatformAttribute attr = new("windows6.0", "Use newer version");
        attr.PlatformName.Should().Be("windows6.0");
        attr.Message.Should().Be("Use newer version");
    }

    [Fact]
    public void ObsoletedOSPlatformAttribute_UrlProperty()
    {
        ObsoletedOSPlatformAttribute attr = new("windows6.0") { Url = "https://example.com" };
        attr.Url.Should().Be("https://example.com");
    }

    [Fact]
    public void SupportedOSPlatformGuardAttribute_PlatformName()
    {
        SupportedOSPlatformGuardAttribute attr = new("windows10.0.19041");
        attr.PlatformName.Should().Be("windows10.0.19041");
    }

    [Fact]
    public void UnsupportedOSPlatformGuardAttribute_PlatformName()
    {
        UnsupportedOSPlatformGuardAttribute attr = new("browser");
        attr.PlatformName.Should().Be("browser");
    }

    [Fact]
    public void OSPlatformAttribute_InheritedByDerived()
    {
        typeof(SupportedOSPlatformAttribute).IsSubclassOf(typeof(OSPlatformAttribute)).Should().BeTrue();
        typeof(UnsupportedOSPlatformAttribute).IsSubclassOf(typeof(OSPlatformAttribute)).Should().BeTrue();
        typeof(TargetPlatformAttribute).IsSubclassOf(typeof(OSPlatformAttribute)).Should().BeTrue();
        typeof(ObsoletedOSPlatformAttribute).IsSubclassOf(typeof(OSPlatformAttribute)).Should().BeTrue();
        typeof(SupportedOSPlatformGuardAttribute).IsSubclassOf(typeof(OSPlatformAttribute)).Should().BeTrue();
        typeof(UnsupportedOSPlatformGuardAttribute).IsSubclassOf(typeof(OSPlatformAttribute)).Should().BeTrue();
    }
}
