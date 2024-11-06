// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.Design;
using FluentAssertions;
using Moq;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class FolderNameEditorTests : ControlTestBase
{
    public FolderNameEditorTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public void FolderNameEditor_EditValue_ReturnsExpected()
    {
        TestFolderNameEditor editor = new();
        Mock<IServiceProvider> serviceProviderMock = new();
        var serviceProvider = serviceProviderMock.Object;
        string value = "value";

        object? result = editor.EditValue(context: null, provider: serviceProvider, value: value);

        result.Should().Be(value);
    }

    private class TestFolderNameEditor : FolderNameEditor
    {
        private FolderBrowser? _folderBrowser;

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            using DialogHostForm dialogOwnerForm = new();

            if (_folderBrowser is null)
            {
                _folderBrowser = new FolderBrowser();
                InitializeDialog(_folderBrowser);
            }

            if (_folderBrowser.ShowDialog(dialogOwnerForm) == DialogResult.OK)
            {
                return _folderBrowser.DirectoryPath;
            }

            return value;
        }
    }
}
