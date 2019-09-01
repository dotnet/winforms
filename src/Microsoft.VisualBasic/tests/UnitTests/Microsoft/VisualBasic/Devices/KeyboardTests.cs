// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace Microsoft.VisualBasic.Devices.Tests
{
    public class KeyboardTests
    {
        [Fact]
        public void Properties()
        {
            var keyboard = new Keyboard();
            _ = keyboard.ShiftKeyDown;
            _ = keyboard.AltKeyDown;
            _ = keyboard.CtrlKeyDown;
            _ = keyboard.CapsLock;
            _ = keyboard.NumLock;
            _ = keyboard.ScrollLock;
        }

        // Not tested:
        //    Public Sub SendKeys(ByVal keys As String)
        //    Public Sub SendKeys(ByVal keys As String, ByVal wait As Boolean)
    }
}
