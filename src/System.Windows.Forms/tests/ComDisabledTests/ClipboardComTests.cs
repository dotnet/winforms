// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")]
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
        using BinaryFormatterInClipboardScope scope = new(enable: true);
        Clipboard.GetData(format).Should().Be(Color.Black);
    }

    [WinFormsFact]
    public void Clipboard_SetData_CustomFormat_Color_Fail()
    {
        string format = nameof(Clipboard_SetData_CustomFormat_Color);
        Clipboard.SetData(format, Color.Black);

        using BinaryFormatterInClipboardScope scope = new(enable: false);
        // GetData does not resolve system.Object type.
        ((Action)(() => Clipboard.GetData(format))).Should().Throw<NotSupportedException>();
    }
}
