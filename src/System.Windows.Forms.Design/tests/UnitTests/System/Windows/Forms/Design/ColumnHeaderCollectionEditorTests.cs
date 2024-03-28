// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public class ColumnHeaderCollectionEditorTests
{
    [Fact]
    public void ColumnHeaderCollectionEditor_Ctor_Default()
    {
        ColumnHeaderCollectionEditor editor = new(typeof(string));
        Assert.False(editor.IsDropDownResizable);
    }

    [Fact]
    public void ColumnHeaderCollectionEditor_EditValue_ReturnsValue()
    {
        ColumnHeaderCollectionEditor editor = new(typeof(string));
        string[] value = ["asdf", "qwer", "zxcv"];

        Assert.Same(value, editor.EditValue(null, value));
    }
}
