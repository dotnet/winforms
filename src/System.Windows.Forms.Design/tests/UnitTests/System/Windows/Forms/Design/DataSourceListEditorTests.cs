// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class DataSourceListEditorTests
{
    private readonly DataSourceListEditor _dataSourceListEditor;

    public DataSourceListEditorTests()
    {
        _dataSourceListEditor = new();
    }

    [Fact]
    public void IsDropDownResizable_ReturnsTrue() =>
        _dataSourceListEditor.IsDropDownResizable.Should().BeTrue();

    [Fact]
    public void EditValue_WithValidParameters_ReturnsNewValue()
    {
        Mock<ITypeDescriptorContext> contextMock = new();
        Mock<IServiceProvider> providerMock = new();

        BindingSource oldValue = new() { DataSource = new List<string> { "OldValue" } };
        BindingSource newValue = new() { DataSource = new List<string> { "NewValue" } };
        DesignBinding designBinding = new(oldValue, "");

        contextMock.Setup(c => c.Instance).Returns(new object());

        TestDesignBindingPicker designBindingPicker = new()
        {
            PickFunc = (context, provider, showDataSources, showDataMembers, selectListMembers, rootDataSource, rootDataMember, initialSelectedItem) =>
            designBinding
        };

        var accessor = _dataSourceListEditor.TestAccessor().Dynamic;
        accessor._designBindingPicker = designBindingPicker;

        _dataSourceListEditor.EditValue(context: contextMock.Object, provider: providerMock.Object, value: newValue).Should().Be(newValue);
    }

    [Fact]
    public void EditValue_WithNullProvider_ReturnsOriginalValue()
    {
        Mock<ITypeDescriptorContext> contextMock = new();
        object oldValue = new();

        contextMock.Setup(c => c.Instance).Returns(new object());

        _dataSourceListEditor.EditValue(context: contextMock.Object, provider: null!, value: oldValue).Should().Be(oldValue);
    }

    [Fact]
    public void EditValue_WithNullContext_ReturnsOriginalValue()
    {
        Mock<IServiceProvider> providerMock = new();
        object oldValue = new();

        _dataSourceListEditor.EditValue(context: null, provider: providerMock.Object, value: oldValue).Should().Be(oldValue);
    }

    [Fact]
    public void GetEditStyle_ReturnsDropDown() =>
     _dataSourceListEditor.GetEditStyle(null).Should().Be(UITypeEditorEditStyle.DropDown);

    private class TestDesignBindingPicker : DesignBindingPicker
    {
        public Func<ITypeDescriptorContext?, IServiceProvider, bool, bool, bool, object?, string, DesignBinding, DesignBinding?>? PickFunc { get; set; }

        public DesignBinding? PickWrapper(ITypeDescriptorContext? context, IServiceProvider provider, bool showDataSources, bool showDataMembers, bool selectListMembers, object? rootDataSource, string rootDataMember, DesignBinding initialSelectedItem)
        {
            return PickFunc?.Invoke(context, provider, showDataSources, showDataMembers, selectListMembers, rootDataSource, rootDataMember, initialSelectedItem);
        }
    }
}
