// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Controls.Dialogs;

namespace System.Windows.Forms.Primitives.Tests.Interop.Comdlg32;

public class PRINTDLGWTests
{
    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
    public unsafe void PRINTDLGW_32_Size()
    {
        if (Environment.Is64BitProcess)
        {
            return;
        }

        Assert.Equal(66, sizeof(PRINTDLGW_32));
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
    public unsafe void PRINTDLGW_32_ensure_layout()
    {
        if (Environment.Is64BitProcess)
        {
            return;
        }

        PRINTDLGW_32 sut = default;
        byte* addr = (byte*)&sut;

        Assert.Equal(0, (byte*)&sut.lStructSize - addr);           // 4, DWORD
        Assert.Equal(4, (byte*)&sut.hwndOwner - addr);             // 4, HWND
        Assert.Equal(8, (byte*)&sut.hDevMode - addr);              // 4, HGLOBAL
        Assert.Equal(12, (byte*)&sut.hDevNames - addr);            // 4, HGLOBAL
        Assert.Equal(16, (byte*)&sut.hDC - addr);                  // 4, HDC
        Assert.Equal(20, (byte*)&sut.Flags - addr);                // 4, DWORD
        Assert.Equal(24, (byte*)&sut.nFromPage - addr);            // 2, WORD
        Assert.Equal(26, (byte*)&sut.nToPage - addr);              // 2, WORD
        Assert.Equal(28, (byte*)&sut.nMinPage - addr);             // 2, WORD
        Assert.Equal(30, (byte*)&sut.nMaxPage - addr);             // 2, WORD
        Assert.Equal(32, (byte*)&sut.nCopies - addr);              // 2, WORD
        Assert.Equal(34, (byte*)&sut.hInstance - addr);            // 4, HINSTANCE
        Assert.Equal(38, (byte*)&sut.lCustData - addr);            // 4, LPARAM
        Assert.Equal(42, (byte*)&sut.lpfnPrintHook - addr);        // 4, LPPRINTHOOKPROC
        Assert.Equal(46, (byte*)&sut.lpfnSetupHook - addr);        // 4, LPSETUPHOOKPROC
        Assert.Equal(50, (byte*)&sut.lpPrintTemplateName - addr);  // 4, LPCWSTR
        Assert.Equal(54, (byte*)&sut.lpSetupTemplateName - addr);  // 4, LPCWSTR
        Assert.Equal(58, (byte*)&sut.hPrintTemplate - addr);       // 4, HGLOBAL
        Assert.Equal(62, (byte*)&sut.hSetupTemplate - addr);       // 4, HGLOBAL
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
    public unsafe void PRINTDLGW_64_Size()
    {
        if (!Environment.Is64BitProcess)
        {
            return;
        }

        Assert.Equal(120, sizeof(PRINTDLGW_64));
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
    public unsafe void PRINTDLGW_64_ensure_layout()
    {
        if (!Environment.Is64BitProcess)
        {
            return;
        }

        PRINTDLGW_64 sut = default;
        byte* addr = (byte*)&sut;

        Assert.Equal(0, (byte*)&sut.lStructSize - addr);           // 8, DWORD
        Assert.Equal(8, (byte*)&sut.hwndOwner - addr);             // 8, HWND
        Assert.Equal(16, (byte*)&sut.hDevMode - addr);             // 8, HGLOBAL
        Assert.Equal(24, (byte*)&sut.hDevNames - addr);            // 8, HGLOBAL
        Assert.Equal(32, (byte*)&sut.hDC - addr);                  // 8, HDC
        Assert.Equal(40, (byte*)&sut.Flags - addr);                // 8, DWORD
        Assert.Equal(44, (byte*)&sut.nFromPage - addr);            // 2, WORD
        Assert.Equal(46, (byte*)&sut.nToPage - addr);              // 2, WORD
        Assert.Equal(48, (byte*)&sut.nMinPage - addr);             // 2, WORD
        Assert.Equal(50, (byte*)&sut.nMaxPage - addr);             // 2, WORD
        Assert.Equal(52, (byte*)&sut.nCopies - addr);              // 2, WORD
        // 2 bytes alignment 54 -> 56
        Assert.Equal(56, (byte*)&sut.hInstance - addr);            // 8, HINSTANCE
        Assert.Equal(64, (byte*)&sut.lCustData - addr);            // 8, LPARAM
        Assert.Equal(72, (byte*)&sut.lpfnPrintHook - addr);        // 8, LPPRINTHOOKPROC
        Assert.Equal(80, (byte*)&sut.lpfnSetupHook - addr);        // 8, LPSETUPHOOKPROC
        Assert.Equal(88, (byte*)&sut.lpPrintTemplateName - addr);  // 8, LPCWSTR
        Assert.Equal(96, (byte*)&sut.lpSetupTemplateName - addr);  // 8, LPCWSTR
        Assert.Equal(104, (byte*)&sut.hPrintTemplate - addr);      // 8, HGLOBAL
        Assert.Equal(112, (byte*)&sut.hSetupTemplate - addr);      // 8, HGLOBAL
    }
}
