// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
    public unsafe class VARIANTTests
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

        [StaTheory]
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

        [StaTheory]
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

        [StaTheory]
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
            Assert.Equal(IntPtr.Zero, variant.data.punkVal);
        }

        [StaFact]
        public void VARIANT_Clear_InvokeCustom_Success()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BOOL,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = (IntPtr)1,
                }
            };
            variant.Clear();
            Assert.Equal(VARENUM.EMPTY, variant.vt);
            Assert.Equal(IntPtr.Zero, variant.data.punkVal);
        }

        [StaFact]
        public void VARIANT_Clear_InvokeBSTR_Success()
        {
            IntPtr data = Marshal.StringToBSTR("abc");
            using var variant = new VARIANT
            {
                vt = VARENUM.BSTR,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            variant.Clear();
            Assert.Equal(VARENUM.EMPTY, variant.vt);
            Assert.Equal(IntPtr.Zero, variant.data.punkVal);
        }

        [StaTheory]
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
            Assert.Equal(IntPtr.Zero, variant.data.punkVal);
        }

        [StaFact]
        public void VARIANT_Dispose_InvokeCustom_Success()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BOOL,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = (IntPtr)1,
                }
            };
            variant.Dispose();
            Assert.Equal(VARENUM.EMPTY, variant.vt);
            Assert.Equal(IntPtr.Zero, variant.data.punkVal);
        }

        [StaFact]
        public void VARIANT_Dispose_InvokeBSTR_Success()
        {
            IntPtr data = Marshal.StringToBSTR("abc");
            using var variant = new VARIANT
            {
                vt = VARENUM.BSTR,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            variant.Dispose();
            Assert.Equal(VARENUM.EMPTY, variant.vt);
            Assert.Equal(IntPtr.Zero, variant.data.punkVal);
        }

        public static IEnumerable<object[]> ToObject_TestData()
        {
            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.I1, (IntPtr)long.MinValue, (sbyte)0 };
            }
            yield return new object[] { VARENUM.I1, (IntPtr)int.MinValue, (sbyte)0 };
            yield return new object[] { VARENUM.I1, (IntPtr)short.MinValue, (sbyte)0 };
            yield return new object[] { VARENUM.I1, (IntPtr)sbyte.MinValue, sbyte.MinValue };
            yield return new object[] { VARENUM.I1, (IntPtr)(-10), (sbyte)(-10) };
            yield return new object[] { VARENUM.I1, (IntPtr)0, (sbyte)0 };
            yield return new object[] { VARENUM.I1, (IntPtr)10, (sbyte)10 };
            yield return new object[] { VARENUM.I1, (IntPtr)sbyte.MaxValue, sbyte.MaxValue };
            yield return new object[] { VARENUM.I1, (IntPtr)short.MaxValue, (sbyte)(-1) };
            yield return new object[] { VARENUM.I1, (IntPtr)int.MaxValue, (sbyte)(-1) };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.I1, (IntPtr)long.MaxValue, (sbyte)(-1) };
            }

            yield return new object[] { VARENUM.UI1, (IntPtr)(-10), (byte)246 };
            yield return new object[] { VARENUM.UI1, (IntPtr)0, (byte)0 };
            yield return new object[] { VARENUM.UI1, (IntPtr)10, (byte)10 };
            yield return new object[] { VARENUM.UI1, (IntPtr)byte.MaxValue, byte.MaxValue };
            yield return new object[] { VARENUM.UI1, (IntPtr)ushort.MaxValue, byte.MaxValue };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.UI1, (IntPtr)uint.MaxValue, byte.MaxValue };
            }
            yield return new object[] { VARENUM.UI1, (IntPtr)(-1), byte.MaxValue };

            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.I2, (IntPtr)long.MinValue, (short)0 };
            }
            yield return new object[] { VARENUM.I2, (IntPtr)int.MinValue, (short)0 };
            yield return new object[] { VARENUM.I2, (IntPtr)short.MinValue, short.MinValue };
            yield return new object[] { VARENUM.I2, (IntPtr)sbyte.MinValue, (short)sbyte.MinValue };
            yield return new object[] { VARENUM.I2, (IntPtr)(-10), (short)(-10) };
            yield return new object[] { VARENUM.I2, (IntPtr)0, (short)0 };
            yield return new object[] { VARENUM.I2, (IntPtr)10, (short)10 };
            yield return new object[] { VARENUM.I2, (IntPtr)sbyte.MaxValue, (short)sbyte.MaxValue };
            yield return new object[] { VARENUM.I2, (IntPtr)short.MaxValue, short.MaxValue };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.I2, (IntPtr)long.MaxValue, (short)(-1) };
            }

            yield return new object[] { VARENUM.UI2, (IntPtr)(-10), (ushort)65526 };
            yield return new object[] { VARENUM.UI2, (IntPtr)0, (ushort)0 };
            yield return new object[] { VARENUM.UI2, (IntPtr)10, (ushort)10 };
            yield return new object[] { VARENUM.UI2, (IntPtr)byte.MaxValue, (ushort)byte.MaxValue };
            yield return new object[] { VARENUM.UI2, (IntPtr)ushort.MaxValue, ushort.MaxValue };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.UI2, (IntPtr)uint.MaxValue, ushort.MaxValue };
            }
            yield return new object[] { VARENUM.UI2, (IntPtr)(-1), ushort.MaxValue };

            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.I4, (IntPtr)long.MinValue, 0 };
            }
            yield return new object[] { VARENUM.I4, (IntPtr)int.MinValue, int.MinValue };
            yield return new object[] { VARENUM.I4, (IntPtr)short.MinValue, (int)short.MinValue };
            yield return new object[] { VARENUM.I4, (IntPtr)sbyte.MinValue, (int)sbyte.MinValue };
            yield return new object[] { VARENUM.I4, (IntPtr)(-10), -10 };
            yield return new object[] { VARENUM.I4, (IntPtr)0, 0 };
            yield return new object[] { VARENUM.I4, (IntPtr)10, 10 };
            yield return new object[] { VARENUM.I4, (IntPtr)sbyte.MaxValue, (int)sbyte.MaxValue };
            yield return new object[] { VARENUM.I4, (IntPtr)short.MaxValue, (int)short.MaxValue };
            yield return new object[] { VARENUM.I4, (IntPtr)int.MaxValue, int.MaxValue };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.I4, (IntPtr)long.MaxValue, -1 };
            }

            yield return new object[] { VARENUM.UI4, (IntPtr)(-10), (uint)4294967286 };
            yield return new object[] { VARENUM.UI4, (IntPtr)0, (uint)0 };
            yield return new object[] { VARENUM.UI4, (IntPtr)10, (uint)10 };
            yield return new object[] { VARENUM.UI4, (IntPtr)byte.MaxValue, (uint)byte.MaxValue };
            yield return new object[] { VARENUM.UI4, (IntPtr)ushort.MaxValue, (uint)ushort.MaxValue };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.UI4, (IntPtr)uint.MaxValue, uint.MaxValue };
            }
            yield return new object[] { VARENUM.UI4, (IntPtr)(-1), uint.MaxValue };

            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.INT, (IntPtr)long.MinValue, 0 };
            }
            yield return new object[] { VARENUM.INT, (IntPtr)int.MinValue, int.MinValue };
            yield return new object[] { VARENUM.INT, (IntPtr)short.MinValue, (int)short.MinValue };
            yield return new object[] { VARENUM.INT, (IntPtr)sbyte.MinValue, (int)sbyte.MinValue };
            yield return new object[] { VARENUM.INT, (IntPtr)(-10), -10 };
            yield return new object[] { VARENUM.INT, (IntPtr)0, 0 };
            yield return new object[] { VARENUM.INT, (IntPtr)10, 10 };
            yield return new object[] { VARENUM.INT, (IntPtr)sbyte.MaxValue, (int)sbyte.MaxValue };
            yield return new object[] { VARENUM.INT, (IntPtr)short.MaxValue, (int)short.MaxValue };
            yield return new object[] { VARENUM.INT, (IntPtr)int.MaxValue, int.MaxValue };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.INT, (IntPtr)long.MaxValue, -1 };
            }

            yield return new object[] { VARENUM.UINT, (IntPtr)(-10), (uint)4294967286 };
            yield return new object[] { VARENUM.UINT, (IntPtr)0, (uint)0 };
            yield return new object[] { VARENUM.UINT, (IntPtr)10, (uint)10 };
            yield return new object[] { VARENUM.UINT, (IntPtr)byte.MaxValue, (uint)byte.MaxValue };
            yield return new object[] { VARENUM.UINT, (IntPtr)ushort.MaxValue, (uint)ushort.MaxValue };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.UINT, (IntPtr)uint.MaxValue, uint.MaxValue };
            }
            yield return new object[] { VARENUM.UINT, (IntPtr)(-1), uint.MaxValue };

            yield return new object[] { VARENUM.BOOL, (IntPtr)(-1), true };
            yield return new object[] { VARENUM.BOOL, (IntPtr)0, false };
            yield return new object[] { VARENUM.BOOL, (IntPtr)1, true };

            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.ERROR, (IntPtr)long.MinValue, 0 };
            }
            yield return new object[] { VARENUM.ERROR, (IntPtr)int.MinValue, int.MinValue };
            yield return new object[] { VARENUM.ERROR, (IntPtr)short.MinValue, (int)short.MinValue };
            yield return new object[] { VARENUM.ERROR, (IntPtr)sbyte.MinValue, (int)sbyte.MinValue };
            yield return new object[] { VARENUM.ERROR, (IntPtr)(-10), -10 };
            yield return new object[] { VARENUM.ERROR, (IntPtr)0, 0 };
            yield return new object[] { VARENUM.ERROR, (IntPtr)10, 10 };
            yield return new object[] { VARENUM.ERROR, (IntPtr)sbyte.MaxValue, (int)sbyte.MaxValue };
            yield return new object[] { VARENUM.ERROR, (IntPtr)short.MaxValue, (int)short.MaxValue };
            yield return new object[] { VARENUM.ERROR, (IntPtr)int.MaxValue, int.MaxValue };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { VARENUM.ERROR, (IntPtr)long.MaxValue, -1 };
            }
        }

        [StaTheory]
        [MemberData(nameof(ToObject_TestData))]
        public void VARIANT_ToObject_Invoke_ReturnsExpected(ushort vt, IntPtr data, object expected)
        {
            using var variant = new VARIANT
            {
                vt = (VARENUM)vt,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = (IntPtr)data
                }
            };
            AssertToObjectEqual(expected, variant);
        }

        [StaTheory]
        [MemberData(nameof(ToObject_TestData))]
        public void VARIANT_ToObject_InvokeBYREF_ReturnsExpected(ushort vt, IntPtr data, object expected)
        {
            using var variant = new VARIANT
            {
                vt = (VARENUM)vt | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = (IntPtr)(&data)
                }
            };
            AssertToObjectEqual(expected, variant);
        }

        public static IEnumerable<object[]> BYREFNoData_TestData()
        {
            yield return new object[] { VARENUM.I2 };
            yield return new object[] { VARENUM.I4 };
            yield return new object[] { VARENUM.R4 };
            yield return new object[] { VARENUM.R8 };
            yield return new object[] { VARENUM.CY };
            yield return new object[] { VARENUM.DATE };
            yield return new object[] { VARENUM.BSTR };
            yield return new object[] { VARENUM.DISPATCH };
            yield return new object[] { VARENUM.ERROR };
            yield return new object[] { VARENUM.BOOL };
            yield return new object[] { VARENUM.VARIANT };
            yield return new object[] { VARENUM.UNKNOWN };
            yield return new object[] { VARENUM.DECIMAL };
            yield return new object[] { VARENUM.I1 };
            yield return new object[] { VARENUM.UI1 };
            yield return new object[] { VARENUM.UI2 };
            yield return new object[] { VARENUM.UI4 };
            yield return new object[] { VARENUM.I8 };
            yield return new object[] { VARENUM.UI8 };
            yield return new object[] { VARENUM.INT };
            yield return new object[] { VARENUM.UINT };
            yield return new object[] { VARENUM.VOID };
            yield return new object[] { VARENUM.HRESULT };
            yield return new object[] { VARENUM.PTR };
            yield return new object[] { VARENUM.SAFEARRAY };
            yield return new object[] { VARENUM.CARRAY };
            yield return new object[] { VARENUM.USERDEFINED };
            yield return new object[] { VARENUM.LPSTR };
            yield return new object[] { VARENUM.LPWSTR };
            yield return new object[] { VARENUM.RECORD };
            yield return new object[] { VARENUM.INT_PTR };
            yield return new object[] { VARENUM.UINT_PTR };
            yield return new object[] { VARENUM.FILETIME };
            yield return new object[] { VARENUM.BLOB };
            yield return new object[] { VARENUM.STREAM };
            yield return new object[] { VARENUM.STORAGE };
            yield return new object[] { VARENUM.STREAMED_OBJECT };
            yield return new object[] { VARENUM.STORED_OBJECT };
            yield return new object[] { VARENUM.BLOB_OBJECT };
            yield return new object[] { VARENUM.CF };
            yield return new object[] { VARENUM.CLSID };
            yield return new object[] { VARENUM.VERSIONED_STREAM };
            yield return new object[] { VARENUM.BSTR_BLOB };
        }

        [StaTheory]
        [MemberData(nameof(BYREFNoData_TestData))]
        public void VARIANT_Toobject_BYREFNoData_Throws(ushort vt)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BYREF | (VARENUM)vt,
            };
            AssertToObjectThrows<ArgumentException>(variant);
        }

        public static IEnumerable<object[]> ToObject_I8_TestData()
        {
            if (IntPtr.Size == 8)
            {
                yield return new object[] { (IntPtr)long.MinValue, long.MinValue };
                yield return new object[] { (IntPtr)int.MinValue, (long)int.MinValue };
                yield return new object[] { (IntPtr)short.MinValue, (long)short.MinValue };
                yield return new object[] { (IntPtr)sbyte.MinValue, (long)sbyte.MinValue };
                yield return new object[] { (IntPtr)(-10), (long)(-10) };
            }
            yield return new object[] { (IntPtr)0, (long)0 };
            yield return new object[] { (IntPtr)10, (long)10 };
            yield return new object[] { (IntPtr)sbyte.MaxValue, (long)sbyte.MaxValue };
            yield return new object[] { (IntPtr)short.MaxValue, (long)short.MaxValue };
            yield return new object[] { (IntPtr)int.MaxValue, (long)int.MaxValue };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { (IntPtr)long.MaxValue, long.MaxValue };
            }
        }

        [Theory]
        [MemberData(nameof(ToObject_I8_TestData))]
        public void VARIANT_ToObject_I8_ReturnsExpected(IntPtr data, long expected)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.I8,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            AssertToObjectEqual(expected, variant);
        }

        public static IEnumerable<object[]> ToObject_I8BYREF_TestData()
        {
            yield return new object[] { long.MinValue };
            yield return new object[] { int.MinValue };
            yield return new object[] { short.MinValue };
            yield return new object[] { sbyte.MinValue };
            yield return new object[] { -10, };
            yield return new object[] { 0, };
            yield return new object[] { 10, };
            yield return new object[] { sbyte.MaxValue };
            yield return new object[] { short.MaxValue };
            yield return new object[] { int.MaxValue };
            yield return new object[] { long.MaxValue };
        }

        [Theory]
        [MemberData(nameof(ToObject_I8BYREF_TestData))]
        public void VARIANT_ToObject_I8BYREF_ReturnsExpected(long data)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.I8 | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = (IntPtr)(&data)
                }
            };
            AssertToObjectEqual(data, variant);
        }

        public static IEnumerable<object[]> ToObject_UI8_TestData()
        {
            if (IntPtr.Size == 8)
            {
                yield return new object[] { (IntPtr)(-10), (ulong)18446744073709551606 };
            }
            yield return new object[] { (IntPtr)0, (ulong)0 };
            yield return new object[] { (IntPtr)10, (ulong)10 };
            yield return new object[] { (IntPtr)byte.MaxValue, (ulong)byte.MaxValue };
            yield return new object[] { (IntPtr)ushort.MaxValue, (ulong)ushort.MaxValue };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { (IntPtr)uint.MaxValue, (ulong)uint.MaxValue };
                yield return new object[] { (IntPtr)ulong.MaxValue, ulong.MaxValue };
            }
        }

        [Theory]
        [MemberData(nameof(ToObject_UI8_TestData))]
        public void VARIANT_ToObject_UI8_ReturnsExpected(IntPtr data, ulong expected)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.UI8,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            AssertToObjectEqual(expected, variant);
        }

        public static IEnumerable<object[]> ToObject_UI8BYREF_TestData()
        {
            yield return new object[] { 0 };
            yield return new object[] { 10 };
            yield return new object[] { byte.MaxValue };
            yield return new object[] { ushort.MaxValue};
            yield return new object[] { uint.MaxValue };
            yield return new object[] { ulong.MaxValue };
        }

        [Theory]
        [MemberData(nameof(ToObject_UI8BYREF_TestData))]
        public void VARIANT_ToObject_UI8BYREF_ReturnsExpected(ulong data)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.UI8 | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = (IntPtr)(&data)
                }
            };
            AssertToObjectEqual(data, variant);
        }

        public static IEnumerable<object[]> ToObject_CY_TestData()
        {
            yield return new object[] { (IntPtr)0, 0.0m };
            yield return new object[] { (IntPtr)10, 0.001m };
            yield return new object[] { (IntPtr)10000, 1m };
            yield return new object[] { (IntPtr)123456, 12.3456m };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { (IntPtr)(-10), -0.001m };
                yield return new object[] { (IntPtr)(-10000), -1m };
                yield return new object[] { (IntPtr)(-123456), -12.3456m };
            }
        }

        [Theory]
        [MemberData(nameof(ToObject_CY_TestData))]
        public void VARIANT_ToObject_CY_ReturnsExpected(IntPtr data, decimal expected)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.CY,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            AssertToObjectEqual(expected, variant);
        }

        public static IEnumerable<object[]> ToObject_CYBYREF_TestData()
        {
            yield return new object[] { 0, 0.0m };
            yield return new object[] { 10, 0.001m };
            yield return new object[] { 10000, 1m };
            yield return new object[] { 123456, 12.3456m };
            yield return new object[] { -10, -0.001m };
            yield return new object[] { -10000, -1m };
            yield return new object[] { -123456, -12.3456m };
        }

        [Theory]
        [MemberData(nameof(ToObject_CYBYREF_TestData))]
        public void VARIANT_ToObject_CYBYREF_ReturnsExpected(long data, decimal expected)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.CY | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = (IntPtr)(&data)
                }
            };
            AssertToObjectEqual(expected, variant);
        }

        public static IEnumerable<object[]> ToObject_R4_TestData()
        {
            yield return new object[] { (IntPtr)0, 0.0f };
            yield return new object[] { (IntPtr)1067030938, 1.2f };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { (IntPtr)3214514586, -1.2f };
                yield return new object[] { (IntPtr)4290772992, float.NaN };
            }
        }

        [Theory]
        [MemberData(nameof(ToObject_R4_TestData))]
        public void VARIANT_ToObject_R4_ReturnsExpected(IntPtr data, float expected)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.R4,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            AssertToObjectEqual(expected, variant);
        }

        public static IEnumerable<object[]> ToObject_R4BYREF_TestData()
        {
            yield return new object[] { 0.0f };
            yield return new object[] { 1.2f };
            yield return new object[] { -1.2f };
            yield return new object[] { float.NaN };
        }

        [Theory]
        [MemberData(nameof(ToObject_R4BYREF_TestData))]
        public void VARIANT_ToObject_R4BYREF_ReturnsExpected(float data)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.R4 | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = (IntPtr)(&data)
                }
            };
            AssertToObjectEqual(data, variant);
        }

        public static IEnumerable<object[]> ToObject_R8_TestData()
        {
            yield return new object[] { (IntPtr)0, 0.0 };
            if (IntPtr.Size == 8)
            {
                yield return new object[] { (IntPtr)4608083138725491507, 1.2 };
                yield return new object[] { (IntPtr)(-4615288898129284301), -1.2 };
                yield return new object[] { (IntPtr)(-2251799813685248), double.NaN };
            }
        }

        [Theory]
        [MemberData(nameof(ToObject_R8_TestData))]
        public void VARIANT_ToObject_R8_ReturnsExpected(IntPtr data, double expected)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.R8,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            AssertToObjectEqual(expected, variant);
        }

        public static IEnumerable<object[]> ToObject_R8BYREF_TestData()
        {
            yield return new object[] { 0.0 };
            yield return new object[] { 1.2 };
            yield return new object[] { -1.2 };
            yield return new object[] { double.NaN };
        }

        [Theory]
        [MemberData(nameof(ToObject_R8BYREF_TestData))]
        public void VARIANT_ToObject_R8BYREF_ReturnsExpected(double data)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.R8 | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = (IntPtr)(&data)
                }
            };
            AssertToObjectEqual(data, variant);
        }

        public static IEnumerable<object[]> NULL_TestData()
        {
            yield return new object[] { IntPtr.Zero };
            yield return new object[] { (IntPtr)1 };
        }

        [StaTheory]
        [MemberData(nameof(NULL_TestData))]
        public void VARIANT_ToObject_NULL_Sucess(IntPtr data)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BYREF | VARENUM.NULL,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            AssertToObjectEqual(Convert.DBNull, variant);
        }

        [StaTheory]
        [MemberData(nameof(NULL_TestData))]
        public void VARIANT_ToObject_NULLBYREFData_Sucess(IntPtr data)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BYREF | VARENUM.NULL,
                data = new VARIANT.VARIANTUnion
                {
                    ppunkVal = &data
                }
            };
            AssertToObjectEqual(Convert.DBNull, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_NULLBYREFNoData_Sucess()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BYREF | VARENUM.NULL
            };
            AssertToObjectEqual(Convert.DBNull, variant);
        }

        public static IEnumerable<object[]> EMPTY_TestData()
        {
            yield return new object[] { IntPtr.Zero };
            yield return new object[] { (IntPtr)1 };
        }

        [StaTheory]
        [MemberData(nameof(EMPTY_TestData))]
        public void VARIANT_ToObject_EMPTY_Sucess(IntPtr data)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.EMPTY,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            AssertToObjectEqual(null, variant);
        }

        [StaTheory]
        [MemberData(nameof(EMPTY_TestData))]
        public void VARIANT_ToObject_EMPTYBYREFData_Sucess(IntPtr data)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BYREF | VARENUM.EMPTY,
                data = new VARIANT.VARIANTUnion
                {
                    ppunkVal = &data
                }
            };
            AssertToObject(variant, value =>
            {
                if (IntPtr.Size == 8)
                {
                    Assert.Equal((ulong)(IntPtr)variant.data.ppunkVal, value);
                }
                else
                {
                    Assert.Equal((uint)(IntPtr)variant.data.ppunkVal, value);
                }
            });
        }

        [StaFact]
        public void VARIANT_ToObject_EMPTYBYREFNoData_Sucess()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BYREF | VARENUM.EMPTY
            };
            AssertToObject(variant, value =>
            {
                if (IntPtr.Size == 8)
                {
                    Assert.Equal((ulong)0, value);
                }
                else
                {
                    Assert.Equal((uint)0, value);
                }
            });
        }

        public static IEnumerable<object[]> HRESULT_TestData()
        {
            yield return new object[] { (IntPtr)int.MinValue, int.MinValue };
            yield return new object[] { (IntPtr)short.MinValue, (int)short.MinValue };
            yield return new object[] { (IntPtr)sbyte.MinValue, (int)sbyte.MinValue };
            yield return new object[] { (IntPtr)(-10), -10 };
            yield return new object[] { (IntPtr)0, 0 };
            yield return new object[] { (IntPtr)10, 10 };
            yield return new object[] { (IntPtr)sbyte.MaxValue, (int)sbyte.MaxValue };
            yield return new object[] { (IntPtr)short.MaxValue, (int)short.MaxValue };
            yield return new object[] { (IntPtr)int.MaxValue, int.MaxValue };
            if (IntPtr.Size == 8)
            {
                            if (IntPtr.Size == 8)
                            {
                yield return new object[] { (IntPtr)long.MinValue, 0 };
            }
                if (IntPtr.Size == 8)
                {
                    yield return new object[] { (IntPtr)long.MaxValue, -1 };
                }
            }
        }

        [StaTheory]
        [MemberData(nameof(HRESULT_TestData))]
        public void VARIANT_ToObject_HRESULT_Success(IntPtr data, int expected)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.HRESULT,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            AssertToObjectEqualExtension<ArgumentException>(expected, variant);
        }

        [StaTheory]
        [MemberData(nameof(HRESULT_TestData))]
        public void VARIANT_ToObject_HRESULTBYREF_Success(IntPtr data, int expected)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.HRESULT | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    ppunkVal = &data
                }
            };
            AssertToObjectEqualExtension<ArgumentException>(expected, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_FILETIME_Success()
        {
            using var variant = new VARIANT();
            var dt = new DateTime(2020, 05, 13, 13, 3, 12);
            var ft = new FILETIME(dt);
            HRESULT hr = InitPropVariantFromFileTime(&ft, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VARENUM.FILETIME, variant.Type);

            AssertToObjectEqualExtension<ArgumentException>(new DateTime(2020, 05, 13, 13, 3, 12), variant);
        }

        [StaTheory]
        [InlineData(-10)]
        public void VARIANT_ToObject_InvalidFILETIME_ThrowsArgumentOutOfRangeException(int value)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.FILETIME,
                data = new VARIANT.VARIANTUnion
                {
                    cyVal = value
                }
            };
            Assert.Throws<ArgumentOutOfRangeException>("fileTime", () => variant.ToObject());
        }

        [StaFact]
        public void VARIANT_ToObject_DateFromFILETIME_Success()
        {
            using var variant = new VARIANT();
            var dt = new DateTime(2020, 05, 13, 13, 3, 12);
            var ft = new FILETIME(dt);
            HRESULT hr = InitVariantFromFileTime(&ft, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VARENUM.DATE, variant.Type);

            AssertToObjectEqual(new DateTime(2020, 05, 13, 13, 3, 12), variant);
        }

        [StaFact]
        public void VARIANT_ToObject_Date_Success()
        {
            var dt = new DateTime(2020, 05, 13, 13, 3, 12);
            double date = dt.ToOADate();
            using var variant = new VARIANT
            {
                vt = VARENUM.DATE,
                data = new VARIANT.VARIANTUnion
                {
                    date = date
                }
            };
            AssertToObjectEqual(new DateTime(2020, 05, 13, 13, 3, 12), variant);
        }

        [StaFact]
        public void VARIANT_ToObject_DateEmpty_Success()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.DATE
            };
            AssertToObjectEqual(new DateTime(1899, 12, 30), variant);
        }

        [StaFact]
        public void VARIANT_ToObject_DateBYREF_Success()
        {
            var dt = new DateTime(2020, 05, 13, 13, 3, 12);
            double date = dt.ToOADate();
            using var variant = new VARIANT
            {
                vt = VARENUM.DATE | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    pdate = &date
                }
            };
            AssertToObjectEqual(new DateTime(2020, 05, 13, 13, 3, 12), variant);
        }

        [StaFact]
        public void VARIANT_ToObject_DateBYREFEmpty_Success()
        {
            double date = 0;
            using var variant = new VARIANT
            {
                vt = VARENUM.DATE | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    pdate = &date
                }
            };
            AssertToObjectEqual(new DateTime(1899, 12, 30), variant);
        }

        [StaTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("text")]
        public void VARIANT_ToObject_BSTR_ReturnsExpected(string text)
        {
            IntPtr ptr = Marshal.StringToBSTR(text);
            using var variant = new VARIANT
            {
                vt = VARENUM.BSTR,
                data = new VARIANT.VARIANTUnion
                {
                    bstrVal = ptr
                }
            };
            AssertToObjectEqual(text, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_BSTRNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.BSTR
            };
            AssertToObjectEqual(null, variant);
        }

        [StaTheory]
        [InlineData("")]
        [InlineData("text")]
        public void VARIANT_ToObject_BSTRBYREF_ReturnsExpected(string text)
        {
            IntPtr ptr = Marshal.StringToBSTR(text);
            try
            {
                using var variant = new VARIANT
                {
                    vt = VARENUM.BSTR | VARENUM.BYREF,
                    data = new VARIANT.VARIANTUnion
                    {
                        pbstrVal = &ptr
                    }
                };
                AssertToObjectEqual(text, variant);
            }
            finally
            {
                Marshal.FreeBSTR(ptr);
            }
        }

        [StaTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("text")]
        public void VARIANT_ToObject_LPWSTR_ReturnsExpected(string text)
        {
            IntPtr ptr = Marshal.StringToCoTaskMemUni(text);
            using var variant = new VARIANT
            {
                vt = VARENUM.LPWSTR,
                data = new VARIANT.VARIANTUnion
                {
                    bstrVal = ptr
                }
            };
            AssertToObjectEqualExtension<ArgumentException>(text, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_LPWSTRNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.LPWSTR
            };
            AssertToObjectEqualExtension<ArgumentException>(null, variant);
        }

        [StaTheory]
        [InlineData("")]
        [InlineData("text")]
        public void VARIANT_ToObject_LPWSTRBYREF_ReturnsExpected(string text)
        {
            IntPtr ptr = Marshal.StringToCoTaskMemUni(text);
            try
            {
                using var variant = new VARIANT
                {
                    vt = VARENUM.LPWSTR | VARENUM.BYREF,
                    data = new VARIANT.VARIANTUnion
                    {
                        pbstrVal = &ptr
                    }
                };
                AssertToObjectEqualExtension<ArgumentException>(text, variant);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [StaTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("text")]
        public void VARIANT_ToObject_LPSTR_ReturnsExpected(string text)
        {
            IntPtr ptr = Marshal.StringToCoTaskMemAnsi(text);
            using var variant = new VARIANT
            {
                vt = VARENUM.LPSTR,
                data = new VARIANT.VARIANTUnion
                {
                    bstrVal = ptr
                }
            };
            AssertToObjectEqualExtension<ArgumentException>(text, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_LPSTRNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.LPSTR
            };
            AssertToObjectEqualExtension<ArgumentException>(null, variant);
        }

        [StaTheory]
        [InlineData("")]
        [InlineData("text")]
        public void VARIANT_ToObject_LPSTRBYREF_ReturnsExpected(string text)
        {
            IntPtr ptr = Marshal.StringToCoTaskMemAnsi(text);
            try
            {
                using var variant = new VARIANT
                {
                    vt = VARENUM.LPSTR | VARENUM.BYREF,
                    data = new VARIANT.VARIANTUnion
                    {
                        pbstrVal = &ptr
                    }
                };
                AssertToObjectEqualExtension<ArgumentException>(text, variant);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_Dispatch_ReturnsExpected()
        {
            var o = new object();
            IntPtr pUnk = Marshal.GetIUnknownForObject(o);
            using var variant = new VARIANT
            {
                vt = VARENUM.DISPATCH,
                data = new VARIANT.VARIANTUnion
                {
                    pdispVal = pUnk
                }
            };
            AssertToObjectEqual(o, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_DispatchNoData_ReturnsNull()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.DISPATCH
            };
            AssertToObjectEqual(null, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_DispatchBYREF_ReturnsExpected()
        {
            var o = new object();
            IntPtr pUnk = Marshal.GetIUnknownForObject(o);
            using var variant = new VARIANT
            {
                vt = VARENUM.DISPATCH | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    ppdispVal = &pUnk
                }
            };
            AssertToObjectEqual(o, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_DispatchBYREFNullData_ReturnsNull()
        {
            IntPtr pUnk = IntPtr.Zero;
            using var variant = new VARIANT
            {
                vt = VARENUM.DISPATCH | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    ppunkVal = &pUnk
                }
            };
            AssertToObjectEqual(null, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_UNKNOWN_ReturnsExpected()
        {
            var o = new object();
            IntPtr pUnk = Marshal.GetIUnknownForObject(o);
            using var variant = new VARIANT
            {
                vt = VARENUM.UNKNOWN,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = pUnk
                }
            };
            AssertToObjectEqual(o, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_UNKNOWNNoData_ReturnsNull()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.UNKNOWN
            };
            AssertToObjectEqual(null, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_UNKNOWNBYREF_ReturnsExpected()
        {
            var o = new object();
            IntPtr pUnk = Marshal.GetIUnknownForObject(o);
            using var variant = new VARIANT
            {
                vt = VARENUM.UNKNOWN | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    ppunkVal = &pUnk
                }
            };
            AssertToObjectEqual(o, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_UNKNOWNBYREFNullData_ReturnsNull()
        {
            IntPtr pUnk = IntPtr.Zero;
            using var variant = new VARIANT
            {
                vt = VARENUM.UNKNOWN | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    ppunkVal = &pUnk
                }
            };
            AssertToObjectEqual(null, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_I4VARIANTBYREF_ReturnsExpected()
        {
            using var target = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 10
                }
            };
            using var variant = new VARIANT
            {
                vt = VARENUM.VARIANT | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    pvarVal = &target
                }
            };
            AssertToObjectEqual(10, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_BSTRVARIANTBYREF_ReturnsExpected()
        {
            IntPtr ptr = Marshal.StringToBSTR("test");
            using var target = new VARIANT
            {
                vt = VARENUM.BSTR,
                data = new VARIANT.VARIANTUnion
                {
                    bstrVal = ptr
                }
            };
            using var variant = new VARIANT
            {
                vt = VARENUM.VARIANT | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    pvarVal = &target
                }
            };
            AssertToObjectEqual("test", variant);
        }

        [StaFact]
        public void VARIANT_ToObject_EMPTYVARIANTBYREF_ThrowsInvalidOleVariantTypeException()
        {
            using var target = new VARIANT
            {
                vt = VARENUM.EMPTY,
            };
            using var variant = new VARIANT
            {
                vt = VARENUM.VARIANT | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    pvarVal = &target
                }
            };
            AssertToObjectEqual(null, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_BYREFVARIANTBYREF_ThrowsInvalidOleVariantTypeException()
        {
            int lval = 10;
            using var target = new VARIANT
            {
                vt = VARENUM.BYREF | VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    plVal = &lval
                }
            };
            using var variant = new VARIANT
            {
                vt = VARENUM.VARIANT | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    pvarVal = &target
                }
            };
            AssertToObjectThrows<InvalidOleVariantTypeException>(variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VARIANT_ThrowsArgumentException()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VARIANT
            };
            AssertToObjectThrows<ArgumentException>(variant);
        }

        public static IEnumerable<object[]> Decimal_I8_TestData()
        {
            yield return new object[] { long.MinValue, (decimal)long.MinValue };
            yield return new object[] { -123, -123m };
            yield return new object[] { 0, 0m };
            yield return new object[] { 123, 123m };
            yield return new object[] { long.MaxValue, (decimal)long.MaxValue };
        }

        [StaTheory]
        [MemberData(nameof(Decimal_I8_TestData))]
        public void VARIANT_ToObject_DecimalI8_ReturnsExpected(long i8, decimal expected)
        {
            VarDecFromI8(i8, out DECIMAL d);
            VARIANT_ToObject_Decimal_ReturnsExpected(d, expected);
            VARIANT_ToObject_DecimalBYREF_ReturnsExpected(d, expected);
        }

        public static IEnumerable<object[]> Decimal_R8_TestData()
        {
            yield return new object[] { -10e12, -10e12m };
            yield return new object[] { -123.456, -123.456m };
            yield return new object[] { 0.0, 0m };
            yield return new object[] { 123.456, 123.456m };
            yield return new object[] { 10e12, 10e12m };
        }

        [StaTheory]
        [MemberData(nameof(Decimal_R8_TestData))]
        public void VARIANT_ToObject_DecimalR8_ReturnsExpected(double r8, decimal expected)
        {
            VarDecFromR8(r8, out DECIMAL d);
            VARIANT_ToObject_Decimal_ReturnsExpected(d, expected);
            VARIANT_ToObject_DecimalBYREF_ReturnsExpected(d, expected);
        }

        public static IEnumerable<object[]> Decimal_TestData()
        {
            yield return new object[] { new DECIMAL(), 0.0m };
        }

        [StaTheory]
        [MemberData(nameof(Decimal_TestData))]
        public void VARIANT_ToObject_Decimal_ReturnsExpected(object d, decimal expected)
        {
            var variant = new VARIANT();
            *(DECIMAL*)(&variant) = (DECIMAL)d;
            variant.vt = VARENUM.DECIMAL;
            AssertToObjectEqual(expected, variant);
        }

        [StaTheory]
        [MemberData(nameof(Decimal_TestData))]
        public void VARIANT_ToObject_DecimalBYREF_ReturnsExpected(object d, decimal expected)
        {
            DECIMAL asD = (DECIMAL)d;
            using var variant = new VARIANT
            {
                vt = VARENUM.DECIMAL | VARENUM.BYREF,
                data = new VARIANT.VARIANTUnion
                {
                    pdecVal = &asD
                }
            };
            AssertToObjectEqual(expected, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_CLSID_ReturnsExpected()
        {
            var guid = Guid.NewGuid();
            using var variant = new VARIANT();
            HRESULT hr = InitPropVariantFromCLSID(&guid, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VARENUM.CLSID, variant.Type);

            AssertToObjectEqualExtension<ArgumentException>(guid, variant);
        }

        public static IEnumerable<object[]> VOID_TestData()
        {
            yield return new object[] { IntPtr.Zero };
            yield return new object[] { (IntPtr)1 };
        }

        [StaTheory]
        [MemberData(nameof(VOID_TestData))]
        public void VARIANT_ToObject_VOID_ReturnsExpected(IntPtr data)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VOID,
                data = new VARIANT.VARIANTUnion
                {
                    punkVal = data
                }
            };
            IntPtr pv = (IntPtr)(&variant);
            Assert.Null(Marshal.GetObjectForNativeVariant(pv));
        }

        [StaTheory]
        [InlineData((ushort)VARENUM.USERDEFINED)]
        [InlineData((ushort)(VARENUM.USERDEFINED | VARENUM.BYREF))]
        public void VARIANT_ToObject_USERDATA_ThrowsArgumentException(ushort vt)
        {
            using var variant = new VARIANT
            {
                vt = (VARENUM)vt
            };
            AssertToObjectThrows<ArgumentException>(variant);
        }

        [StaTheory]
        [InlineData((ushort)(VARENUM.VOID | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.PTR)]
        [InlineData((ushort)(VARENUM.PTR | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.SAFEARRAY)]
        [InlineData((ushort)(VARENUM.SAFEARRAY | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.CARRAY)]
        [InlineData((ushort)(VARENUM.CARRAY | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.RECORD)]
        [InlineData((ushort)(VARENUM.RECORD | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.BLOB)]
        [InlineData((ushort)(VARENUM.BLOB | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.STREAM)]
        [InlineData((ushort)(VARENUM.STREAM | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.STORAGE)]
        [InlineData((ushort)(VARENUM.STORAGE | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.STREAMED_OBJECT)]
        [InlineData((ushort)(VARENUM.STREAMED_OBJECT | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.STORED_OBJECT)]
        [InlineData((ushort)(VARENUM.STORED_OBJECT | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.BLOB_OBJECT)]
        [InlineData((ushort)(VARENUM.BLOB_OBJECT | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.CF)]
        [InlineData((ushort)(VARENUM.CF | VARENUM.BYREF))]
        [InlineData((ushort)(VARENUM.BSTR_BLOB | VARENUM.BYREF))]
        [InlineData((ushort)VARENUM.ILLEGAL)]
        [InlineData((ushort)VARENUM.INT_PTR)]
        [InlineData((ushort)VARENUM.UINT_PTR)]
        [InlineData(127)]
        [InlineData(0x000F)]
        [InlineData(0x0020)]
        [InlineData(0x0021)]
        [InlineData(0x0022)]
        [InlineData(0x0023)]
        [InlineData(0x0024)]
        public void VARIANT_ToObject_CantConvert_ThrowsArgumentException(ushort vt)
        {
            using var variant = new VARIANT
            {
                vt = (VARENUM)vt
            };
            AssertToObjectThrows<ArgumentException>(variant);
        }

        [StaTheory]
        [InlineData(128)]
        [InlineData(129)]
        [InlineData((ushort)VARENUM.BSTR_BLOB)]
        public void VARIANT_ToObject_Illegal_ThrowsInvalidOleVariantTypeException(ushort vt)
        {
            using var variant = new VARIANT
            {
                vt = (VARENUM)vt
            };
            AssertToObjectThrows<InvalidOleVariantTypeException>(variant);
        }

        public static IEnumerable<object[]> VectorI1_TestData()
        {
            yield return new object[] { Array.Empty<sbyte>() };
            yield return new object[] { new sbyte[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorI1_TestData))]
        public void VARIANT_ToObject_VECTORI1_ReturnsExpected(sbyte[] result)
        {
            var variant = new VARIANT();
            try
            {
                fixed (sbyte* pResult = result)
                {
                    HRESULT hr = InitPropVariantFromBuffer(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VARENUM.VECTOR | VARENUM.UI1, variant.vt);
                }

                // I1 and UI1 have the same size.
                variant.vt = VARENUM.VECTOR | VARENUM.I1;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORI1NoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.I1
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<sbyte>(), variant);
        }

        public static IEnumerable<object[]> VectorUI1_TestData()
        {
            yield return new object[] { Array.Empty<byte>() };
            yield return new object[] { new byte[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorUI1_TestData))]
        public void VARIANT_ToObject_VECTORUI1_ReturnsExpected(byte[] result)
        {
            using var variant = new VARIANT();
            fixed (byte* pResult = result)
            {
                HRESULT hr = InitPropVariantFromBuffer(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VARENUM.VECTOR | VARENUM.UI1, variant.vt);
            }
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORUI1NoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.UI1
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<byte>(), variant);
        }

        public static IEnumerable<object[]> VectorI2_TestData()
        {
            yield return new object[] { Array.Empty<short>() };
            yield return new object[] { new short[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorI2_TestData))]
        public void VARIANT_ToObject_VECTORI2_ReturnsExpected(short[] result)
        {
            using var variant = new VARIANT();
            fixed (short* pResult = result)
            {
                HRESULT hr = InitPropVariantFromInt16Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VARENUM.VECTOR | VARENUM.I2, variant.vt);
            }
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORI2NoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.I2
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<short>(), variant);
        }

        public static IEnumerable<object[]> VectorUI2_TestData()
        {
            yield return new object[] { Array.Empty<ushort>() };
            yield return new object[] { new ushort[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorUI2_TestData))]
        public void VARIANT_ToObject_VECTORUI2_ReturnsExpected(ushort[] result)
        {
            using var variant = new VARIANT();
            fixed (ushort* pResult = result)
            {
                HRESULT hr = InitPropVariantFromUInt16Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VARENUM.VECTOR | VARENUM.UI2, variant.vt);
            }
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORUI2NoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.UI2
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<ushort>(), variant);
        }

        public static IEnumerable<object[]> VectorBOOL_TestData()
        {
            yield return new object[] { Array.Empty<BOOL>(), Array.Empty<bool>() };
            yield return new object[] { new BOOL[] { BOOL.TRUE, BOOL.FALSE, BOOL.TRUE }, new bool[] { true, false, true } };
        }

        [StaTheory]
        [MemberData(nameof(VectorBOOL_TestData))]
        public void VARIANT_ToObject_VECTORBOOL_ReturnsExpected(object result, bool[] expected)
        {
            using var variant = new VARIANT();
            BOOL[] boolResult = (BOOL[])result;
            fixed (BOOL* pResult = boolResult)
            {
                HRESULT hr = InitPropVariantFromBooleanVector(pResult, (uint)boolResult.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VARENUM.VECTOR | VARENUM.BOOL, variant.vt);
            }

            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(expected, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORBOOLNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.BOOL
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<bool>(), variant);
        }

        public static IEnumerable<object[]> VectorI4_TestData()
        {
            yield return new object[] { Array.Empty<int>() };
            yield return new object[] { new int[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorI4_TestData))]
        public void VARIANT_ToObject_VECTORI4_ReturnsExpected(int[] result)
        {
            using var variant = new VARIANT();
            fixed (int* pResult = result)
            {
                HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VARENUM.VECTOR | VARENUM.I4, variant.vt);
            }
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORI4NoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.I4
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
        }

        public static IEnumerable<object[]> VectorUI4_TestData()
        {
            yield return new object[] { Array.Empty<uint>() };
            yield return new object[] { new uint[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorUI4_TestData))]
        public void VARIANT_ToObject_VECTORUI4_ReturnsExpected(uint[] result)
        {
            using var variant = new VARIANT();
            fixed (uint* pResult = result)
            {
                HRESULT hr = InitPropVariantFromUInt32Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VARENUM.VECTOR | VARENUM.UI4, variant.vt);
            }
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORUI4NoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.UI4
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<uint>(), variant);
        }

        public static IEnumerable<object[]> VectorINT_TestData()
        {
            yield return new object[] { Array.Empty<int>() };
            yield return new object[] { new int[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorINT_TestData))]
        public void VARIANT_ToObject_VECTORINT_ReturnsExpected(int[] result)
        {
            var variant = new VARIANT();
            try
            {
                fixed (int* pResult = result)
                {
                    HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VARENUM.VECTOR | VARENUM.I4, variant.vt);
                }

                // I4 and INT have the same size.
                variant.vt = VARENUM.VECTOR | VARENUM.INT;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORINTNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.INT
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
        }

        public static IEnumerable<object[]> VectorUINT_TestData()
        {
            yield return new object[] { Array.Empty<uint>() };
            yield return new object[] { new uint[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorUINT_TestData))]
        public void VARIANT_ToObject_VECTORUINT_ReturnsExpected(uint[] result)
        {
            var variant = new VARIANT();
            try
            {
                fixed (uint* pResult = result)
                {
                    HRESULT hr = InitPropVariantFromUInt32Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VARENUM.VECTOR | VARENUM.UI4, variant.vt);
                }

                // UI4 and UINT have the same size.
                variant.vt = VARENUM.VECTOR | VARENUM.UINT;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORUINTNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.UINT
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<uint>(), variant);
        }

        public static IEnumerable<object[]> VectorI8_TestData()
        {
            yield return new object[] { Array.Empty<long>() };
            yield return new object[] { new long[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorI8_TestData))]
        public void VARIANT_ToObject_VECTORI8_ReturnsExpected(long[] result)
        {
            using var variant = new VARIANT();
            fixed (long* pResult = result)
            {
                HRESULT hr = InitPropVariantFromInt64Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VARENUM.VECTOR | VARENUM.I8, variant.vt);
            }
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORI8NoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.I8
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<long>(), variant);
        }

        public static IEnumerable<object[]> VectorUI8_TestData()
        {
            yield return new object[] { Array.Empty<ulong>() };
            yield return new object[] { new ulong[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorUI8_TestData))]
        public void VARIANT_ToObject_VECTORUI8_ReturnsExpected(ulong[] result)
        {
            using var variant = new VARIANT();
            fixed (ulong* pResult = result)
            {
                HRESULT hr = InitPropVariantFromUInt64Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VARENUM.VECTOR | VARENUM.UI8, variant.vt);
            }
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORUI8NoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.UI8
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<ulong>(), variant);
        }

        public static IEnumerable<object[]> VectorR4_TestData()
        {
            yield return new object[] { Array.Empty<float>() };
            yield return new object[] { new float[] { 1.1f, 2.2f, 3.3f } };
        }

        [StaTheory]
        [MemberData(nameof(VectorR4_TestData))]
        public void VARIANT_ToObject_VECTORR4_ReturnsExpected(float[] result)
        {
            var variant = new VARIANT();
            try
            {
                fixed (float* pResult = result)
                {
                    HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VARENUM.VECTOR | VARENUM.I4, variant.vt);
                }

                // I4 and R4 are the same size.
                variant.vt = VARENUM.VECTOR | VARENUM.R4;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORR4NoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.R4
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
        }

        public static IEnumerable<object[]> VectorR8_TestData()
        {
            yield return new object[] { Array.Empty<double>() };
            yield return new object[] { new double[] { 1.1, 2.2, 3.3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorR8_TestData))]
        public void VARIANT_ToObject_VECTORR8_ReturnsExpected(double[] result)
        {
            using var variant = new VARIANT();
            fixed (double* pResult = result)
            {
                HRESULT hr = InitPropVariantFromDoubleVector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VARENUM.VECTOR | VARENUM.R8, variant.vt);
            }
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORR8NoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.R8
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<double>(), variant);
        }

        public static IEnumerable<object[]> VectorERROR_TestData()
        {
            yield return new object[] { Array.Empty<uint>() };
            yield return new object[] { new uint[] { 1, 2, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorERROR_TestData))]
        public void VARIANT_ToObject_VECTORERROR_ReturnsExpected(uint[] result)
        {
            var variant = new VARIANT();
            try
            {
                fixed (uint* pResult = result)
                {
                    HRESULT hr = InitPropVariantFromUInt32Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VARENUM.VECTOR | VARENUM.UI4, variant.vt);
                }

                // UI4 and ERROR are the same size.
                variant.vt = VARENUM.VECTOR | VARENUM.ERROR;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORERRORNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.ERROR
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
        }

        public static IEnumerable<object[]> VectorCY_TestData()
        {
            yield return new object[] { Array.Empty<long>(), Array.Empty<decimal>() };
            yield return new object[] { new long[] { 11000, 22000, 30000 } , new decimal[] { 1.1m, 2.2m, 3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorCY_TestData))]
        public void VARIANT_ToObject_VECTORCY_ReturnsExpected(long[] result, decimal[] expected)
        {
            var variant = new VARIANT();
            try
            {
                fixed (long* pResult = result)
                {
                    HRESULT hr = InitPropVariantFromInt64Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VARENUM.VECTOR | VARENUM.I8, variant.vt);
                }

                // I8 and CY have the same size.
                variant.vt = VARENUM.VECTOR | VARENUM.CY;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(expected, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORCYNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.CY
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<decimal>(), variant);
        }

        public static IEnumerable<object[]> VectorDATE_TestData()
        {
            yield return new object[] { Array.Empty<double>(), Array.Empty<DateTime>() };

            var d1 = new DateTime(2020, 05, 13, 13, 3, 12);
            var d2 = new DateTime(2020, 05, 13, 13, 3, 11);
            var d3 = new DateTime(2020, 3, 13, 13, 3, 12);
            yield return new object[] { new double[] { d1.ToOADate(), d2.ToOADate(), d3.ToOADate() }, new DateTime[] { d1, d2, d3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorDATE_TestData))]
        public void VARIANT_ToObject_VECTORDATE_ReturnsExpected(double[] result, DateTime[] expected)
        {
            var variant = new VARIANT();
            try
            {
                fixed (double* pResult = result)
                {
                    HRESULT hr = InitPropVariantFromDoubleVector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VARENUM.VECTOR | VARENUM.R8, variant.vt);
                }

                // R8 and DATE have the same size.
                variant.vt = VARENUM.VECTOR | VARENUM.DATE;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(expected, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORDATENoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.DATE
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<DateTime>(), variant);
        }

        public static IEnumerable<object[]> VectorFILETIME_TestData()
        {
            yield return new object[] { Array.Empty<FILETIME>(), Array.Empty<DateTime>() };

            var d1 = new DateTime(2020, 05, 13, 13, 3, 12);
            var d2 = new DateTime(2020, 05, 13, 13, 3, 11);
            var d3 = new DateTime(2020, 3, 13, 13, 3, 12);
            yield return new object[] { new FILETIME[] { new FILETIME(d1), new FILETIME(d2), new FILETIME(d3) }, new DateTime[] { d1, d2, d3 } };
        }

        [StaTheory]
        [MemberData(nameof(VectorFILETIME_TestData))]
        public void VARIANT_ToObject_VECTORFILETIME_ReturnsExpected(object result, DateTime[] expected)
        {
            using var variant = new VARIANT();
            FILETIME[] fileTimeResult = (FILETIME[])result;
            fixed (FILETIME* pResult = fileTimeResult)
            {
                HRESULT hr = InitPropVariantFromFileTimeVector(pResult, (uint)fileTimeResult.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VARENUM.VECTOR | VARENUM.FILETIME, variant.vt);
            }

            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(expected, variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORFILETIMENoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.FILETIME
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<DateTime>(), variant);
        }

        public static IEnumerable<object[]> VectorCLSID_TestData()
        {
            yield return new object[] { Array.Empty<Guid>() };
            yield return new object[] { new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() } };
        }

        [StaTheory]
        [MemberData(nameof(VectorCLSID_TestData))]
        public void VARIANT_ToObject_VECTORCLSID_ReturnsExpected(Guid[] result)
        {
            var variant = new VARIANT();
            try
            {
                fixed (Guid* pResult = result)
                {
                    HRESULT hr = InitPropVariantFromBuffer(pResult, (uint)(result.Length * sizeof(Guid)), &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VARENUM.VECTOR | VARENUM.UI1, variant.vt);
                }

                variant.data.ca.cElems = (uint)(variant.data.ca.cElems / sizeof(Guid));
                variant.vt = VARENUM.VECTOR | VARENUM.CLSID;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORCLSIDNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.CLSID
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORBSTR_ReturnsExpected()
        {
            var variant = new VARIANT();
            IntPtr ptr1 = Marshal.StringToBSTR("text");
            IntPtr ptr2 = Marshal.StringToBSTR("");
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                fixed (IntPtr* pResult = result)
                {
                    if (IntPtr.Size == 4)
                    {
                        HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                        Assert.Equal(HRESULT.S_OK, hr);
                        Assert.Equal(VARENUM.VECTOR | VARENUM.I4, variant.vt);
                    }
                    else
                    {
                        HRESULT hr = InitPropVariantFromInt64Vector(pResult, (uint)result.Length, &variant);
                        Assert.Equal(HRESULT.S_OK, hr);
                        Assert.Equal(VARENUM.VECTOR | VARENUM.I8, variant.vt);
                    }
                }

                // I4/I8 and BSTR have same size.
                variant.vt = VARENUM.VECTOR | VARENUM.BSTR;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(new string[] { null, "text", "" }, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORBSTRNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.BSTR
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORLPWSTR_ReturnsExpected()
        {
            var variant = new VARIANT();
            IntPtr ptr1 = Marshal.StringToCoTaskMemUni("text");
            IntPtr ptr2 = Marshal.StringToCoTaskMemUni("");
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                fixed (IntPtr* pResult = result)
                {
                    if (IntPtr.Size == 4)
                    {
                        HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                        Assert.Equal(HRESULT.S_OK, hr);
                        Assert.Equal(VARENUM.VECTOR | VARENUM.I4, variant.vt);
                    }
                    else
                    {
                        HRESULT hr = InitPropVariantFromInt64Vector(pResult, (uint)result.Length, &variant);
                        Assert.Equal(HRESULT.S_OK, hr);
                        Assert.Equal(VARENUM.VECTOR | VARENUM.I8, variant.vt);
                    }
                }

                // I4/I8 and LPWSTR have same size.
                variant.vt = VARENUM.VECTOR | VARENUM.LPWSTR;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(new string[] { null, "text", "" }, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORLPWSTRNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.LPWSTR
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORLPSTR_ReturnsExpected()
        {
            var variant = new VARIANT();
            IntPtr ptr1 = Marshal.StringToCoTaskMemAnsi("text");
            IntPtr ptr2 = Marshal.StringToCoTaskMemAnsi("");
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                fixed (IntPtr* pResult = result)
                {
                    if (IntPtr.Size == 4)
                    {
                        HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                        Assert.Equal(HRESULT.S_OK, hr);
                        Assert.Equal(VARENUM.VECTOR | VARENUM.I4, variant.vt);
                    }
                    else
                    {
                        HRESULT hr = InitPropVariantFromInt64Vector(pResult, (uint)result.Length, &variant);
                        Assert.Equal(HRESULT.S_OK, hr);
                        Assert.Equal(VARENUM.VECTOR | VARENUM.I8, variant.vt);
                    }
                }

                // I4/I8 and LPSTR have same size.
                variant.vt = VARENUM.VECTOR | VARENUM.LPSTR;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(new string[] { null, "text", "" }, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORLPSTRNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.LPSTR
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORVARIANT_ReturnsExpected()
        {
            var variant = new VARIANT();
            try
            {
                var variant1 = new VARIANT
                {
                    vt = VARENUM.I4,
                    data = new VARIANT.VARIANTUnion
                    {
                        lVal = 1
                    }
                };
                var variant2 = new VARIANT
                {
                    vt = VARENUM.UI4,
                    data = new VARIANT.VARIANTUnion
                    {
                        ulVal = 2
                    }
                };
                var result = new VARIANT[] { variant1, variant2 };
                fixed (VARIANT* pResult = result)
                {
                    HRESULT hr = InitPropVariantFromBuffer(pResult, (uint)(result.Length * sizeof(VARIANT)), &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VARENUM.VECTOR | VARENUM.UI1, variant.vt);
                }

                variant.data.ca.cElems = (uint)(variant.data.ca.cElems / sizeof(VARIANT));
                variant.vt = VARENUM.VECTOR | VARENUM.VARIANT;
                AssertToObjectEqualExtension<InvalidOleVariantTypeException>(new object[] { 1, 2u }, variant);
            }
            finally
            {
                variant.Dispose();
            }
        }

        [StaFact]
        public void VARIANT_ToObject_VECTORVARIANTNoData_ReturnsExpected()
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | VARENUM.VARIANT
            };
            AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
        }

        [StaTheory]
        [InlineData((ushort)VARENUM.EMPTY)]
        [InlineData((ushort)VARENUM.DECIMAL)]
        [InlineData((ushort)VARENUM.UNKNOWN)]
        [InlineData((ushort)VARENUM.DISPATCH)]
        [InlineData((ushort)VARENUM.NULL)]
        [InlineData((ushort)VARENUM.CF)]
        [InlineData((ushort)VARENUM.VOID)]
        [InlineData((ushort)VARENUM.PTR)]
        [InlineData((ushort)VARENUM.SAFEARRAY)]
        [InlineData((ushort)VARENUM.CARRAY)]
        [InlineData((ushort)VARENUM.RECORD)]
        [InlineData((ushort)VARENUM.BLOB)]
        [InlineData((ushort)VARENUM.STREAM)]
        [InlineData((ushort)VARENUM.STORAGE)]
        [InlineData((ushort)VARENUM.STREAMED_OBJECT)]
        [InlineData((ushort)VARENUM.STORED_OBJECT)]
        [InlineData((ushort)VARENUM.BLOB_OBJECT)]
        [InlineData(127)]
        [InlineData(0x000F)]
        [InlineData(0x0020)]
        [InlineData(0x0021)]
        [InlineData(0x0022)]
        [InlineData(0x0023)]
        [InlineData(0x0024)]
        public void VARIANT_ToObject_VECTORInvalidType_ThrowsArgumentException(ushort vt)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | (VARENUM)vt
            };
            Assert.Throws<ArgumentException>(null, () => variant.ToObject());
        }

        [StaTheory]
        [InlineData(128)]
        [InlineData(129)]
        [InlineData((ushort)VARENUM.BSTR_BLOB)]
        public void VARIANT_ToObject_VECTORInvalidTypeNoData_ThrowsInvalidOleVariantTypeException(ushort vt)
        {
            using var variant = new VARIANT
            {
                vt = VARENUM.VECTOR | (VARENUM)vt
            };
            AssertToObjectThrows<InvalidOleVariantTypeException>(variant);
        }

        [StaTheory]
        [MemberData(nameof(VectorUI1_TestData))]
        public void VARIANT_ToObject_ARRAYUI1SingleDimension_ReturnsExpected(byte[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI1, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI1,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<byte[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUI1_TestData))]
        public void VARIANT_ToObject_ARRAYUI1SingleDimensionNonZeroLowerBounds_ReturnsExpected(byte[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI1, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI1,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(byte).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionUI1_TestData()
        {
            yield return new object[] { new byte[0,0] };
            yield return new object[] { new byte[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUI1_TestData))]
        public void VARIANT_ToObject_ARRAYUI1MultiDimension_ReturnsExpected(byte[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI1, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI1,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(byte).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUI1_TestData))]
        public void VARIANT_ToObject_ARRAYUI1MultiDimensionNonZeroLowerBound_ReturnsExpected(byte[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI1, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI1,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(byte).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorI1_TestData))]
        public void VARIANT_ToObject_ARRAYI1SingleDimension_ReturnsExpected(sbyte[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I1, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I1,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<sbyte[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorI1_TestData))]
        public void VARIANT_ToObject_ARRAYI1SingleDimensionNonZeroLowerBounds_ReturnsExpected(sbyte[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I1, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I1,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(sbyte).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionI1_TestData()
        {
            yield return new object[] { new sbyte[0,0] };
            yield return new object[] { new sbyte[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionI1_TestData))]
        public void VARIANT_ToObject_ARRAYI1MultiDimension_ReturnsExpected(sbyte[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I1, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I1,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(sbyte).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionI1_TestData))]
        public void VARIANT_ToObject_ARRAYI1MultiDimensionNonZeroLowerBound_ReturnsExpected(sbyte[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I1, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I1,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(sbyte).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorI2_TestData))]
        public void VARIANT_ToObject_ARRAYI2SingleDimension_ReturnsExpected(short[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I2, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I2,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<short[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorI2_TestData))]
        public void VARIANT_ToObject_ARRAYI2SingleDimensionNonZeroLowerBounds_ReturnsExpected(short[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I2, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I2,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(short).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionI2_TestData()
        {
            yield return new object[] { new short[0,0] };
            yield return new object[] { new short[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionI2_TestData))]
        public void VARIANT_ToObject_ARRAYI2MultiDimension_ReturnsExpected(short[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I2, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I2,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(short).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionI2_TestData))]
        public void VARIANT_ToObject_ARRAYI2MultiDimensionNonZeroLowerBound_ReturnsExpected(short[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I2, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I2,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(short).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUI2_TestData))]
        public void VARIANT_ToObject_ARRAYUI2SingleDimension_ReturnsExpected(ushort[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI2, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI2,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<ushort[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUI2_TestData))]
        public void VARIANT_ToObject_ARRAYUI2SingleDimensionNonZeroLowerBounds_ReturnsExpected(ushort[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI2, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI2,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(ushort).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionUI2_TestData()
        {
            yield return new object[] { new ushort[0,0] };
            yield return new object[] { new ushort[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUI2_TestData))]
        public void VARIANT_ToObject_ARRAYUI2MultiDimension_ReturnsExpected(ushort[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI2, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI2,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(ushort).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUI2_TestData))]
        public void VARIANT_ToObject_ARRAYUI2MultiDimensionNonZeroLowerBound_ReturnsExpected(ushort[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI2, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI2,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(ushort).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorI4_TestData))]
        public void VARIANT_ToObject_ARRAYI4SingleDimension_ReturnsExpected(int[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I4, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<int[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorI4_TestData))]
        public void VARIANT_ToObject_ARRAYI4SingleDimensionNonZeroLowerBounds_ReturnsExpected(int[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I4, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionI4_TestData()
        {
            yield return new object[] { new int[0,0] };
            yield return new object[] { new int[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionI4_TestData))]
        public void VARIANT_ToObject_ARRAYI4MultiDimension_ReturnsExpected(int[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I4, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionI4_TestData))]
        public void VARIANT_ToObject_ARRAYI4MultiDimensionNonZeroLowerBound_ReturnsExpected(int[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I4, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorI4_TestData))]
        public void VARIANT_ToObject_INTArrayI4SingleDimension_ReturnsExpected(int[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.INT, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<int[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorI4_TestData))]
        public void VARIANT_ToObject_INTArrayI4SingleDimensionNonZeroLowerBounds_ReturnsExpected(int[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.INT, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionI4_TestData))]
        public void VARIANT_ToObject_INTArrayI4MultiDimension_ReturnsExpected(int[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.INT, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionI4_TestData))]
        public void VARIANT_ToObject_INTArrayI4MultiDimensionNonZeroLowerBound_ReturnsExpected(int[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.INT, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUI4_TestData))]
        public void VARIANT_ToObject_ARRAYUI4SingleDimension_ReturnsExpected(uint[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI4, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<uint[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUI4_TestData))]
        public void VARIANT_ToObject_ARRAYUI4SingleDimensionNonZeroLowerBounds_ReturnsExpected(uint[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI4, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionUI4_TestData()
        {
            yield return new object[] { new uint[0,0] };
            yield return new object[] { new uint[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUI4_TestData))]
        public void VARIANT_ToObject_ARRAYUI4MultiDimension_ReturnsExpected(uint[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI4, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUI4_TestData))]
        public void VARIANT_ToObject_ARRAYUI4MultiDimensionNonZeroLowerBound_ReturnsExpected(uint[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI4, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUI4_TestData))]
        public void VARIANT_ToObject_UINTArrayUI4SingleDimension_ReturnsExpected(uint[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UINT, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<uint[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUI4_TestData))]
        public void VARIANT_ToObject_UINTArrayUI4SingleDimensionNonZeroLowerBounds_ReturnsExpected(uint[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UINT, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUI4_TestData))]
        public void VARIANT_ToObject_UINTArrayUI4MultiDimension_ReturnsExpected(uint[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UINT, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUI4_TestData))]
        public void VARIANT_ToObject_UINTArrayUI4MultiDimensionNonZeroLowerBound_ReturnsExpected(uint[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UINT, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorINT_TestData))]
        public void VARIANT_ToObject_ARRAYINTSingleDimension_ReturnsExpected(int[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.INT, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.INT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<int[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorINT_TestData))]
        public void VARIANT_ToObject_ARRAYINTSingleDimensionNonZeroLowerBounds_ReturnsExpected(int[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.INT, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.INT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionINT_TestData()
        {
            yield return new object[] { new int[0,0] };
            yield return new object[] { new int[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionINT_TestData))]
        public void VARIANT_ToObject_ARRAYINTMultiDimension_ReturnsExpected(int[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.INT, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.INT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionINT_TestData))]
        public void VARIANT_ToObject_ARRAYINTMultiDimensionNonZeroLowerBound_ReturnsExpected(int[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.INT, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.INT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorINT_TestData))]
        public void VARIANT_ToObject_I4ArrayINTSingleDimension_ReturnsExpected(int[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I4, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.INT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<int[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorINT_TestData))]
        public void VARIANT_ToObject_I4ArrayINTSingleDimensionNonZeroLowerBounds_ReturnsExpected(int[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I4, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.INT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionINT_TestData))]
        public void VARIANT_ToObject_I4ArrayINTMultiDimension_ReturnsExpected(int[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I4, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.INT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionINT_TestData))]
        public void VARIANT_ToObject_I4ArrayINTMultiDimensionNonZeroLowerBound_ReturnsExpected(int[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I4, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.INT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(int).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUINT_TestData))]
        public void VARIANT_ToObject_ARRAYUINTSingleDimension_ReturnsExpected(uint[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UINT, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UINT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<uint[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUINT_TestData))]
        public void VARIANT_ToObject_ARRAYUINTSingleDimensionNonZeroLowerBounds_ReturnsExpected(uint[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UINT, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UINT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionUINT_TestData()
        {
            yield return new object[] { new uint[0,0] };
            yield return new object[] { new uint[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUINT_TestData))]
        public void VARIANT_ToObject_ARRAYUINTMultiDimension_ReturnsExpected(uint[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UINT, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UINT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUINT_TestData))]
        public void VARIANT_ToObject_ARRAYUINTMultiDimensionNonZeroLowerBound_ReturnsExpected(uint[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UINT, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UINT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUINT_TestData))]
        public void VARIANT_ToObject_UI4ArrayUINTSingleDimension_ReturnsExpected(uint[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI4, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UINT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<uint[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUINT_TestData))]
        public void VARIANT_ToObject_UI4ArrayUINTSingleDimensionNonZeroLowerBounds_ReturnsExpected(uint[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI4, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UINT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUINT_TestData))]
        public void VARIANT_ToObject_UI4ArrayUINTMultiDimension_ReturnsExpected(uint[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI4, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UINT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUINT_TestData))]
        public void VARIANT_ToObject_UI4ArrayUINTMultiDimensionNonZeroLowerBound_ReturnsExpected(uint[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI4, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UINT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorI8_TestData))]
        public void VARIANT_ToObject_ARRAYI8SingleDimension_ReturnsExpected(long[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I8, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<long[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorI8_TestData))]
        public void VARIANT_ToObject_ARRAYI8SingleDimensionNonZeroLowerBounds_ReturnsExpected(long[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I8, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(long).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionI8_TestData()
        {
            yield return new object[] { new long[0,0] };
            yield return new object[] { new long[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionI8_TestData))]
        public void VARIANT_ToObject_ARRAYI8MultiDimension_ReturnsExpected(long[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I8, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(long).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionI8_TestData))]
        public void VARIANT_ToObject_ARRAYI8MultiDimensionNonZeroLowerBound_ReturnsExpected(long[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.I8, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(long).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUI8_TestData))]
        public void VARIANT_ToObject_ARRAYUI8SingleDimension_ReturnsExpected(ulong[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI8, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<ulong[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorUI8_TestData))]
        public void VARIANT_ToObject_ARRAYUI8SingleDimensionNonZeroLowerBounds_ReturnsExpected(ulong[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI8, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(ulong).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionUI8_TestData()
        {
            yield return new object[] { new ulong[0,0] };
            yield return new object[] { new ulong[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUI8_TestData))]
        public void VARIANT_ToObject_ARRAYUI8MultiDimension_ReturnsExpected(ulong[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI8, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(ulong).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionUI8_TestData))]
        public void VARIANT_ToObject_ARRAYUI8MultiDimensionNonZeroLowerBound_ReturnsExpected(ulong[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.UI8, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.UI8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(ulong).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorR4_TestData))]
        public void VARIANT_ToObject_ARRAYR4SingleDimension_ReturnsExpected(float[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.R4, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.R4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<float[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorR4_TestData))]
        public void VARIANT_ToObject_ARRAYR4SingleDimensionNonZeroLowerBounds_ReturnsExpected(float[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.R4, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.R4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(float).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionR4_TestData()
        {
            yield return new object[] { new float[0,0] };
            yield return new object[] { new float[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionR4_TestData))]
        public void VARIANT_ToObject_ARRAYR4MultiDimension_ReturnsExpected(float[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.R4, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.R4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(float).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionR4_TestData))]
        public void VARIANT_ToObject_ARRAYR4MultiDimensionNonZeroLowerBound_ReturnsExpected(float[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.R4, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.R4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(float).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorR8_TestData))]
        public void VARIANT_ToObject_ARRAYR8SingleDimension_ReturnsExpected(double[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.R8, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.R8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<double[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorR8_TestData))]
        public void VARIANT_ToObject_ARRAYR8SingleDimensionNonZeroLowerBounds_ReturnsExpected(double[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.R8, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.R8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(double).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionR8_TestData()
        {
            yield return new object[] { new double[0,0] };
            yield return new object[] { new double[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionR8_TestData))]
        public void VARIANT_ToObject_ARRAYR8MultiDimension_ReturnsExpected(double[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.R8, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.R8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(double).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionR8_TestData))]
        public void VARIANT_ToObject_ARRAYR8MultiDimensionNonZeroLowerBound_ReturnsExpected(double[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.R8, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.R8,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(double).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorERROR_TestData))]
        public void VARIANT_ToObject_ARRAYERRORSingleDimension_ReturnsExpected(uint[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.ERROR, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.ERROR,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<uint[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorERROR_TestData))]
        public void VARIANT_ToObject_ARRAYERRORSingleDimensionNonZeroLowerBounds_ReturnsExpected(uint[] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.ERROR, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.ERROR,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionERROR_TestData()
        {
            yield return new object[] { new uint[0,0] };
            yield return new object[] { new uint[2,3] { { 1, 2, 3 }, { 4, 5, 6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionERROR_TestData))]
        public void VARIANT_ToObject_ARRAYERRORMultiDimension_ReturnsExpected(uint[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.ERROR, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.ERROR,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionERROR_TestData))]
        public void VARIANT_ToObject_ARRAYERRORMultiDimensionNonZeroLowerBound_ReturnsExpected(int[,] result)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.ERROR, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.ERROR,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(uint).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(result, array);
            });
        }

        public static IEnumerable<object[]> ArrayBOOL_TestData()
        {
            yield return new object[] { Array.Empty<VARIANT_BOOL>(), Array.Empty<bool>() };
            yield return new object[] { new VARIANT_BOOL[] { VARIANT_BOOL.TRUE, VARIANT_BOOL.FALSE, VARIANT_BOOL.TRUE }, new bool[] { true, false, true } };
        }

        [StaTheory]
        [MemberData(nameof(ArrayBOOL_TestData))]
        public void VARIANT_ToObject_ARRAYBOOLSingleDimension_ReturnsExpected(object result, bool[] expected)
        {
            VARIANT_BOOL[] boolResult = (VARIANT_BOOL[])result;
            SAFEARRAY *psa = CreateSafeArray(VARENUM.BOOL, boolResult);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.BOOL,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<bool[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(expected.Length, array.GetLength(0));
                Assert.Equal(expected, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(ArrayBOOL_TestData))]
        public void VARIANT_ToObject_ARRAYBOOLSingleDimensionNonZeroLowerBounds_ReturnsExpected(object result, bool[] expected)
        {
            VARIANT_BOOL[] boolResult = (VARIANT_BOOL[])result;
            SAFEARRAY *psa = CreateSafeArray(VARENUM.BOOL, boolResult, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.BOOL,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(bool).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(expected.Length, array.GetLength(0));
                Assert.Equal(expected, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionBOOL_TestData()
        {
            yield return new object[] { new VARIANT_BOOL[0,0], new bool[0,0] };
            yield return new object[] { new VARIANT_BOOL[2, 3] { { VARIANT_BOOL.TRUE, VARIANT_BOOL.FALSE, VARIANT_BOOL.TRUE }, { VARIANT_BOOL.FALSE, VARIANT_BOOL.TRUE, VARIANT_BOOL.FALSE } }, new bool[2,3] { { true, false, true }, { false, true, false } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionBOOL_TestData))]
        public void VARIANT_ToObject_ARRAYBOOLMultiDimension_ReturnsExpected(object result, bool[,] expected)
        {
            VARIANT_BOOL[,] boolResult = (VARIANT_BOOL[,])result;
            SAFEARRAY *psa = CreateSafeArray(VARENUM.BOOL, boolResult);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.BOOL,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(bool).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(expected.GetLength(0), array.GetLength(0));
                Assert.Equal(expected.GetLength(1), array.GetLength(1));
                Assert.Equal(expected, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionBOOL_TestData))]
        public void VARIANT_ToObject_ARRAYBOOLMultiDimensionNonZeroLowerBound_ReturnsExpected(object result, bool[,] expected)
        {
            VARIANT_BOOL[,] boolResult = (VARIANT_BOOL[,])result;
            SAFEARRAY *psa = CreateSafeArray(VARENUM.BOOL, boolResult, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.BOOL,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(bool).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(expected.GetLength(0), array.GetLength(0));
                Assert.Equal(expected.GetLength(1), array.GetLength(1));
                Assert.Equal(expected, array);
            });
        }

        public static IEnumerable<object[]> ArrayDECIMAL_TestData()
        {
            yield return new object[] { Array.Empty<DECIMAL>(), Array.Empty<decimal>() };

            VarDecFromR8(1.1, out DECIMAL d1);
            VarDecFromR8(2.2, out DECIMAL d2);
            VarDecFromR8(3.3, out DECIMAL d3);
            yield return new object[] { new DECIMAL[] { d1, d2, d3 }, new decimal[] { 1.1m, 2.2m, 3.3m } };
        }

        [StaTheory]
        [MemberData(nameof(ArrayDECIMAL_TestData))]
        public void VARIANT_ToObject_ARRAYDECIMALSingleDimension_ReturnsExpected(object result, decimal[] expected)
        {
            DECIMAL[] decimalResult = (DECIMAL[])result;
            SAFEARRAY *psa = CreateSafeArray(VARENUM.DECIMAL, decimalResult);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.DECIMAL,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<decimal[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(expected.Length, array.GetLength(0));
                Assert.Equal(expected, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(ArrayDECIMAL_TestData))]
        public void VARIANT_ToObject_ARRAYDECIMALSingleDimensionNonZeroLowerBounds_ReturnsExpected(object result, decimal[] expected)
        {
            DECIMAL[] decimalResult = (DECIMAL[])result;
            SAFEARRAY *psa = CreateSafeArray(VARENUM.DECIMAL, decimalResult, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.DECIMAL,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(decimal).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(expected.Length, array.GetLength(0));
                Assert.Equal(expected, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionDECIMAL_TestData()
        {
            yield return new object[] { new DECIMAL[0,0], new decimal[0,0] };
            VarDecFromR8(1.1, out DECIMAL d1);
            VarDecFromR8(2.2, out DECIMAL d2);
            VarDecFromR8(3.3, out DECIMAL d3);
            VarDecFromR8(3.1, out DECIMAL d4);
            VarDecFromR8(2.2, out DECIMAL d5);
            VarDecFromR8(1.3, out DECIMAL d6);
            yield return new object[] { new DECIMAL[2, 3] { { d1, d2, d3 }, { d4, d5, d6 } }, new decimal[2,3] { { 1.1m, 2.2m, 3.3m }, { 3.1m, 2.2m, 1.3m } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionDECIMAL_TestData))]
        public void VARIANT_ToObject_ARRAYDECIMALMultiDimension_ReturnsExpected(object result, decimal[,] expected)
        {
            DECIMAL[,] decimalResult = (DECIMAL[,])result;
            SAFEARRAY *psa = CreateSafeArray(VARENUM.DECIMAL, decimalResult);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.DECIMAL,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(decimal).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(expected.GetLength(0), array.GetLength(0));
                Assert.Equal(expected.GetLength(1), array.GetLength(1));
                Assert.Equal(expected, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionDECIMAL_TestData))]
        public void VARIANT_ToObject_ARRAYDECIMALMultiDimensionNonZeroLowerBound_ReturnsExpected(object result, decimal[,] expected)
        {
            DECIMAL[,] decimalResult = (DECIMAL[,])result;
            SAFEARRAY *psa = CreateSafeArray(VARENUM.DECIMAL, decimalResult, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.DECIMAL,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(decimal).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(expected.GetLength(0), array.GetLength(0));
                Assert.Equal(expected.GetLength(1), array.GetLength(1));
                Assert.Equal(expected, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorCY_TestData))]
        public void VARIANT_ToObject_ARRAYCYSingleDimension_ReturnsExpected(long[] result, decimal[] expected)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.CY, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.CY,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<decimal[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(expected.Length, array.GetLength(0));
                Assert.Equal(expected, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorCY_TestData))]
        public void VARIANT_ToObject_ARRAYCYSingleDimensionNonZeroLowerBounds_ReturnsExpected(long[] result, decimal[] expected)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.CY, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.CY,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(decimal).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(expected.Length, array.GetLength(0));
                Assert.Equal(expected, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionCY_TestData()
        {
            yield return new object[] { new long[0,0], new decimal[0,0] };
            yield return new object[] { new long[2, 3] { { 11000, 22000, 33000 }, { 31000, 22000, 13000 } }, new decimal[2,3] { { 1.1m, 2.2m, 3.3m }, { 3.1m, 2.2m, 1.3m } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionCY_TestData))]
        public void VARIANT_ToObject_ARRAYCYMultiDimension_ReturnsExpected(long[,] result, decimal[,] expected)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.CY, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.CY,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(decimal).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(expected.GetLength(0), array.GetLength(0));
                Assert.Equal(expected.GetLength(1), array.GetLength(1));
                Assert.Equal(expected, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionCY_TestData))]
        public void VARIANT_ToObject_ARRAYCYMultiDimensionNonZeroLowerBound_ReturnsExpected(long[,] result, decimal[,] expected)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.CY, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.CY,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(decimal).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(expected.GetLength(0), array.GetLength(0));
                Assert.Equal(expected.GetLength(1), array.GetLength(1));
                Assert.Equal(expected, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorDATE_TestData))]
        public void VARIANT_ToObject_ARRAYDATESingleDimension_ReturnsExpected(double[] result, DateTime[] expected)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.DATE, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.DATE,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<DateTime[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(expected.Length, array.GetLength(0));
                Assert.Equal(expected, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(VectorDATE_TestData))]
        public void VARIANT_ToObject_ARRAYDATESingleDimensionNonZeroLowerBounds_ReturnsExpected(double[] result, DateTime[] expected)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.DATE, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.DATE,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(DateTime).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(expected.Length, array.GetLength(0));
                Assert.Equal(expected, array);
            });
        }

        public static IEnumerable<object[]> MultiDimensionDATE_TestData()
        {
            yield return new object[] { new double[0,0], new DateTime[0,0] };

            var d1 = new DateTime(2020, 05, 13, 13, 3, 12);
            var d2 = new DateTime(2020, 05, 13, 13, 3, 11);
            var d3 = new DateTime(2020, 3, 13, 13, 3, 12);
            var d4 = new DateTime(1892, 1, 2, 3, 4, 5, 6);
            var d5 = new DateTime(2010, 2, 3, 4, 5, 6);
            var d6 = new DateTime(8000, 10, 11, 12, 13, 14);
            yield return new object[] { new double[2, 3] { { d1.ToOADate(), d2.ToOADate(), d3.ToOADate() }, { d4.ToOADate(), d5.ToOADate(), d6.ToOADate() } }, new DateTime[2,3] { { d1, d2, d3 }, { d4, d5, d6 } } };
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionDATE_TestData))]
        public void VARIANT_ToObject_ARRAYDATEMultiDimension_ReturnsExpected(double[,] result, DateTime[,] expected)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.DATE, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.DATE,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(DateTime).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(expected.GetLength(0), array.GetLength(0));
                Assert.Equal(expected.GetLength(1), array.GetLength(1));
                Assert.Equal(expected, array);
            });
        }

        [StaTheory]
        [MemberData(nameof(MultiDimensionDATE_TestData))]
        public void VARIANT_ToObject_ARRAYDATEMultiDimensionNonZeroLowerBound_ReturnsExpected(double[,] result, DateTime[,] expected)
        {
            SAFEARRAY *psa = CreateSafeArray(VARENUM.DATE, result, 1, 2);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.DATE,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(DateTime).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(2, array.GetLowerBound(1));
                Assert.Equal(expected.GetLength(0), array.GetLength(0));
                Assert.Equal(expected.GetLength(1), array.GetLength(1));
                Assert.Equal(expected, array);
            });
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYBSTRSingleDimension_ReturnsExpected()
        {
            IntPtr ptr1 = Marshal.StringToBSTR("text");
            IntPtr ptr2 = Marshal.StringToBSTR("");
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.BSTR, result);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.BSTR,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType<string[]>(array);
                    Assert.Equal(1, array.Rank);
                    Assert.Equal(0, array.GetLowerBound(0));
                    Assert.Equal(result.Length, array.GetLength(0));
                    Assert.Equal(new string[] { null, "text", "" }, array);
                });
            }
            finally
            {
                Marshal.FreeBSTR(ptr1);
                Marshal.FreeBSTR(ptr2);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYBSTRSingleDimensionNonZeroLowerBound_ReturnsExpected()
        {
            IntPtr ptr1 = Marshal.StringToBSTR("text");
            IntPtr ptr2 = Marshal.StringToBSTR("");
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.BSTR, result, 1);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.BSTR,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(string).MakeArrayType(1), array);
                    Assert.Equal(1, array.Rank);
                    Assert.Equal(1, array.GetLowerBound(0));
                    Assert.Equal(result.Length, array.GetLength(0));
                    Assert.Equal(new string[] { null, "text", "" }, array);
                });
            }
            finally
            {
                Marshal.FreeBSTR(ptr1);
                Marshal.FreeBSTR(ptr2);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYBSTRMultiDimension_ReturnsExpected()
        {
            IntPtr ptr1 = Marshal.StringToBSTR("text");
            IntPtr ptr2 = Marshal.StringToBSTR("");
            IntPtr ptr3 = Marshal.StringToBSTR("text3");
            IntPtr ptr4 = Marshal.StringToBSTR("text4");
            IntPtr ptr5 = Marshal.StringToBSTR("text5");
            try
            {
                var result = new IntPtr[2,3] { { IntPtr.Zero, ptr1, ptr2 }, { ptr3, ptr4, ptr5 } };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.BSTR, result);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.BSTR,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(string).MakeArrayType(2), array);
                    Assert.Equal(2, array.Rank);
                    Assert.Equal(0, array.GetLowerBound(0));
                    Assert.Equal(0, array.GetLowerBound(1));
                    Assert.Equal(result.GetLength(0), array.GetLength(0));
                    Assert.Equal(result.GetLength(1), array.GetLength(1));
                    Assert.Equal(new string[,] { { null, "text", "" }, { "text3", "text4", "text5" } }, array);
                });
            }
            finally
            {
                Marshal.FreeBSTR(ptr1);
                Marshal.FreeBSTR(ptr2);
                Marshal.FreeBSTR(ptr3);
                Marshal.FreeBSTR(ptr4);
                Marshal.FreeBSTR(ptr5);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYBSTRMultiDimensionNonZeroLowerBound_ReturnsExpected()
        {
            IntPtr ptr1 = Marshal.StringToBSTR("text");
            IntPtr ptr2 = Marshal.StringToBSTR("");
            IntPtr ptr3 = Marshal.StringToBSTR("text3");
            IntPtr ptr4 = Marshal.StringToBSTR("text4");
            IntPtr ptr5 = Marshal.StringToBSTR("text5");
            try
            {
                var result = new IntPtr[2,3] { { IntPtr.Zero, ptr1, ptr2 }, { ptr3, ptr4, ptr5 } };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.BSTR, result, 1, 2);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.BSTR,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(string).MakeArrayType(2), array);
                    Assert.Equal(2, array.Rank);
                    Assert.Equal(1, array.GetLowerBound(0));
                    Assert.Equal(2, array.GetLowerBound(1));
                    Assert.Equal(result.GetLength(0), array.GetLength(0));
                    Assert.Equal(result.GetLength(1), array.GetLength(1));
                    Assert.Equal(new string[,] { { null, "text", "" }, { "text3", "text4", "text5" } }, array);
                });
            }
            finally
            {
                Marshal.FreeBSTR(ptr1);
                Marshal.FreeBSTR(ptr2);
                Marshal.FreeBSTR(ptr3);
                Marshal.FreeBSTR(ptr4);
                Marshal.FreeBSTR(ptr5);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYUNKNOWNSingleDimension_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.UNKNOWN, result);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.UNKNOWN,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType<object[]>(array);
                    Assert.Equal(1, array.Rank);
                    Assert.Equal(0, array.GetLowerBound(0));
                    Assert.Equal(result.Length, array.GetLength(0));
                    Assert.Equal(new object[] { null, o1, o2 }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYUNKNOWNSingleDimensionNonZeroLowerBound_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.UNKNOWN, result, 1);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.UNKNOWN,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(object).MakeArrayType(1), array);
                    Assert.Equal(1, array.Rank);
                    Assert.Equal(1, array.GetLowerBound(0));
                    Assert.Equal(result.Length, array.GetLength(0));
                    Assert.Equal(new object[] { null, o1, o2 }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYUNKNOWNMultiDimension_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();
            var o5 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            IntPtr ptr3 = Marshal.GetIUnknownForObject(o3);
            IntPtr ptr4 = Marshal.GetIUnknownForObject(o4);
            IntPtr ptr5 = Marshal.GetIUnknownForObject(o5);
            try
            {
                var result = new IntPtr[2,3] { { IntPtr.Zero, ptr1, ptr2 }, { ptr3, ptr4, ptr5 } };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.UNKNOWN, result);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.UNKNOWN,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(object).MakeArrayType(2), array);
                    Assert.Equal(2, array.Rank);
                    Assert.Equal(0, array.GetLowerBound(0));
                    Assert.Equal(0, array.GetLowerBound(1));
                    Assert.Equal(result.GetLength(0), array.GetLength(0));
                    Assert.Equal(result.GetLength(1), array.GetLength(1));
                    Assert.Equal(new object[,] { { null, o1, o2 }, { o3, o4, o5 } }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
                Marshal.Release(ptr3);
                Marshal.Release(ptr4);
                Marshal.Release(ptr5);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYUNKNOWNMultiDimensionNonZeroLowerBound_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();
            var o5 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            IntPtr ptr3 = Marshal.GetIUnknownForObject(o3);
            IntPtr ptr4 = Marshal.GetIUnknownForObject(o4);
            IntPtr ptr5 = Marshal.GetIUnknownForObject(o5);
            try
            {
                var result = new IntPtr[2,3] { { IntPtr.Zero, ptr1, ptr2 }, { ptr3, ptr4, ptr5 } };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.UNKNOWN, result, 1, 2);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.UNKNOWN,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(object).MakeArrayType(2), array);
                    Assert.Equal(2, array.Rank);
                    Assert.Equal(1, array.GetLowerBound(0));
                    Assert.Equal(2, array.GetLowerBound(1));
                    Assert.Equal(result.GetLength(0), array.GetLength(0));
                    Assert.Equal(result.GetLength(1), array.GetLength(1));
                    Assert.Equal(new object[,] { { null, o1, o2 }, { o3, o4, o5 } }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
                Marshal.Release(ptr3);
                Marshal.Release(ptr4);
                Marshal.Release(ptr5);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_DISPATCHArrayUNKNOWNSingleDimension_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.DISPATCH, result);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.UNKNOWN,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType<object[]>(array);
                    Assert.Equal(1, array.Rank);
                    Assert.Equal(0, array.GetLowerBound(0));
                    Assert.Equal(result.Length, array.GetLength(0));
                    Assert.Equal(new object[] { null, o1, o2 }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_DISPATCHArrayUNKNOWNSingleDimensionNonZeroLowerBound_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.DISPATCH, result, 1);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.UNKNOWN,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(object).MakeArrayType(1), array);
                    Assert.Equal(1, array.Rank);
                    Assert.Equal(1, array.GetLowerBound(0));
                    Assert.Equal(result.Length, array.GetLength(0));
                    Assert.Equal(new object[] { null, o1, o2 }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_DISPATCHArrayUNKNOWNMultiDimension_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();
            var o5 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            IntPtr ptr3 = Marshal.GetIUnknownForObject(o3);
            IntPtr ptr4 = Marshal.GetIUnknownForObject(o4);
            IntPtr ptr5 = Marshal.GetIUnknownForObject(o5);
            try
            {
                var result = new IntPtr[2,3] { { IntPtr.Zero, ptr1, ptr2 }, { ptr3, ptr4, ptr5 } };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.DISPATCH, result);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.UNKNOWN,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(object).MakeArrayType(2), array);
                    Assert.Equal(2, array.Rank);
                    Assert.Equal(0, array.GetLowerBound(0));
                    Assert.Equal(0, array.GetLowerBound(1));
                    Assert.Equal(result.GetLength(0), array.GetLength(0));
                    Assert.Equal(result.GetLength(1), array.GetLength(1));
                    Assert.Equal(new object[,] { { null, o1, o2 }, { o3, o4, o5 } }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
                Marshal.Release(ptr3);
                Marshal.Release(ptr4);
                Marshal.Release(ptr5);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_DISPATCHArrayUNKNOWNMultiDimensionNonZeroLowerBound_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();
            var o5 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            IntPtr ptr3 = Marshal.GetIUnknownForObject(o3);
            IntPtr ptr4 = Marshal.GetIUnknownForObject(o4);
            IntPtr ptr5 = Marshal.GetIUnknownForObject(o5);
            try
            {
                var result = new IntPtr[2,3] { { IntPtr.Zero, ptr1, ptr2 }, { ptr3, ptr4, ptr5 } };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.DISPATCH, result, 1, 2);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.UNKNOWN,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(object).MakeArrayType(2), array);
                    Assert.Equal(2, array.Rank);
                    Assert.Equal(1, array.GetLowerBound(0));
                    Assert.Equal(2, array.GetLowerBound(1));
                    Assert.Equal(result.GetLength(0), array.GetLength(0));
                    Assert.Equal(result.GetLength(1), array.GetLength(1));
                    Assert.Equal(new object[,] { { null, o1, o2 }, { o3, o4, o5 } }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
                Marshal.Release(ptr3);
                Marshal.Release(ptr4);
                Marshal.Release(ptr5);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYDISPATCHSingleDimension_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.DISPATCH, result);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.DISPATCH,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType<object[]>(array);
                    Assert.Equal(1, array.Rank);
                    Assert.Equal(0, array.GetLowerBound(0));
                    Assert.Equal(result.Length, array.GetLength(0));
                    Assert.Equal(new object[] { null, o1, o2 }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYDISPATCHSingleDimensionNonZeroLowerBound_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            try
            {
                var result = new IntPtr[] { IntPtr.Zero, ptr1, ptr2 };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.DISPATCH, result, 1);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.DISPATCH,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(object).MakeArrayType(1), array);
                    Assert.Equal(1, array.Rank);
                    Assert.Equal(1, array.GetLowerBound(0));
                    Assert.Equal(result.Length, array.GetLength(0));
                    Assert.Equal(new object[] { null, o1, o2 }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYDISPATCHMultiDimension_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();
            var o5 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            IntPtr ptr3 = Marshal.GetIUnknownForObject(o3);
            IntPtr ptr4 = Marshal.GetIUnknownForObject(o4);
            IntPtr ptr5 = Marshal.GetIUnknownForObject(o5);
            try
            {
                var result = new IntPtr[2,3] { { IntPtr.Zero, ptr1, ptr2 }, { ptr3, ptr4, ptr5 } };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.DISPATCH, result);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.DISPATCH,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(object).MakeArrayType(2), array);
                    Assert.Equal(2, array.Rank);
                    Assert.Equal(0, array.GetLowerBound(0));
                    Assert.Equal(0, array.GetLowerBound(1));
                    Assert.Equal(result.GetLength(0), array.GetLength(0));
                    Assert.Equal(result.GetLength(1), array.GetLength(1));
                    Assert.Equal(new object[,] { { null, o1, o2 }, { o3, o4, o5 } }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
                Marshal.Release(ptr3);
                Marshal.Release(ptr4);
                Marshal.Release(ptr5);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYDISPATCHMultiDimensionNonZeroLowerBound_ReturnsExpected()
        {
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();
            var o5 = new object();
            IntPtr ptr1 = Marshal.GetIUnknownForObject(o1);
            IntPtr ptr2 = Marshal.GetIUnknownForObject(o2);
            IntPtr ptr3 = Marshal.GetIUnknownForObject(o3);
            IntPtr ptr4 = Marshal.GetIUnknownForObject(o4);
            IntPtr ptr5 = Marshal.GetIUnknownForObject(o5);
            try
            {
                var result = new IntPtr[2,3] { { IntPtr.Zero, ptr1, ptr2 }, { ptr3, ptr4, ptr5 } };
                SAFEARRAY *psa = CreateSafeArray(VARENUM.DISPATCH, result, 1, 2);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.DISPATCH,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObject(variant, value =>
                {
                    Array array = (Array)value;
                    Assert.IsType(typeof(object).MakeArrayType(2), array);
                    Assert.Equal(2, array.Rank);
                    Assert.Equal(1, array.GetLowerBound(0));
                    Assert.Equal(2, array.GetLowerBound(1));
                    Assert.Equal(result.GetLength(0), array.GetLength(0));
                    Assert.Equal(result.GetLength(1), array.GetLength(1));
                    Assert.Equal(new object[,] { { null, o1, o2 }, { o3, o4, o5 } }, array);
                });
            }
            finally
            {
                Marshal.Release(ptr1);
                Marshal.Release(ptr2);
                Marshal.Release(ptr3);
                Marshal.Release(ptr4);
                Marshal.Release(ptr5);
            }
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYVARIANTSingleDimension_ReturnsExpected()
        {
            using var v1 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 1
                }
            };
            using var v2 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 2
                }
            };
            using var v3 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 3
                }
            };
            var result = new VARIANT[] { v1, v2, v3 };
            SAFEARRAY *psa = CreateSafeArray(VARENUM.VARIANT, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.VARIANT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType<object[]>(array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(new object[] { 1, 2, 3 }, array);
            });
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYVARIANTSingleDimensionNonZeroLowerBound_ReturnsExpected()
        {
            using var v1 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 1
                }
            };
            using var v2 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 2
                }
            };
            using var v3 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 3
                }
            };
            var result = new VARIANT[] { v1, v2, v3 };
            SAFEARRAY *psa = CreateSafeArray(VARENUM.VARIANT, result, 1);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.VARIANT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(object).MakeArrayType(1), array);
                Assert.Equal(1, array.Rank);
                Assert.Equal(1, array.GetLowerBound(0));
                Assert.Equal(result.Length, array.GetLength(0));
                Assert.Equal(new object[] { 1, 2, 3 }, array);
            });
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYVARIANTMultiDimension_ReturnsExpected()
        {
            using var v1 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 1
                }
            };
            using var v2 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 2
                }
            };
            using var v3 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 3
                }
            };
            using var v4 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 4
                }
            };
            using var v5 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 5
                }
            };
            using var v6 = new VARIANT
            {
                vt = VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    llVal = 6
                }
            };

            var result = new VARIANT[2,3] { { v1, v2, v3 }, { v4, v5, v6 } };
            SAFEARRAY *psa = CreateSafeArray(VARENUM.VARIANT, result);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.VARIANT,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObject(variant, value =>
            {
                Array array = (Array)value;
                Assert.IsType(typeof(object).MakeArrayType(2), array);
                Assert.Equal(2, array.Rank);
                Assert.Equal(0, array.GetLowerBound(0));
                Assert.Equal(0, array.GetLowerBound(1));
                Assert.Equal(result.GetLength(0), array.GetLength(0));
                Assert.Equal(result.GetLength(1), array.GetLength(1));
                Assert.Equal(new object[,] { { 1, 2, 3 }, { 4, 5, 6 } }, array);
            });
        }

        [StaTheory]
        [InlineData((ushort)VARENUM.I1)]
        [InlineData((ushort)VARENUM.UI1)]
        [InlineData((ushort)VARENUM.I2)]
        [InlineData((ushort)VARENUM.UI2)]
        [InlineData((ushort)VARENUM.I4)]
        [InlineData((ushort)VARENUM.UI4)]
        [InlineData((ushort)VARENUM.I8)]
        [InlineData((ushort)VARENUM.UI8)]
        [InlineData((ushort)VARENUM.BSTR)]
        [InlineData((ushort)VARENUM.LPWSTR)]
        [InlineData((ushort)VARENUM.LPSTR)]
        [InlineData((ushort)VARENUM.UNKNOWN)]
        [InlineData((ushort)VARENUM.DISPATCH)]
        [InlineData((ushort)VARENUM.EMPTY)]
        [InlineData((ushort)VARENUM.NULL)]
        [InlineData((ushort)VARENUM.CF)]
        [InlineData((ushort)VARENUM.VOID)]
        [InlineData((ushort)VARENUM.PTR)]
        [InlineData((ushort)VARENUM.SAFEARRAY)]
        [InlineData((ushort)VARENUM.CARRAY)]
        [InlineData((ushort)VARENUM.RECORD)]
        [InlineData((ushort)VARENUM.BLOB)]
        [InlineData((ushort)VARENUM.STREAM)]
        [InlineData((ushort)VARENUM.STORAGE)]
        [InlineData((ushort)VARENUM.STREAMED_OBJECT)]
        [InlineData((ushort)VARENUM.STORED_OBJECT)]
        [InlineData((ushort)VARENUM.BLOB_OBJECT)]
        public void VARIANT_ToObject_ARRAYNoData_ReturnsExpected(ushort vt)
        {
            SAFEARRAY* psa = CreateSafeArray(VARENUM.I1, Array.Empty<byte>());
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | (VARENUM)vt
            };
            AssertToObjectEqual(null, variant);
        }

        [StaTheory]
        [InlineData(128)]
        [InlineData(129)]
        [InlineData((ushort)VARENUM.BSTR_BLOB)]
        public void VARIANT_ToObject_ARRAYInvalidTypeNoData_ThrowsInvalidOleVariantTypeException(ushort vt)
        {
            SAFEARRAY* psa = CreateSafeArray(VARENUM.I1, Array.Empty<byte>());
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | (VARENUM)vt
            };
            AssertToObjectThrows<InvalidOleVariantTypeException>(variant);
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYVECTOR_ThrowsInvalidOleVariantTypeException()
        {
            SAFEARRAY* psa = CreateSafeArray(VARENUM.I1, Array.Empty<byte>());
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.VECTOR | VARENUM.I4
            };
            Assert.Throws<ArgumentException>(null, () => variant.ToObject());
        }

        [StaFact]
        public void VARIANT_ToObject_ARRAYTypeEMPTY_ThrowsInvalidOleVariantTypeException()
        {
            SAFEARRAY* psa = CreateSafeArray(VARENUM.I1, Array.Empty<byte>());
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.EMPTY,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObjectThrows<InvalidOleVariantTypeException>(variant);
        }

        [StaTheory]
        [InlineData((ushort)VARENUM.I1, (ushort)VARENUM.UI1)]
        [InlineData((ushort)VARENUM.UI1, (ushort)VARENUM.I1)]
        [InlineData((ushort)VARENUM.I2, (ushort)VARENUM.UI2)]
        [InlineData((ushort)VARENUM.UI2, (ushort)VARENUM.I2)]
        [InlineData((ushort)VARENUM.I4, (ushort)VARENUM.UI4)]
        [InlineData((ushort)VARENUM.UI4, (ushort)VARENUM.I4)]
        [InlineData((ushort)VARENUM.INT, (ushort)VARENUM.UINT)]
        [InlineData((ushort)VARENUM.INT, (ushort)VARENUM.I2)]
        [InlineData((ushort)VARENUM.INT, (ushort)VARENUM.I8)]
        [InlineData((ushort)VARENUM.UINT, (ushort)VARENUM.INT)]
        [InlineData((ushort)VARENUM.UINT, (ushort)VARENUM.UI2)]
        [InlineData((ushort)VARENUM.UINT, (ushort)VARENUM.UI8)]
        [InlineData((ushort)VARENUM.I8, (ushort)VARENUM.UI8)]
        [InlineData((ushort)VARENUM.UI8, (ushort)VARENUM.I8)]
        [InlineData((ushort)VARENUM.UNKNOWN, (ushort)VARENUM.DISPATCH)]
        [InlineData((ushort)VARENUM.UNKNOWN, (ushort)VARENUM.I4)]
        [InlineData((ushort)VARENUM.UNKNOWN, (ushort)VARENUM.UI4)]
        [InlineData((ushort)VARENUM.UNKNOWN, (ushort)VARENUM.I8)]
        [InlineData((ushort)VARENUM.UNKNOWN, (ushort)VARENUM.UI8)]
        [InlineData((ushort)VARENUM.DISPATCH, (ushort)VARENUM.I4)]
        [InlineData((ushort)VARENUM.DISPATCH, (ushort)VARENUM.UI4)]
        [InlineData((ushort)VARENUM.DISPATCH, (ushort)VARENUM.I8)]
        [InlineData((ushort)VARENUM.DISPATCH, (ushort)VARENUM.UI8)]
        public void VARIANT_ToObject_ARRAYTypeDifferent_ThrowsSafeArrayTypeMismatchException(ushort arrayVt, ushort vt)
        {
            SAFEARRAY* psa = CreateSafeArray((VARENUM)arrayVt, Array.Empty<byte>());
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | (VARENUM)vt,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObjectThrows<SafeArrayTypeMismatchException>(variant);
        }

        [StaTheory]
        [InlineData((ushort)VARENUM.INT_PTR)]
        [InlineData((ushort)VARENUM.UINT_PTR)]
        public void VARIANT_ToObject_ARRAYTypeInvalid_ThrowsArgumentException(ushort vt)
        {
            SAFEARRAY* psa = CreateSafeArray((VARENUM)vt, Array.Empty<byte>());
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | (VARENUM)vt,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObjectThrows<ArgumentException>(variant);
        }

        [StaTheory]
        [InlineData(33)]
        [InlineData(255)]
        [InlineData(256)]
        [InlineData(512)]
        public void VARIANT_ARRAYBigRank_ThrowsTypeLoadException(int rank)
        {
            SAFEARRAYBOUND* saBounds = stackalloc SAFEARRAYBOUND[rank];
            for (uint i = 0; i < rank; i++)
            {
                saBounds[i] = new SAFEARRAYBOUND
                {
                    cElements = 0,
                    lLbound = 0
                };
            }

            SAFEARRAY *psa = SafeArrayCreate(VARENUM.I4, (uint)rank, saBounds);
            using var variant = new VARIANT
            {
                vt = VARENUM.ARRAY | VARENUM.I4,
                data = new VARIANT.VARIANTUnion
                {
                    parray = psa
                }
            };
            AssertToObjectThrows<TypeLoadException>(variant);
        }

        private unsafe static SAFEARRAY* CreateSafeArray<T>(VARENUM vt, T[] result, int lbound = 0) where T : unmanaged
        {
            var saBound = new SAFEARRAYBOUND
            {
                cElements = (uint)result.Length,
                lLbound = lbound
            };
            SAFEARRAY *psa = SafeArrayCreate(vt, 1, &saBound);
            Assert.True(psa != null);

            VARENUM arrayVt = VARENUM.EMPTY;
            HRESULT hr = SafeArrayGetVartype(psa, &arrayVt);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(vt, arrayVt);

            for (int i = 0; i < result.Length; i++)
            {
                T value = result[i];
                int index = i + lbound;
                // Insert pointers directly.
                if (value is IntPtr valuePtr)
                {
                    hr = SafeArrayPutElement(psa, &index, (void*)valuePtr);
                }
                else
                {
                    hr = SafeArrayPutElement(psa, &index, &value);
                }
                Assert.Equal(HRESULT.S_OK, hr);
            }

            return psa;
        }

        private unsafe SAFEARRAY* CreateSafeArray<T>(VARENUM vt, T[,] multiDimArray, int lbound1 = 0, int lbound2 = 0) where T : unmanaged
        {
            SAFEARRAYBOUND* saBounds = stackalloc SAFEARRAYBOUND[2];
            saBounds[0] = new SAFEARRAYBOUND
            {
                cElements = (uint)multiDimArray.GetLength(0),
                lLbound = lbound1
            };
            saBounds[1] = new SAFEARRAYBOUND
            {
                cElements = (uint)multiDimArray.GetLength(1),
                lLbound = lbound2
            };
            SAFEARRAY *psa = SafeArrayCreate(vt, 2, saBounds);
            Assert.True(psa != null);

            VARENUM arrayVt = VARENUM.EMPTY;
            HRESULT hr = SafeArrayGetVartype(psa, &arrayVt);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(vt, arrayVt);

            for (int i = 0; i < multiDimArray.GetLength(0); i++)
            {
                for (int j = 0; j < multiDimArray.GetLength(1); j++)
                {
                    int* indices = stackalloc int[] { i + lbound1, j + lbound2 };
                    T value = multiDimArray[i, j];
                    // Insert pointers directly.
                    if (value is IntPtr valuePtr)
                    {
                        hr = SafeArrayPutElement(psa, indices, (void*)valuePtr);
                    }
                    else
                    {
                        hr = SafeArrayPutElement(psa, indices, &value);
                    }
                    Assert.Equal(HRESULT.S_OK, hr);
                }
            }

            return psa;
        }

        [StaFact]
        public void ToObject_RECORDRecordData_ReturnsExpected()
        {
            int record = 1;
            IntPtr mem = Marshal.AllocCoTaskMem(sizeof(int));
            (*(int*)mem) = record;
            var recordInfo = new CustomRecordInfo
            {
                GetGuidAction = () => (typeof(int).GUID, HRESULT.S_OK)
            };
            IntPtr pRecordInfo = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(recordInfo);

            using var variant = new VARIANT
            {
                vt = VARENUM.RECORD,
                data = new VARIANT.VARIANTUnion
                {
                    recordVal = new VARIANT.VARIANTRecord
                    {
                        pRecInfo = pRecordInfo,
                        pvRecord = mem.ToPointer()
                    }
                }
            };
            // Records actually don't work in .NET Core...
#if false
            AssertToObjectEqual(1, variant);
#else
            AssertToObjectThrows<ArgumentException>(variant);
#endif
        }

        [StaFact]
        public void ToObject_RECORDNullRecordData_ReturnsNull()
        {
            var recordInfo = new CustomRecordInfo();
            IntPtr pRecordInfo = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(recordInfo);
            using var variant = new VARIANT
            {
                vt = VARENUM.RECORD,
                data = new VARIANT.VARIANTUnion
                {
                    recordVal = new VARIANT.VARIANTRecord
                    {
                        pRecInfo = pRecordInfo
                    }
                }
            };
            AssertToObjectEqual(null, variant);
        }

        [StaFact]
        public void ToObject_RECORDNullRecordInfo_ThrowsArgumentException()
        {
            int record = 1;
            IntPtr mem = Marshal.AllocCoTaskMem(sizeof(int));
            (*(int*)mem) = record;

            using var variant = new VARIANT
            {
                vt = VARENUM.RECORD,
                data = new VARIANT.VARIANTUnion
                {
                    recordVal = new VARIANT.VARIANTRecord
                    {
                        pRecInfo = IntPtr.Zero,
                        pvRecord = mem.ToPointer()
                    }
                }
            };
            AssertToObjectThrows<ArgumentException>(variant);
        }

        [StaFact]
        public void ToObject_RECORDInvalidGetGuidHRData_ThrowsArgumentException()
        {
            int record = 1;
            IntPtr mem = Marshal.AllocCoTaskMem(sizeof(int));
            (*(int*)mem) = record;

            var recordInfo = new CustomRecordInfo
            {
                GetGuidAction = () => (Guid.Empty, HRESULT.DISP_E_DIVBYZERO)
            };
            IntPtr pRecordInfo = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(recordInfo);
            using var variant = new VARIANT
            {
                vt = VARENUM.RECORD,
                data = new VARIANT.VARIANTUnion
                {
                    recordVal = new VARIANT.VARIANTRecord
                    {
                        pRecInfo = pRecordInfo,
                        pvRecord = mem.ToPointer()
                    }
                }
            };
            // Records actually don't work in .NET Core...
#if false
            AssertToObjectThrows<DivideByZeroException>(variant);
#endif
        }

        [StaFact]
        public void ToObject_RECORDInvalidGetGuidHRNoData_ReturnsNull()
        {
            var record = new CustomRecordInfo
            {
                GetGuidAction = () => (Guid.Empty, HRESULT.DISP_E_DIVBYZERO)
            };
            IntPtr pRecordInfo = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(record);
            using var variant = new VARIANT
            {
                vt = VARENUM.RECORD,
                data = new VARIANT.VARIANTUnion
                {
                    recordVal = new VARIANT.VARIANTRecord
                    {
                        pRecInfo = pRecordInfo,
                    }
                }
            };
            AssertToObjectEqual(null, variant);
        }

        public static IEnumerable<object[]> RECORD_TestData()
        {
            yield return new object[] { Guid.Empty };
            yield return new object[] { new Guid("8856f961-340a-11d0-a96b-00c04fd705a2") };
        }

        [StaTheory]
        [MemberData(nameof(RECORD_TestData))]
        public void ToObject_RECORDInvalidGuidData_ThrowsArgumentException(Guid guid)
        {
            int record = 1;
            IntPtr mem = Marshal.AllocCoTaskMem(sizeof(int));
            (*(int*)mem) = record;

            var recordInfo = new CustomRecordInfo
            {
                GetGuidAction = () => (guid, HRESULT.S_OK)
            };
            IntPtr pRecordInfo = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(recordInfo);
            using var variant = new VARIANT
            {
                vt = VARENUM.RECORD,
                data = new VARIANT.VARIANTUnion
                {
                    recordVal = new VARIANT.VARIANTRecord
                    {
                        pRecInfo = pRecordInfo,
                        pvRecord = mem.ToPointer()
                    }
                }
            };
            AssertToObjectThrows<ArgumentException>(variant);
        }

        [StaTheory]
        [MemberData(nameof(RECORD_TestData))]
        public void ToObject_RECORDInvalidGuidNoData_ReturnsNull(Guid guid)
        {
            var record = new CustomRecordInfo
            {
                GetGuidAction = () => (guid, HRESULT.S_OK)
            };
            IntPtr pRecordInfo = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(record);
            try
            {
                using var variant = new VARIANT
                {
                    vt = VARENUM.RECORD,
                    data = new VARIANT.VARIANTUnion
                    {
                        recordVal = new VARIANT.VARIANTRecord
                        {
                            pRecInfo = pRecordInfo,
                        }
                    }
                };
                AssertToObjectEqual(null, variant);
            }
            finally
            {
                Marshal.Release(pRecordInfo);
            }
        }

        [StaFact]
        public void ToObject_RECORDARRAYValid_ReturnsExpected()
        {
            var result = new int[] { 1, 2 };
            var recordInfo = new CustomRecordInfo
            {
                GetGuidAction = () => (typeof(int).GUID, HRESULT.S_OK)
            };
            IntPtr pRecordInfo = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(recordInfo);
            try
            {
                SAFEARRAY *psa = CreateRecordSafeArray(result, pRecordInfo);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.RECORD,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                // Records actually don't work in .NET Core...
#if false
                AssertToObjectEqual(new int[] { 0, 0 }, variant);
#endif
            }
            finally
            {
                Marshal.Release(pRecordInfo);
            }
        }

        [StaFact]
        public void ToObject_RECORDARRAYInvalidFFeatures_ThrowsArgumentException()
        {
            var result = new int[] { 1, 2 };
            var record = new CustomRecordInfo();
            IntPtr pRecordInfo = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(record);
            try
            {
                SAFEARRAY *psa = CreateRecordSafeArray(result, pRecordInfo);
                psa->fFeatures &= ~FADF.RECORD;
                try
                {
                    using var variant = new VARIANT
                    {
                        vt = VARENUM.ARRAY | VARENUM.RECORD,
                        data = new VARIANT.VARIANTUnion
                        {
                            parray = psa
                        }
                    };
                    AssertToObjectThrows<ArgumentException>(variant);
                }
                finally
                {
                    // Make sure disposal works.
                    psa->fFeatures |= FADF.RECORD;
                }
            }
            finally
            {
                Marshal.Release(pRecordInfo);
            }
        }

        [StaFact]
        public void ToObject_RECORDARRAYInvalidGetGuidHR_ThrowsArgumentException()
        {
            var result = new int[] { 1, 2 };
            var record = new CustomRecordInfo
            {
                GetGuidAction = () => (Guid.Empty, HRESULT.DISP_E_DIVBYZERO)
            };
            IntPtr pRecordInfo = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(record);
            try
            {
                SAFEARRAY *psa = CreateRecordSafeArray(result, pRecordInfo);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.RECORD,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObjectThrows<DivideByZeroException>(variant);
            }
            finally
            {
                Marshal.Release(pRecordInfo);
            }
        }

        public static IEnumerable<object[]> RECORDARRAY_InvalidGuid_TestData()
        {
            yield return new object[] { Guid.Empty };
            yield return new object[] { new Guid("8856f961-340a-11d0-a96b-00c04fd705a2") };
        }

        [StaTheory]
        [MemberData(nameof(RECORDARRAY_InvalidGuid_TestData))]
        public void ToObject_RECORDARRAY_InvokeInvalidGuid_ThrowsArgumentException(Guid guid)
        {
            var result = new int[] { 1, 2 };
            var record = new CustomRecordInfo
            {
                GetGuidAction = () => (guid, HRESULT.S_OK)
            };
            IntPtr pRecordInfo = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(record);
            try
            {
                SAFEARRAY *psa = CreateRecordSafeArray(result, pRecordInfo);
                using var variant = new VARIANT
                {
                    vt = VARENUM.ARRAY | VARENUM.RECORD,
                    data = new VARIANT.VARIANTUnion
                    {
                        parray = psa
                    }
                };
                AssertToObjectThrows<ArgumentException>(variant);
            }
            finally
            {
                Marshal.Release(pRecordInfo);
            }
        }

        private class CustomRecordInfo : IRecordInfo
        {
            HRESULT IRecordInfo.RecordInit(void* pvNew) => throw new NotImplementedException();

            HRESULT IRecordInfo.RecordClear(void* pvExisting) => throw new NotImplementedException();

            HRESULT IRecordInfo.RecordCopy(void* pvExisting, void* pvNew) => throw new NotImplementedException();

            public Func<(Guid, HRESULT)> GetGuidAction { get; set; }

            HRESULT IRecordInfo.GetGuid(Guid* pguid)
            {
                (Guid guid, HRESULT hr) = GetGuidAction();
                *pguid = guid;
                return hr;
            }

            HRESULT IRecordInfo.GetName(BSTR* pbstrName) => throw new NotImplementedException();

            HRESULT IRecordInfo.GetSize(uint* pcbSize)
            {
                *pcbSize = (uint)sizeof(int);
                return HRESULT.S_OK;
            }

            HRESULT IRecordInfo.GetTypeInfo(out ITypeInfo ppTypeInfo) => throw new NotImplementedException();

            HRESULT IRecordInfo.GetField(void* pvData, out string szFieldName, VARIANT* pvarField) => throw new NotImplementedException();

            HRESULT IRecordInfo.GetFieldNoCopy(void* pvData, out string szFieldName, VARIANT* pvarField, void* ppvDataCArray) => throw new NotImplementedException();

            HRESULT IRecordInfo.PutField(INVOKEKIND wFlags, void* pvData, out string szFieldName, VARIANT* pvarField) => throw new NotImplementedException();

            HRESULT IRecordInfo.PutFieldNoCopy(INVOKEKIND wFlags, void* pvData, out string szFieldName, VARIANT* pvarField) => throw new NotImplementedException();

            HRESULT IRecordInfo.GetFieldNames(uint* pcNames, BSTR* rgBstrNames) => throw new NotImplementedException();

            BOOL IRecordInfo.IsMatchingType(ref IRecordInfo pRecordInfoInfo) => throw new NotImplementedException();

            void* IRecordInfo.RecordCreate() => throw new NotImplementedException();

            HRESULT IRecordInfo.RecordCreateCopy(void* pvSource, void** ppvDest) => throw new NotImplementedException();

            HRESULT IRecordInfo.RecordDestroy(void* pvRecord) => throw new NotImplementedException();
        }

        private static SAFEARRAY* CreateRecordSafeArray<T>(T[] result, IntPtr recordInfo, int lbound = 0)
        {
            var saBound = new SAFEARRAYBOUND
            {
                cElements = (uint)result.Length,
                lLbound = lbound
            };
            SAFEARRAY *psa = SafeArrayCreateEx(VARENUM.RECORD, 1, &saBound, recordInfo);
            Assert.True(psa != null);

            VARENUM arrayVt = VARENUM.EMPTY;
            HRESULT hr = SafeArrayGetVartype(psa, &arrayVt);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VARENUM.RECORD, arrayVt);

            return psa;
        }

        private static void AssertToObjectThrows<T>(VARIANT variant) where T : Exception
        {
            VARIANT copy = variant;
            IntPtr pv = (IntPtr)(&copy);
            Assert.Throws<T>(() => Marshal.GetObjectForNativeVariant(pv));

            Assert.Throws<T>(() => variant.ToObject());
        }

        private static void AssertToObjectEqual(object expected, VARIANT variant)
            => AssertToObject(variant, actual => Assert.Equal(expected, actual));

        private static void AssertToObjectEqualExtension<T>(object expected, VARIANT variant) where T : Exception
        {
            // Not supported type.
            VARIANT copy = variant;
            IntPtr pv = (IntPtr)(&copy);
            Assert.Throws<T>(() => Marshal.GetObjectForNativeVariant(pv));

            Assert.Equal(expected, variant.ToObject());
        }

        private static void AssertToObject(VARIANT variant, Action<object> action)
        {
            IntPtr pv = (IntPtr)(&variant);
            action(Marshal.GetObjectForNativeVariant(pv));

            action(variant.ToObject());
        }

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromCLSID(Guid* clsid, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromFileTime(FILETIME* pftIn, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitVariantFromFileTime(FILETIME* pftIn, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromBuffer(void* pv, uint cb, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromInt16Vector(void* pv, uint cb, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromUInt16Vector(void* pv, uint cb, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromBooleanVector(void* pv, uint cb, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromInt32Vector(void* pv, uint cb, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromUInt32Vector(void* pv, uint cb, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromInt64Vector(void* pv, uint cb, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromUInt64Vector(void* pv, uint cb, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromDoubleVector(void* pv, uint cb, VARIANT* ppropvar);

        [DllImport(Libraries.Propsys, ExactSpelling = true)]
        private unsafe static extern HRESULT InitPropVariantFromFileTimeVector(void* pv, uint cb, VARIANT* ppropvar);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private static unsafe extern SAFEARRAY* SafeArrayCreate(VARENUM vt, uint cDims, SAFEARRAYBOUND* rgsabound);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private static unsafe extern SAFEARRAY* SafeArrayCreateEx(VARENUM vt, uint cDims, SAFEARRAYBOUND* rgsabound, IntPtr pvExtra);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private static unsafe extern HRESULT SafeArrayDestroy(SAFEARRAY* psa);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private unsafe static extern HRESULT SafeArrayPutElement(SAFEARRAY* psa, int* rgIndices, void* pv);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private static extern HRESULT VarDecFromI8(long i64In, out DECIMAL pdecOut);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private static extern HRESULT VarDecFromR8(double dblIn, out DECIMAL pdecOut);
    }
}
