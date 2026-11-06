// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Design.Tests;

public class FolderNameEditorTests
{
    [Fact]
    public void FolderNameEditor_Ctor_Default()
    {
        FileNameEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void FolderNameEditor_GetEditStyle_Invoke_ReturnsModal(ITypeDescriptorContext context)
    {
        FolderNameEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
    }

    [Theory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetITypeDescriptorContextTestData))]
    public void FolderNameEditor_GetPaintValueSupported_Invoke_ReturnsFalse(ITypeDescriptorContext context)
    {
        FolderNameEditor editor = new();
        Assert.False(editor.GetPaintValueSupported(context));
    }

    [Fact]
    public void FolderNameEditor_InitializeDialog_Invoke_Nop()
    {
        SubFolderNameEditor editor = new();
        using FolderBrowserDialog dialog = new();

        // The base implementation is intentionally a no-op; invoking it should not
        // throw and should leave the dialog's defaults untouched.
        string originalSelectedPath = dialog.SelectedPath;
        string originalDescription = dialog.Description;
        Environment.SpecialFolder originalRootFolder = dialog.RootFolder;

        editor.InitializeDialog(dialog);

        Assert.Equal(originalSelectedPath, dialog.SelectedPath);
        Assert.Equal(originalDescription, dialog.Description);
        Assert.Equal(originalRootFolder, dialog.RootFolder);
    }

    private class SubFolderNameEditor : FolderNameEditor
    {
        public new void InitializeDialog(FolderBrowserDialog folderBrowserDialog) =>
            base.InitializeDialog(folderBrowserDialog);
    }
}
