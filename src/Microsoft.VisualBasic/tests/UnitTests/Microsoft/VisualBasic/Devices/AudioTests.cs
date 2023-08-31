// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.Devices.Tests;

public class AudioTests
{
    [Fact]
    public void Play()
    {
        var location = Path.Combine(Path.GetTempPath(), GetUniqueName());
        var audio = new Audio();
        Assert.Throws<FileNotFoundException>(() => audio.Play(location));
    }

    // Not tested:
    //    Public Sub PlaySystemSound(ByVal systemSound As System.Media.SystemSound)

    [Fact]
    public void Stop()
    {
        var audio = new Audio();
        audio.Stop();
    }

    private static string GetUniqueName() => Guid.NewGuid().ToString("D");
}
