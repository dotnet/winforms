// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class DataGridViewRowConverterTests
{
    [Fact]
    public void CanConvertTo_returns_expected()
    {
        using DataGridViewRow row = new();
        TypeConverter converter = TypeDescriptor.GetConverter(row);

        Assert.True(converter.CanConvertTo(typeof(InstanceDescriptor)));
    }

    [Fact]
    public void ConvertTo_returns_InstanceDescriptor()
    {
        using DataGridViewRow row = new();

        TypeConverter converter = TypeDescriptor.GetConverter(row);
        object descriptor = converter.ConvertTo(row, typeof(InstanceDescriptor));

        Assert.NotNull(descriptor);
        Assert.IsType<InstanceDescriptor>(descriptor);
    }
}
