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
        var editor = new ListControlStringCollectionEditor(typeof(string));
        var provider = new Mock<IServiceProvider>().Object;
        object? value = new();

        object? result = editor.EditValue(null, provider, value);

        result.Should().Be(value);
    }

    [Fact]
    public void EditValue_WithContextInstanceNotListControl_ReturnsBaseEditValue()
    {
        var editor = new ListControlStringCollectionEditor(typeof(string));
        Mock<ITypeDescriptorContext> context = new();
        context.Setup(c => c.Instance).Returns(new object());
        var provider = new Mock<IServiceProvider>().Object;
        object? value = new();

        object? result = editor.EditValue(context.Object, provider, value);

        result.Should().Be(value);
    }

    [Fact]
    public void EditValue_WithListControlAndNullDataSource_ReturnsBaseEditValue()
    {
        var editor = new ListControlStringCollectionEditor(typeof(string));
        ListBox listControl = new();
        Mock<ITypeDescriptorContext> context = new();
        context.Setup(c => c.Instance).Returns(listControl);
        var provider = new Mock<IServiceProvider>().Object;
        object? value = new();

        object? result = editor.EditValue(context.Object, provider, value);

        result.Should().Be(value);
    }

    [Fact]
    public void EditValue_WithListControlAndNonNullDataSource_ThrowsArgumentException()
    {
        var editor = new ListControlStringCollectionEditor(typeof(string));

        ListBox listControl = new() { DataSource = new List<string> { "item1", "item2", "item3" } };

        Mock<ITypeDescriptorContext> context = new();
        context.Setup(c => c.Instance).Returns(listControl);

        var provider = new Mock<IServiceProvider>().Object;
        object? value = new();

        ArgumentException exception = Assert.Throws<ArgumentException>(() => editor.EditValue(context.Object, provider, value));
        exception.Message.Should().Be(SR.DataSourceLocksItems);
    }
}
