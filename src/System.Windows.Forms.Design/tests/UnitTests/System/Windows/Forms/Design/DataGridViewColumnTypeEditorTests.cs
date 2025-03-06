// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
}
