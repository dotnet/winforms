// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Design.Tests;

public class FileNameEditorTests
{
    [Fact]
    public void FileNameEditor_Ctor_Default()
    {
        FileNameEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetEditValueInvalidProviderTestData))]
    public void FileNameEditor_EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider, object value)
    {
        FileNameEditor editor = new();
        Assert.Same(value, editor.EditValue(null, provider, value));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void FileNameEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        FileNameEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void FileNameEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
    {
        FileNameEditor editor = new();
        Assert.False(editor.GetPaintValueSupported(context));
    }

    [Fact]
    public void FileNameEditor_InitializeDialog_Invoke_Success()
    {
        SubFileNameEditor editor = new();
        using OpenFileDialog openFileDialog = new();
        editor.InitializeDialog(openFileDialog);
        Assert.Equal("All Files(*.*)|*.*", openFileDialog.Filter);
        Assert.Equal("Open File", openFileDialog.Title);
    }

    [Fact]
    public void FileNameEditor_InitializeDialog_NullOpenFileDialog_ThrowsArgumentNullException()
    {
        SubFileNameEditor editor = new();
        Assert.Throws<ArgumentNullException>("openFileDialog", () => editor.InitializeDialog(null));
    }

    private class SubFileNameEditor : FileNameEditor
    {
        public new void InitializeDialog(OpenFileDialog openFileDialog) => base.InitializeDialog(openFileDialog);
    }
}
