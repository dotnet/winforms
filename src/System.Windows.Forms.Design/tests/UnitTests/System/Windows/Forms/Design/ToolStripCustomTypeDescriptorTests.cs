// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripCustomTypeDescriptorTests
{
    private readonly ToolStrip _toolStrip;
    private readonly ToolStripCustomTypeDescriptor _descriptor;

    public ToolStripCustomTypeDescriptorTests()
    {
        _toolStrip = new();
        _descriptor = new(_toolStrip);
    }

    [Fact]
    public void Constructor_InitializesInstance() =>
        _descriptor.GetPropertyOwner(null).Should().Be(_toolStrip);

    [Fact]
    public void GetPropertyOwner_ReturnsInstance() =>
        _descriptor.GetPropertyOwner(null).Should().Be(_toolStrip);

    [Fact]
    public void GetProperties_ReturnsCachedCollection()
    {
        PropertyDescriptorCollection properties1 = _descriptor.GetProperties();
        PropertyDescriptorCollection properties2 = _descriptor.GetProperties();

        properties1.Should().BeSameAs(properties2);
    }

    [Fact]
    public void GetProperties_WithAttributes_ReturnsCachedCollection()
    {
        Attribute[] attributes = [new BrowsableAttribute(true)];

        PropertyDescriptorCollection properties1 = _descriptor.GetProperties(attributes);
        PropertyDescriptorCollection properties2 = _descriptor.GetProperties(attributes);

        properties1.Should().BeSameAs(properties2);
    }

    [Fact]
    public void GetProperties_RemovesItemsProperty() =>
        _descriptor.GetProperties().Cast<PropertyDescriptor>().Should().NotContain(p => p.Name == "Items");

    [Fact]
    public void GetProperties_WithAttributes_RemovesItemsProperty()
    {
        Attribute[] attributes = [new BrowsableAttribute(true)];

        _descriptor.GetProperties(attributes).Cast<PropertyDescriptor>().Should().NotContain(p => p.Name == "Items");
    }

    [Fact]
    public void GetProperties_RemovesItemsProperty_WhenCollectionIsNotNull()
    {
        _descriptor.GetProperties();
        _descriptor.GetProperties().Cast<PropertyDescriptor>().Should().NotContain(p => p.Name == "Items");
    }

    [Fact]
    public void GetProperties_WithAttributes_RemovesItemsProperty_WhenCollectionIsNotNull()
    {
        Attribute[] attributes = [new BrowsableAttribute(true)];
        _descriptor.GetProperties(attributes);
        _descriptor.GetProperties(attributes).Cast<PropertyDescriptor>().Should().NotContain(p => p.Name == "Items");
    }
}
