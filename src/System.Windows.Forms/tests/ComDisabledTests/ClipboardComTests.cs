// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // Each registered Clipboard format is an OS singleton,
                           // and we should not run this test at the same time as other tests using the same format.
[UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
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
