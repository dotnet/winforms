// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public class ListControlStringCollectionEditorTests
{
    [Fact]
    public void EditValue_WithNullContext_ReturnsBaseEditValue()
    {
        ListControlStringCollectionEditor editor = new(typeof(string));
        IServiceProvider provider = new Mock<IServiceProvider>().Object;
        object? value = new();

        object? result = editor.EditValue(null, provider, value);

        result.Should().Be(value);
    }

    [Fact]
    public void EditValue_WithContextInstanceNotListControl_ReturnsBaseEditValue()
    {
        ListControlStringCollectionEditor editor = new(typeof(string));
        Mock<ITypeDescriptorContext> context = new();
        context.Setup(c => c.Instance).Returns(new object());
        IServiceProvider provider = new Mock<IServiceProvider>().Object;
        object? value = new();

        object? result = editor.EditValue(context.Object, provider, value);

        result.Should().Be(value);
    }

    [Fact]
    public void EditValue_WithListControlAndNullDataSource_ReturnsBaseEditValue()
    {
        ListControlStringCollectionEditor editor = new(typeof(string));
        using ListBox listControl = new();
        Mock<ITypeDescriptorContext> context = new();
        context.Setup(c => c.Instance).Returns(listControl);
        IServiceProvider provider = new Mock<IServiceProvider>().Object;
        object? value = new();

        object? result = editor.EditValue(context.Object, provider, value);

        result.Should().Be(value);
    }

    [Fact]
    public void EditValue_WithListControlAndNonNullDataSource_ThrowsArgumentException()
    {
        ListControlStringCollectionEditor editor = new(typeof(string));

        using ListBox listControl = new() { DataSource = new List<string> { "item1", "item2", "item3" } };

        Mock<ITypeDescriptorContext> context = new();
        context.Setup(c => c.Instance).Returns(listControl);

        IServiceProvider provider = new Mock<IServiceProvider>().Object;
        object? value = new();

        ArgumentException exception = ((Action)(() => editor.EditValue(context.Object, provider, value))).Should().Throw<ArgumentException>().Which;
        exception.Message.Should().Be(SR.DataSourceLocksItems);
    }
}
