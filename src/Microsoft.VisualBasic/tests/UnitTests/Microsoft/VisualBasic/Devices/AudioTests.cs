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

    [Theory]
    [EnumData<AudioPlayMode>]
    public void PlayAllModes_Throws(AudioPlayMode mode)
    {
        string location = Path.Combine(Path.GetTempPath(), GetUniqueName());
        Audio audio = new();
        Action testCode = () => audio.Play(location, mode);
        testCode.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void PlayBytes_Throws()
    {
        byte[] data = Array.Empty<byte>();
        Audio audio = new();
        Action testCode = () => audio.Play(data, AudioPlayMode.Background);
        testCode.Should().Throw<InvalidOperationException>();

        data = null;
        testCode = () => audio.Play(data, AudioPlayMode.Background);
        testCode.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PlayEmptyFileName_Throws()
    {
        Audio audio = new();
        Action testCode = () => audio.Play(string.Empty);
        testCode.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PlayInvalidMode_Throws()
    {
        string location = Path.Combine(Path.GetTempPath(), GetUniqueName());
        Audio audio = new();
        Action testCode = () => audio.Play(location, (AudioPlayMode)(-1));
        testCode.Should().Throw<InvalidEnumArgumentException>();
    }

    [Fact]
    public void PlayStream()
    {
        var data = new MemoryStream();
        Audio audio = new();
        Action testCode = () => audio.Play(data, AudioPlayMode.Background);
        testCode.Should().Throw<InvalidOperationException>();

        data = null;
        testCode = () => audio.Play(data, AudioPlayMode.Background);
        testCode.Should().Throw<ArgumentNullException>();
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
