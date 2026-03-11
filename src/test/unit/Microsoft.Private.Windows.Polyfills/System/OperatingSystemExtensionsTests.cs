// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class OperatingSystemExtensionsTests
{
    [Fact]
    public void IsWindowsVersionAtLeast_CurrentMajor_ReturnsTrue()
    {
        // We are running on Windows, so current major version should be at least 6
        OperatingSystem.IsWindowsVersionAtLeast(6).Should().BeTrue();
    }

    [Fact]
    public void IsWindowsVersionAtLeast_VeryHighMajor_ReturnsFalse()
    {
        OperatingSystem.IsWindowsVersionAtLeast(999).Should().BeFalse();
    }

    [Fact]
    public void IsWindowsVersionAtLeast_MajorZero_ReturnsTrue()
    {
        OperatingSystem.IsWindowsVersionAtLeast(0).Should().BeTrue();
    }

    [Fact]
    public void IsWindowsVersionAtLeast_ExactCurrentVersion_ReturnsTrue()
    {
        Version current = Environment.OSVersion.Version;
        OperatingSystem.IsWindowsVersionAtLeast(
            current.Major,
            current.Minor,
            current.Build < 0 ? 0 : current.Build,
            current.Revision < 0 ? 0 : current.Revision).Should().BeTrue();
    }

    [Fact]
    public void IsWindowsVersionAtLeast_HigherMinor_ReturnsFalse()
    {
        Version current = Environment.OSVersion.Version;
        OperatingSystem.IsWindowsVersionAtLeast(current.Major, current.Minor + 1).Should().BeFalse();
    }

    [Fact]
    public void IsWindowsVersionAtLeast_LowerMajor_ReturnsTrue()
    {
        Version current = Environment.OSVersion.Version;
        if (current.Major > 1)
        {
            OperatingSystem.IsWindowsVersionAtLeast(current.Major - 1).Should().BeTrue();
        }
    }
}
