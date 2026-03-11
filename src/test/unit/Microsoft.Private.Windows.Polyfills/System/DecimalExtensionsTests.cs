// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class DecimalExtensionsTests
{
    [Fact]
    public void TryGetBits_Zero_ReturnsExpectedBits()
    {
        Span<int> bits = stackalloc int[4];
        bool result = decimal.TryGetBits(0m, bits, out int valuesWritten);

        result.Should().BeTrue();
        valuesWritten.Should().Be(4);
        bits[0].Should().Be(0); // Low
        bits[1].Should().Be(0); // Mid
        bits[2].Should().Be(0); // High
        bits[3].Should().Be(0); // Flags
    }

    [Fact]
    public void TryGetBits_One_ReturnsExpectedBits()
    {
        Span<int> bits = stackalloc int[4];
        bool result = decimal.TryGetBits(1m, bits, out int valuesWritten);

        result.Should().BeTrue();
        valuesWritten.Should().Be(4);
        bits[0].Should().Be(1);
        bits[1].Should().Be(0);
        bits[2].Should().Be(0);
        bits[3].Should().Be(0);
    }

    [Fact]
    public void TryGetBits_NegativeValue_HasSignFlag()
    {
        Span<int> bits = stackalloc int[4];
        bool result = decimal.TryGetBits(-1m, bits, out int valuesWritten);

        result.Should().BeTrue();
        valuesWritten.Should().Be(4);
        bits[0].Should().Be(1);
        // Sign bit should be set in flags
        (bits[3] & unchecked((int)0x80000000)).Should().NotBe(0);
    }

    [Fact]
    public void TryGetBits_DecimalWithScale_HasScaleInFlags()
    {
        Span<int> bits = stackalloc int[4];
        // 1.5 = 15 / 10^1, scale is 1
        bool result = decimal.TryGetBits(1.5m, bits, out int valuesWritten);

        result.Should().BeTrue();
        valuesWritten.Should().Be(4);
        bits[0].Should().Be(15);
    }

    [Fact]
    public void TryGetBits_MatchesDecimalGetBits()
    {
        decimal[] testValues = [0m, 1m, -1m, decimal.MaxValue, decimal.MinValue, 123.456m, -987.654m];
        Span<int> actual = stackalloc int[4];

        foreach (decimal value in testValues)
        {
            int[] expected = decimal.GetBits(value);
            bool result = decimal.TryGetBits(value, actual, out int valuesWritten);

            result.Should().BeTrue();
            valuesWritten.Should().Be(4);
            actual[0].Should().Be(expected[0]);
            actual[1].Should().Be(expected[1]);
            actual[2].Should().Be(expected[2]);
            actual[3].Should().Be(expected[3]);
        }
    }

    [Fact]
    public void TryGetBits_BufferTooSmall_ReturnsFalse()
    {
        Span<int> bits = stackalloc int[3];
        bool result = decimal.TryGetBits(1m, bits, out int valuesWritten);

        result.Should().BeFalse();
        valuesWritten.Should().Be(0);
    }

    [Fact]
    public void TryGetBits_EmptyBuffer_ReturnsFalse()
    {
        Span<int> bits = [];
        bool result = decimal.TryGetBits(1m, bits, out int valuesWritten);

        result.Should().BeFalse();
        valuesWritten.Should().Be(0);
    }

    [Fact]
    public void TryGetBits_LargerBuffer_WritesOnlyFour()
    {
        Span<int> bits = stackalloc int[8];
        bits.Fill(-1);
        bool result = decimal.TryGetBits(42m, bits, out int valuesWritten);

        result.Should().BeTrue();
        valuesWritten.Should().Be(4);
        // Beyond the 4 values, the buffer should be untouched
        bits[4].Should().Be(-1);
        bits[5].Should().Be(-1);
    }
}
