// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Tests;

// These tests ensure that Clipboard works when built-in COM is disabled,
// which is the case in trimming scenarios
public class ClipboardComTests
{
    [WinFormsFact]
    public void Clipboard_SetText_InvokeString_GetReturnsExpected()
    {
        Clipboard.SetText("text");

        Clipboard.GetText().Should().Be("text");
        Clipboard.ContainsText().Should().BeTrue();
    }
}
