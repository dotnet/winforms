// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Tests;

public class FlatButtonAppearanceConverterTests
{
    private readonly FlatButtonAppearanceConverter _converter = new();

    [Fact]
    public void ConvertTo_NullContextOrCulture_ReturnsEmptyString()
    {
        string resultWithNullContext = (string)_converter.ConvertTo(context: null, culture: null, value: null, destinationType: typeof(string));
        string resultWithNullCulture = (string)_converter.ConvertTo(context: null, culture: null, value: null, destinationType: typeof(string));

        resultWithNullContext.Should().Be("");
        resultWithNullCulture.Should().Be("");
    }

    [Fact]
    public void ConvertTo_InvalidDestinationType_ThrowsException()
    {
        Action action1 = () => _converter.ConvertTo(context: null, culture: null, value: null, destinationType: null!);
        Action action2 = () => _converter.ConvertTo(context: null, culture: null, value: null, destinationType: typeof(int));

        action1.Should().Throw<ArgumentNullException>();
        action2.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void GetsProperties_WithNullValues()
    {
        using Button button = new();

        Action action = () => _converter.GetProperties(context: null, value: button, attributes: null);

        action.Should().NotThrow();
    }

    [Fact]
    public void GetProperties_ContextNull_ReturnsProperties()
    {
        using Button button = new();
        Attribute[] attributes = [new BrowsableAttribute(true)];

        PropertyDescriptorCollection properties = _converter.GetProperties(context: null, button, attributes);

        properties.Should().NotBeNull();
    }

    [Fact]
    public void GetProperties_AttributesNull_ReturnsProperties()
    {
        using Button button = new();
        Mock<ITypeDescriptorContext> contextMock = new();
        contextMock.Setup(c => c.Instance).Returns(button);

        PropertyDescriptorCollection properties = _converter.GetProperties(contextMock.Object, button, attributes: null);

        properties.Should().NotBeNull();
    }
}
