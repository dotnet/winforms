// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using Moq;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Design.Tests;

public class DockEditorTests
{
    [Fact]
    public void DockEditor_Ctor_Default()
    {
        DockEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    public static IEnumerable<object[]> EditValue_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { "value" };
        yield return new object[] { DockStyle.Top };
        yield return new object[] { new() };
    }

    [Theory]
    [MemberData(nameof(EditValue_TestData))]
    public void DockEditor_EditValue_ValidProvider_ReturnsValue(object value)
    {
        DockEditor editor = new();
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object)
            .Verifiable();
        mockEditorService
            .Setup(e => e.DropDownControl(It.IsAny<Control>()))
            .Verifiable();
        Assert.Equal(value, editor.EditValue(null, mockServiceProvider.Object, value));
        mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Once());
        mockEditorService.Verify(e => e.DropDownControl(It.IsAny<Control>()), Times.Once());

        // Edit again.
        Assert.Equal(value, editor.EditValue(null, mockServiceProvider.Object, value));
        mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Exactly(2));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetEditValueInvalidProviderTestData))]
    public void DockEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
    {
        DockEditor editor = new();
        Assert.Same(value, editor.EditValue(null, provider, value));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void DockEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        DockEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.DropDown, editor.GetEditStyle(context));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void DockEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
    {
        DockEditor editor = new();
        Assert.False(editor.GetPaintValueSupported(context));
    }
}
