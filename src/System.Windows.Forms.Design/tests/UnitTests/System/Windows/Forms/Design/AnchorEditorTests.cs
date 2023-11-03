// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms.TestUtilities;
using Moq;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Design.Tests;

public class AnchorEditorTests
{
    [Fact]
    public void AnchorEditor_Ctor_Default()
    {
        AnchorEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    public static IEnumerable<object[]> EditValue_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { "value" };
        yield return new object[] { AnchorStyles.Top };
        yield return new object[] { new() };
    }

    [Theory]
    [MemberData(nameof(EditValue_TestData))]
    public void AnchorEditor_EditValue_ValidProvider_ReturnsValue(object value)
    {
        AnchorEditor editor = new();
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
    public void AnchorEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
    {
        AnchorEditor editor = new();
        Assert.Same(value, editor.EditValue(null, provider, value));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void AnchorEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        AnchorEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.DropDown, editor.GetEditStyle(context));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void AnchorEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
    {
        AnchorEditor editor = new();
        Assert.False(editor.GetPaintValueSupported(context));
    }

    [Theory]
    [InlineData("_left")]
    [InlineData("_right")]
    [InlineData("_top")]
    [InlineData("_bottom")]
    public void AnchorEditor_AnchorUI_ControlType_IsCheckButton(string fieldName)
    {
        Type type = typeof(AnchorEditor)
            .GetNestedType("AnchorUI", BindingFlags.NonPublic | BindingFlags.Instance);
        var anchorUI = (Control)Activator.CreateInstance(type);
        var item = (Control)anchorUI.GetType()
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(anchorUI);

        var actual = (UIA_CONTROLTYPE_ID)(int)item.AccessibilityObject.TestAccessor().Dynamic
            .GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_CheckBoxControlTypeId, actual);
    }
}
