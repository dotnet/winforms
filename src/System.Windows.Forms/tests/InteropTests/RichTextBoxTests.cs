// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.RichEdit;
using static Interop.Richedit;

namespace System.Windows.Forms.Interop.Tests;

// Magic numbers in this file are coming from unmanaged part of the test
// at src/System.Windows.Forms/tests/InteropTests/NativeTests/RichTextBoxTests.cpp
public class RichTextBoxTests
{
    public const string NativeTests = "NativeTests";

    [Fact]
    public static void RichTextBox_EnLink_Test()
    {
        RichTextBox_EnLink(out var value);

        Assert.Equal(132u, value.nmhdr.code);
        Assert.Equal((HWND)(nint)765, value.nmhdr.hwndFrom);
        Assert.Equal(432u, value.nmhdr.idFrom);
        Assert.Equal(22, value.msg);
        Assert.Equal((nuint)6578, value.wParam);
        Assert.Equal(54425, value.lParam);
        Assert.Equal(109, value.charrange.cpMin);
        Assert.Equal(1577, value.charrange.cpMax);
    }

    [Fact]
    public static void RichTextBox_EnProtected_Test()
    {
        RichTextBox_EnProtected(out var value);

        Assert.Equal(132u, value.nmhdr.code);
        Assert.Equal((HWND)(nint)765, value.nmhdr.hwndFrom);
        Assert.Equal(432u, value.nmhdr.idFrom);
        Assert.Equal(22, value.msg);
        Assert.Equal((nuint)6578, value.wParam);
        Assert.Equal(54425, value.lParam);
        Assert.Equal(109, value.chrg.cpMin);
        Assert.Equal(1577, value.chrg.cpMax);
    }

    [Fact]
    public static void RichTextBox_EnDropFiles_Test()
    {
        RichTextBox_EnDropFiles(out var value);

        Assert.Equal(132u, value.nmhdr.code);
        Assert.Equal((HWND)(nint)765, value.nmhdr.hwndFrom);
        Assert.Equal(432u, value.nmhdr.idFrom);
        Assert.Equal(22, value.hDrop);
        Assert.Equal(6578, value.cp);
        Assert.Equal(BOOL.TRUE, value.fProtected);
    }

    [Fact]
    public static void RichTextBox_CharRange_Test()
    {
        RichTextBox_CharRange(out var value);

        Assert.Equal(109, value.cpMin);
        Assert.Equal(1577, value.cpMax);
    }

    [Fact]
    public static void RichTextBox_EditStream_Test()
    {
        RichTextBox_EditStream(out var value);

        Assert.Equal((UIntPtr)109, value.dwCookie);
        Assert.Equal(1577u, value.dwError);
        Assert.Equal(6578, value.pfnCallback);
    }

    [Fact]
    public static unsafe void RichTextBox_FindTextW_Test()
    {
        RichTextBox_FindTextW(out var value);

        Assert.Equal("Fine Text", Marshal.PtrToStringUni((IntPtr)value.lpstrText));
        Assert.Equal(109, value.chrg.cpMin);
        Assert.Equal(1577, value.chrg.cpMax);
    }

    [Fact]
    public static unsafe void RichTextBox_GetTextEx_Test()
    {
        RichTextBox_GetTextEx(out var value);

        Assert.Equal(132u, value.cb);
        Assert.Equal(GETTEXTEX_FLAGS.GT_RAWTEXT, value.flags);
        Assert.Equal(432u, value.codepage);
        Assert.Equal(22, value.lpDefaultChar);
        Assert.Equal(6578, value.lpUsedDefChar);
    }

    [Fact]
    public static unsafe void RichTextBox_GetTextLengthEx_Test()
    {
        RichTextBox_GetTextLengthEx(out var value);

        Assert.Equal(GETTEXTLENGTHEX_FLAGS.GTL_NUMBYTES, value.flags);
        Assert.Equal(432u, value.codepage);
    }

    [Fact]
    public static unsafe void RichTextBox_ParaFormat_Test()
    {
        RichTextBox_ParaFormat(out var value);

        Assert.Equal(132u, value.cbSize);
        Assert.Equal(PFM.ALIGNMENT, value.dwMask);
        Assert.Equal(PARAFORMAT_NUMBERING.PFN_UCROMAN, value.wNumbering);
        Assert.Equal(6578, value.wReserved);
        Assert.Equal(109, value.dxStartIndent);
        Assert.Equal(432, value.dxRightIndent);
        Assert.Equal(54425, value.dxOffset);
        Assert.Equal(PFA.JUSTIFY, value.wAlignment);
        Assert.Equal(6565, value.cTabCount);
        Assert.Equal(8989, value.rgxTabs[0]);
        Assert.Equal(812, value.rgxTabs[31]);
    }

    [Fact]
    public static void RichTextBox_ReqResize_Test()
    {
        RichTextBox_ReqResize(out var value);

        Assert.Equal(132u, value.nmhdr.code);
        Assert.Equal((HWND)(nint)765, value.nmhdr.hwndFrom);
        Assert.Equal(432u, value.nmhdr.idFrom);
        Assert.Equal(6578, value.rc.left);
        Assert.Equal(109, value.rc.right);
        Assert.Equal(54425, value.rc.top);
        Assert.Equal(8989, value.rc.bottom);
    }

    [Fact]
    public static void RichTextBox_SelChange_Test()
    {
        RichTextBox_SelChange(out var value);

        Assert.Equal(132u, value.nmhdr.code);
        Assert.Equal((HWND)(nint)765, value.nmhdr.hwndFrom);
        Assert.Equal(432u, value.nmhdr.idFrom);
        Assert.Equal(RICH_EDIT_GET_CONTEXT_MENU_SEL_TYPE.SEL_MULTICHAR, value.seltyp);
        Assert.Equal(109, value.chrg.cpMin);
        Assert.Equal(1577, value.chrg.cpMax);
    }

    [Fact]
    public static unsafe void RichTextBox_TextRange_Test()
    {
        RichTextBox_TextRange(out var value);

        Assert.Equal("Fine Text", Marshal.PtrToStringAnsi(value.lpstrText));
        Assert.Equal(109, value.chrg.cpMin);
        Assert.Equal(1577, value.chrg.cpMax);
    }

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_EnLink(out ENLINK enlink);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_EnProtected(out ENPROTECTED enprotected);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_EnDropFiles(out ENDROPFILES value);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_CharRange(out CHARRANGE value);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_EditStream(out EDITSTREAM value);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_FindTextW(out FINDTEXTW value);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_GetTextEx(out GETTEXTEX value);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_GetTextLengthEx(out GETTEXTLENGTHEX value);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_ParaFormat(out PARAFORMAT value);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_ReqResize(out REQRESIZE value);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_SelChange(out SELCHANGE value);

    [DllImport(NativeTests, PreserveSig = true)]
    private static extern void RichTextBox_TextRange(out TEXTRANGE value);
}
