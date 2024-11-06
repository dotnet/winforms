// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.VisualBasic.Devices.Tests;

public class ComputerInfoTests
{
    [Fact]
    public void Properties()
    {
        ComputerInfo info = new();
        CultureInfo.InstalledUICulture.Should().Be(info.InstalledUICulture);
        Environment.OSVersion.Platform.ToString().Should().Be(info.OSPlatform);
        Environment.OSVersion.Version.ToString().Should().Be(info.OSVersion);
    }

    [Fact]
    public void Memory()
    {
        ComputerInfo info = new();
        info.AvailablePhysicalMemory.Should().NotBe(0u);
        info.AvailableVirtualMemory.Should().NotBe(0u);
        info.TotalPhysicalMemory.Should().NotBe(0u);
        info.TotalVirtualMemory.Should().NotBe(0u);
    }

    [Fact]
    public void OSFullName()
    {
        ComputerInfo info = new();
        string fullName = info.OSFullName;
        fullName.Should().NotBeNullOrEmpty();
        RuntimeInformation.OSDescription.Should().Be(fullName);
    }
}
