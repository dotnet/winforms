// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Tests;

public partial class ToolTipBufferTests
{
    [WinFormsFact]
    public void ToolTipBuffer_Buffer_GetEmpty_ReturnsZero()
    {
        ToolTipBuffer buffer = default;
        Assert.Equal(IntPtr.Zero, buffer.Buffer);
    }

    [WinFormsFact]
    public void ToolTipBuffer_SetText_NoBuffer_Success()
    {
        ToolTipBuffer buffer = default;
        IntPtr memory1 = buffer.Buffer;
        Assert.Null(Marshal.PtrToStringUni(memory1));

        buffer.SetText("text");
        IntPtr memory2 = buffer.Buffer;
        Assert.NotEqual(memory1, memory2);
        Assert.Equal("text", Marshal.PtrToStringUni(memory2));
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    public void ToolTipBuffer_SetText_EmptyNoBuffer_Success(string empty)
    {
        ToolTipBuffer buffer = default;
        IntPtr memory1 = buffer.Buffer;
        Assert.Null(Marshal.PtrToStringUni(memory1));

        buffer.SetText(empty);
        IntPtr memory2 = buffer.Buffer;
        Assert.NotEqual(memory1, memory2);
        Assert.Empty(Marshal.PtrToStringUni(memory2));
    }

    [WinFormsTheory]
    [InlineData("abcde")]
    [InlineData("abcdef")]
    public void ToolTipBuffer_SetTextLonger_Success(string longer)
    {
        ToolTipBuffer buffer = default;
        buffer.SetText("text");
        Assert.Equal("text", Marshal.PtrToStringUni(buffer.Buffer));

        // Set longer.
        buffer.SetText(longer);
        IntPtr memory2 = buffer.Buffer;
        Assert.NotEqual(IntPtr.Zero, memory2);
        Assert.Equal(longer, Marshal.PtrToStringUni(memory2));
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData("ab", "ab")]
    [InlineData("abc", "abc")]
    public void ToolTipBuffer_SetTextShorter_Success(string shorter, string expected)
    {
        ToolTipBuffer buffer = default;
        buffer.SetText("text");
        IntPtr memory1 = buffer.Buffer;
        Assert.Equal("text", Marshal.PtrToStringUni(buffer.Buffer));

        // Set longer.
        buffer.SetText(shorter);
        IntPtr memory2 = buffer.Buffer;
        Assert.Equal(memory1, memory2);
        Assert.Equal(expected, Marshal.PtrToStringUni(memory2));
    }

    [WinFormsTheory]
    [InlineData("text")]
    [InlineData("abcd")]
    public void ToolTipBuffer_SetTextSameLength_Success(string sameLength)
    {
        ToolTipBuffer buffer = default;
        buffer.SetText("text");
        IntPtr memory1 = buffer.Buffer;
        Assert.Equal("text", Marshal.PtrToStringUni(buffer.Buffer));

        // Set longer.
        buffer.SetText(sameLength);
        IntPtr memory2 = buffer.Buffer;
        Assert.Equal(memory1, memory2);
        Assert.Equal(sameLength, Marshal.PtrToStringUni(memory2));
    }
}
