// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Windows.Forms.Design;
using Moq;
using System.Windows.Forms.TestUtilities;

namespace System.Drawing.Design.Tests;

public class FontNameEditorTests
{
    private readonly ITypeDescriptorContext _typeDescriptorContext;

    public FontNameEditorTests()
    {
        _typeDescriptorContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict).Object;
    }

    [Fact]
    public void FontNameEditor_Ctor_Default()
    {
        FontNameEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    public static IEnumerable<object[]> EditValue_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { "value" };
        yield return new object[] { new() };
    }

    [Theory]
    [MemberData(nameof(EditValue_TestData))]
    public void FontNameEditor_EditValue_ValidProvider_ReturnsValue(object value)
    {
        FontNameEditor editor = new();
        Mock<IWindowsFormsEditorService> mockEditorService = new(MockBehavior.Strict);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
            .Returns(mockEditorService.Object)
            .Verifiable();
        Assert.Same(value, editor.EditValue(null, mockServiceProvider.Object, value));
        mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Never());

        // Edit again.
        Assert.Same(value, editor.EditValue(null, mockServiceProvider.Object, value));
        mockServiceProvider.Verify(p => p.GetService(typeof(IWindowsFormsEditorService)), Times.Never());
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetEditValueInvalidProviderTestData))]
    public void FontNameEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
    {
        FontNameEditor editor = new();
        Assert.Same(value, editor.EditValue(null, provider, value));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void FontNameEditor_GetEditStyle_Invoke_ReturnsNone(ITypeDescriptorContext context)
    {
        FontNameEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.None, editor.GetEditStyle(context));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void FontNameEditor_GetPaintValueSupported_Invoke_ReturnsTrue(ITypeDescriptorContext context)
    {
        FontNameEditor editor = new();
        Assert.True(editor.GetPaintValueSupported(context));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\t")]
    public void FontNameEditor_PaintValue_ReturnsEarly_InvalidPaintValueEventArgsValue(string fontName)
    {
        PaintValueEventArgs e;
        using (Bitmap bitmap = new(1, 1))
        {
            using var g = Graphics.FromImage(bitmap);
            e = new PaintValueEventArgs(_typeDescriptorContext, fontName, g, Rectangle.Empty);
        }

        // assert by the virtue of calling the method
        // if the implementation is incorrect, having disposed of the Graphics object
        // we would received an AE attempting to call e.Graphics.FillRectangle()
        FontNameEditor editor = new();
        editor.PaintValue(e);
    }
}
