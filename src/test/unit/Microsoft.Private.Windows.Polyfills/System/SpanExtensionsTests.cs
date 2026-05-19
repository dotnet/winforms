// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class SpanExtensionsTests
{
    [Fact]
    public void IndexOfAnyExcept_AllSame_ReturnsNegativeOne()
    {
        ReadOnlySpan<int> span = [1, 1, 1, 1];
        span.IndexOfAnyExcept(1).Should().Be(-1);
    }

    [Fact]
    public void IndexOfAnyExcept_FirstIsDifferent_ReturnsZero()
    {
        ReadOnlySpan<int> span = [2, 1, 1, 1];
        span.IndexOfAnyExcept(1).Should().Be(0);
    }

    [Fact]
    public void IndexOfAnyExcept_LastIsDifferent_ReturnsLastIndex()
    {
        ReadOnlySpan<int> span = [1, 1, 1, 2];
        span.IndexOfAnyExcept(1).Should().Be(3);
    }

    [Fact]
    public void IndexOfAnyExcept_MiddleIsDifferent_ReturnsMiddleIndex()
    {
        ReadOnlySpan<int> span = [1, 1, 3, 1];
        span.IndexOfAnyExcept(1).Should().Be(2);
    }

    [Fact]
    public void IndexOfAnyExcept_Empty_ReturnsNegativeOne()
    {
        ReadOnlySpan<int> span = [];
        span.IndexOfAnyExcept(1).Should().Be(-1);
    }

    [Fact]
    public void IndexOfAnyExcept_SingleElement_Same_ReturnsNegativeOne()
    {
        ReadOnlySpan<int> span = [5];
        span.IndexOfAnyExcept(5).Should().Be(-1);
    }

    [Fact]
    public void IndexOfAnyExcept_SingleElement_Different_ReturnsZero()
    {
        ReadOnlySpan<int> span = [5];
        span.IndexOfAnyExcept(3).Should().Be(0);
    }

    [Fact]
    public void IndexOfAnyExcept_Chars_FindsFirstNonSpace()
    {
        ReadOnlySpan<char> span = "   hello".AsSpan();
        span.IndexOfAnyExcept(' ').Should().Be(3);
    }

    [Fact]
    public void IndexOfAnyExcept_Chars_AllSpaces_ReturnsNegativeOne()
    {
        ReadOnlySpan<char> span = "     ".AsSpan();
        span.IndexOfAnyExcept(' ').Should().Be(-1);
    }

    [Fact]
    public void IndexOfAnyExcept_Bytes_FindsFirstNonZero()
    {
        ReadOnlySpan<byte> span = [0, 0, 0, 42, 0];
        span.IndexOfAnyExcept((byte)0).Should().Be(3);
    }
}
