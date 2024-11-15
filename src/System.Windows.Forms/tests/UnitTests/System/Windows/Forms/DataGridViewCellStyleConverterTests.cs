// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Forms.Tests;

public class DataGridViewCellStyleConverterTests
{
    private readonly DataGridViewCellStyleConverter _converter;
    private readonly DataGridViewCellStyle _style;

    public DataGridViewCellStyleConverterTests()
    {
        _converter = new();
        _style = new();
    }

    [WinFormsFact]
    public void CanConvertTo_InstanceDescriptor_ReturnsTrue()
    {
        _converter.CanConvertTo(null, typeof(InstanceDescriptor)).Should().BeTrue();
    }

    [WinFormsFact]
    public void CanConvertTo_OtherType_ReturnsFalse()
    {
        _converter.CanConvertTo(null, typeof(int)).Should().BeFalse();
    }

    [WinFormsFact]
    public void ConvertTo_InstanceDescriptor_ReturnsInstanceDescriptor()
    {
        object? convertTo = _converter.ConvertTo(null, CultureInfo.InvariantCulture, _style, typeof(InstanceDescriptor));
        convertTo.Should().NotBeNull();
        convertTo.Should().BeOfType<InstanceDescriptor>();
    }

    [WinFormsFact]
    public void ConvertTo_UnsupportedType_ThrowsNotSupportedException()
    {
        Action action = () => _converter.ConvertTo(null, CultureInfo.InvariantCulture, _style, typeof(int));
        action.Should().Throw<NotSupportedException>();
    }
}
