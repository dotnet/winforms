// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using static Interop.Richedit;

namespace System.Windows.Forms.Interop.Tests;

public class RichTextBoxTests
{
    public const string NativeTests = "NativeTests";

    [Fact]
    public static void RichTextBox_Enlink_Test()
    {
        RichTextBox_Enlink(out var enlink);

        Assert.Equal(132, enlink.nmhdr.code);
        Assert.Equal((IntPtr)765, enlink.nmhdr.hwndFrom);
        Assert.Equal((IntPtr)432, enlink.nmhdr.idFrom);
        Assert.Equal(22, enlink.msg);
        Assert.Equal((nuint)6578, enlink.wParam);
        Assert.Equal(54425, enlink.lParam);
        Assert.Equal(109, enlink.charrange.cpMin);
        Assert.Equal(1577, enlink.charrange.cpMax);
    }

    [Fact]
    public static void RichTextBox_Enprotected_Test()
    {
        RichTextBox_Enprotected(out var enprotected);

        Assert.Equal(132, enprotected.nmhdr.code);
        Assert.Equal((IntPtr)765, enprotected.nmhdr.hwndFrom);
        Assert.Equal((IntPtr)432, enprotected.nmhdr.idFrom);
        Assert.Equal(22, enprotected.msg);
        Assert.Equal((nuint)6578, enprotected.wParam);
        Assert.Equal(54425, enprotected.lParam);
        Assert.Equal(109, enprotected.chrg.cpMin);
        Assert.Equal(1577, enprotected.chrg.cpMax);
    }

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_Enlink(out ENLINK enlink);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_Enprotected(out ENPROTECTED enprotected);
}
