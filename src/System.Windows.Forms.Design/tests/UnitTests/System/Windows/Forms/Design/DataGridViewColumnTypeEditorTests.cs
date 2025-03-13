// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class DataGridViewColumnTypeEditorTests
{
    private readonly DataGridViewColumnTypeEditor _editor;

    public DataGridViewColumnTypeEditorTests()
    {
        _editor = new();
    }

    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        _editor.Should().NotBeNull();
        _editor.IsDropDownResizable.Should().BeTrue();
    }

    [Fact]
    public void IsDropDownResizable_ShouldReturnTrue() =>
        _editor.IsDropDownResizable.Should().BeTrue();

    [Fact]
    public void GetEditStyle_ShouldReturnDropDown() =>
        _editor.GetEditStyle(null).Should().Be(UITypeEditorEditStyle.DropDown);

    [Fact]
    public void EditValue_ShouldReturnOriginalValue_WhenNoServiceProvider() =>
        _editor.EditValue(null, null!, typeof(DataGridViewTextBoxColumn)).Should().Be(typeof(DataGridViewTextBoxColumn));

    [Fact]
    public void EditValue_ShouldReturnOriginalValue_WhenNoEditorService()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IWindowsFormsEditorService))).Returns(null!);

        _editor.EditValue(null, serviceProviderMock.Object, typeof(DataGridViewTextBoxColumn)).Should().Be(typeof(DataGridViewTextBoxColumn));
    }

    [Fact]
    public void EditValue_ShouldReturnOriginalValue_WhenNoContextInstance()
    {
        Mock<IServiceProvider> serviceProviderMock = new();
        Mock<IWindowsFormsEditorService> editorServiceMock = new();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(IWindowsFormsEditorService))).Returns(editorServiceMock.Object);

        _editor.EditValue(null, serviceProviderMock.Object, typeof(DataGridViewTextBoxColumn)).Should().Be(typeof(DataGridViewTextBoxColumn));
    }

    [Fact]
    public void EditValue_ShouldReturnOriginalValue_WhenSelectedTypeIsNull()
    {
        Mock<IServiceProvider> mockProvider = new();
        Mock<IWindowsFormsEditorService> mockEditorService = new();
        Mock<ITypeDescriptorContext> mockContext = new();
        Mock<IDesignerHost> mockDesignerHost = new();
        Mock<ITypeDiscoveryService> mockDiscoveryService = new();

        mockProvider.Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                    .Returns(mockEditorService.Object);
        mockProvider.Setup(p => p.GetService(typeof(IDesignerHost)))
                    .Returns(mockDesignerHost.Object);
        mockDesignerHost.Setup(d => d.GetService(typeof(ITypeDiscoveryService)))
                        .Returns(mockDiscoveryService.Object);

        ArrayList mockTypeCollection = [typeof(DataGridViewTextBoxColumn), typeof(DataGridViewCheckBoxColumn)];
        mockDiscoveryService.Setup(d => d.GetTypes(typeof(DataGridViewColumn), false))
                            .Returns(mockTypeCollection);

        DataGridViewTextBoxColumn column = new();
        SubListBoxItem listBoxItem = new(column);
        mockContext.Setup(c => c.Instance).Returns(listBoxItem);

        object? result = _editor.EditValue(mockContext.Object, mockProvider.Object, null);

        mockEditorService.Verify(s => s.DropDownControl(It.IsAny<Control>()), Times.Once);
        result.Should().Be(null);
    }

    private class SubListBoxItem : DataGridViewColumnCollectionDialog.ListBoxItem
    {
        public SubListBoxItem(DataGridViewColumn column) : base(column, null!, null!) { }
    }
}
