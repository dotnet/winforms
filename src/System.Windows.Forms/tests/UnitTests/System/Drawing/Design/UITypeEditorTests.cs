// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;
using System.Windows.Forms.TestUtilities;

namespace System.Drawing.Design.Tests;

// NB: doesn't require thread affinity
public class UITypeEditorTests
{
    [Fact]
    public void UITypeEditor_Ctor_Default()
    {
        UITypeEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    public static IEnumerable<object[]> EditValue_ITypeDescriptorContext_IServiceProvider_Object_TestData()
    {
        yield return new object[] { null, null, null };
        Mock<ITypeDescriptorContext> mockTypeDescriptorContext = new(MockBehavior.Strict);
        Mock<ITypeDescriptorContext> mockServiceProvider = new(MockBehavior.Strict);
        yield return new object[] { mockTypeDescriptorContext.Object, mockServiceProvider.Object, new() };
    }

    [Theory]
    [MemberData(nameof(EditValue_ITypeDescriptorContext_IServiceProvider_Object_TestData))]
    public void UITypeEditor_EditValue_Invoke_ReturnsValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
        UITypeEditor editor = new();
        Assert.Same(value, editor.EditValue(context, provider, value));
    }

    public static IEnumerable<object[]> EditValue_IServiceProvider_Object_TestData()
    {
        yield return new object[] { null, null };
        Mock<ITypeDescriptorContext> mockServiceProvider = new(MockBehavior.Strict);
        yield return new object[] { mockServiceProvider.Object, new() };
    }

    [Theory]
    [MemberData(nameof(EditValue_IServiceProvider_Object_TestData))]
    public void UITypeEditor_EditValue_Invoke_CallsVirtualEditValue(IServiceProvider provider, object value)
    {
        object result = new();
        Mock<UITypeEditor> mockEditor = new(MockBehavior.Strict);
        mockEditor
            .Setup(e => e.EditValue(null, provider, value))
            .Returns(result)
            .Verifiable();
        Assert.Same(result, mockEditor.Object.EditValue(provider, value));
        mockEditor.Verify(e => e.EditValue(null, provider, value), Times.Once());
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void UITypeEditor_GetEditStyle_Invoke_ReturnsNone(ITypeDescriptorContext context)
    {
        UITypeEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.None, editor.GetEditStyle(context));
    }

    [Fact]
    public void UITypeEditor_GetEditStyle_Invoke_CallsVirtualGetEditStyle()
    {
        Mock<UITypeEditor> mockEditor = new(MockBehavior.Strict);
        mockEditor
            .Setup(e => e.GetEditStyle(null))
            .Returns(UITypeEditorEditStyle.Modal)
            .Verifiable();
        Assert.Equal(UITypeEditorEditStyle.Modal, mockEditor.Object.GetEditStyle());
        mockEditor.Verify(e => e.GetEditStyle(null), Times.Once());
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void UITypeEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
    {
        UITypeEditor editor = new();
        Assert.False(editor.GetPaintValueSupported(context));
    }

    [Theory]
    [BoolData]
    public void UITypeEditor_GetPaintValueSupported_Invoke_CallsVirtualGetPaintValueSupported(bool result)
    {
        Mock<UITypeEditor> mockEditor = new(MockBehavior.Strict);
        mockEditor
            .Setup(e => e.GetPaintValueSupported(null))
            .Returns(result)
            .Verifiable();
        Assert.Equal(result, mockEditor.Object.GetPaintValueSupported());
        mockEditor.Verify(e => e.GetPaintValueSupported(null), Times.Once());
    }

    public static IEnumerable<object[]> PaintValue_PaintValueEventArgs_TestData()
    {
        Bitmap bitmap = new(10, 10);
        Graphics graphics = Graphics.FromImage(bitmap);
        yield return new object[] { null };
        yield return new object[] { new PaintValueEventArgs(null, null, graphics, Rectangle.Empty) };
    }

    [Theory]
    [MemberData(nameof(PaintValue_PaintValueEventArgs_TestData))]
    public void UITypeEditor_PaintValue_Invoke_Nop(PaintValueEventArgs e)
    {
        using Bitmap image = new(10, 10);
        using var graphics = Graphics.FromImage(image);
        UITypeEditor editor = new();
        editor.PaintValue(e);
    }

    public static IEnumerable<object[]> PaintValue_Object_Graphics_Rectangle_TestData()
    {
        Bitmap bitmap = new(10, 10);
        Graphics graphics = Graphics.FromImage(bitmap);
        yield return new object[] { null, graphics, Rectangle.Empty };
        yield return new object[] { new(), graphics, new Rectangle(1, 2, 3, 4) };
    }

    [Theory]
    [MemberData(nameof(PaintValue_Object_Graphics_Rectangle_TestData))]
    public void UITypeEditor_PaintValue_Invoke_CallsVirtualPaintValue(object value, Graphics canvas, Rectangle rectangle)
    {
        Mock<UITypeEditor> mockEditor = new(MockBehavior.Strict);
        mockEditor
            .Setup(e => e.PaintValue(It.IsAny<PaintValueEventArgs>()))
            .Verifiable();
        using (Bitmap image = new(10, 10))
        using (var graphics = Graphics.FromImage(image))
        {
            mockEditor.Object.PaintValue(value, canvas, rectangle);
        }

        mockEditor.Verify(e => e.PaintValue(It.IsAny<PaintValueEventArgs>()), Times.Once());
    }

    [Fact]
    public void UITypeEditor_PaintValue_NullCanvas_ThrowsArgumentNullException()
    {
        UITypeEditor editor = new();
        Assert.Throws<ArgumentNullException>("graphics", () => editor.PaintValue(new object(), null, Rectangle.Empty));
    }
}
