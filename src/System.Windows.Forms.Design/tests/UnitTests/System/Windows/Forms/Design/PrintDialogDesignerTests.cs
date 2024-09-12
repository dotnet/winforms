// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class PrintDialogDesignerTests
{
    [Fact]
    public void InitializeNewComponent_SetsUseEXDialogToTrue()
    {
        using PrintDialogDesigner designer = new();
        using PrintDialog printDialog = new();
        designer.Initialize(printDialog);

        designer.InitializeNewComponent(null);

        printDialog.UseEXDialog.Should().BeTrue();
    }
}
