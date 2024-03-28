// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Design;
using System.Windows.Forms.Design;
using Moq;
using System.Windows.Forms.TestUtilities;

namespace System.ComponentModel.Design.Tests;

public class ObjectSelectorEditorTests
{
    [Fact]
    public void ObjectSelectorEditor_Ctor_Default()
    {
        SubObjectSelectorEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
        Assert.False(editor.SubObjectSelector);
    }

    [Theory]
    [BoolData]
    public void ObjectSelectorEditor_Ctor_Bool(bool subObjectSelector)
    {
        SubObjectSelectorEditor editor = new(subObjectSelector);
        Assert.False(editor.IsDropDownResizable);
        Assert.Equal(subObjectSelector, editor.SubObjectSelector);
    }

    public static IEnumerable<object[]> EditValue_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { "value" };
        yield return new object[] { new() };
    }

    [Theory]
    [MemberData(nameof(EditValue_TestData))]
    public void ObjectSelectorEditor_EditValue_ValidProvider_ReturnsValue(object value)
    {
        SubObjectSelectorEditor editor = new();
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object)
            .Verifiable();
        mockEditorService
            .Setup(e => e.DropDownControl(It.IsAny<Control>()))
            .Verifiable();
        Assert.Same(value, editor.EditValue(null, mockServiceProvider.Object, value));
        mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Once());
        mockEditorService.Verify(e => e.DropDownControl(It.IsAny<Control>()), Times.Once());

        // Edit again.
        Assert.Same(value, editor.EditValue(null, mockServiceProvider.Object, value));
        mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Exactly(2));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetEditValueInvalidProviderTestData))]
    public void ObjectSelectorEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
    {
        SubObjectSelectorEditor editor = new();
        Assert.Same(value, editor.EditValue(null, provider, value));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void ObjectSelectorEditor_GetEditStyle_Invoke_ReturnsDropDown(ITypeDescriptorContext context)
    {
        SubObjectSelectorEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.DropDown, editor.GetEditStyle(context));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void ObjectSelectorEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
    {
        SubObjectSelectorEditor editor = new();
        Assert.False(editor.GetPaintValueSupported(context));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("value", false)]
    public void ObjectSelectorEditor_EqualsToValue_InvokeWithoutValue_ReturnsExpected(object value, bool expected)
    {
        SubObjectSelectorEditor editor = new();
        Assert.Equal(expected, editor.EqualsToValue(value));
    }

    [Fact]
    public void ObjectSelectorEditor_EqualsToValue_InvokeWithValue_ReturnsExpected()
    {
        SubObjectSelectorEditor editor = new();
        editor.SetValue("value");
        Assert.True(editor.EqualsToValue("value"));
        Assert.False(editor.EqualsToValue("other value"));
        Assert.False(editor.EqualsToValue(null));
    }

    [Fact]
    public void ObjectSelectorEditor_Selector()
    {
        ObjectSelectorEditor.Selector underTest = new(new SubObjectSelectorEditor());

        Assert.NotNull(underTest);
        underTest.AddNode("node", "value", null);
        Assert.Single(underTest.Nodes);
        Assert.True(underTest.SetSelection("value", null));
        Assert.False(underTest.SetSelection("other value", null));
        underTest.Clear();
        Assert.Empty(underTest.Nodes);
    }

    private class SubObjectSelectorEditor : ObjectSelectorEditor
    {
        public SubObjectSelectorEditor()
        {
        }

        public SubObjectSelectorEditor(bool subObjectSelector) : base(subObjectSelector)
        {
        }
    }
}
