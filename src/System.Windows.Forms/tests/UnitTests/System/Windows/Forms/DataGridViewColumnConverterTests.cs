// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Forms.Tests;

public class DataGridViewColumnConverterTests : IDisposable
{
    private readonly DataGridViewColumnConverter _converter = new();
    private readonly DataGridViewColumn _column = new();

    public void Dispose() => _column.Dispose();

    [WinFormsTheory]
    [InlineData(typeof(InstanceDescriptor))]
    [InlineData(typeof(string))]
    public void CanConvertTo_ReturnsExpectedResult(Type destinationType) =>
        _converter.CanConvertTo(null, destinationType).Should().BeTrue();

    [WinFormsFact]
    public void ConvertTo_InstanceDescriptorWithoutCellType_ReturnsInstanceDescriptor()
    {
        var descriptor = _converter.ConvertTo(null, CultureInfo.InvariantCulture, _column, typeof(InstanceDescriptor)) as InstanceDescriptor;

        descriptor.Should().NotBeNull();
    }

    [WinFormsFact]
    public void ConvertTo_InstanceDescriptorWithHeaderText_ReturnsInstanceDescriptor()
    {
        _column.HeaderText = "Test Header";
        var descriptor = _converter.ConvertTo(null, CultureInfo.InvariantCulture, _column, typeof(InstanceDescriptor)) as InstanceDescriptor;

        descriptor.Should().NotBeNull();
    }

    [WinFormsFact]
    public void ConvertTo_InstanceDescriptorWithToolTipText_ReturnsInstanceDescriptor()
    {
        _column.ToolTipText = "Test ToolTip";
        var descriptor = _converter.ConvertTo(null, CultureInfo.InvariantCulture, _column, typeof(InstanceDescriptor)) as InstanceDescriptor;

        descriptor.Should().NotBeNull();
    }

    [WinFormsFact]
    public void ConvertTo_NonInstanceDescriptorWithDataPropertyName_ReturnsBaseResult()
    {
        _column.DataPropertyName = "TestDataProperty";
        _converter.ConvertTo(null, CultureInfo.InvariantCulture, _column, typeof(string)).Should().Be(_column.ToString());
    }

    [WinFormsFact]
    public void ConvertTo_NonInstanceDescriptor_ReturnsBaseResult() =>
        _converter.ConvertTo(null, CultureInfo.InvariantCulture, _column, typeof(string)).Should().Be(_column.ToString());

    [WinFormsFact]
    public void ConvertTo_NullDestinationType_ThrowsArgumentNullException()
    {
        Action action = () => _converter.ConvertTo(null, CultureInfo.InvariantCulture, _column, null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [WinFormsFact]
    public void ConvertTo_InvalidDestinationType_ThrowsNotSupportedException()
    {
        Action action = () => _converter.ConvertTo(null, CultureInfo.InvariantCulture, _column, typeof(int));

        action.Should().Throw<NotSupportedException>();
    }
}
