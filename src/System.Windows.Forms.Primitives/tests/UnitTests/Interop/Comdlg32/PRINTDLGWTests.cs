// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.Comdlg32;

namespace System.Windows.Forms.Primitives.Tests.Interop.Comdlg32
{
    public class PRINTDLGWTests
    {
        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void PRINTDLGW_32_Size()
        {
            Assert.Equal(66, sizeof(PRINTDLGW_32));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void PRINTDLGW_32_ensure_layout()
        {
            PRINTDLGW_32 sut = new PRINTDLGW_32();
            byte* addr = (byte*)&sut;

            Assert.Equal(0, (byte*)&sut._lStructSize - addr);           // 4, DWORD
            Assert.Equal(4, (byte*)&sut._hwndOwner - addr);             // 4, HWND
            Assert.Equal(8, (byte*)&sut._hDevMode - addr);              // 4, HGLOBAL
            Assert.Equal(12, (byte*)&sut._hDevNames - addr);            // 4, HGLOBAL
            Assert.Equal(16, (byte*)&sut._hDC - addr);                  // 4, HDC
            Assert.Equal(20, (byte*)&sut._flags - addr);                // 4, DWORD
            Assert.Equal(24, (byte*)&sut._nFromPage - addr);            // 2, WORD
            Assert.Equal(26, (byte*)&sut._nToPage - addr);              // 2, WORD
            Assert.Equal(28, (byte*)&sut._nMinPage - addr);             // 2, WORD
            Assert.Equal(30, (byte*)&sut._nMaxPage - addr);             // 2, WORD
            Assert.Equal(32, (byte*)&sut._nCopies - addr);              // 2, WORD
            Assert.Equal(34, (byte*)&sut._hInstance - addr);            // 4, HINSTANCE
            Assert.Equal(38, (byte*)&sut._lCustData - addr);            // 4, LPARAM
            Assert.Equal(42, (byte*)&sut._lpfnPrintHook - addr);        // 4, LPPRINTHOOKPROC
            Assert.Equal(46, (byte*)&sut._lpfnSetupHook - addr);        // 4, LPSETUPHOOKPROC
            Assert.Equal(50, (byte*)&sut._lpPrintTemplateName - addr);  // 4, LPCWSTR
            Assert.Equal(54, (byte*)&sut._lpSetupTemplateName - addr);  // 4, LPCWSTR
            Assert.Equal(58, (byte*)&sut._hPrintTemplate - addr);       // 4, HGLOBAL
            Assert.Equal(62, (byte*)&sut._hSetupTemplate - addr);       // 4, HGLOBAL
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void PRINTDLGW_64_Size()
        {
            Assert.Equal(120, sizeof(PRINTDLGW_64));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void PRINTDLGW_64_ensure_layout()
        {
            PRINTDLGW_64 sut = new PRINTDLGW_64();
            byte* addr = (byte*)&sut;

            Assert.Equal(0, (byte*)&sut._lStructSize - addr);           // 8, DWORD
            Assert.Equal(8, (byte*)&sut._hwndOwner - addr);             // 8, HWND
            Assert.Equal(16, (byte*)&sut._hDevMode - addr);             // 8, HGLOBAL
            Assert.Equal(24, (byte*)&sut._hDevNames - addr);            // 8, HGLOBAL
            Assert.Equal(32, (byte*)&sut._hDC - addr);                  // 8, HDC
            Assert.Equal(40, (byte*)&sut._flags - addr);                // 8, DWORD
            Assert.Equal(44, (byte*)&sut._nFromPage - addr);            // 2, WORD
            Assert.Equal(46, (byte*)&sut._nToPage - addr);              // 2, WORD
            Assert.Equal(48, (byte*)&sut._nMinPage - addr);             // 2, WORD
            Assert.Equal(50, (byte*)&sut._nMaxPage - addr);             // 2, WORD
            Assert.Equal(52, (byte*)&sut._nCopies - addr);              // 2, WORD
            // 2 bytes alignment 54 -> 56
            Assert.Equal(56, (byte*)&sut._hInstance - addr);            // 8, HINSTANCE
            Assert.Equal(64, (byte*)&sut._lCustData - addr);            // 8, LPARAM
            Assert.Equal(72, (byte*)&sut._lpfnPrintHook - addr);        // 8, LPPRINTHOOKPROC
            Assert.Equal(80, (byte*)&sut._lpfnSetupHook - addr);        // 8, LPSETUPHOOKPROC
            Assert.Equal(88, (byte*)&sut._lpPrintTemplateName - addr);  // 8, LPCWSTR
            Assert.Equal(96, (byte*)&sut._lpSetupTemplateName - addr);  // 8, LPCWSTR
            Assert.Equal(104, (byte*)&sut._hPrintTemplate - addr);      // 8, HGLOBAL
            Assert.Equal(112, (byte*)&sut._hSetupTemplate - addr);      // 8, HGLOBAL
        }
    }
}
