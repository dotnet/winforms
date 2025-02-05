// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.Tests;

public class FlatButtonAppearanceConverterTests
{
    private readonly FlatButtonAppearanceConverter _converter = new();

    [Fact]
    public void ConvertTo_NullContextOrCulture_ReturnsEmptyString()
    {
        var resultWithNullContext = ConvertToHelper(context: null, culture: null, value: null, destinationType: typeof(string));
        var resultWithNullCulture = ConvertToHelper(context: null, culture: null, value: null, destinationType: typeof(string));

        resultWithNullContext.Should().Be("");
        resultWithNullCulture.Should().Be("");
    }

    [Fact]
    public void ConvertTo_InvalidDestinationType_ThrowsException()
    {
        Action action1 = () => ConvertToHelper(context: null, culture: null, value: null, destinationType: null!);
        Action action2 = () => ConvertToHelper(context: null, culture: null, value: null, destinationType: typeof(int));

        action1.Should().Throw<ArgumentNullException>();
        action2.Should().Throw<NotSupportedException>();
    }

    private object ConvertToHelper(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        return _converter.ConvertTo(context: context, culture: culture, value: value, destinationType: destinationType);
    }
}
