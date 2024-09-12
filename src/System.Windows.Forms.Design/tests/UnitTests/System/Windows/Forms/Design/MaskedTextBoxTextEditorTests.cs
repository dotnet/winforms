// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using Moq;

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class MaskedTextBoxTextEditorTests
{
    [Fact]
    public void EditValue_ReturnsOriginalValue_WhenContextInstanceIsNull()
    {
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.Instance)
            .Returns((object?)null);

        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
            .Setup(s => s.ShowDialog(It.IsAny<Form>()))
            .Returns(DialogResult.OK);

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        MaskedTextBoxTextEditor editor = new();
        object? value = "TestValue";

        value.Should().BeSameAs(editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
    }

    [WinFormsFact]
    public void EditValue_ReturnsOriginalValue_WhenEditorServiceIsNotAvailable()
    {
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.Instance)
            .Returns(new object());

        IWindowsFormsEditorService? editorService = null;

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(editorService);

        MaskedTextBoxTextEditor editor = new();
        object? value = "TestValue";

        value.Should().BeSameAs(editor.EditValue(mockContext.Object, mockServiceProvider.Object, value));
    }

    [Fact]
    public void EditValue_ReturnsOriginalValue_WhenContextInstanceIsNotMaskedTextBox()
    {
        // Set context.Instance to new object.
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.Instance)
            .Returns(new object());

        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
              .Setup(s => s.DropDownControl(It.IsAny<MaskedTextBoxTextEditorDropDown>()));

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        MaskedTextBoxTextEditor editor = new();
        string originalValue = "TestValue";
        object? actualValue = editor.EditValue(mockContext.Object, mockServiceProvider.Object, originalValue);

        // When Context.Instance is not MaskedTextBox, its Text property should be the original value,
        // and the return value of EditValue is also the original value.
        mockContext.Object.Instance.Should().NotBeOfType<MaskedTextBox>();
        MaskedTextBox? maskedText = mockContext.Object.Instance as MaskedTextBox;

        maskedText?.Text.Should().Be(originalValue);
        actualValue.Should().Be(originalValue);
    }

    [Fact]
    public void EditValue_ReturnsDropDownValue_WhenContextInstanceIsMaskedTextBox_AndDropDownValueIsNotNull()
    {
        MaskedTextBox CustomMaskedTextBox = new()
        {
            Text = "MaskText"
        };

        // Set context.Instance to a new MaskedTextBox with Text.
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.Instance)
            .Returns(CustomMaskedTextBox);

        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
              .Setup(s => s.DropDownControl(It.IsAny<MaskedTextBoxTextEditorDropDown>()));

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        MaskedTextBoxTextEditor editor = new();
        string originalValue = "TestValue";
        object? actualValue = editor.EditValue(mockContext.Object, mockServiceProvider.Object, originalValue);

        // When Context.Instance is MaskedTextBox, its own Text property of control itself is not changed,
        // and the return value of EditValue is equal to the control's own Text value.
        Assert.True(mockContext.Object.Instance is MaskedTextBox);
        MaskedTextBox? maskedTextBox = mockContext.Object.Instance as MaskedTextBox;

        maskedTextBox?.Text.Should().NotBe(originalValue);
        actualValue.Should().Be(CustomMaskedTextBox.Text);
    }

    [Fact]
    public void EditValue_ReturnsOriginalValue_WhenContextInstanceIsMaskedTextBox_AndDropDownValueIsNull()
    {
        MaskedTextBox CustomMaskedTextBox = new();

        // Set context.Instance to a new MaskedTextBox without Text value.
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.Instance)
            .Returns(CustomMaskedTextBox);

        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        mockEditorService
              .Setup(s => s.DropDownControl(It.IsAny<MaskedTextBoxTextEditorDropDown>()));

        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object);

        MaskedTextBoxTextEditor editor = new();
        string originalValue = "TestValue";
        object? actualValue = editor.EditValue(mockContext.Object, mockServiceProvider.Object, originalValue);

        // When Context.Instance is MaskedTextBox, its own Text property of control itself will not change,
        // and the return value of EditValue is also original value.
        Assert.True(mockContext.Object.Instance is MaskedTextBox);
        MaskedTextBox? maskedTextBox = mockContext.Object.Instance as MaskedTextBox;

        maskedTextBox?.Text.Should().NotBe(originalValue);
        actualValue.Should().BeSameAs(CustomMaskedTextBox.Text);
    }

    [Theory]
    [InlineData([new object[] { }])]
    [InlineData(null)]
    public void GetEditStyle_ReturnExpected(object? host)
    {
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.Instance)
            .Returns(host);

        MaskedTextBoxTextEditor editor = new();
        UITypeEditorEditStyle result = editor.GetEditStyle(mockContext.Object);

        if (host is null)
        {
            // Returns base.GetEditStyle when Context.Instance is null.
            result.Should().Be(editor.GetEditStyle(mockContext.Object));
        }
        else
        {
            // Returns DropDown when Context.Instance is not null.
            result.Should().Be(UITypeEditorEditStyle.DropDown);
        }
    }

    [Theory]
    [InlineData([new object[] { }])]
    [InlineData(null)]
    public void GetPaintValueSupported_ReturnExpected(object? host)
    {
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        mockContext
            .Setup(c => c.Instance)
            .Returns(host);

        MaskedTextBoxTextEditor editor = new();
        bool result = editor.GetPaintValueSupported(mockContext.Object);

        if (host is null)
        {
            // Returns base.GetPaintValueSupported when Context.Instance is null.
            result.Should().Be(editor.GetPaintValueSupported(mockContext.Object));
        }
        else
        {
            // Returns false when Context.Instance is not null.
            result.Should().BeFalse();
        }
    }

    [Fact]
    public void IsDropDownResizable_ReturnsFalse()
    {
        MaskedTextBoxTextEditor editor = new();
        editor.IsDropDownResizable.Should().BeFalse();
    }
}
