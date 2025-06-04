// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.Devices.Tests;

public class KeyboardTests
{
    [Fact]
    public void Properties()
    {
        Keyboard keyboard = new();
        _ = keyboard.ShiftKeyDown;
        _ = keyboard.AltKeyDown;
        _ = keyboard.CtrlKeyDown;
        _ = keyboard.CapsLock;
        _ = keyboard.NumLock;
        _ = keyboard.ScrollLock;
    }

    // Not tested:
    //    Public Sub SendKeys(keys As String)
    //    Public Sub SendKeys(keys As String, wait As Boolean)
}
