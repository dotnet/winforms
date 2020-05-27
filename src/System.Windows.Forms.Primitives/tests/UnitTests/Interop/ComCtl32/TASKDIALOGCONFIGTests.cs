// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using static Interop.ComCtl32;

namespace System.Windows.Forms.Primitives.Tests.Interop.ComCtl32
{
    public class TASKDIALOGCONFIGTests
    {
        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void TASKDIALOGCONFIG_x32_Size()
        {
            Assert.Equal(96, sizeof(TASKDIALOGCONFIG));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void TASKDIALOGCONFIG_x32_ensure_layout()
        {
            TASKDIALOGCONFIG sut = new TASKDIALOGCONFIG();
            byte* addr = (byte*)&sut;

            Assert.Equal(0, (byte*)&sut.cbSize - addr);                   // 4, UINT
            Assert.Equal(4, (byte*)&sut.hwndParent - addr);               // 4, HWND
            Assert.Equal(8, (byte*)&sut.hInstance - addr);                // 4, HINSTANCE
            Assert.Equal(12, (byte*)&sut.dwFlags - addr);                 // 4, TASKDIALOG_FLAGS
            Assert.Equal(16, (byte*)&sut.dwCommonButtons - addr);         // 4, TASKDIALOG_COMMON_BUTTON_FLAGS
            Assert.Equal(20, (byte*)&sut.pszWindowTitle - addr);          // 4, PCWSTR
            Assert.Equal(24, (byte*)&sut.mainIcon - addr);                // 4, union { HICON; PCWSTR; }
            Assert.Equal(28, (byte*)&sut.pszMainInstruction - addr);      // 4, PCWSTR
            Assert.Equal(32, (byte*)&sut.pszContent - addr);              // 4, PCWSTR
            Assert.Equal(36, (byte*)&sut.cButtons - addr);                // 4, UINT
            Assert.Equal(40, (byte*)&sut.pButtons - addr);                // 4, const TASKDIALOG_BUTTON *
            Assert.Equal(44, (byte*)&sut.nDefaultButton - addr);          // 4, int
            Assert.Equal(48, (byte*)&sut.cRadioButtons - addr);           // 4, UINT
            Assert.Equal(52, (byte*)&sut.pRadioButtons - addr);           // 4, const TASKDIALOG_BUTTON *
            Assert.Equal(56, (byte*)&sut.nDefaultRadioButton - addr);     // 4, int
            Assert.Equal(60, (byte*)&sut.pszVerificationText - addr);     // 4, PCWSTR
            Assert.Equal(64, (byte*)&sut.pszExpandedInformation - addr);  // 4, PCWSTR
            Assert.Equal(68, (byte*)&sut.pszExpandedControlText - addr);  // 4, PCWSTR
            Assert.Equal(72, (byte*)&sut.pszCollapsedControlText - addr); // 4, PCWSTR
            Assert.Equal(76, (byte*)&sut.footerIcon - addr);              // 4, union { HICON; PCWSTR; }
            Assert.Equal(80, (byte*)&sut.pszFooter - addr);               // 4, PCWSTR
            Assert.Equal(84, (byte*)&sut.pfCallback - addr);              // 4, PFTASKDIALOGCALLBACK
            Assert.Equal(88, (byte*)&sut.lpCallbackData - addr);          // 4, LONG_PTR
            Assert.Equal(92, (byte*)&sut.cxWidth - addr);                 // 4, UINT
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void TASKDIALOGCONFIG_x64_Size()
        {
            Assert.Equal(160, sizeof(TASKDIALOGCONFIG));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void TASKDIALOGCONFIG_x64_ensure_layout()
        {
            TASKDIALOGCONFIG sut = new TASKDIALOGCONFIG();
            byte* addr = (byte*)&sut;

            Assert.Equal(0, (byte*)&sut.cbSize - addr);                    // 4, UINT
            Assert.Equal(4, (byte*)&sut.hwndParent - addr);                // 8, HWND
            Assert.Equal(12, (byte*)&sut.hInstance - addr);                // 8, HINSTANCE
            Assert.Equal(20, (byte*)&sut.dwFlags - addr);                  // 4, TASKDIALOG_FLAGS
            Assert.Equal(24, (byte*)&sut.dwCommonButtons - addr);          // 4, TASKDIALOG_COMMON_BUTTON_FLAGS
            Assert.Equal(28, (byte*)&sut.pszWindowTitle - addr);           // 8, PCWSTR
            Assert.Equal(36, (byte*)&sut.mainIcon - addr);                 // 8, union { HICON; PCWSTR; }
            Assert.Equal(44, (byte*)&sut.pszMainInstruction - addr);       // 8, PCWSTR
            Assert.Equal(52, (byte*)&sut.pszContent - addr);               // 8, PCWSTR
            Assert.Equal(60, (byte*)&sut.cButtons - addr);                 // 4, UINT
            Assert.Equal(64, (byte*)&sut.pButtons - addr);                 // 8, const TASKDIALOG_BUTTON *
            Assert.Equal(72, (byte*)&sut.nDefaultButton - addr);           // 4, int
            Assert.Equal(76, (byte*)&sut.cRadioButtons - addr);            // 4, UINT
            Assert.Equal(80, (byte*)&sut.pRadioButtons - addr);            // 8, const TASKDIALOG_BUTTON *
            Assert.Equal(88, (byte*)&sut.nDefaultRadioButton - addr);      // 4, int
            Assert.Equal(92, (byte*)&sut.pszVerificationText - addr);      // 8, PCWSTR
            Assert.Equal(100, (byte*)&sut.pszExpandedInformation - addr);  // 8, PCWSTR
            Assert.Equal(108, (byte*)&sut.pszExpandedControlText - addr);  // 8, PCWSTR
            Assert.Equal(116, (byte*)&sut.pszCollapsedControlText - addr); // 8, PCWSTR
            Assert.Equal(124, (byte*)&sut.footerIcon - addr);              // 8, union { HICON; PCWSTR; }
            Assert.Equal(132, (byte*)&sut.pszFooter - addr);               // 8, PCWSTR
            Assert.Equal(140, (byte*)&sut.pfCallback - addr);              // 8, PFTASKDIALOGCALLBACK
            Assert.Equal(148, (byte*)&sut.lpCallbackData - addr);          // 8, LONG_PTR
            Assert.Equal(156, (byte*)&sut.cxWidth - addr);                 // 4, UINT
        }
    }
}
