// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;
using static Interop;
using static Interop.Kernel32;
using static Interop.Ole32;
using static Interop.Oleaut32;

namespace System.Windows.Forms.Tests.Interop.Oleaut32
{
    public unsafe class VARIANTTests : IClassFixture<ThreadExceptionFixture>
    {
        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public void VARIANT_Sizeof_InvokeX86_ReturnsExpected()
        {
            Assert.Equal(16, Marshal.SizeOf<VARIANT>());
            Assert.Equal(16, sizeof(VARIANT));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public void VARIANT_Sizeof_InvokeX64_ReturnsExpected()
        {
            Assert.Equal(24, Marshal.SizeOf<VARIANT>());
            Assert.Equal(24, sizeof(VARIANT));
        }

        [WinFormsTheory]
        [InlineData((ushort)VARENUM.EMPTY, false)]
        [InlineData((ushort)VARENUM.BOOL, false)]
        [InlineData((ushort)(VARENUM.BYREF), true)]
        [InlineData((ushort)(VARENUM.BOOL | VARENUM.BYREF), true)]
        [InlineData((ushort)(VARENUM.BOOL | VARENUM.BYREF | VARENUM.ARRAY), true)]
        [InlineData((ushort)(VARENUM.BOOL | VARENUM.BYREF | VARENUM.VECTOR), true)]
        [InlineData((ushort)(VARENUM.BOOL | VARENUM.ARRAY), false)]
        [InlineData((ushort)(VARENUM.BOOL | VARENUM.VECTOR), false)]
        public void VARIANT_Byref_Get_ReturnsExpected(ushort vt, bool expected)
        {
            using var variant = new VARIANT
            {
                vt = (VARENUM)vt
            };
            Assert.Equal(expected, variant.Byref);
        }

        [WinFormsTheory]
        [InlineData((ushort)VARENUM.EMPTY, (ushort)VARENUM.EMPTY)]
        [InlineData((ushort)VARENUM.BOOL, (ushort)VARENUM.BOOL)]
        [InlineData((ushort)(VARENUM.BYREF), (ushort)VARENUM.EMPTY)]
        [InlineData((ushort)(VARENUM.BOOL | VARENUM.BYREF), (ushort)VARENUM.BOOL)]
        public void VARIANT_Type_Get_ReturnsExpected(ushort vt, ushort expected)
        {
            using var variant = new VARIANT
            {
                vt = (VARENUM)vt
            };
            Assert.Equal((VARENUM)expected, variant.Type);
        }

        [WinFormsTheory]
        [InlineData((ushort)VARENUM.EMPTY)]
        [InlineData((ushort)(VARENUM.EMPTY | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.UNKNOWN)]
        [InlineData((ushort)(VARENUM.UNKNOWN | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.DISPATCH)]
        [InlineData((ushort)(VARENUM.DISPATCH | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.BSTR)]
        [InlineData((ushort)(VARENUM.BSTR | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.BOOL)]
        [InlineData((ushort)(VARENUM.BOOL | VARENUM.BYREF))]
        public void VARIANT_Clear_InvokeDefault_Success(ushort vt)
        {
            using var variant = new VARIANT
            {
                vt = (VARENUM)vt
            };
            variant.Clear();
            Assert.Equal(VARENUM.EMPTY, variant.vt);
            Assert.Equal(IntPtr.Zero, variant.data1);
            Assert.Equal(IntPtr.Zero, variant.data2);
        }

        [WinFormsFact]
        public void VARIANT_Clear_InvokeCustom_Success()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BOOL,
                data1 = (IntPtr)1
            };
            variant.Clear();
            Assert.Equal(VARENUM.EMPTY, variant.vt);
            Assert.Equal(IntPtr.Zero, variant.data1);
            Assert.Equal(IntPtr.Zero, variant.data2);
        }

        [WinFormsFact]
        public void VARIANT_Clear_InvokeBSTR_Success()
        {
            IntPtr data = Marshal.StringToBSTR("abc");
            using var variant = new VARIANT
            {
                vt = VARENUM.BSTR,
                data1 = data
            };
            variant.Clear();
            Assert.Equal(VARENUM.EMPTY, variant.vt);
            Assert.Equal(IntPtr.Zero, variant.data1);
            Assert.Equal(IntPtr.Zero, variant.data2);
        }

        [WinFormsTheory]
        [InlineData((ushort)VARENUM.EMPTY)]
        [InlineData((ushort)(VARENUM.EMPTY | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.UNKNOWN)]
        [InlineData((ushort)(VARENUM.UNKNOWN | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.DISPATCH)]
        [InlineData((ushort)(VARENUM.DISPATCH | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.BSTR)]
        [InlineData((ushort)(VARENUM.BSTR | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.BOOL)]
        [InlineData((ushort)(VARENUM.BOOL | VARENUM.BYREF))]
        public void VARIANT_Dispose_InvokeDefault_Success(ushort vt)
        {
            using var variant = new VARIANT
            {
                vt = (VARENUM)vt
            };
            variant.Dispose();
            Assert.Equal(VARENUM.EMPTY, variant.vt);
            Assert.Equal(IntPtr.Zero, variant.data1);
            Assert.Equal(IntPtr.Zero, variant.data2);
        }

        [WinFormsFact]
        public void VARIANT_Dispose_InvokeCustom_Success()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BOOL,
                data1 = (IntPtr)1
            };
            variant.Dispose();
            Assert.Equal(VARENUM.EMPTY, variant.vt);
            Assert.Equal(IntPtr.Zero, variant.data1);
            Assert.Equal(IntPtr.Zero, variant.data2);
        }

        [WinFormsFact]
        public void VARIANT_Dispose_InvokeBSTR_Success()
        {
            IntPtr data = Marshal.StringToBSTR("abc");
            using var variant = new VARIANT
            {
                vt = VARENUM.BSTR,
                data1 = data
            };
            variant.Dispose();
            Assert.Equal(VARENUM.EMPTY, variant.vt);
            Assert.Equal(IntPtr.Zero, variant.data1);
            Assert.Equal(IntPtr.Zero, variant.data2);
        }
    }
}
