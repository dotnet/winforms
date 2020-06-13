// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using static Interop.ComCtl32;

namespace System.Windows.Forms.Primitives.Tests.Interop.ComCtl32
{
    public class MCGRIDINFOTests
    {
        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void MCGRIDINFO_x32_Size()
        {
            Assert.Equal(84, sizeof(MCGRIDINFO));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void MCGRIDINFO_x32_Marshal_Size()
        {
            Assert.Equal(84, Marshal.SizeOf<MCGRIDINFO>());
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void MCGRIDINFO_x32_ensure_layout()
        {
            MCGRIDINFO sut = new MCGRIDINFO();
            byte* addr = (byte*)&sut;

            Assert.Equal(0, (byte*)&sut.cbSize - addr);          // 4, UINT
            Assert.Equal(4, (byte*)&sut.dwPart - addr);          // 4, DWORD
            Assert.Equal(8, (byte*)&sut.dwFlags - addr);         // 4, DWORD
            Assert.Equal(12, (byte*)&sut.iCalendar - addr);      // 4, int
            Assert.Equal(16, (byte*)&sut.iRow - addr);           // 4, int
            Assert.Equal(20, (byte*)&sut.iCol - addr);           // 4, int
            Assert.Equal(24, (byte*)&sut.bSelected - addr);      // 4, BOOL
            Assert.Equal(28, (byte*)&sut.stStart - addr);        // 16, SYSTEMTIME
            Assert.Equal(44, (byte*)&sut.stEnd - addr);          // 16, SYSTEMTIME
            Assert.Equal(60, (byte*)&sut.rc - addr);             // 16, RECT
            Assert.Equal(76, (byte*)&sut.pszName - addr);        // 4, PWSTR
            Assert.Equal(80, (byte*)&sut.cchName - addr);        // 4, size_t
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public void MCGRIDINFO_x32_Marshal_OffsetOf_IsCorrect()
        {
            Assert.Equal(0, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.cbSize)));            // 4, UINT
            Assert.Equal(4, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.dwPart)));            // 4, DWORD
            Assert.Equal(8, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.dwFlags)));           // 4, DWORD
            Assert.Equal(12, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.iCalendar)));        // 4, int
            Assert.Equal(16, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.iRow)));             // 4, int
            Assert.Equal(20, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.iCol)));             // 4, int
            Assert.Equal(24, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.bSelected)));        // 4, BOOL
            Assert.Equal(28, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.stStart)));          // 16, SYSTEMTIME
            Assert.Equal(44, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.stEnd)));            // 16, SYSTEMTIME
            Assert.Equal(60, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.rc)));               // 16, RECT
            Assert.Equal(76, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.pszName)));          // 8, PWSTR
            Assert.Equal(80, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.cchName)));          // 8, size_t
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void MCGRIDINFO_x64_Size()
        {
            Assert.Equal(96, sizeof(MCGRIDINFO));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public void MCGRIDINFO_x64_Marshal_Size()
        {
            Assert.Equal(96, Marshal.SizeOf<MCGRIDINFO>());
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void MCGRIDINFO_x64_ensure_layout()
        {
            MCGRIDINFO sut = new MCGRIDINFO();
            byte* addr = (byte*)&sut;

            Assert.Equal(0, (byte*)&sut.cbSize - addr);          // 4, UINT
            Assert.Equal(4, (byte*)&sut.dwPart - addr);          // 4, DWORD
            Assert.Equal(8, (byte*)&sut.dwFlags - addr);         // 4, DWORD
            Assert.Equal(12, (byte*)&sut.iCalendar - addr);      // 4, int
            Assert.Equal(16, (byte*)&sut.iRow - addr);           // 4, int
            Assert.Equal(20, (byte*)&sut.iCol - addr);           // 4, int
            Assert.Equal(24, (byte*)&sut.bSelected - addr);      // 4, BOOL
            Assert.Equal(28, (byte*)&sut.stStart - addr);        // 16, SYSTEMTIME
            Assert.Equal(44, (byte*)&sut.stEnd - addr);          // 16, SYSTEMTIME
            Assert.Equal(60, (byte*)&sut.rc - addr);             // 16, RECT
            // 4 bytes alignment 76 -> 80
            Assert.Equal(80, (byte*)&sut.pszName - addr);        // 8, PWSTR
            Assert.Equal(88, (byte*)&sut.cchName - addr);        // 8, size_t
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public void MCGRIDINFO_x64_Marshal_OffsetOf_IsCorrect()
        {
            Assert.Equal(0, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.cbSize)));            // 4, UINT
            Assert.Equal(4, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.dwPart)));            // 4, DWORD
            Assert.Equal(8, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.dwFlags)));           // 4, DWORD
            Assert.Equal(12, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.iCalendar)));        // 4, int
            Assert.Equal(16, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.iRow)));             // 4, int
            Assert.Equal(20, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.iCol)));             // 4, int
            Assert.Equal(24, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.bSelected)));        // 4, BOOL
            Assert.Equal(28, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.stStart)));          // 16, SYSTEMTIME
            Assert.Equal(44, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.stEnd)));            // 16, SYSTEMTIME
            Assert.Equal(60, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.rc)));               // 16, RECT
            // 4 bytes alignment 76 -> 80
            Assert.Equal(80, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.pszName)));          // 8, PWSTR
            Assert.Equal(88, (int)Marshal.OffsetOf<MCGRIDINFO>(nameof(MCGRIDINFO.cchName)));          // 8, size_t
        }
    }
}
