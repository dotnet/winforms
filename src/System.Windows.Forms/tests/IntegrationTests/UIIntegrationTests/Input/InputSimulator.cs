// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
}
