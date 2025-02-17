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
    public void GetProperties_RemovesItemsProperty() =>
        _descriptor.GetProperties().Cast<PropertyDescriptor>().Should().NotContain(p => p.Name == "Items");

    [Fact]
    public void GetProperties_WithAttributes_RemovesItemsProperty()
    {
        Attribute[] attributes = [new BrowsableAttribute(true)];

        _descriptor.GetProperties(attributes).Cast<PropertyDescriptor>().Should().NotContain(p => p.Name == "Items");
    }
}
