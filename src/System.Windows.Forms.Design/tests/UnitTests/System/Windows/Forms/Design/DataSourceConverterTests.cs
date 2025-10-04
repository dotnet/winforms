// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public class DataSourceConverterTests
{
    private readonly DataSourceConverter _converter = new();

    [Fact]
    public void ConvertTo_NullValueToString_ReturnsNoneLowercase() =>
        (_converter.ConvertTo(context: null, culture: null, value: null, destinationType: typeof(string)) as string).Should().Be("(none)");

    [Fact]
    public void ConvertTo_NonNullValueToString_CallsBase() =>
        _converter.ConvertTo(context: null, culture: null, value: new(), destinationType: typeof(string)).Should().NotBe("(none)");

    [Fact]
    public void ConvertTo_NullValueToNonString_ThrowsNotSupportedException()
    {
        Action act = () => _converter.ConvertTo(context: null, culture: null, value: null, destinationType: typeof(int));

        act.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [InlineData("de-DE")]
    public void ConvertTo_NullValueWithDifferentCultures_ReturnsNoneLowercase(string cultureName) =>
        (_converter.ConvertTo(context: null, culture: new(cultureName), value: null, destinationType: typeof(string)) as string).Should().Be("(none)");
}
