// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class InheritedPropertyDescriptorTests
{
    [Fact]
    public void GetValue_ReturnsExpected()
    {
        using Control control = new();
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
        PropertyDescriptor propertyDescriptor = properties.Find(nameof(Control.Size), false);
        InheritedPropertyDescriptor inheritedPropertyDescriptor = new(propertyDescriptor, control);

        Size size = (Size)inheritedPropertyDescriptor.GetValue(control);
        size.Height.Should().Be(0);

        control.Size = new Size(0, 120);
        size = (Size)inheritedPropertyDescriptor.GetValue(control);
        size.Height.Should().Be(120);
    }

    [Fact]
    public void ResetValue_ReturnsExpected()
    {
        using Control control = new();
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
        PropertyDescriptor propertyDescriptor = properties.Find(nameof(Control.Anchor), false);
        InheritedPropertyDescriptor inheritedPropertyDescriptor = new(propertyDescriptor, control);

        AnchorStyles anchor = (AnchorStyles)inheritedPropertyDescriptor.GetValue(control);
        anchor.Should().Be(AnchorStyles.Top | AnchorStyles.Left);

        control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
        anchor = (AnchorStyles)inheritedPropertyDescriptor.GetValue(control);
        anchor.Should().Be(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom);

        inheritedPropertyDescriptor.ResetValue(control);
        anchor = (AnchorStyles)inheritedPropertyDescriptor.GetValue(control);
        anchor.Should().Be(AnchorStyles.Top | AnchorStyles.Left);
    }

    [Fact]
    public void SetValue_ReturnsExpected()
    {
        using Control control = new();
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
        PropertyDescriptor propertyDescriptor = properties.Find(nameof(Control.BackColor), false);
        InheritedPropertyDescriptor inheritedPropertyDescriptor = new(propertyDescriptor, control);

        Color backColor = (Color)inheritedPropertyDescriptor.GetValue(control);
        backColor.Name.Should().Be("Control");

        inheritedPropertyDescriptor.SetValue(control, Color.Purple);
        backColor = (Color)inheritedPropertyDescriptor.GetValue(control);
        backColor.Name.Should().Be("Purple");
    }

    [Fact]
    public void ShouldSerializeValue_ReturnsExpected()
    {
        using Control control = new();
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(control);
        PropertyDescriptor propertyDescriptor = properties.Find(nameof(Control.Visible), false);
        InheritedPropertyDescriptor inheritedPropertyDescriptor = new(propertyDescriptor, control);

        inheritedPropertyDescriptor.ShouldSerializeValue(control).Should().BeFalse();

        control.Visible = !control.Visible;
        inheritedPropertyDescriptor.ShouldSerializeValue(control).Should().BeTrue();
    }
}
