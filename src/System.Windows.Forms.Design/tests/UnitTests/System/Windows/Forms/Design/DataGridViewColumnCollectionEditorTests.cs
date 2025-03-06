// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Moq;
using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design.Tests;

public class DataGridViewColumnCollectionEditorTests
{
    [Fact]
    public void DataGridViewColumnCollectionEditor_GetEditStyle() =>
        new DataGridViewColumnCollectionEditor().GetEditStyle().Should().Be(UITypeEditorEditStyle.Modal);

    [Fact]
    public void DataGridViewColumnCollectionEditor_IsDropDownResizable() =>
        new DataGridViewColumnCollectionEditor().IsDropDownResizable.Should().Be(false);

    [Fact]
    public void DataGridViewColumnCollectionEditor_EditValue()
    {
        DataGridViewColumnCollectionEditor dataGridViewColumnCollectionEditor = new();
        object value = "123";
        dataGridViewColumnCollectionEditor.EditValue(null, null!, value).Should().Be(value);

        Mock<ITypeDescriptorContext> mockTypeDescriptorContext = new(MockBehavior.Strict);
        dataGridViewColumnCollectionEditor.EditValue(mockTypeDescriptorContext.Object, null!, value).Should().Be(value);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider.Setup(x => x.GetService(typeof(IWindowsFormsEditorService))).Returns(null!);
        dataGridViewColumnCollectionEditor.EditValue(null, mockServiceProvider.Object, value).Should().Be(value);

        mockTypeDescriptorContext.Setup(x => x.Instance).Returns(null!);
        dataGridViewColumnCollectionEditor.EditValue(null, mockServiceProvider.Object, value).Should().Be(value);
    }
}
