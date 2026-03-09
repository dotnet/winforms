// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.IO.Tests;

public class PathExtensionsTests
{
    [Fact]
    public void Join_TwoStrings_JoinsWithSeparator()
    {
        string result = Path.Join("dir", "file.txt");
        result.Should().Be(@"dir\file.txt");
    }

    [Fact]
    public void Join_TwoStrings_FirstEmpty_ReturnsSecond()
    {
        string result = Path.Join("", "file.txt");
        result.Should().Be("file.txt");
    }

    [Fact]
    public void Join_TwoStrings_SecondEmpty_ReturnsFirst()
    {
        string result = Path.Join("dir", "");
        result.Should().Be("dir");
    }

    [Fact]
    public void Join_TwoStrings_FirstNull_ReturnsSecond()
    {
        string result = Path.Join(null!, "file.txt");
        result.Should().Be("file.txt");
    }

    [Fact]
    public void Join_TwoStrings_FirstHasTrailingSeparator_NoDoubleSeparator()
    {
        string result = Path.Join(@"dir\", "file.txt");
        result.Should().Be(@"dir\file.txt");
    }

    [Fact]
    public void Join_TwoStrings_SecondHasLeadingSeparator_NoDoubleSeparator()
    {
        string result = Path.Join("dir", @"\file.txt");
        result.Should().Be(@"dir\file.txt");
    }

    [Fact]
    public void Join_TwoStrings_AltSeparator_NoDoubleSeparator()
    {
        string result = Path.Join("dir/", "file.txt");
        result.Should().Be("dir/file.txt");
    }

    [Fact]
    public void Join_TwoSpans_JoinsWithSeparator()
    {
        string result = Path.Join("dir".AsSpan(), "file.txt".AsSpan());
        result.Should().Be(@"dir\file.txt");
    }

    [Fact]
    public void Join_TwoSpans_FirstEmpty_ReturnsSecond()
    {
        string result = Path.Join(ReadOnlySpan<char>.Empty, "file.txt".AsSpan());
        result.Should().Be("file.txt");
    }

    [Fact]
    public void Join_TwoSpans_SecondEmpty_ReturnsFirst()
    {
        string result = Path.Join("dir".AsSpan(), ReadOnlySpan<char>.Empty);
        result.Should().Be("dir");
    }

    [Fact]
    public void Join_TwoSpans_BothEmpty_ReturnsEmpty()
    {
        string result = Path.Join(ReadOnlySpan<char>.Empty, ReadOnlySpan<char>.Empty);
        result.Should().BeEmpty();
    }

    [Fact]
    public void Join_TwoSpans_TrailingSeparator_NoDoubleSeparator()
    {
        string result = Path.Join(@"dir\".AsSpan(), "file.txt".AsSpan());
        result.Should().Be(@"dir\file.txt");
    }

    [Fact]
    public void Join_MultiplePathParts()
    {
        string result = Path.Join("root", "sub");
        result = Path.Join(result, "file.txt");
        result.Should().Be(@"root\sub\file.txt");
    }
}
