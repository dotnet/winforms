// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class DataGridViewCellStyleBuilderTests
{
    [Fact]
    public void CellStyle_SetAndGet_ReturnsCorrectValue()
    {
        Mock<IServiceProvider> serviceProvider = new();
        Mock<IComponent> component = new();
        using DataGridViewCellStyleBuilder builder = new(serviceProvider.Object, component.Object);

        builder.CellStyle = new DataGridViewCellStyle { BackColor = Color.Red };
        var result = builder.CellStyle;

        result.Should().NotBeNull();
        result.BackColor.Should().Be(Color.Red);
    }

    [Fact]
    public void Context_Set_ReturnsCorrectValue()
    {
        Mock<IServiceProvider> serviceProvider = new();
        Mock<IComponent> component = new();
        using DataGridViewCellStyleBuilder builder = new(serviceProvider.Object, component.Object);
        Mock<ITypeDescriptorContext> context = new();

        builder.Context = context.Object;
        ITypeDescriptorContext result = builder.TestAccessor().Dynamic._context;

        result.Should().Be(context.Object);
    }
}
