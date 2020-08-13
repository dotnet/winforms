// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using static Interop;
using static Interop.Oleaut32;

namespace System.Windows.Forms.Tests.Interop.Oleaut32
{
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
            var dec = new DECIMAL();
            Assert.Equal(0m, dec.ToDecimal());
        }

        [Theory]
        [InlineData((double)int.MinValue)]
        [InlineData(-1.2)]
        [InlineData(0)]
        [InlineData(1.2)]
        [InlineData((double)int.MaxValue)]
        public void DECIMAL_ToDecimal_InvokeCustom_ReturnsExpected(double value)
        {
            HRESULT hr = VarDecFromR8(value, out DECIMAL dec);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal((decimal)value, dec.ToDecimal());
        }

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private static extern HRESULT VarDecFromR8(double dblIn, out DECIMAL pdecOut);
    }
}
