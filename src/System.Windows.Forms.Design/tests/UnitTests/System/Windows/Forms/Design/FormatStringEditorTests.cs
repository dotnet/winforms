// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.TestUtilities;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class FormatStringEditorTests
{
    private readonly FormatStringEditor _editor = new();
    private readonly Mock<IWindowsFormsEditorService> _mockEditorService;
    private readonly Mock<IComponentChangeService> _mockChangeService;
    private readonly Mock<IServiceProvider> _provider;
    private readonly Mock<ITypeDescriptorContext> _context;
    private readonly DataGridViewCellStyle _cellStyle;

    public FormatStringEditorTests()
    {
        _mockEditorService = new();
        _mockChangeService = new();
        _cellStyle = new() { Format = "Initial" };

        _provider = new(MockBehavior.Strict);
        _provider.Setup(p => p.GetService(typeof(IWindowsFormsEditorService))).Returns(_mockEditorService.Object);
        _provider.Setup(p => p.GetService(typeof(IComponentChangeService))).Returns(_mockChangeService.Object);

        _context = new(MockBehavior.Strict);
        _context.Setup(c => c.Instance).Returns(_cellStyle);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetEditValueInvalidProviderTestData))]
    public void EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
    {
        _editor.EditValue(null, provider, value).Should().Be(value);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        _editor.GetEditStyle(context).Should().Be(UITypeEditorEditStyle.Modal);
    }

    [WinFormsFact]
    public void EditValue_ShowsDialogAndUpdatesValue()
    {
        object? result = _editor.EditValue(_context.Object, _provider.Object, "NewValue");

        result.Should().Be("NewValue");
        _mockEditorService.Verify(es => es.ShowDialog(It.IsAny<FormatStringDialog>()), Times.Once);
    }

    [WinFormsFact]
    public void EditValue_CallsOnComponentChanging_Once()
    {
        _editor.EditValue(_context.Object, _provider.Object, "NewValue");

        _mockChangeService.Verify(
            cs => cs.OnComponentChanging(_cellStyle, TypeDescriptor.GetProperties(_cellStyle)["Format"]),
            Times.Once);

        _mockChangeService.Verify(
            cs => cs.OnComponentChanging(_cellStyle, TypeDescriptor.GetProperties(_cellStyle)["NullValue"]),
            Times.Once);

        _mockChangeService.Verify(
            cs => cs.OnComponentChanging(_cellStyle, TypeDescriptor.GetProperties(_cellStyle)["FormatProvider"]),
            Times.Once);
    }

    [WinFormsFact]
    public void EditValue_WithDirtyDialog_CallsOnComponentChanged_Once()
    {
        using FormatStringDialog dialog = new(_context.Object);

        _editor.TestAccessor().Dynamic._formatStringDialog = dialog;
        dialog.TestAccessor().Dynamic._dirty = true;

        _editor.EditValue(_context.Object, _provider.Object, "NewValue");

        _mockChangeService.Verify(
            cs => cs.OnComponentChanged(_cellStyle, TypeDescriptor.GetProperties(_cellStyle)["Format"], null, null),
            Times.Once);

        _mockChangeService.Verify(
            cs => cs.OnComponentChanged(_cellStyle, TypeDescriptor.GetProperties(_cellStyle)["NullValue"], null, null),
            Times.Once);

        _mockChangeService.Verify(
            cs => cs.OnComponentChanged(_cellStyle, TypeDescriptor.GetProperties(_cellStyle)["FormatProvider"], null, null),
            Times.Once);
    }
}
