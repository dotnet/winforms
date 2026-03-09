// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class ArgumentExceptionTests
{
    [Fact]
    public void ThrowIfNullOrEmpty_NonNullNonEmpty_DoesNotThrow()
    {
        string value = "hello";
        ArgumentException.ThrowIfNullOrEmpty(value);
    }

    [Fact]
    public void ThrowIfNullOrEmpty_Null_ThrowsArgumentNullException()
    {
        string? value = null;
        Action action = () => ArgumentException.ThrowIfNullOrEmpty(value);
        action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void ThrowIfNullOrEmpty_Empty_ThrowsArgumentException()
    {
        string value = string.Empty;
        Action action = () => ArgumentException.ThrowIfNullOrEmpty(value);
        action.Should().Throw<ArgumentException>().Which.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void ThrowIfNullOrEmpty_Whitespace_DoesNotThrow()
    {
        string value = "  ";
        ArgumentException.ThrowIfNullOrEmpty(value);
    }

    [Fact]
    public void ThrowIfNullOrWhiteSpace_NonNullNonWhiteSpace_DoesNotThrow()
    {
        string value = "hello";
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
    }

    [Fact]
    public void ThrowIfNullOrWhiteSpace_Null_ThrowsArgumentNullException()
    {
        string? value = null;
        Action action = () => ArgumentException.ThrowIfNullOrWhiteSpace(value);
        action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void ThrowIfNullOrWhiteSpace_Empty_ThrowsArgumentException()
    {
        string value = string.Empty;
        Action action = () => ArgumentException.ThrowIfNullOrWhiteSpace(value);
        action.Should().Throw<ArgumentException>().Which.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void ThrowIfNullOrWhiteSpace_Whitespace_ThrowsArgumentException()
    {
        string value = "   ";
        Action action = () => ArgumentException.ThrowIfNullOrWhiteSpace(value);
        action.Should().Throw<ArgumentException>().Which.ParamName.Should().Be(nameof(value));
    }

    [Theory]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void ThrowIfNullOrWhiteSpace_VariousWhitespace_ThrowsArgumentException(string value)
    {
        Action action = () => ArgumentException.ThrowIfNullOrWhiteSpace(value);
        action.Should().Throw<ArgumentException>();
    }
}
