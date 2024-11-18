// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.Devices.Tests;

public class ComputerTests
{
    [Fact]
    // This test does not modify the system clipboard state, do not move it into the
    // sequential collection, it is safe to run in parallel with other tests in this assembly.
    public void Properties()
    {
        Computer computer = new();

        var audio = computer.Audio;
        Assert.NotNull(audio);
        Assert.Same(audio, computer.Audio);

        var clipboard = computer.Clipboard;
        Assert.NotNull(clipboard);
        Assert.Same(clipboard, computer.Clipboard);

        var keyboard = computer.Keyboard;
        Assert.NotNull(keyboard);
        Assert.Same(keyboard, computer.Keyboard);

        var mouse = computer.Mouse;
        Assert.NotNull(mouse);
        Assert.Same(mouse, computer.Mouse);
    }

    [Fact]
    public void Screen()
    {
        Computer computer = new();
        Assert.Equal(System.Windows.Forms.Screen.PrimaryScreen, computer.Screen);
    }
}
