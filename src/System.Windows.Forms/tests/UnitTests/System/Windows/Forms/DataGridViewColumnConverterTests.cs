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

    [Theory]
    [InlineData(typeof(InstanceDescriptor))]
    [InlineData(typeof(string))]
    public void CanConvertTo_ReturnsExpectedResult(Type destinationType) =>
        _converter.CanConvertTo(context: null, destinationType).Should().BeTrue();

    [Fact]
    public void ConvertTo_InstanceDescriptorWithoutCellType_ReturnsInstanceDescriptor() =>
        AssertInstanceDescriptor(_column);

    [Fact]
    public void ConvertTo_InstanceDescriptorWithHeaderText_ReturnsInstanceDescriptor()
    {
        _column.HeaderText = "Test Header";
        AssertInstanceDescriptor(_column);
    }

    [Fact]
    public void ConvertTo_InstanceDescriptorWithToolTipText_ReturnsInstanceDescriptor()
    {
        _column.ToolTipText = "Test ToolTip";
        AssertInstanceDescriptor(_column);
    }

    [Fact]
    public void ConvertTo_NonInstanceDescriptorWithDataPropertyName_ReturnsBaseResult()
    {
        _column.DataPropertyName = "TestDataProperty";
        AssertBaseResult(_column);
    }

    [Fact]
    public void ConvertTo_NonInstanceDescriptor_ReturnsBaseResult() =>
        AssertBaseResult(_column);

    [Fact]
    public void ConvertTo_NullDestinationType_ThrowsArgumentNullException()
    {
        Action action = () => _converter.ConvertTo(context: null, culture: CultureInfo.InvariantCulture, value: _column, destinationType: null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ConvertTo_InvalidDestinationType_ThrowsNotSupportedException()
    {
        Action action = () => _converter.ConvertTo(context: null, culture: CultureInfo.InvariantCulture, value: _column, destinationType: typeof(int));
        action.Should().Throw<NotSupportedException>();
    }

    [Fact]
    public void ConvertTo_WithCellTypeNotNull_ReturnsInstanceDescriptor()
    {
        using DataGridViewTextBoxCell cellTemplate = new();
        using DataGridViewColumn column = new(cellTemplate);

        var descriptor = _converter.ConvertTo(context: null, culture: CultureInfo.InvariantCulture, value: column, destinationType: typeof(InstanceDescriptor)) as InstanceDescriptor;

        descriptor.Should().NotBeNull();
    }

    private void AssertInstanceDescriptor(DataGridViewColumn column)
    {
        var descriptor = _converter.ConvertTo(context: null, culture: CultureInfo.InvariantCulture, value: column, destinationType: typeof(InstanceDescriptor)) as InstanceDescriptor;
        descriptor.Should().NotBeNull();
    }

    private void AssertBaseResult(DataGridViewColumn column)
    {
        _converter.ConvertTo(context: null, culture: CultureInfo.InvariantCulture, value: column, destinationType: typeof(string)).Should().Be(column.ToString());
    }
}
