// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Windows.Forms.TestUtilities;

namespace System.Drawing.Design.Tests;

public class FontEditorTests
{
    [Fact]
    public void FontEditor_Ctor_Default()
    {
        FontEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetEditValueInvalidProviderTestData))]
    public void FontEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
    {
        FontEditor editor = new();
        Assert.Same(value, editor.EditValue(null, provider, value));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void FontEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        FontEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void FontEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
    {
        FontEditor editor = new();
        Assert.False(editor.GetPaintValueSupported(context));
    }
}
