// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Forms.Tests;

public class DataGridViewCellConverterTests
{
    private readonly DataGridViewCellConverter _converter = new();

    [WinFormsTheory]
    [InlineData(typeof(InstanceDescriptor), true)]
    [InlineData(typeof(string), true)]
    public void CanConvertTo_ReturnsExpected(Type destinationType, bool expected) =>
        _converter.CanConvertTo(context: null, destinationType).Should().Be(expected);

    [WinFormsFact]
    public void ConvertTo_InstanceDescriptorWithDataGridViewCell_ReturnsInstanceDescriptor()
    {
        using DataGridViewTextBoxCell cell = new();
        Type destinationType = typeof(InstanceDescriptor);

        _converter.ConvertTo(context: null, CultureInfo.InvariantCulture, cell, destinationType).Should().NotBeNull();
        _converter.ConvertTo(context: null, CultureInfo.InvariantCulture, cell, destinationType).Should().BeOfType<InstanceDescriptor>();
    }

    [WinFormsFact]
    public void ConvertTo_InstanceDescriptorWithNonDataGridViewCell_ThrowsNotSupportedException()
    {
        object value = new();
        Type destinationType = typeof(InstanceDescriptor);

        Action action = () => _converter.ConvertTo(context: null, CultureInfo.InvariantCulture, value, destinationType);
        action.Should().Throw<NotSupportedException>()
            .WithMessage("'DataGridViewCellConverter' is unable to convert 'System.Object' to 'System.ComponentModel.Design.Serialization.InstanceDescriptor'.");
    }

    [WinFormsFact]
    public void ConvertTo_OtherType_ReturnsStringRepresentation()
    {
        using DataGridViewTextBoxCell cell = new();
        Type destinationType = typeof(string);

        _converter.ConvertTo(context: null, CultureInfo.InvariantCulture, cell, destinationType).Should().NotBeNull();
        _converter.ConvertTo(context: null, CultureInfo.InvariantCulture, cell, destinationType).Should().BeOfType<string>();
        _converter.ConvertTo(context: null, CultureInfo.InvariantCulture, cell, destinationType).Should().Be(cell.ToString());
    }
}
