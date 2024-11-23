// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;
using static Windows.Win32.System.Com.ADVANCED_FEATURE_FLAGS;
using static Windows.Win32.System.Variant.VARENUM;

namespace System.Windows.Forms.Tests.Interop.Oleaut32;

public unsafe class VARIANTTests
{
    private static VARIANT Create(VARENUM type)
        => new() { vt = type };

    private static VARIANT Create(VARENUM type, void* value)
        => new()
        {
            vt = type,
            data = new() { byref = value }
        };

    private static VARIANT Create(bool value)
        => new()
        {
            vt = VT_BOOL,
            data = new() { boolVal = value ? VARIANT_BOOL.VARIANT_TRUE : VARIANT_BOOL.VARIANT_FALSE }
        };

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
    public void VARIANT_Sizeof_InvokeX86_ReturnsExpected()
    {
        if (Environment.Is64BitProcess)
        {
            return;
        }

        Assert.Equal(16, Marshal.SizeOf<VARIANT>());
        Assert.Equal(16, sizeof(VARIANT));
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
    public void VARIANT_Sizeof_InvokeX64_ReturnsExpected()
    {
        if (!Environment.Is64BitProcess)
        {
            return;
        }

        Assert.Equal(24, Marshal.SizeOf<VARIANT>());
        Assert.Equal(24, sizeof(VARIANT));
    }

    [StaTheory]
    [InlineData((ushort)VT_EMPTY, false)]
    [InlineData((ushort)VT_BOOL, false)]
    [InlineData((ushort)(VT_BYREF), true)]
    [InlineData((ushort)(VT_BOOL | VT_BYREF), true)]
    [InlineData((ushort)(VT_BOOL | VT_BYREF | VT_ARRAY), true)]
    [InlineData((ushort)(VT_BOOL | VT_BYREF | VT_VECTOR), true)]
    [InlineData((ushort)(VT_BOOL | VT_ARRAY), false)]
    [InlineData((ushort)(VT_BOOL | VT_VECTOR), false)]
    public void VARIANT_Byref_Get_ReturnsExpected(ushort vt, bool expected)
    {
        using VARIANT variant = Create((VARENUM)vt);
        Assert.Equal(expected, variant.Byref);
    }

    [StaTheory]
    [InlineData((ushort)VT_EMPTY, (ushort)VT_EMPTY)]
    [InlineData((ushort)VT_BOOL, (ushort)VT_BOOL)]
    [InlineData((ushort)(VT_BYREF), (ushort)VT_EMPTY)]
    [InlineData((ushort)(VT_BOOL | VT_BYREF), (ushort)VT_BOOL)]
    public void VARIANT_Type_Get_ReturnsExpected(ushort vt, ushort expected)
    {
        using VARIANT variant = Create((VARENUM)vt);
        Assert.Equal((VARENUM)expected, variant.Type);
    }

    [StaTheory]
    [InlineData((ushort)VT_EMPTY)]
    [InlineData((ushort)(VT_EMPTY | VT_BYREF))]
    [InlineData((ushort)VT_UNKNOWN)]
    [InlineData((ushort)(VT_UNKNOWN | VT_BYREF))]
    [InlineData((ushort)VT_DISPATCH)]
    [InlineData((ushort)(VT_DISPATCH | VT_BYREF))]
    [InlineData((ushort)VT_BSTR)]
    [InlineData((ushort)(VT_BSTR | VT_BYREF))]
    [InlineData((ushort)VT_BOOL)]
    [InlineData((ushort)(VT_BOOL | VT_BYREF))]
    public void VARIANT_Clear_InvokeDefault_Success(ushort vt)
    {
        using VARIANT variant = Create((VARENUM)vt);
        variant.Clear();
        Assert.Equal(VT_EMPTY, variant.vt);
        Assert.True(variant.Anonymous.Anonymous.Anonymous.punkVal is null);
    }

    [StaFact]
    public void VARIANT_Clear_InvokeCustom_Success()
    {
        using VARIANT variant = Create(true);
        variant.Clear();
        Assert.Equal(VT_EMPTY, variant.vt);
        Assert.True(variant.Anonymous.Anonymous.Anonymous.punkVal is null);
    }

    [StaFact]
    public void VARIANT_Clear_InvokeBSTR_Success()
    {
        using VARIANT variant = new()
        {
            vt = VT_BSTR,
            data = new() { bstrVal = new BSTR("abc") }
        };

        variant.Clear();
        Assert.Equal(VT_EMPTY, variant.vt);
        Assert.True(variant.Anonymous.Anonymous.Anonymous.pbstrVal is null);
    }

    [StaTheory]
    [InlineData((ushort)VT_EMPTY)]
    [InlineData((ushort)(VT_EMPTY | VT_BYREF))]
    [InlineData((ushort)VT_UNKNOWN)]
    [InlineData((ushort)(VT_UNKNOWN | VT_BYREF))]
    [InlineData((ushort)VT_DISPATCH)]
    [InlineData((ushort)(VT_DISPATCH | VT_BYREF))]
    [InlineData((ushort)VT_BSTR)]
    [InlineData((ushort)(VT_BSTR | VT_BYREF))]
    [InlineData((ushort)VT_BOOL)]
    [InlineData((ushort)(VT_BOOL | VT_BYREF))]
    public void VARIANT_Dispose_InvokeDefault_Success(ushort vt)
    {
        using VARIANT variant = Create((VARENUM)vt);
        variant.Dispose();
        Assert.Equal(VT_EMPTY, variant.vt);
        Assert.True(variant.Anonymous.Anonymous.Anonymous.punkVal is null);
    }

    [StaFact]
    public void VARIANT_Dispose_InvokeCustom_Success()
    {
        using VARIANT variant = Create(true);
        variant.Dispose();
        Assert.Equal(VT_EMPTY, variant.vt);
        Assert.True(variant.Anonymous.Anonymous.Anonymous.punkVal is null);
    }

    [StaFact]
    public void VARIANT_Dispose_InvokeBSTR_Success()
    {
        VARIANT variant = new()
        {
            vt = VT_BSTR,
            data = new() { bstrVal = new BSTR("abc") }
        };

        variant.Dispose();
        Assert.Equal(VT_EMPTY, variant.vt);
        Assert.True(variant.Anonymous.Anonymous.Anonymous.pbstrVal is null);
    }

    public static IEnumerable<object[]> ToObject_TestData()
    {
        if (nint.Size == 8)
        {
            yield return new object[] { VT_I1, unchecked((nint)long.MinValue), (sbyte)0 };
        }

        yield return new object[] { VT_I1, (nint)int.MinValue, (sbyte)0 };
        yield return new object[] { VT_I1, (nint)short.MinValue, (sbyte)0 };
        yield return new object[] { VT_I1, (nint)sbyte.MinValue, sbyte.MinValue };
        yield return new object[] { VT_I1, (nint)(-10), (sbyte)(-10) };
        yield return new object[] { VT_I1, (nint)0, (sbyte)0 };
        yield return new object[] { VT_I1, (nint)10, (sbyte)10 };
        yield return new object[] { VT_I1, (nint)sbyte.MaxValue, sbyte.MaxValue };
        yield return new object[] { VT_I1, (nint)short.MaxValue, (sbyte)(-1) };
        yield return new object[] { VT_I1, (nint)int.MaxValue, (sbyte)(-1) };
        if (nint.Size == 8)
        {
            yield return new object[] { VT_I1, unchecked((nint)long.MaxValue), (sbyte)(-1) };
        }

        yield return new object[] { VT_UI1, (nint)(-10), (byte)246 };
        yield return new object[] { VT_UI1, (nint)0, (byte)0 };
        yield return new object[] { VT_UI1, (nint)10, (byte)10 };
        yield return new object[] { VT_UI1, (nint)byte.MaxValue, byte.MaxValue };
        yield return new object[] { VT_UI1, (nint)ushort.MaxValue, byte.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { VT_UI1, unchecked((nint)uint.MaxValue), byte.MaxValue };
        }

        yield return new object[] { VT_UI1, (nint)(-1), byte.MaxValue };

        if (nint.Size == 8)
        {
            yield return new object[] { VT_I2, unchecked((nint)long.MinValue), (short)0 };
        }

        yield return new object[] { VT_I2, (nint)int.MinValue, (short)0 };
        yield return new object[] { VT_I2, (nint)short.MinValue, short.MinValue };
        yield return new object[] { VT_I2, (nint)sbyte.MinValue, (short)sbyte.MinValue };
        yield return new object[] { VT_I2, (nint)(-10), (short)(-10) };
        yield return new object[] { VT_I2, (nint)0, (short)0 };
        yield return new object[] { VT_I2, (nint)10, (short)10 };
        yield return new object[] { VT_I2, (nint)sbyte.MaxValue, (short)sbyte.MaxValue };
        yield return new object[] { VT_I2, (nint)short.MaxValue, short.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { VT_I2, unchecked((nint)long.MaxValue), (short)(-1) };
        }

        yield return new object[] { VT_UI2, (nint)(-10), (ushort)65526 };
        yield return new object[] { VT_UI2, (nint)0, (ushort)0 };
        yield return new object[] { VT_UI2, (nint)10, (ushort)10 };
        yield return new object[] { VT_UI2, (nint)byte.MaxValue, (ushort)byte.MaxValue };
        yield return new object[] { VT_UI2, (nint)ushort.MaxValue, ushort.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { VT_UI2, unchecked((nint)uint.MaxValue), ushort.MaxValue };
        }

        yield return new object[] { VT_UI2, (nint)(-1), ushort.MaxValue };

        if (nint.Size == 8)
        {
            yield return new object[] { VT_I4, unchecked((nint)long.MinValue), 0 };
        }

        yield return new object[] { VT_I4, (nint)int.MinValue, int.MinValue };
        yield return new object[] { VT_I4, (nint)short.MinValue, (int)short.MinValue };
        yield return new object[] { VT_I4, (nint)sbyte.MinValue, (int)sbyte.MinValue };
        yield return new object[] { VT_I4, (nint)(-10), -10 };
        yield return new object[] { VT_I4, (nint)0, 0 };
        yield return new object[] { VT_I4, (nint)10, 10 };
        yield return new object[] { VT_I4, (nint)sbyte.MaxValue, (int)sbyte.MaxValue };
        yield return new object[] { VT_I4, (nint)short.MaxValue, (int)short.MaxValue };
        yield return new object[] { VT_I4, (nint)int.MaxValue, int.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { VT_I4, unchecked((nint)long.MaxValue), -1 };
        }

        yield return new object[] { VT_UI4, (nint)(-10), 4294967286 };
        yield return new object[] { VT_UI4, (nint)0, (uint)0 };
        yield return new object[] { VT_UI4, (nint)10, (uint)10 };
        yield return new object[] { VT_UI4, (nint)byte.MaxValue, (uint)byte.MaxValue };
        yield return new object[] { VT_UI4, (nint)ushort.MaxValue, (uint)ushort.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { VT_UI4, unchecked((nint)uint.MaxValue), uint.MaxValue };
        }

        yield return new object[] { VT_UI4, (nint)(-1), uint.MaxValue };

        if (nint.Size == 8)
        {
            yield return new object[] { VT_INT, unchecked((nint)long.MinValue), 0 };
        }

        yield return new object[] { VT_INT, (nint)int.MinValue, int.MinValue };
        yield return new object[] { VT_INT, (nint)short.MinValue, (int)short.MinValue };
        yield return new object[] { VT_INT, (nint)sbyte.MinValue, (int)sbyte.MinValue };
        yield return new object[] { VT_INT, (nint)(-10), -10 };
        yield return new object[] { VT_INT, (nint)0, 0 };
        yield return new object[] { VT_INT, (nint)10, 10 };
        yield return new object[] { VT_INT, (nint)sbyte.MaxValue, (int)sbyte.MaxValue };
        yield return new object[] { VT_INT, (nint)short.MaxValue, (int)short.MaxValue };
        yield return new object[] { VT_INT, (nint)int.MaxValue, int.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { VT_INT, unchecked((nint)long.MaxValue), -1 };
        }

        yield return new object[] { VT_UINT, (nint)(-10), 4294967286 };
        yield return new object[] { VT_UINT, (nint)0, (uint)0 };
        yield return new object[] { VT_UINT, (nint)10, (uint)10 };
        yield return new object[] { VT_UINT, (nint)byte.MaxValue, (uint)byte.MaxValue };
        yield return new object[] { VT_UINT, (nint)ushort.MaxValue, (uint)ushort.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { VT_UINT, unchecked((nint)uint.MaxValue), uint.MaxValue };
        }

        yield return new object[] { VT_UINT, (nint)(-1), uint.MaxValue };

        yield return new object[] { VT_BOOL, (nint)(-1), true };
        yield return new object[] { VT_BOOL, (nint)0, false };
        yield return new object[] { VT_BOOL, (nint)1, true };

        if (nint.Size == 8)
        {
            yield return new object[] { VT_ERROR, unchecked((nint)long.MinValue), 0 };
        }

        yield return new object[] { VT_ERROR, (nint)int.MinValue, int.MinValue };
        yield return new object[] { VT_ERROR, (nint)short.MinValue, (int)short.MinValue };
        yield return new object[] { VT_ERROR, (nint)sbyte.MinValue, (int)sbyte.MinValue };
        yield return new object[] { VT_ERROR, (nint)(-10), -10 };
        yield return new object[] { VT_ERROR, (nint)0, 0 };
        yield return new object[] { VT_ERROR, (nint)10, 10 };
        yield return new object[] { VT_ERROR, (nint)sbyte.MaxValue, (int)sbyte.MaxValue };
        yield return new object[] { VT_ERROR, (nint)short.MaxValue, (int)short.MaxValue };
        yield return new object[] { VT_ERROR, (nint)int.MaxValue, int.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { VT_ERROR, unchecked((nint)long.MaxValue), -1 };
        }
    }

    [StaTheory]
    [MemberData(nameof(ToObject_TestData))]
    public void VARIANT_ToObject_Invoke_ReturnsExpected(ushort vt, nint data, object expected)
    {
        using VARIANT variant = Create((VARENUM)vt, (IUnknown*)data);
        AssertToObjectEqual(expected, variant);
    }

    [StaTheory]
    [MemberData(nameof(ToObject_TestData))]
    public void VARIANT_ToObject_InvokeBYREF_ReturnsExpected(ushort vt, nint data, object expected)
    {
        using VARIANT variant = Create((VARENUM)vt | VT_BYREF, (IUnknown*)&data);
        AssertToObjectEqual(expected, variant);
    }

    public static IEnumerable<object[]> BYREFNoData_TestData()
    {
        yield return new object[] { VT_I2 };
        yield return new object[] { VT_I4 };
        yield return new object[] { VT_R4 };
        yield return new object[] { VT_R8 };
        yield return new object[] { VT_CY };
        yield return new object[] { VT_DATE };
        yield return new object[] { VT_BSTR };
        yield return new object[] { VT_DISPATCH };
        yield return new object[] { VT_ERROR };
        yield return new object[] { VT_BOOL };
        yield return new object[] { VT_VARIANT };
        yield return new object[] { VT_UNKNOWN };
        yield return new object[] { VT_DECIMAL };
        yield return new object[] { VT_I1 };
        yield return new object[] { VT_UI1 };
        yield return new object[] { VT_UI2 };
        yield return new object[] { VT_UI4 };
        yield return new object[] { VT_I8 };
        yield return new object[] { VT_UI8 };
        yield return new object[] { VT_INT };
        yield return new object[] { VT_UINT };
        yield return new object[] { VT_VOID };
        yield return new object[] { VT_HRESULT };
        yield return new object[] { VT_PTR };
        yield return new object[] { VT_SAFEARRAY };
        yield return new object[] { VT_CARRAY };
        yield return new object[] { VT_USERDEFINED };
        yield return new object[] { VT_LPSTR };
        yield return new object[] { VT_LPWSTR };
        yield return new object[] { VT_RECORD };
        yield return new object[] { VT_INT_PTR };
        yield return new object[] { VT_UINT_PTR };
        yield return new object[] { VT_FILETIME };
        yield return new object[] { VT_BLOB };
        yield return new object[] { VT_STREAM };
        yield return new object[] { VT_STORAGE };
        yield return new object[] { VT_STREAMED_OBJECT };
        yield return new object[] { VT_STORED_OBJECT };
        yield return new object[] { VT_BLOB_OBJECT };
        yield return new object[] { VT_CF };
        yield return new object[] { VT_CLSID };
        yield return new object[] { VT_VERSIONED_STREAM };
        yield return new object[] { VT_BSTR_BLOB };
    }

    [StaTheory]
    [MemberData(nameof(BYREFNoData_TestData))]
    public void VARIANT_ToObject_BYREFNoData_Throws(ushort vt)
    {
        using VARIANT variant = Create((VARENUM)vt | VT_BYREF);
        AssertToObjectThrows<ArgumentException>(variant);
    }

    public static IEnumerable<object[]> ToObject_I8_TestData()
    {
        if (nint.Size == 8)
        {
            yield return new object[] { unchecked((nint)long.MinValue), long.MinValue };
            yield return new object[] { (nint)int.MinValue, (long)int.MinValue };
            yield return new object[] { (nint)short.MinValue, (long)short.MinValue };
            yield return new object[] { (nint)sbyte.MinValue, (long)sbyte.MinValue };
            yield return new object[] { (nint)(-10), (long)(-10) };
        }

        yield return new object[] { (nint)0, (long)0 };
        yield return new object[] { (nint)10, (long)10 };
        yield return new object[] { (nint)sbyte.MaxValue, (long)sbyte.MaxValue };
        yield return new object[] { (nint)short.MaxValue, (long)short.MaxValue };
        yield return new object[] { (nint)int.MaxValue, (long)int.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { unchecked((nint)long.MaxValue), long.MaxValue };
        }
    }

    [Theory]
    [MemberData(nameof(ToObject_I8_TestData))]
    public void VARIANT_ToObject_I8_ReturnsExpected(nint data, long expected)
    {
        using VARIANT variant = Create(VT_I8, (IUnknown*)data);
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
        using VARIANT variant = Create(VT_I8 | VT_BYREF, (IUnknown*)&data);
        AssertToObjectEqual(data, variant);
    }

    public static IEnumerable<object[]> ToObject_UI8_TestData()
    {
        if (nint.Size == 8)
        {
            yield return new object[] { (nint)(-10), 18446744073709551606 };
        }

        yield return new object[] { (nint)0, (ulong)0 };
        yield return new object[] { (nint)10, (ulong)10 };
        yield return new object[] { (nint)byte.MaxValue, (ulong)byte.MaxValue };
        yield return new object[] { (nint)ushort.MaxValue, (ulong)ushort.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { unchecked((nint)uint.MaxValue), (ulong)uint.MaxValue };
            yield return new object[] { (nint)(-1L), ulong.MaxValue };
        }
    }

    [Theory]
    [MemberData(nameof(ToObject_UI8_TestData))]
    public void VARIANT_ToObject_UI8_ReturnsExpected(nint data, ulong expected)
    {
        using VARIANT variant = Create(VT_UI8, (IUnknown*)data);
        AssertToObjectEqual(expected, variant);
    }

    public static IEnumerable<object[]> ToObject_UI8BYREF_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { 10 };
        yield return new object[] { byte.MaxValue };
        yield return new object[] { ushort.MaxValue };
        yield return new object[] { uint.MaxValue };
        yield return new object[] { ulong.MaxValue };
    }

    [Theory]
    [MemberData(nameof(ToObject_UI8BYREF_TestData))]
    public void VARIANT_ToObject_UI8BYREF_ReturnsExpected(ulong data)
    {
        using VARIANT variant = Create(VT_UI8 | VT_BYREF, (IUnknown*)&data);
        AssertToObjectEqual(data, variant);
    }

    public static IEnumerable<object[]> ToObject_CY_TestData()
    {
        yield return new object[] { (nint)0, 0.0m };
        yield return new object[] { (nint)10, 0.001m };
        yield return new object[] { (nint)10000, 1m };
        yield return new object[] { (nint)123456, 12.3456m };
        if (nint.Size == 8)
        {
            yield return new object[] { (nint)(-10), -0.001m };
            yield return new object[] { (nint)(-10000), -1m };
            yield return new object[] { (nint)(-123456), -12.3456m };
        }
    }

    [Theory]
    [MemberData(nameof(ToObject_CY_TestData))]
    public void VARIANT_ToObject_CY_ReturnsExpected(nint data, decimal expected)
    {
        using VARIANT variant = Create(VT_CY, (IUnknown*)data);
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
        using VARIANT variant = Create(VT_CY | VT_BYREF, (IUnknown*)&data);
        AssertToObjectEqual(expected, variant);
    }

    public static IEnumerable<object[]> ToObject_R4_TestData()
    {
        yield return new object[] { (nint)0, 0.0f };
        yield return new object[] { (nint)1067030938, 1.2f };
        if (nint.Size == 8)
        {
            yield return new object[] { unchecked((nint)3214514586), -1.2f };
            yield return new object[] { unchecked((nint)4290772992), float.NaN };
        }
    }

    [Theory]
    [MemberData(nameof(ToObject_R4_TestData))]
    public void VARIANT_ToObject_R4_ReturnsExpected(nint data, float expected)
    {
        using VARIANT variant = Create(VT_R4, (IUnknown*)data);
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
        using VARIANT variant = new()
        {
            vt = VT_R4 | VT_BYREF,
            data = new()
            {
                pfltVal = &data
            }
        };

        AssertToObjectEqual(data, variant);
    }

    public static IEnumerable<object[]> ToObject_R8_TestData()
    {
        yield return new object[] { (nint)0, 0.0 };
        if (nint.Size == 8)
        {
            yield return new object[] { unchecked((nint)4608083138725491507), 1.2 };
            yield return new object[] { unchecked((nint)(-4615288898129284301)), -1.2 };
            yield return new object[] { unchecked((nint)(-2251799813685248)), double.NaN };
        }
    }

    [Theory]
    [MemberData(nameof(ToObject_R8_TestData))]
    public void VARIANT_ToObject_R8_ReturnsExpected(nint data, double expected)
    {
        using VARIANT variant = Create(VT_R8, (IUnknown*)data);
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
        using VARIANT variant = Create(VT_R8 | VT_BYREF, (IUnknown*)&data);
        AssertToObjectEqual(data, variant);
    }

    public static IEnumerable<object[]> NULL_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { (nint)1 };
    }

    [StaTheory]
    [MemberData(nameof(NULL_TestData))]
    public void VARIANT_ToObject_NULL_Success(nint data)
    {
        using VARIANT variant = Create(VT_BYREF | VT_NULL, (IUnknown*)data);
        AssertToObjectEqual(Convert.DBNull, variant);
    }

    [StaTheory]
    [MemberData(nameof(NULL_TestData))]
    public void VARIANT_ToObject_NULLBYREFData_Success(nint data)
    {
        using VARIANT variant = Create(VT_BYREF | VT_NULL, (IUnknown*)&data);
        AssertToObjectEqual(Convert.DBNull, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_NULLBYREFNoData_Success()
    {
        using VARIANT variant = Create(VT_BYREF | VT_NULL);
        AssertToObjectEqual(Convert.DBNull, variant);
    }

    public static IEnumerable<object[]> EMPTY_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { (nint)1 };
    }

    [StaTheory]
    [MemberData(nameof(EMPTY_TestData))]
    public void VARIANT_ToObject_EMPTY_Success(nint data)
    {
        using VARIANT variant = Create(VT_EMPTY, (IUnknown*)data);
        AssertToObjectEqual(null, variant);
    }

    [StaTheory]
    [MemberData(nameof(EMPTY_TestData))]
    public void VARIANT_ToObject_EMPTYBYREFData_Success(nint data)
    {
        using VARIANT variant = Create(VT_BYREF | VT_EMPTY, (IUnknown*)&data);
        AssertToObject(variant, value =>
        {
            if (nint.Size == 8)
            {
                Assert.Equal((ulong)(nint)variant.Anonymous.Anonymous.Anonymous.ppunkVal, value);
            }
            else
            {
                Assert.Equal((uint)(nint)variant.Anonymous.Anonymous.Anonymous.ppunkVal, value);
            }
        });
    }

    [StaFact]
    public void VARIANT_ToObject_EMPTYBYREFNoData_Success()
    {
        using VARIANT variant = Create(VT_BYREF | VT_EMPTY);
        AssertToObject(variant, value =>
        {
            if (nint.Size == 8)
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
        yield return new object[] { (nint)int.MinValue, int.MinValue };
        yield return new object[] { (nint)short.MinValue, (int)short.MinValue };
        yield return new object[] { (nint)sbyte.MinValue, (int)sbyte.MinValue };
        yield return new object[] { (nint)(-10), -10 };
        yield return new object[] { (nint)0, 0 };
        yield return new object[] { (nint)10, 10 };
        yield return new object[] { (nint)sbyte.MaxValue, (int)sbyte.MaxValue };
        yield return new object[] { (nint)short.MaxValue, (int)short.MaxValue };
        yield return new object[] { (nint)int.MaxValue, int.MaxValue };
        if (nint.Size == 8)
        {
            yield return new object[] { unchecked((nint)long.MinValue), 0 };
            yield return new object[] { unchecked((nint)long.MaxValue), -1 };
        }
    }

    [StaTheory]
    [MemberData(nameof(HRESULT_TestData))]
    public void VARIANT_ToObject_HRESULT_Success(nint data, int expected)
    {
        using VARIANT variant = Create(VT_HRESULT, (IUnknown*)data);
        AssertToObjectEqualExtension<ArgumentException>(expected, variant);
    }

    [StaTheory]
    [MemberData(nameof(HRESULT_TestData))]
    public void VARIANT_ToObject_HRESULTBYREF_Success(nint data, int expected)
    {
        using VARIANT variant = Create(VT_HRESULT | VT_BYREF, (IUnknown*)&data);
        AssertToObjectEqualExtension<ArgumentException>(expected, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_FILETIME_Success()
    {
        using VARIANT variant = default;
        DateTime dt = new(2020, 05, 13, 13, 3, 12);
        var ft = new FILETIME(dt);
        HRESULT hr = InitPropVariantFromFileTime(&ft, &variant);
        Assert.Equal(HRESULT.S_OK, hr);
        Assert.Equal(VT_FILETIME, variant.vt);

        AssertToObjectEqualExtension<ArgumentException>(new DateTime(2020, 05, 13, 13, 3, 12), variant);
    }

    [StaTheory]
    [InlineData(-10)]
    public void VARIANT_ToObject_InvalidFILETIME_ThrowsArgumentOutOfRangeException(int value)
    {
        using VARIANT variant = new()
        {
            Anonymous = new()
            {
                Anonymous = new()
                {
                    vt = VT_FILETIME,
                    Anonymous = new() { cyVal = new() { int64 = value } }
                }
            }
        };

        Assert.Throws<ArgumentOutOfRangeException>("fileTime", variant.ToObject);
    }

    [StaFact]
    public void VARIANT_ToObject_DateFromFILETIME_Success()
    {
        using VARIANT variant = default;
        DateTime dt = new DateTime(2020, 05, 13, 13, 3, 12, DateTimeKind.Utc).ToLocalTime();
        var ft = new FILETIME(dt);
        HRESULT hr = InitVariantFromFileTime(&ft, &variant);
        Assert.Equal(HRESULT.S_OK, hr);
        Assert.Equal(VT_DATE, variant.vt);

        AssertToObjectEqual(new DateTime(2020, 05, 13, 13, 3, 12, DateTimeKind.Utc).ToUniversalTime(), variant);
    }

    [StaFact]
    public void VARIANT_ToObject_Date_Success()
    {
        DateTime dt = new(2020, 05, 13, 13, 3, 12);
        double date = dt.ToOADate();
        using VARIANT variant = new()
        {
            Anonymous = new()
            {
                Anonymous = new()
                {
                    vt = VT_DATE,
                    Anonymous = new() { date = date }
                }
            }
        };

        AssertToObjectEqual(new DateTime(2020, 05, 13, 13, 3, 12), variant);
    }

    [StaFact]
    public void VARIANT_ToObject_DateEmpty_Success()
    {
        using VARIANT variant = Create(VT_DATE);
        AssertToObjectEqual(new DateTime(1899, 12, 30), variant);
    }

    [StaFact]
    public void VARIANT_ToObject_DateBYREF_Success()
    {
        DateTime dt = new(2020, 05, 13, 13, 3, 12);
        double date = dt.ToOADate();
        using VARIANT variant = new()
        {
            Anonymous = new()
            {
                Anonymous = new()
                {
                    vt = VT_DATE | VT_BYREF,
                    Anonymous = new() { pdate = &date }
                }
            }
        };

        AssertToObjectEqual(new DateTime(2020, 05, 13, 13, 3, 12), variant);
    }

    [StaFact]
    public void VARIANT_ToObject_DateBYREFEmpty_Success()
    {
        double date = 0;
        using VARIANT variant = Create(VT_DATE | VT_BYREF);
        variant.data.pdate = &date;

        AssertToObjectEqual(new DateTime(1899, 12, 30), variant);
    }

    [StaTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("text")]
    public void VARIANT_ToObject_BSTR_ReturnsExpected(string text)
    {
        using VARIANT variant = new()
        {
            vt = VT_BSTR,
            data = new() { bstrVal = new BSTR(text) }
        };
        AssertToObjectEqual(text, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_BSTRNoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_BSTR);
        AssertToObjectEqual(null, variant);
    }

    [StaTheory]
    [InlineData("")]
    [InlineData("text")]
    public void VARIANT_ToObject_BSTRBYREF_ReturnsExpected(string text)
    {
        // ByRef VARIANTs are not freed
        using BSTR bstr = new(text);
        using VARIANT variant = new()
        {
            vt = VT_BSTR | VT_BYREF,
            data = new() { pbstrVal = &bstr }
        };
        AssertToObjectEqual(text, variant);
    }

    [StaTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("text")]
    public void VARIANT_ToObject_LPWSTR_ReturnsExpected(string text)
    {
        using VARIANT variant = Create(VT_LPWSTR, (IUnknown*)(void*)Marshal.StringToCoTaskMemUni(text));
        AssertToObjectEqualExtension<ArgumentException>(text, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_LPWSTRNoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_LPWSTR);
        AssertToObjectEqualExtension<ArgumentException>(null, variant);
    }

    [StaTheory]
    [InlineData("")]
    [InlineData("text")]
    public void VARIANT_ToObject_LPWSTRBYREF_ReturnsExpected(string text)
    {
        fixed (char* t = text)
        {
            // Not freed when by ref, can just pin.
            using VARIANT variant = Create(VT_LPWSTR | VT_BYREF, &t);
            AssertToObjectEqualExtension<ArgumentException>(text, variant);
        }
    }

    [StaTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("text")]
    public void VARIANT_ToObject_LPSTR_ReturnsExpected(string text)
    {
        using VARIANT variant = Create(VT_LPSTR, (void*)Marshal.StringToCoTaskMemAnsi(text));
        AssertToObjectEqualExtension<ArgumentException>(text, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_LPSTRNoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_LPSTR);
        AssertToObjectEqualExtension<ArgumentException>(null, variant);
    }

    [StaTheory]
    [InlineData("")]
    [InlineData("text")]
    public void VARIANT_ToObject_LPSTRBYREF_ReturnsExpected(string text)
    {
        nint ptr = Marshal.StringToCoTaskMemAnsi(text);
        try
        {
            using VARIANT variant = Create(VT_LPSTR | VT_BYREF, &ptr);
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
        object o = new();
        nint pUnk = Marshal.GetIUnknownForObject(o);
        using VARIANT variant = Create(VT_DISPATCH, (void*)pUnk);
        AssertToObjectEqual(o, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_DispatchNoData_ReturnsNull()
    {
        using VARIANT variant = Create(VT_DISPATCH);
        AssertToObjectEqual(null, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_DispatchBYREF_ReturnsExpected()
    {
        object o = new();
        using ComScope<IUnknown> unknown = new((IUnknown*)(void*)Marshal.GetIUnknownForObject(o));
        using VARIANT variant = Create(VT_DISPATCH | VT_BYREF, &unknown);
        AssertToObjectEqual(o, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_DispatchBYREFNullData_ReturnsNull()
    {
        IUnknown* unknown = null;
        using VARIANT variant = Create(VT_DISPATCH | VT_BYREF, &unknown);
        AssertToObjectEqual(null, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_UNKNOWN_ReturnsExpected()
    {
        object o = new();
        using VARIANT variant = Create(VT_UNKNOWN, (void*)Marshal.GetIUnknownForObject(o));
        AssertToObjectEqual(o, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_UNKNOWNNoData_ReturnsNull()
    {
        using VARIANT variant = Create(VT_UNKNOWN);
        AssertToObjectEqual(null, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_UNKNOWNBYREF_ReturnsExpected()
    {
        object o = new();
        using ComScope<IUnknown> unknown = new((IUnknown*)(void*)Marshal.GetIUnknownForObject(o));
        using VARIANT variant = Create(VT_UNKNOWN | VT_BYREF, &unknown);
        AssertToObjectEqual(o, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_UNKNOWNBYREFNullData_ReturnsNull()
    {
        IUnknown* unknown = null;
        using VARIANT variant = Create(VT_UNKNOWN | VT_BYREF, &unknown);
        AssertToObjectEqual(null, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_I4VARIANTBYREF_ReturnsExpected()
    {
        using VARIANT target = new()
        {
            Anonymous = new()
            {
                Anonymous = new()
                {
                    vt = VT_I4,
                    Anonymous = new() { llVal = 10 }
                }
            }
        };

        using VARIANT variant = Create(VT_VARIANT | VT_BYREF, &target);
        AssertToObjectEqual(10, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_BSTRVARIANTBYREF_ReturnsExpected()
    {
        using VARIANT target = new()
        {
            vt = VT_BSTR,
            data = new() { bstrVal = new BSTR("test") }
        };

        using VARIANT variant = Create(VT_VARIANT | VT_BYREF, &target);
        AssertToObjectEqual("test", variant);
    }

    [StaFact]
    public void VARIANT_ToObject_EMPTYVARIANTBYREF_ThrowsInvalidOleVariantTypeException()
    {
        using VARIANT target = Create(VT_EMPTY);
        using VARIANT variant = Create(VT_VARIANT | VT_BYREF, &target);
        AssertToObjectEqual(null, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_BYREFVARIANTBYREF_ThrowsInvalidOleVariantTypeException()
    {
        int lval = 10;
        using VARIANT target = new()
        {
            Anonymous = new()
            {
                Anonymous = new()
                {
                    vt = VT_BYREF | VT_I4,
                    Anonymous = new() { plVal = &lval }
                }
            }
        };

        using VARIANT variant = Create(VT_VARIANT | VT_BYREF, &target);
        AssertToObjectThrows<InvalidOleVariantTypeException>(variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VARIANT_ThrowsArgumentException()
    {
        using VARIANT variant = Create(VT_VARIANT);
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
        yield return new object[] { default(DECIMAL), 0.0m };
    }

    [StaTheory]
    [MemberData(nameof(Decimal_TestData))]
    public void VARIANT_ToObject_Decimal_ReturnsExpected(object d, decimal expected)
    {
        VARIANT variant = default;
        *(DECIMAL*)(&variant) = (DECIMAL)d;
        variant.Anonymous.Anonymous.vt = VT_DECIMAL;
        AssertToObjectEqual(expected, variant);
    }

    [StaTheory]
    [MemberData(nameof(Decimal_TestData))]
    public void VARIANT_ToObject_DecimalBYREF_ReturnsExpected(object d, decimal expected)
    {
        DECIMAL asD = (DECIMAL)d;
        using VARIANT variant = Create(VT_DECIMAL | VT_BYREF, &asD);
        AssertToObjectEqual(expected, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_CLSID_ReturnsExpected()
    {
        var guid = Guid.NewGuid();
        using VARIANT variant = default;
        HRESULT hr = InitPropVariantFromCLSID(&guid, &variant);
        Assert.Equal(HRESULT.S_OK, hr);
        Assert.Equal(VT_CLSID, variant.vt);

        AssertToObjectEqualExtension<ArgumentException>(guid, variant);
    }

    public static IEnumerable<object[]> VOID_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { (nint)1 };
    }

    [StaTheory]
    [MemberData(nameof(VOID_TestData))]
    public void VARIANT_ToObject_VOID_ReturnsExpected(nint data)
    {
        using VARIANT variant = Create(VT_VOID, (void*)data);
        nint pv = (nint)(&variant);
        Assert.Null(Marshal.GetObjectForNativeVariant(pv));
    }

    [StaTheory]
    [InlineData((ushort)VT_USERDEFINED)]
    [InlineData((ushort)(VT_USERDEFINED | VT_BYREF))]
    public void VARIANT_ToObject_USERDATA_ThrowsArgumentException(ushort vt)
    {
        using VARIANT variant = Create((VARENUM)vt);
        AssertToObjectThrows<ArgumentException>(variant);
    }

    [StaTheory]
    [InlineData((ushort)(VT_VOID | VT_BYREF))]
    [InlineData((ushort)VT_PTR)]
    [InlineData((ushort)(VT_PTR | VT_BYREF))]
    [InlineData((ushort)VT_SAFEARRAY)]
    [InlineData((ushort)(VT_SAFEARRAY | VT_BYREF))]
    [InlineData((ushort)VT_CARRAY)]
    [InlineData((ushort)(VT_CARRAY | VT_BYREF))]
    [InlineData((ushort)VT_RECORD)]
    [InlineData((ushort)(VT_RECORD | VT_BYREF))]
    [InlineData((ushort)VT_BLOB)]
    [InlineData((ushort)(VT_BLOB | VT_BYREF))]
    [InlineData((ushort)VT_STREAM)]
    [InlineData((ushort)(VT_STREAM | VT_BYREF))]
    [InlineData((ushort)VT_STORAGE)]
    [InlineData((ushort)(VT_STORAGE | VT_BYREF))]
    [InlineData((ushort)VT_STREAMED_OBJECT)]
    [InlineData((ushort)(VT_STREAMED_OBJECT | VT_BYREF))]
    [InlineData((ushort)VT_STORED_OBJECT)]
    [InlineData((ushort)(VT_STORED_OBJECT | VT_BYREF))]
    [InlineData((ushort)VT_BLOB_OBJECT)]
    [InlineData((ushort)(VT_BLOB_OBJECT | VT_BYREF))]
    [InlineData((ushort)VT_CF)]
    [InlineData((ushort)(VT_CF | VT_BYREF))]
    [InlineData((ushort)(VT_BSTR_BLOB | VT_BYREF))]
    [InlineData((ushort)VT_ILLEGAL)]
    [InlineData((ushort)VT_INT_PTR)]
    [InlineData((ushort)VT_UINT_PTR)]
    [InlineData(127)]
    [InlineData(0x000F)]
    [InlineData(0x0020)]
    [InlineData(0x0021)]
    [InlineData(0x0022)]
    [InlineData(0x0023)]
    [InlineData(0x0024)]
    public void VARIANT_ToObject_CantConvert_ThrowsArgumentException(ushort vt)
    {
        using VARIANT variant = Create((VARENUM)vt);
        AssertToObjectThrows<ArgumentException>(variant);
    }

    [StaTheory]
    [InlineData(128)]
    [InlineData(129)]
    [InlineData((ushort)VT_BSTR_BLOB)]
    public void VARIANT_ToObject_Illegal_ThrowsInvalidOleVariantTypeException(ushort vt)
    {
        using VARIANT variant = Create((VARENUM)vt);
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
        VARIANT variant = default;
        try
        {
            fixed (sbyte* pResult = result)
            {
                HRESULT hr = InitPropVariantFromBuffer(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VT_VECTOR | VT_UI1, variant.vt);
            }

            // I1 and UI1 have the same size.
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_I1;
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
        using VARIANT variant = Create(VT_VECTOR | VT_I1);
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
        using VARIANT variant = default;
        fixed (byte* pResult = result)
        {
            HRESULT hr = InitPropVariantFromBuffer(pResult, (uint)result.Length, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_VECTOR | VT_UI1, variant.vt);
        }

        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORUI1NoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_VECTOR | VT_UI1);
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
        using VARIANT variant = default;
        fixed (short* pResult = result)
        {
            HRESULT hr = InitPropVariantFromInt16Vector(pResult, (uint)result.Length, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_VECTOR | VT_I2, variant.vt);
        }

        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORI2NoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_VECTOR | VT_I2);
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
        using VARIANT variant = default;
        fixed (ushort* pResult = result)
        {
            HRESULT hr = InitPropVariantFromUInt16Vector(pResult, (uint)result.Length, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_VECTOR | VT_UI2, variant.vt);
        }

        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORUI2NoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_VECTOR | VT_UI2);
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
        using VARIANT variant = default;
        BOOL[] boolResult = (BOOL[])result;
        fixed (BOOL* pResult = boolResult)
        {
            HRESULT hr = InitPropVariantFromBooleanVector(pResult, (uint)boolResult.Length, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_VECTOR | VT_BOOL, variant.vt);
        }

        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(expected, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORBOOLNoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_VECTOR | VT_BOOL);
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
        using VARIANT variant = default;
        fixed (int* pResult = result)
        {
            HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_VECTOR | VT_I4, variant.vt);
        }

        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORI4NoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_VECTOR | VT_I4);
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
        using VARIANT variant = default;
        fixed (uint* pResult = result)
        {
            HRESULT hr = InitPropVariantFromUInt32Vector(pResult, (uint)result.Length, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_VECTOR | VT_UI4, variant.vt);
        }

        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORUI4NoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_VECTOR | VT_UI4);
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
        VARIANT variant = default;
        try
        {
            fixed (int* pResult = result)
            {
                HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VT_VECTOR | VT_I4, variant.vt);
            }

            // I4 and INT have the same size.
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_INT;
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
        using VARIANT variant = Create(VT_VECTOR | VT_INT);
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
        VARIANT variant = default;
        try
        {
            fixed (uint* pResult = result)
            {
                HRESULT hr = InitPropVariantFromUInt32Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VT_VECTOR | VT_UI4, variant.vt);
            }

            // UI4 and UINT have the same size.
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_UINT;
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
        using VARIANT variant = Create(VT_VECTOR | VT_UINT);
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
        using VARIANT variant = default;
        fixed (long* pResult = result)
        {
            HRESULT hr = InitPropVariantFromInt64Vector(pResult, (uint)result.Length, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_VECTOR | VT_I8, variant.vt);
        }

        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORI8NoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_VECTOR | VT_I8);
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
        using VARIANT variant = default;
        fixed (ulong* pResult = result)
        {
            HRESULT hr = InitPropVariantFromUInt64Vector(pResult, (uint)result.Length, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_VECTOR | VT_UI8, variant.vt);
        }

        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORUI8NoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_VECTOR | VT_UI8);
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
        VARIANT variant = default;
        try
        {
            fixed (float* pResult = result)
            {
                HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VT_VECTOR | VT_I4, variant.vt);
            }

            // I4 and R4 are the same size.
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_R4;
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
        using VARIANT variant = Create(VT_VECTOR | VT_R4);
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
        using VARIANT variant = default;
        fixed (double* pResult = result)
        {
            HRESULT hr = InitPropVariantFromDoubleVector(pResult, (uint)result.Length, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_VECTOR | VT_R8, variant.vt);
        }

        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(result, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORR8NoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_VECTOR | VT_R8);
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
        VARIANT variant = default;
        try
        {
            fixed (uint* pResult = result)
            {
                HRESULT hr = InitPropVariantFromUInt32Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VT_VECTOR | VT_UI4, variant.vt);
            }

            // UI4 and ERROR are the same size.
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_ERROR;
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
        using VARIANT variant = Create(VT_VECTOR | VT_ERROR);
        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
    }

    public static IEnumerable<object[]> VectorCY_TestData()
    {
        yield return new object[] { Array.Empty<long>(), Array.Empty<decimal>() };
        yield return new object[] { new long[] { 11000, 22000, 30000 }, new decimal[] { 1.1m, 2.2m, 3 } };
    }

    [StaTheory]
    [MemberData(nameof(VectorCY_TestData))]
    public void VARIANT_ToObject_VECTORCY_ReturnsExpected(long[] result, decimal[] expected)
    {
        VARIANT variant = default;
        try
        {
            fixed (long* pResult = result)
            {
                HRESULT hr = InitPropVariantFromInt64Vector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VT_VECTOR | VT_I8, variant.vt);
            }

            // I8 and CY have the same size.
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_CY;
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
        using VARIANT variant = Create(VT_VECTOR | VT_CY);
        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<decimal>(), variant);
    }

    public static IEnumerable<object[]> VectorDATE_TestData()
    {
        yield return new object[] { Array.Empty<double>(), Array.Empty<DateTime>() };

        DateTime d1 = new(2020, 05, 13, 13, 3, 12);
        DateTime d2 = new(2020, 05, 13, 13, 3, 11);
        DateTime d3 = new(2020, 3, 13, 13, 3, 12);
        yield return new object[] { new double[] { d1.ToOADate(), d2.ToOADate(), d3.ToOADate() }, new DateTime[] { d1, d2, d3 } };
    }

    [StaTheory]
    [MemberData(nameof(VectorDATE_TestData))]
    public void VARIANT_ToObject_VECTORDATE_ReturnsExpected(double[] result, DateTime[] expected)
    {
        VARIANT variant = default;
        try
        {
            fixed (double* pResult = result)
            {
                HRESULT hr = InitPropVariantFromDoubleVector(pResult, (uint)result.Length, &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VT_VECTOR | VT_R8, variant.vt);
            }

            // R8 and DATE have the same size.
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_DATE;
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
        using VARIANT variant = Create(VT_VECTOR | VT_DATE);
        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<DateTime>(), variant);
    }

    public static IEnumerable<object[]> VectorFILETIME_TestData()
    {
        yield return new object[] { Array.Empty<FILETIME>(), Array.Empty<DateTime>() };

        DateTime d1 = new(2020, 05, 13, 13, 3, 12);
        DateTime d2 = new(2020, 05, 13, 13, 3, 11);
        DateTime d3 = new(2020, 3, 13, 13, 3, 12);
        yield return new object[] { new FILETIME[] { new(d1), new(d2), new(d3) }, new DateTime[] { d1, d2, d3 } };
    }

    [StaTheory]
    [MemberData(nameof(VectorFILETIME_TestData))]
    public void VARIANT_ToObject_VECTORFILETIME_ReturnsExpected(object result, DateTime[] expected)
    {
        using VARIANT variant = default;
        FILETIME[] fileTimeResult = (FILETIME[])result;
        fixed (FILETIME* pResult = fileTimeResult)
        {
            HRESULT hr = InitPropVariantFromFileTimeVector(pResult, (uint)fileTimeResult.Length, &variant);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_VECTOR | VT_FILETIME, variant.vt);
        }

        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(expected, variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORFILETIMENoData_ReturnsExpected()
    {
        using VARIANT variant = Create(VT_VECTOR | VT_FILETIME);
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
        VARIANT variant = default;
        try
        {
            fixed (Guid* pResult = result)
            {
                HRESULT hr = InitPropVariantFromBuffer(pResult, (uint)(result.Length * sizeof(Guid)), &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VT_VECTOR | VT_UI1, variant.vt);
            }

            variant.Anonymous.Anonymous.Anonymous.ca.cElems = (uint)(variant.Anonymous.Anonymous.Anonymous.ca.cElems / sizeof(Guid));
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_CLSID;
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
        using VARIANT variant = Create(VT_VECTOR | VT_CLSID);
        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORBSTR_ReturnsExpected()
    {
        VARIANT variant = default;
        BSTR ptr1 = new("text");
        BSTR ptr2 = new("");

        try
        {
            nint[] result = [0, ptr1, ptr2];
            fixed (nint* pResult = result)
            {
                if (nint.Size == 4)
                {
                    HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VT_VECTOR | VT_I4, variant.vt);
                }
                else
                {
                    HRESULT hr = InitPropVariantFromInt64Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VT_VECTOR | VT_I8, variant.vt);
                }
            }

            // I4/I8 and BSTR have same size.
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_BSTR;
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
        using VARIANT variant = Create(VT_VECTOR | VT_BSTR);
        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORLPWSTR_ReturnsExpected()
    {
        VARIANT variant = default;
        nint ptr1 = Marshal.StringToCoTaskMemUni("text");
        nint ptr2 = Marshal.StringToCoTaskMemUni("");
        try
        {
            nint[] result = [0, ptr1, ptr2];
            fixed (nint* pResult = result)
            {
                if (nint.Size == 4)
                {
                    HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VT_VECTOR | VT_I4, variant.vt);
                }
                else
                {
                    HRESULT hr = InitPropVariantFromInt64Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VT_VECTOR | VT_I8, variant.vt);
                }
            }

            // I4/I8 and LPWSTR have same size.
            variant.vt = VT_VECTOR | VT_LPWSTR;
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
        using VARIANT variant = Create(VT_VECTOR | VT_LPWSTR);
        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORLPSTR_ReturnsExpected()
    {
        VARIANT variant = default;
        nint ptr1 = Marshal.StringToCoTaskMemAnsi("text");
        nint ptr2 = Marshal.StringToCoTaskMemAnsi("");
        try
        {
            nint[] result = [0, ptr1, ptr2];
            fixed (nint* pResult = result)
            {
                if (nint.Size == 4)
                {
                    HRESULT hr = InitPropVariantFromInt32Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VT_VECTOR | VT_I4, variant.vt);
                }
                else
                {
                    HRESULT hr = InitPropVariantFromInt64Vector(pResult, (uint)result.Length, &variant);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VT_VECTOR | VT_I8, variant.vt);
                }
            }

            // I4/I8 and LPSTR have same size.
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_LPSTR;
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
        using VARIANT variant = Create(VT_VECTOR | VT_LPSTR);
        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
    }

    [StaFact]
    public void VARIANT_ToObject_VECTORVARIANT_ReturnsExpected()
    {
        VARIANT variant = default;
        try
        {
            VARIANT variant1 = Create(VT_I4);
            variant1.data.llVal = 1;
            VARIANT variant2 = Create(VT_UI4);
            variant2.data.ullVal = 2;
            var result = new VARIANT[] { variant1, variant2 };
            fixed (VARIANT* pResult = result)
            {
                HRESULT hr = InitPropVariantFromBuffer(pResult, (uint)(result.Length * sizeof(VARIANT)), &variant);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(VT_VECTOR | VT_UI1, variant.vt);
            }

            variant.data.ca.cElems = (uint)(variant.data.ca.cElems / sizeof(VARIANT));
            variant.Anonymous.Anonymous.vt = VT_VECTOR | VT_VARIANT;
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
        using VARIANT variant = Create(VT_VECTOR | VT_VARIANT);
        AssertToObjectEqualExtension<InvalidOleVariantTypeException>(Array.Empty<int>(), variant);
    }

    [StaTheory]
    [InlineData((ushort)VT_EMPTY)]
    [InlineData((ushort)VT_DECIMAL)]
    [InlineData((ushort)VT_UNKNOWN)]
    [InlineData((ushort)VT_DISPATCH)]
    [InlineData((ushort)VT_NULL)]
    [InlineData((ushort)VT_CF)]
    [InlineData((ushort)VT_VOID)]
    [InlineData((ushort)VT_PTR)]
    [InlineData((ushort)VT_SAFEARRAY)]
    [InlineData((ushort)VT_CARRAY)]
    [InlineData((ushort)VT_RECORD)]
    [InlineData((ushort)VT_BLOB)]
    [InlineData((ushort)VT_STREAM)]
    [InlineData((ushort)VT_STORAGE)]
    [InlineData((ushort)VT_STREAMED_OBJECT)]
    [InlineData((ushort)VT_STORED_OBJECT)]
    [InlineData((ushort)VT_BLOB_OBJECT)]
    [InlineData(127)]
    [InlineData(0x000F)]
    [InlineData(0x0020)]
    [InlineData(0x0021)]
    [InlineData(0x0022)]
    [InlineData(0x0023)]
    [InlineData(0x0024)]
    public void VARIANT_ToObject_VECTORInvalidType_ThrowsArgumentException(ushort vt)
    {
        using VARIANT variant = new()
        {
            vt = VT_VECTOR | (VARENUM)vt
        };
        Assert.Throws<ArgumentException>(variant.ToObject);
    }

    [StaTheory]
    [InlineData(128)]
    [InlineData(129)]
    [InlineData((ushort)VT_BSTR_BLOB)]
    public void VARIANT_ToObject_VECTORInvalidTypeNoData_ThrowsInvalidOleVariantTypeException(ushort vt)
    {
        using VARIANT variant = new()
        {
            vt = VT_VECTOR | (VARENUM)vt
        };

        AssertToObjectThrows<InvalidOleVariantTypeException>(variant);
    }

    [StaTheory]
    [MemberData(nameof(VectorUI1_TestData))]
    public void VARIANT_ToObject_ARRAYUI1SingleDimension_ReturnsExpected(byte[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UI1, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI1
        };
        variant.data.parray = psa;

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
        SAFEARRAY* psa = CreateSafeArray(VT_UI1, result, 1);
        using VARIANT variant = Create(VT_ARRAY | VT_UI1);
        variant.data.parray = psa;

        AssertToObject(variant, value =>
        {
            Array array = (Array)value;
            Assert.IsType(typeof(byte).MakeArrayType(1), array);
            Assert.Equal(1, array.Rank);
            Assert.Equal(1, array.GetLowerBound(0));
            Assert.Equal(result.Length, array.GetLength(0));
        });
    }

    public static IEnumerable<object[]> MultiDimensionUI1_TestData()
    {
        yield return new object[] { new byte[0, 0] };
        yield return new object[]
        {
            new byte[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionUI1_TestData))]
    public void VARIANT_ToObject_ARRAYUI1MultiDimension_ReturnsExpected(byte[,] result)
    {
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI1
        };
        variant.data.parray = CreateSafeArray(VT_UI1, result);

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
        using VARIANT variant = new() { vt = VT_ARRAY | VT_UI1 };
        variant.data.parray = CreateSafeArray(VT_UI1, result, 1, 2);
        AssertToObject(variant, value =>
        {
            Array array = (Array)value;
            Assert.IsType(typeof(byte).MakeArrayType(2), array);
            Assert.Equal(2, array.Rank);
            Assert.Equal(1, array.GetLowerBound(0));
            Assert.Equal(2, array.GetLowerBound(1));
            Assert.Equal(result.GetLength(0), array.GetLength(0));
            Assert.Equal(result.GetLength(1), array.GetLength(1));
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorI1_TestData))]
    public void VARIANT_ToObject_ARRAYI1SingleDimension_ReturnsExpected(sbyte[] result)
    {
        using VARIANT variant = new() { vt = VT_ARRAY | VT_I1 };
        variant.data.parray = CreateSafeArray(VT_I1, result);
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
        using VARIANT variant = new() { vt = VT_ARRAY | VT_I1 };
        variant.data.parray = CreateSafeArray(VT_I1, result, 1);
        AssertToObject(variant, value =>
        {
            Array array = (Array)value;
            Assert.IsType(typeof(sbyte).MakeArrayType(1), array);
            Assert.Equal(1, array.Rank);
            Assert.Equal(1, array.GetLowerBound(0));
            Assert.Equal(result.Length, array.GetLength(0));
        });
    }

    public static IEnumerable<object[]> MultiDimensionI1_TestData()
    {
        yield return new object[] { new sbyte[0, 0] };
        yield return new object[]
        {
            new sbyte[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionI1_TestData))]
    public void VARIANT_ToObject_ARRAYI1MultiDimension_ReturnsExpected(sbyte[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_I1, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I1,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_I1, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I1,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorI2_TestData))]
    public void VARIANT_ToObject_ARRAYI2SingleDimension_ReturnsExpected(short[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_I2, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I2,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_I2, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I2,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionI2_TestData()
    {
        yield return new object[] { new short[0, 0] };
        yield return new object[]
        {
            new short[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionI2_TestData))]
    public void VARIANT_ToObject_ARRAYI2MultiDimension_ReturnsExpected(short[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_I2, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I2,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_I2, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I2,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorUI2_TestData))]
    public void VARIANT_ToObject_ARRAYUI2SingleDimension_ReturnsExpected(ushort[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UI2, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI2,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UI2, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI2,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionUI2_TestData()
    {
        yield return new object[] { new ushort[0, 0] };
        yield return new object[]
        {
            new ushort[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionUI2_TestData))]
    public void VARIANT_ToObject_ARRAYUI2MultiDimension_ReturnsExpected(ushort[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UI2, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI2,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UI2, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI2,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorI4_TestData))]
    public void VARIANT_ToObject_ARRAYI4SingleDimension_ReturnsExpected(int[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_I4, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I4,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_I4, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I4,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionI4_TestData()
    {
        yield return new object[] { new int[0, 0] };
        yield return new object[]
        {
            new int[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionI4_TestData))]
    public void VARIANT_ToObject_ARRAYI4MultiDimension_ReturnsExpected(int[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_I4, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I4,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_I4, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I4,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorI4_TestData))]
    public void VARIANT_ToObject_INTArrayI4SingleDimension_ReturnsExpected(int[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_INT, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I4,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_INT, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I4,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionI4_TestData))]
    public void VARIANT_ToObject_INTArrayI4MultiDimension_ReturnsExpected(int[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_INT, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I4,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_INT, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I4,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorUI4_TestData))]
    public void VARIANT_ToObject_ARRAYUI4SingleDimension_ReturnsExpected(uint[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UI4, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI4,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UI4, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI4,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionUI4_TestData()
    {
        yield return new object[] { new uint[0, 0] };
        yield return new object[]
        {
            new uint[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionUI4_TestData))]
    public void VARIANT_ToObject_ARRAYUI4MultiDimension_ReturnsExpected(uint[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UI4, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI4,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UI4, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI4,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorUI4_TestData))]
    public void VARIANT_ToObject_UINTArrayUI4SingleDimension_ReturnsExpected(uint[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UINT, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI4,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UINT, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI4,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionUI4_TestData))]
    public void VARIANT_ToObject_UINTArrayUI4MultiDimension_ReturnsExpected(uint[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UINT, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI4,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UINT, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI4,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorINT_TestData))]
    public void VARIANT_ToObject_ARRAYINTSingleDimension_ReturnsExpected(int[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_INT, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_INT,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_INT, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_INT,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionINT_TestData()
    {
        yield return new object[] { new int[0, 0] };
        yield return new object[]
        {
            new int[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionINT_TestData))]
    public void VARIANT_ToObject_ARRAYINTMultiDimension_ReturnsExpected(int[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_INT, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_INT,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_INT, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_INT,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorINT_TestData))]
    public void VARIANT_ToObject_I4ArrayINTSingleDimension_ReturnsExpected(int[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_I4, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_INT,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_I4, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_INT,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionINT_TestData))]
    public void VARIANT_ToObject_I4ArrayINTMultiDimension_ReturnsExpected(int[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_I4, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_INT,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_I4, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_INT,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorUINT_TestData))]
    public void VARIANT_ToObject_ARRAYUINTSingleDimension_ReturnsExpected(uint[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UINT, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UINT,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UINT, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UINT,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionUINT_TestData()
    {
        yield return new object[] { new uint[0, 0] };
        yield return new object[]
        {
            new uint[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionUINT_TestData))]
    public void VARIANT_ToObject_ARRAYUINTMultiDimension_ReturnsExpected(uint[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UINT, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UINT,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UINT, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UINT,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorUINT_TestData))]
    public void VARIANT_ToObject_UI4ArrayUINTSingleDimension_ReturnsExpected(uint[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UI4, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UINT,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UI4, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UINT,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionUINT_TestData))]
    public void VARIANT_ToObject_UI4ArrayUINTMultiDimension_ReturnsExpected(uint[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UI4, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UINT,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UI4, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UINT,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorI8_TestData))]
    public void VARIANT_ToObject_ARRAYI8SingleDimension_ReturnsExpected(long[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_I8, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I8,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_I8, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I8,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionI8_TestData()
    {
        yield return new object[] { new long[0, 0] };
        yield return new object[]
        {
            new long[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionI8_TestData))]
    public void VARIANT_ToObject_ARRAYI8MultiDimension_ReturnsExpected(long[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_I8, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I8,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_I8, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I8,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorUI8_TestData))]
    public void VARIANT_ToObject_ARRAYUI8SingleDimension_ReturnsExpected(ulong[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UI8, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI8,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UI8, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI8,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionUI8_TestData()
    {
        yield return new object[] { new ulong[0, 0] };
        yield return new object[]
        {
            new ulong[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionUI8_TestData))]
    public void VARIANT_ToObject_ARRAYUI8MultiDimension_ReturnsExpected(ulong[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_UI8, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI8,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_UI8, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_UI8,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorR4_TestData))]
    public void VARIANT_ToObject_ARRAYR4SingleDimension_ReturnsExpected(float[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_R4, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_R4,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_R4, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_R4,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionR4_TestData()
    {
        yield return new object[] { new float[0, 0] };
        yield return new object[]
        {
            new float[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionR4_TestData))]
    public void VARIANT_ToObject_ARRAYR4MultiDimension_ReturnsExpected(float[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_R4, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_R4,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_R4, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_R4,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorR8_TestData))]
    public void VARIANT_ToObject_ARRAYR8SingleDimension_ReturnsExpected(double[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_R8, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_R8,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_R8, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_R8,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionR8_TestData()
    {
        yield return new object[] { new double[0, 0] };
        yield return new object[]
        {
            new double[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionR8_TestData))]
    public void VARIANT_ToObject_ARRAYR8MultiDimension_ReturnsExpected(double[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_R8, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_R8,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_R8, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_R8,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorERROR_TestData))]
    public void VARIANT_ToObject_ARRAYERRORSingleDimension_ReturnsExpected(uint[] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_ERROR, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_ERROR,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_ERROR, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_ERROR,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionERROR_TestData()
    {
        yield return new object[] { new uint[0, 0] };
        yield return new object[]
        {
            new uint[2, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionERROR_TestData))]
    public void VARIANT_ToObject_ARRAYERRORMultiDimension_ReturnsExpected(uint[,] result)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_ERROR, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_ERROR,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_ERROR, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_ERROR,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> ArrayBOOL_TestData()
    {
        yield return new object[] { Array.Empty<VARIANT_BOOL>(), Array.Empty<bool>() };
        yield return new object[] { new VARIANT_BOOL[] { VARIANT_BOOL.VARIANT_TRUE, VARIANT_BOOL.VARIANT_FALSE, VARIANT_BOOL.VARIANT_TRUE }, new bool[] { true, false, true } };
    }

    [StaTheory]
    [MemberData(nameof(ArrayBOOL_TestData))]
    public void VARIANT_ToObject_ARRAYBOOLSingleDimension_ReturnsExpected(object result, bool[] expected)
    {
        VARIANT_BOOL[] boolResult = (VARIANT_BOOL[])result;
        SAFEARRAY* psa = CreateSafeArray(VT_BOOL, boolResult);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_BOOL,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_BOOL, boolResult, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_BOOL,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionBOOL_TestData()
    {
        yield return new object[] { new VARIANT_BOOL[0, 0], new bool[0, 0] };
        yield return new object[]
        {
            new VARIANT_BOOL[2, 3]
            {
                { VARIANT_BOOL.VARIANT_TRUE, VARIANT_BOOL.VARIANT_FALSE, VARIANT_BOOL.VARIANT_TRUE },
                { VARIANT_BOOL.VARIANT_FALSE, VARIANT_BOOL.VARIANT_TRUE, VARIANT_BOOL.VARIANT_FALSE }
            },
            new bool[2, 3]
            {
                { true, false, true },
                { false, true, false }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionBOOL_TestData))]
    public void VARIANT_ToObject_ARRAYBOOLMultiDimension_ReturnsExpected(object result, bool[,] expected)
    {
        VARIANT_BOOL[,] boolResult = (VARIANT_BOOL[,])result;
        SAFEARRAY* psa = CreateSafeArray(VT_BOOL, boolResult);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_BOOL,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_BOOL, boolResult, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_BOOL,
            data = new()
            {
                parray = psa
            }
        };

        AssertToObject(variant, value =>
        {
            Array array = (Array)value;
            Assert.Equal(2, array.Rank);
            Assert.Equal(1, array.GetLowerBound(0));
            Assert.Equal(2, array.GetLowerBound(1));
            Assert.Equal(expected.GetLength(0), array.GetLength(0));
            Assert.Equal(expected.GetLength(1), array.GetLength(1));
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
        SAFEARRAY* psa = CreateSafeArray(VT_DECIMAL, decimalResult);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_DECIMAL,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_DECIMAL, decimalResult, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_DECIMAL,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionDECIMAL_TestData()
    {
        yield return new object[] { new DECIMAL[0, 0], new decimal[0, 0] };
        VarDecFromR8(1.1, out DECIMAL d1);
        VarDecFromR8(2.2, out DECIMAL d2);
        VarDecFromR8(3.3, out DECIMAL d3);
        VarDecFromR8(3.1, out DECIMAL d4);
        VarDecFromR8(2.2, out DECIMAL d5);
        VarDecFromR8(1.3, out DECIMAL d6);
        yield return new object[]
        {
            new DECIMAL[2, 3]
            {
                { d1, d2, d3 },
                { d4, d5, d6 }
            },
            new decimal[2, 3]
            {
                { 1.1m, 2.2m, 3.3m },
                { 3.1m, 2.2m, 1.3m }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionDECIMAL_TestData))]
    public void VARIANT_ToObject_ARRAYDECIMALMultiDimension_ReturnsExpected(object result, decimal[,] expected)
    {
        DECIMAL[,] decimalResult = (DECIMAL[,])result;
        SAFEARRAY* psa = CreateSafeArray(VT_DECIMAL, decimalResult);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_DECIMAL,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_DECIMAL, decimalResult, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_DECIMAL,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorCY_TestData))]
    public void VARIANT_ToObject_ARRAYCYSingleDimension_ReturnsExpected(long[] result, decimal[] expected)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_CY, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_CY,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_CY, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_CY,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionCY_TestData()
    {
        yield return new object[] { new long[0, 0], new decimal[0, 0] };
        yield return new object[]
        {
            new long[2, 3]
            {
                { 11000, 22000, 33000 },
                { 31000, 22000, 13000 }
            },
            new decimal[2, 3]
            {
                { 1.1m, 2.2m, 3.3m },
                { 3.1m, 2.2m, 1.3m }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionCY_TestData))]
    public void VARIANT_ToObject_ARRAYCYMultiDimension_ReturnsExpected(long[,] result, decimal[,] expected)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_CY, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_CY,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_CY, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_CY,
            data = new()
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
        });
    }

    [StaTheory]
    [MemberData(nameof(VectorDATE_TestData))]
    public void VARIANT_ToObject_ARRAYDATESingleDimension_ReturnsExpected(double[] result, DateTime[] expected)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_DATE, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_DATE,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_DATE, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_DATE,
            data = new()
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
        });
    }

    public static IEnumerable<object[]> MultiDimensionDATE_TestData()
    {
        yield return new object[] { new double[0, 0], new DateTime[0, 0] };

        DateTime d1 = new(2020, 05, 13, 13, 3, 12);
        DateTime d2 = new(2020, 05, 13, 13, 3, 11);
        DateTime d3 = new(2020, 3, 13, 13, 3, 12);
        DateTime d4 = new(1892, 1, 2, 3, 4, 5, 6);
        DateTime d5 = new(2010, 2, 3, 4, 5, 6);
        DateTime d6 = new(8000, 10, 11, 12, 13, 14);
        yield return new object[]
        {
            new double[2, 3]
            {
                { d1.ToOADate(), d2.ToOADate(), d3.ToOADate() },
                { d4.ToOADate(), d5.ToOADate(), d6.ToOADate() }
            },
            new DateTime[2, 3]
            {
                { d1, d2, d3 },
                { d4, d5, d6 }
            }
        };
    }

    [StaTheory]
    [MemberData(nameof(MultiDimensionDATE_TestData))]
    public void VARIANT_ToObject_ARRAYDATEMultiDimension_ReturnsExpected(double[,] result, DateTime[,] expected)
    {
        SAFEARRAY* psa = CreateSafeArray(VT_DATE, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_DATE,
            data = new()
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
        SAFEARRAY* psa = CreateSafeArray(VT_DATE, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_DATE,
            data = new()
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
        });
    }

    [StaFact]
    public void VARIANT_ToObject_ARRAYBSTRSingleDimension_ReturnsExpected()
    {
        using BSTR ptr1 = new("text");
        using BSTR ptr2 = new("");

        nint[] result = [0, ptr1, ptr2];
        SAFEARRAY* psa = CreateSafeArray(VT_BSTR, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_BSTR,
            data = new()
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
        });
    }

    [StaFact]
    public void VARIANT_ToObject_ARRAYBSTRSingleDimensionNonZeroLowerBound_ReturnsExpected()
    {
        using BSTR ptr1 = new("text");
        using BSTR ptr2 = new("");

        nint[] result = [0, ptr1, ptr2];
        SAFEARRAY* psa = CreateSafeArray(VT_BSTR, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_BSTR,
            data = new()
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
        });
    }

    [StaFact]
    public void VARIANT_ToObject_ARRAYBSTRMultiDimension_ReturnsExpected()
    {
        using BSTR ptr1 = new("text");
        using BSTR ptr2 = new("");
        using BSTR ptr3 = new("text3");
        using BSTR ptr4 = new("text4");
        using BSTR ptr5 = new("text5");

        nint[,] result = new nint[2, 3]
        {
            { 0, ptr1, ptr2 },
            { ptr3, ptr4, ptr5 }
        };

        SAFEARRAY* psa = CreateSafeArray(VT_BSTR, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_BSTR,
            data = new()
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
        });
    }

    [StaFact]
    public void VARIANT_ToObject_ARRAYBSTRMultiDimensionNonZeroLowerBound_ReturnsExpected()
    {
        using BSTR ptr1 = new("text");
        using BSTR ptr2 = new("");
        using BSTR ptr3 = new("text3");
        using BSTR ptr4 = new("text4");
        using BSTR ptr5 = new("text5");

        nint[,] result = new nint[2, 3]
        {
            { 0, ptr1, ptr2 },
            { ptr3, ptr4, ptr5 }
        };

        SAFEARRAY* psa = CreateSafeArray(VT_BSTR, result, 1, 2);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_BSTR,
            data = new()
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
        });
    }

    [StaFact]
    public void VARIANT_ToObject_ARRAYUNKNOWNSingleDimension_ReturnsExpected()
    {
        object o1 = new();
        object o2 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);

        try
        {
            nint[] result = [0, ptr1, ptr2];
            SAFEARRAY* psa = CreateSafeArray(VT_UNKNOWN, result);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_UNKNOWN,
                data = new()
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
        object o1 = new();
        object o2 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);
        try
        {
            nint[] result = [0, ptr1, ptr2];
            SAFEARRAY* psa = CreateSafeArray(VT_UNKNOWN, result, 1);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_UNKNOWN,
                data = new()
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
        object o1 = new();
        object o2 = new();
        object o3 = new();
        object o4 = new();
        object o5 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);
        nint ptr3 = Marshal.GetIUnknownForObject(o3);
        nint ptr4 = Marshal.GetIUnknownForObject(o4);
        nint ptr5 = Marshal.GetIUnknownForObject(o5);
        try
        {
            nint[,] result = new nint[2, 3]
            {
                { 0, ptr1, ptr2 },
                { ptr3, ptr4, ptr5 }
            };

            SAFEARRAY* psa = CreateSafeArray(VT_UNKNOWN, result);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_UNKNOWN,
                data = new()
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
                Assert.Equal(new object[,]
                {
                    { null, o1, o2 },
                    { o3, o4, o5 }
                }, array);
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
        object o1 = new();
        object o2 = new();
        object o3 = new();
        object o4 = new();
        object o5 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);
        nint ptr3 = Marshal.GetIUnknownForObject(o3);
        nint ptr4 = Marshal.GetIUnknownForObject(o4);
        nint ptr5 = Marshal.GetIUnknownForObject(o5);
        try
        {
            nint[,] result = new nint[2, 3]
            {
                { 0, ptr1, ptr2 },
                { ptr3, ptr4, ptr5 }
            };

            SAFEARRAY* psa = CreateSafeArray(VT_UNKNOWN, result, 1, 2);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_UNKNOWN,
                data = new()
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
        object o1 = new();
        object o2 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);
        try
        {
            nint[] result = [0, ptr1, ptr2];
            SAFEARRAY* psa = CreateSafeArray(VT_DISPATCH, result);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_UNKNOWN,
                data = new()
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
        object o1 = new();
        object o2 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);
        try
        {
            nint[] result = [0, ptr1, ptr2];
            SAFEARRAY* psa = CreateSafeArray(VT_DISPATCH, result, 1);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_UNKNOWN,
                data = new()
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
        object o1 = new();
        object o2 = new();
        object o3 = new();
        object o4 = new();
        object o5 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);
        nint ptr3 = Marshal.GetIUnknownForObject(o3);
        nint ptr4 = Marshal.GetIUnknownForObject(o4);
        nint ptr5 = Marshal.GetIUnknownForObject(o5);
        try
        {
            nint[,] result = new nint[2, 3]
            {
                { 0, ptr1, ptr2 },
                { ptr3, ptr4, ptr5 }
            };

            SAFEARRAY* psa = CreateSafeArray(VT_DISPATCH, result);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_UNKNOWN,
                data = new()
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
                Assert.Equal(new object[,]
                {
                    { null, o1, o2 },
                    { o3, o4, o5 }
                }, array);
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
        object o1 = new();
        object o2 = new();
        object o3 = new();
        object o4 = new();
        object o5 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);
        nint ptr3 = Marshal.GetIUnknownForObject(o3);
        nint ptr4 = Marshal.GetIUnknownForObject(o4);
        nint ptr5 = Marshal.GetIUnknownForObject(o5);
        try
        {
            nint[,] result = new nint[2, 3]
            {
                { 0, ptr1, ptr2 },
                { ptr3, ptr4, ptr5 }
            };

            SAFEARRAY* psa = CreateSafeArray(VT_DISPATCH, result, 1, 2);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_UNKNOWN,
                data = new()
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
        object o1 = new();
        object o2 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);

        try
        {
            nint[] result = [0, ptr1, ptr2];
            SAFEARRAY* psa = CreateSafeArray(VT_DISPATCH, result);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_DISPATCH,
                data = new()
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
        object o1 = new();
        object o2 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);

        try
        {
            nint[] result = [0, ptr1, ptr2];
            SAFEARRAY* psa = CreateSafeArray(VT_DISPATCH, result, 1);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_DISPATCH,
                data = new()
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
        object o1 = new();
        object o2 = new();
        object o3 = new();
        object o4 = new();
        object o5 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);
        nint ptr3 = Marshal.GetIUnknownForObject(o3);
        nint ptr4 = Marshal.GetIUnknownForObject(o4);
        nint ptr5 = Marshal.GetIUnknownForObject(o5);

        try
        {
            nint[,] result = new nint[2, 3]
            {
                { 0, ptr1, ptr2 },
                { ptr3, ptr4, ptr5 }
            };

            SAFEARRAY* psa = CreateSafeArray(VT_DISPATCH, result);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_DISPATCH,
                data = new()
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
                Assert.Equal(new object[,]
                {
                    { null, o1, o2 },
                    { o3, o4, o5 }
                }, array);
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
        object o1 = new();
        object o2 = new();
        object o3 = new();
        object o4 = new();
        object o5 = new();
        nint ptr1 = Marshal.GetIUnknownForObject(o1);
        nint ptr2 = Marshal.GetIUnknownForObject(o2);
        nint ptr3 = Marshal.GetIUnknownForObject(o3);
        nint ptr4 = Marshal.GetIUnknownForObject(o4);
        nint ptr5 = Marshal.GetIUnknownForObject(o5);

        try
        {
            nint[,] result = new nint[2, 3]
            {
                { 0, ptr1, ptr2 },
                { ptr3, ptr4, ptr5 }
            };

            SAFEARRAY* psa = CreateSafeArray(VT_DISPATCH, result, 1, 2);
            using VARIANT variant = new()
            {
                vt = VT_ARRAY | VT_DISPATCH,
                data = new()
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
        using VARIANT v1 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 1
            }
        };

        using VARIANT v2 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 2
            }
        };

        using VARIANT v3 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 3
            }
        };

        VARIANT[] result = [v1, v2, v3];
        SAFEARRAY* psa = CreateSafeArray(VT_VARIANT, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_VARIANT,
            data = new()
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
        using VARIANT v1 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 1
            }
        };

        using VARIANT v2 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 2
            }
        };

        using VARIANT v3 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 3
            }
        };

        VARIANT[] result = [v1, v2, v3];
        SAFEARRAY* psa = CreateSafeArray(VT_VARIANT, result, 1);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_VARIANT,
            data = new()
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
        });
    }

    [StaFact]
    public void VARIANT_ToObject_ARRAYVARIANTMultiDimension_ReturnsExpected()
    {
        using VARIANT v1 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 1
            }
        };

        using VARIANT v2 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 2
            }
        };

        using VARIANT v3 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 3
            }
        };

        using VARIANT v4 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 4
            }
        };

        using VARIANT v5 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 5
            }
        };

        using VARIANT v6 = new()
        {
            vt = VT_I4,
            data = new()
            {
                llVal = 6
            }
        };

        VARIANT[,] result = new VARIANT[2, 3]
        {
            { v1, v2, v3 },
            { v4, v5, v6 }
        };

        SAFEARRAY* psa = CreateSafeArray(VT_VARIANT, result);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_VARIANT,
            data = new()
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
            Assert.Equal(new object[,]
            {
                { 1, 2, 3 },
                { 4, 5, 6 }
            }, array);
        });
    }

    [StaTheory]
    [InlineData((ushort)VT_I1)]
    [InlineData((ushort)VT_UI1)]
    [InlineData((ushort)VT_I2)]
    [InlineData((ushort)VT_UI2)]
    [InlineData((ushort)VT_I4)]
    [InlineData((ushort)VT_UI4)]
    [InlineData((ushort)VT_I8)]
    [InlineData((ushort)VT_UI8)]
    [InlineData((ushort)VT_BSTR)]
    [InlineData((ushort)VT_LPWSTR)]
    [InlineData((ushort)VT_LPSTR)]
    [InlineData((ushort)VT_UNKNOWN)]
    [InlineData((ushort)VT_DISPATCH)]
    [InlineData((ushort)VT_EMPTY)]
    [InlineData((ushort)VT_NULL)]
    [InlineData((ushort)VT_CF)]
    [InlineData((ushort)VT_VOID)]
    [InlineData((ushort)VT_PTR)]
    [InlineData((ushort)VT_SAFEARRAY)]
    [InlineData((ushort)VT_CARRAY)]
    [InlineData((ushort)VT_RECORD)]
    [InlineData((ushort)VT_BLOB)]
    [InlineData((ushort)VT_STREAM)]
    [InlineData((ushort)VT_STORAGE)]
    [InlineData((ushort)VT_STREAMED_OBJECT)]
    [InlineData((ushort)VT_STORED_OBJECT)]
    [InlineData((ushort)VT_BLOB_OBJECT)]
    public void VARIANT_ToObject_ARRAYNoData_ReturnsExpected(ushort vt)
    {
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | (VARENUM)vt
        };

        AssertToObjectEqual(null, variant);
    }

    [StaTheory]
    [InlineData(128)]
    [InlineData(129)]
    [InlineData((ushort)VT_BSTR_BLOB)]
    public void VARIANT_ToObject_ARRAYInvalidTypeNoData_ThrowsInvalidOleVariantTypeException(ushort vt)
    {
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | (VARENUM)vt
        };

        AssertToObjectThrows<InvalidOleVariantTypeException>(variant);
    }

    [StaFact]
    public void VARIANT_ToObject_ARRAYVECTOR_ThrowsInvalidOleVariantTypeException()
    {
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_VECTOR | VT_I4
        };

        Assert.Throws<ArgumentException>(variant.ToObject);
    }

    [StaFact]
    public void VARIANT_ToObject_ARRAYTypeEMPTY_ThrowsInvalidOleVariantTypeException()
    {
        SAFEARRAY* psa = CreateSafeArray(VT_I1, Array.Empty<byte>());
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_EMPTY,
            data = new()
            {
                parray = psa
            }
        };

        AssertToObjectThrows<InvalidOleVariantTypeException>(variant);
    }

    [StaTheory]
    [InlineData((ushort)VT_I1, (ushort)VT_UI1)]
    [InlineData((ushort)VT_UI1, (ushort)VT_I1)]
    [InlineData((ushort)VT_I2, (ushort)VT_UI2)]
    [InlineData((ushort)VT_UI2, (ushort)VT_I2)]
    [InlineData((ushort)VT_I4, (ushort)VT_UI4)]
    [InlineData((ushort)VT_UI4, (ushort)VT_I4)]
    [InlineData((ushort)VT_INT, (ushort)VT_UINT)]
    [InlineData((ushort)VT_INT, (ushort)VT_I2)]
    [InlineData((ushort)VT_INT, (ushort)VT_I8)]
    [InlineData((ushort)VT_UINT, (ushort)VT_INT)]
    [InlineData((ushort)VT_UINT, (ushort)VT_UI2)]
    [InlineData((ushort)VT_UINT, (ushort)VT_UI8)]
    [InlineData((ushort)VT_I8, (ushort)VT_UI8)]
    [InlineData((ushort)VT_UI8, (ushort)VT_I8)]
    [InlineData((ushort)VT_UNKNOWN, (ushort)VT_DISPATCH)]
    [InlineData((ushort)VT_UNKNOWN, (ushort)VT_I4)]
    [InlineData((ushort)VT_UNKNOWN, (ushort)VT_UI4)]
    [InlineData((ushort)VT_UNKNOWN, (ushort)VT_I8)]
    [InlineData((ushort)VT_UNKNOWN, (ushort)VT_UI8)]
    [InlineData((ushort)VT_DISPATCH, (ushort)VT_I4)]
    [InlineData((ushort)VT_DISPATCH, (ushort)VT_UI4)]
    [InlineData((ushort)VT_DISPATCH, (ushort)VT_I8)]
    [InlineData((ushort)VT_DISPATCH, (ushort)VT_UI8)]
    public void VARIANT_ToObject_ARRAYTypeDifferent_ThrowsSafeArrayTypeMismatchException(ushort arrayVt, ushort vt)
    {
        SAFEARRAY* psa = CreateSafeArray((VARENUM)arrayVt, Array.Empty<byte>());
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | (VARENUM)vt,
            data = new()
            {
                parray = psa
            }
        };

        AssertToObjectThrows<SafeArrayTypeMismatchException>(variant);
    }

    [StaTheory]
    [InlineData((ushort)VT_INT_PTR)]
    [InlineData((ushort)VT_UINT_PTR)]
    public void VARIANT_ToObject_ARRAYTypeInvalid_ThrowsArgumentException(ushort vt)
    {
        SAFEARRAY* psa = CreateSafeArray((VARENUM)vt, Array.Empty<byte>());
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | (VARENUM)vt,
            data = new()
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

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreate(VT_I4, (uint)rank, saBounds);
        using VARIANT variant = new()
        {
            vt = VT_ARRAY | VT_I4,
            data = new()
            {
                parray = psa
            }
        };

        AssertToObjectThrows<TypeLoadException>(variant);
    }

    private static unsafe SAFEARRAY* CreateSafeArray<T>(VARENUM vt, T[] result, int lbound = 0) where T : unmanaged
    {
        SAFEARRAYBOUND saBound = new()
        {
            cElements = (uint)result.Length,
            lLbound = lbound
        };

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreate(vt, 1, &saBound);
        NativeAssert.NotNull(psa);

        VARENUM arrayVt = VT_EMPTY;
        HRESULT hr = PInvokeCore.SafeArrayGetVartype(psa, &arrayVt);
        Assert.Equal(HRESULT.S_OK, hr);
        Assert.Equal(vt, arrayVt);

        for (int i = 0; i < result.Length; i++)
        {
            T value = result[i];
            int index = i + lbound;

            // Insert pointers directly.
            hr = value is nint valuePtr
                ? PInvokeCore.SafeArrayPutElement(psa, &index, (void*)valuePtr)
                : PInvokeCore.SafeArrayPutElement(psa, &index, &value);

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

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreate(vt, 2, saBounds);
        NativeAssert.NotNull(psa);

        VARENUM arrayVt = VT_EMPTY;
        HRESULT hr = PInvokeCore.SafeArrayGetVartype(psa, &arrayVt);
        Assert.Equal(HRESULT.S_OK, hr);
        Assert.Equal(vt, arrayVt);

        for (int i = 0; i < multiDimArray.GetLength(0); i++)
        {
            for (int j = 0; j < multiDimArray.GetLength(1); j++)
            {
#pragma warning disable CA2014 // Do not use stackalloc in loops
                int* indices = stackalloc int[] { i + lbound1, j + lbound2 };
#pragma warning restore CA2014
                T value = multiDimArray[i, j];

                // Insert pointers directly.
                hr = value is nint valuePtr
                    ? PInvokeCore.SafeArrayPutElement(psa, indices, (void*)valuePtr)
                    : PInvokeCore.SafeArrayPutElement(psa, indices, &value);

                Assert.Equal(HRESULT.S_OK, hr);
            }
        }

        return psa;
    }

    [StaFact]
    public void ToObject_RECORDRecordData_ReturnsExpected()
    {
        int record = 1;
        nint mem = Marshal.AllocCoTaskMem(sizeof(int));
        *(int*)mem = record;
        CustomRecordInfo recordInfo = new()
        {
            GetGuidAction = () => (typeof(int).GUID, HRESULT.S_OK)
        };

        using VARIANT variant = new() { vt = VT_RECORD };
        variant.data.Anonymous.pRecInfo = recordInfo.GetComInterface();
        variant.data.Anonymous.pvRecord = mem.ToPointer();

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
        CustomRecordInfo recordInfo = new();
        using VARIANT variant = new() { vt = VT_RECORD };
        variant.data.Anonymous.pRecInfo = recordInfo.GetComInterface();
        AssertToObjectEqual(null, variant);
    }

    [StaFact]
    public void ToObject_RECORDNullRecordInfo_ThrowsArgumentException()
    {
        int record = 1;
        nint mem = Marshal.AllocCoTaskMem(sizeof(int));
        (*(int*)mem) = record;

        using VARIANT variant = new() { vt = VT_RECORD };
        variant.data.Anonymous.pvRecord = mem.ToPointer();
        AssertToObjectThrows<ArgumentException>(variant);
    }

    [StaFact]
    public void ToObject_RECORDInvalidGetGuidHRData_ThrowsArgumentException()
    {
        int record = 1;
        nint mem = Marshal.AllocCoTaskMem(sizeof(int));
        (*(int*)mem) = record;

        CustomRecordInfo recordInfo = new()
        {
            GetGuidAction = () => (Guid.Empty, HRESULT.DISP_E_DIVBYZERO)
        };

        using VARIANT variant = new() { vt = VT_RECORD };
        variant.data.Anonymous.pRecInfo = recordInfo.GetComInterface();
        variant.data.Anonymous.pvRecord = mem.ToPointer();

        // Records actually don't work in .NET Core...
#if false
        AssertToObjectThrows<DivideByZeroException>(variant);
#endif
    }

    [StaFact]
    public void ToObject_RECORDInvalidGetGuidHRNoData_ReturnsNull()
    {
        CustomRecordInfo recordInfo = new()
        {
            GetGuidAction = () => (Guid.Empty, HRESULT.DISP_E_DIVBYZERO)
        };

        using VARIANT variant = new() { vt = VT_RECORD };
        variant.data.Anonymous.pRecInfo = recordInfo.GetComInterface();
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
        nint mem = Marshal.AllocCoTaskMem(sizeof(int));
        (*(int*)mem) = record;

        CustomRecordInfo recordInfo = new CustomRecordInfo
        {
            GetGuidAction = () => (guid, HRESULT.S_OK)
        };

        using VARIANT variant = new() { vt = VT_RECORD };
        variant.data.Anonymous.pRecInfo = recordInfo.GetComInterface();
        variant.data.Anonymous.pvRecord = mem.ToPointer();

        AssertToObjectThrows<ArgumentException>(variant);
    }

    [StaTheory]
    [MemberData(nameof(RECORD_TestData))]
    public void ToObject_RECORDInvalidGuidNoData_ReturnsNull(Guid guid)
    {
        CustomRecordInfo recordInfo = new()
        {
            GetGuidAction = () => (guid, HRESULT.S_OK)
        };

        using VARIANT variant = new() { vt = VT_RECORD };
        variant.data.Anonymous.pRecInfo = recordInfo.GetComInterface();
        AssertToObjectEqual(null, variant);
    }

    [StaFact]
    public void ToObject_RECORDARRAYValid_ReturnsExpected()
    {
        int[] result = [1, 2];
        CustomRecordInfo recordInfo = new()
        {
            GetGuidAction = () => (typeof(int).GUID, HRESULT.S_OK)
        };

        using ComScope<IRecordInfo> pRecordInfo = new(recordInfo.GetComInterface());
        using VARIANT variant = new() { vt = VT_ARRAY | VT_RECORD };
        variant.data.parray = CreateRecordSafeArray(result, pRecordInfo);

        // Records actually don't work in .NET Core...
#if false
        AssertToObjectEqual(new int[] { 0, 0 }, variant);
#endif
    }

    [StaFact]
    public void ToObject_RECORDARRAYInvalidFFeatures_ThrowsArgumentException()
    {
        int[] result = [1, 2];
        CustomRecordInfo recordInfo = new();
        using ComScope<IRecordInfo> pRecordInfo = new(recordInfo.GetComInterface());
        SAFEARRAY* psa = CreateRecordSafeArray(result, pRecordInfo);
        psa->fFeatures &= ~FADF_RECORD;
        try
        {
            using VARIANT variant = new() { vt = VT_ARRAY | VT_RECORD };
            variant.data.parray = psa;
            AssertToObjectThrows<ArgumentException>(variant);
        }
        finally
        {
            // Make sure disposal works.
            psa->fFeatures |= FADF_RECORD;
        }
    }

    [StaFact]
    public void ToObject_RECORDARRAYInvalidGetGuidHR_ThrowsArgumentException()
    {
        int[] result = [1, 2];
        CustomRecordInfo record = new()
        {
            GetGuidAction = () => (Guid.Empty, HRESULT.DISP_E_DIVBYZERO)
        };

        using ComScope<IRecordInfo> pRecordInfo = new(record.GetComInterface());
        using VARIANT variant = new() { vt = VT_ARRAY | VT_RECORD };
        variant.data.parray = CreateRecordSafeArray(result, pRecordInfo);

        VARIANT copy = variant;
        nint pv = (nint)(&copy);
        Assert.Throws<ArgumentException>(() => Marshal.GetObjectForNativeVariant(pv));
        Assert.Throws<ArgumentException>(variant.ToObject);
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
        int[] result = [1, 2];
        CustomRecordInfo record = new()
        {
            GetGuidAction = () => (guid, HRESULT.S_OK)
        };

        using ComScope<IRecordInfo> pRecordInfo = new(record.GetComInterface());
        using VARIANT variant = new() { vt = VT_ARRAY | VT_RECORD };
        variant.data.parray = CreateRecordSafeArray(result, pRecordInfo);
        AssertToObjectThrows<ArgumentException>(variant);
    }

    private class CustomRecordInfo : IRecordInfo.Interface
    {
        public IRecordInfo* GetComInterface() => (IRecordInfo*)Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo.Interface>(this);

        public HRESULT RecordInit(void* pvNew) => HRESULT.E_NOTIMPL;

        public HRESULT RecordClear(void* pvExisting) => HRESULT.E_NOTIMPL;

        public HRESULT RecordCopy(void* pvExisting, void* pvNew) => HRESULT.E_NOTIMPL;

        public Func<(Guid, HRESULT)> GetGuidAction { get; set; }

        public HRESULT GetGuid(Guid* pguid)
        {
            (Guid guid, HRESULT hr) = GetGuidAction();
            *pguid = guid;
            return hr;
        }

        public HRESULT GetName(BSTR* pbstrName) => HRESULT.E_NOTIMPL;

        public HRESULT GetSize(uint* pcbSize)
        {
            *pcbSize = sizeof(int);
            return HRESULT.S_OK;
        }

        public HRESULT GetTypeInfo(ITypeInfo** ppTypeInfo) => HRESULT.E_NOTIMPL;

        public HRESULT GetField(void* pvData, PCWSTR szFieldName, VARIANT* pvarField) => HRESULT.E_NOTIMPL;

        public HRESULT GetFieldNoCopy(void* pvData, PCWSTR szFieldName, VARIANT* pvarField, void** ppvDataCArray) => HRESULT.E_NOTIMPL;

        public HRESULT PutField(uint wFlags, void* pvData, PCWSTR szFieldName, VARIANT* pvarField) => HRESULT.E_NOTIMPL;

        public HRESULT PutFieldNoCopy(uint wFlags, void* pvData, PCWSTR szFieldName, VARIANT* pvarField) => HRESULT.E_NOTIMPL;

        public HRESULT GetFieldNames(uint* pcNames, BSTR* rgBstrNames) => HRESULT.E_NOTIMPL;

        public BOOL IsMatchingType(IRecordInfo* pRecordInfoInfo) => throw new NotImplementedException();

        public void* RecordCreate() => throw new NotImplementedException();

        public HRESULT RecordCreateCopy(void* pvSource, void** ppvDest) => HRESULT.E_NOTIMPL;

        public HRESULT RecordDestroy(void* pvRecord) => HRESULT.E_NOTIMPL;
    }

    private static SAFEARRAY* CreateRecordSafeArray<T>(T[] result, IRecordInfo* recordInfo, int lbound = 0)
    {
        SAFEARRAYBOUND saBound = new()
        {
            cElements = (uint)result.Length,
            lLbound = lbound
        };

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreateEx(VT_RECORD, 1, &saBound, recordInfo);
        NativeAssert.NotNull(psa);

        VARENUM arrayVt = VT_EMPTY;
        HRESULT hr = PInvokeCore.SafeArrayGetVartype(psa, &arrayVt);
        Assert.Equal(HRESULT.S_OK, hr);
        Assert.Equal(VT_RECORD, arrayVt);

        return psa;
    }

    private static void AssertToObjectThrows<T>(VARIANT variant) where T : Exception
    {
        VARIANT copy = variant;
        nint pv = (nint)(&copy);
        Assert.Throws<T>(() => Marshal.GetObjectForNativeVariant(pv));

        Assert.Throws<T>(variant.ToObject);
    }

    private static void AssertToObjectEqual(object expected, VARIANT variant)
        => AssertToObject(variant, actual => Assert.Equal(expected, actual));

    private static void AssertToObjectEqualExtension<T>(object expected, VARIANT variant) where T : Exception
    {
        // Not supported type.
        VARIANT copy = variant;
        nint pv = (nint)(&copy);
        Assert.Throws<T>(() => Marshal.GetObjectForNativeVariant(pv));

        Assert.Equal(expected, variant.ToObject());
    }

    private static void AssertToObject(VARIANT variant, Action<object> action)
    {
        object marshaller = Marshal.GetObjectForNativeVariant((nint)(void*)&variant);
        action(marshaller);

        object toObject = variant.ToObject();
        action(toObject);

        Assert.Equal(marshaller, toObject);
    }

    [Fact]
    public void MarshallingFromExchangeTypes()
    {
        // These are the common TypeConverter types we're using.

        using (VARIANT variant = default)
        {
            byte[] bytes = [1, 2, 3];
            Marshal.GetNativeVariantForObject(bytes, (nint)(void*)&variant);
            Assert.Equal(VT_UI1 | VT_ARRAY, variant.vt);
        }

        using (VARIANT variant = default)
        {
            string value = "Testing";
            Marshal.GetNativeVariantForObject(value, (nint)(void*)&variant);
            Assert.Equal(VT_BSTR, variant.vt);
        }
    }

    [Fact]
    public void MarshallingFromIntAndUint()
    {
        // Interop marshals as VT_I4/VT_UI4 and not VT_INT/VT_UINT
        VARIANT variant = default;
        int intValue = 42;
        Marshal.GetNativeVariantForObject(intValue, (nint)(void*)&variant);
        variant.vt.Should().Be(VT_I4);
        uint uintValue = 42;
        Marshal.GetNativeVariantForObject(uintValue, (nint)(void*)&variant);
        variant.vt.Should().Be(VT_UI4);
    }

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromCLSID(Guid* clsid, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromFileTime(FILETIME* pftIn, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitVariantFromFileTime(FILETIME* pftIn, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromBuffer(void* pv, uint cb, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromInt16Vector(void* pv, uint cb, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromUInt16Vector(void* pv, uint cb, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromBooleanVector(void* pv, uint cb, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromInt32Vector(void* pv, uint cb, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromUInt32Vector(void* pv, uint cb, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromInt64Vector(void* pv, uint cb, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromUInt64Vector(void* pv, uint cb, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromDoubleVector(void* pv, uint cb, VARIANT* ppropvar);

    [DllImport(Libraries.Propsys, ExactSpelling = true)]
    private static extern unsafe HRESULT InitPropVariantFromFileTimeVector(void* pv, uint cb, VARIANT* ppropvar);

    [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
    private static extern HRESULT VarDecFromI8(long i64In, out DECIMAL pdecOut);

    [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
    private static extern HRESULT VarDecFromR8(double dblIn, out DECIMAL pdecOut);
}
