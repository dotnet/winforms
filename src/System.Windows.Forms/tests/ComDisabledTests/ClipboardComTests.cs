// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public partial class ClipboardTests
{
    [WinFormsFact]
    public void Clipboard_SetText_InvokeString_GetReturnsExpected()
    {
        Clipboard.SetText("text");
        Assert.Equal("text", Clipboard.GetText());
        Assert.True(Clipboard.ContainsText());
    }
}
