// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using static Interop.ComCtl32;

namespace System.Windows.Forms.Primitives.Tests.Interop.ComCtl32
{
    public class TASKDIALOGCONFIGIconUnionTests
    {
        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void TASKDIALOGCONFIGIconUnion_x32_Size()
        {
            Assert.Equal(4, sizeof(TASKDIALOGCONFIG.IconUnion));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void TASKDIALOGCONFIGIconUnion_x32_ensure_layout()
        {
            TASKDIALOGCONFIG.IconUnion sut = new TASKDIALOGCONFIG.IconUnion();
            byte* addr = (byte*)&sut;

            Assert.Equal(0, (byte*)&sut.hIcon - addr);  // 4, HICON
            Assert.Equal(0, (byte*)&sut.pszIcon - addr);  // 4, PCWSTR
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void TASKDIALOGCONFIGIconUnion_x64_Size()
        {
            Assert.Equal(8, sizeof(TASKDIALOGCONFIG.IconUnion));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void TASKDIALOGCONFIGIconUnion_x64_ensure_layout()
        {
            TASKDIALOGCONFIG.IconUnion sut = new TASKDIALOGCONFIG.IconUnion();
            byte* addr = (byte*)&sut;

            Assert.Equal(0, (byte*)&sut.hIcon - addr);  // 8, HICON
            Assert.Equal(0, (byte*)&sut.pszIcon - addr);  // 8, PCWSTR
        }
    }
}
