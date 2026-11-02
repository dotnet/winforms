// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Tests.Interop.Oleaut32;

// NB: doesn't require thread affinity
public class DECIMALTests
{
    [Fact]
    public unsafe void DECIMAL_Sizeof_Invoke_ReturnsExpected()
    {
        Assert.Equal(16, Marshal.SizeOf<DECIMAL>());
        Assert.Equal(16, sizeof(DECIMAL));
    }

    [Fact]
    public void DECIMAL_ToDecimal_InvokeEmpty_ReturnsExpected()
    {
        DECIMAL dec = default;
        Assert.Equal(0m, dec.ToDecimal());
    }

    public static IEnumerable<object[]> DECIMAL_ToDecimal_TestData()
    {
        yield return new object[] { (double)int.MinValue, (decimal)int.MinValue };
        yield return new object[] { -1.2, -1.2m };
        yield return new object[] { 0d, 0m };
        yield return new object[] { 1.2, 1.2m };
        yield return new object[] { (double)int.MaxValue, (decimal)int.MaxValue };
    }

    [Theory]
    [MemberData(nameof(DECIMAL_ToDecimal_TestData))]
    public void DECIMAL_ToDecimal_InvokeCustom_ReturnsExpected(double value, decimal expected)
    {
        HRESULT hr = VarDecFromR8(value, out DECIMAL dec);
        Assert.Equal(HRESULT.S_OK, hr);
        Assert.Equal(expected, dec.ToDecimal());
    }

    [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
    private static extern HRESULT VarDecFromR8(double dblIn, out DECIMAL pdecOut);
}
