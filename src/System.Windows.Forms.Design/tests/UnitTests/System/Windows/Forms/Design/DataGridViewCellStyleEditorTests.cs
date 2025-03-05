// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing.Design;

namespace System.Windows.Forms.Design.Tests;

public class DataGridViewCellStyleEditorTests
{
    [Fact]
    public void DataGridViewCellStyleEditor_Ctor_Default()
    {
        DataGridViewCellStyleEditor editor = new();
        Assert.False(editor.IsDropDownResizable);
    }

    [Fact]
    public void DataGridViewCellStyleEditor_GetEditStyle_ReturnsModal()
    {
        DataGridViewCellStyleEditor editor = new();
        Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(null));
    }
}
