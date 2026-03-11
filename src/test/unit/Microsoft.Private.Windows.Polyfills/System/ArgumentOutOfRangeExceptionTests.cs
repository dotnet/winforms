// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class ArgumentOutOfRangeExceptionTests
{
    [Fact]
    public void ThrowIfNegative_Zero_DoesNotThrow()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(0);
    }

    [Fact]
    public void ThrowIfNegative_Positive_DoesNotThrow()
    {
        ArgumentOutOfRangeException.ThrowIfNegative(42);
    }

    [Fact]
    public void ThrowIfNegative_Negative_ThrowsArgumentOutOfRangeException()
    {
        int value = -1;
        Action action = () => ArgumentOutOfRangeException.ThrowIfNegative(value);
        action.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void ThrowIfNegative_MinValue_ThrowsArgumentOutOfRangeException()
    {
        int value = int.MinValue;
        Action action = () => ArgumentOutOfRangeException.ThrowIfNegative(value);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ThrowIfLessThan_Equal_DoesNotThrow()
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(5, 5);
    }

    [Fact]
    public void ThrowIfLessThan_Greater_DoesNotThrow()
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(10, 5);
    }

    [Fact]
    public void ThrowIfLessThan_Less_ThrowsArgumentOutOfRangeException()
    {
        int value = 3;
        Action action = () => ArgumentOutOfRangeException.ThrowIfLessThan(value, 5);
        action.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void ThrowIfLessThan_Double_Less_ThrowsArgumentOutOfRangeException()
    {
        double value = 1.5;
        Action action = () => ArgumentOutOfRangeException.ThrowIfLessThan(value, 2.0);
        action.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void ThrowIfLessThan_Double_Equal_DoesNotThrow()
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(2.0, 2.0);
    }

    [Fact]
    public void ThrowIfLessThan_String_Less_ThrowsArgumentOutOfRangeException()
    {
        string value = "a";
        Action action = () => ArgumentOutOfRangeException.ThrowIfLessThan(value, "b");
        action.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be(nameof(value));
    }
}
