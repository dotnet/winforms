// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

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

    [WinFormsFact]
    public void ClipBoard_SetData_CustomFormat_Color()
    {
        string format = nameof(ClipBoard_SetData_CustomFormat_Color);
        Clipboard.SetData(format, Color.Black);
        Assert.True(Clipboard.ContainsData(format));
        Assert.Equal(Color.Black, Clipboard.GetData(format));
    }
}
