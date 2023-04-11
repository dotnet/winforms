// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Xunit;

namespace System.Windows.Forms.UITests.Input;

internal class InputSimulator
{
    public InputSimulator()
    {
        Keyboard = new KeyboardSimulator(this);
        Mouse = new MouseSimulator(this);
    }

    public KeyboardSimulator Keyboard { get; }

    public MouseSimulator Mouse { get; }

    internal void SendInput(Span<INPUT> inputs)
    {
        Assert.Equal((uint)inputs.Length, PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>()));
    }
}
