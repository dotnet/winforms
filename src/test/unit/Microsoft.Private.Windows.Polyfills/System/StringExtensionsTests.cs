// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void CopyTo_CopiesContents()
    {
        string source = "Hello";
        Span<char> destination = stackalloc char[10];
        source.CopyTo(destination);

        destination[..5].SequenceEqual("Hello").Should().BeTrue();
    }

    [Fact]
    public void CopyTo_ExactLength_Succeeds()
    {
        string source = "Test";
        Span<char> destination = stackalloc char[4];
        source.CopyTo(destination);

        destination.SequenceEqual("Test").Should().BeTrue();
    }

    [Fact]
    public void CopyTo_DestinationTooShort_ThrowsArgumentException()
    {
        string source = "Hello";
        char[] destination = new char[3];

        Action action = () => source.CopyTo(destination);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CopyTo_EmptyString_Succeeds()
    {
        string source = string.Empty;
        Span<char> destination = stackalloc char[5];
        source.CopyTo(destination);
    }

    [Fact]
    public void Concat_TwoSpans_ConcatenatesCorrectly()
    {
        ReadOnlySpan<char> a = "Hello".AsSpan();
        ReadOnlySpan<char> b = " World".AsSpan();

        string result = string.Concat(a, b);
        result.Should().Be("Hello World");
    }

    [Fact]
    public void Concat_TwoSpans_BothEmpty_ReturnsEmpty()
    {
        string result = string.Concat(ReadOnlySpan<char>.Empty, ReadOnlySpan<char>.Empty);
        result.Should().BeEmpty();
    }

    [Fact]
    public void Concat_TwoSpans_FirstEmpty_ReturnsSecond()
    {
        string result = string.Concat(ReadOnlySpan<char>.Empty, "World".AsSpan());
        result.Should().Be("World");
    }

    [Fact]
    public void Concat_ThreeSpans_ConcatenatesCorrectly()
    {
        ReadOnlySpan<char> a = "A".AsSpan();
        ReadOnlySpan<char> b = "B".AsSpan();
        ReadOnlySpan<char> c = "C".AsSpan();

        string result = string.Concat(a, b, c);
        result.Should().Be("ABC");
    }

    [Fact]
    public void Concat_ThreeSpans_AllEmpty_ReturnsEmpty()
    {
        string result = string.Concat(
            ReadOnlySpan<char>.Empty,
            ReadOnlySpan<char>.Empty,
            ReadOnlySpan<char>.Empty);
        result.Should().BeEmpty();
    }

    [Fact]
    public void Concat_FourSpans_ConcatenatesCorrectly()
    {
        ReadOnlySpan<char> a = "1".AsSpan();
        ReadOnlySpan<char> b = "2".AsSpan();
        ReadOnlySpan<char> c = "3".AsSpan();
        ReadOnlySpan<char> d = "4".AsSpan();

        string result = string.Concat(a, b, c, d);
        result.Should().Be("1234");
    }

    [Fact]
    public void Concat_FourSpans_AllEmpty_ReturnsEmpty()
    {
        string result = string.Concat(
            ReadOnlySpan<char>.Empty,
            ReadOnlySpan<char>.Empty,
            ReadOnlySpan<char>.Empty,
            ReadOnlySpan<char>.Empty);
        result.Should().BeEmpty();
    }

    [Fact]
    public void Concat_FourSpans_MixedEmpty_ConcatenatesCorrectly()
    {
        string result = string.Concat(
            "A".AsSpan(),
            ReadOnlySpan<char>.Empty,
            "C".AsSpan(),
            ReadOnlySpan<char>.Empty);
        result.Should().Be("AC");
    }

    [Fact]
    public void Concat_LongerStrings_ConcatenatesCorrectly()
    {
        string long1 = new('x', 100);
        string long2 = new('y', 200);
        string result = string.Concat(long1.AsSpan(), long2.AsSpan());
        result.Length.Should().Be(300);
        result[0].Should().Be('x');
        result[99].Should().Be('x');
        result[100].Should().Be('y');
        result[299].Should().Be('y');
    }
}
