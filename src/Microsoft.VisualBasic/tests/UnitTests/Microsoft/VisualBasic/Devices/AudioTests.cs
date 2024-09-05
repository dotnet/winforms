// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace Microsoft.VisualBasic.Devices.Tests;

public class AudioTests
{
    private static string GetUniqueName() => Guid.NewGuid().ToString("D");

    [Fact]
    public void Play()
    {
        string location = Path.Combine(Path.GetTempPath(), GetUniqueName());
        Audio audio = new();
        Action testCode = () => audio.Play(location);
        testCode.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void PlayInvalidMode_Throws()
    {
        string location = Path.Combine(Path.GetTempPath(), GetUniqueName());
        Audio audio = new();
        Action testCode = () => audio.Play(location, (AudioPlayMode)(-1));
        testCode.Should().Throw<InvalidEnumArgumentException>();
    }

    [Theory]
    [InlineData(AudioPlayMode.Background)]
    [InlineData(AudioPlayMode.BackgroundLoop)]
    [InlineData(AudioPlayMode.WaitToComplete)]
    public void PlayAllModes_Throws(AudioPlayMode mode)
    {
        string location = Path.Combine(Path.GetTempPath(), GetUniqueName());
        Audio audio = new();
        Action testCode = () => audio.Play(location, mode);
        testCode.Should().Throw<FileNotFoundException>();
    }

    // Not tested:
    //    Public Sub PlaySystemSound(systemSound As System.Media.SystemSound)

    [Fact]
    public void Stop()
    {
        Audio audio = new();
        audio.Stop();
    }
}
