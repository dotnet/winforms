// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Editors.Tests;

public class BinaryEditorTests
{
    [WinFormsFact]
    public void BinaryEditor_EditValue()
    {
        // Ensure that we can instantiate the modal editor.

        BinaryEditor editor = new();
        var editorService = new Mock<IWindowsFormsEditorService>();
        editorService.Setup(e => e.ShowDialog(It.IsAny<Form>()))
            .Callback<Form>(f => { f.Show(); f.Close(); })
            .Returns(DialogResult.OK);
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(s => s.GetService(typeof(IWindowsFormsEditorService))).Returns(editorService.Object);

        var result = editor.EditValue(serviceProvider.Object, new byte[10]);
    }
}
