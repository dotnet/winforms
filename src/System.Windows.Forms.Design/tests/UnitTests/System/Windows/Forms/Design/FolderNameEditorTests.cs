// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
        editor.InitializeDialog();
    }

    public class FolderBrowserTests : FolderNameEditor
    {
        [Fact]
        public void FolderBrowser_Ctor_Default()
        {
            FolderBrowser browser = new();
            Assert.Empty(browser.DirectoryPath);
            Assert.Empty(browser.Description);
            Assert.Equal(FolderBrowserStyles.RestrictToFilesystem, browser.Style);
            Assert.Equal(FolderBrowserFolder.Desktop, browser.StartLocation);
        }

        [Theory]
        [NormalizedStringData]
        public void FolderBrowser_Description_Set_GetReturnsExpected(string value, string expected)
        {
            FolderBrowser browser = new()
            {
                Description = value
            };
            Assert.Equal(expected, browser.Description);

            // Set same.
            browser.Description = value;
            Assert.Equal(expected, browser.Description);
        }

        [Theory]
        [EnumData<FolderBrowserFolder>]
        [InvalidEnumData<FolderBrowserFolder>]
        protected void FolderBrowser_StartLocation_Set_GetReturnsExpected(FolderBrowserFolder value)
        {
            FolderBrowser browser = new()
            {
                StartLocation = value
            };
            Assert.Equal(value, browser.StartLocation);

            // Set same.
            browser.StartLocation = value;
            Assert.Equal(value, browser.StartLocation);
        }

        [Theory]
        [EnumData<FolderBrowserStyles>]
        [InvalidEnumData<FolderBrowserStyles>]
        protected void FolderBrowser_Style_Set_GetReturnsExpected(FolderBrowserStyles value)
        {
            FolderBrowser browser = new()
            {
                Style = value
            };
            Assert.Equal(value, browser.Style);

            // Set same.
            browser.Style = value;
            Assert.Equal(value, browser.Style);
        }
    }

    private class SubFolderNameEditor : FolderNameEditor
    {
        public void InitializeDialog() => base.InitializeDialog(null);
    }
}
