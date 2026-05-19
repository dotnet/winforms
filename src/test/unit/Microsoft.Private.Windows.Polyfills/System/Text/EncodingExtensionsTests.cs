// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Text.Tests;

public class EncodingExtensionsTests
{
    [Fact]
    public void GetBytes_Span_ReturnsCorrectBytes()
    {
        Encoding encoding = Encoding.UTF8;
        ReadOnlySpan<char> source = "Hello".AsSpan();
        Span<byte> destination = stackalloc byte[10];

        int bytesWritten = encoding.GetBytes(source, destination);

        bytesWritten.Should().Be(5);
        destination[..5].SequenceEqual("Hello"u8).Should().BeTrue();
    }

    [Fact]
    public void GetBytes_Span_EmptyInput_ReturnsZero()
    {
        Encoding encoding = Encoding.UTF8;
        ReadOnlySpan<char> source = ReadOnlySpan<char>.Empty;
        Span<byte> destination = stackalloc byte[10];

        int bytesWritten = encoding.GetBytes(source, destination);

        bytesWritten.Should().Be(0);
    }

    [Fact]
    public void GetBytes_Span_UnicodeCharacters()
    {
        Encoding encoding = Encoding.UTF8;
        ReadOnlySpan<char> source = "\u00E9".AsSpan(); // é
        Span<byte> destination = stackalloc byte[10];

        int bytesWritten = encoding.GetBytes(source, destination);

        bytesWritten.Should().Be(2); // é is 2 bytes in UTF-8
    }

    [Fact]
    public void GetBytes_Span_MatchesByteArrayOverload()
    {
        Encoding encoding = Encoding.UTF8;
        string text = "Hello, World! \u00E9\u00F1\u00FC";
        byte[] expected = encoding.GetBytes(text);

        Span<byte> destination = stackalloc byte[expected.Length];
        int bytesWritten = encoding.GetBytes(text.AsSpan(), destination);

        bytesWritten.Should().Be(expected.Length);
        destination.SequenceEqual(expected).Should().BeTrue();
    }

    [Fact]
    public void GetString_Span_ReturnsCorrectString()
    {
        Encoding encoding = Encoding.UTF8;
        ReadOnlySpan<byte> source = "Hello"u8;

        string result = encoding.GetString(source);

        result.Should().Be("Hello");
    }

    [Fact]
    public void GetString_Span_EmptyInput_ReturnsEmpty()
    {
        Encoding encoding = Encoding.UTF8;
        ReadOnlySpan<byte> source = [];

        string result = encoding.GetString(source);

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetString_Span_UnicodeBytes()
    {
        Encoding encoding = Encoding.UTF8;
        byte[] utf8Bytes = encoding.GetBytes("\u00E9"); // é
        ReadOnlySpan<byte> source = utf8Bytes;

        string result = encoding.GetString(source);

        result.Should().Be("\u00E9");
    }

    [Fact]
    public void GetString_Span_MatchesByteArrayOverload()
    {
        Encoding encoding = Encoding.UTF8;
        string original = "Hello, World! \u00E9\u00F1\u00FC";
        byte[] bytes = encoding.GetBytes(original);

        string fromSpan = encoding.GetString(bytes.AsSpan());
        string fromArray = encoding.GetString(bytes);

        fromSpan.Should().Be(fromArray);
        fromSpan.Should().Be(original);
    }

    [Fact]
    public void GetBytes_DifferentEncodings()
    {
        ReadOnlySpan<char> source = "Test".AsSpan();

        Span<byte> utf8Buffer = stackalloc byte[20];
        int utf8Written = Encoding.UTF8.GetBytes(source, utf8Buffer);

        Span<byte> asciiBuffer = stackalloc byte[20];
        int asciiWritten = Encoding.ASCII.GetBytes(source, asciiBuffer);

        // For simple ASCII text, both should be the same
        utf8Written.Should().Be(asciiWritten);
        utf8Buffer[..utf8Written].SequenceEqual(asciiBuffer[..asciiWritten]).Should().BeTrue();
    }
}
