// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // Each registered Clipboard format is an OS singleton,
                           // and we should not run this test at the same time as other tests using the same format.
public partial class ClipboardTests
{
    [WinFormsFact]
    public void Clipboard_SetText_InvokeString_GetReturnsExpected()
    {
        Clipboard.SetText("text");

        Clipboard.GetText().Should().Be("text");
        Clipboard.ContainsText().Should().BeTrue();
    }

    [WinFormsFact]
    public void Clipboard_SetData_CustomFormat_Color()
    {
        string format = nameof(Clipboard_SetData_CustomFormat_Color);
        Clipboard.SetData(format, Color.Black);

        Clipboard.ContainsData(format).Should().BeTrue();
        Clipboard.GetData(format).Should().Be(Color.Black);
    }
}
