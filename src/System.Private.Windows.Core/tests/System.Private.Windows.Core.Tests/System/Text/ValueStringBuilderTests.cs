// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Text;

public class ValueStringBuilderTests
{
    [Fact]
    public void Append_ShouldAppendSingleString()
    {
        using ValueStringBuilder builder = new(10);
        builder.Append("Hello");
        builder.ToString().Should().Be("Hello");
    }

    [Fact]
    public void Append_ShouldAppendMultipleStrings()
    {
        using ValueStringBuilder builder = new(10);
        builder.Append("Hello");
        builder.Append(", ");
        builder.Append("world!");
        builder.ToString().Should().Be("Hello, world!");
    }

    [Fact]
    public void Append_Char_ShouldAppendCharacters()
    {
        using ValueStringBuilder builder = new(10);
        builder.Append('A');
        builder.Append('B');
        builder.Append('C');
        builder.ToString().Should().Be("ABC");
    }

    [Fact]
    public void Insert_ShouldInsertStringAtIndex()
    {
        using ValueStringBuilder builder = new(10);
        builder.Append("Hello world!");
        builder.Insert(6, "beautiful ");
        builder.ToString().Should().Be("Hello beautiful world!");
    }

    [Fact]
    public void Length_ShouldReturnCorrectValue()
    {
        using ValueStringBuilder builder = new(10);
        builder.Append("12345");
        builder.Length.Should().Be(5);
    }

    [Fact]
    public void Capacity_ShouldIncreaseWhenExceeded()
    {
        using ValueStringBuilder builder = new(10);
        builder.Append("This is a long string that exceeds the initial capacity.");
        builder.Capacity.Should().BeGreaterThan(10);
        builder.ToString().Should().Be("This is a long string that exceeds the initial capacity.");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public void AsHandler_Int(int value)
    {
        string result = TestFormat($"Hello, {value}!");
        result.Should().Be($"Hello, {value}!");
    }

    [Theory]
    [InlineData(DayOfWeek.Monday)]
    [InlineData(DayOfWeek.Friday)]
    public void AsHandler_Enum(DayOfWeek value)
    {
        string result = TestFormat($"Hello, it's {value}!");
        result.Should().Be($"Hello, it's {value}!");
    }

    private static string TestFormat(ref ValueStringBuilder builder) => builder.ToStringAndClear();
}
