// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Media;

namespace Microsoft.VisualBasic.Devices.Tests;

public class AudioTests
{
    private static string GetUniqueName() => Guid.NewGuid().ToString("D");

    [Fact]
    public void Play()
    {
        Audio audio = new();
        Action testCode = () => audio.Play(null);
        testCode.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InvalidEnumData<AudioPlayMode>]
    public void PlayBytesWithPlayMode_Throws(AudioPlayMode audioPlayMode)
    {
        Audio audio = new();
        Action testCode = () => audio.Play(([0]), audioPlayMode);
        testCode.Should().Throw<InvalidEnumArgumentException>();
    }

    [Theory]
    [NullAndEmptyStringData]
    public void PlayEmptyFileName_Throws(string fileName)
    {
        Audio audio = new();
        Action testCode = () => audio.Play(fileName);
        testCode.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [NullAndEmptyStringData]
    public void PlayEmptyFileNameAudioMode_Throws(string fileName)
    {
        Audio audio = new();
        Action testCode = () => audio.Play(fileName, AudioPlayMode.Background);
        testCode.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InvalidEnumData<AudioPlayMode>]
    public void PlayModeInvalid_Throws(AudioPlayMode audioPlayMode)
    {
        string location = Path.Combine(Path.GetTempPath(), GetUniqueName());
        Audio audio = new();
        Action testCode = () => audio.Play(location, audioPlayMode);
        testCode.Should().Throw<InvalidEnumArgumentException>();
    }

    [Theory]
    [EnumData<AudioPlayMode>]
    public void PlayModesAll_Throws(AudioPlayMode mode)
    {
        Audio audio = new();
        Action testCode = () => audio.Play((string)null, mode);
        testCode.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PlayStream_Throws()
    {
        Audio audio = new();
        Action testCode = () => audio.Play((MemoryStream)null, AudioPlayMode.Background);
        testCode.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PlaySystemSound_Throws()
    {
        Audio audio = new();
        Action testCode = () => audio.PlaySystemSound((SystemSound)null);
        testCode.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Stop()
    {
        Audio audio = new();
        Action testCode = audio.Stop;
        testCode.Should().NotThrow();
    }
}
