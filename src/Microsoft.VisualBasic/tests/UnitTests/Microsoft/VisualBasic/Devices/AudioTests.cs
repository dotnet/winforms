// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.Devices.Tests;

public class AudioTests
{
    [Fact]
    public void Play()
    {
        string location = Path.Combine(Path.GetTempPath(), GetUniqueName());
        Audio audio = new();
        Assert.Throws<FileNotFoundException>(() => audio.Play(location));
    }

    // Not tested:
    //    Public Sub PlaySystemSound(ByVal systemSound As System.Media.SystemSound)

    [Fact]
    public void Stop()
    {
        Audio audio = new();
        audio.Stop();
    }

    private static string GetUniqueName() => Guid.NewGuid().ToString("D");
}
